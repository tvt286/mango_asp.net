using Mango.Areas.Admin.Models;
using Mango.Data;
using Mango.Data.Enums;
using Mango.Security;
using Mango.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Mango.Common;
namespace Mango.Areas.Admin.Controllers
{
    public class GroupPermissionController : Controller
    {
        private mangoEntities db = new mangoEntities();
        // GET: GroupPermission
        [AuthorizeAdmin(Permissions = new[] { Permission.GroupPermission_Create, Permission.GroupPermission_View })]
        public ActionResult Index(GroupPermissionSearchModel searchModel)
        {
            if (Request.HttpMethod == "GET")
            {
                var user = UserService.GetUserInfo();
                if (!(UserPermission.Has(Permission.GroupPermission_View)))
                {
                    return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Index") });
                }

                return View(searchModel);
            }
            var pagedList = GroupPermissionService.Search(searchModel.Name, searchModel.PageSize, searchModel.PageIndex);
            pagedList.SearchModel = searchModel;
            return PartialView("_List", pagedList);
        }


        public ActionResult Create()
        {
            var user = UserService.GetUserInfo();
            ViewBag.User = user;
            var data = new Group();
            ViewBag.GroupId = UserService.GetGroups().Select(x => new SelectListItem() { Text = x.Name, Value = x.GroupId.ToString() });
            return View(data);
        }

        // POST: /Groups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[AuthorizeAdmin(Permission = Permission.GroupPermission_Create)]
        public ActionResult Create([Bind(Include = "GroupId,Name,Description,CreateDate,CompanyId")] Group group, int[] selectedGroupPermission, int[] groupId)
        {
            var user = UserService.GetUserInfo();

            if (ModelState.IsValid)
            {
                group.CreateDate = DateTime.Now;
                db.Groups.Add(group);
                db.SaveChanges();

                var permissionId = new HashSet<int>();
                if (groupId != null)
                {
                    foreach (var item in groupId)
                    {
                        var permission = db.Group_Permission.Where(x => x.GroupId == item).ToList();
                        if (permission.Count > 0)
                        {
                            foreach (var groupPermission in permission)
                            {
                                permissionId.Add(groupPermission.PermissionId);
                            }
                        }
                    }
                }


                foreach (var i in permissionId)
                {
                    db.Group_Permission.Add(new Group_Permission()
                    {
                        GroupId = group.GroupId,
                        PermissionId = i,
                    });
                }
                db.SaveChanges();
                TempData["Message"] = "Thêm mới thành công .";          
                return View("Index");
            }

            return View("Index");
        }

        [AuthorizeAdmin(Permissions = new[] {Permission.GroupPermission_Create, Permission.GroupPermission_View})]
        public ActionResult Edit(int? id)
        {
            var user = UserService.GetUserInfo();

            ViewBag.User = user;
            Group model = new Group()
            {
                Group_Permission = new List<Group_Permission>()
            };

            if (id.HasValue)
            {
                //var Id = int.Parse(SecurityHelper.Decrypt(id));
                model = db.Groups.Include(x => x.Group_Permission).First(x => x.GroupId == id);
            }

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "GroupId,Name,Description,CompanyId", Exclude = "CreateDate")] Group group, string selectedPermissions)
        {
            var user = UserService.GetUserInfo();
            ViewBag.User = user;
      
            int[] selectedGroupPermission = string.IsNullOrWhiteSpace(selectedPermissions) ? new int[0] : selectedPermissions.Split(',').Select(int.Parse).ToArray();
            ModelState.Remove("CreateDate");
            if (ModelState.IsValid)
            {
                if (group.GroupId == 0)
                {
                    group.CreateDate = DateTime.Now;
                    db.Groups.Add(group);
                    db.SaveChanges();


                    foreach (var i in selectedGroupPermission)
                    {
                        db.Group_Permission.Add(new Group_Permission()
                        {
                            GroupId = group.GroupId,
                            PermissionId = i,
                        });
                    }
                    db.SaveChanges();
                    TempData["Message"] = "Thêm mới thành công .";

                    return View("Index");
                }
                var data = GroupPermissionService.Get(group.GroupId);

                db.Groups.Attach(group);
                db.Entry(group).State = EntityState.Modified;
                db.Entry(group).Property(x => x.CreateDate).IsModified = false;

                Group _group = db.Groups.Include(x => x.Group_Permission).FirstOrDefault(x => x.GroupId == group.GroupId);
                if (_group == null)
                {
                    return HttpNotFound();
                }
                var deleted = _group.Group_Permission.Where(x => selectedGroupPermission.Contains(x.PermissionId) == false);

                foreach (var groupPermission in deleted.ToList())
                {
                    db.Group_Permission.Remove(groupPermission);
                    ViewBag.message = "Cập nhật quyền thành công.";
                }
                var addNew = selectedGroupPermission.Where(x => _group.Group_Permission.All(y => y.PermissionId != x));

                foreach (var permissionId in addNew)
                {
                    var permission = new Group_Permission();
                    permission.GroupId = group.GroupId;
                    permission.PermissionId = permissionId;
                    db.Group_Permission.Add(permission);
                    ViewBag.message = "Cập nhật quyền thành công.";
                }
                db.SaveChanges();
                return View(_group);
            }
            return View(group);
        }
    }
}
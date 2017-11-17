using Mango.Areas.Admin.Models;
using Mango.Common;
using Mango.Data;
using Mango.Data.Enums;
using Mango.Security;
using Mango.Services;
using Mango.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mango.Areas.Admin.Controllers
{
    public class MenuController : Controller
    {

        // GET: Category
        [AuthorizeAdmin(Permissions = new[] { Permission.Category_View, Permission.Category_Create })]
        public ActionResult Index(MenuSearchModel searchModel)
        {
            if (Request.HttpMethod == "GET")
            {
                return View(searchModel);
            }
            var pagedList = MenuService.Search(searchModel.Name, searchModel.PageSize, searchModel.PageIndex);
            pagedList.SearchModel = searchModel;
            return PartialView("_List", pagedList);
        }



        [AuthorizeAdmin(Permissions = new[] { Permission.Category_View, Permission.Category_Create })]
        public ActionResult Detail(int? id)
        {
            var user = UserService.GetUserInfo();
            ViewBag.User = user;
            var data = new Menu();

            if (id.HasValue)
            {
                data = MenuService.Get(id.Value);
            }

            return PartialView("_Detail", data);
        }

        [AuthorizeAdmin(Permission = Permission.Category_Create)]
        public ActionResult Create(Menu model, HttpPostedFileBase fileAttach)
        {
            RedirectCommand result;
            var sourceFile = "";
            if (fileAttach != null)
            {
                if (!Directory.Exists(Server.MapPath("~/content/Upload/Menu")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/content/Upload/Menu"));
                }

                var fileName = Utility.ConvertToUnsign(Path.GetFileNameWithoutExtension(fileAttach.FileName))
                        .Replace("&", "")
                        .Replace("?", "")
                        .Replace(" ", "-") + "-" + Guid.NewGuid() + Path.GetExtension(fileAttach.FileName);
                sourceFile = string.Format("~/content/Upload/Menu/{0}", fileName);
                var pathFile = Server.MapPath(sourceFile);
                var checkFile = UploadHelper.CheckImageUpload(fileAttach);
                if (checkFile == UploadFileStatus.NotSupportExtension)
                {
                    return Json(new RedirectCommand
                    {
                        Code = ResultCode.Fail,
                        Message = "Only upload file image!"
                    }, JsonRequestBehavior.AllowGet);
                }
                if (checkFile == UploadFileStatus.OverLimited)
                {
                    return Json(new RedirectCommand
                    {
                        Code = ResultCode.Fail,
                        Message = "Only upload file image < 5MB!"
                    }, JsonRequestBehavior.AllowGet);
                }
                fileAttach.SaveAs(pathFile);
                //  Bitmap bitmap = (Bitmap)Bitmap.FromStream(fileAttach.InputStream, true);
                // ImageUtils.RewriteImageFix(bitmap, 500, 500, pathFile);
            }

            if (model.Id == 0)
            {
                model.Image = sourceFile;
                result = MenuService.Create(model);
                if (result.Code == ResultCode.Success)
                {
                    result.Url = Url.Action("Index");
                }
                return
                    Json(result, JsonRequestBehavior.AllowGet);
            }

            var data = MenuService.Get(model.Id);
            if (sourceFile.NotEmpty())
            {
                if (data.Image.NotEmpty())
                {
                    try
                    {
                        System.IO.File.Delete(Server.MapPath(data.Image));
                    }
                    catch (Exception ex)
                    {
                    }

                }
                data.Image = sourceFile;
            }
            TryUpdateModel(data);
            result = MenuService.Update(data);

            if (result.Code == ResultCode.Success)
            {
                result.Url = Url.Action("Index");
            }
            return
                    Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(string menuId)
        {
            var result = MenuService.DeleteMenu(menuId);
            if (result.Code == ResultCode.Success)
            {
                result.Url = Url.Action("Index");
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
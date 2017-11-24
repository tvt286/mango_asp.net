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
    public class CategoryController : Controller
    {
        // GET: Category
        [AuthorizeAdmin(Permissions = new[] {Permission.Category_View, Permission.Category_Create})]
        public ActionResult Index(CategorySearchModel searchModel)
        {
            if (Request.HttpMethod == "GET")
            {
                return View(searchModel);
            }
            var pagedList = CategoryService.Search(searchModel.Code, searchModel.Name, searchModel.Description, searchModel.PageSize, searchModel.PageIndex);
            pagedList.SearchModel = searchModel;
            return PartialView("_List", pagedList);
        }



        [AuthorizeAdmin(Permissions = new[] { Permission.Category_View, Permission.Category_Create })]
        public ActionResult Detail(int? id)
        {
            var user = UserService.GetUserInfo();
            ViewBag.User = user;
            var data = new Category();
         
            if (id.HasValue)
            {
                data = CategoryService.Get(id.Value);
            }

            ViewBag.MenuId = new SelectList(MenuService.GetAllMenu1(), "Id", "Title", data.MenuId);

            return PartialView("_Detail", data);
        }

        [AuthorizeAdmin(Permission = Permission.Category_Create)]
        public ActionResult Create(Category model, HttpPostedFileBase fileAttach)
        {
            RedirectCommand result;
            var operation = String.Empty;

            if (model.Code != null && !model.Code.IsCode())
            {
                return Json(new RedirectCommand
                {
                    Code = ResultCode.Fail,
                    Message = "Code only import letters, numbers and characters -_."
                }, JsonRequestBehavior.AllowGet);
            }

            var sourceFile = "";
            if (fileAttach != null)
            {
                if (!Directory.Exists(Server.MapPath("~/content/Upload/Category")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/content/Upload/Category"));
                }

                var fileName = Utility.ConvertToUnsign(Path.GetFileNameWithoutExtension(fileAttach.FileName))
                        .Replace("&", "")
                        .Replace("?", "")
                        .Replace(" ", "-") + "-" + Guid.NewGuid() + Path.GetExtension(fileAttach.FileName);
                sourceFile = string.Format("~/content/Upload/Category/{0}", fileName);
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
                model.Description = "";
                result = CategoryService.Create(model);
                if (result.Code == ResultCode.Success)
                {
                    result.Url = Url.Action("Index");
                }
                return
                    Json(result, JsonRequestBehavior.AllowGet);
            }
            var data = CategoryService.Get(model.Id);
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
            result = CategoryService.Update(data);
            if (result.Code == ResultCode.Success)
            {
                result.Url = Url.Action("Index");
            }
            return
                    Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(string categoryId)
        {
            var result = CategoryService.DeleteCategory(categoryId);
            if (result.Code == ResultCode.Success)
            {
                result.Url = Url.Action("Index");
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
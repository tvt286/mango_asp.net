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
    public class ProductController : Controller
    {
        [AuthorizeAdmin(Permission = Permission.Product_View)]
        public ActionResult Index(ProductSearchModel searchModel)
        {
            if (Request.HttpMethod == "GET")
            {
                var user = UserService.GetUserInfo();
                ViewBag.CategoryId =
                    new SelectList(CategoryService.GetAll(), "Id", "Name");
                return View(searchModel);
            }

            var pagedList = ProductService.Search(searchModel.Code, searchModel.Name, searchModel.CategoryId,
                searchModel.PageSize, searchModel.PageIndex);
            pagedList.SearchModel = searchModel;

            return PartialView("_List", pagedList);
        }


        [AuthorizeAdmin(Permission = Permission.Product_View)]
        public ActionResult Detail(int? id)
        {
            var user = UserService.GetUserInfo();
            ViewBag.User = user;
            var data = new Product();


            if (id.HasValue)
            {
                data = ProductService.Get(id.Value);
            }
            ViewBag.CategoryId = new SelectList(CategoryService.GetAll(), "Id", "Name", data.CategoryId);
            //ViewBag.UnitId = new SelectList(UnitService.GetUnitByCompany(user.CompanyId.GetValueOrDefault(0)), "Id",
            //    "Name", data.UnitId);
            return View(data);
        }

        [AuthorizeAdmin(Permission = Permission.Product_Create)]
        public ActionResult Create(Product data, HttpPostedFileBase fileAttach)
        {

            if (data.SellingPrice < 500)
            {
                return Json(new CommandResult
                {
                    Code = ResultCode.Fail,
                    Message = "Vui lòng nhập giá bán lẻ (gía niêm yết) lớn hơn 500 đồng"
                }, JsonRequestBehavior.AllowGet);
            }


            string sourceFile = "";
            var operation = String.Empty;
            if (data.Code != null && !data.Code.IsCode())
            {
                return Json(new CommandResult
                {
                    Code = ResultCode.Fail,
                    Message = "Mã chỉ được nhập chữ, số và ký tự -_."
                }, JsonRequestBehavior.AllowGet);
            }
            if (fileAttach != null)
            {
                if (!Directory.Exists(Server.MapPath("~/content/Upload/Product")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/content/Upload/Product"));
                }

                var fileName = Utility.ConvertToUnsign(Path.GetFileNameWithoutExtension(fileAttach.FileName))
                        .Replace("&", "")
                        .Replace("?", "")
                        .Replace(" ", "-") + "-" + Guid.NewGuid() + Path.GetExtension(fileAttach.FileName);
                sourceFile = string.Format("~/content/Upload/Product/{0}", fileName);
                var pathFile = Server.MapPath(sourceFile);
                var checkFile = UploadHelper.CheckImageUpload(fileAttach);
                if (checkFile == UploadFileStatus.NotSupportExtension)
                {
                    return Json(new CommandResult
                    {
                        Code = ResultCode.Fail,
                        Message = "Chỉ được up file hình!"
                    }, JsonRequestBehavior.AllowGet);
                }
                if (checkFile == UploadFileStatus.OverLimited)
                {
                    return Json(new CommandResult
                    {
                        Code = ResultCode.Fail,
                        Message = "Chỉ được up file 5MB!"
                    }, JsonRequestBehavior.AllowGet);
                }
                fileAttach.SaveAs(pathFile);
                //  Bitmap bitmap = (Bitmap)Bitmap.FromStream(fileAttach.InputStream, true);
                // ImageUtils.RewriteImageFix(bitmap, 500, 500, pathFile);
            }

            if (data.Id == 0)
            {
                data.Image = sourceFile;
                var result = ProductService.Create(data);

                if (result.Code == ResultCode.Success)
                {
                    result.Url = Url.Action("Index");
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            var dataItem = ProductService.Get(data.Id);

            if (sourceFile.NotEmpty())
            {
                if (dataItem.Image.NotEmpty())
                {
                    try
                    {
                        System.IO.File.Delete(Server.MapPath(dataItem.Image));
                    }
                    catch (Exception ex)
                    {
                    }

                }
                dataItem.Image = sourceFile;
            }
            TryUpdateModel(dataItem);
            var resultEdit = ProductService.Update(dataItem);

            if (resultEdit.Code == ResultCode.Success)
            {
                resultEdit.Url = Url.Action("Index");
            }
            return Json(resultEdit, JsonRequestBehavior.AllowGet);
        }


        public ActionResult DeleteProduct(string productId)
        {
            var result = ProductService.DeleteProduct(productId);
            if (result.Code == ResultCode.Success)
            {
                result.Url = Url.Action("Index");
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}
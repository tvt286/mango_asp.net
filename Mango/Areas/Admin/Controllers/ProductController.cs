using Mango.Areas.Admin.Models;
using Mango.Common;
using Mango.Data;
using Mango.Data.Enums;
using Mango.Security;
using Mango.Services;
using Mango.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mango.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {
        [AuthorizeAdmin(Permissions = new[] { Permission.Product_View, Permission.Product_Create})]
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


        [AuthorizeAdmin(Permissions = new[] { Permission.Product_View, Permission.Product_Create })]
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

            return View(data);
        }

        [AuthorizeAdmin(Permission = Permission.Product_Create)]
        [ValidateInput(false)]
        public ActionResult Create(Product data, string fileUploadImage, HttpPostedFileBase fileAttach)
        {

            if (data.SellingPrice < 500)
            {
                return Json(new CommandResult
                {
                    Code = ResultCode.Fail,
                    Message = "Please import selling price > 500"
                }, JsonRequestBehavior.AllowGet);
            }

            string sourceFile = "";
            if (data.Code != null && !data.Code.IsCode())
            {
                return Json(new CommandResult
                {
                    Code = ResultCode.Fail,
                    Message = "Code only import letters, numbers and characters -_."
                }, JsonRequestBehavior.AllowGet);
            }

            if (data.Id == 0 && fileUploadImage == "")
            {
                
                    return Json(new CommandResult
                    {
                        Code = ResultCode.Fail,
                        Message = "Please import image product!"
                    }, JsonRequestBehavior.AllowGet);
               
            }
            else
            {
                if (data.Image == null && fileUploadImage == "")
                {

                    return Json(new CommandResult
                    {
                        Code = ResultCode.Fail,
                        Message = "Please import image product!"
                    }, JsonRequestBehavior.AllowGet);

                }
            }

            // nếu chưa có thư mục thì tạo
            if (!Directory.Exists(Server.MapPath(string.Format("~/content/Upload/Product"))))
            {
                Directory.CreateDirectory(
                    Server.MapPath(string.Format("~/content/Upload/Product")));
            }

            // nếu có file up len thì lưu lại
            if (fileUploadImage != "")
            {
                var pathFile = "";

                string[] files = fileUploadImage.Replace("data:image/jpeg;base64,", "").Replace("data:image/png;base64,", "").Split(' ');

                for (int i = 0; i < files.Length; i++)
                {
                    var fileName =
                    Utility.ConvertToUnsign(Path.GetFileNameWithoutExtension(data.Code + i + DateTime.Now))
                        .Replace("&", "")
                        .Replace("?", "")
                        .Replace(" ", "-") + "-" + Guid.NewGuid() + Path.GetExtension(data.Code + i + DateTime.Now) + ".jpg";
                    sourceFile += string.Format("~/content/Upload/Product/{0}", fileName) + ";";
                    pathFile = Server.MapPath(string.Format("~/content/Upload/Product/{0}", fileName));

                    Byte[] bytes = Convert.FromBase64String(files[i]);

                    using (var imageFile = new FileStream(pathFile, FileMode.Create))
                    {
                        imageFile.Write(bytes, 0, bytes.Length);
                        imageFile.Flush();
                    }
                }
             
            }

            /// nếu tạo mới thì lưu lại
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

            // nếu như chỉnh sửa
            var dataItem = ProductService.Get(data.Id);
            try
            {
                TryUpdateModel(dataItem);
            }
            catch (Exception ex)
            {
                var mess = ex.Message;
            }
            dataItem.Description = data.Description;
            dataItem.Image = data.Image;

            dataItem.Image += sourceFile;

            var resultEdit = ProductService.Update(dataItem);

            if (resultEdit.Code == ResultCode.Success)
            {
                resultEdit.Url = Url.Action("Index");
            }
            return Json(resultEdit, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Delete(string productId)
        {

            var result = ProductService.DeleteProduct(productId);
            if (result.Code == ResultCode.Success)
            {
                result.Url = Url.Action("Index");
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetInfoProduct(int? id)
        {
            ProductInfo model;
            if (id.HasValue)
            {
                var product = ProductService.Get(id.Value,true);
                model = new ProductInfo
                {
                    Image = Url.ImageProduct(product),
                    CategoryName = product.Category.Name,
                    SellingPrice = product.SellingPrice.MoneyToStringVN(),
                    SupplierPrice = product.SupplierPrice.MoneyToStringVN()           
                };
            }
            else
            {
                model = new ProductInfo
                {
                    Image = "",
                    CategoryName = "",
                    SellingPrice = "0",
                    SupplierPrice = "0",
                };
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }

    }
}
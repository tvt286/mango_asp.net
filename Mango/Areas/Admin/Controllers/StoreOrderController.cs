using Mango.Areas.Admin.Models;
using Mango.Data;
using Mango.Data.Enums;
using Mango.Security;
using Mango.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mango.Common;
using System.Text;

namespace Mango.Areas.Admin.Controllers
{
    public class StoreOrderController : Controller
    {
        //
        // GET: /Admin/StoreOrder/
        [AuthorizeAdmin(Permissions = new[] { Permission.WarehouseOrder_ViewImport })]
        public ActionResult StoreOrderImport(StoreOrderImportSearchModel searchModel)
        {
            if (Request.HttpMethod == "GET")
            {
                var user = UserService.GetUserInfo();
           
                ViewBag.UserImport = new SelectList(UserService.GetAll(), "Id", "FullName", searchModel.UserImport);
                ViewBag.StoreId = new SelectList(new List<Mango.Data.Store> { StoreService.GetStoreRoot() }, "Id", "Name");

                return View(searchModel);
            }
            var pagedList = StoreOrderService.SearchStoreOrderImport(searchModel.Code, searchModel.UserImport, searchModel.StoreId, searchModel.TimeImportFrom, searchModel.TimeImportTo, searchModel.PageSize, searchModel.PageIndex);
            pagedList.SearchModel = searchModel;
            return PartialView("_StoreOrderImportList", pagedList);
        }



        [AuthorizeAdmin(Permissions = new[] { Permission.WarehouseOrder_ViewImport })]
        [HttpGet]
        public ActionResult DetailStoreImport(string id)
        {
            var user = UserService.GetUserInfo();
            var data = new StoreOrder
            {
                StoreImExTypeCode = StoreImExTypeCode.NhapTuKhoKhac,
                UserImportId = user.Id,
                TimeImport = DateTime.Now
            };

            //if (id.NotEmpty())
            //{
            //    data = WarehouseOrderService.GetDetailWarehouseImport(int.Parse(SecurityHelper.Decrypt(id)));
            //}

            ViewBag.ProductSelect = ProductService.GetAll().Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Code + " - " + x.Name
            }).ToList();

            ViewBag.StoreId = new SelectList(StoreService.GetAll(), "Id", "Name", data.StoreId);
            if (user.IsAdminCompany)
            {
                ViewBag.UserImportId = new SelectList(UserService.GetAll(), "Id", "FullName", data.UserImportId);
            }
            else
            {
                ViewBag.UserImportId = new SelectList(new List<User> { user }, "Id", "FullName", data.UserImportId);
            }

            return View(data);
        }


        [AuthorizeAdmin(Permissions = new[] { Permission.WarehouseOrder_CreateImport })]
        [HttpPost]
        public ActionResult CreateStoreOrderImport(StoreOrder storeOrder, int[] productId, int[] quantityRequestImport,  int[] mainSupplierPrice)
        {
      
            var result = new RedirectCommand();

            if (storeOrder.TimeImport.Value.Year < 1975 || storeOrder.TimeImport.Value.Year > DateTime.Now.Year)
            {
                result.Code = ResultCode.Fail;
                result.Message = "Vui lòng kiểm tra lại ngày nhập";
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            var importDetailList = new List<StoreOrderImportDetail>();
            for (int i = 0; i < productId.Length; i++)
            {
                var storeOrderImportDetail = new StoreOrderImportDetail
                {
                    ProductId = productId[i],
                    Quantity = quantityRequestImport[i],
                    MainSupplierPrice = mainSupplierPrice[i],
                };

                importDetailList.Add(storeOrderImportDetail);
            }


          
            if (importDetailList.Any(x => x.Quantity == 0))
            {
                result.Code = ResultCode.Fail;
                result.Message = "Vui lòng nhập số lượng lớn hơn 0";
                return Json(result, JsonRequestBehavior.AllowGet);
            }


            storeOrder.Code = StoreOrderService.GenerateCode(StoreImExTypeCode.NhapTuKhoKhac, storeOrder.StoreId, null);
            StoreOrderService.CreateWarehouseOrderImport(storeOrder, importDetailList);
            result.Message = "Đã tạo lệnh nhập hàng thành công!";
            result.Url = Url.Action("StoreOrderImport", new { storeOrder.Code});
        
            return Json(result, JsonRequestBehavior.AllowGet);
        }


	}
}
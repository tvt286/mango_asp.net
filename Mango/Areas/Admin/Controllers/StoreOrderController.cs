﻿using Mango.Areas.Admin.Models;
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
   
        [AuthorizeAdmin(Permissions = new[] { Permission.WarehouseOrder_ViewImport })]
        public ActionResult StoreOrderImport(StoreOrderImportSearchModel searchModel)
        {
            if (Request.HttpMethod == "GET")
            {
                var user = UserService.GetUserInfo();
           
                ViewBag.UserImport = new SelectList(UserService.GetAll(), "Id", "FullName", searchModel.UserImport);
                ViewBag.RefStoreId = new SelectList(new List<Mango.Data.Store> { StoreService.GetStoreRoot() }, "Id", "Name");

                return View(searchModel);
            }
            var pagedList = StoreOrderService.SearchStoreOrderImport(searchModel.Code, searchModel.UserImport, searchModel.RefStoreId, searchModel.TimeImportFrom, searchModel.TimeImportTo, searchModel.PageSize, searchModel.PageIndex);
            pagedList.SearchModel = searchModel;
            return PartialView("_StoreOrderImportList", pagedList);
        }



        [AuthorizeAdmin(Permissions = new[] { Permission.WarehouseOrder_ViewImport })]
        [HttpGet]
        public ActionResult DetailStoreImport(int? id)
        {
            var user = UserService.GetUserInfo();
            var data = new StoreOrder
            {
                StoreImExTypeCode = StoreImExTypeCode.NhapTuKhoKhac,
                UserImportId = user.Id,
                TimeImport = DateTime.Now
            };

            if (id.HasValue)
            {
                data = StoreOrderService.GetDetailStoreImport(id.Value);
            }

            ViewBag.ProductSelect = ProductService.GetAll().Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Code + " - " + x.Name
            }).ToList();

            ViewBag.RefStoreId = new SelectList(StoreService.GetAll(), "Id", "Name", data.RefStoreId);
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

        [AuthorizeAdmin(Permissions = new[] { Permission.WarehouseOrder_ViewExport })]
        [HttpGet]
        public ActionResult DetailStoreExport(int? id)
        {
            var user = UserService.GetUserInfo();
            var data = new StoreOrder
            {
                StoreImExTypeCode = StoreImExTypeCode.XuatKhoKhac,
                UserExportId = user.Id,
                TimeExport = DateTime.Now
            };

            if (id.HasValue)
            {
                data = StoreOrderService.GetDetailStoreImport(id.Value);
            }

            var storeHasProductList = StoreService.GetStoreHasProduct().Select(x => x.Id).ToList();
            ViewBag.ProductSelect = new List<SelectListItem>();

            ViewBag.StoreId = new SelectList(StoreService.GetAll().Where(x => storeHasProductList.Contains(x.Id)).ToList(), "ID", "Name", data.StoreId);
            ViewBag.RefStoreId = new SelectList(StoreService.GetAll(), "ID", "Name", data.RefStoreId);


            ViewBag.StoreId = new SelectList(StoreService.GetAll(), "Id", "Name", data.StoreId);
            if (user.IsAdminCompany)
            {
                ViewBag.UserExportId = new SelectList(UserService.GetAll(), "Id", "FullName", data.UserExportId);
            }
            else
            {
                ViewBag.UserExportId = new SelectList(new List<User> { user }, "Id", "FullName", data.UserExportId);
            }

            return View(data);
        }

        [AuthorizeAdmin(Permissions = new[] { Permission.WarehouseOrder_ConfirmImport })]
        public ActionResult ConfirmStoreOrderFromOtherWarehouse(int? id, bool confirmImport = false)
        {
            var user = UserService.GetUserInfo();
            var data = StoreOrderService.GetDetailStoreImport(id.Value);
            var userId = data.UserExportId;
            if (data.UserImportId != null)
                userId = data.UserImportId;

            data.TimeImport = DateTime.Now;
            ViewBag.StoreId = new SelectList(StoreService.GetAll(), "Id", "Name", data.StoreId);
            ViewBag.RefStoreId = new SelectList(StoreService.GetAll(), "Id", "Name", data.RefStoreId);
            if (user.IsAdminCompany)
            {
                ViewBag.UserImportId = new SelectList(UserService.GetAll(), "Id", "FullName", user.Id);
            }
            else
            {
                ViewBag.UserImportId = new SelectList(new List<User> { user }, "Id", "FullName", user.Id);
            }

            ViewBag.confirmImport = confirmImport;
            return View(data);
        }


        [AuthorizeAdmin(Permissions = new[] { Permission.WarehouseOrder_ConfirmImport })]
        public ActionResult ImportWarehouseOrderFromOtherWarehouse(StoreOrder storeOrder,
            int[] storeOrderImportDetailId, int[] quantityRequestImport, string[] note)
        {


            var result = StoreOrderService.ImportStoreOrderFromOtherStore(storeOrder, storeOrderImportDetailId, quantityRequestImport, note);
            if (result.Code == ResultCode.Success)
            {
                result.Url = Url.Action("StoreOrderImport", new { storeOrder.Code, storeOrder.RefStoreId });
            }
            return Json(result, JsonRequestBehavior.AllowGet);
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
                    SupplierPrice = mainSupplierPrice[i],
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

        [AuthorizeAdmin(Permissions = new[] { Permission.WarehouseOrder_ViewExport })]
        public ActionResult StoreOrderExport(StoreOrderExportSearchModel searchModel)
        {
            if (Request.HttpMethod == "GET")
            {
                var user = UserService.GetUserInfo();

                ViewBag.UserExport = new SelectList(UserService.GetAll(), "Id", "FullName", searchModel.UserExport);
                ViewBag.StoreId = new SelectList(StoreService.GetAll(), "ID", "Name", searchModel.StoreId);
                ViewBag.RefStoreId = new SelectList(StoreService.GetAll(), "ID", "Name", searchModel.RefStoreId);
           
                return View(searchModel);
            }
            var pagedList = StoreOrderService.SearchStoreOrderExport(searchModel.Code, searchModel.UserExport, searchModel.StoreId, searchModel.RefStoreId, searchModel.TimeExportFrom, searchModel.TimeExportTo, searchModel.PageSize, searchModel.PageIndex);
            pagedList.SearchModel = searchModel;
            return PartialView("_StoreOrderExportList", pagedList);
        }




        [AuthorizeAdmin(Permissions = new[] { Permission.WarehouseOrder_CreateExport })]
        public ActionResult CreateStoreOrderExport(StoreOrder storeOrder, int[] productId, string[] productName,
            int[] quantityRequestExport, int[] refStoreOrderImportDetailId)
        {
        

            var result = new RedirectCommand();
            if (storeOrder.RefStoreId == storeOrder.StoreId)
            {
                result.Code = ResultCode.Fail;
                result.Message = "Kho nhập và kho xuất không được trùng nhau";
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            if (storeOrder.TimeExport.Value.Year < 1975 || storeOrder.TimeExport.Value.Year > DateTime.Now.Year)
            {
                result.Code = ResultCode.Fail;
                result.Message = "Vui lòng kiểm tra lại ngày xuất";
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            if (refStoreOrderImportDetailId == null)
            {
                result.Code = ResultCode.Fail;
                result.Message = "Vui lòng nhập sản phẩm xuất";
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            var exportDetailList = new List<StoreOrderExportDetail>();
            for (int i = 0; i < productId.Length; i++)
            {
                var warehouseOrderExportDetail = new StoreOrderExportDetail
                {
                    ProductId = productId[i],
                    Quantity = quantityRequestExport[i],
                    RefStoreOrderImportDetailId = refStoreOrderImportDetailId[i]
                };

                exportDetailList.Add(warehouseOrderExportDetail);
            }

            if (exportDetailList.Any(x => x.Quantity == 0))
            {
                result.Code = ResultCode.Fail;
                result.Message = "Vui lòng nhập số lượng lớn hơn 0";
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            storeOrder.Code = StoreOrderService.GenerateCode(StoreImExTypeCode.XuatKhoKhac, storeOrder.RefStoreId, storeOrder.StoreId);
            result = StoreOrderService.CreateStoreOrderExport(storeOrder, exportDetailList);
            if (result.Code == ResultCode.Success)
            {
                result.Message = "Đã tạo lệnh xuất kho thành công!";
                result.Url = Url.Action("WarehouseOrderExport");
                
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetStoreOrderImportDetail(int? productId, int storeId)
        {
            var user = UserService.GetUserInfo();
            var data = StoreOrderService.GetStoreOrderImportDetail(productId, storeId);

            return PartialView("_StoreOrderImportDetailList", data);
        }

	}
}
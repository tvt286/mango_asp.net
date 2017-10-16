using Mango.Areas.Admin.Models;
using Mango.Common;
using Mango.Data;
using Mango.Data.Enums;
using Mango.Security;
using Mango.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mango.Areas.Admin.Controllers
{
    public class OrderController : Controller
    {
        [AuthorizeAdmin(Permissions = new[] { Permission.Order_View, Permission.Order_Create })]
        public ActionResult Index(OrderSearchViewModel searchModel)
        {
            if (Request.HttpMethod == "GET")
            {
                var user = UserService.GetUserInfo();
                ViewBag.CustomerId = new SelectList(UserService.GetAll(true), "Id", "FullName");
                return View(searchModel);
            }
            var pagedList = OrderService.Search(searchModel.Code, searchModel.CustomerId, searchModel.FromDate, searchModel.ToDate, searchModel.PageSize, searchModel.PageIndex);
            pagedList.SearchModel = searchModel;
            return PartialView("_List", pagedList);
        }


        [AuthorizeAdmin(Permissions = new[] { Permission.Order_View, Permission.Order_Create })]
        public ActionResult Detail(int? id)
        {
            var user = UserService.GetUserInfo();
            var data = new Order
            {
                TimeCreate = DateTime.Now,
                Status = OrderStatus.Confirm
            };

            if (id.HasValue)
            {
                data = OrderService.Get(id.Value);
            }

            ViewBag.ProductSelect = new List<SelectListItem>();
            ViewBag.CustomerId = new SelectList(UserService.GetAll(true), "Id", "FullName");
            ViewBag.StoreId = new SelectList(StoreService.GetAll(), "ID", "Name", data.StoreId);
            return View(data);
        }

        [AuthorizeAdmin(Permissions = new[] { Permission.Order_Create })]
        public ActionResult CreateOrder(Order order, int[] productId,
           int[] quantityRequestExport, int[] refStoreOrderImportDetailId, int[] price)
        {
        
            var result = new RedirectCommand();
            if (order.Id != 0)
            {

                if (result.Code == ResultCode.Success)
                {
                    result.Message = "Đã tạo lệnh xuất bán thành công!";
                    result.Url = Url.Action("OrderWholeSale");
                }

                return Json(result, JsonRequestBehavior.AllowGet);

            }
            if (order.TimeCreate.Year < 1975 || order.TimeCreate.Year > DateTime.Now.Year)
            {
                return Json(new RedirectCommand
                {
                    Code = ResultCode.Fail,
                    Message = "Vui lòng kiểm tra lại ngày xuất bán"
                });
            }

            if (refStoreOrderImportDetailId == null)
            {
                result.Code = ResultCode.Fail;
                result.Message = "Vui lòng nhập sản phẩm để xuất bán";
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            var orderDetailList = new List<OrderDetail>();
            for (int i = 0; i < productId.Length; i++)
            {
                var orderDetail = new OrderDetail
                {
                    ProductId = productId[i],
                    Quantity = quantityRequestExport[i],
                    RefStoreOrderImportDetailId = refStoreOrderImportDetailId[i],
                    SellingPrice = price[i]
                };

                orderDetailList.Add(orderDetail);
            }

            if (orderDetailList.Any(x => x.Quantity == 0))
            {
                result.Code = ResultCode.Fail;
                result.Message = "Vui lòng nhập số lượng lớn hơn 0";
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            order.Code = StoreOrderService.GenerateCode(StoreImExTypeCode.XuatBanKhachHang,
                order.StoreId, null, customerId: order.CustomerId);
          //  order.Type = OrderType.FromBackEnd;
            result = OrderService.CreateOrder(order, orderDetailList);


            if (result.Code == ResultCode.Success)
            {
                result.Message = "Đã tạo lệnh xuất bán thành công!";
                result.Url = Url.Action("OrderWholeSale");
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

	}
}
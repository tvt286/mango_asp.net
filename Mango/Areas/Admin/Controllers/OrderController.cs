using Mango.Areas.Admin.Models;
using Mango.Data.Enums;
using Mango.Security;
using Mango.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Store.Areas.Admin.Controllers
{
    public class OrderController : Controller
    {
        [AuthorizeAdmin(Permissions = new[] { Permission.Order_View, Permission.Order_Create })]
        public ActionResult Index(OrderSearchViewModel searchModel)
        {
            if (Request.HttpMethod == "GET")
            {
                var user = UserService.GetUserInfo();
                ViewBag.CustomerId = new SelectList(UserService.GetAll(true), "Id", "Name");
                return View(searchModel);
            }
            var pagedList = OrderService.Search(searchModel.Code, searchModel.CustomerId, searchModel.FromDate, searchModel.ToDate, searchModel.PageSize, searchModel.PageIndex);
            pagedList.SearchModel = searchModel;
            return PartialView("_List", pagedList);
        }
	}
}
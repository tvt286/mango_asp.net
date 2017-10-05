using Mango.Services;
using Store.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mango.Areas.Admin.Controllers
{
    public class StoreProductController : Controller
    {
        public ActionResult Index(StoreProductSearchModel searchModel)
        {
            var pagedList = StoreProductService.Search(searchModel.ProductId, searchModel.CategoryId, searchModel.StoreId, searchModel.PageSize, searchModel.PageIndex);
            pagedList.SearchModel = searchModel;
            return PartialView("_StoreProduct", pagedList);
        }

        public ActionResult GetProductInStore(int storeId)
        {
            var data = StoreProductService.GetStockProductInStore(storeId)
                .Select(x => new { x.Id, Name = x.Code + " - " + x.Name }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
	}
}
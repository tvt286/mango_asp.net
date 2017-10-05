using Mango.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mango.Areas.Admin.Controllers
{
    public class WarehouseController : Controller
    {
        //
        // GET: /Admin/Warehouse/
        public ActionResult Index()
        {
            var model = new Mango.Data.Store();
            model = StoreService.GetStoreRoot();
            ViewBag.CityId = new SelectList(LocationService.GetAllCity(), "Id", "Name", model.CityId);
            ViewBag.DistrictId = new SelectList(LocationService.GetDistrictByCity(model.CityId.GetValueOrDefault(-1)).Select(x => new { x.Id, Name = x.Prefix + " " + x.Name }).ToList(), "Id", "Name", model.DistrictId);
            ViewBag.WardId = new SelectList(LocationService.GetWardByDistrict(model.DistrictId.GetValueOrDefault(-1)).Select(x => new { x.Id, Name = x.Prefix + " " + x.Name }).ToList(), "Id", "Name", model.WardId);
            ViewBag.StreetId = new SelectList(LocationService.GetStreetByDistrictAndCity(model.CityId.GetValueOrDefault(-1), model.DistrictId.GetValueOrDefault(-1)).Select(x => new { x.Id, x.Name }).ToList(), "Id", "Name", model.StreetId);
            ViewBag.ProductId = new SelectList(ProductService.GetAll()
            .Select(x => new { x.Id, Name = x.Code + " - " + x.Name }).ToList(), "Id", "Name");
            ViewBag.CategoryId = new SelectList(CategoryService.GetAll(), "Id", "Name");
         
            return View(model);
        }
	}
}
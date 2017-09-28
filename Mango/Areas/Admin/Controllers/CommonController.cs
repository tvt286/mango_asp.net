using System.Linq;
using System.Web.Mvc;
using Mango.Services;

namespace Mango.Areas.Admin.Controllers
{
    public class CommonController : Controller
    {
        
        public ActionResult GetDictrict(int cityId)
        {
            var list = LocationService.GetDistrictByCity(cityId).Select(x => new { ID = x.Id, Text = x.Prefix + " " + x.Name }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetWard(int dictrictId)
        {
            var list = LocationService.GetWardByDistrict(dictrictId).Select(x => new { ID = x.Id, Text = x.Prefix + " " + x.Name }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetStreet(int cityId, int dictrictId)
        {
            var list = LocationService.GetStreetByDistrictAndCity(cityId, dictrictId).Select(x => new { ID = x.Id, Text = x.Name }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
    }
}
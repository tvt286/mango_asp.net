using Mango.Common;
using Mango.Data;
using Mango.Models;
using Mango.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Mango.Controllers
{
    public class UsersController : Controller
    {
        //
        // GET: /Users/
        public ActionResult Login(string returnUrl = "")
        {
            ViewBag.ReturnUrl = returnUrl;
            return PartialView("_Login");
        }

        public ActionResult Register()
        {
            var model = new Mango.Data.User();

            ViewBag.CityId = new SelectList(LocationService.GetAllCity(), "Id", "Name", model.CityId);
            ViewBag.DistrictId = new SelectList(LocationService.GetDistrictByCity(model.CityId.GetValueOrDefault(-1)).Select(x => new { x.Id, Name = x.Prefix + " " + x.Name }).ToList(), "Id", "Name", model.DistrictId);
            ViewBag.WardId = new SelectList(LocationService.GetWardByDistrict(model.DistrictId.GetValueOrDefault(-1)).Select(x => new { x.Id, Name = x.Prefix + " " + x.Name }).ToList(), "Id", "Name", model.WardId);
            ViewBag.StreetId = new SelectList(LocationService.GetStreetByDistrictAndCity(model.CityId.GetValueOrDefault(-1), model.DistrictId.GetValueOrDefault(-1)).Select(x => new { x.Id, x.Name }).ToList(), "Id", "Name", model.StreetId);

            return PartialView("_Register", model);
        }

        [HttpPost]
        public ActionResult DoRegister(User data)
        {
           
            data.Password = Encryptor.MD5Hash(data.Password);
            data.Type = UserType.FrontEnd;
            data.Status = UserStatus.Active;
            data.IsDeleted = false;
            data.TimeCreate = DateTime.Now;
            data.Address = LocationService.BuildAddress(data.CityId, data.DistrictId, data.WardId, data.StreetId, data.NumberStreet, data.Address);
            // lay Lat va Lng
            DataTable location = new DataTable();
            location = Utility.FindCoordinates(data.Address);
            if (location != null)
            {
                foreach (DataRow row in location.Rows)
                {
                    data.Lat = row["Latitude"].ToString();
                    data.Lng = row["Longitude"].ToString();
                }
            }
            UserService.CreateCustomer(data);
            return RedirectToAction("Index", "Home", "");

        }

        [HttpGet]
        public ActionResult LogOff()
        {
            AuthorizeService.ClearCache(HttpContext.User.Identity.Name);
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home", "");
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> DoLogin(string UserName, string Password, string returnUrl)
        {
            if (Request.HttpMethod == "GET")
            {
                return View();
            }
            else
            {
                var passMD5 = Encryptor.MD5Hash(Password);

                var user = UserService.Get(UserName, true);

                if (user != null && (!string.IsNullOrWhiteSpace(user.Password) && user.Password == passMD5))
                {
                    if (user.Status == UserStatus.InActive)
                    {
                        ViewBag.Message = "Account locked!";
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        FormsAuthentication.SetAuthCookie(user.UserName, true);
                        if (string.IsNullOrEmpty(returnUrl))
                        {
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            return RedirectPermanent(returnUrl);
                        }
                    }
                }
                ViewBag.Message = "Username or password is invalid!";
                return View();
            }
        }
	}
}
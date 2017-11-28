using Mango.Common;
using Mango.Data;
using Mango.Models;
using Mango.Services;
using System;
using System.Collections.Generic;
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
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return PartialView("_Login");
        }

        public ActionResult Register()
        {
            return PartialView("_Register");
        }

        [HttpPost]
        public ActionResult DoRegister(string FullName, string UserName, string Password, string Email, string Phone)
        {
            var model = new User{
                FullName = FullName,
                UserName = UserName,
                Password = Encryptor.MD5Hash(Password),
                Email = Email,
                Phone = Phone,
                Type = UserType.FrontEnd,
                Status = UserStatus.Active,
                IsDeleted = false,
                IsAdminCompany = false,
                IsAdminRoot = false,
                TimeCreate = DateTime.Now
            };
            UserService.CreateCustomer(model);
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
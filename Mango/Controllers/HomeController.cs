﻿using Mango.Data;
using Mango.Models;
using Mango.Service;
using Mango.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

namespace Mango.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            HomeModel model = new HomeModel();
            model.menus = new List<MenuModel>();
            model.menus = HomeService.getListProductsInHome();
            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult MenuLayout()
        {
            var menu = MenuService.GetAll();
            return PartialView("_Navigation", menu);
        }

        public ActionResult Footer()
        {
            var menu = MenuService.GetAll();
            return PartialView("_Footer", menu);
        }
    }
}

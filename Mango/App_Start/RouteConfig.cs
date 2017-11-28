using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Mango
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.MapRoute(
            //    name: "Default",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            //);

            routes.MapRoute(
              "User logout",
              "logout",
              new { controller = "Users", action = "LogOff", id = UrlParameter.Optional },
              new[] { "Mango.Controllers" }
            );


            routes.MapRoute(
              "User register",
              "register",
              new { controller = "Users", action = "Register", id = UrlParameter.Optional },
              new[] { "Mango.Controllers" }
            );

            routes.MapRoute(
               "User login",
               "login",
               new { controller = "Users", action = "Login", id = UrlParameter.Optional },
               new[] { "Mango.Controllers" }
           );
            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                new[] { "Mango.Controllers" }
            );

            routes.MapRoute(
                "Product",
                "{controller}/{action}/{id}",
                new { controller = "Products", action = "Index", id = UrlParameter.Optional },
                new[] { "Mango.Controllers" }
            );

            routes.MapRoute(
                "Users",
                "{controller}/{action}/{id}",
                new { controller = "Users", action = "Login", id = UrlParameter.Optional },
                new[] { "Mango.Controllers" }
            );
        }
    }
}
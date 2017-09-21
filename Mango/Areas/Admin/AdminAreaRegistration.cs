using System.Web.Http;
using System.Web.Mvc;

namespace Mango.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            //context.MapRoute(
            //    "Admin_default",
            //    "Admin/{controller}/{action}/{id}",
            //    new { action = "Index", id = UrlParameter.Optional }
            //);
            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                new[] { "Mango.Areas.Admin.Controllers" }
            );
            //context.Routes.MapHttpRoute(
            //    "Admin_default",
            //    "Admin/{controller}/{action}/{id}",
            //    new { id = RouteParameter.Optional }
            //);
        }
    }
}

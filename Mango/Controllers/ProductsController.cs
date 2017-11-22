using Mango.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Store.Controllers
{
    public class ProductsController : Controller
    {
        //
        // GET: /Product/
        public ActionResult Index(int? id)
        {
            var category = CategoryService.Get(id.Value);

            return View(category);
        }

        public ActionResult Detail(int? id)
        {
            var product = ProductService.Get(id.Value);
            return View(product);
        }
	}
}
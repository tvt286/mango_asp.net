using Mango.Data;
using Mango.Models;
using Mango.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Store.Controllers
{
    public class CategoriesController : Controller
    {
        
        // GET: /Categories/
        public ActionResult Index(int? id)
        {
            var category = CategoryService.Get(id.Value);
            var model = new CategoryModel();
      
            return View("Index", model);
        }
	}
}
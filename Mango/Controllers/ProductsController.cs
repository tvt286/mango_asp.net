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
    public class ProductsController : Controller
    {
        //
        // GET: /Product/
        public ActionResult Index(int? id)
        {
            var category = CategoryService.Get(id.Value);
            var model = new CategoryModel();
            model.category = category;
            model.categories = new List<Category>();
            model.categories.AddRange(CategoryService.GetByMenuId(category.MenuId));

            return View("Index", model);
        }

        public ActionResult Detail(int? id)
        {
            ProductModel model = new ProductModel();
            var product = ProductService.Get(id.Value);
            model.product = product;
            var productHot = ProductService.GetAll();
            model.productsHot = new List<Product>();
            model.productsHot.AddRange(productHot);

            return View(model);
        }
	}
}
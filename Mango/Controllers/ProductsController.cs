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
        public ActionResult Index(int? id, int? menuId)
        {
            if(!id.HasValue)
            {
                id = MenuService.GetMenu1ByMenuId(menuId.Value).ElementAt(0).Categories.ElementAt(0).Id;
            }
            // lấy ra loại sản phẩm theo id
            var category = CategoryService.Get(id.Value);
            var model = new CategoryModel();
            model.category = category;
            model.menus1 = new List<Menu1>();
            // lay ra danh sách menu cấp 1 theo menu id
            model.menus1.AddRange(MenuService.GetMenu1ByMenuId(menuId.Value));            
            return View("Index", model);
        }

        public ActionResult Detail(int? id)
        {
            ProductModel model = new ProductModel();
            var product = ProductService.Get(id.Value);
            if (product.Star == null)
                product.Star = 3;
            model.product = product;

            var productHot = ProductService.GetProductsHot();
            model.productsHot = new List<Product>();
            model.productsHot.AddRange(productHot);

            return View(model);
        }

        public JsonResult UpdateStar(int id, int star)
        {
            ProductService.UpdateStar(id, star);
            return Json(new
            {
                status = true,
            });
        }
	}
}
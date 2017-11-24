using Mango.Models;
using Mango.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mango.Data;

namespace Mango.Service
{
    public class HomeService
    {
        public static List<MenuModel> getListProductsInHome()
        {
            List<MenuModel> results = new List<MenuModel>();
            var menus = MenuService.GetMenusHasCategory();
            foreach(var menu in menus)
            {
                results.Add(new MenuModel
                {
                    Id = menu.Id,
                    Name = menu.Name
                });
            }

            foreach (var item in results)
            {
                item.products = new List<Product>();
                item.products.AddRange(ProductService.GetProductsInHome(item.Id));
            }
            return results;
        }
    }
}
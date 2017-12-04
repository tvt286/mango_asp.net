using Mango.Services;
using Mango.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mango.Data;

namespace Mango.Controllers
{
    public class OrdersController : Controller
    {
        private const string CART_SESSION = "CART_SESSION";
        //
        // GET: /Orders/
        public ActionResult Index()
        {
            List<CartItem> cart = (List<CartItem>)Session[CART_SESSION];

            return View("Index",cart);
        }

        public ActionResult Detail()
        {
            var user = UserService.GetUserInfo(true);
            if (user == null)
                return RedirectToAction("Login", "Users", new { returnUrl = "/order" });

            List<CartItem> cart = (List<CartItem>)Session[CART_SESSION];
            return PartialView("_Detail", cart);
        }

        public ActionResult Create(int[] productId, int[] quantity)
        {
            var user = UserService.GetUserInfo(true);
            // lấy ra store gần user nhất
            var store = StoreService.GetStoreOrderForUser(user);
            var order = new Order { 
                Code = StoreOrderService.GenerateCode(StoreImExTypeCode.XuatBanKhachHang, store.Id, null, customerId: user.Id),
                CustomerId = user.Id,
                Status = OrderStatus.Confirm,
                StoreId = store.Id,
                TimeCreate = DateTime.Now
            };
           
            var listOrderDetail = new List<OrderDetail>();
            for (int i = 0; i < productId.Length; i++)
            {
                var product = ProductService.Get(productId[i]);
                var orderDetail = new OrderDetail
                {
                    ProductId = productId[i],
                    Quantity = quantity[i],
                    SellingPrice = product.SellingPrice,
                    SupplierPrice = product.SupplierPrice,
                    MainSupplierPrice = 0
                };
                listOrderDetail.Add(orderDetail);
            }

            order.OrderDetails = listOrderDetail;

            OrderService.CreateOrderCustomer(order);

            return RedirectToAction("Index", "Orders");
        }

        public JsonResult Delete(int id)
        {
            List<CartItem> cart = (List<CartItem>)Session[CART_SESSION];
            cart.RemoveAll(x => x.product.Id == id);
            Session[CART_SESSION] = cart;
            if(cart.Count > 0)
            {
                return Json(new
                {
                    status = true,
                });
            }
            else
            {
                return Json(new
                {
                    status = false,
                });
            }
        }
        public ActionResult AddCartItem(int productId, int quantity)
        {
            var cart = Session[CART_SESSION];
            var product = ProductService.Get(productId);

            if (cart != null)
            {
                var list = (List<CartItem>)cart;
                if (list.Exists(x => x.product.Id == productId))
                {
                    foreach (var item in list)
                    {
                        if (item.product.Id == productId)
                        {
                            item.quantity += quantity;
                        }
                    }
                }
                else
                {
                    CartItem item = new CartItem
                    {
                        product = product,
                        quantity = quantity
                    };
                    list.Add(item);
                }
                Session[CART_SESSION] = list;
            }
            else
            {
                CartItem item = new CartItem
                {
                    product = product,
                    quantity = quantity
                };
                var list = new List<CartItem>();
                list.Add(item);
                Session[CART_SESSION] = list;
            }
            return RedirectToAction("Index","Orders");
        }
	}
}
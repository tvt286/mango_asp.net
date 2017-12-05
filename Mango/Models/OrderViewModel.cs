using Mango.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mango.Models
{
    public class OrderViewModel
    {
        public List<Order> orders { get; set; }
        public List<CartItem> carts { get; set; }
    }
}
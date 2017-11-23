using Mango.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mango.Models
{
    public class ProductModel
    {
        public Product product { get; set; }
        public List<Product> productsHot { get; set; }
    }
}
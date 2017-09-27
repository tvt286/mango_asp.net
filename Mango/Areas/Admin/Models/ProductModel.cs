using Mango.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mango.Areas.Admin.Models
{
    public class ProductSearchModel : SearchModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int? CategoryId { get; set; }
    }
}
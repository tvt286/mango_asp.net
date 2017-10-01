using Mango.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Store.Areas.Admin.Models
{
    public class StoreProductSearchModel : SearchModel
    {
        public int? ProductId { get; set; }
        public int? CategoryId { get; set; }
        public int StoreId { get; set; }
    }
}
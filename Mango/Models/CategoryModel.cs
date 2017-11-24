using Mango.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mango.Models
{
    public class CategoryModel
    {
        public Category category { get; set; }
        public List<Menu1> menus1 { get; set; }

    }
}
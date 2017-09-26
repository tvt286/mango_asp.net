using Mango.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mango.Areas.Admin.Models
{
    public class GroupPermissionSearchModel :SearchModel
    {
        public string Name { get; set; }
    }
}
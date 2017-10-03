using Mango.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mango.Areas.Admin.Models
{
    public class StoreSearchModel : SearchModel
    {

        public string Code { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
    }

    public class StoreOrderImportSearchModel : SearchModel
    {
        public string Code { get; set; }
        public int? UserImport { get; set; }
        public int? RefStoreId { get; set; }
        public DateTime? TimeImportFrom { get; set; }
        public DateTime? TimeImportTo { get; set; }
    }

    public class StoreOrderExportSearchModel : SearchModel
    {
        public string Code { get; set; }
        public int? UserExport { get; set; }
        public int? StoreId { get; set; }
        public int? RefStoreId { get; set; }

        public DateTime? TimeExportFrom { get; set; }
        public DateTime? TimeExportTo { get; set; }
    }
}
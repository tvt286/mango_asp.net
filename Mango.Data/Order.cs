//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Mango.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class Order
    {
        public int Id { get; set; }
        public System.DateTime TimeCreate { get; set; }
        public Nullable<int> Status { get; set; }
        public int StoreId { get; set; }
        public string Code { get; set; }
        public long TotalAmount { get; set; }
    }
}

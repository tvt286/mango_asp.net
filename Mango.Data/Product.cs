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
    
    public partial class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            this.StoreOrderImportDetails = new HashSet<StoreOrderImportDetail>();
            this.StoreOrderExportDetails = new HashSet<StoreOrderExportDetail>();
            this.StoreProducts = new HashSet<StoreProduct>();
            this.OrderDetails = new HashSet<OrderDetail>();
        }
    
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public decimal SupplierPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public Nullable<int> AmountAlertForStore { get; set; }
        public Nullable<System.DateTime> TimeDeleted { get; set; }
        public System.DateTime TimeCreate { get; set; }
        public Nullable<System.DateTime> TimeUpdate { get; set; }
        public int UserCreateId { get; set; }
        public bool IsDeleted { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public Nullable<int> BuyCount { get; set; }
    
        public virtual User User { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StoreOrderImportDetail> StoreOrderImportDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StoreOrderExportDetail> StoreOrderExportDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StoreProduct> StoreProducts { get; set; }
        public virtual Category Category { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}

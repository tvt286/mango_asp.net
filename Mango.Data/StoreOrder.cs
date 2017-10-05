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
    
    public partial class StoreOrder
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public StoreOrder()
        {
            this.StoreOrderImportDetails = new HashSet<StoreOrderImportDetail>();
            this.StoreOrderExportDetails = new HashSet<StoreOrderExportDetail>();
        }
    
        public int Id { get; set; }
        public string Code { get; set; }
        public StoreOrderStatus Status { get; set; }
        public StoreImExTypeCode StoreImExTypeCode { get; set; }
        public Nullable<System.DateTime> TimeExport { get; set; }
        public Nullable<System.DateTime> TimeImport { get; set; }
        public string Note { get; set; }
        public Nullable<int> RefStoreId { get; set; }
        public Nullable<int> RefStoreOrderId { get; set; }
        public Nullable<int> UserImportId { get; set; }
        public Nullable<int> UserExportId { get; set; }
        public Nullable<int> StoreId { get; set; }
    
        public virtual Store StoreImport { get; set; }
        public virtual Store StoreExport { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StoreOrderImportDetail> StoreOrderImportDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StoreOrderExportDetail> StoreOrderExportDetails { get; set; }
        public virtual User UserExport { get; set; }
        public virtual User UserImport { get; set; }
    }
}

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
    
    public partial class Store
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public Nullable<int> CityId { get; set; }
        public Nullable<int> DistrictId { get; set; }
        public Nullable<int> WardId { get; set; }
        public Nullable<int> StreetId { get; set; }
        public string NumberStreet { get; set; }
        public string Address { get; set; }
        public System.DateTime TimeCreate { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<System.DateTime> TimeDeleted { get; set; }
        public int UserCreateId { get; set; }
    
        public virtual City City { get; set; }
        public virtual District District { get; set; }
        public virtual Street Street { get; set; }
        public virtual User User { get; set; }
        public virtual Ward Ward { get; set; }
    }
}
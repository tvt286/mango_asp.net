﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class mangoEntities : DbContext
    {
        public mangoEntities()
            : base("name=mangoEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<Group_Permission> Group_Permission { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<User_Permission> User_Permission { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<District> Districts { get; set; }
        public virtual DbSet<Street> Streets { get; set; }
        public virtual DbSet<Ward> Wards { get; set; }
        public virtual DbSet<StoreOrderExportDetail> StoreOrderExportDetails { get; set; }
        public virtual DbSet<StoreProduct> StoreProducts { get; set; }
        public virtual DbSet<Store> Stores { get; set; }
        public virtual DbSet<StoreOrderImportDetail> StoreOrderImportDetails { get; set; }
        public virtual DbSet<StoreOrder> StoreOrders { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<Product> Products { get; set; }
    }
}

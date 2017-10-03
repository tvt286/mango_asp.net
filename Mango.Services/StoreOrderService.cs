using Mango.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data;
using Mango.Common;
using System.Data.Entity.Validation;
using System.Threading;

namespace Mango.Services
{
     public class StoreOrderService
    {

         public static PagedSearchList<StoreOrder> SearchStoreOrderImport(string code,
           int? userImport, int? storeId, DateTime? timeImportFrom, DateTime? timeImportTo,
           int pageSize, int pageIndex)
         {
             var listExTypeCode = new List<StoreImExTypeCode>
            {
                StoreImExTypeCode.NhapTuKhoKhac,
            };

             using (var context = new mangoEntities(IsolationLevel.ReadUncommitted))
             {
                 var user = UserService.GetUserInfo();
                 IQueryable<StoreOrder> query = context.StoreOrders.Where(x => listExTypeCode.Contains(x.StoreImExTypeCode)).AsNoTracking();
              
                 if (code.NotEmpty())
                 {
                     query = query.Where(x => x.Code.Contains(code));
                 }

             
                 if (userImport.HasValue)
                 {
                     query = query.Where(x => x.UserImportId == userImport.Value);
                 }

                 if (storeId.HasValue)
                 {
                     query = query.Where(x => x.StoreId == storeId.Value);
                 }

                 if (timeImportFrom.HasValue)
                 {
                     query = query.Where(x => x.TimeImport >= timeImportFrom.Value);
                 }

                 if (timeImportTo.HasValue)
                 {
                     timeImportTo = timeImportTo.Value.AddDays(1).AddMinutes(-1);
                     query = query.Where(x => x.TimeImport <= timeImportTo.Value);
                 }

                 query = query.Include(x => x.UserImport).Include(x => x.Store)
                     .Include(x => x.RefStore)
                     .OrderByDescending(x => x.Id);
                 pageIndex = pageIndex < 1 ? 1 : pageIndex;
                 return new PagedSearchList<StoreOrder>(query, pageIndex, pageSize);
             }
         }

         public static PagedSearchList<StoreOrder> SearchStoreOrderExport(string code, int? userExport,
           int? storeId, int? refstoreId, DateTime? timeExportFrom, DateTime? timeExportTo,
           int pageSize, int pageIndex)
         {
             using (var context = new mangoEntities(IsolationLevel.ReadUncommitted))
             {
                 var user = UserService.GetUserInfo();
                 IQueryable<StoreOrder> query =
                     context.StoreOrders.Where(x => x.StoreImExTypeCode == StoreImExTypeCode.XuatKhoKhac).AsNoTracking();
              
                 if (code.NotEmpty())
                 {
                     query = query.Where(x => x.Code.Contains(code));
                 }

                 if (userExport.HasValue)
                 {
                     query = query.Where(x => x.UserExportId == userExport.Value);
                 }

                 if (storeId.HasValue)
                 {
                     query = query.Where(x => x.StoreId == storeId.Value);
                 }

                 if (refstoreId.HasValue)
                 {
                     query = query.Where(x => x.RefStoreId == refstoreId.Value);
                 }

                 if (timeExportFrom.HasValue)
                 {
                     query = query.Where(x => x.TimeExport >= timeExportFrom.Value);
                 }

                 if (timeExportTo.HasValue)
                 {
                     timeExportTo = timeExportTo.Value.AddDays(1).AddMinutes(-1);
                     query = query.Where(x => x.TimeExport <= timeExportTo.Value);
                 }

                 query =
                     query.Include(x => x.UserExport)
                         .Include(x => x.Store)
                         .Include(x => x.RefStore)
                         .OrderByDescending(x => x.Id);
                 pageIndex = pageIndex < 1 ? 1 : pageIndex;
                 return new PagedSearchList<StoreOrder>(query, pageIndex, pageSize);
             }
         }


         public static StoreOrder GetDetailStoreImport(int id)
         {
             using (var context = new mangoEntities(IsolationLevel.ReadUncommitted))
             {
                 return context.StoreOrders
                     .Include(x => x.StoreOrderImportDetails.Select(d => d.Product.Category))
                     .Include(x => x.StoreOrderImportDetails.Select(d => d.StoreOrderExportDetail))
                     .First(x => x.Id == id);
             }
         }

         public static string GenerateCode(StoreImExTypeCode? type,  int? storeId, int? refStoreId, bool warehouseCheck = false)
         {
             var code = string.Empty;
             var tableName = "StoreOrder";
             var month = DateTime.Now.Month;
             var day = DateTime.Now.Day;
             var year = DateTime.Now.Year;
             var strDate = DateTime.Now.ToString("yyMMdd");

             if (warehouseCheck)
             {
                 var storeCode = StoreService.Get(storeId.GetValueOrDefault()).Code;
                 code = string.Format("KK.{0}.{1}", storeCode, strDate);
             }

             else if (type == StoreImExTypeCode.NhapTuKhoKhac)
             {
                 var storeCode = StoreService.Get(storeId.GetValueOrDefault()).Code;
                 var refStoreCode = "NCC";

                 code = string.Format("A.{0}.{1}.{2}", refStoreCode, storeCode, strDate);
             }

             else if (type == StoreImExTypeCode.XuatKhoKhac)
             {
                 var storeCode = StoreService.Get(storeId.GetValueOrDefault()).Code;
                 var refStoreCode = StoreService.Get(refStoreId.GetValueOrDefault()).Code;

                 code = string.Format("X.{0}.{1}.{2}", refStoreCode, storeCode, strDate);
             }
     
             code = ProductService.GetNewCode(code, 0, tableName, true);
             return code;
         }


         public static void CreateWarehouseOrderImport(StoreOrder storeOrder,
           List<StoreOrderImportDetail> storeOrderImportDetailList)
         {
             var date = DateTime.Now;
             //var warehouseTransaction = new WarehouseTransaction
             //{
             //    Code = warehouseOrder.Code,
             //    CompanyId = warehouseOrder.CompanyId,
             //    ImExTypeCode = warehouseOrder.ImExTypeCode,
             //    Packages = warehouseOrder.Packages,
             //    WarehouseID = warehouseOrder.WarehouseID,
             //    WarehouseOrderId = warehouseOrder.Id,
             //    SupplierId = warehouseOrder.SupplierId,
             //    TimeCreate = date,
             //    UserCreateId = warehouseOrder.UserImportId.GetValueOrDefault()
             //};
             var storeProductList = new List<StoreProduct>();

             using (var context = new mangoEntities(IsolationLevel.ReadUncommitted))
             {
                 foreach (var storeOrderImportDetail in storeOrderImportDetailList)
                 {
                     if (!context.StoreProducts.Any(x => x.ProductId == storeOrderImportDetail.ProductId
                                                             && x.StoreId == storeOrder.StoreId) &&
                         !storeProductList.Any(x => x.ProductId == storeOrderImportDetail.ProductId
                                                        && x.StoreId == storeOrder.StoreId)
                     )
                     {
                         storeProductList.Add(new StoreProduct
                         {
                             ProductId = storeOrderImportDetail.ProductId,
                             QuantityExchange = 0,
                             StoreId = storeOrder.StoreId
                         });
                     }
                     storeOrder.StoreOrderImportDetails.Add(storeOrderImportDetail);
                     //warehouseTransaction.WarehouseTransactionDetails.Add(new WarehouseTransactionDetail
                     //{
                     //    ExpireDate = warehouseOrderImportDetail.ExpireDate,
                     //    Packages = warehouseOrderImportDetail.Packages,
                     //    ProductId = warehouseOrderImportDetail.ProductId,
                     //    Position = warehouseOrderImportDetail.Position,
                     //    QuantityChange = warehouseOrderImportDetail.Quantity,
                     //    QuantityExchangeChange = warehouseOrderImportDetail.QuantityExchange,
                     //    UnitDetailId = warehouseOrderImportDetail.UnitDetailId,
                     //    SupplierPrice = warehouseOrderImportDetail.SupplierPrice
                     //});
                 }

                 storeOrder.TimeImport = storeOrder.TimeExport = date;
                 //   warehouseOrder.TimeImport = date;
                 var user = UserService.GetUserInfo();
                 storeOrder.UserExportId = user.Id;
                 //storeOrder.UserCreateId = warehouseOrder.UserImportId.GetValueOrDefault();
                 storeOrder.Status = StoreOrderStatus.Completed;

                 context.StoreOrders.Add(storeOrder);
                 //context.WarehouseTransactions.Add(warehouseTransaction);
                 context.StoreProducts.AddRange(storeProductList);
                 //context.SaveChanges();
                 try
                 {
                     context.SaveChanges();
                 }
                 catch (DbEntityValidationException e)
                 {
                     var strError = new StringBuilder();
                     foreach (var eve in e.EntityValidationErrors)
                     {
                         strError.AppendFormat(
                             "Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                             eve.Entry.Entity.GetType().Name, eve.Entry.State);
                         foreach (var ve in eve.ValidationErrors)
                         {
                             strError.AppendFormat("- Property: \"{0}\", Error: \"{1}\"",
                                 ve.PropertyName, ve.ErrorMessage);
                         }
                     }
                     var temp = strError.ToString();
                     throw e;
                 }
             }

             new Thread(() =>
             {
                 Thread.CurrentThread.IsBackground = true;
                 UpdateQuantityStoreProduct(storeOrder.StoreId,
                 storeOrderImportDetailList.Select(x => x.ProductId).Distinct().ToArray());

             }).Start();

         }

         public static void UpdateQuantityStoreProduct(int storeId, int[] productIdList)
         {
             using (var context = new mangoEntities(IsolationLevel.ReadUncommitted))
             {
                 foreach (var productId in productIdList)
                 {
                     context.Database.ExecuteSqlCommand(
                         "exec sp_UpdateQuantityStoreProduct @productId={0}, @storeId={1}", productId, storeId);
                 }
             }
         }

    }
}

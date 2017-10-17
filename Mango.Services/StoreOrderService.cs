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
          int? userImport, int? refStoreId, DateTime? timeImportFrom, DateTime? timeImportTo,
          int pageSize, int pageIndex)
        {
            var listExTypeCode = new List<StoreImExTypeCode>
            {
                StoreImExTypeCode.NhapTuNCC,
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

                if (refStoreId.HasValue)
                {
                    query = query.Where(x => x.RefStoreId == refStoreId.Value);
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

                query = query.Include(x => x.UserImport).Include(x => x.StoreImport)
                    .Include(x => x.StoreExport)
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
                        .Include(x => x.StoreImport)
                        .Include(x => x.StoreExport)
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

        public static StoreOrder GetDetailStoreExport(int id)
        {
            using (var context = new mangoEntities(IsolationLevel.ReadUncommitted))
            {
                return context.StoreOrders
                    .Include(x => x.StoreOrderExportDetails.Select(d => d.Product.Category))
                    .Include(x => x.StoreOrderExportDetails.Select(d => d.StoreOrderImportDetail))
                    .First(x => x.Id == id);
            }
        }

        public static string GenerateCode(StoreImExTypeCode? type, int? storeId, int? refStoreId, int? customerId, bool warehouseCheck = false)
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
                var refstoreCode = StoreService.Get(refStoreId.GetValueOrDefault()).Code;
                var storeCode = "NCC";

                code = string.Format("A.{0}.{1}.{2}", storeCode, refstoreCode, strDate);
            }
            else if (type == StoreImExTypeCode.NhapTuNCC)
            {
                var refstoreCode = StoreService.Get(refStoreId.GetValueOrDefault()).Code;
                code = string.Format("N.{0}.NCC.{1}", refstoreCode, strDate);
            }

            else if (type == StoreImExTypeCode.XuatKhoKhac)
            {
                var storeCode = StoreService.Get(storeId.GetValueOrDefault()).Code;
                var refStoreCode = StoreService.Get(refStoreId.GetValueOrDefault()).Code;

                code = string.Format("X.{0}.{1}.{2}", storeCode, refStoreCode, strDate);
            }
            else
            if (type == StoreImExTypeCode.XuatBanKhachHang && storeId.HasValue)
            {
                tableName = "[Order]";
                var store = StoreService.Get(storeId.Value);
                var customer = new User();
                if (customerId.HasValue)
                {
                    customer = UserService.Get(customerId.Value);
                }
                code = string.Format("B.{0}.{1}.{2}", store.Code, customerId.HasValue ? customer.UserName : "KH", strDate);
            }
            code = ProductService.GetNewCode(code, 0, tableName, true);
            return code;
        }


        public static void CreateWarehouseOrderImport(StoreOrder storeOrder,
          List<StoreOrderImportDetail> storeOrderImportDetailList)
        {
            var date = DateTime.Now;

            var storeProductList = new List<StoreProduct>();

            using (var context = new mangoEntities(IsolationLevel.ReadUncommitted))
            {
                foreach (var storeOrderImportDetail in storeOrderImportDetailList)
                {
                    if (!context.StoreProducts.Any(x => x.ProductId == storeOrderImportDetail.ProductId
                                                            && x.StoreId == storeOrder.RefStoreId.Value) &&
                        !storeProductList.Any(x => x.ProductId == storeOrderImportDetail.ProductId
                                                       && x.StoreId == storeOrder.RefStoreId.Value)
                    )
                    {
                        storeProductList.Add(new StoreProduct
                        {
                            ProductId = storeOrderImportDetail.ProductId,
                            QuantityExchange = (int)storeOrderImportDetail.Quantity,
                            StoreId = storeOrder.RefStoreId.Value
                        });
                    }
                    storeOrder.StoreOrderImportDetails.Add(storeOrderImportDetail);

                }

                storeOrder.TimeImport = date;
                var user = UserService.GetUserInfo();
                storeOrder.UserImportId = user.Id;
                storeOrder.Status = StoreOrderStatus.Completed;
                context.StoreOrders.Add(storeOrder);
                context.StoreProducts.AddRange(storeProductList);
                context.SaveChanges();
                //try
                //{
                //    context.SaveChanges();
                //}
                //catch (DbEntityValidationException e)
                //{
                //    var strError = new StringBuilder();
                //    foreach (var eve in e.EntityValidationErrors)
                //    {
                //        strError.AppendFormat(
                //            "Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                //            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                //        foreach (var ve in eve.ValidationErrors)
                //        {
                //            strError.AppendFormat("- Property: \"{0}\", Error: \"{1}\"",
                //                ve.PropertyName, ve.ErrorMessage);
                //        }
                //    }
                //    var temp = strError.ToString();
                //    throw e;
                //}
            }

            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                UpdateQuantityStoreProduct(storeOrder.RefStoreId.Value,
                storeOrderImportDetailList.Select(x => x.ProductId).Distinct().ToArray());

            }).Start();

        }

        public static RedirectCommand CreateStoreOrderExport(StoreOrder storeOrderExport,
          List<StoreOrderExportDetail> storeOrderExportDetailList)
        {
            var result = new RedirectCommand
            {
                Message = "Đã tạo lệnh xuất kho thành công!",
                Code = ResultCode.Success
            };

            var date = DateTime.Now;

            using (var context = new mangoEntities(IsolationLevel.ReadUncommitted))
            {

                var storeOrderImport = new StoreOrder
                {
                    Code = GenerateCode(StoreImExTypeCode.NhapTuKhoKhac, storeOrderExport.StoreId,
                            storeOrderExport.RefStoreId, null),
                    StoreId = storeOrderExport.StoreId,
                    RefStoreId = storeOrderExport.RefStoreId.GetValueOrDefault(0),
                    UserExportId = storeOrderExport.UserExportId,
                    TimeImport = date,
                    StoreImExTypeCode = StoreImExTypeCode.NhapTuKhoKhac,
                    Status = StoreOrderStatus.Pending
                };
                var number = 1;
                foreach (var storeOrderExportDetail in storeOrderExportDetailList)
                {

                    storeOrderExportDetail.Quantity = storeOrderExportDetail.Quantity;

                    var refstoreOrderImportDetail =
                        context.StoreOrderImportDetails.Include(x => x.Product).First(
                            x => x.Id == storeOrderExportDetail.RefStoreOrderImportDetailId);

                    if (refstoreOrderImportDetail.Quantity <
                        storeOrderExportDetail.Quantity)
                    {
                        result.Message = string.Format("Dòng thứ {0} đã nhập quá số lượng còn lại {1}",
                            number, refstoreOrderImportDetail.Product.Name);
                        result.Code = ResultCode.Fail;
                        return result;
                    }
                    number++;

                    refstoreOrderImportDetail.Quantity -= storeOrderExportDetail.Quantity;

                    storeOrderExport.StoreOrderExportDetails.Add(storeOrderExportDetail);

                    var storeOrderImportDetail = new StoreOrderImportDetail
                    {
                        ProductId = storeOrderExportDetail.ProductId,
                        Quantity = storeOrderExportDetail.Quantity,
                    };

                    storeOrderImportDetail.MainSupplierPrice = refstoreOrderImportDetail.MainSupplierPrice;
                    storeOrderImport.StoreOrderImportDetails.Add(storeOrderImportDetail);
                }

                storeOrderExport.Status = StoreOrderStatus.Completed;

                context.StoreOrders.Add(storeOrderExport);
                context.StoreOrders.Add(storeOrderImport);
                //   context.SaveChanges();

                var listStoreExportDetail = storeOrderExport.StoreOrderExportDetails.ToList();
                var listStoreImportDetail = storeOrderImport.StoreOrderImportDetails.ToList();
                for (int i = 0; i < listStoreExportDetail.Count; i++)
                {
                    listStoreImportDetail[i].RefStoreOrderExportDetailId = listStoreExportDetail[i].Id;
                }
                context.SaveChanges();
            }

            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                UpdateQuantityStoreProduct(storeOrderExport.StoreId.Value, storeOrderExportDetailList.Select(x => x.ProductId).Distinct().ToArray());

            }).Start();

            return result;
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
        public static List<StoreOrderImportDetail> GetStoreOrderImportDetail(int? productId, int storeId)
        {
            var imExTypeCodeList = new List<StoreImExTypeCode>
            {
                StoreImExTypeCode.NhapTuNCC,
                StoreImExTypeCode.NhapTuKhoKhac
            };
            using (var context = new mangoEntities(IsolationLevel.ReadUncommitted))
            {
                IQueryable<StoreOrderImportDetail> query =
                    context.StoreOrderImportDetails
                        .Include(x => x.Product)
                        .Include(x => x.Product.Category)
                        .Where(x => x.StoreOrder.RefStoreId == storeId &&
                                imExTypeCodeList.Contains(x.StoreOrder.StoreImExTypeCode) && x.Quantity > 0);
                if (productId.HasValue)
                {
                    query = query.Where(x => x.ProductId == productId.Value);
                }
                return query.AsNoTracking().ToList();
            }
        }

        public static RedirectCommand ImportStoreOrderFromOtherStore(StoreOrder storeOrder,
        int[] storeOrderImportDetailIdList, int[] quantityRequestImport, string[] note)
        {
            var result = new RedirectCommand
            {
                Message = "Đã xác nhận nhập kho từ kho khác thành công!",
                Code = ResultCode.Success
            };
            var date = DateTime.Now;

            var storeProductList = new List<StoreProduct>();

            using (var context = new mangoEntities(IsolationLevel.ReadCommitted))
            {
                for (int i = 0; i < storeOrderImportDetailIdList.Length; i++)
                {
                    var storeOrderImportDetailId = storeOrderImportDetailIdList[i];

                    var storeOrderImportDetail =
                        context.StoreOrderImportDetails.Include(x => x.Product)
                            .First(x => x.Id == storeOrderImportDetailId);

                    if (storeOrderImportDetail.Quantity != quantityRequestImport[i])
                    {
                        result.Code = ResultCode.Fail;
                        result.Message = string.Format("Vui lòng kiểm tra số lượng nhập của sp: {0}",
                            storeOrderImportDetail.Product.Name);
                        return result;
                    }
                    storeOrderImportDetail.Quantity = quantityRequestImport[i];

                    storeOrderImportDetail.Note = note[i];

                    if (!context.StoreProducts.Any(x => x.ProductId == storeOrderImportDetail.ProductId
                                                            && x.StoreId == storeOrder.RefStoreId) &&
                        !storeProductList.Any(x => x.ProductId == storeOrderImportDetail.ProductId
                                                       && x.StoreId == storeOrder.RefStoreId)
                    )
                    {
                        storeProductList.Add(new StoreProduct
                        {
                            ProductId = storeOrderImportDetail.ProductId,
                            QuantityExchange = 0,
                            StoreId = storeOrder.RefStoreId.Value
                        });
                    }


                }
                if (storeOrder.TimeImport.Value.ToString("dd-MM-yyyy").Contains(date.ToString("dd-MM-yyyy")))
                    storeOrder.TimeImport = date;
                else
                {
                    TimeSpan ts = new TimeSpan(date.Hour, date.Minute, 0);
                    storeOrder.TimeImport = storeOrder.TimeImport.Value.Date + ts;
                }
                storeOrder.Status = StoreOrderStatus.Completed;
                context.StoreOrders.Attach(storeOrder);
                context.Entry(storeOrder).State = EntityState.Modified;
                context.Entry(storeOrder).Property(x => x.TimeExport).IsModified = false;
                //context.Entry(storeOrder).Property(x => x.UserExport).IsModified = false;
                context.StoreProducts.AddRange(storeProductList);
                context.SaveChanges();
            }

            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                UpdateQuantityStoreProduct(storeOrder.RefStoreId.Value,
                storeOrder.StoreOrderImportDetails.Select(x => x.ProductId).Distinct().ToArray());
            }).Start();

            return result;
        }
    }
}

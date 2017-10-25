using Mango.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mango.Common;
using System.Threading;

namespace Mango.Services
{
    public class OrderService
    {
        public static PagedSearchList<Order> Search(string code, int? customerId,
           DateTime? fromDate, DateTime? toDate,
           int pageSize, int pageIndex)
        {
            using (var context = new mangoEntities(IsolationLevel.ReadUncommitted))
            {
                var user = UserService.GetUserInfo();
                IQueryable<Order> query = context.Orders.AsNoTracking();
            
                if (code.NotEmpty())
                {
                    query = query.Where(x => x.Code.Contains(code));
                }

                if (fromDate.HasValue)
                {
                    query = query.Where(x => x.TimeCreate >= fromDate.Value);
                }

                if (toDate.HasValue)
                {
                    toDate = toDate.Value.AddDays(1).AddMinutes(-1);
                    query = query.Where(x => x.TimeCreate <= toDate.Value);
                }

                if (customerId.HasValue)
                {
                    query = query.Where(x => x.CustomerId == customerId);
                }

                query = query.Include(x => x.Customer).Include(x => x.Store).OrderByDescending(x => x.Id);
                pageIndex = pageIndex < 1 ? 1 : pageIndex;
                return new PagedSearchList<Order>(query, pageIndex, pageSize);
            }
        }

        public static Order Get(int id)
        {
            using (var context = new mangoEntities(IsolationLevel.ReadUncommitted))
            {              
                    return context.Orders
                        .Include(x => x.OrderDetails.Select(d => d.StoreOrderImportDetail))
                        .Include(x => x.OrderDetails.Select(d => d.Product.Category))
                        .Include(x => x.Customer)
                        .First(x => x.Id == id);             
            }
        }

        public static void ConfirmOrder(int id)
        {
            using (var context = new mangoEntities(IsolationLevel.ReadUncommitted))
            {
                var order =  context.Orders
                    .Include(x => x.OrderDetails.Select(d => d.StoreOrderImportDetail))
                    .First(x => x.Id == id);

                order.Status = OrderStatus.Pending;
                foreach (var item in order.OrderDetails)
                {
                    var refStoreOrderImportDetail = context.StoreOrderImportDetails.Include(x => x.Product).First(
                          x => x.Id == item.RefStoreOrderImportDetailId);
                    refStoreOrderImportDetail.Quantity -= item.Quantity; // trừ số lượng trong kho

                }
               
                context.SaveChanges();

                StoreOrderService.UpdateQuantityStoreProduct(order.StoreId,
                order.OrderDetails.Select(x => x.ProductId).Distinct().ToArray());
            }
        }

        public static void setCompleteOrder(int id)
        {
            using (var context = new mangoEntities(IsolationLevel.ReadUncommitted))
            {
                var order = context.Orders
                    .First(x => x.Id == id);
                order.Status = OrderStatus.Complete;
               
                context.SaveChanges();

            }
        }

        public static RedirectCommand CreateOrder(Order order,
          List<OrderDetail> orderDetailList)
        {
            var result = new RedirectCommand
            {
                Message = "Create order export successfully!",
                Code = ResultCode.Success
            };
            var user = UserService.GetUserInfo();
            var date = DateTime.Now;
          
            using (var context = new mangoEntities(IsolationLevel.ReadUncommitted))
            {
                var number = 1;
                foreach (var orderDetail in orderDetailList)
                {
                  
                     var refStoreOrderImportDetail = context.StoreOrderImportDetails.Include(x => x.Product).First(
                            x => x.Id == orderDetail.RefStoreOrderImportDetailId);

                     orderDetail.MainSupplierPrice = refStoreOrderImportDetail.MainSupplierPrice;

                     orderDetail.Product = refStoreOrderImportDetail.Product;
                     if (refStoreOrderImportDetail.Quantity < orderDetail.Quantity)
                        {
                            result.Message = string.Format("Line {0} import quantity > quantity remaining {1}",
                                number,
                                refStoreOrderImportDetail.Product.Name);
                            result.Code = ResultCode.Fail;
                            return result;
                        }
                    number++;
                  

                 //   refStoreOrderImportDetail.Quantity -= orderDetail.Quantity; // trừ số lượng trong kho
                   
                    //refStoreOrderImportDetail.QuantityExchangeExport += orderDetail.QuantityExchange;

                    orderDetail.SupplierPrice = refStoreOrderImportDetail.SupplierPrice;


                    order.OrderDetails.Add(orderDetail);
                }

                
                order.TotalAmount = (long)order.OrderDetails.Sum(x => x.Quantity * x.SellingPrice);
                context.Orders.Add(order);
                context.SaveChanges();
            }

            //new Thread(() =>
            //{
            //    Thread.CurrentThread.IsBackground = true;
            //    StoreOrderService.UpdateQuantityStoreProduct(order.StoreId,
            //        orderDetailList.Select(x => x.ProductId).Distinct().ToArray());
               
            //}).Start();

            return result;
        }
    }
}

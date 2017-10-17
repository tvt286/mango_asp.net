﻿using Mango.Data;
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
                        .Include(x => x.OrderDetails.Select(d => d.Product.Category))
                        .Include(x => x.Customer)
                        .First(x => x.Id == id);             
            }
        }

        public static RedirectCommand CreateOrder(Order order,
          List<OrderDetail> orderDetailList)
        {
            var result = new RedirectCommand
            {
                Message = "Đã tạo lệnh xuất kho thành công!",
                Code = ResultCode.Success
            };
            var user = UserService.GetUserInfo();
            var date = DateTime.Now;
          
            using (var context = new mangoEntities(IsolationLevel.ReadUncommitted))
            {
                var number = 1;
                foreach (var orderDetail in orderDetailList)
                {
                  
                     var refStoreOrderImportDetail =
                        context.StoreOrderImportDetails.Include(x => x.Product).First(
                            x => x.Id == orderDetail.RefStoreOrderImportDetailId);

                     orderDetail.MainSupplierPrice = refStoreOrderImportDetail.MainSupplierPrice;

                     orderDetail.Product = refStoreOrderImportDetail.Product;
                     if (refStoreOrderImportDetail.Quantity < orderDetail.Quantity)
                    {
                        result.Message = string.Format("Dòng thứ {0} đã nhập quá số lượng còn lại {1}",
                            number,
                            refStoreOrderImportDetail.Product.Name);
                        result.Code = ResultCode.Fail;
                        return result;
                    }
                    number++;
                  

                 //   refStoreOrderImportDetail.Quantity -= orderDetail.Quantity;
                   
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
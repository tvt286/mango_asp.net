using Mango.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mango.Common;

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

                query = query.Include(x => x.Customer).OrderByDescending(x => x.Id);
                pageIndex = pageIndex < 1 ? 1 : pageIndex;
                return new PagedSearchList<Order>(query, pageIndex, pageSize);
            }
        }

    }
}

using Mango.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Entity;

namespace Mango.Services
{
    public class StoreProductService
    {
        public static PagedSearchList<StoreProduct> Search(int? productId, int? categoryId, int StoreId,
          int pageSize, int pageIndex)
        {
            using (var context = new mangoEntities(IsolationLevel.ReadUncommitted))
            {
                IQueryable<StoreProduct> query = context.StoreProducts.AsNoTracking().Where(x => x.StoreId == StoreId);

                if (productId.HasValue)
                {
                    query = query.Where(x => x.ProductId == productId);
                }

                if (categoryId.HasValue)
                {
                    query = query.Where(x => x.Product.CategoryId == categoryId);
                }

                query = query.Include(x => x.Product.Category).Include(x => x.Product).OrderByDescending(x => x.QuantityExchange);
                pageIndex = pageIndex < 1 ? 1 : pageIndex;
                return new PagedSearchList<StoreProduct>(query, pageIndex, pageSize);
            }
        }

    }
}

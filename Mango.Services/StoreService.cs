using Mango.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Entity;
using Mango.Common;

namespace Mango.Services
{
    public class StoreService
    {
        public static Store Get(int id)
        {
            using (var context = new mangoEntities(IsolationLevel.ReadUncommitted))
            {
                var user = UserService.GetUserInfo();
                return context.Stores.Include(x => x.StoreOrders.Select(s => s.StoreOrderImportDetails)).Include(x => x.StoreProducts.Select(p => p.Product)).FirstOrDefault(x => x.Id == id);
            }
        }

        public static void CheckProductOutStock()
        {
            using (var context = new mangoEntities(IsolationLevel.ReadUncommitted))
            {
                var stores = GetAll();
                foreach (var store in stores)
                {
                    foreach (var item in store.StoreProducts)
                    {
                        if(item.QuantityExchange <= item.Product.AmountAlertForStore)
                        {

                        }
                    }
                }
            }
        }

        public static Store GetStoreRoot()
        {
            using (var context = new mangoEntities(IsolationLevel.ReadUncommitted))
            {
                var user = UserService.GetUserInfo();
                return context.Stores.Include(x => x.StoreProducts).FirstOrDefault(x => x.IsRoot == true);
            }
        }

        public static List<Store> GetAll(bool includeRoot = false)
        {
            using (var context = new mangoEntities(IsolationLevel.ReadUncommitted))
            {
                if(includeRoot)
                    return context.Stores.AsNoTracking().ToList();
                return context.Stores.Where(x=> x.IsRoot == false).Include(x => x.StoreProducts.Select(a => a.Product)).AsNoTracking().ToList();
            }
        }

        public static RedirectCommand Create(Store data)
        {
            var result = new RedirectCommand
            {
                Code = ResultCode.Success,
                Message = "Create store successfully!"
            };

            using (var context = new mangoEntities())
            {
                var user = UserService.GetUserInfo();
                data.UserCreateId = user.Id;
                data.TimeCreate = DateTime.Now;
                data.Address = LocationService.GetAdress(data.NumberStreet, data.WardId, data.StreetId, data.DistrictId, data.CityId);
                // lay Lat va Lng
                DataTable location = new DataTable();
                location = Utility.FindCoordinates(data.Address);
                if (location != null)
                {
                    foreach (DataRow row in location.Rows)
                    {
                        data.Lat = row["Latitude"].ToString();
                        data.Lng = row["Longitude"].ToString();
                    }
                }
                context.Stores.Add(data);
                context.SaveChanges();
            }

            return result;

        }

        public static void Update(Store data)
        {
            using (var context = new mangoEntities())
            {
                data.Address = LocationService.GetAdress(data.NumberStreet, data.WardId, data.StreetId, data.DistrictId, data.CityId);
                // lay Lat va Lng
                DataTable location = new DataTable();
                location = Utility.FindCoordinates(data.Address);
                if (location != null)
                {
                    foreach (DataRow row in location.Rows)
                    {
                        data.Lat = row["Latitude"].ToString();
                        data.Lng = row["Longitude"].ToString();
                    }
                }
                context.Stores.Attach(data);
                context.Entry(data).State = EntityState.Modified;
                context.Entry(data).Property(x => x.TimeCreate).IsModified = false;
                context.SaveChanges();
            }

        }

        public static bool Has(string code)
        {
            using (var context = new mangoEntities(IsolationLevel.ReadUncommitted))
            {
                return context.Stores.Any(x => x.Code == code);
            }
        }

        public static PagedSearchList<Store> Search(string adress, string Code, string vendingName,int pageSize, int pageIndex)
        {
            using (var context = new mangoEntities(IsolationLevel.ReadUncommitted))
            {
                IQueryable<Store> query = context.Stores.Where(x => x.IsRoot == false && x.IsDeleted == false).AsNoTracking();

                if (vendingName.NotEmpty())
                {
                    query = query.Where(x => x.Name.Contains(vendingName));
                }
          
                if (Code.NotEmpty())
                {
                    query = query.Where(x => x.Code.Contains(Code));
                }

                if (adress.NotEmpty())
                {
                    query = query.Where(x => x.Address.Contains(adress));

                }

              
                query = query.OrderByDescending(x => x.Id);
                query = query.Include(x => x.Ward).OrderByDescending(x => x.Id);
                query = query.Include(x => x.Street).OrderByDescending(x => x.Id);
                query = query.Include(x => x.District).OrderByDescending(x => x.Id);
                query = query.Include(x => x.City).OrderByDescending(x => x.Id);
                pageIndex = pageIndex < 1 ? 1 : pageIndex;

                return new PagedSearchList<Store>(query, pageIndex, pageSize);
            }
        }

        public static List<Store> GetStoreHasProduct(bool includeRoot = false, int? productId = null, int? quantity = null)
        {
            using (var context = new mangoEntities(IsolationLevel.ReadUncommitted))
            {
                var ownStoreIdList = GetAll(includeRoot).Select(x => x.Id).ToList();
                if (productId.HasValue && quantity.HasValue)
                {
                    return
                    context.StoreProducts.Where(x => ownStoreIdList.Contains(x.StoreId) && x.ProductId == productId.Value
                    && x.QuantityExchange > quantity)
                        .Select(x => x.Store)
                        .Include(x => x.Street)
                        .Include(x => x.District)
                        .Include(x => x.Ward)
                        .Include(x => x.City)
                        .ToList();
                }
                return
                    context.StoreProducts.Where(x => ownStoreIdList.Contains(x.StoreId)
                    && x.QuantityExchange > 0)
                        .Select(x => x.Store)
                        .ToList();
            }
        }

        public static Store GetStoreOrderForUser(User user)
        {
            using (var context = new mangoEntities(IsolationLevel.ReadUncommitted))
            {
                var stores = GetAll();
                var store = stores.First();
                var avgLat = float.Parse(stores.Max(x => x.Lat));
                var avgLng = float.Parse(stores.Max(x => x.Lng));
                if (stores.Count != 0)
                {
                    foreach (var item in stores)
                    {
                        if (item.Lat != null && item.Lng != null && (avgLat > Math.Abs(float.Parse(item.Lat) - float.Parse(user.Lat))) && (avgLng > Math.Abs(float.Parse(item.Lng) - float.Parse(user.Lng))))
                        {
                            avgLat = Math.Abs(float.Parse(item.Lat) - float.Parse(user.Lat));
                            avgLng = Math.Abs(float.Parse(item.Lng) - float.Parse(user.Lng));
                            store = item;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    return store;
                }
                else
                {
                    return context.Stores.Include(x => x.StoreProducts.Select(a => a.Product)).FirstOrDefault(x => x.IsRoot == true);
                }
            }
        }

        public static RedirectCommand DeleteStore(string categoryIdStr)
        {
            var result = new RedirectCommand
            {
                Code = ResultCode.Success,
                Message = "Delete successfully!",
            };
            using (var context = new mangoEntities())
            {
                var categoryIdList =
                    categoryIdStr.Replace(" ", "").Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                foreach (var categoryId in categoryIdList)
                {
                    var Id = int.Parse(categoryId);
                    var store = context.Stores.FirstOrDefault(x => x.Id == Id);
                    var products = context.StoreProducts.Where(x => x.StoreId == store.Id).ToList();
                    if (products.Count > 0)
                    {
                        result.Code = ResultCode.Fail;
                        result.Message = "Store này có sản phẩm không thể xóa được!";
                        return result;
                    }
                    store.IsDeleted = true;
                    store.TimeDeleted = DateTime.Now;
                }
                context.SaveChanges();
            }

            return result;
        }
    }
}

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
                return context.Stores.FirstOrDefault(x => x.Id == id);
            }
        }


        public static List<Store> GetAll()
        {
            using (var context = new mangoEntities(IsolationLevel.ReadUncommitted))
            {
                return context.Stores.AsNoTracking().ToList();
            }
        }

        public static RedirectCommand Create(Store data)
        {
            var result = new RedirectCommand
            {
                Code = ResultCode.Success,
                Message = "Đã tạo mới store thành công"
            };

            using (var context = new mangoEntities())
            {
                var user = UserService.GetUserInfo();
                data.UserCreateId = user.Id;
                data.TimeCreate = DateTime.Now;
                data.Address = LocationService.GetAdress(data.NumberStreet, data.WardId, data.StreetId, data.DistrictId, data.CityId);

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
                IQueryable<Store> query = context.Stores.AsNoTracking();

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


    }
}

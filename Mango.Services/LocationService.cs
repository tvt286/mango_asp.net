using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mango.Data;
using System.Data.Entity;
using System.Data;

namespace Mango.Services
{
    public class LocationService
    {

        public static string GetAdress(string StreetNumber, int? wardId, int? streetId, int? districtId, int? cityId)
        {
            using (var context = new mangoEntities(IsolationLevel.ReadUncommitted))
            {
                Ward ward = context.Wards.First(x => x.Id == wardId);
                Street street = context.Streets.First(x => x.Id == streetId);
                City city = context.Cities.First(x => x.Id == cityId);
                District district = context.Districts.First(x => x.Id == districtId);
                return string.Format(StreetNumber + ", Đường " + street.Name + ", " + ward.Prefix + " " + ward.Name + ", " + district.Prefix + " " + district.Name + ", " + city.Prefix + " " + city.Name);
            }
        }


        public static List<City> GetAllCity()
        {
            using (var context = new mangoEntities(IsolationLevel.ReadUncommitted))
            {
                return context.Cities.AsNoTracking().ToList();
            }
        }

        public static List<District> GetDistrictByCity(int cityId)
        {
            using (var context = new mangoEntities(IsolationLevel.ReadUncommitted))
            {
                return context.Districts.Where(x => x.CityId == cityId).OrderBy(x => x.Name).AsNoTracking().ToList();
            }
        }

        public static List<Ward> GetWardByDistrict(int districtId)
        {
            using (var context = new mangoEntities(IsolationLevel.ReadUncommitted))
            {
                return context.Wards.Where(x => x.DistrictId == districtId).OrderBy(x => x.Name).AsNoTracking().ToList();
            }
        }

        public static List<Street> GetStreetByDistrictAndCity(int cityId, int dictrictId)
        {
            using (var context = new mangoEntities(IsolationLevel.ReadUncommitted))
            {
                return context.Streets.Where(x => x.DistrictId == dictrictId && x.CityId == cityId).OrderBy(x => x.Name).AsNoTracking().ToList();
            }
        }

        public static Ward GetWard(int wardId)
        {
            using (var context = new mangoEntities(IsolationLevel.ReadUncommitted))
            {
                return context.Wards.First(x => x.Id == wardId);
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mango.Data;
using System.Data.Entity;
using System.Data;
using Mango.Common;

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

        public static District GetDistrict(int dictrictId)
        {
            using (var context = new mangoEntities(IsolationLevel.ReadUncommitted))
            {
                return context.Districts.First(x => x.Id == dictrictId);
            }
        }

        public static City GetCity(int cityId)
        {
            using (var context = new mangoEntities(IsolationLevel.ReadUncommitted))
            {
                return context.Cities.First(x => x.Id == cityId);
            }
        }

        public static Street GetStreet(int streetId)
        {
            using (var context = new mangoEntities(IsolationLevel.ReadUncommitted))
            {
                return context.Streets.First(x => x.Id == streetId);
            }
        }

        public static string BuildAddress(int? cityId, int? districtId, int? wardId, int? streetId, string numberStreet = "", string streetName = "")
        {
            City city = cityId.HasValue ? GetCity(cityId.Value) : null;
            Ward ward = wardId.HasValue ? GetWard(wardId.Value) : null;
            District district = districtId.HasValue ? GetDistrict(districtId.Value) : null;
            Street street = streetId.HasValue ? GetStreet(streetId.Value) : null;

            var listAddressStr = new List<string>
            {
                streetId.HasValue ? string.Format("{0} Đường {1}", numberStreet, street.Name) : streetName,
                wardId.HasValue ? string.Format("{0} {1}", ward.Prefix, ward.Name) : "",
                districtId.HasValue ? string.Format("{0} {1}", district.Prefix, district.Name) : "",
                cityId.HasValue ? string.Format("{0} {1}", city.Prefix, city.Name) : ""
            };
            var address = string.Join(", ", listAddressStr.Where(x => x.NotEmpty()));
            return address;
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

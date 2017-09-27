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
    public class ProductService
    {
        public static Product Get(int id)
        {
            using (var context = new mangoEntities(IsolationLevel.ReadUncommitted))
            {
                var query = context.Products.Include(x => x.Category).Where(x => x.Id == id);
                return query.First();
            }
        }

        public static Product GetByCode(string Code)
        {
            using (var context = new mangoEntities(IsolationLevel.ReadUncommitted))
            {
                var query = context.Products.Where(x => x.Code == Code);
                return query.First();
            }
        }

        public static RedirectCommand Create(Product data)
        {
            var result = new RedirectCommand
            {
                Code = ResultCode.Success,
                Message = "Đã tạo sản phẩm thành công!"
            };
            using (var context = new mangoEntities())
            {
                var user = UserService.GetUserInfo();
                var category = context.Categories.First(x => x.Id == data.CategoryId);
                data.Code = category.Code + data.Code;

                if (context.Products.Any(x => x.Code == data.Code))
                {
                    result.Code = ResultCode.Fail;
                    result.Message = "Mã này đã tồn tại trong hệ thống rồi, vui lòng kiểm tra lại!";
                    return result;
                }
                data.UserCreateId = user.Id;
                data.TimeCreate = DateTime.Now;
                context.Products.Add(data);
                context.SaveChanges();
            }
            return result;
        }

        public static RedirectCommand Update(Product data)
        {
            var result = new RedirectCommand
            {
                Code = ResultCode.Success,
                Message = "Đã cập nhật sản phẩm thành công!",
            };
            using (var context = new mangoEntities())
            {
                if (context.Products.Any(x => x.Code == data.Code && x.Id != data.Id))
                {
                    result.Code = ResultCode.Fail;
                    result.Message = "Mã này đã tồn tại trong hệ thống rồi, vui lòng kiểm tra lại!";
                    return result;
                }

                data.TimeUpdate = DateTime.Now;
                context.Products.Attach(data);
                context.Entry(data).State = EntityState.Modified;
                context.Entry(data).Property(x => x.TimeCreate).IsModified = false;
                context.Entry(data).Property(x => x.UserCreateId).IsModified = false;
                context.SaveChanges();
            }
            return result;
        }

        public static string GetNewCode(string codeOld, int id, string table, bool hasPad = false, int? companyId = null)
        {
            using (var context = new mangoEntities(IsolationLevel.ReadUncommitted))
            {
                if (companyId == null)
                {
                    var user = UserService.GetUserInfo();
                }
                List<string> listCode;
                if (id == 0)
                {
                    listCode =
                        context.Database.SqlQuery<string>(
                            string.Format("select Code from {2} where CompanyId = {0} and Code like '{1}%'", companyId, codeOld, table)).ToList();
                }
                else
                {
                    listCode =
                            context.Database.SqlQuery<string>(
                                string.Format("select Code from {3} where CompanyId = {0} and Code like '{1}%' and Id != {2}", companyId, codeOld, id, table)).ToList();
                }
                for (int i = 1; i < 1000; i++)
                {
                    var codeNew = codeOld + i;
                    if (hasPad)
                    {
                        codeNew = codeOld + "." + i.ToString().PadLeft(4, '0');
                    }
                    if (!listCode.Contains(codeNew))
                    {
                        return codeNew;
                    }
                }
                Random rnd = new Random();

                return codeOld + rnd.Next(1, 100000);
            }
        }

        public static PagedSearchList<Product> Search(string code, string name, int? categoryId,
            int pageSize, int pageIndex)
        {
            using (var context = new mangoEntities(IsolationLevel.ReadUncommitted))
            {
                var user = UserService.GetUserInfo();
                IQueryable<Product> query = context.Products.Where(x => x.IsDeleted == false).AsNoTracking();
          
                if (code.NotEmpty())
                {
                    query = query.Where(x => x.Code.Contains(code));
                }

                if (name.NotEmpty())
                {
                    query = query.Where(x => x.Name.Contains(name));
                }

                if (categoryId.HasValue)
                {
                    query = query.Where(x => x.CategoryId == categoryId.Value);
                }

                query = query.Include(x => x.Category).OrderByDescending(x => x.Id);

                pageIndex = pageIndex < 1 ? 1 : pageIndex;
                return new PagedSearchList<Product>(query, pageIndex, pageSize);
            }
        }

       

       
        public static bool HasProduct(string code, int? companyId)
        {
            using (var context = new mangoEntities(IsolationLevel.ReadUncommitted))
            {
                return context.Products.Any(x =>  x.IsDeleted == false && x.Code == code);
            }
        }

        public static RedirectCommand DeleteProduct(string productIdStr)
        {
            var result = new RedirectCommand
            {
                Code = ResultCode.Success,
                Message = "Đã xóa sản phẩm thành công!",
            };
            using (var context = new mangoEntities(IsolationLevel.ReadUncommitted))
            {
                var productIdList =
                    productIdStr.Replace(" ", "").Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                foreach (var productId in productIdList)
                {
                    var product = context.Products.First(x => x.Id == int.Parse(productId));
                    //if (context.WarehouseProducts
                    //    .Any(x => x.ProductId == product.Id
                    //    && x.CompanyId == product.CompanyId
                    //    && (x.QuantityExchange > 0 ||
                    //    x.QuantityExchangeFail > 0 ||
                    //    x.QuantityExchangeLose > 0)))
                    //{
                    //    result.Code = ResultCode.Fail;
                    //    result.Message = "Sản phẩm này còn tồn trong kho không thể xóa được!";
                    //    return result;
                    //}

                    product.IsDeleted = true;
                    product.TimeDeleted = DateTime.Now;
                }
                context.SaveChanges();
            }

            return result;
        }

        //public static List<Product> GetTopProductBuyMore(int companyId)
        //{
        //    using (var context = new vendingEntities(IsolationLevel.ReadUncommitted))
        //    {
        //        return
        //           context.Database.SqlQuery<Product>("exec sp_GetTopProductBuyMore @companyId={0}",
        //               companyId).ToList();
        //    }
        //}

    }
}

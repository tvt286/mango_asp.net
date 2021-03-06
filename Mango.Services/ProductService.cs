﻿using Mango.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Entity;
using Mango.Common;
using System.Data.Entity.Validation;

namespace Mango.Services
{
    public class ProductService
    {
        public static Product Get(int id, bool includeDetail = false)
        {
            using (var context = new mangoEntities(IsolationLevel.ReadUncommitted))
            {
                if(!includeDetail)
                    return context.Products.First(x => x.IsDeleted == false && x.Id == id);

                return context.Products.Include(x => x.Category).First(x => x.IsDeleted == false && x.Id == id);
            }
        }

        public static List<Product> GetAll()
        {
            using (var context = new mangoEntities(IsolationLevel.ReadUncommitted))
            {
                return context.Products.Where(x => x.IsDeleted == false).Include(x => x.Category).AsNoTracking().ToList();
            }
        }

        // lấy ra danh sách sản phẩm bán được nhiều
        public static List<Product> GetProductsHot()
        {
            using (var context = new mangoEntities(IsolationLevel.ReadUncommitted))
            {
                return context.Products.Where(x => x.IsDeleted == false).OrderByDescending(x => x.BuyCount).AsNoTracking().Take(4).ToList();
            }
        }

        // lấy ra sản phẩm bản chạy của menu hiển thị ở home
        public static List<Product> GetProductsInHome(int menuId)
        {
            using (var context = new mangoEntities(IsolationLevel.ReadUncommitted))
            {
                var results = new List<Product>();
                Menu1 menu1 = context.Menu1.FirstOrDefault(x => x.MenuId == menuId);
                var categories = CategoryService.GetByMenuId(menu1.Id);
                foreach (var item in categories)
                {
                    results.AddRange(item.Products.OrderByDescending(x => x.BuyCount).Take(1));
                }

                return results;
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
                Message = "Create product successfully!"
            };
            using (var context = new mangoEntities())
            {
                var user = UserService.GetUserInfo();
                var category = context.Categories.First(x => x.Id == data.CategoryId);
                data.Code = category.Code + data.Code;

                if (context.Products.Any(x => x.Code == data.Code))
                {
                    result.Code = ResultCode.Fail;
                    result.Message = "Code already exists!";
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
                Message = "Update successfully!",
            };
            using (var context = new mangoEntities())
            {
                if (context.Products.Any(x => x.Code == data.Code && x.Id != data.Id))
                {
                    result.Code = ResultCode.Fail;
                    result.Message = "Code already exists!";
                    return result;
                }

                data.TimeUpdate = DateTime.Now;
                context.Products.Attach(data);
                context.Entry(data).State = EntityState.Modified;
                context.Entry(data).Property(x => x.TimeCreate).IsModified = false;
                context.Entry(data).Property(x => x.UserCreateId).IsModified = false;
              //  context.SaveChanges();
                try
                {
                    context.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    var strError = new StringBuilder();
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        strError.AppendFormat(
                            "Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            strError.AppendFormat("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    var temp = strError.ToString();
                }
            }
            return result;
        }

        public static string GetNewCode(string codeOld, int id, string table, bool hasPad = false)
        {
            using (var context = new mangoEntities(IsolationLevel.ReadUncommitted))
            {
               
                List<string> listCode;
                if (id == 0)
                {
                    listCode =
                        context.Database.SqlQuery<string>(
                            string.Format("select Code from {0} where  Code like '{1}%'", table, codeOld)).ToList();
                }
                else
                {
                    listCode =
                            context.Database.SqlQuery<string>(
                                string.Format("select Code from {2} where  Code like '{0}%' and Id != {1}", codeOld, id, table)).ToList();
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
                Message = "Delete product successfully!",
            };
            using (var context = new mangoEntities(IsolationLevel.ReadUncommitted))
            {
                var productIdList =
                    productIdStr.Replace(" ", "").Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                foreach (var productId in productIdList)
                {
                    var Id = int.Parse(productId);
                    var product = context.Products.First(x => x.Id == Id);
                    if (context.StoreProducts
                        .Any(x => x.ProductId == product.Id
                        && (x.QuantityExchange > 0)))
                    {
                        result.Code = ResultCode.Fail;
                        result.Message = "Sản phẩm này còn tồn trong kho không thể xóa được!";
                        return result;
                    }

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


        public static void UpdateStar(int id, int star)
        {
            using (var context = new mangoEntities())
            {
                var product = context.Products.FirstOrDefault(x => x.Id == id);
                if (product.CountStar != null)
                {
                    product.CountStar += 1;
                    product.Star = star;
                }
                else
                {
                    product.Star = star;
                    product.CountStar = 1;
                }

                product.TimeUpdate = DateTime.Now;
                context.SaveChanges();
            }
        }
    }
}

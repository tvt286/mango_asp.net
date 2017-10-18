using Mango.Data;
using System.Data;
using System.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mango.Common;

namespace Mango.Services
{
    public class CategoryService
    {
        public static Category Get(int id)
        {
            using (var context = new mangoEntities(IsolationLevel.ReadUncommitted))
            {
                return context.Categories.First(x => x.Id == id);
            }
        }

        public static List<Category> GetAll()
        {
            using (var context = new mangoEntities(IsolationLevel.ReadUncommitted))
            {
                return context.Categories.Where(x => x.IsDeleted == false).AsNoTracking().ToList();
            }
        }

        public static RedirectCommand Create(Category data)
        {
            var result = new RedirectCommand
            {
                Code = ResultCode.Success,
                Message = "Create category successfully!"
            };
            var user = UserService.GetUserInfo();
            using (var context = new mangoEntities())
            {
                if (context.Categories.Any(x => x.Code == data.Code))
                {
                    result.Code = ResultCode.Fail;
                    result.Message = "Code already exist!";
                    return result;
                }
                data.UserCreateId = user.Id;
                data.TimeCreate = DateTime.Now;

                context.Categories.Add(data);
                context.SaveChanges();
            }
            return result;
        }


        public static RedirectCommand Update(Category data)
        {
            var result = new RedirectCommand
            {
                Code = ResultCode.Success,
                Message = "Update successfully!"
            };

            using (var context = new mangoEntities())
            {
                if (context.Categories.Any(x => x.Code == data.Code && x.Id != data.Id))
                {
                    result.Code = ResultCode.Fail;
                    result.Message = "Code already exist!";
                    return result;
                }

                data.TimeUpdate = DateTime.Now;
                context.Categories.Attach(data);
                context.Entry(data).State = EntityState.Modified;
                context.Entry(data).Property(x => x.TimeCreate).IsModified = false;
                context.Entry(data).Property(x => x.UserCreateId).IsModified = false;
                context.SaveChanges();
            }
            return result;
        }


        public static PagedSearchList<Category> Search(string code, string name, string description,
            int pageSize, int pageIndex)
        {
            using (var context = new mangoEntities(IsolationLevel.ReadUncommitted))
            {
                var user = UserService.GetUserInfo();
                IQueryable<Category> query = context.Categories.Where(x => x.IsDeleted == false).AsNoTracking();

                if (code.NotEmpty())
                {
                    query = query.Where(x => x.Code.Contains(code));
                }

                if (name.NotEmpty())
                {
                    query = query.Where(x => x.Name.Contains(name));
                }

                if (description.NotEmpty())
                {
                    query = query.Where(x => x.Description.Contains(description));
                }

                query = query.OrderByDescending(x => x.Id);
                pageIndex = pageIndex < 1 ? 1 : pageIndex;
                return new PagedSearchList<Category>(query, pageIndex, pageSize);
            }
        }

        public static RedirectCommand DeleteCategory(string categoryIdStr)
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
                    var category = context.Categories.FirstOrDefault(x => x.Id == Id);
                    var products = context.Products.Where(x => x.CategoryId == category.Id).ToList();
                    if (products.Count > 0)
                    {
                        result.Code = ResultCode.Fail;
                        result.Message = "Loại sản phẩm này có sản phẩm không thể xóa được!";
                        return result;
                    }
                    category.IsDeleted = true;
                    category.TimeDeleted = DateTime.Now;
                }
                context.SaveChanges();
            }

            return result;
        }
    }
}

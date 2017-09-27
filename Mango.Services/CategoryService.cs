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
                return context.Categories.AsNoTracking().ToList();
            }
        }

        public static CommandResult Create(Category data)
        {
            var result = new CommandResult
            {
                Code = ResultCode.Success,
                Message = "Đã tạo danh mục thành công!"
            };
            var user = UserService.GetUserInfo();
            using (var context = new mangoEntities())
            {
                if (context.Categories.Any(x => x.Code == data.Code))
                {
                    result.Code = ResultCode.Fail;
                    result.Message = "Đã tồn tại mã loại sản phẩm này rồi";
                    return result;
                }
                data.UserCreateId = user.Id;
                data.TimeCreate = DateTime.Now;

                context.Categories.Add(data);
                context.SaveChanges();
            }
            return result;
        }


        public static CommandResult Update(Category data)
        {
            var result = new CommandResult
            {
                Code = ResultCode.Success,
                Message = "Đã cập nhật danh mục thành công!"
            };

            using (var context = new mangoEntities())
            {
                if (context.Categories.Any(x => x.Code == data.Code))
                {
                    result.Code = ResultCode.Fail;
                    result.Message = "Đã tồn tại mã loại sản phẩm này rồi";
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
                IQueryable<Category> query = context.Categories.AsNoTracking();

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


    }
}

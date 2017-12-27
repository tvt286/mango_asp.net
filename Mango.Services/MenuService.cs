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
    public class MenuService
    {
        public static Menu Get(int id)
        {
            using (var context = new mangoEntities(IsolationLevel.ReadUncommitted))
            {
                return context.Menus.First(x => x.Id == id);
            }
        }

        public static List<Menu> GetAll()
        {
            using (var context = new mangoEntities(IsolationLevel.ReadUncommitted))
            {
                return context.Menus.Where(x => x.IsDeleted == false)
                    .Include(x => x.Menu1)
                    .Include(x => x.Menu1.Select(a => a.Categories))
                    .AsNoTracking().ToList();
            }
        }

       

        public static List<Menu> GetMenusHasCategory()
        {
            using (var context = new mangoEntities(IsolationLevel.ReadUncommitted))
            {
                return context.Menus
                    .Include(x => x.Menu1)
                    .Include(x => x.Menu1.Select(a => a.Categories))
                    .Where(x => x.IsDeleted == false && x.Menu1.Any())
                    .AsNoTracking().ToList();
            }
        }

        public static List<Menu1> GetAllMenu1()
        {
            using (var context = new mangoEntities(IsolationLevel.ReadUncommitted))
            {
                return context.Menu1.Where(x => x.IsDeleted == false).Include(x => x.Categories).AsNoTracking().ToList();
            }
        }

        public static List<Menu1> GetMenu1ByMenuId(int id)
        {
            using (var context = new mangoEntities(IsolationLevel.ReadUncommitted))
            {
                return context.Menu1.Where(x => x.IsDeleted == false)
                    .Where(x=>x.MenuId == id)
                    .Include(x => x.Categories).AsNoTracking().ToList();
            }
        }


        public static RedirectCommand Create(Menu data)
        {
            var result = new RedirectCommand
            {
                Code = ResultCode.Success,
                Message = "Create menu successfully!"
            };
            using (var context = new mangoEntities())
            {            
                data.TimeCreate = DateTime.Now;
                data.IsDeleted = false;
                context.Menus.Add(data);
                context.SaveChanges();
            }
            return result;
        }


        public static RedirectCommand Update(Menu data)
        {
            var result = new RedirectCommand
            {
                Code = ResultCode.Success,
                Message = "Update successfully!"
            };

            using (var context = new mangoEntities())
            {
                context.Menus.Attach(data);
                context.Entry(data).State = EntityState.Modified;
                context.Entry(data).Property(x => x.TimeCreate).IsModified = false;
                context.SaveChanges();
            }
            return result;
        }


        public static PagedSearchList<Menu> Search(string name,
            int pageSize, int pageIndex)
        {
            using (var context = new mangoEntities(IsolationLevel.ReadUncommitted))
            {
                var user = UserService.GetUserInfo();
                IQueryable<Menu> query = context.Menus.Where(x => x.IsDeleted == false).AsNoTracking();
       
                if (name.NotEmpty())
                {
                    query = query.Where(x => x.Name.Contains(name));
                }

                query = query.OrderByDescending(x => x.Id);
                pageIndex = pageIndex < 1 ? 1 : pageIndex;
                return new PagedSearchList<Menu>(query, pageIndex, pageSize);
            }
        }

        public static RedirectCommand DeleteMenu(string menuStr)
        {
            var result = new RedirectCommand
            {
                Code = ResultCode.Success,
                Message = "Delete successfully!",
            };
            using (var context = new mangoEntities())
            {
                var menuIdList =
                    menuStr.Replace(" ", "").Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                foreach (var menuId in menuIdList)
                {
                    var Id = int.Parse(menuId);
                    var menu = context.Menus.FirstOrDefault(x => x.Id == Id);
                    var menu1 = context.Menu1.Where(x => x.MenuId == menu.Id).ToList();
                    if (menu1.Count > 0)
                    {
                        result.Code = ResultCode.Fail;
                        result.Message = "Menu này có loại sản phẩm không thể xóa được!";
                        return result;
                    }
                    menu.IsDeleted = true;
                }
                context.SaveChanges();
            }

            return result;
        }
    }
}

using Mango.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mango.Common;

namespace Mango.Services
{
    public class GroupPermissionService
    {
        public static List<Group> GetAll()
        {
            using (var context = new mangoEntities(IsolationLevel.ReadUncommitted))
            {
                return context.Groups.AsNoTracking().ToList();
            }
        }

        public static Group Get(int groupId)
        {
            using (var context = new mangoEntities(IsolationLevel.ReadUncommitted))
            {
                return context.Groups.Where(x => x.GroupId == groupId).FirstOrDefault();
            }
        }


        public static PagedSearchList<Group> Search(string name,
            int pageSize, int pageIndex)
        {
            using (var context = new mangoEntities(IsolationLevel.ReadUncommitted))
            {
                var user = UserService.GetUserInfo();
                IQueryable<Group> query = context.Groups.AsNoTracking();
              
                if (name.NotEmpty())
                {
                    query = query.Where(x => x.Name.Contains(name));
                }

                query = query.OrderByDescending(x => x.GroupId);
                pageIndex = pageIndex < 1 ? 1 : pageIndex;
                return new PagedSearchList<Group>(query, pageIndex, pageSize);
            }
        }
    }
}

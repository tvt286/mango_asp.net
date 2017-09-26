using Mango.Common;
using Mango.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;

namespace Mango.Services
{
    public class UserService
    {
        public static PagedSearchList<User> Search(string userName, string fullName, string phone, UserStatus? status,
           int[] groupIdList,
           int pageSize, int pageIndex)
        {
            using (var context = new mangoEntities(IsolationLevel.ReadUncommitted))
            {
                var user = GetUserInfo();
                IQueryable<User> query = context.Users.AsNoTracking();
                //if (user.IsAdminRoot == false)
                //{
                //    query = query.Where(x => x.CompanyId == user.CompanyId);
                //}

                if (userName.NotEmpty())
                {
                    query = query.Where(x => x.UserName.Contains(userName));
                }

                if (fullName.NotEmpty())
                {
                    query = query.Where(x => x.FullName.Contains(fullName));
                }

                if (status.HasValue)
                {
                    query = query.Where(x => x.Status == status.Value);
                }

                if (groupIdList != null)
                {
                    query = query.Where(x => x.Groups.Any(g => groupIdList.Contains(g.GroupId)));
                }

                query = query.Include(x => x.Groups).OrderByDescending(x => x.Id);
                pageIndex = pageIndex < 1 ? 1 : pageIndex;
                return new PagedSearchList<User>(query, pageIndex, pageSize);
            }
        }

        public static CommandResult CreateUser(User data)
        {
            using (var context = new mangoEntities())
            {
                if (context.Users.Any(x => x.UserName == data.UserName))
                {
                    return new CommandResult
                    {
                        Code = ResultCode.Success,
                        Message = "Đã tồn tại tài khoản này trong hệ thống!"
                    };
                }

                var user = GetUserInfo();
                //if (user.IsAdminRoot == false)
                //{
                //    data.CompanyId = user.CompanyId;
                //}
                data.TimeCreate = DateTime.Now;
                context.Users.Add(data);
                context.SaveChanges();

                return new CommandResult
                {
                    Code = ResultCode.Success,
                    Message = "Đã tạo mới tài khoản thành công!"
                };
            }
        }

        public static CommandResult UpdateUser(User data)
        {
            using (var context = new mangoEntities())
            {
                if (context.Users.Any(x => x.UserName == data.UserName && x.Id != data.Id))
                {
                    return new CommandResult
                    {
                        Code = ResultCode.Success,
                        Message = "Đã tồn tại tài khoản này trong hệ thống!"
                    };
                }

                context.Users.Attach(data);
                context.Entry(data).State = EntityState.Modified;
                context.SaveChanges();

                return new CommandResult
                {
                    Code = ResultCode.Success,
                    Message = "Đã tạo mới tài khoản thành công!"
                };
            }
        }

        public static User Get(int id, bool includeGroup = false, bool includePermission = false)
        {
            using (var context = new mangoEntities(IsolationLevel.ReadUncommitted))
            {
                var query = context.Users.Where(x => x.Id == id);
                if (includeGroup)
                {
                    query = query.Include(x => x.Groups);
                }
                if (includePermission)
                {
                    query = query.Include(x => x.User_Permission);
                }

                return query.First();
            }
        }


        public static User Get(string userName)
        {
            using (var context = new mangoEntities(IsolationLevel.ReadUncommitted))
            {
                return context.Users.FirstOrDefault(x => x.UserName == userName);
            }
        }

        public static bool Login(string userName, string password)
        {
            using (var context = new mangoEntities(IsolationLevel.ReadUncommitted))
            {
                var passMD5 = Encryptor.MD5Hash(password);

                return context.Users.Any(x => x.UserName == userName && x.Password == passMD5);
            }
        }

        public static User GetUserInfo()
        {
            if (!HttpContext.Current.User.Identity.IsAuthenticated) return null;
            var username = HttpContext.Current.User.Identity.Name;
            string key = string.Format("UserService-Info-{0}", username);
            var user = new MemoryCacheManager().Get(key, () =>
            {
                using (var context = new mangoEntities(IsolationLevel.ReadUncommitted))
                {
                    return context.Users.AsNoTracking().FirstOrDefault(x => x.UserName == username);
                }
            });
            if (user == null)
            {
                HttpContext.Current.Session.Abandon();
                FormsAuthentication.SignOut();
            }
            return user;
        }

        public static List<Group> GetGroups()
        {
            using (var context = new mangoEntities(IsolationLevel.ReadUncommitted))
            {
                return context.Groups.AsNoTracking().ToList();
            }
        }

        public static RedirectCommand CreateUser(User data, int[] groupIdList, int[] permissionIdList)
        {
            var result = new RedirectCommand
            {
                Code = ResultCode.Success,
                Message = "Đã tạo mới tài khoản thành công"
            };

            using (var context = new mangoEntities())
            {
                if (context.Users.Any(x => x.UserName == data.UserName))
                {
                    result.Code = ResultCode.Fail;
                    result.Message = "Tài khoản này đã tồn tại trên hệ thống!";
                    return result;
                }

                if (permissionIdList != null)
                {
                    foreach (var permissionId in permissionIdList)
                    {
                        data.User_Permission.Add(new User_Permission
                        {
                            PermissionId = permissionId
                        });
                    }
                }

                if (groupIdList != null)
                {
                    foreach (var groupId in groupIdList)
                    {
                        var group = context.Groups.First(x => x.GroupId == groupId);
                        data.Groups.Add(group);
                    }
                }

                data.TimeCreate = DateTime.Now;
                context.Users.Add(data);
                //context.SaveChanges();
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

        public static RedirectCommand UpdateUser(User data, int[] groupIdList, int[] permissionIdList)
        {
            var result = new RedirectCommand
            {
                Code = ResultCode.Success,
                Message = "Đã cập nhật tài khoản thành công",
            };
            using (var context = new mangoEntities())
            {
                if (context.Users.Any(x => x.UserName == data.UserName && x.Id != data.Id))
                {
                    result.Code = ResultCode.Fail;
                    result.Message = "Tài khoản này đã tồn tại trên hệ thống!";
                    return result;
                }



                context.Users.Attach(data);
                context.Entry(data).State = EntityState.Modified;
                data.User_Permission.Clear();
                data.Groups.Clear();


                if (permissionIdList != null)
                {
                    foreach (var permissionId in permissionIdList)
                    {
                        context.User_Permission.Add(new User_Permission
                        {
                            PermissionId = permissionId,
                            UserId = data.Id
                        });
                    }
                }

                if (groupIdList != null)
                {
                    foreach (var groupId in groupIdList)
                    {
                        var group = context.Groups.First(x => x.GroupId == groupId);
                        data.Groups.Add(group);
                    }
                }


                context.Entry(data).Property(x => x.TimeCreate).IsModified = false;
                context.SaveChanges();
            }
            return result;
        }


        public static void Update(User data)
        {
            using (var context = new mangoEntities())
            {
                context.Users.Attach(data);
                context.Entry(data).State = EntityState.Modified;
                context.Entry(data).Property(x => x.TimeCreate).IsModified = false;
                context.SaveChanges();
                //try
                //{
                //    context.SaveChanges();
                //}
                //catch (DbEntityValidationException e)
                //{
                //    var strError = new StringBuilder();
                //    foreach (var eve in e.EntityValidationErrors)
                //    {
                //        strError.AppendFormat(
                //            "Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                //            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                //        foreach (var ve in eve.ValidationErrors)
                //        {
                //            strError.AppendFormat("- Property: \"{0}\", Error: \"{1}\"",
                //                ve.PropertyName, ve.ErrorMessage);
                //        }
                //    }
                //    var temp = strError.ToString();
                //}
            }
        }


        public static bool CheckUserHasPermission(int userId, int permission)
        {
            using (var context = new mangoEntities(IsolationLevel.ReadUncommitted))
            {
                var user =
                    context.Users.Include(x => x.User_Permission).Include(x => x.Groups.Select(y => y.Group_Permission))
                        .FirstOrDefault(x => x.Id == userId);

                var result = user.Groups.SelectMany(x => x.Group_Permission.Select(y => y.PermissionId)).ToList().Union(user.User_Permission.Select(x => x.PermissionId)).Distinct().ToList();
                if (result.Contains(permission))
                    return true;
                return false;
            }
        }

        public static void DeleteUser(string userIdStr)
        {
            using (var context = new mangoEntities(IsolationLevel.ReadUncommitted))
            {
                var userIdList =
                    userIdStr.Replace(" ", "").Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
                foreach (var userId in userIdList)
                {
                    var user = context.Users.First(x => x.Id == userId);
                    user.IsDeleted = true;
                }
                context.SaveChanges();
            }
        }
    }

}

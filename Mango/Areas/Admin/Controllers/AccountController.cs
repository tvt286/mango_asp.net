﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Mango.Common;
using Mango.Data.Enums;
using Mango.Data;
using Mango.Services;
using Mango.Web;

namespace Mango.Areas.Admin.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login(string username, string password, bool rememberMe, string returnUrl)
        {
            if (Request.HttpMethod == "GET")
            {
                return View();
            }
            else
            {
                var passMD5 = Encryptor.MD5Hash(password);

                var user = UserService.Get(username);

                if (user != null && (!string.IsNullOrWhiteSpace(user.Password) && user.Password == passMD5))
                {
                    if (user.Status == UserStatus.InActive)
                    {
                        ViewBag.Message = "Account locked!";
                        return View();
                    }
                    else
                    {
                        FormsAuthentication.SetAuthCookie(user.UserName, rememberMe);

                        if (string.IsNullOrEmpty(returnUrl))
                        {
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            return RedirectPermanent(returnUrl);
                        }
                    }
                }
                ViewBag.Message = "Username or password is invalid!";
                return View();
            }
        }

        [HttpPost]
        public ActionResult LogOff()
        {
            AuthorizeService.ClearCache(HttpContext.User.Identity.Name);
            FormsAuthentication.SignOut();

            return RedirectToAction("Login", "Account", "");
        }

         public ActionResult ProfileUser(User model, HttpPostedFileBase fileAttach)
        {
            if (Request.HttpMethod == "GET")
            {
                var user = UserService.GetUserInfo();
                return View(user);
            }

            var data = UserService.Get(model.Id);
            

            string sourceFile = "";
            if (fileAttach != null)
            {
                if (!Directory.Exists(Server.MapPath(string.Format("~/content/Upload/User"))))
                {
                    Directory.CreateDirectory(Server.MapPath(string.Format("~/content/Upload/User")));
                }

                var fileName = Utility.ConvertToUnsign(Path.GetFileNameWithoutExtension(fileAttach.FileName)).Replace("&", "").Replace("?", "").Replace(" ", "-") + "-" + Guid.NewGuid() + Path.GetExtension(fileAttach.FileName);
                sourceFile = string.Format("~/content/Upload/User/{0}", fileName);
                var pathFile = Server.MapPath(sourceFile);
                var checkFile = UploadHelper.CheckImageUpload(fileAttach);
                if (checkFile == UploadFileStatus.NotSupportExtension)
                {
                    return Json(new CommandResult
                    {
                        Code = ResultCode.Fail,
                        Message = "Only upload file image!"
                    }, JsonRequestBehavior.AllowGet);
                }
                if (checkFile == UploadFileStatus.OverLimited)
                {
                    return Json(new CommandResult
                    {
                        Code = ResultCode.Fail,
                        Message = "Only upload file image < 5MB!"
                    }, JsonRequestBehavior.AllowGet);
                }
                Bitmap bitmap = (Bitmap)Bitmap.FromStream(fileAttach.InputStream, true);
                ImageUtils.RewriteImageFix(bitmap, 500, 500, pathFile);

            }

            var tempPass = data.Password;
            TryUpdateModel(data);
            if (sourceFile.NotEmpty())
            {
                if (data.Image.NotEmpty())
                {
                    try
                    {
                        System.IO.File.Delete(Server.MapPath(data.Image));
                    }
                    catch (Exception ex)
                    {
                    }

                }
                data.Image = sourceFile;
            }

            if (data.Password.IsEmpty())
            {
                data.Password = tempPass;
            }
            else
            {
                data.Password = Encryptor.MD5Hash(model.Password);
            }

            UserService.Update(data);
            AuthorizeService.ClearCache(HttpContext.User.Identity.Name);

            return Json(new CommandResult()
            {
                Message = "Update successfully!"
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
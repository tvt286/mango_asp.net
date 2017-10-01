using Mango.Areas.Admin.Models;
using Mango.Data.Enums;
using Mango.Security;
using Mango.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mango.Data;
using Mango.Common;

namespace Mango.Areas.Admin.Controllers
{
    public class StoreController : Controller
    {
        [AuthorizeAdmin(Permissions = new[] { Permission.Store_View })]
        public ActionResult Index(StoreSearchModel searchModel)
        {

            if (Request.HttpMethod == "GET")
            {
                return View(searchModel);
            }

            var pagedList = StoreService.Search(searchModel.Address, searchModel.Name, searchModel.Code, searchModel.PageSize, searchModel.PageIndex);
            pagedList.SearchModel = searchModel;

            return PartialView("_List", pagedList);
        }

        [AuthorizeAdmin(Permission = Permission.Store_Create)]
        public ActionResult Detail(int? id, bool redirectWarehouse = false)
        {
            var user = UserService.GetUserInfo();
            ViewBag.User = user;

            var model = new Mango.Data.Store();


            ViewBag.CategoryId = new SelectList(CategoryService.GetAll(), "Id", "Name");
            ViewBag.ProductId = new SelectList(ProductService.GetAll()
                     .Select(x => new { x.Id, Name = x.Code + " - " + x.Name }).ToList(), "Id", "Name");
            ViewBag.CategoryId = new SelectList(CategoryService.GetAll(), "Id", "Name");
            if (id.HasValue)
            {
                model = StoreService.Get(id.Value);
                ViewBag.CityId = new SelectList(LocationService.GetAllCity(), "Id", "Name", model.CityId);
                ViewBag.DistrictId = new SelectList(LocationService.GetDistrictByCity(model.CityId.GetValueOrDefault(0)).Select(x => new { x.Id, Name = x.Prefix + " " + x.Name }).ToList(), "Id", "Name", model.DistrictId);
                ViewBag.WardId = new SelectList(LocationService.GetWardByDistrict(model.DistrictId.GetValueOrDefault(0)).Select(x => new { x.Id, Name = x.Prefix + " " + x.Name }).ToList(), "Id", "Name", model.WardId);
                ViewBag.StreetId = new SelectList(LocationService.GetStreetByDistrictAndCity(model.CityId.GetValueOrDefault(0), model.DistrictId.GetValueOrDefault(0)), "Id", "Name", model.StreetId);
            }
            else
            {
                //ViewBag.ParentWarehouseId = new SelectList(WarehouseService.GetAllWarehouseOfCompany(user.CompanyId.GetValueOrDefault(0)).Select(x => new { x.Id, Name = x.Code + " - " + x.Name }), "Id", "Name", model.WarehouseId);
                ViewBag.CityId = new SelectList(LocationService.GetAllCity(), "Id", "Name", model.CityId);
                ViewBag.DistrictId = new SelectList(LocationService.GetDistrictByCity(model.CityId.GetValueOrDefault(-1)).Select(x => new { x.Id, Name = x.Prefix + " " + x.Name }).ToList(), "Id", "Name", model.DistrictId);
                ViewBag.WardId = new SelectList(LocationService.GetWardByDistrict(model.DistrictId.GetValueOrDefault(-1)).Select(x => new { x.Id, Name = x.Prefix + " " + x.Name }).ToList(), "Id", "Name", model.WardId);
                ViewBag.StreetId = new SelectList(LocationService.GetStreetByDistrictAndCity(model.CityId.GetValueOrDefault(-1), model.DistrictId.GetValueOrDefault(-1)).Select(x => new { x.Id, x.Name }).ToList(), "Id", "Name", model.StreetId);
            }

            return View(model);
        }


        [AuthorizeAdmin(Permission = Permission.Store_Create)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Mango.Data.Store model, bool redirectWarehouse = false)
        {

            var result = new RedirectCommand();

            if (model.Code != null && !model.Code.IsCode())
            {
                return Json(new CommandResult
                {
                    Code = ResultCode.Fail,
                    Message = "Mã chỉ được nhập chữ, số và ký tự -_."
                }, JsonRequestBehavior.AllowGet);
            }

            if (model.Phone.NotEmpty())
            {
                if (!Utility.IsValidPhone(model.Phone))
                {
                    return
                    Json(
                        new RedirectCommand() { Code = ResultCode.Fail, Message = "Vui lòng kiểm tra lại số điện thoại!" },
                        JsonRequestBehavior.AllowGet);
                }
            }

            var user = UserService.GetUserInfo();

            if (model.Id == 0)
            {
                if (StoreService.Has(model.Code))
                {
                    return Json(new CommandResult
                    {
                        Code = ResultCode.Fail,
                        Message = "Mã máy đã tồn tại"
                    }, JsonRequestBehavior.AllowGet);
                }

                result = StoreService.Create(model);
                return Json(new RedirectCommand
                {
                    Code = ResultCode.Success,
                    Message = "Đã tạo máy thành công!",
                    Url = Url.Action("Index")
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {

                var data = StoreService.Get(model.Id);
                if (model.Code != null && !model.Code.Equals(data.Code))
                {
                    if (StoreService.Has(model.Code))
                    {
                        return Json(new CommandResult
                        {
                            Code = ResultCode.Fail,
                            Message = "Mã máy đã tồn tại"
                        }, JsonRequestBehavior.AllowGet);
                    }
                }

                TryUpdateModel(data);
                StoreService.Update(data);
            }
            return Json(new RedirectCommand
            {
                Code = ResultCode.Success,
                Message = "Đã cập nhật máy thành công!",
                Url = Url.Action("Index")
            }, JsonRequestBehavior.AllowGet);

        }

    }
}
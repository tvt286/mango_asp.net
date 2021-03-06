﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mango.Data;

namespace Mango.Web
{
    public static class UrlHelperExtension
    {
        public static string ImageUser(this UrlHelper url, User user)
        {
            if (string.IsNullOrEmpty(user.Image))
            {
                return url.Content("~/Content/Upload/default.png");
            }
            return url.Content(user.Image);
        }


        public static string ImageProduct(this UrlHelper url, Product product)
        {
            if (string.IsNullOrEmpty(product.Image))
            {
                return url.Content("~/Content/Upload/noimage.png");
            }
            return url.Content(product.Image);
        }

        public static string ImageProductInfo(this UrlHelper url, Product product)
        {
            if (string.IsNullOrEmpty(product.Image))
            {
                return url.Content("~/Content/Upload/noimage.png");
            }
            var image = product.Image.Split(';')[1];
            return url.Content(image);
        }

        public static string ImageProductSlide(this UrlHelper url, Product product, int position)
        {
            if (string.IsNullOrEmpty(product.Image))
            {
                return url.Content("~/Content/Upload/noimage.png");
            }
            var image = product.Image.Split(';')[position];
            return url.Content(image);
        }

        public static string ImageMenu(this UrlHelper url, Menu menu)
        {
            if (string.IsNullOrEmpty(menu.Image))
            {
                return url.Content("~/Content/Upload/noimage.png");
            }
            return url.Content(menu.Image);
        }


        //public static string ImageCompany(this UrlHelper url, Company company)
        //{
        //    if (string.IsNullOrEmpty(company.Image))
        //    {
        //        return url.Content("~/Content/Upload/noimage.png");
        //    }
        //    return url.Content(company.Image);
        //}

        //public static string ImageLogo(this UrlHelper url, Company company)
        //{
        //    if (string.IsNullOrEmpty(company.Logo))
        //    {
        //        return url.Content("~/Content/Upload/noimage.png");
        //    }
        //    return url.Content(company.Logo);
        //}
    }
}
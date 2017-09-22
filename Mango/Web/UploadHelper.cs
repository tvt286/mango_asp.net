using System.Configuration;
using System.IO;
using System.Web;
using Mango.Data.Enums;
using System;


namespace Mango.Web
{

    public class UploadHelper
    {
        public static UploadFileStatus CheckImageUpload(HttpPostedFileBase httpPostedFileBase)
        {
            const string extension = "jpge,jpg,png,gif,bmp";
            const int limitedLength = 4 * 1024;

            if (httpPostedFileBase == null) return UploadFileStatus.NotFile;
            if (httpPostedFileBase.ContentLength == 0) return UploadFileStatus.NotFile;

            var ext = Path.GetExtension(httpPostedFileBase.FileName).ToLower();
            ext = ext.Replace(".", "");
            if (extension.IndexOf(ext) < 0)
            {
                return UploadFileStatus.NotSupportExtension;
            }
            var overLimited = httpPostedFileBase.ContentLength > limitedLength * 1024;
            if (overLimited)
            {
                return UploadFileStatus.OverLimited;
            }

            return UploadFileStatus.Success;
        }



        public static bool CheckImageDimension(HttpPostedFileBase httpPostedFileBase)
        {
            System.IO.Stream stream = httpPostedFileBase.InputStream;
            System.Drawing.Image image = System.Drawing.Image.FromStream(stream);
            var height = image.Height;
            var width = image.Width;
            if (width != 1080 || height != 1920)
                return false;
            return true;
           
        }

        public static bool CheckLogoDimension(HttpPostedFileBase httpPostedFileBase)
        {
            System.IO.Stream stream = httpPostedFileBase.InputStream;
            System.Drawing.Image image = System.Drawing.Image.FromStream(stream);
            var height = image.Height;
            var width = image.Width;
            if (width <= height)
                return false;
            return true;

        }


        public static UploadFileStatus CheckVideoUpload(HttpPostedFileBase httpPostedFileBase)
        {
            const string extension = "mp4";
            const int limitedLength = 50 * 1024;

            if (httpPostedFileBase == null) return UploadFileStatus.NotFile;
            if (httpPostedFileBase.ContentLength == 0) return UploadFileStatus.NotFile;

            var ext = Path.GetExtension(httpPostedFileBase.FileName).ToLower();
            ext = ext.Replace(".", "");
            if (extension.IndexOf(ext) < 0)
            {
                return UploadFileStatus.NotSupportExtension;
            }
            var overLimited = httpPostedFileBase.ContentLength > limitedLength * 1024;
            if (overLimited)
            {
                return UploadFileStatus.OverLimited;
            }
            return UploadFileStatus.Success;
        }


        public static UploadFileStatus CheckUploadSubmissionFile(HttpPostedFileBase httpPostedFileBase)
        {
            const string extension = "jpge,jpg,png,gif,bmp,doc,docx,xls,xlsx,pdf";
            const int limitedLength = 10 * 1024;

            if (httpPostedFileBase == null) return UploadFileStatus.NotFile;
            if (httpPostedFileBase.ContentLength == 0) return UploadFileStatus.NotFile;

            var ext = Path.GetExtension(httpPostedFileBase.FileName).ToLower();
            ext = ext.Replace(".", "");
            if (extension.IndexOf(ext) < 0)
            {
                return UploadFileStatus.NotSupportExtension;
            }
            var overLimited = httpPostedFileBase.ContentLength > limitedLength * 1024;
            if (overLimited)
            {
                return UploadFileStatus.OverLimited;
            }

            return UploadFileStatus.Success;
        }
        
    }
}
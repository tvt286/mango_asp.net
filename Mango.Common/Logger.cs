using System;
using System.Text;
using System.Web;
using log4net;
using System.Data.Entity.Validation;

namespace Mango.Common
{
    public class Logger
    {
        public static ILog DefaultLogger { get; set; }
        public static ILog EmailLogger { get; set; }
        public static ILog SMSLogger { get; set; }

        static Logger()
        {
            log4net.Config.XmlConfigurator.Configure();
            DefaultLogger = LogManager.GetLogger("DefaultLogger");
            EmailLogger = LogManager.GetLogger("EmailLogger");
            SMSLogger = LogManager.GetLogger("SMSLogger");
        }

        public static void WriteWebLog(Exception exception)
        {
            if (exception is System.Web.HttpException)
            {
                return;
            }
            else if (exception is DbEntityValidationException)
            {
                var ex = exception as DbEntityValidationException;

                using (var stream = System.IO.File.AppendText(HttpContext.Current.Server.MapPath("~/log.txt")))
                {
                    stream.AutoFlush = true;
                    var sb = new StringBuilder();
                    foreach (var dbEntityValidationResult in ex.EntityValidationErrors)
                    {
                        foreach (var dbValidationError in dbEntityValidationResult.ValidationErrors)
                        {
                            sb.Append(dbValidationError.ErrorMessage + System.Environment.NewLine);
                        }
                    }
                    stream.WriteLine("------------------------------------------------------------------------\r\n" +
                                     DateTime.Now + "\r\n\r\n" +
                                     HttpContext.Current.Request.Url.AbsoluteUri + "\r\n\r\n" +
                                     sb.ToString() + "\r\n\r\n" +
                                     ex.ToString());
                }
            }
            else
            {
                using (var stream = System.IO.File.AppendText(HttpContext.Current.Server.MapPath("~/log.txt")))
                {
                    stream.AutoFlush = true;
                    stream.WriteLine("------------------------------------------------------------------------\r\n" +
                                     DateTime.Now + "\r\n\r\n" +
                                     HttpContext.Current.Request.Url.AbsoluteUri + "\r\n\r\n" +
                                     exception.ToString());
                }
            }

        }
    }
}

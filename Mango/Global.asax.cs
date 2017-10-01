using Mango.App_Start;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Mango
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            GlobalConfiguration.Configuration.Formatters.Remove(GlobalConfiguration.Configuration.Formatters.XmlFormatter);


            ModelBinders.Binders.Add(typeof(string), new TrimModelBinder());
            ModelBinders.Binders.Add(typeof(DateTime), new CustomDateBinder());
            ModelBinders.Binders.Add(typeof(DateTime?), new NullableCustomDateBinder());
            ModelBinders.Binders.Add(typeof(decimal), new DecimalModelBinder());

        }
     
    }

    public static class ExtensionHelpers
    {
        public static ValueProviderResult GetValueFromValueProvider(this ModelBindingContext bindingContext, bool performRequestValidation)
        {
            var unvalidatedValueProvider = bindingContext.ValueProvider as IUnvalidatedValueProvider;
            return (unvalidatedValueProvider != null)
              ? unvalidatedValueProvider.GetValue(bindingContext.ModelName, !performRequestValidation)
              : bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
        }
    }

    public class DecimalModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            ValueProviderResult valueResult = bindingContext.ValueProvider
                .GetValue(bindingContext.ModelName);

            ModelState modelState = new ModelState { Value = valueResult };

            object actualValue = null;

            if (valueResult.AttemptedValue != string.Empty)
            {
                try
                {
                    actualValue = Convert.ToDecimal(valueResult.AttemptedValue, CultureInfo.CurrentCulture);
                }
                catch (FormatException e)
                {
                    modelState.Errors.Add(e);
                }
            }

            bindingContext.ModelState.Add(bindingContext.ModelName, modelState);

            return actualValue;
        }
    }

    public class TrimModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            // First check if request validation is required
            var shouldPerformRequestValidation = controllerContext.Controller.ValidateRequest && bindingContext.ModelMetadata.RequestValidationEnabled;

            // Get value
            var valueProviderResult = bindingContext.GetValueFromValueProvider(shouldPerformRequestValidation);
            object actualValue = null;
            if (valueProviderResult != null)
            {
                actualValue = valueProviderResult.AttemptedValue.Trim();
            }

            //ValueProviderResult valueResult = bindingContext.ValueProvider
            //    .GetValue(bindingContext.ModelName);

            //ModelState modelState = new ModelState { Value = valueResult };

            //object actualValue = null;

            //if (valueResult != null && !string.IsNullOrWhiteSpace(valueResult.AttemptedValue))
            //{
            //    actualValue = valueResult.AttemptedValue.Trim();
            //}

            //bindingContext.ModelState.Add(bindingContext.ModelName, modelState);

            return actualValue;
        }
    }

    public class CustomDateBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (controllerContext == null)
                throw new ArgumentNullException("controllerContext", "controllerContext is null.");
            if (bindingContext == null)
                throw new ArgumentNullException("bindingContext", "bindingContext is null.");

            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (value == null)
                throw new ArgumentNullException(bindingContext.ModelName);

            CultureInfo cultureInf = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            cultureInf.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";

            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, value);

            try
            {
                var date = value.ConvertTo(typeof(DateTime), cultureInf);

                return date;
            }
            catch (Exception ex)
            {
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, ex);
                return null;
            }
        }
    }

    public class NullableCustomDateBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (controllerContext == null)
                throw new ArgumentNullException("controllerContext", "controllerContext is null.");
            if (bindingContext == null)
                throw new ArgumentNullException("bindingContext", "bindingContext is null.");

            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (value == null) return null;

            CultureInfo cultureInf = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            cultureInf.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";

            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, value);

            try
            {
                var date = value.ConvertTo(typeof(DateTime), cultureInf);

                return date;
            }
            catch (Exception ex)
            {
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, ex);
                return null;
            }
        }
    }
}
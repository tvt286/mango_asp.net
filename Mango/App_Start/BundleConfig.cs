using System.Web;
using System.Web.Optimization;

namespace Mango.App_Start
{
    public class BundleConfig
    { // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            // Vendor scripts
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery.min.js", "~/Scripts/jquery-2.1.4.min.js", "~/Scripts/easy-responsive-tabs.js", "~/Scripts/bootstrap.js"));

            // modernizr scripts
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                "~/Scripts/jquery.min.js", "~/Scripts/modernizr.custom.js"));

            // minicart scripts
            bundles.Add(new ScriptBundle("~/bundles/minicart").Include(
                "~/Scripts/jquery.min.js", "~/Scripts/minicart.min.js"));

            // stats responsive tabs
            bundles.Add(new ScriptBundle("~/bundles/responsive").Include(
                "~/Scripts/move-top.js", "~/Scripts/easy-responsive-tabs.js"));
            // stats scripts
            bundles.Add(new ScriptBundle("~/bundles/stats").Include(
                "~/Scripts/jquery.waypoints.min.js", "~/Scripts/jquery.countup.js"));

            // jQuery scrolling
            bundles.Add(new ScriptBundle("~/bundles/scrolling").Include("~/Scripts/move-top.js", "~/Scripts/jquery.easing.min.js"));
            //"~/Scripts/jquery.validate.min.js", "~/Scripts/jquery.unobtrusive-ajax.js", "~/Scripts/jquery.validate.unobtrusive.js", "~/Scripts/Fix.js", "~/Scripts/common.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include("~/Scripts/bootstrap.min.js"));
                      //"~/Scripts/bootstrap.min.js", "~/Scripts/moment-with-locales.js", "~/Scripts/bootstrap-datetimepicker.js", "~/Scripts/autoNumeric-min.js"));

            // Inspinia script
            //bundles.Add(new ScriptBundle("~/bundles/inspinia").Include(
            //          "~/Scripts/app/inspinia.js"));

            // SimpleCart script
            bundles.Add(new ScriptBundle("~/bundles/simplecart").Include(
                      "~/Scripts/plugins/simpleCart.min.js"));

            // SlimScroll
            //bundles.Add(new ScriptBundle("~/plugins/slimScroll").Include(
            //          "~/Scripts/plugins/slimScroll/jquery.slimscroll.min.js"));

            // jQuery plugins
            //bundles.Add(new ScriptBundle("~/plugins/metsiMenu").Include(
            //          "~/Scripts/plugins/metisMenu/metisMenu.min.js"));

            //bundles.Add(new ScriptBundle("~/plugins/pace").Include(
            //          "~/Scripts/plugins/pace/pace.min.js"));

            //bundles.Add(new ScriptBundle("~/plugins/sweetalert").Include(
            //          "~/Scripts/plugins/sweetalert/sweetalert.min.js"));

            //bundles.Add(new ScriptBundle("~/plugins/select2").Include(
            //          "~/Scripts/plugins/select2/select2.full.min.js"));

            //bundles.Add(new ScriptBundle("~/plugins/chosen").Include(
            //          "~/Scripts/plugins/chosen/chosen.jquery.js"));

            // CSS style (bootstrap/inspinia)
            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/css/bootstrap.css", "~/Content/css/style.css", "~/Content/css/font-awesome.css", "~/Content/css/easy-responsive-tabs.css"));
                //      "~/Content/animate.css",
                ///*"~/Content/style.css",*/ "~/Content/bootstrap-datetimepicker.css", "~/Content/fix.css",
                //      "~/Content/plugin/sweetalert/sweetalert.css", "~/Content/plugin/select2/select2.min.css",
                //      "~/Content/plugin/chosen/chosen.css", "~/Content/Plugin/boostrap-tagsinput/bootstrap-tagsinput.css", "~/Content/nv.d3.css"));


            // Font Awesome icons
            bundles.Add(new StyleBundle("~/font-awesome/css").Include(
                      "~/fonts/font-awesome/css/font-awesome.min.css", new CssRewriteUrlTransform()));


            /*Admin*/
            // Vendor scripts
            bundles.Add(new ScriptBundle("~/bundles/admin/jquery").Include(
                "~/Scripts/Admin/jquery.min.js"));

            // jQuery Validation
            bundles.Add(new ScriptBundle("~/bundles/admin/jqueryval").Include(
           "~/Scripts/Admin/jquery.validate.min.js", "~/Scripts/Admin/jquery.unobtrusive-ajax.js", "~/Scripts/Admin/jquery.validate.unobtrusive.js", "~/Scripts/Admin/common.js"));

            bundles.Add(new ScriptBundle("~/bundles/admin/bootstrap").Include("~/Scripts/Admin/removeads.js", "~/Scripts/Admin/jquery.core.js", "~/Scripts/Admin/jquery.app.js",
                      "~/Scripts/Admin/bootstrap.min.js", "~/Scripts/Admin/moment-with-locales.js", "~/Scripts/Admin/bootstrap-datetimepicker.js", "~/Scripts/Admin/autoNumeric-min.js"));

            bundles.Add(new ScriptBundle("~/bundles/admin/login").Include("~/Scripts/Admin/modernizr.js",
                     "~/Scripts/Admin/detect.js", "~/Scripts/Admin/fastclick.js", "~/Scripts/Admin/jquery.blockUI.js", "~/Scripts/Admin/waves.js", "~/Scripts/Admin/jquery.slimscroll.js", "~/Scripts/Admin/jquery.scrollTo.min.js"));

            // Inspinia script
            bundles.Add(new ScriptBundle("~/bundles/admin/inspinia").Include(
                      "~/Scripts/Admin/app/inspinia.js"));

            // SlimScroll
            bundles.Add(new ScriptBundle("~/plugins/admin/slimScroll").Include(
                      "~/Scripts/Admin/plugins/slimScroll/jquery.slimscroll.min.js"));

            // jQuery plugins
            bundles.Add(new ScriptBundle("~/plugins/admin/metsiMenu").Include(
                      "~/Scripts/Admin/plugins/metisMenu/metisMenu.min.js"));

            bundles.Add(new ScriptBundle("~/plugins/admin/pace").Include(
                      "~/Scripts/Admin/plugins/pace/pace.min.js"));

            bundles.Add(new ScriptBundle("~/plugins/admin/sweetalert").Include(
                      "~/Scripts/Admin/plugins/sweetalert/sweetalert.min.js"));

            bundles.Add(new ScriptBundle("~/plugins/admin/select2").Include(
                      "~/Scripts/Admin/plugins/select2/select2.full.min.js"));

            bundles.Add(new ScriptBundle("~/plugins/admin/chosen").Include(
                      "~/Scripts/Admin/plugins/chosen/chosen.jquery.js"));

            // CSS style (bootstrap/inspinia)
            bundles.Add(new StyleBundle("~/Content/admin/css").Include(
                      "~/Content/Admin/animate.css",
                /*"~/Content/style.css",*/ "~/Content/Admin/bootstrap-datetimepicker.css", "~/Content/Admin/fix.css",
                      "~/Content/Admin/plugin/sweetalert/sweetalert.css", "~/Content/Admin/plugin/select2/select2.min.css",
                      "~/Content/Admin/plugin/chosen/chosen.css", "~/Content/Admin/Plugin/boostrap-tagsinput/bootstrap-tagsinput.css", "~/Content/Admin/nv.d3.css"));


            // Font Awesome icons
            bundles.Add(new StyleBundle("~/font-awesome/admin/css").Include(
                      "~/fonts/Admin/font-awesome/css/font-awesome.min.css", new CssRewriteUrlTransform()));
        }
    }
}
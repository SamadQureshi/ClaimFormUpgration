using System.Web.Optimization;

namespace Onion.WebApp
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include("~/Scripts/jquery-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include("~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include("~/Scripts/modernizr-*"));

            // Core Required
            bundles.Add(new ScriptBundle("~/bundles/core").Include(
                "~/Scripts/core/popper.min.js",
                "~/Scripts/core/bootstrap-material-design.min.js",
                "~/Scripts/plugins/perfect-scrollbar.jquery.min.js"
                ));

            // Plugins for the Projects
            bundles.Add(new ScriptBundle("~/bundles/plugins").Include(
                "~/Scripts/plugins/moment.min.js",
                "~/Scripts/plugins/sweetalert2.js",
                //"~/Scripts/plugins/jquery.validate.min.js",
                "~/Scripts/plugins/bootstrap-selectpicker.js",
                "~/Scripts/plugins/bootstrap-datetimepicker.min.js",
                "~/Scripts/plugins/jquery.dataTables.min.js",
                "~/Scripts/plugins/jquery.dataTables.responsive.js",
                "~/Scripts/plugins/jasny-bootstrap.min.js",
                "~/Scripts/core.min.js",
                "~/Scripts/plugins/arrive.min.js",
                "~/Scripts/plugins/bootstrap-notify.js",
                "~/Scripts/material-dashboard.js"));

            bundles.Add(new ScriptBundle("~/bundles/ajax").Include(
                "~/Scripts/ajax/json2.js"
            ));




            // Project style files
            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/material-dashboard.min.css",
                "~/Content/site.css"));

            // Bootstrap file upload.
            bundles.Add(new ScriptBundle("~/bundles/bootstrap-file").Include("~/Scripts/script-bootstrap-file.js"));
            bundles.Add(new StyleBundle("~/Content/Bootstrap-file/css").Include("~/Content/style/style-bootstrap-file.css"));
        }
    }
}

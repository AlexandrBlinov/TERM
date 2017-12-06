using System.Web;
using System.Web.Optimization;

namespace Term.Web
{
    public class BundleConfig
    {
        // Дополнительные сведения о Bundling см. по адресу http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));


        //    bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include("~/Scripts/jquery.validate.js")); 
            
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                       "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*")); 

            bundles.Add(new ScriptBundle("~/bundles/other_service").Include(
                //       "~/Content/js/bootstrap.js",
                       "~/Scripts/bootstrap.js",
                       "~/Scripts/jquery.mask.js",
       //                "~/Scripts/jquery.maskedinput.js",
                       "~/Scripts/jquery.bootstrap-touchspin.js",
                       "~/Scripts/toastr.js",
                 //      "~/Scripts/imgPreload.js",
                       "~/Scripts/moment-with-locales.js",
                       "~/Scripts/bootstrap-datetimepicker.js",
                       "~/Scripts/jquery.fancybox.js"
                       ));


            

            bundles.Add(new ScriptBundle("~/bundles/term_service").Include(
                       "~/Scripts/yst-localize-20170619.js",
                       "~/Scripts/yst-utility.js",
                       "~/Scripts/yst-scripts-20170927.js",
                       "~/Scripts/term-scripts-20170906.js",
                       "~/Scripts/common-v6.js"));
            
            bundles.Add(new ScriptBundle("~/bundles/term_seasonscripts").Include("~/Scripts/term-season-scripts.js"));
            bundles.Add(new ScriptBundle("~/bundles/term_shoppingcart").Include("~/Scripts/yst-shoppingcart-20171205.js"));

            // Используйте версию Modernizr для разработчиков, чтобы учиться работать. Когда вы будете готовы перейти к работе,
            // используйте средство построения на сайте http://modernizr.com, чтобы выбрать только нужные тесты.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));
            
            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                        "~/Content/themes/base/jquery.ui.accordion.css",
                        "~/Content/themes/base/autocomplete.css",
                        "~/Content/themes/base/jquery.ui.button.css",
                        "~/Content/themes/base/jquery.ui.dialog.css",
                        "~/Content/themes/base/jquery.ui.slider.css",
                        "~/Content/themes/base/jquery.ui.tabs.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css",
                        "~/Content/themes/base/jquery.ui.theme.css"));


            bundles.Add(new StyleBundle("~/content/sitestyles").Include(
                "~/Content/css/common.css",
                "~/Content/css/toastr.css",
                "~/Content/seasonstyles.css",
                "~/Content/bootstrap-datetimepicker.css",
                "~/Content/css/ReSite.css"));
            
            
   

    //        BundleTable.EnableOptimizations = true;
        }
    }
}
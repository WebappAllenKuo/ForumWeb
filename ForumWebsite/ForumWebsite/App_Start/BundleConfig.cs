using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace ForumWebsite
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/js/jquery.min.js",
                        "~/Scripts/js/all.js"));


            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
           
            bundles.Add(new StyleBundle("~/All/Css").Include(
                        "~/Content/css/style.css",
                        "~/Content/css/bootstrap.css",
                        "~/Content/css/all.css",
                        "~/Content/css/style_page.css"));
            bundles.Add(new StyleBundle("~/Pages/Css").Include(
                        "~/Content/css/style_page.css"
                ));
            BundleTable.EnableOptimizations = false;
        }
    }
}
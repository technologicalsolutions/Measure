using System.Web;
using System.Web.Optimization;

namespace Measure
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content/tables").Include(
                        "~/Content/datatables/jquery.dataTables.min.css",
                        "~/Content/datatables/buttons.dataTables.min.css",
                        "~/Content/datatables/select.dataTables.min.css"));

            bundles.Add(new StyleBundle("~/Content/base").Include(
                        "~/css/light.css",
                        "~/Content/base/Site.css"));

            bundles.Add(new StyleBundle("~/Content/apx").Include(
                        "~/Content/Apexchart/apexcharts.css"));

            bundles.Add(new StyleBundle("~/Content/select2").Include(
                        "~/Content/select2/select2.css",
                        "~/Content/select2/select2-bootstrap-5-theme.min.css",
                        "~/Content/select2/select2-bootstrap-5-theme.rtl.min.css"));

            bundles.Add(new StyleBundle("~/Content/sweet").Include(
                        "~/Content/sweetalert2/sweetalert2.css",
                        "~/Content/sweetalert2/sweetalert2.min.css"));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include("~/Scripts/base/jquery-3.4.1.js"));

            bundles.Add(new ScriptBundle("~/bundles/tables").Include(
                       "~/Scripts/datatables/jquery.dataTables.min.js",
                       "~/Scripts/datatables/dataTables.buttons.min.js",
                       "~/Scripts/datatables/buttons.flash.min.js",
                       "~/Scripts/datatables/buttons.html5.min.js",
                       "~/Scripts/datatables/dataTables.select.min.js",
                       "~/Scripts/datatables/dataTables.select.js",
                       "~/Scripts/datatables/dataTables.bootstrap5.min.js",
                       "~/Scripts/datatables/dataTables.fixedColumns.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/select2").Include("~/Scripts/select2/select2.full.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/apexcharts").Include("~/Scripts/apexcharts/apexcharts.js"));

            bundles.Add(new ScriptBundle("~/bundles/jscolor").Include(
                        "~/Scripts/jscolor/jscolor.js",
                        "~/Scripts/jscolor/jscolor.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/base").Include(
                        "~/Scripts/Utilities/util.js",
                        "~/Scripts/base/Site.js"));

            bundles.Add(new ScriptBundle("~/bundles/sweet").Include(
                       "~/Scripts/sweetalert2/sweetalert2.js",
                       "~/Scripts/sweetalert2/sweetalert2.all.min.js",
                       "~/Scripts/sweetalert2/sweetalert2.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/PDF").Include(
                       "~/Scripts/pdf/html2canvas.js",
                       "~/Scripts/pdf/jspdf.js",
                       "~/Scripts/pdf/addimage.js",
                       "~/Scripts/pdf/FileSaver.js"));

            bundles.Add(new ScriptBundle("~/bundles/unobtrusive").Include(
                        "~/Scripts/base/jquery.unobtrusive-ajax.js",
                        "~/Scripts/base/jquery.unobtrusive-ajax.min.js",
                        "~/Scripts/base/jquery.validate-vsdoc.js",
                        "~/Scripts/base/jquery.validate.js",
                        "~/Scripts/base/jquery.validate.min.js",
                        "~/Scripts/base/jquery.validate.unobtrusive.js",
                        "~/Scripts/base/jquery.validate.unobtrusive.min.js"));
            
            bundles.Add(new ScriptBundle("~/bundles/ckeditor").Include(
                        "~/Scripts/ckeditor4/ckeditor.js",
                        "~/Scripts/ckeditor4/build-config.js"));            
        }
    }
}

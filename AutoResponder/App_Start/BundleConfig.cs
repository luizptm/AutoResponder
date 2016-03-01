using System;
using System.Web.Optimization;

namespace AutoResponder
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.IgnoreList.Clear();
            AddDefaultIgnorePatterns(bundles.IgnoreList);

			bundles.Add(
              new ScriptBundle("~/Scripts/vendor_mvc")
				.Include("~/Scripts/bootstrap.js") //version 2.3.2
				.Include("~/Scripts/bootstrap-datepicker.js") //version 2.3.2
				.Include("~/Scripts/bootstrap-datetimepicker_pt-BR.js")
				.Include("~/Scripts/bootstrap-dialog.js")
				.Include("~/Scripts/bootstrap-select.js")
				.Include("~/Scripts/colorbox/jquery.colorbox.js")
				.Include("~/Scripts/colorbox/i18n/jquery.colorbox-pt-br.js")
				.Include("~/Scripts/chosen.jquery.js")
				.Include("~/Scripts/ckeditor/ckeditor.js") //version 3.6.4
				//.Include("~/Scripts/ckeditor_4.3.2/ckeditor.min.js") //version 4.3.2
				//.Include("~/Scripts/ckeditor/adapters/jquery.js")
				.Include("~/Scripts/gridmvc.js")
				//.Include("~/Scripts/gridmvc-ext.js")
				.Include("~/Scripts/gridmvc.lang.pt-br.js")
				.Include("~/Scripts/jquery.plugin.min.js")
				.Include("~/Scripts/jquery.datetimeentry.js")
				.Include("~/Scripts/jquery.datetimeentry-pt.js")
				.Include("~/Scripts/jquery.maskedinput-1.3.1.js")
				//.Include("~/Scripts/jquery.plugin.js")
				.Include("~/Scripts/jquery.rating.js") //version 4.01
				.Include("~/Scripts/jquery-ui.datepicker.pt-br.js")
				.Include("~/Scripts/jquery.unobtrusive-ajax.js")
				.Include("~/Scripts/jquery.validate.js")
				.Include("~/Scripts/additional-methods.js")
				.Include("~/Scripts/jquery.validate.unobtrusive.js")
				.Include("~/Scripts/tag-it.js")
				.Include("~/Scripts/URI.js")
			);

			bundles.Add(
              new ScriptBundle("~/Scripts/vendor_mvc_min")
				.Include("~/Scripts/bootstrap.min.js") //version 2.3.2
				.Include("~/Scripts/bootstrap-datepicker.js") //version 2.3.2
				.Include("~/Scripts/bootstrap-datetimepicker_pt-BR.js")
				.Include("~/Scripts/bootstrap-dialog.min.js")
				.Include("~/Scripts/bootstrap-select.min.js")
				.Include("~/Scripts/colorbox/jquery.colorbox-min.js")
				.Include("~/Scripts/colorbox/i18n/jquery.colorbox-pt-br.js")
				.Include("~/Scripts/chosen.jquery.min.js")
				.Include("~/Scripts/ckeditor/ckeditor.js") //version 3.6.4
				//.Include("~/Scripts/ckeditor_4.3.2/ckeditor.js") //version 4.3.2
				//.Include("~/Scripts/ckeditor/adapters/jquery.js")
				.Include("~/Scripts/gridmvc.min.js")
				//.Include("~/Scripts/gridmvc-ext.js")
				.Include("~/Scripts/gridmvc.lang.pt-br.js")
				.Include("~/Scripts/jquery.plugin.min.js")
				.Include("~/Scripts/jquery.datetimeentry.min.js")
				.Include("~/Scripts/jquery.datetimeentry-pt.js")
				.Include("~/Scripts/jquery.maskedinput-1.3.1.min.js")
				//.Include("~/Scripts/jquery.plugin.min.js")
				.Include("~/Scripts/jquery.rating.min.js") //version 4.01
				.Include("~/Scripts/jquery-ui.datepicker.pt-br.js")
				.Include("~/Scripts/jquery.unobtrusive-ajax.min.js")
				.Include("~/Scripts/jquery.validate.min.js")
				.Include("~/Scripts/additional-methods.min.js")
				.Include("~/Scripts/jquery.validate.unobtrusive.min.js")
				.Include("~/Scripts/tag-it.min.js")
				.Include("~/Scripts/URI.js")
			);

			bundles.Add(
			  new ScriptBundle("~/Scripts/globalize")
				.Include("~/Scripts/globalize/globalize.js")
				.Include("~/Scripts/globalize/cultures/globalize.culture.pt-BR.js")
				.Include("~/Scripts/jquery.validate.globalize.min.js") // depends on 'jquery.validate.js' and 'globalize.js'
			);

			bundles.Add(new ScriptBundle("~/Scripts/jquery")
				.Include("~/Scripts/jquery-1.9.1.js")
				.Include("~/Scripts/bootstrap-select.js")
			);

			bundles.Add(new ScriptBundle("~/Scripts/jquery_min")
				.Include("~/Scripts/jquery-1.9.1.min.js")
				.Include("~/Scripts/bootstrap-select.js")
			);

			bundles.Add(new ScriptBundle("~/Scripts/vendor_validation")
				.Include("~/Scripts/jquery.validate.js")
				.Include("~/Scripts/globalize/globalize.js")
				.Include("~/Scripts/globalize/cultures/globalize.culture.pt-BR.js")
				//.Include("~/Scripts/jquery.validate.globalize.js") // depends on 'jquery.validate.js' and 'globalize.js'
				.Include("~/Scripts/jquery.validate.unobtrusive.js")
				
			);

			bundles.Add(new ScriptBundle("~/Scripts/vendor_validation_min")
				.Include("~/Scripts/jquery.validate.min.js")
				.Include("~/Scripts/globalize/globalize.js")
				.Include("~/Scripts/globalize/cultures/globalize.culture.pt-BR.js")
				//.Include("~/Scripts/jquery.validate.globalize.min.js") // depends on 'jquery.validate.min.js' and 'globalize.js'
				.Include("~/Scripts/jquery.validate.unobtrusive.min.js")
			);
bundles.Add(new ScriptBundle("~/bundles/jquery_admin")
				.Include("~/Scripts/app.Admin.js")
			);
			

			bundles.Add(
             new StyleBundle("~/Content/css_bootstrap")
                .Include("~/Content/bootstrap.css")
				.Include("~/Content/bootstrap-dialog.css")
				.Include("~/Content/bootstrap-mvc-validation.css")
				.Include("~/Content/bootstrap-responsive.css")
				.Include("~/Content/bootstrap-theme.css")
				.Include("~/Content/bootstrap-select.css")
				.Include("~/Content/chosen.css")
				.Include("~/Content/font-awesome.css")
                .Include("~/Content/Gridmvc.css")
				.Include("~/Content/gridmvc.datepicker.css")
				.Include("~/Content/jquery.datetimeentry/jquery.datetimeentry.css")
				.Include("~/Content/jquery.rating.css")
				.Include("~/Content/jquery.tagit.css")
				.Include("~/Content/jquery-ui-1.10.3.custom.min.css")
				.Include("~/Content/tagit.ui-zendesk.css")
				.Include("~/Content/PagedList.css")
             );

			bundles.Add(
             new StyleBundle("~/Content/css_bootstrap_min")
                .Include("~/Content/bootstrap.min.css")
				.Include("~/Content/bootstrap-dialog.min.css")
				.Include("~/Content/bootstrap-mvc-validation.css")
				.Include("~/Content/bootstrap-responsive.min.css")
				.Include("~/Content/bootstrap-theme.mincss")
				.Include("~/Content/bootstrap-select.min.css")
				.Include("~/Content/chosen.css")
				.Include("~/Content/font-awesome.min.css")
                .Include("~/Content/Gridmvc.css")
				.Include("~/Content/gridmvc.datepicker.min.css")
				.Include("~/Content/jquery.rating.css")
				.Include("~/Content/jquery.tagit.css")
				.Include("~/Content/jquery-ui-1.10.3.custom.min.css")
				.Include("~/Content/tagit.ui-zendesk.css")
				.Include("~/Content/PagedList.css")
             );

			BundleTable.EnableOptimizations = false;//change on production!
        }

        public static void AddDefaultIgnorePatterns(IgnoreList ignoreList)
        {
            if (ignoreList == null)
            {
                throw new ArgumentNullException("ignoreList");
            }

            ignoreList.Ignore("*.intellisense.js");
            ignoreList.Ignore("*-vsdoc.js");

            //ignoreList.Ignore("*.debug.js", OptimizationMode.WhenEnabled);
            //ignoreList.Ignore("*.min.js", OptimizationMode.WhenDisabled);
            //ignoreList.Ignore("*.min.css", OptimizationMode.WhenDisabled);
        }
    }
}
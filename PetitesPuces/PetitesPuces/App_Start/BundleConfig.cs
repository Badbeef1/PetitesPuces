using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace PetitesPuces.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundle(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/jquerybootstrap").Include(
                "~/Scripts/jquery-3.3.3.mini.js",
                "~/Scripts/bootstrap.mini.js"));
        }
    }
}
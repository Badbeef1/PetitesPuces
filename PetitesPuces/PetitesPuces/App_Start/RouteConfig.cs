using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace PetitesPuces
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}",
                defaults: new { controller = "Home", action = "accueil_internaute" }
            );

            routes.MapRoute(
            name: "Inactivite",
            url: "{controller}/{action}/{id}",
            defaults: new { controller = "Administrateur", action = "GestionInactivite", id = UrlParameter.Optional }
);
        }
    }
}

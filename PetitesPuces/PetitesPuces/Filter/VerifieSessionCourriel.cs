﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace PetitesPuces.Filter
{
    public class VerifieSessionCourriel : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var Session = filterContext.HttpContext.Session;

            if ((Session["clientObj"] == null) && (Session["vendeurObj"] == null) && (Session["gestionnaireObj"] == null))
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Internaute" }));
            }
            else
            {
                if (Session["retour"] == null)
                {
                    Session["retour"] = filterContext.HttpContext.Request.Url;
                }
                else if (filterContext.HttpContext.Request.UrlReferrer.AbsolutePath != filterContext.HttpContext.Request.Url.AbsolutePath)
                {
                    Session["retour"] = filterContext.HttpContext.Request.UrlReferrer.AbsoluteUri;
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
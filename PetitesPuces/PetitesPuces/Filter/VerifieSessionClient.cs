using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;

namespace PetitesPuces.Filter
{
    public class VerifieSessionClient : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var Session = filterContext.HttpContext.Session;

            if (filterContext.ActionDescriptor.ActionName != "ProduitDetaille")
            {
                if (Session["vendeurObj"] != null)
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Vendeur" }));
                }
                else if (Session["gestionnaireObj"] != null)
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Gestionnaire" }));
                }
                else if (Session["clientObj"] == null)
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Internaute" }));
                }
                else
                {
                    try
                    {
                        if (filterContext.ActionDescriptor.ActionName == "ConfirmationTransaction" || filterContext.ActionDescriptor.ActionName == "VoirPDFCommande")
                        {

                        }
                        else if (Session["retour"] == null)
                        {
                            Session["retour"] = filterContext.HttpContext.Request.Url;
                        }
                        else if (filterContext.HttpContext.Request.UrlReferrer.AbsolutePath != filterContext.HttpContext.Request.Url.AbsolutePath)
                        {
                            Session["retour"] = filterContext.HttpContext.Request.UrlReferrer.AbsoluteUri;
                        }
                    }catch(NullReferenceException )
                    {
                        if ((filterContext.HttpContext.Request.UrlReferrer is null) && (filterContext.HttpContext.Request.Url.AbsolutePath != "/Client") && (Session["retour"] != null))
                        {
                            Session["retour"] = null;
                            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Client" }));
                        }
                    }
                }
            }
            else
            {
                if ((Session["clientObj"] == null) && (Session["vendeurObj"] == null) && (Session["gestionnaireObj"] == null))
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Internaute" }));
                }
                else
                {
                    try
                    {
                        if (filterContext.ActionDescriptor.ActionName == "ConfirmationTransaction" || filterContext.ActionDescriptor.ActionName == "VoirPDFCommande")
                        {

                        }
                        else if (Session["retour"] == null)
                        {
                            Session["retour"] = filterContext.HttpContext.Request.Url;
                        }
                        else if (filterContext.HttpContext.Request.UrlReferrer.AbsolutePath != filterContext.HttpContext.Request.Url.AbsolutePath)
                        {
                            Session["retour"] = filterContext.HttpContext.Request.UrlReferrer.AbsoluteUri;
                        }
                    }catch(NullReferenceException )
                    {
                        if ((filterContext.HttpContext.Request.UrlReferrer is null) && (filterContext.HttpContext.Request.Url.AbsolutePath != "/Client") && (Session["retour"]  != null))
                        {
                            Session["retour"] = null;
                            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Client" }));
                        }
                    }   
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}

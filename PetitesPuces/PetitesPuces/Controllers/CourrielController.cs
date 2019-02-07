using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PetitesPuces.Views
{
    public class CourrielController : Controller
    {
        // GET: Courriel
        public ActionResult Index(string page)
        {
            if (page != "")
            {
                ViewBag.TypePg = "Nouveau";
            }
            else
            {
                ViewBag.TypePg = "Liste";
            }

            return View();
        }
    }
}
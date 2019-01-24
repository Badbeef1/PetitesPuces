using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PetitesPuces.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View("accueil_internaute");
        }
        public ActionResult accueil_gestionnaire()
        {
            return View();
        }
        public ActionResult accueil_vendeur()
        {
            return View();
        }
        public ActionResult accueil_internaute()
        {
            return View();
        }
        public ActionResult home_client()
        {
            return View();
        }

    }
}
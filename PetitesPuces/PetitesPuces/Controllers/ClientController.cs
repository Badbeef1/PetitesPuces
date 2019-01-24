using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PetitesPuces.Controllers
{
    public class ClientController : Controller
    {
        // GET: Client
        public ActionResult SaisieCommande()
        {
            return View();
        }

        // GET: Catalogue
        public ActionResult Catalogue()
        {
            return View();
        }

        // GET: ProduitDetail
        public ActionResult ProduitDetaille()
        {
            return View();
        }
    }
}
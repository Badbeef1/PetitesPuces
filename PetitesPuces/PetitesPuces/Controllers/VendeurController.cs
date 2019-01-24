using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PetitesPuces.Controllers
{
    public class VendeurController : Controller
    {
        // GET: Vendeur
        public ActionResult GestionCommande()
        {
            return View();
        }

        //GET: GestionProduit
        public ActionResult GestionProduit()
        {
            return View();
        }
    }
}
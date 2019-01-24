using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PetitesPuces.Controllers
{
    public class VendeurController : Controller
    {
        public ActionResult Index() => View("AccueilVendeur");

        public ActionResult AccueilVendeur() => View();

        // GET: Vendeur
        public ActionResult GestionCommande() => View();

        //GET: GestionProduit
        public ActionResult GestionProduit() => View();

        //GET: CatalogueVendeur
        public ActionResult CatalogueVendeur() => View();

        public ActionResult GestionProfileVendeur() => View();
    }
}
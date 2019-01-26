using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PetitesPuces.Controllers
{
    public class ClientController : Controller
    {
        public ActionResult Index() => View("AccueilClient");

        public ActionResult AccueilClient() => View();

        // GET: Client
        public ActionResult SaisieCommande() => View();

        // GET: Cataloguess
        public ActionResult Catalogue() => View();

        // GET: ProduitDetail
        public ActionResult ProduitDetaille() => View();

        public ActionResult GestionProfilClient() => View();
        
        //Panier Détaillé du client
        public ActionResult PanierDetail() => View();
   }
}
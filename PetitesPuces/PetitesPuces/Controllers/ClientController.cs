using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PetitesPuces.Models;

namespace PetitesPuces.Controllers
{
    public class ClientController : Controller
    {
        Models.DataClasses1DataContext contextPP = new Models.DataClasses1DataContext();

        public ActionResult Index() => View("AccueilClient");

      public ActionResult AccueilClient()
      {
        String noClient = "10000";
        //long noClient = ((Models.PPClients)Session["clientObj"]).NoClient;
        /* Compare data with Database */
        Models.DataClasses1DataContext db = new Models.DataClasses1DataContext();
         db.Connection.Open();

         //Requête qui va permettre d'aller chercher les paniers du client
         var paniers = from panier in db.GetTable<Models.PPArticlesEnPanier>()
                          where panier.NoClient.Equals(noClient)
                          group panier by panier.PPVendeurs;

         db.Connection.Close();

         return View(paniers);
      }

      // GET: Client
      public ActionResult SaisieCommande() => View();

      // GET: Cataloguess
      public ActionResult Catalogue() => View();

      // GET: ProduitDetail
      public ActionResult ProduitDetaille() => View();


      //Panier Détaillé du client
      public ActionResult PanierDetail(int id)
      {
         Models.DataClasses1DataContext db = new Models.DataClasses1DataContext();
         db.Connection.Open();
            //requête pour aller chercher les produits à l'aide d'un vendeur
            List<PPArticlesEnPanier> items = (from panier in db.GetTable<Models.PPArticlesEnPanier>()
                     where panier.NoClient.Equals(10000) && panier.NoVendeur.Equals(id)
                     select panier).ToList();

         return View(items);
      }
        
        [HttpPost]
        public ActionResult PanierDetail(int id,List<PPArticlesEnPanier> model)
        {
            var rnd = model;
            return View();
        }
        public ActionResult GestionProfilClient()
        {
            //HttpContext.User.Identity.Name
            String strAdresseCourrielClient = "Client10000@cgodin.qc.ca";

            var client = from unClient in contextPP.PPClients
                         where unClient.AdresseEmail == strAdresseCourrielClient
                         select unClient;

            List<Models.Province> lstProvinces = new List<Models.Province>
            {
                new Models.Province { Abreviation = "NB", Nom = "Nouveau-Brunswick"},
                new Models.Province { Abreviation = "ON", Nom = "Ontario"},
                new Models.Province { Abreviation = "QC", Nom = "Québec"},
            };

            ViewBag.ListeProvinces = new SelectList(lstProvinces, "Abreviation", "Nom");

            
            return View(client.First());
        }

        //Vue partiel Information personnel
        [ChildActionOnly]
        public ActionResult InformationPersonnel()
        {
            return PartialView();
        }

        //Vue partiel modification du mot de passe
        [ChildActionOnly]
        public ActionResult ModificationMDP() => PartialView();
   }
}
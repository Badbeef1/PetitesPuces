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

      public ActionResult AccueilClient()
      {
         /* Compare data with Database */
         Models.DataClasses1DataContext db = new Models.DataClasses1DataContext();
         db.Connection.Open();

         //Requête qui va permettre d'aller chercher les paniers du client
         var paniers = from panier in db.GetTable<Models.PPArticlesEnPanier>()
                          where panier.NoClient.Equals(10000)
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

      public ActionResult GestionProfilClient() => View();

      //Panier Détaillé du client
      public ActionResult PanierDetail(int id)
      {
         Models.DataClasses1DataContext db = new Models.DataClasses1DataContext();
         db.Connection.Open();
         //requête pour aller chercher les produits à l'aide d'un vendeur
         var items = from panier in db.GetTable<Models.PPArticlesEnPanier>()
                     where panier.NoClient.Equals(10000) && panier.NoVendeur.Equals(id)
                     select panier;

         return View(items);
      }
   }
}
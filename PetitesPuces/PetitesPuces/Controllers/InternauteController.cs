using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PetitesPuces.Controllers
{
   public class InternauteController : Controller
   {
      // GET: Inscription
      public ActionResult Index() => View("AccueilInternaute");

      public ActionResult AccueilInternaute()
      {
         /* Compare data with Database */
         Models.DataClasses1DataContext db = new Models.DataClasses1DataContext();
         db.Connection.Open();
         var categories = (from cat in db.GetTable<Models.PPCategories>() select cat);

         db.Connection.Close();

         return View(categories);
      }


        public ActionResult Inscription(Models.PPClientViewModel model)
        {
            if (model.vendeur != null)
            {
                /* Variables required for client registration */
                var username = model.vendeur.AdresseEmail;
                var confUsername = model.confirmUsername;
                var password = model.vendeur.MotDePasse;
                var confPassword = model.confirmPassword;

                /* Variables required for seller registration */
                var businessName = model.vendeur.NomAffaires;
                var lastName = model.vendeur.Nom;
                var firstName = model.vendeur.Prenom;
                var street = model.vendeur.Rue;
                var city = model.vendeur.Ville;
                var province = model.vendeur.Province;
                var postalCode = model.vendeur.CodePostal;
                var country = model.vendeur.Pays;
                var tel1 = model.vendeur.Tel1;
                var tel2 = model.vendeur.Tel2;
                var freeDelivery = model.vendeur.LivraisonGratuite;
                var weightDelivery = model.vendeur.PoidsMaxLivraison;
                var taxes = model.vendeur.Taxes;


            }


            return View();
        }

      [HttpPost]
      public ActionResult VerifyEntry() => null;

   }
}
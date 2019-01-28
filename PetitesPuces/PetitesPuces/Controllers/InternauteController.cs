using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using PetitesPuces.Models;

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

      public ActionResult CatalogueNouveaute() => View();



      //GET
      public ActionResult Inscription() => View();


      [HttpPost] //POST
      public ActionResult Inscription(Models.PPClientViewModel model)
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

         var clientSectionValide = (username ?? confUsername ?? password ?? confPassword) != null &&
                                   username == confUsername && password == confPassword;
         
         var vendeurSectionValide = (businessName ?? lastName ?? firstName ?? street ?? city ?? province ??
                                     postalCode ?? tel1) != null && (freeDelivery ?? weightDelivery) != null;
         
            var etreVendeur = model.boolVendeur;

         var context = new DataClasses1DataContext();

         if (clientSectionValide && !etreVendeur)
         {
            //Register client  
            context.Connection.Open();

            context.Connection.Close();
         }
         else if (clientSectionValide && vendeurSectionValide && ModelState.IsValid)
         {
            //Register vendeur
            context.Connection.Open();

            using (var transaction = new TransactionScope())
            {
               try
               {
                  model.vendeur.DateCreation = DateTime.Now;

                  var max = (context.PPVendeurs.Max(x => x.NoVendeur) + 1);
                  model.vendeur.NoVendeur = max > 10 ? max : 11;
                  context.PPVendeurs.InsertOnSubmit(model.vendeur);

                  context.SubmitChanges();
                  model.okMessage = "L'ajout dans la base de données a réussi. ";
                  transaction.Complete();
               }
               catch (Exception ex)
               {
                  model.errorMessage = "L'ajout dans la base de données a échoué. " + ex.Message;
               }
            }

            context.Connection.Close();

            if (model.okMessage != null)
               return RedirectToAction("Index","Connexion", model);
         }
         else if(!etreVendeur)
         {
            //Clear errors in Vendeur section
            foreach (var item in ModelState.Keys.Where(s => !s.Equals("vendeur.AdresseEmail") &&
                    !s.Equals("vendeur.MotDePasse") && !s.Equals("confirmUsername") && !s.Equals("confirmPassword")))
            {
               ModelState[item].Errors.Clear();
            }
         }
         

         return View(model);
      }

      [HttpPost]
      public ActionResult VerifyEntry() => null;


   }
}
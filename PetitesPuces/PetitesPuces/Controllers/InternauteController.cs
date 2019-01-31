using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using PetitesPuces.Models;


namespace PetitesPuces.Controllers
{
   public class InternauteController : Controller
   {
      // GET: Inscription
      public ActionResult Index()
      {
         List<Models.EntrepriseCategorie> lstEntreCate = new List<Models.EntrepriseCategorie>();
         /* Compare data with Database */
         Models.DataClasses1DataContext db = new Models.DataClasses1DataContext();
         db.Connection.Open();
         var toutesCategories = (from cat in db.GetTable<Models.PPCategories>()
               select cat
            );
         foreach (var cat in toutesCategories)
         {
            List<PPVendeurs> lstVendeurs = new List<PPVendeurs>();
            var query = (from prod in db.GetTable<Models.PPProduits>()
                  where prod.NoCategorie.Equals(cat.NoCategorie)
                  select prod
               );
            foreach (var item in query)
            {
               if (!lstVendeurs.Contains(item.PPVendeurs))
               {
                  lstVendeurs.Add(item.PPVendeurs);
               }
            }
            lstEntreCate.Add(new Models.EntrepriseCategorie(cat, lstVendeurs));
         }
         return View("AccueilInternaute",lstEntreCate);
      }

      public ActionResult AccueilInternaute()
      {
         List<Models.EntrepriseCategorie> lstEntreCate = new List<Models.EntrepriseCategorie>();
         /* Compare data with Database */
         Models.DataClasses1DataContext db = new Models.DataClasses1DataContext();
         db.Connection.Open();
         var toutesCategories = (from cat in db.GetTable<Models.PPCategories>()
                                 select cat
                                 );
         foreach (var cat in toutesCategories)
         {
            List<PPVendeurs> lstVendeurs = new List<PPVendeurs>();
            var query = (from prod in db.GetTable<Models.PPProduits>()
                         where prod.NoCategorie.Equals(cat.NoCategorie)
                         select prod
                         );
            foreach (var item in query)
            {
               if (!lstVendeurs.Contains(item.PPVendeurs))
               {
                  lstVendeurs.Add(item.PPVendeurs);
               }
            }
            lstEntreCate.Add(new Models.EntrepriseCategorie(cat, lstVendeurs));
         }
         return View(lstEntreCate);
      }

      public ActionResult CatalogueNouveaute()
      {
         Models.DataClasses1DataContext db = new Models.DataClasses1DataContext();
         db.Connection.Open();
         var nouveauProduit = (from prod in db.GetTable<Models.PPProduits>()
                               orderby prod.DateCreation descending
                               select prod
                               ).ToList();
         db.Connection.Close();

         return View(nouveauProduit);
      }

      public ActionResult ProduitDetaille(long noProduit)
      {
         PPProduits produitDetail;
         Models.DataClasses1DataContext db = new Models.DataClasses1DataContext();
         db.Connection.Open();

         var produit = (from prod in db.GetTable<PPProduits>()
                        where prod.NoProduit.Equals(noProduit)
                        select prod
                        );
         produitDetail = produit.First();

         db.Connection.Close();
         return View(produitDetail);
      }

      //GET
      public ActionResult Inscription() => View();


      [HttpPost] //POST
      [ValidateAntiForgeryToken]
      public ActionResult Inscription(Models.PPClientViewModel model)
      {

         /* Variables required for client registration */
         var username = model.vendeur.AdresseEmail;
         var confUsername = model.confirmUsername;
         var password = model.vendeur.MotDePasse;
         var confPassword = model.confirmPassword;
         var clientSectionValid = (username != null && password != null);
         var usernameConfirmValid = username == confUsername;
         var passwordConfirmValid = password == confPassword;

         /* Variables required for seller registration */
         var etreVendeur = model.boolVendeur;

         //reset
         model.errorMessage = "";
         model.okMessage = "";

         /* Some validations */
         try
         {
            MailAddress ma = new MailAddress(username);
         }
         catch (Exception)
         {
            ModelState["vendeur.AdresseEmail"].Errors.Add("Le format de l'adresse courriel n'est pas valide !");
            model.errorMessage = " ";
         }

         if (!usernameConfirmValid)
         {
            ModelState[nameof(model.confirmUsername)].Errors.Add("Le deuxième courriel doit correspondre au premier.");
            model.errorMessage = " ";
         }

         if (!passwordConfirmValid)
         {
            ModelState[nameof(model.confirmPassword)].Errors.Add("Le deuxième mot de passe doit correspondre au premier.");
            model.errorMessage = " ";
         }

         if (!etreVendeur) // "Je veux etre vendeur" is not checked
         {
            //Clear errors in Vendeur section
            foreach (var item in ModelState.Keys.Where(s => !s.Equals("vendeur.AdresseEmail") &&
                    !s.Equals("vendeur.MotDePasse") && !s.Equals("confirmUsername") && !s.Equals("confirmPassword")))
            {
               ModelState[item].Errors.Clear();
            }
         }

         /* Database section */
         var context = new DataClasses1DataContext();

         if (clientSectionValid && !etreVendeur && usernameConfirmValid && passwordConfirmValid)
         {
            //
            //Register client
            //
            try
            {
               context.Connection.Open();
            }
            catch (Exception)
            {
               model.errorMessage = "Connexion à la base de donnée échouée !";
               return View(model);
            }

            var max = (context.PPClients.Max(x => x.NoClient) + 1);

            //Maximum amount of clients reached
            if (max >= 99999)
               model.errorMessage = "Nous avons atteint le nombre maximum de vendeurs (100).";

            //Email exists             
            else if (context.PPClients.Any(x => x.AdresseEmail.ToLower().Equals(model.vendeur.AdresseEmail.ToLower())))
            {
               ModelState["vendeur.AdresseEmail"].Errors.Add("Ce courriel est déjà inscrit !");
               model.errorMessage = " ";
            }

            //Stop right there if theres an error
            if (model.errorMessage != "") return View(model);

            //Transaction
            using (var transaction = new TransactionScope())
            {
               try
               {
                  context.PPClients.InsertOnSubmit(new PPClients()
                  {
                     NoClient = max > 10000 ? max : 10001,
                     AdresseEmail = model.vendeur.AdresseEmail,
                     MotDePasse = model.vendeur.MotDePasse,
                     DateCreation = DateTime.Now,
                     Statut = 1
                  });

                  context.SubmitChanges();
                  transaction.Complete();
                  ModelState.Clear();

                  context.Connection.Close();
                  return View(new PPClientViewModel() { okMessage = "L'ajout dans la base de données a réussi. " });
               }
               catch (Exception ex)
               {
                  model.errorMessage = "L'ajout dans la base de données a échoué. " + ex.Message;
               }
            }

            context.Connection.Close();

         }
         else if (clientSectionValid && usernameConfirmValid && passwordConfirmValid && etreVendeur && ModelState.IsValid)
         {
            //
            //Register seller
            //
            try
            {
               context.Connection.Open();
            }
            catch (Exception)
            {
               model.errorMessage = "Connexion à la base de donnée échouée !";
               return View(model);
            }

            var max = (context.PPVendeurs.Max(x => x.NoVendeur) + 1);

            //Maximum amount of sellers reached
            if (max >= 100)
               model.errorMessage = "Nous avons atteint le nombre maximum de vendeurs (100).";

            //Email exists
            else if (context.PPVendeurs.Any(x => x.AdresseEmail.ToLower().Equals(model.vendeur.AdresseEmail.ToLower())))
            {
               ModelState["vendeur.AdresseEmail"].Errors.Add("Ce courriel est déjà inscrit !");
               model.errorMessage = " ";
            }

            //Company name exists
            else if (context.PPVendeurs.Any(x => x.NomAffaires.ToLower().Equals(model.vendeur.NomAffaires.ToLower())))
            {
               ModelState["vendeur.NomAffaires"].Errors.Add("Le nom d'entreprise existe déjà !");
               model.errorMessage = " ";
            }

            //Stop right there if theres an error
            if (model.errorMessage != "") return View(model);

            //Transaction
            using (var transaction = new TransactionScope())
            {
               try
               {
                  model.vendeur.DateCreation = DateTime.Now;

                  model.vendeur.NoVendeur = max > 10 ? max : 11;
                  model.vendeur.Statut = 0;
                  context.PPVendeurs.InsertOnSubmit(model.vendeur);

                  context.SubmitChanges();
                  transaction.Complete();

                  ModelState.Clear();

                  context.Connection.Close();
                  return View(new PPClientViewModel() { okMessage = "L'ajout dans la base de données a réussi. " });
               }
               catch (Exception ex)
               {
                  model.errorMessage = "L'ajout dans la base de données a échoué. " + ex.Message;
               }
            }

            context.Connection.Close();
         }

         return View(model);
      }

      [HttpPost]
      public ActionResult VerifyEntry() => null;


   }
}
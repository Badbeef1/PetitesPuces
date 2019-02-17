using System;
using System.Linq;
using System.Web.Mvc;
using PetitesPuces.Models;

namespace PetitesPuces.Controllers
{
   public class ConnexionController : Controller
   {
      // GET: Login
      public ActionResult Index()
      {
         ViewBag.Message = "";
         if (Session["clientObj"] != null)
            return RedirectToAction("AccueilClient", "Client");
         if (Session["vendeurObj"] != null)
            return RedirectToAction("AccueilVendeur", "Vendeur");
         if (Session["gestionnaireObj"] != null)
            return RedirectToAction("AccueilGestionnaire", "Gestionnaire");

         return View();
      }

      [HttpPost]
      public ActionResult VerifyLogin(PPClientViewModel model)
      {
         ViewBag.Message = "";
         var username = model.client.AdresseEmail?.ToLower();
         var password = model.client.MotDePasse;

         /* First checks  (Empty) */
         if (username == null || password == null)
         {
            model.errorMessage = "Vous avez oublié au moins un champ !";
            model.client.MotDePasse = (password != null) ? "" : null;  // null = red outline, "" = none
            return View("Index", model);
         }

         /* Compare data with Database */
         using (DataClasses1DataContext db = new DataClasses1DataContext())
         {
            var clientLogged = db.PPClients.FirstOrDefault(x => x.AdresseEmail.ToLower() == username && x.MotDePasse == password);

            var vendeurLogged = db.PPVendeurs.FirstOrDefault(x => x.AdresseEmail.ToLower() == username && x.MotDePasse == password);

            var gestionnaireLogged = db.PPGestionnaire.FirstOrDefault(x => x.AdresseEmail.ToLower() == username && x.MotDePasse == password);

            if (clientLogged != null && clientLogged.Statut == 1)
            {
               Session["clientObj"] = clientLogged;
               clientLogged.NbConnexions += 1;
               clientLogged.DateDerniereConnexion = DateTime.Now;
               db.SubmitChanges();

               ((PPClients)Session["clientObj"]).MotDePasse = "";
               return RedirectToAction("AccueilClient", "Client");
            }

            if (vendeurLogged != null && vendeurLogged.Statut == 1)
            {
               Session["vendeurObj"] = vendeurLogged;
               ((PPVendeurs)Session["vendeurObj"]).MotDePasse = "";
               return RedirectToAction("AccueilVendeur", "Vendeur");
            }

            if (gestionnaireLogged != null)
            {
               Session["gestionnaireObj"] = gestionnaireLogged;
               ((PPGestionnaire)Session["gestionnaireObj"]).MotDePasse = "";
               return RedirectToAction("AccueilGestionnaire", "Gestionnaire");
            }


            model.errorMessage = clientLogged?.Statut == 2 || vendeurLogged?.Statut == 0 || vendeurLogged?.Statut == 2
                ? "Votre compte n'est pas actif."
                : "Le courriel ou le mot de passe n'est pas valide.";

            model.client.MotDePasse = ""; //No red border
         }
         return View("Index", model);
      }

      public ActionResult RecupererMDP()
      {
         ViewBag.msgErreur = "";
         return View();
      }

      [HttpPost]
      public ActionResult RecupererMDP(ValiderMotPasse validerMotPasse)
      {
         ViewBag.msgErreur = "";
         Models.DataClasses1DataContext db = new Models.DataClasses1DataContext();
         db.Connection.Open();
         var estClient = (from client in db.GetTable<PPClients>()
                          where client.AdresseEmail.Equals(validerMotPasse.courriel)
                          select client
                          ).ToList();
         var estVendeur = (from vendeur in db.GetTable<PPVendeurs>()
                           where vendeur.AdresseEmail.Equals(validerMotPasse.courriel)
                           select vendeur
                           ).ToList();
         var estGestionnaire = (from gestionnaire in db.GetTable<PPGestionnaire>()
                                where gestionnaire.AdresseEmail.Equals(validerMotPasse.courriel)
                                select gestionnaire
                                ).ToList();

         if((estClient.Count > 0) || (estVendeur.Count > 0) || (estGestionnaire.Count() > 0))
         {
            //C'est un email qui a un compte associé
            NouveauMDP nMDP = new NouveauMDP();
            nMDP.courriel = validerMotPasse.courriel;

            return View("ChangerMDP",nMDP);
         }
         else
         {
            ViewBag.msgErreur = "Il n'y pas de compte associé à ce courriel";
            db.Connection.Close();
            return View();
         }
      }

      public ActionResult ChangerMDP()
      {

         return View("Internaute/AccueilInternaute");
      }
      [HttpPost]
      public ActionResult ChangerMDP(NouveauMDP nMDP)
      {
         ViewBag.msgErreur = "";
         Models.DataClasses1DataContext db = new Models.DataClasses1DataContext();
         db.Connection.Open();
         //LES DEUX PASSWORD sont valides
         if (ModelState.IsValid)
         {
            if (nMDP.password1.Equals(nMDP.password2))
            {
               var estClient = (from client in db.GetTable<PPClients>()
                                where client.AdresseEmail.Equals(nMDP.courriel)
                                select client
                             ).ToList();
               var estVendeur = (from vendeur in db.GetTable<PPVendeurs>()
                                 where vendeur.AdresseEmail.Equals(nMDP.courriel)
                                 select vendeur
                                 ).ToList();
               var estGestionnaire = (from gestionnaire in db.GetTable<PPGestionnaire>()
                                      where gestionnaire.AdresseEmail.Equals(nMDP.courriel)
                                      select gestionnaire
                                      ).ToList();
               if (estClient.Count() > 0)
               {
                  PPClients client = estClient.First();
                  client.MotDePasse = nMDP.password1;
               }
               else if (estVendeur.Count() > 0)
               {
                  PPVendeurs vendeur = estVendeur.First();
                  vendeur.MotDePasse = nMDP.password1;
               }
               else
               {
                  PPGestionnaire gestionnaire = estGestionnaire.First();
                  gestionnaire.MotDePasse = nMDP.password1;
               }
               try
               {
                  db.SubmitChanges();
               }
               catch (Exception e)
               {

               }

               db.Connection.Close();
               ViewBag.Message = "Mot de passe modifié avec succès";
               return View("Index");
            }
            else
            {
               ViewBag.msgErreur = "Les mots de passes ne sont pas identiques.";
               return View(nMDP);
            }
         }
         else
         {
            return View(nMDP);
         }
         
      }
      public ActionResult Deconnexion()
      {
         Session.Abandon();
         return View();
      }
   }
}
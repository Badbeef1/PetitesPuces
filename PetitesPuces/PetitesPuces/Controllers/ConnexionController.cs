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


            model.errorMessage = clientLogged?.Statut == 0 || vendeurLogged?.Statut == 0
                ? "Votre compte n'est pas actif."
                : "Le courriel ou le mot de passe n'est pas valide.";

            model.client.MotDePasse = ""; //No red border
         }
         return View("Index", model);
      }

      public ActionResult RecupererMDP() => View();

      public ActionResult Deconnexion()
      {
         Session.Abandon();
         return View();
      }
   }
}
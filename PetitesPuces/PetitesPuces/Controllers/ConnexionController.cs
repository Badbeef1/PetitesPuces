using System.Web.Mvc;
using System.Linq;

namespace PetitesPuces.Controllers
{
   public class ConnexionController : Controller
   {
      // GET: Login
      public ActionResult Index() => View();

      [HttpPost]
      public ActionResult VerifyLogin(Models.PPClientViewModel model)
      {
         var username = model.client.AdresseEmail?.ToLower();
         var password = model.client.MotDePasse;


         /* First checks  (Empty / IsAdmin) */
         if (username == null || password == null)
         {
            model.errorMessage = "Vous avez oublié au moins un champ !";
            model.client.MotDePasse = (password != null) ? "" : null;  // null = red outline, "" = none
            return View("Index", model);
         }
         else if (username.ToLower().Equals("admin") && password == "Secret123")
         {
            Session["username"] = "Administrateur";
            return RedirectToAction("Index", "Gestionnaire");
         }


         /* Compare data with Database */
         Models.DataClasses1DataContext db = new Models.DataClasses1DataContext();
         db.Connection.Open();
         var clientLogged = (from cli in db.GetTable<Models.PPClients>()
                             where cli.AdresseEmail.ToLower() == username &&
                            cli.MotDePasse == password
                             select cli).FirstOrDefault();

         var vendeurLogged = (from ven in db.GetTable<Models.PPVendeurs>()
                              where ven.AdresseEmail.ToLower() == username.ToLower() &&
                             ven.MotDePasse == password
                              select ven).FirstOrDefault();

         var gestionnaireLogged = (from gest in db.GetTable<Models.PPGestionnaire>()
                                   where gest.AdresseEmail.ToLower() == username.ToLower() &&
                                         gest.MotDePasse == password
                                   select gest).FirstOrDefault();

         db.Connection.Close();


         if (clientLogged != null && clientLogged.Statut == 1)
         {
            clientLogged.MotDePasse = "";
            Session["clientObj"] = clientLogged;
            return RedirectToAction("AccueilClient", "Client");
         }
         else if (vendeurLogged != null && vendeurLogged.Statut == 1)
         {
            vendeurLogged.MotDePasse = "";
            Session["vendeurObj"] = vendeurLogged;
            return RedirectToAction("AccueilVendeur", "Vendeur");
         }
         else if (gestionnaireLogged != null)
         {
            gestionnaireLogged.MotDePasse = "";
            Session["gestionnaireObj"] = gestionnaireLogged;
            return RedirectToAction("AccueilGestionnaire", "Gestionnaire");
         }
         else
         {
            model.errorMessage = "Le courriel ou le mot de passe n'est pas valide.";
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
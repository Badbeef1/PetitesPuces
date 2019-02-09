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


            /* First checks  (Empty / IsAdmin) */
            if (username == null || password == null)
            {
                model.errorMessage = "Vous avez oublié au moins un champ !";
                model.client.MotDePasse = (password != null) ? "" : null;  // null = red outline, "" = none
                return View("Index", model);
            }


            /* Compare data with Database */
            DataClasses1DataContext db = new DataClasses1DataContext();
            db.Connection.Open();
            var clientLogged = (from cli in db.GetTable<PPClients>()
                                where cli.AdresseEmail.ToLower() == username &&
                               cli.MotDePasse == password
                                select cli).FirstOrDefault();

            var vendeurLogged = (from ven in db.GetTable<PPVendeurs>()
                                 where ven.AdresseEmail.ToLower() == username.ToLower() &&
                                ven.MotDePasse == password
                                 select ven).FirstOrDefault();

            var gestionnaireLogged = (from gest in db.GetTable<PPGestionnaire>()
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

            if (vendeurLogged != null && vendeurLogged.Statut == 1)
            {
                vendeurLogged.MotDePasse = "";
                Session["vendeurObj"] = vendeurLogged;
                return RedirectToAction("AccueilVendeur", "Vendeur");
            }

            if (gestionnaireLogged != null)
            {
                gestionnaireLogged.MotDePasse = "";
                Session["gestionnaireObj"] = gestionnaireLogged;
                return RedirectToAction("AccueilGestionnaire", "Gestionnaire");
            }

            model.errorMessage = clientLogged?.Statut == 0 || vendeurLogged?.Statut == 0
                ? "Votre compte n'est pas actif."
                : "Le courriel ou le mot de passe n'est pas valide.";

            model.client.MotDePasse = ""; //No red border

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
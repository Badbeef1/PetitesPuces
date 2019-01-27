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
            db.Connection.Close();


            if (clientLogged != null)
            {
                clientLogged.MotDePasse = "";
                Session["clientObj"] = vendeurLogged;
                return RedirectToAction("Index", "Client");
            }
            else if (vendeurLogged != null)
            {
                vendeurLogged.MotDePasse = "";
                Session["vendeurObj"] = vendeurLogged;
                return RedirectToAction("Index", "Vendeur");
            }
            else
            {
                model.errorMessage = "Le courriel ou le mot de passe n'est pas valide.";
                model.client.MotDePasse = ""; //No red border
            }

            return View("Index", model);
        }

        public ActionResult RecupererMDP() => View();

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PetitesPuces.Controllers
{
    public class ClientController : Controller
    {
        Models.DataClasses1DataContext contextPP = new Models.DataClasses1DataContext();

        public ActionResult Index() => View("AccueilClient");

        public ActionResult AccueilClient() => View();

        // GET: Client
        public ActionResult SaisieCommande() => View();

        // GET: Cataloguess
        public ActionResult Catalogue() => View();

        // GET: ProduitDetail
        public ActionResult ProduitDetaille() => View();

        public ActionResult GestionProfilClient()
        {
            //HttpContext.User.Identity.Name
            String strAdresseCourrielClient = "Client10000@cgodin.qc.ca";

            var client = from unClient in contextPP.PPClients
                         where unClient.AdresseEmail == strAdresseCourrielClient
                         select unClient;

            //client.ToList().ForEach(test => System.Diagnostics.Debug.WriteLine(test.AdresseEmail));
            

            return View(client.First());
        }
        
        //Panier Détaillé du client
        public ActionResult PanierDetail() => View();

        //Vue partiel Information personnel
        [ChildActionOnly]
        public ActionResult InformationPersonnel(PetitesPuces.Models.PPClients pp)
        {
            System.Diagnostics.Debug.WriteLine("Il passe ici la vue partiel!!!");

            List<Models.Province> lstProvinces = new List<Models.Province>
            {
                new Models.Province { Abreviation = "AB", Nom = "Alberta"},
                new Models.Province { Abreviation = "BC", Nom = "Colombie-Britanique"},
                new Models.Province { Abreviation = "PE", Nom = "Île-du-Prince-Édouard"},
                new Models.Province { Abreviation = "MB", Nom = "Manitoba"},
                new Models.Province { Abreviation = "NB", Nom = "Nouveau-Brunswick"},
                new Models.Province { Abreviation = "NS", Nom = "Nouvelle-Écosse"},
                new Models.Province { Abreviation = "ON", Nom = "Ontario"},
                new Models.Province { Abreviation = "QC", Nom = "Québec"},
                new Models.Province { Abreviation = "SK", Nom = "Saskatchewan"},
                new Models.Province { Abreviation = "NL", Nom = "Terre-Neuve-et-Labrador"}
            };

            ViewBag.ListeDesProvinces = new SelectList(lstProvinces, "Abreviation", "Nom");

            return PartialView();
        }

        //Vue partiel modification du mot de passe
        [ChildActionOnly]
        public ActionResult ModificationMDP() => PartialView();
   }
}
using PetitesPuces.Models;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace PetitesPuces.Controllers
{
    public class VendeurController : Controller
    {
        Models.DataClasses1DataContext contextPP = new Models.DataClasses1DataContext();

        public ActionResult Index() => View("AccueilVendeur");

        public ActionResult AccueilVendeur() => View();

        // GET: Vendeur
        public ActionResult GestionCommande() => View();

        //GET: GestionProduit
        public ActionResult GestionProduit() => View();

        //GET: CatalogueVendeur
        public ActionResult CatalogueVendeur() => View();

        public ActionResult GestionProfilVendeur()
        {
            String strAdresseCourrielVendeur = "L.CHAPLEAU@TOTO.COM";

            var vendeur = from unVendeur in contextPP.PPVendeurs
                         where unVendeur.AdresseEmail == strAdresseCourrielVendeur
                         select unVendeur;
            //vendeur.ToList().ForEach(x => System.Diagnostics.Debug.WriteLine(x.AdresseEmail));

            List<Province> lstProvinces = new List<Province>
            {
                new Province { Abreviation = "NB", Nom = "Nouveau-Brunswick"},
                new Province { Abreviation = "ON", Nom = "Ontario"},
                new Province { Abreviation = "QC", Nom = "Québec"},
            };

            ViewBag.ListeProvinces = new SelectList(lstProvinces, "Abreviation", "Nom");

            return View(vendeur.First());
        }

        [ChildActionOnly]
        public ActionResult InformayionPersonnel()
        {
            return PartialView();
        }

        [ChildActionOnly]
        public ActionResult ModificationMDP()
        {
            return PartialView();
        }
    }
}
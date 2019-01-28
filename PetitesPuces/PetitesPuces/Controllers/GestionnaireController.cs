using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PetitesPuces.Models;

namespace PetitesPuces.Controllers
{
    
    public class GestionnaireController : Controller
    {
        DataClasses1DataContext dc = new DataClasses1DataContext();
        public ActionResult Index() => View("AccueilGestionnaire");

        public ActionResult AccueilGestionnaire() => View();

        public ActionResult GestionInactivite()
        {
            IList<SelectListItem> lstClient = new List<SelectListItem>();
            IList<SelectListItem> lstVendeur = new List<SelectListItem>();

            for (int i = 0; i < dc.GetTable<PPClients>().ToList().Count; i++)
            {
                lstClient.Add(new SelectListItem
                {
                    Value = dc.GetTable<PPClients>().ToList()[i].NoClient.ToString()
                });
            }

            for (int i = 0; i < dc.GetTable<PPVendeurs>().ToList().Count; i++)
            {
                lstVendeur.Add(new SelectListItem
                {
                    Value = dc.GetTable<PPVendeurs>().ToList()[i].NoVendeur.ToString()
                });
            }

            InactiviteViewModel iVM = new InactiviteViewModel
            {
                clients = dc.GetTable<PPClients>().ToList(),
                vendeurs = dc.GetTable<PPVendeurs>().ToList(),
                cbClients = lstClient,
                cbVendeurs = lstVendeur
            };
            return View("GestionInactivite", iVM);

        }

        [HttpPost]
        public ActionResult GestionInactivite(List<SelectListItem> items)
        {
            return View();
        }

        public ActionResult Statistiques() => View();

    }
}
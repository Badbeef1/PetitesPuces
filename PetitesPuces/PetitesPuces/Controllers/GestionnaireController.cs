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
            InactiviteViewModel iVM = new InactiviteViewModel
            {
                clients = dc.GetTable<PPClients>().ToList(),
                vendeurs = dc.GetTable<PPVendeurs>().ToList()
            };
            return View("GestionInactivite", iVM);

        }

        [HttpPost]
        public ActionResult btnDetruire(List<String> lstClients, List<String> lstVendeurs)
        {
            InactiviteViewModel iVM = new InactiviteViewModel
            {
                clients = dc.GetTable<PPClients>().ToList(),
                vendeurs = dc.GetTable<PPVendeurs>().ToList()
            };
            return View("GestionInactivite", iVM);
        }

        public ActionResult Statistiques() => View();

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
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
            List<Inactiver> lstClient = new List<Inactiver>();
            List<Inactiver> lstVendeur = new List<Inactiver>();
            int counter = 0;
            foreach (PPClients client in dc.GetTable<PPClients>().ToList())
            {
                lstClient.Add(
                    new Inactiver
                    {
                        ID =  counter,
                        IsSelected = false,
                        idClient = client.NoClient.ToString()
                    });
                counter++;
            }
            counter = 0;
            foreach (PPVendeurs vendeur in dc.GetTable<PPVendeurs>().ToList())
            {
                lstVendeur.Add(
                    new Inactiver
                    {
                        ID = counter,
                        IsSelected = false,
                        idClient = vendeur.NoVendeur.ToString()
                    });
                counter++;
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
        public ActionResult GestionInactivite(InactiviteViewModel form)
        {
            List<PPClients> lstClientInfo = new List<PPClients>();
            List<PPClients> lstClientDelete = new List<PPClients>();
            List<PPVendeurs> lstVendeurInfo = new List<PPVendeurs>();
            List<Inactiver> lstClients = new List<Inactiver>();
            List<Inactiver> lstVendeurs = new List<Inactiver>();
            PPClients clientRetirer = new PPClients();
            //Retirer client
            foreach (Inactiver client in form.cbClients)
            {
                if(client.IsSelected == true)
                {
                    clientRetirer = null;
                    if (dc.GetTable<PPVendeursClients>().Where(m => m.NoClient.ToString() == client.idClient).ToList().Count <= 0)
                    {
                        foreach(PPClients clientBDD in dc.GetTable<PPClients>().ToList())
                        {
                            if(clientBDD.NoClient.ToString() == client.idClient)
                            {
                                lstClientDelete.Add(clientBDD);
                            }
                        }

                    }
                      
                }
                else
                {
                    lstClients.Add(client);
                }
            }
            dc.Connection.Open();
            dc.GetTable<PPClients>().DeleteAllOnSubmit(lstClientDelete);
            dc.SubmitChanges();
            dc.Connection.Close();
            //Retirer vendeur
            foreach (Inactiver vendeur in form.cbVendeurs)
            {
                if (vendeur.IsSelected == false)
                {
                    foreach (PPVendeurs vendeurInfos in dc.GetTable<PPVendeurs>().ToList())
                    {
                        if (vendeurInfos.NoVendeur.ToString() == vendeur.idClient)
                        {
                            lstVendeurInfo.Add(vendeurInfos);
                        }
                    }
                    lstVendeurs.Add(vendeur);
                }
            }
            InactiviteViewModel renvoyer = new InactiviteViewModel
            {
                clients = dc.GetTable<PPClients>().ToList(),
                vendeurs = lstVendeurInfo,
                cbClients = lstClients,
                cbVendeurs = lstVendeurs
            };
            ModelState.Clear();
            return View("GestionInactivite", renvoyer);
        }

        public ActionResult Statistiques() => View();

    }
}
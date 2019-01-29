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
            List<PPArticlesEnPanier> lstPanierAVider = new List<PPArticlesEnPanier>();

            //Retirer client
            foreach (Inactiver client in form.cbClients)
            {
                if(client.IsSelected == true)
                {

                    dc.Connection.Open();
                    // Client ayant une commande, donc le plus d'endroit dans la BDD
                    if (dc.GetTable<PPCommandes>().Where(m => m.NoClient.ToString() == client.idClient).ToList().Count > 0)
                    {
                        // On vide le panier
                        foreach (PPArticlesEnPanier art in dc.GetTable<PPArticlesEnPanier>().Where(m => m.NoClient.ToString() == client.idClient).ToList())
                        {
                            lstPanierAVider.Add(art);
                        }

                        // On modifie le status pour le rendre "inactif"
                        dc.GetTable<PPClients>().Where(m => m.NoClient.ToString() == client.idClient).First().Statut = 2;

                        // On copie son historique de la table commande et détails
                        dc.ExecuteCommand("SELECT * into HistoCommandes FROM PPCommandes WHERE NoClient = " + client.idClient);
                        dc.ExecuteCommand("SELECT * into HistoDetailsCommandes FROM PPDetailsCommandes WHERE NoClient = " + client.idClient);

                        // On retire les commandes actives
                        foreach (PPCommandes comm in dc.GetTable<PPCommandes>().Where(m => m.NoClient.ToString() == client.idClient).ToList())
                        {
                            foreach (PPDetailsCommandes det in dc.GetTable<PPDetailsCommandes>().Where(m => m.NoCommande == comm.NoCommande).ToList())
                            {
                                dc.GetTable<PPDetailsCommandes>().DeleteOnSubmit(det);
                            }
                            dc.GetTable<PPCommandes>().DeleteOnSubmit(comm);

                        }

                        // On retire les visites de la BDD
                        foreach (PPVendeursClients vencli in dc.GetTable<PPVendeursClients>().Where(m => m.NoClient.ToString() == client.idClient).ToList())
                        {
                            dc.GetTable<PPVendeursClients>().DeleteOnSubmit(vencli);
                        }

                        if (lstPanierAVider.Count > 0)
                        {
                            dc.GetTable<PPArticlesEnPanier>().DeleteAllOnSubmit(lstPanierAVider);
                        }

                        // Stats? 

                    }

                    // Client présent nul part dans la BDD, car il doit avoir visité un vendeur pour avoir été ajouté ailleur
                    else if (dc.GetTable<PPVendeursClients>().Where(m => m.NoClient.ToString() == client.idClient).ToList().Count <= 0)
                    {
                        foreach(PPClients clientBDD in dc.GetTable<PPClients>().ToList())
                        {
                            if(clientBDD.NoClient.ToString() == client.idClient)
                            {
                                dc.GetTable<PPClients>().DeleteOnSubmit(clientBDD);
                            }
                        }
                        dc.GetTable<PPClients>().DeleteOnSubmit(dc.GetTable<PPClients>().Where(m => m.NoClient.ToString() == client.idClient).First());

                    }
                    // Client ayant visité un vendeur, mais n'ayant pas de commande
                    else
                    {
                        // On liste les items dans le panier
                        foreach (PPArticlesEnPanier art in dc.GetTable<PPArticlesEnPanier>().Where(m => m.NoClient.ToString() == client.idClient).ToList())
                        {
                            dc.GetTable<PPArticlesEnPanier>().DeleteOnSubmit(art);
                        }
                        // On retire les visites de la BDD
                        foreach (PPVendeursClients vencli in dc.GetTable<PPVendeursClients>().Where(m => m.NoClient.ToString() == client.idClient).ToList())
                        {
                            dc.GetTable<PPVendeursClients>().DeleteOnSubmit(vencli);
                        }
                        if (lstPanierAVider.Count > 0)
                        {
                            dc.GetTable<PPArticlesEnPanier>().DeleteAllOnSubmit(lstPanierAVider);
                        }
                        dc.GetTable<PPClients>().DeleteOnSubmit(dc.GetTable<PPClients>().Where(m => m.NoClient.ToString() == client.idClient).First());
                    }
                    dc.SubmitChanges();
                    dc.Connection.Close();
                }
                else
                {
                    lstClients.Add(client);
                }
            }


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
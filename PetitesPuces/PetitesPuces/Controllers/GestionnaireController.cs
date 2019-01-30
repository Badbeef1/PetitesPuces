using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
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
            foreach (PPClients client in dc.GetTable<PPClients>().Where(m => m.Statut == 1).ToList())
            {
                lstClient.Add(
                    new Inactiver
                    {
                        ID = counter,
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
        // Bouton confirmer
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
                if (client.IsSelected == true)
                {


                    try
                    {
                        using (var trans = new TransactionScope())
                        {
                            dc.Connection.Open();

                            // On vide le panier
                            foreach (PPArticlesEnPanier art in dc.GetTable<PPArticlesEnPanier>().Where(m => m.NoClient.ToString() == client.idClient).ToList())
                            {
                                lstPanierAVider.Add(art);
                            }
                            if (lstPanierAVider.Count > 0)
                            {
                                dc.GetTable<PPArticlesEnPanier>().DeleteAllOnSubmit(lstPanierAVider);
                            }

                            // On retire les visites
                            foreach (PPVendeursClients vencli in dc.GetTable<PPVendeursClients>().Where(m => m.NoClient.ToString() == client.idClient).ToList())
                            {
                                dc.GetTable<PPVendeursClients>().DeleteOnSubmit(vencli);
                            }

                            // Client ayant une commande, donc le plus d'endroit dans la BDD
                            if (dc.GetTable<PPCommandes>().Where(m => m.NoClient.ToString() == client.idClient).ToList().Count > 0)
                            {
                                // Si la table HistoCommandes n'existe pas on la crée et y met les informations qu'on retire
                                try
                                {
                                    dc.ExecuteCommand("SELECT * INTO HistoCommandes FROM PPCommandes WHERE NoClient = '" + client.idClient + "'");
                                }
                                // Sinon on insère simplement les données
                                catch (Exception e)
                                {
                                    dc.ExecuteCommand("INSERT INTO HistoCommandes SELECT * FROM PPCommandes WHERE NoClient = '" + client.idClient + "'");
                                }

                                // On retire les commandes actives
                                foreach (PPCommandes comm in dc.GetTable<PPCommandes>().Where(m => m.NoClient.ToString() == client.idClient).ToList())
                                {
                                    //Même chose, mais pour HistoDetailsCommandes
                                    try
                                    {
                                        dc.ExecuteCommand("SELECT * INTO HistoDetailsCommandes FROM PPDetailsCommandes WHERE NoCommande = '" + comm.NoCommande + "'");
                                    }
                                    catch (Exception e)
                                    {
                                        dc.ExecuteCommand("INSERT INTO HistoDetailsCommandes SELECT * FROM PPDetailsCOmmandes WHERE NoCommande = '" + comm.NoCommande + "'");
                                    }
                                    // Puis on supprime
                                    foreach (PPDetailsCommandes det in dc.GetTable<PPDetailsCommandes>().Where(m => m.NoCommande == comm.NoCommande).ToList())
                                    {
                                        dc.GetTable<PPDetailsCommandes>().DeleteOnSubmit(det);
                                    }
                                    dc.GetTable<PPCommandes>().DeleteOnSubmit(comm);

                                }

                                // On met le statut à 2 (Intégrité)
                                dc.GetTable<PPClients>().Where(m => m.NoClient.ToString() == client.idClient).First().Statut = 2;

                                // Stats? 

                            }
                            // Client n'ayant pas de commande (Avec ou sans panier)
                            else
                            {
                                // On delete (Déjà retiré des autres tables)
                                dc.GetTable<PPClients>().DeleteOnSubmit(dc.GetTable<PPClients>().Where(m => m.NoClient.ToString() == client.idClient).First());
                            }
                            dc.SubmitChanges();
                            dc.Connection.Close();
                            trans.Complete();


                        }
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine("Rollback");
                    }
                        
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
            ModelState.Clear();
            dc = new DataClasses1DataContext();
            InactiviteViewModel renvoyer = new InactiviteViewModel
            {
                clients = dc.GetTable<PPClients>().Where(m => m.Statut == 1).ToList(),
                vendeurs = lstVendeurInfo,
                cbClients = lstClients,
                cbVendeurs = lstVendeurs
            };
            return View("GestionInactivite", renvoyer);
        }

        public ActionResult Statistiques() => View();

    }
}
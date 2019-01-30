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
            List<Inactiver> lstClient = creeClient();
            List<Inactiver> lstVendeur = creeVendeur();
            InactiviteViewModel iVM = new InactiviteViewModel
            {
                cbClients = lstClient,
                cbVendeurs = lstVendeur
            };
            return View("GestionInactivite", iVM);

        }

        [HttpPost]
        // Bouton confirmer
        public ActionResult GestionInactivite(InactiviteViewModel form)
        {
            List<Inactiver> lstClients = new List<Inactiver>();
            List<Inactiver> lstVendeurs = new List<Inactiver>();
            List<PPArticlesEnPanier> lstPanierAVider = new List<PPArticlesEnPanier>();
            List<PPProduits> lstProduitNonCommander = new List<PPProduits>();

            //Retirer client
            foreach (Inactiver client in form.cbClients)
            {
                if (client.IsSelected == true)
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
                                dc.ExecuteCommand("INSERT INTO HistoDetailsCommandes SELECT * FROM PPDetailsCommandes WHERE NoCommande = '" + comm.NoCommande + "'");

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
                } else
                {
                    lstClients.Add(client);
                }
            }

            //Retirer vendeur
            foreach (Inactiver vendeur in form.cbVendeurs)
            {
                if (vendeur.IsSelected == true)
                {
                    dc.Connection.Open();
                    foreach (PPProduits produitNonCommande in dc.GetTable<PPProduits>().Where(m => m.NoVendeur.ToString() == vendeur.idClient).ToList())
                    {
                        if(produitNonCommande.DateVente == null)
                        {
                            lstProduitNonCommander.Add(produitNonCommande);
                        }
                        else
                        {
                            produitNonCommande.Disponibilité = false;
                        }
                    }
                    dc.GetTable<PPProduits>().DeleteAllOnSubmit(lstProduitNonCommander);
                    dc.GetTable<PPVendeurs>().Where(m => m.NoVendeur.ToString() == vendeur.idClient).First().Statut = 2;
                    dc.SubmitChanges();
                    dc.Connection.Close();
                }
                else
                {
                    lstVendeurs.Add(vendeur);
                }
            }
            ModelState.Clear();
            dc = new DataClasses1DataContext();
            InactiviteViewModel renvoyer = new InactiviteViewModel
            {
                cbClients = lstClients,
                cbVendeurs = lstVendeurs
            };
            return View("GestionInactivite", renvoyer);
        }

        public ActionResult Statistiques() => View();
        public ActionResult ddlChanger(string id)
        {
            List<Inactiver> cbClients = creeClient();
            List<Inactiver> cbVendeur = creeVendeur();
            
            switch (id.Split(';')[0])
            {
                case "1":
                    cbClients = cbClients.Where(m => m.dernierPresence < DateTime.Today.AddMonths(-1)).ToList();
                    break;
                case "2":
                    cbClients = cbClients.Where(m => m.dernierPresence < DateTime.Today.AddMonths(-3)).ToList();
                    break;
                case "3":
                    cbClients = cbClients.Where(m => m.dernierPresence < DateTime.Today.AddMonths(-6)).ToList();
                    break;
                case "4":
                    cbClients = cbClients.Where(m => m.dernierPresence < DateTime.Today.AddYears(-1)).ToList();
                    break;
                case "5":
                    cbClients = cbClients.Where(m => m.dernierPresence < DateTime.Today.AddYears(-2)).ToList();
                    break;
                case "6":
                    cbClients = cbClients.Where(m => m.dernierPresence < DateTime.Today.AddYears(-3)).ToList();
                    break;
                case "7":
                    cbClients = cbClients.Where(m => m.dernierPresence == DateTime.MinValue).ToList();
                    break;
                default:
                    break;
            }
            switch (id.Split(';')[1])
            {
                case "1":
                    cbVendeur = cbVendeur.Where(m => m.dernierPresence < DateTime.Today.AddYears(-1)).ToList();
                    break;
                case "2":
                    cbVendeur = cbVendeur.Where(m => m.dernierPresence < DateTime.Today.AddYears(-2)).ToList();
                    break;
                case "3":
                    cbVendeur = cbVendeur.Where(m => m.dernierPresence < DateTime.Today.AddYears(-3)).ToList();
                    break;
                case "4":
                    cbVendeur = cbVendeur.Where(m => m.dernierPresence == DateTime.MinValue).ToList();
                    break;
                default:
                    break;
            }
            ModelState.Clear();
            InactiviteViewModel renvoyer = new InactiviteViewModel
            {
                cbClients = cbClients,
                cbVendeurs = cbVendeur
            };
            return View("GestionInactivite", renvoyer);

        }

        public List<Inactiver> creeClient()
        {
            int counter = 0;
            List<Inactiver> clients = new List<Inactiver>();
            foreach (PPClients client in dc.GetTable<PPClients>().Where(m => m.Statut == 1).ToList())
            {
                DateTime dateDernierePresence = DateTime.MinValue;
                PPCommandes derniereCommande = dc.GetTable<PPCommandes>().Where(m => m.NoClient.ToString() == client.NoClient.ToString()).OrderByDescending(m => m.DateCommande).FirstOrDefault();
                PPArticlesEnPanier dernierPanier = dc.GetTable<PPArticlesEnPanier>().Where(m => m.NoClient.ToString() == client.NoClient.ToString()).OrderByDescending(m => m.DateCreation).FirstOrDefault();
                if (derniereCommande != null && dernierPanier != null)
                {
                    if ((DateTime)derniereCommande.DateCommande > (DateTime)dernierPanier.DateCreation)
                    {
                        dateDernierePresence = (DateTime)derniereCommande.DateCommande;
                    }
                    else
                    {
                        dateDernierePresence = (DateTime)dernierPanier.DateCreation;
                    }
                }
                else
                {
                    if (derniereCommande != null)
                    {
                        dateDernierePresence = (DateTime)derniereCommande.DateCommande;
                    }
                    else if (dernierPanier != null)
                    {
                        dateDernierePresence = (DateTime)dernierPanier.DateCreation;
                    }

                }
                clients.Add(
                    new Inactiver
                    {
                        ID = counter,
                        IsSelected = false,
                        idClient = client.NoClient.ToString(),
                        Nom = client.Nom,
                        Prenom = client.Prenom,
                        dernierPresence = dateDernierePresence
                    });
                counter++;
            }
            return clients.OrderByDescending(m => m.dernierPresence).ToList();
        }

        public List<Inactiver> creeVendeur()
        {
            List<Inactiver> vendeurs = new List<Inactiver>();
            int counter = 0;

            foreach(PPVendeurs vendeur in dc.GetTable<PPVendeurs>().Where(m => m.Statut == 1).ToList())
            {
                DateTime dateDernierePresence = DateTime.MinValue;
                PPCommandes derniereCommande = dc.GetTable<PPCommandes>().Where(m => m.NoVendeur.ToString() == vendeur.NoVendeur.ToString()).OrderByDescending(m => m.DateCommande).FirstOrDefault();
                PPProduits dernierProduit = dc.GetTable<PPProduits>().Where(m => m.NoVendeur.ToString() == vendeur.NoVendeur.ToString()).OrderByDescending(m => m.DateCreation).FirstOrDefault();
                if (derniereCommande != null && dernierProduit != null)
                {
                    if ((DateTime)derniereCommande.DateCommande > (DateTime)dernierProduit.DateCreation)
                    {
                        dateDernierePresence = (DateTime)derniereCommande.DateCommande;
                    }
                    else
                    {
                        dateDernierePresence = (DateTime)dernierProduit.DateCreation;
                    }
                }
                else
                {
                    if (derniereCommande != null)
                    {
                        dateDernierePresence = (DateTime)derniereCommande.DateCommande;
                    }
                    else if (dernierProduit != null)
                    {
                        dateDernierePresence = (DateTime)dernierProduit.DateCreation;
                    }

                }
                vendeurs.Add(
                    new Inactiver
                    {
                        ID = counter,
                        IsSelected = false,
                        idClient = vendeur.NoVendeur.ToString(),
                        Nom = vendeur.Nom,
                        Prenom = vendeur.Prenom,
                        dernierPresence = dateDernierePresence
                    });
                counter++;
            }
            return vendeurs.OrderByDescending(m => m.dernierPresence).ToList();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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

      public ActionResult AccueilGestionnaire()
      {
         Dictionary<PPCategories, bool> dicCategories = new Dictionary<PPCategories, bool>();
         Models.DataClasses1DataContext db = new Models.DataClasses1DataContext();
         db.Connection.Open();
         //Aller chercher toutes les demandes de vendeurs
         var vendeurs = (from vendeur in db.GetTable<PPVendeurs>()
                         where vendeur.Statut.Equals(0)
                         select vendeur
                         ).ToList();

         Dictionary<PPVendeurs, bool> dicVendeurs = new Dictionary<PPVendeurs, bool>();
         //Aller chercher la liste des vendeurs admis dans la BD
         var vendeursAdmis = (from vendeur in db.GetTable<PPVendeurs>()
                              where vendeur.Statut.Equals(1)
                              select vendeur
                              ).ToList();
         foreach(var vendeurAdmis in vendeursAdmis)
         {
            var nbCommandes = (from commande in db.GetTable<PPCommandes>()
                               where commande.NoVendeur.Equals(vendeurAdmis.NoVendeur)
                               select commande
                               ).ToList();
            
            //Vérifier si le vendeur a déjà effectué des commandes.
            if(nbCommandes.Count() > 0)
            {
               dicVendeurs.Add(vendeurAdmis, true);
            }
            else
            {
               dicVendeurs.Add(vendeurAdmis, false);
            }
         }


         var categories = (from categorie in db.GetTable<PPCategories>()
                           select categorie
                           ).ToList();

         foreach (PPCategories cat in categories)
         {
            var query = (from produit in db.GetTable<PPProduits>()
                         where produit.NoCategorie.Equals(cat.NoCategorie)
                         select produit
                         ).ToList();
            if (query.Count() > 0)
            {
               dicCategories.Add(cat, false);
            }
            else
            {
               dicCategories.Add(cat, true);
            }
         }
         PPCategories c = new PPCategories();
         AccueilGestionnaireViewModel accueilGestionnaireViewModel = new AccueilGestionnaireViewModel(vendeurs, dicCategories,c);
         accueilGestionnaireViewModel.lstVendeurs = dicVendeurs;
         db.Connection.Close();
         return View(accueilGestionnaireViewModel);
      }

      [HttpPost]
      [ValidateAntiForgeryToken]
      public ActionResult AccueilGestionnaire(AccueilGestionnaireViewModel viewModel)
      {
         Models.DataClasses1DataContext db = new Models.DataClasses1DataContext();
         db.Connection.Open();

         if (ModelState.IsValid)
         {
            var query = (from nbCat in db.GetTable<PPCategories>()
                         select nbCat
                      ).ToList();

            viewModel.categorie.NoCategorie = (query.Count() + 1) * 10;

            db.PPCategories.InsertOnSubmit(viewModel.categorie);

            // Submit the changes to the database.
            try
            {
               db.SubmitChanges();
            }
            catch (Exception e)
            {
               Console.WriteLine(e);
            }
         }

         Dictionary<PPVendeurs, bool> dicVendeurs = new Dictionary<PPVendeurs, bool>();
         //Aller chercher la liste des vendeurs admis dans la BD
         var vendeursAdmis = (from vendeur in db.GetTable<PPVendeurs>()
                              where vendeur.Statut.Equals(1)
                              select vendeur
                              ).ToList();
         foreach (var vendeurAdmis in vendeursAdmis)
         {
            var nbCommandes = (from commande in db.GetTable<PPCommandes>()
                               where commande.NoVendeur.Equals(vendeurAdmis.NoVendeur)
                               select commande
                               ).ToList();

            //Vérifier si le vendeur a déjà effectué des commandes.
            if (nbCommandes.Count() > 0)
            {
               dicVendeurs.Add(vendeurAdmis, true);
            }
            else
            {
               dicVendeurs.Add(vendeurAdmis, false);
            }
         }


         //Aller chercher toutes les demandes de vendeurs
         var vendeurs = (from vendeur in db.GetTable<PPVendeurs>()
                         where vendeur.Statut.Equals(0)
                         select vendeur
                         ).ToList();
         Dictionary<PPCategories, bool> dicCategories = new Dictionary<PPCategories, bool>();
         var categories = (from categorie in db.GetTable<PPCategories>()
                           select categorie
                           ).ToList();

         foreach (PPCategories cate in categories)
         {
            var query2 = (from produit in db.GetTable<PPProduits>()
                          where produit.NoCategorie.Equals(cate.NoCategorie)
                          select produit
                         ).ToList();
            if (query2.Count() > 0)
            {
               dicCategories.Add(cate, false);
            }
            else
            {
               dicCategories.Add(cate, true);
            }
         }
         PPCategories c = new PPCategories();
         AccueilGestionnaireViewModel accueilGestionnaireViewModel = new AccueilGestionnaireViewModel(vendeurs, dicCategories, c);
         accueilGestionnaireViewModel.lstVendeurs = dicVendeurs;
         db.Connection.Close();
         return View("AccueilGestionnaire", accueilGestionnaireViewModel);
      }

      public ActionResult AccepterVendeur(int id, decimal redevance)
      {
         Models.DataClasses1DataContext db = new Models.DataClasses1DataContext();
         db.Connection.Open();
         //Aller changer le status du vendeur
         var vendeurAccepter = (from vendeur in db.GetTable<PPVendeurs>()
                                where vendeur.NoVendeur.Equals(id)
                                select vendeur
                                ).ToList();

         //changer le status du vendeur.
         vendeurAccepter.First().Statut = 1;
         vendeurAccepter.First().Pourcentage = redevance;
         // Submit the changes to the database.
         try
         {
            db.SubmitChanges();
         }
         catch (Exception e)
         {
            Console.WriteLine(e);
         }
         //Aller chercher toutes les demandes de vendeurs
         var vendeurs = (from vendeur in db.GetTable<PPVendeurs>()
                         where vendeur.Statut.Equals(0)
                         select vendeur
                         ).ToList();

         Dictionary<PPVendeurs, bool> dicVendeurs = new Dictionary<PPVendeurs, bool>();
         //Aller chercher la liste des vendeurs admis dans la BD
         var vendeursAdmis = (from vendeur in db.GetTable<PPVendeurs>()
                              where vendeur.Statut.Equals(1)
                              select vendeur
                              ).ToList();
         foreach (var vendeurAdmis in vendeursAdmis)
         {
            var nbCommandes = (from commande in db.GetTable<PPCommandes>()
                               where commande.NoVendeur.Equals(vendeurAdmis.NoVendeur)
                               select commande
                               ).ToList();

            //Vérifier si le vendeur a déjà effectué des commandes.
            if (nbCommandes.Count() > 0)
            {
               dicVendeurs.Add(vendeurAdmis, true);
            }
            else
            {
               dicVendeurs.Add(vendeurAdmis, false);
            }
         }

         Dictionary<PPCategories, bool> dicCategories = new Dictionary<PPCategories, bool>();
         var categories = (from categorie in db.GetTable<PPCategories>()
                           select categorie
                           ).ToList();

         foreach (PPCategories cat in categories)
         {
            var query = (from produit in db.GetTable<PPProduits>()
                         where produit.NoCategorie.Equals(cat.NoCategorie)
                         select produit
                         ).ToList();
            if (query.Count() > 0)
            {
               dicCategories.Add(cat, false);
            }
            else
            {
               dicCategories.Add(cat, true);
            }
         }

         PPCategories c = new PPCategories();
         AccueilGestionnaireViewModel accueilGestionnaireViewModel = new AccueilGestionnaireViewModel(vendeurs, dicCategories, c);
         accueilGestionnaireViewModel.lstVendeurs = dicVendeurs;
         db.Connection.Close();
         return View("AccueilGestionnaire", accueilGestionnaireViewModel);
      }

      public ActionResult CancellerAjout(int id)
      {
         Models.DataClasses1DataContext db = new Models.DataClasses1DataContext();
         db.Connection.Open();
         //Aller changer le status du vendeur
         var vendeurAccepter = (from vendeur in db.GetTable<PPVendeurs>()
                                where vendeur.NoVendeur.Equals(id)
                                select vendeur
                                ).ToList();

         //changer le status du vendeur.
         vendeurAccepter.First().Statut = 0;
         vendeurAccepter.First().Pourcentage = (decimal)0.00;
         // Submit the changes to the database.
         try
         {
            db.SubmitChanges();
         }
         catch (Exception e)
         {
            Console.WriteLine(e);
         }
         //Aller chercher toutes les demandes de vendeurs
         var vendeurs = (from vendeur in db.GetTable<PPVendeurs>()
                         where vendeur.Statut.Equals(0)
                         select vendeur
                         ).ToList();

         Dictionary<PPVendeurs, bool> dicVendeurs = new Dictionary<PPVendeurs, bool>();
         //Aller chercher la liste des vendeurs admis dans la BD
         var vendeursAdmis = (from vendeur in db.GetTable<PPVendeurs>()
                              where vendeur.Statut.Equals(1)
                              select vendeur
                              ).ToList();
         foreach (var vendeurAdmis in vendeursAdmis)
         {
            var nbCommandes = (from commande in db.GetTable<PPCommandes>()
                               where commande.NoVendeur.Equals(vendeurAdmis.NoVendeur)
                               select commande
                               ).ToList();

            //Vérifier si le vendeur a déjà effectué des commandes.
            if (nbCommandes.Count() > 0)
            {
               dicVendeurs.Add(vendeurAdmis, true);
            }
            else
            {
               dicVendeurs.Add(vendeurAdmis, false);
            }
         }

         Dictionary<PPCategories, bool> dicCategories = new Dictionary<PPCategories, bool>();
         var categories = (from categorie in db.GetTable<PPCategories>()
                           select categorie
                           ).ToList();

         foreach (PPCategories cat in categories)
         {
            var query = (from produit in db.GetTable<PPProduits>()
                         where produit.NoCategorie.Equals(cat.NoCategorie)
                         select produit
                         ).ToList();
            if (query.Count() > 0)
            {
               dicCategories.Add(cat, false);
            }
            else
            {
               dicCategories.Add(cat, true);
            }
         }

         PPCategories c = new PPCategories();
         AccueilGestionnaireViewModel accueilGestionnaireViewModel = new AccueilGestionnaireViewModel(vendeurs, dicCategories, c);
         accueilGestionnaireViewModel.lstVendeurs = dicVendeurs;
         db.Connection.Close();
         return View("AccueilGestionnaire", accueilGestionnaireViewModel);
      }

      public ActionResult RefuserVendeur(int id, decimal redevance)
      {
         Models.DataClasses1DataContext db = new Models.DataClasses1DataContext();
         db.Connection.Open();
         //Aller changer le status du vendeur
         var vendeurAccepter = (from vendeur in db.GetTable<PPVendeurs>()
                                where vendeur.NoVendeur.Equals(id)
                                select vendeur
                                ).ToList();

         //changer le status du vendeur.
         vendeurAccepter.First().Statut = 2;
         vendeurAccepter.First().Pourcentage = redevance;
         // Submit the changes to the database.
         try
         {
            db.SubmitChanges();
         }
         catch (Exception e)
         {
            Console.WriteLine(e);
         }
         //Aller chercher toutes les demandes de vendeurs
         var vendeurs = (from vendeur in db.GetTable<PPVendeurs>()
                         where vendeur.Statut.Equals(0)
                         select vendeur
                         ).ToList();
         Dictionary<PPVendeurs, bool> dicVendeurs = new Dictionary<PPVendeurs, bool>();
         //Aller chercher la liste des vendeurs admis dans la BD
         var vendeursAdmis = (from vendeur in db.GetTable<PPVendeurs>()
                              where vendeur.Statut.Equals(1)
                              select vendeur
                              ).ToList();
         foreach (var vendeurAdmis in vendeursAdmis)
         {
            var nbCommandes = (from commande in db.GetTable<PPCommandes>()
                               where commande.NoVendeur.Equals(vendeurAdmis.NoVendeur)
                               select commande
                               ).ToList();

            //Vérifier si le vendeur a déjà effectué des commandes.
            if (nbCommandes.Count() > 0)
            {
               dicVendeurs.Add(vendeurAdmis, true);
            }
            else
            {
               dicVendeurs.Add(vendeurAdmis, false);
            }
         }

         Dictionary<PPCategories, bool> dicCategories = new Dictionary<PPCategories, bool>();
         var categories = (from categorie in db.GetTable<PPCategories>()
                           select categorie
                           ).ToList();

         foreach (PPCategories cat in categories)
         {
            var query = (from produit in db.GetTable<PPProduits>()
                         where produit.NoCategorie.Equals(cat.NoCategorie)
                         select produit
                         ).ToList();
            if (query.Count() > 0)
            {
               dicCategories.Add(cat, false);
            }
            else
            {
               dicCategories.Add(cat, true);
            }
         }

         PPCategories c = new PPCategories();
         AccueilGestionnaireViewModel accueilGestionnaireViewModel = new AccueilGestionnaireViewModel(vendeurs, dicCategories, c);
         db.Connection.Close();
         return View("AccueilGestionnaire", accueilGestionnaireViewModel);
      }

      public ActionResult SupprimerCategorie(int id)
      {
         Models.DataClasses1DataContext db = new Models.DataClasses1DataContext();
         db.Connection.Open();
         //Aller changer le status du vendeur
         var categorie = (from cat in db.GetTable<PPCategories>()
                          where cat.NoCategorie.Equals(id)
                          select cat
                           ).ToList();

         db.PPCategories.DeleteOnSubmit(categorie.First());

         //Faire les modifications dans la base de données
         try
         {
            db.SubmitChanges();
         }
         catch (Exception e)
         {
            Console.WriteLine(e);
         }

         //Aller chercher toutes les demandes de vendeurs
         var vendeurs = (from vendeur in db.GetTable<PPVendeurs>()
                         where vendeur.Statut.Equals(0)
                         select vendeur
                         ).ToList();
         Dictionary<PPVendeurs, bool> dicVendeurs = new Dictionary<PPVendeurs, bool>();
         //Aller chercher la liste des vendeurs admis dans la BD
         var vendeursAdmis = (from vendeur in db.GetTable<PPVendeurs>()
                              where vendeur.Statut.Equals(1)
                              select vendeur
                              ).ToList();
         foreach (var vendeurAdmis in vendeursAdmis)
         {
            var nbCommandes = (from commande in db.GetTable<PPCommandes>()
                               where commande.NoVendeur.Equals(vendeurAdmis.NoVendeur)
                               select commande
                               ).ToList();

            //Vérifier si le vendeur a déjà effectué des commandes.
            if (nbCommandes.Count() > 0)
            {
               dicVendeurs.Add(vendeurAdmis, true);
            }
            else
            {
               dicVendeurs.Add(vendeurAdmis, false);
            }
         }

         Dictionary<PPCategories, bool> dicCategories = new Dictionary<PPCategories, bool>();
         var categories = (from cat in db.GetTable<PPCategories>()
                           select cat
                           ).ToList();

         foreach (PPCategories cat in categories)
         {
            var query = (from produit in db.GetTable<PPProduits>()
                         where produit.NoCategorie.Equals(cat.NoCategorie)
                         select produit
                         ).ToList();
            if (query.Count() > 0)
            {
               dicCategories.Add(cat, false);
            }
            else
            {
               dicCategories.Add(cat, true);
            }
         }

         PPCategories c = new PPCategories();
         AccueilGestionnaireViewModel accueilGestionnaireViewModel = new AccueilGestionnaireViewModel(vendeurs, dicCategories, c);
         accueilGestionnaireViewModel.lstVendeurs = dicVendeurs;
         db.Connection.Close();
         return View("AccueilGestionnaire", accueilGestionnaireViewModel);
      }

      public ActionResult ModifierRedevance(int id, decimal redevance)
      {
         Models.DataClasses1DataContext db = new Models.DataClasses1DataContext();
         db.Connection.Open();

         var vendeurAModifier = (from vendeur in db.GetTable<PPVendeurs>()
                                 where vendeur.NoVendeur.Equals(id)
                                 select vendeur
                                 ).ToList();

         vendeurAModifier.First().Pourcentage = redevance;
         //Faire les modifications dans la base de données
         try
         {
            db.SubmitChanges();
         }
         catch (Exception e)
         {
            Console.WriteLine(e);
         }

         //Aller chercher toutes les demandes de vendeurs
         var vendeurs = (from vendeur in db.GetTable<PPVendeurs>()
                         where vendeur.Statut.Equals(0)
                         select vendeur
                         ).ToList();
         Dictionary<PPVendeurs, bool> dicVendeurs = new Dictionary<PPVendeurs, bool>();
         //Aller chercher la liste des vendeurs admis dans la BD
         var vendeursAdmis = (from vendeur in db.GetTable<PPVendeurs>()
                              where vendeur.Statut.Equals(1)
                              select vendeur
                              ).ToList();
         foreach (var vendeurAdmis in vendeursAdmis)
         {
            var nbCommandes = (from commande in db.GetTable<PPCommandes>()
                               where commande.NoVendeur.Equals(vendeurAdmis.NoVendeur)
                               select commande
                               ).ToList();

            //Vérifier si le vendeur a déjà effectué des commandes.
            if (nbCommandes.Count() > 0)
            {
               dicVendeurs.Add(vendeurAdmis, true);
            }
            else
            {
               dicVendeurs.Add(vendeurAdmis, false);
            }
         }

         Dictionary<PPCategories, bool> dicCategories = new Dictionary<PPCategories, bool>();
         var categories = (from cat in db.GetTable<PPCategories>()
                           select cat
                           ).ToList();

         foreach (PPCategories cat in categories)
         {
            var query = (from produit in db.GetTable<PPProduits>()
                         where produit.NoCategorie.Equals(cat.NoCategorie)
                         select produit
                         ).ToList();
            if (query.Count() > 0)
            {
               dicCategories.Add(cat, false);
            }
            else
            {
               dicCategories.Add(cat, true);
            }
         }

         PPCategories c = new PPCategories();
         AccueilGestionnaireViewModel accueilGestionnaireViewModel = new AccueilGestionnaireViewModel(vendeurs, dicCategories, c);
         accueilGestionnaireViewModel.lstVendeurs = dicVendeurs;
         db.Connection.Close();
         return View("AccueilGestionnaire", accueilGestionnaireViewModel);
      }

      public ActionResult EnvoyerMessageDemandeVendeur(int noDestinataire, int noExpediteur, string message)
      {
         Models.DataClasses1DataContext db = new Models.DataClasses1DataContext();
         db.Connection.Open();
         //Aller chercher le dernier noMessage
         var messages = (from msg in db.GetTable<PPMessages>()
                         select msg
                         ).ToList();
         int noMessages = messages.Count() + 1;

         //Ajouter un message
         PPMessages ppMessage = new PPMessages
         {
            NoMsg = noMessages,
            NoExpediteur = noExpediteur,
            DescMsg = message,
            FichierJoint = null,
            Lieu = 2,
            dateEnvoi = DateTime.Now,
            objet = "Demande de vendeur"
         };
         //Ajouter un Destinataire
         PPDestinataires ppDestinataires = new PPDestinataires
         {
            NoMsg = noMessages,
            NoDestinataire = noDestinataire,
            EtatLu = 0,
            Lieu = 1
         };

         //Ajouter les nouveaux objets à leur collection
         db.PPMessages.InsertOnSubmit(ppMessage);
         db.PPDestinataires.InsertOnSubmit(ppDestinataires);

         // Submit the change to the database.
         try
         {
            db.SubmitChanges();
         }
         catch (Exception e)
         {
            Console.WriteLine(e);
         }

         //Aller chercher toutes les demandes de vendeurs
         var vendeurs = (from vendeur in db.GetTable<PPVendeurs>()
                         where vendeur.Statut.Equals(0)
                         select vendeur
                         ).ToList();

         Dictionary<PPVendeurs, bool> dicVendeurs = new Dictionary<PPVendeurs, bool>();
         //Aller chercher la liste des vendeurs admis dans la BD
         var vendeursAdmis = (from vendeur in db.GetTable<PPVendeurs>()
                              where vendeur.Statut.Equals(1)
                              select vendeur
                              ).ToList();
         foreach (var vendeurAdmis in vendeursAdmis)
         {
            var nbCommandes = (from commande in db.GetTable<PPCommandes>()
                               where commande.NoVendeur.Equals(vendeurAdmis.NoVendeur)
                               select commande
                               ).ToList();

            //Vérifier si le vendeur a déjà effectué des commandes.
            if (nbCommandes.Count() > 0)
            {
               dicVendeurs.Add(vendeurAdmis, true);
            }
            else
            {
               dicVendeurs.Add(vendeurAdmis, false);
            }
         }

         Dictionary<PPCategories, bool> dicCategories = new Dictionary<PPCategories, bool>();
         var categories = (from cat in db.GetTable<PPCategories>()
                           select cat
                           ).ToList();

         foreach (PPCategories cat in categories)
         {
            var query = (from produit in db.GetTable<PPProduits>()
                         where produit.NoCategorie.Equals(cat.NoCategorie)
                         select produit
                         ).ToList();
            if (query.Count() > 0)
            {
               dicCategories.Add(cat, false);
            }
            else
            {
               dicCategories.Add(cat, true);
            }
         }
         PPCategories c = new PPCategories();
         AccueilGestionnaireViewModel accueilGestionnaireViewModel = new AccueilGestionnaireViewModel(vendeurs, dicCategories, c);
         accueilGestionnaireViewModel.lstVendeurs = dicVendeurs;
         db.Connection.Close();
         return View("AccueilGestionnaire", accueilGestionnaireViewModel);
      }

      public ActionResult DetailVendeur(int id)
      {
         Models.DataClasses1DataContext db = new Models.DataClasses1DataContext();
         db.Connection.Open();

         //Aller chercher le vendeur pour le NoVendeur passé en paramètre
         var vendeur = (from ven in db.GetTable<PPVendeurs>()
                        where ven.NoVendeur.Equals(id)
                        select ven
                        ).ToList();

         db.Connection.Close();

         return View(vendeur.First());
      }

      public ActionResult GestionInactivite()
      {
         List<Inactiver> lstClient = creeClient();
         List<Inactiver> lstVendeur = creeVendeur();
         InactiviteViewModel iVM = new InactiviteViewModel
         {
            cbClients = lstClient,
            cbVendeurs = lstVendeur,
            blnOpenPDF = false
         };
         return View("GestionInactivite", iVM);

      }

      [HttpPost]
      // Bouton confirmer
      public ActionResult GestionInactivite(InactiviteViewModel form)
      {
         List<Inactiver> lstClients = new List<Inactiver>();
         List<Inactiver> lstVendeurs = new List<Inactiver>();
         List<Inactiver> lstClientsDeleter = new List<Inactiver>();
         List<Inactiver> lstVendeursDeleter = new List<Inactiver>();
         List<PPArticlesEnPanier> lstPanierAVider = new List<PPArticlesEnPanier>();
         List<PPProduits> lstProduitNonCommander = new List<PPProduits>();

         List<PPClients> lstClientsPDF = new List<PPClients>();
         Dictionary<string, List<PPCommandes>> lstClientsCommandesPDF = new Dictionary<string, List<PPCommandes>>();
         Dictionary<string, int> lstClientsVisitesPDF = new Dictionary<string, int>();
         Dictionary<PPCommandes, List<PPDetailsCommandes>> lstCommandesDtail = new Dictionary<PPCommandes, List<PPDetailsCommandes>>();

         List<PPCommandes> lstCommDynamique = new List<PPCommandes>();
         List<PPDetailsCommandes> lstDetCommDynamique = new List<PPDetailsCommandes>();

         String path = "";
         String date = "";


         //Retirer client
         foreach (Inactiver client in form.cbClients)
         {
            if (client.IsSelected == true)
            {
               lstClientsPDF.Add(dc.GetTable<PPClients>().Where(m => m.NoClient.ToString() == client.idClient.ToString()).First());
               lstClientsVisitesPDF.Add(client.idClient, 0);
               lstClientsCommandesPDF.Add(client.idClient, null);
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
                  lstClientsVisitesPDF[client.idClient]++;
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
                     lstCommDynamique.Add(comm);
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
                     lstDetCommDynamique = new List<PPDetailsCommandes>();
                     foreach (PPDetailsCommandes det in dc.GetTable<PPDetailsCommandes>().Where(m => m.NoCommande == comm.NoCommande).ToList())
                     {
                        lstDetCommDynamique.Add(det);
                        dc.GetTable<PPDetailsCommandes>().DeleteOnSubmit(det);
                     }
                     lstCommandesDtail.Add(comm, lstDetCommDynamique);
                     dc.GetTable<PPCommandes>().DeleteOnSubmit(comm);

                  }
                  lstClientsCommandesPDF[client.idClient] = lstCommDynamique;
                  if (!Directory.Exists(Server.MapPath("~/Inactiver")))
                  {
                     Directory.CreateDirectory(Server.MapPath("~/Inactiver"));
                  }
                  date = DateTime.Now.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds.ToString().Split(',')[0];
                  path = "~/Inactiver/" + date + ".pdf";
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
            }
            else
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
                  if (produitNonCommande.DateVente == null)
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
            cbVendeurs = lstVendeurs,
            blnOpenPDF = true,
            lastPDF = date
         };

         return View("GestionInactivite", renvoyer);
      }

      public ActionResult seePDF(string date)
      {

         string filePath = "~/Inactiver/" + date + ".pdf";

         Response.AddHeader("Content-Disposition", "inline; filename=" + date + ".pdf");
         System.Diagnostics.Debug.WriteLine(date);
         return File(filePath, "application/pdf");

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
               cbVendeur = cbVendeur.Where(m => m.dernierPresence < DateTime.Today.AddMonths(-1)).ToList();
               break;
            case "2":
               cbVendeur = cbVendeur.Where(m => m.dernierPresence < DateTime.Today.AddMonths(-3)).ToList();
               break;
            case "3":
               cbVendeur = cbVendeur.Where(m => m.dernierPresence < DateTime.Today.AddMonths(-6)).ToList();
               break;
            case "4":
               cbVendeur = cbVendeur.Where(m => m.dernierPresence < DateTime.Today.AddYears(-1)).ToList();
               break;
            case "5":
               cbVendeur = cbVendeur.Where(m => m.dernierPresence < DateTime.Today.AddYears(-2)).ToList();
               break;
            case "6":
               cbVendeur = cbVendeur.Where(m => m.dernierPresence < DateTime.Today.AddYears(-3)).ToList();
               break;
            case "7":
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

         foreach (PPVendeurs vendeur in dc.GetTable<PPVendeurs>().Where(m => m.Statut == 1).ToList())
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
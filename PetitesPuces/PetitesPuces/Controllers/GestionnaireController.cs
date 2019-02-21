using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
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

         //Aller chercher la liste des redevances dues trié par date.
         Dictionary<PPHistoriquePaiements, PPVendeurs> dicRedevances = new Dictionary<PPHistoriquePaiements, PPVendeurs>();
         var listeRedevances = (from redevance in db.GetTable<PPHistoriquePaiements>()
                                where redevance.Redevance > 0
                                orderby redevance.DateVente ascending
                                select redevance
                                ).ToList();

         foreach (var paiement in listeRedevances)
         {
            var vendeur = (from v in db.GetTable<PPVendeurs>()
                           where v.NoVendeur.Equals(paiement.NoVendeur)
                           select v
                           ).ToList();

            dicRedevances.Add(paiement, vendeur.First());
         }

         PPCategories c = new PPCategories();
         AccueilGestionnaireViewModel accueilGestionnaireViewModel = new AccueilGestionnaireViewModel(vendeurs, dicCategories,c);
         accueilGestionnaireViewModel.lstVendeurs = dicVendeurs;
         accueilGestionnaireViewModel.lstRedevances = dicRedevances;
         db.Connection.Close();
         return View(accueilGestionnaireViewModel);
      }

      [HttpPost]
      public ActionResult AccueilGestionnaire(AccueilGestionnaireViewModel viewModel)
      {
         Models.DataClasses1DataContext db = new Models.DataClasses1DataContext();
         db.Connection.Open();

         ViewBag.Section = "sec_ajout_categorie";
         if (ModelState.IsValid)
         {
            var categorieExisteDeja = (from cat in db.GetTable<PPCategories>()
                                       where cat.Description.Equals(viewModel.categorie.Description.Trim())
                                       select cat
                                       ).ToList();

            if(categorieExisteDeja.Count() == 0)
            {
               var query = (from nbCat in db.GetTable<PPCategories>()
                            select nbCat
               ).ToList();

               viewModel.categorie.NoCategorie = (query.Count() + 1) * 10;

               db.PPCategories.InsertOnSubmit(viewModel.categorie);
               ModelState.Clear();
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
            else
            {
               ViewBag.ErreurCat = "Cette catégorie existe déjà.";
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
         //Aller chercher la liste des redevances dues trié par date.
         Dictionary<PPHistoriquePaiements, PPVendeurs> dicRedevances = new Dictionary<PPHistoriquePaiements, PPVendeurs>();
         var listeRedevances = (from redevance in db.GetTable<PPHistoriquePaiements>()
                                where redevance.Redevance > 0
                                orderby redevance.DateVente ascending
                                select redevance
                                ).ToList();

         foreach (var paiement in listeRedevances)
         {
            var vendeur = (from v in db.GetTable<PPVendeurs>()
                           where v.NoVendeur.Equals(paiement.NoVendeur)
                           select v
                           ).ToList();

            dicRedevances.Add(paiement, vendeur.First());
         }
         PPCategories c = new PPCategories();
         AccueilGestionnaireViewModel accueilGestionnaireViewModel = new AccueilGestionnaireViewModel(vendeurs, dicCategories, c);
         accueilGestionnaireViewModel.lstVendeurs = dicVendeurs;
         accueilGestionnaireViewModel.lstRedevances = dicRedevances;
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
         //Aller chercher la liste des redevances dues trié par date.
         Dictionary<PPHistoriquePaiements, PPVendeurs> dicRedevances = new Dictionary<PPHistoriquePaiements, PPVendeurs>();
         var listeRedevances = (from red in db.GetTable<PPHistoriquePaiements>()
                                where red.Redevance > 0
                                orderby red.DateVente ascending
                                select red
                                ).ToList();

         foreach (var paiement in listeRedevances)
         {
            var vendeur = (from v in db.GetTable<PPVendeurs>()
                           where v.NoVendeur.Equals(paiement.NoVendeur)
                           select v
                           ).ToList();

            dicRedevances.Add(paiement, vendeur.First());
         }

         PPCategories c = new PPCategories();
         AccueilGestionnaireViewModel accueilGestionnaireViewModel = new AccueilGestionnaireViewModel(vendeurs, dicCategories, c);
         accueilGestionnaireViewModel.lstVendeurs = dicVendeurs;
         accueilGestionnaireViewModel.lstRedevances = dicRedevances;
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

         //Aller chercher la liste des redevances dues trié par date.
         Dictionary<PPHistoriquePaiements, PPVendeurs> dicRedevances = new Dictionary<PPHistoriquePaiements, PPVendeurs>();
         var listeRedevances = (from redevance in db.GetTable<PPHistoriquePaiements>()
                                where redevance.Redevance > 0
                                orderby redevance.DateVente ascending
                                select redevance
                                ).ToList();

         foreach (var paiement in listeRedevances)
         {
            var vendeur = (from v in db.GetTable<PPVendeurs>()
                           where v.NoVendeur.Equals(paiement.NoVendeur)
                           select v
                           ).ToList();

            dicRedevances.Add(paiement, vendeur.First());
         }

         PPCategories c = new PPCategories();
         AccueilGestionnaireViewModel accueilGestionnaireViewModel = new AccueilGestionnaireViewModel(vendeurs, dicCategories, c);
         accueilGestionnaireViewModel.lstVendeurs = dicVendeurs;
         accueilGestionnaireViewModel.lstRedevances = dicRedevances;
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
         //Aller chercher la liste des redevances dues trié par date.
         Dictionary<PPHistoriquePaiements, PPVendeurs> dicRedevances = new Dictionary<PPHistoriquePaiements, PPVendeurs>();
         var listeRedevances = (from red in db.GetTable<PPHistoriquePaiements>()
                                where red.Redevance > 0
                                orderby red.DateVente ascending
                                select red
                                ).ToList();

         foreach (var paiement in listeRedevances)
         {
            var vendeur = (from v in db.GetTable<PPVendeurs>()
                           where v.NoVendeur.Equals(paiement.NoVendeur)
                           select v
                           ).ToList();

            dicRedevances.Add(paiement, vendeur.First());
         }

         PPCategories c = new PPCategories();
         AccueilGestionnaireViewModel accueilGestionnaireViewModel = new AccueilGestionnaireViewModel(vendeurs, dicCategories, c);
         accueilGestionnaireViewModel.lstVendeurs = dicVendeurs;
         accueilGestionnaireViewModel.lstRedevances = dicRedevances;
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
         //Aller chercher la liste des redevances dues trié par date.
         Dictionary<PPHistoriquePaiements, PPVendeurs> dicRedevances = new Dictionary<PPHistoriquePaiements, PPVendeurs>();
         var listeRedevances = (from redevance in db.GetTable<PPHistoriquePaiements>()
                                where redevance.Redevance > 0
                                orderby redevance.DateVente ascending
                                select redevance
                                ).ToList();

         foreach (var paiement in listeRedevances)
         {
            var vendeur = (from v in db.GetTable<PPVendeurs>()
                           where v.NoVendeur.Equals(paiement.NoVendeur)
                           select v
                           ).ToList();

            dicRedevances.Add(paiement, vendeur.First());
         }

         PPCategories c = new PPCategories();
         AccueilGestionnaireViewModel accueilGestionnaireViewModel = new AccueilGestionnaireViewModel(vendeurs, dicCategories, c);
         accueilGestionnaireViewModel.lstVendeurs = dicVendeurs;
         accueilGestionnaireViewModel.lstRedevances = dicRedevances;
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

         //Aller chercher la liste des redevances dues trié par date.
         Dictionary<PPHistoriquePaiements, PPVendeurs> dicRedevances = new Dictionary<PPHistoriquePaiements, PPVendeurs>();
         var listeRedevances = (from red in db.GetTable<PPHistoriquePaiements>()
                                where red.Redevance > 0
                                orderby red.DateVente ascending
                                select red
                                ).ToList();

         foreach (var paiement in listeRedevances)
         {
            var vendeur = (from v in db.GetTable<PPVendeurs>()
                           where v.NoVendeur.Equals(paiement.NoVendeur)
                           select v
                           ).ToList();

            dicRedevances.Add(paiement, vendeur.First());
         }

         PPCategories c = new PPCategories();
         AccueilGestionnaireViewModel accueilGestionnaireViewModel = new AccueilGestionnaireViewModel(vendeurs, dicCategories, c);
         accueilGestionnaireViewModel.lstVendeurs = dicVendeurs;
         accueilGestionnaireViewModel.lstRedevances = dicRedevances;
         db.Connection.Close();
         return View("AccueilGestionnaire", accueilGestionnaireViewModel);
      }


      public ActionResult EnvoyerMessageDemandeVendeur(int noDestinataire, int noExpediteur, string message)
      {
         Models.DataClasses1DataContext db = new Models.DataClasses1DataContext();
         db.Connection.Open();
         int noMessages = 1;
         //Aller chercher le dernier noMessage
         var messages = (from msg in db.GetTable<PPMessages>()
                         group msg by true into r
                         select new
                         {
                            max = r.Max(max => max.NoMsg)
                         }
                         ).ToList();
         if(messages.Count() > 0)
         {
            noMessages = messages.First().max + 1;
         }

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

         //Aller chercher la liste des redevances dues trié par date.
         Dictionary<PPHistoriquePaiements, PPVendeurs> dicRedevances = new Dictionary<PPHistoriquePaiements, PPVendeurs>();
         var listeRedevances = (from redevance in db.GetTable<PPHistoriquePaiements>()
                                where redevance.Redevance > 0
                                orderby redevance.DateVente ascending
                                select redevance
                                ).ToList();

         foreach (var paiement in listeRedevances)
         {
            var vendeur = (from v in db.GetTable<PPVendeurs>()
                           where v.NoVendeur.Equals(paiement.NoVendeur)
                           select v
                           ).ToList();

            dicRedevances.Add(paiement, vendeur.First());
         }
         PPCategories c = new PPCategories();
         AccueilGestionnaireViewModel accueilGestionnaireViewModel = new AccueilGestionnaireViewModel(vendeurs, dicCategories, c);
         accueilGestionnaireViewModel.lstVendeurs = dicVendeurs;
         accueilGestionnaireViewModel.lstRedevances = dicRedevances;
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

         /*var gestionnaire = (from g in db.GetTable<PPGestionnaire>()
                             where g.NoGestionnaire.Equals(((PPGestionnaire)Session["gestionnaireObj"]).NoGestionnaire)
                             select g
                             ).First();*/
         
         db.Connection.Close();

         return View(vendeur.First());
      }

      //permet de payer la redevance
      public ActionResult PayerRedevance(int id)
      {
         Models.DataClasses1DataContext db = new Models.DataClasses1DataContext();
         db.Connection.Open();
         //Aller chercher l'historique paiement à l'aide de l'id passé en paramètre
         var histoPaiement = (from histo in db.GetTable<PPHistoriquePaiements>()
                              where histo.NoHistorique.Equals(id)
                              select histo
                              ).ToList();


         if(histoPaiement.Count() > 0)
         {
            PPHistoriquePaiements redevanceAPayer = histoPaiement.First();
            redevanceAPayer.Redevance = -(redevanceAPayer.Redevance);
            try
            {
               db.SubmitChanges();
            }
            catch(Exception e)
            {
               //rien besoin de faire.
            }
            db.Connection.Close();
            return new HttpStatusCodeResult(HttpStatusCode.OK);
         }
         else
         {
            db.Connection.Close();
            return new HttpStatusCodeResult(HttpStatusCode.NotAcceptable);
         }
      }

      //Ce méthode permet d'apporter les tris nécessaire au tableau de redevances dues.
      public ActionResult triRedevance(string tri)
      {
         Models.DataClasses1DataContext db = new Models.DataClasses1DataContext();
         db.Connection.Open();
         Dictionary<PPHistoriquePaiements, PPVendeurs> dicRedevances = new Dictionary<PPHistoriquePaiements, PPVendeurs>();
         
         if (tri.Equals("DateA"))
         {
            var query = (from redevance in db.GetTable<PPHistoriquePaiements>()
                         where redevance.Redevance > 0
                         orderby redevance.DateVente ascending
                         select redevance
                         ).ToList();
            //query.GroupBy(a => a.NoVendeur);

            foreach(var red in query)
            {
               var coucou = (from vendeur in db.GetTable<PPVendeurs>()
                             where vendeur.NoVendeur.Equals(red.NoVendeur)
                             select vendeur
                             ).ToList();

               dicRedevances.Add(red, coucou.First());
            }
            db.Connection.Close();
            return PartialView("Gestionnaire/RedevancesDues", dicRedevances);
         }
         else if (tri.Equals("DateD"))
         {
            var query = (from redevance in db.GetTable<PPHistoriquePaiements>()
                         where redevance.Redevance > 0
                         orderby redevance.DateVente descending
                         select redevance
                         ).ToList();
            //query.GroupBy(a => a.NoVendeur);

            foreach (var red in query)
            {
               var coucou = (from vendeur in db.GetTable<PPVendeurs>()
                             where vendeur.NoVendeur.Equals(red.NoVendeur)
                             select vendeur
                             ).ToList();

               dicRedevances.Add(red, coucou.First());
            }
            db.Connection.Close();
            return PartialView("Gestionnaire/RedevancesDues", dicRedevances);
         }
         else if (tri.Equals("Vendeur"))
         {
            var query = (from redevance in db.GetTable<PPHistoriquePaiements>()
                         where redevance.Redevance > 0
                         orderby redevance.NoVendeur ascending
                         select redevance
                         ).ToList();

            foreach (var red in query)
            {
               var coucou = (from vendeur in db.GetTable<PPVendeurs>()
                             where vendeur.NoVendeur.Equals(red.NoVendeur)
                             select vendeur
                             ).ToList();

               dicRedevances.Add(red, coucou.First());
            }
            db.Connection.Close();
            return PartialView("Gestionnaire/RedevancesDues", dicRedevances);
         }

         //Le tri n'est pas dans les choix permis
         db.Connection.Close();
         return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
      }

      public ActionResult GestionInactivite()
      {

            List<ddLInactiviter> lstInactiviterDdl = new List<ddLInactiviter>
        {
            new ddLInactiviter { Valeur = "", Texte = ""},
            new ddLInactiviter { Valeur = "1", Texte = "1 mois et +"},
            new ddLInactiviter { Valeur = "2", Texte = "3 mois et +"},
            new ddLInactiviter { Valeur = "3", Texte = "6 mois et +"},
            new ddLInactiviter { Valeur = "4", Texte = "1 an et +"},
            new ddLInactiviter { Valeur = "5", Texte = "2 an et +"},
            new ddLInactiviter { Valeur = "6", Texte = "3 an et +"},
        };
            ViewBag.ListeDdlClient = new SelectList(lstInactiviterDdl, "Valeur", "Texte");

            ViewBag.ListeDdlVendeur = new SelectList(lstInactiviterDdl, "Valeur", "Texte");

            List<Inactiver> lstClient = creeClient();
         List<Inactiver> lstVendeur = creeVendeur();
         InactiviteViewModel iVM = new InactiviteViewModel
         {
            cbClients = lstClient,
            cbVendeurs = lstVendeur,
            valDdlClient = "",
            valDdlVendeur = "",
         };
         return View("GestionInactivite", iVM);

      }

      [HttpPost]
      // Bouton confirmer
      public ActionResult GestionInactivite(InactiviteViewModel form)
      {
            List<ddLInactiviter> lstInactiviterDdl = new List<ddLInactiviter>
        {
            new ddLInactiviter { Valeur = "", Texte = ""},
            new ddLInactiviter { Valeur = "1", Texte = "1 mois et +"},
            new ddLInactiviter { Valeur = "2", Texte = "3 mois et +"},
            new ddLInactiviter { Valeur = "3", Texte = "6 mois et +"},
            new ddLInactiviter { Valeur = "4", Texte = "1 an et +"},
            new ddLInactiviter { Valeur = "5", Texte = "2 an et +"},
            new ddLInactiviter { Valeur = "6", Texte = "3 an et +"},
        };
            ViewBag.ListeDdlClient = new SelectList(lstInactiviterDdl, "Valeur", "Texte");

            ViewBag.ListeDdlVendeur = new SelectList(lstInactiviterDdl, "Valeur", "Texte");

            List<Inactiver> lstClients = new List<Inactiver>();
         List<Inactiver> lstVendeurs = new List<Inactiver>();
         List<Inactiver> lstClientsDeleter = new List<Inactiver>();
         List<Inactiver> lstVendeursDeleter = new List<Inactiver>();
         List<PPArticlesEnPanier> lstPanierAVider = new List<PPArticlesEnPanier>();
         List<PPProduits> lstProduitNonCommander = new List<PPProduits>();

            List<PPClients> lstClientRetirer = new List<PPClients>();
         List<PPCommandes> lstCommDynamique = new List<PPCommandes>();
         List<PPDetailsCommandes> lstDetCommDynamique = new List<PPDetailsCommandes>();

         //Retirer client
         foreach (Inactiver client in form.cbClients)
         {
            if (client.IsSelected == true)
            {
               // On vide le panier
               foreach (PPArticlesEnPanier art in dc.GetTable<PPArticlesEnPanier>().Where(m => m.NoClient.ToString() == client.idClient).ToList())
               {
                  lstPanierAVider.Add(art);
               }
               if (lstPanierAVider.Count > 0)
               {
                  dc.GetTable<PPArticlesEnPanier>().DeleteAllOnSubmit(lstPanierAVider);
               }

               // Si le client a utilisé le site Web
               if (dc.GetTable<PPEvaluations>().Where(m=> m.NoClient.ToString() == client.idClient).ToList().Count > 0 || dc.GetTable<PPCommandes>().Where(m => m.NoClient.ToString() == client.idClient).ToList().Count > 0 || dc.GetTable<PPVendeursClients>().Where(m => m.NoClient.ToString() == client.idClient).ToList().Count > 0)
               {
                        // On met le statut à 2 (Intégrité)
                    lstClientRetirer.Add(dc.GetTable<PPClients>().Where(m => m.NoClient.ToString() == client.idClient).First());
                  dc.GetTable<PPClients>().Where(m => m.NoClient.ToString() == client.idClient).First().Statut = 2;

               }

               // Client n'ayant pas utilisé le site Web
               else
               {
                  // On delete (Déjà retiré des autres tables)
                  dc.GetTable<PPClients>().DeleteOnSubmit(dc.GetTable<PPClients>().Where(m => m.NoClient.ToString() == client.idClient).First());
               }
               dc.SubmitChanges();
            }
            else
            {
               // On renvoit les clients non-retirés
               lstClients.Add(client);
            }
         }

         //Retirer vendeur
         foreach (Inactiver vendeur in form.cbVendeurs)
         {
            if (vendeur.IsSelected == true)
            {
               foreach (PPProduits produitNonCommande in dc.GetTable<PPProduits>().Where(m => m.NoVendeur.ToString() == vendeur.idClient).ToList())
               {
                  if (produitNonCommande.PPDetailsCommandes.ToList().Count == 0)
                  {
                     lstProduitNonCommander.Add(produitNonCommande);
                  }
                  else
                  {
                     dc.GetTable<PPProduits>().Where(m => m.NoProduit == produitNonCommande.NoProduit).First().Disponibilité = false;
                  }
               }
                    dc.GetTable<PPArticlesEnPanier>().DeleteAllOnSubmit(dc.GetTable<PPArticlesEnPanier>().Where(m => m.NoVendeur.ToString() == vendeur.idClient));
                dc.GetTable<PPProduits>().DeleteAllOnSubmit(lstProduitNonCommander);
                dc.GetTable<PPVendeurs>().Where(m => m.NoVendeur.ToString() == vendeur.idClient).First().Statut = 2;
                dc.SubmitChanges();
            }
            else
            {
               // On renvoit les vendeurs non-retirés
               lstVendeurs.Add(vendeur);
            }
         }
         ModelState.Clear();
         dc = new DataClasses1DataContext();

            //ViewData["ddlClient"] = Request["ddlClient"];
            //ViewData["ddlVendeur"] = Request["ddlVendeur"];
            string valeurRechercheClient = "";
            string valeurRechercheVendeur = "";
            if(form.valDdlVendeur != null)
            {
                valeurRechercheVendeur = form.valDdlVendeur;
            }
            if(form.valDdlClient != null)
            {
                valeurRechercheClient = form.valDdlClient;
            }
            InactiviteViewModel renvoyer = new InactiviteViewModel
            {
                cbClients = lstClients,
                lstClientsRetirer = lstClientRetirer,
            cbVendeurs = lstVendeurs,
            valDdlClient = valeurRechercheClient,
            valDdlVendeur = valeurRechercheVendeur
            };

         return View("GestionInactivite", renvoyer);
      }

      /// <summary>
      /// Cette méthode permet d'accéder au statistiques et fait les requetes nécessaire pour accéder au données
      /// </summary>
      /// <returns></returns>
      public ActionResult Statistiques()
      {
         Models.DataClasses1DataContext db = new Models.DataClasses1DataContext();
         db.Connection.Open();
         //Aller chercher le nombre de vendeur actif et inactif
         int nbVendeurActif = 0;
         int nbVendeurInactif = 0;

         int nbClientActif = 0;
         int nbClientPotentiel = 0;
         int nbClientVisiteurs = 0;

         int nbClientUnMois = 0;
         int nbClientTroisMois = 0;
         int nbClientSixMois = 0;
         int nbClientNeufMois = 0;
         int nbClientDouzeMois = 0;

         int nbVendeurUnMois = 0;
         int nbVendeurTroisMois = 0;
         int nbVendeurSixMois = 0;
         int nbVendeurNeufMois = 0;
         int nbVendeurDouzeMois = 0;

         int nbConnexionsClients = 0;

         int nbClientsActif = 0;
         int nbClientsPotentiel = 0;
         int nbClientsVisiteurs = 0;

         var vendeurs = (from v in db.GetTable<PPVendeurs>()
                         select v
                         ).ToList();
         foreach(var item in vendeurs)
         {
            if (item.Statut==1)
            {
               nbVendeurActif++;
            }
            if (item.Statut==2)
            {
               nbVendeurInactif++;
            }   
         }

         //Aller chercher Nombre total de clients (actif, potentiel et visiteur)
         var clients = (from c in db.GetTable<PPClients>()
                        select c
                        ).ToList();

         foreach(var cli in clients)
         {
            var dejaCommande = (from commande in db.GetTable<PPCommandes>()
                                where commande.NoClient.Equals(cli.NoClient)
                                select commande
                                ).ToList();

            var possedePanier = (from panier in db.GetTable<PPArticlesEnPanier>()
                                 where cli.NoClient.Equals(panier.NoClient)
                                 select panier
                                 ).ToList();

            if(dejaCommande.Count > 0)
            {
               nbClientActif++;
            }
            else if(possedePanier.Count() > 0)
            {
               nbClientPotentiel++;
            }
            else
            {
               nbClientVisiteurs++;
            }
         }

         //Aller chercher les nouveau comptes clients/vendeurs depuis 1, 3, 6, 9 et 12 mois
         foreach(var client in clients)
         {
            if((client.DateCreation <= DateTime.Now) && (client.DateCreation >= DateTime.Now.AddMonths(-1)))
            {
               nbClientUnMois++;
            }
            if ((client.DateCreation <= DateTime.Now) && (client.DateCreation >= DateTime.Now.AddMonths(-3)))
            {
               nbClientTroisMois++;
            }
            if ((client.DateCreation <= DateTime.Now) && (client.DateCreation >= DateTime.Now.AddMonths(-6)))
            {
               nbClientSixMois++;
            }
            if ((client.DateCreation <= DateTime.Now) && (client.DateCreation >= DateTime.Now.AddMonths(-9)))
            {
               nbClientNeufMois++;
            }
            if ((client.DateCreation <= DateTime.Now) && (client.DateCreation >= DateTime.Now.AddMonths(-12)))
            {
               nbClientDouzeMois++;
            }

         }

         foreach(var vendeur in vendeurs)
         {
            if ((vendeur.DateCreation <= DateTime.Now) && (vendeur.DateCreation >= DateTime.Now.AddMonths(-1)))
            {
               nbVendeurUnMois++;
            }
            if ((vendeur.DateCreation <= DateTime.Now) && (vendeur.DateCreation >= DateTime.Now.AddMonths(-3)))
            {
               nbVendeurTroisMois++;
            }
            if ((vendeur.DateCreation <= DateTime.Now) && (vendeur.DateCreation >= DateTime.Now.AddMonths(-6)))
            {
               nbVendeurSixMois++;
            }
            if ((vendeur.DateCreation <= DateTime.Now) && (vendeur.DateCreation >= DateTime.Now.AddMonths(-9)))
            {
               nbVendeurNeufMois++;
            }
            if ((vendeur.DateCreation <= DateTime.Now) && (vendeur.DateCreation >= DateTime.Now.AddMonths(-12)))
            {
               nbVendeurDouzeMois++;
            }
         }

         //Nombre de connexions des clients
         foreach(var cli in clients)
         {
            if(cli.NbConnexions != null)
            {
               nbConnexionsClients += int.Parse(cli.NbConnexions.ToString());
            }
            
         }

         //Aller chercher la listes de client en ordre de connexion décroissante
         var derniereConnexionClient = (from clicli in db.GetTable<PPClients>()
                                        orderby clicli.DateDerniereConnexion descending
                                        select clicli
                                        ).ToList();

         //Aller chercher les stats du premier vendeur de la liste pour les stats client par vendeur
         //Aller chercher la liste de tous les clients

         //Parcourir tout les clients
         foreach (var client in clients)
         {
            var dejaCommande = (from commande in db.GetTable<PPCommandes>()
                                where (commande.NoClient.Equals(client.NoClient)) &&
                                (commande.NoVendeur.Equals(vendeurs.First().NoVendeur))
                                select commande
                         ).ToList();
            var possedePanier = (from panier in db.GetTable<PPArticlesEnPanier>()
                                 where (panier.NoClient.Equals(client.NoClient)) && (panier.NoVendeur.Equals(vendeurs.First().NoVendeur))
                                 select panier
                                 ).ToList();

            if (dejaCommande.Count() > 0)
            {
               nbClientsActif++;
            }
            else if (possedePanier.Count() > 0)
            {
               nbClientsPotentiel++;
            }
            else
            {
               nbClientsVisiteurs++;
            }
         }
         List<int> lstStats = new List<int>();
         lstStats.Add(nbClientsActif);
         lstStats.Add(nbClientsPotentiel);
         lstStats.Add(nbClientsVisiteurs);

         //Nombre de visites d'un client pour un vendeur
         Dictionary<PPClients, int> dicVisitesClientsVendeurs = new Dictionary<PPClients, int>();
         //liste de tout les clients
         foreach (var client in clients)
         {
            var query = (from clientVendeur in db.GetTable<PPVendeursClients>()
                           where (clientVendeur.NoClient.Equals(client.NoClient)) &&
                           (clientVendeur.NoVendeur.Equals(vendeurs.First().NoVendeur))
                           select clientVendeur
                           ).ToList();
            dicVisitesClientsVendeurs.Add(client, query.Count());
         }


         //Aller chercher le total des commandes de chaque client par vendeur
         List<TotalCommandesClientParVendeurViewModel> lstTotCommandes = new List<TotalCommandesClientParVendeurViewModel>();
         PPVendeurs vendeurStatistique = vendeurs.First();
         foreach (var cli in clients)
         {
            double montantTotalCommandes = 0;
            //double dblPrixTotal = 
            var commandeClient = (from commande in db.GetTable<PPCommandes>()
                                  where (commande.NoClient.Equals(cli.NoClient)) && (commande.NoVendeur.Equals(vendeurStatistique.NoVendeur))
                                  orderby commande.DateCommande descending
                                  select commande
                                  ).ToList();
            if(commandeClient.Count > 0)
            {
               foreach (var com in commandeClient)
               {
                  double dblMontantTotal = Convert.ToDouble(com.MontantTotAvantTaxes + com.TPS + com.TVQ + com.CoutLivraison);
                  montantTotalCommandes += dblMontantTotal;
               }
               lstTotCommandes.Add(new TotalCommandesClientParVendeurViewModel(cli, vendeurStatistique, montantTotalCommandes, Convert.ToDateTime(commandeClient.First().DateCommande)));
            }
         }

         StatistiquesViewModel statistiquesViewModel = new StatistiquesViewModel();
         //Instancier les propriétés du model
         statistiquesViewModel.nbVendeurAccepte = nbVendeurActif;
         statistiquesViewModel.nbVendeurRefuse = nbVendeurInactif;

         statistiquesViewModel.nbClientActif = nbClientActif;
         statistiquesViewModel.nbClientPotentiel = nbClientPotentiel;
         statistiquesViewModel.nbClientVisiteurs = nbClientVisiteurs;

         statistiquesViewModel.nbClientsUnMois = nbClientUnMois;
         statistiquesViewModel.nbClientsTroisMois = nbClientTroisMois;
         statistiquesViewModel.nbClientsSixMois = nbClientSixMois;
         statistiquesViewModel.nbClientsNeufMois = nbClientNeufMois;
         statistiquesViewModel.nbClientsDouzeMois = nbClientDouzeMois;

         statistiquesViewModel.nbVendeurUnMois = nbVendeurUnMois;
         statistiquesViewModel.nbVendeurTroisMois = nbVendeurTroisMois;
         statistiquesViewModel.nbVendeurSixMois = nbVendeurSixMois;
         statistiquesViewModel.nbVendeurNeufMois = nbVendeurNeufMois;
         statistiquesViewModel.nbVendeurDouzeMois = nbVendeurDouzeMois;

         statistiquesViewModel.nbConnexionsClient = nbConnexionsClients;

         if(derniereConnexionClient.Count > 10)
         {
            statistiquesViewModel.lstDereniereConnexion = derniereConnexionClient.Take(10).ToList();
         }
         else
         {
            statistiquesViewModel.lstDereniereConnexion = derniereConnexionClient;
         }
         statistiquesViewModel.lstVendeurs = vendeurs;

         statistiquesViewModel.lstClientsVendeur = lstStats;

         statistiquesViewModel.dicVisitesClientsVendeurs = dicVisitesClientsVendeurs;

         statistiquesViewModel.lstTotalCommandes = lstTotCommandes;

         db.Connection.Close();
         return View(statistiquesViewModel);
      }

      /// <summary>
      /// 
      /// </summary>
      public ActionResult StatsClientParVendeur(int id)
      {
         Models.DataClasses1DataContext db = new Models.DataClasses1DataContext();
         db.Connection.Open();
         //Variables utiles pour le calcul des stats
         int nbClientsActif = 0;
         int nbClientsPotentiel = 0;
         int nbClientsVisiteurs = 0;

         var vendeurExiste = (from v in db.GetTable<PPVendeurs>()
                              where v.NoVendeur.Equals(id)
                              select v
                              ).ToList();
         if(vendeurExiste.Count > 0)
         {
            //Aller chercher la liste de tous les clients
            var clients = (from c in db.GetTable<PPClients>()
                           select c
                           ).ToList();

            //Parcourir tout les clients
            foreach (var client in clients)
            {
               var dejaCommande = (from commande in db.GetTable<PPCommandes>()
                                   where (commande.NoClient.Equals(client.NoClient)) &&
                                   (commande.NoVendeur.Equals(id))
                                   select commande
                            ).ToList();
               var possedePanier = (from panier in db.GetTable<PPArticlesEnPanier>()
                                    where (panier.NoClient.Equals(client.NoClient)) && (panier.NoVendeur.Equals(id))
                                    select panier
                                    ).ToList();

               if (dejaCommande.Count() > 0)
               {
                  nbClientsActif++;
               }
               else if (possedePanier.Count() > 0)
               {
                  nbClientsPotentiel++;
               }
               else
               {
                  nbClientsVisiteurs++;
               }
            }
            List<int> lstStats = new List<int>();
            lstStats.Add(nbClientsActif);
            lstStats.Add(nbClientsPotentiel);
            lstStats.Add(nbClientsVisiteurs);
            return PartialView("Gestionnaire/StatsVendeurSpecifique", lstStats);
         }
         else
         {
            db.Connection.Close();
            //Le vendeur envoyer n'est pas dans la liste des vendeurs
            return new HttpStatusCodeResult(HttpStatusCode.NotAcceptable);
         }  
      }

      public ActionResult listeDernieresConnexion(int id)
      {
         Models.DataClasses1DataContext db = new Models.DataClasses1DataContext();
         db.Connection.Open();
         var derniereConnexionClient = (from clicli in db.GetTable<PPClients>()
                                        orderby clicli.DateDerniereConnexion descending
                                        select clicli
                                        ).ToList();
         if(id < derniereConnexionClient.Count())
         {
            db.Connection.Close();
            return PartialView("Gestionnaire/ListeDerniereConnexions",derniereConnexionClient.Take(id).ToList());
         }
         db.Connection.Close();
         return PartialView("Gestionnaire/ListeDerniereConnexions", derniereConnexionClient);
      }

      public ActionResult nbVisitesClientsVendeur(int id)
      {
         Models.DataClasses1DataContext db = new Models.DataClasses1DataContext();
         db.Connection.Open();
         Dictionary<PPClients, int> dicVisitesClientsVendeurs = new Dictionary<PPClients, int>();

         var vendeurExiste = (from vendeur in db.GetTable<PPVendeurs>()
                              where vendeur.NoVendeur.Equals(id)
                              select vendeur
                              ).ToList();

         if(vendeurExiste.Count > 0)
         {
            //liste de tout les clients
            var clients = (from c in db.GetTable<PPClients>()
                           select c
                           ).ToList();

            foreach (var client in clients)
            {
               var query = (from clientVendeur in db.GetTable<PPVendeursClients>()
                            where (clientVendeur.NoClient.Equals(client.NoClient)) &&
                            (clientVendeur.NoVendeur.Equals(id))
                            select clientVendeur
                            ).ToList();
               dicVisitesClientsVendeurs.Add(client, query.Count());
            }

            db.Connection.Close();
            return PartialView("Gestionnaire/GraphiqueVisitesClientVendeur", dicVisitesClientsVendeurs);
         }
         else
         {
            db.Connection.Close();
            //Le vendeur envoyer n'est pas dans la liste des vendeurs
            return new HttpStatusCodeResult(HttpStatusCode.NotAcceptable);
         }

      }

      public ActionResult ListeTotalCommandesClient(int id)
      {
         Models.DataClasses1DataContext db = new Models.DataClasses1DataContext();
         db.Connection.Open();

         //Aller chercher le total des commandes de chaque client par vendeur
         List<TotalCommandesClientParVendeurViewModel> lstTotCommandes = new List<TotalCommandesClientParVendeurViewModel>();
         var vendeurSelectionne = (from v in db.GetTable<PPVendeurs>()
                                   where v.NoVendeur.Equals(id)
                                   select v
                                   ).ToList();

         if(vendeurSelectionne.Count() > 0)
         {
            var clients = (from cl in db.GetTable<PPClients>()
                           select cl
               ).ToList();


            //PPVendeurs vendeurStatistique = vendeurs.First();
            foreach (var cli in clients)
            {
               double montantTotalCommandes = 0;
               //double dblPrixTotal = 
               var commandeClient = (from commande in db.GetTable<PPCommandes>()
                                     where (commande.NoClient.Equals(cli.NoClient)) && (commande.NoVendeur.Equals(vendeurSelectionne.First().NoVendeur))
                                     orderby commande.DateCommande descending
                                     select commande
                                     ).ToList();
               if (commandeClient.Count > 0)
               {
                  foreach (var com in commandeClient)
                  {
                     double dblMontantTotal = Convert.ToDouble(com.MontantTotAvantTaxes + com.TPS + com.TVQ + com.CoutLivraison);
                     montantTotalCommandes += dblMontantTotal;
                  }
                  lstTotCommandes.Add(new TotalCommandesClientParVendeurViewModel(cli, vendeurSelectionne.First(), montantTotalCommandes, Convert.ToDateTime(commandeClient.First().DateCommande)));
               }
            }
            return PartialView("Gestionnaire/ListeTotalCommandesClients", lstTotCommandes);
         }
         else
         {
            db.Connection.Close();
            //Le vendeur envoyer n'est pas dans la liste des vendeurs
            return new HttpStatusCodeResult(HttpStatusCode.NotAcceptable);
         }

      }

      public ActionResult ddlChanger(string id)
      {
         List<Inactiver> cbClients = creeClient();
         List<Inactiver> cbVendeur = creeVendeur();


            List<ddLInactiviter> lstInactiviterDdl = new List<ddLInactiviter>
        {
            new ddLInactiviter { Valeur = "", Texte = ""},
            new ddLInactiviter { Valeur = "1", Texte = "1 mois et +"},
            new ddLInactiviter { Valeur = "2", Texte = "3 mois et +"},
            new ddLInactiviter { Valeur = "3", Texte = "6 mois et +"},
            new ddLInactiviter { Valeur = "4", Texte = "1 an et +"},
            new ddLInactiviter { Valeur = "5", Texte = "2 an et +"},
            new ddLInactiviter { Valeur = "6", Texte = "3 an et +"},
        };
            ViewBag.ListeDdlClient = new SelectList(lstInactiviterDdl, "Valeur", "Texte");

            ViewBag.ListeDdlVendeur = new SelectList(lstInactiviterDdl, "Valeur", "Texte");

            switch (id.Split(';')[0])
         {
            case "1":
               cbClients = cbClients.Where(m => m.dernierPresence <= DateTime.Today.AddMonths(-1)).ToList();
               break;
            case "2":
               cbClients = cbClients.Where(m => m.dernierPresence <= DateTime.Today.AddMonths(-3)).ToList();
               break;
            case "3":
               cbClients = cbClients.Where(m => m.dernierPresence <= DateTime.Today.AddMonths(-6)).ToList();
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
            default:
               break;
         }
         ModelState.Clear();
         InactiviteViewModel renvoyer = new InactiviteViewModel
         {
            cbClients = cbClients,
            cbVendeurs = cbVendeur,
            valDdlClient = id.Split(';')[0],
            valDdlVendeur = id.Split(';')[1]
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
                   dernierPresence = dateDernierePresence,
                   AdresseEmail = client.AdresseEmail
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
                   dernierPresence = dateDernierePresence,
                   AdresseEmail = vendeur.AdresseEmail
                });
            counter++;
         }
         return vendeurs.OrderByDescending(m => m.dernierPresence).ToList();
      }
   }
}
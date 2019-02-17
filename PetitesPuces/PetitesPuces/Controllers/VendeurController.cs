using PetitesPuces.Models;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.IO;
using System.Net;

namespace PetitesPuces.Controllers
{
    public class VendeurController : Controller
    {
        DataClasses1DataContext contextPP = new DataClasses1DataContext();
        VendeurDao vendeurDao;

        public ActionResult Index() => View("AccueilVendeur");

        public ActionResult AccueilVendeur()
        {
            //A changer dans le futur pour la variable de session vendeur
            if (Session["vendeurObj"] == null)
            {
                return PartialView("index");
            }
            long noVendeur = (Session["vendeurObj"] as PPVendeurs).NoVendeur;
            Models.DataClasses1DataContext db = new Models.DataClasses1DataContext();
            db.Connection.Open();
            Dictionary<PPCommandes, List<PPDetailsCommandes>> lstDetailsProduitsCommandes = new Dictionary<PPCommandes, List<PPDetailsCommandes>>();
            //Aller chercher les commandes non traités
            var commandesNonTraite = (from commande in db.GetTable<PPCommandes>()
                                      where commande.NoVendeur.Equals((Session["vendeurObj"] as PPVendeurs).NoVendeur) && commande.Statut.Equals('N')
                                      orderby commande.DateCommande ascending
                                      select commande
                                      ).ToList();

            foreach (var commande in commandesNonTraite)
            {
                List<PPDetailsCommandes> lstDetailCommandes = new List<PPDetailsCommandes>() ;
                var query = (from detailsCommande in db.PPDetailsCommandes
                             where detailsCommande.NoCommande.Equals(commande.NoCommande)
                             select detailsCommande
                             ).ToList();
                lstDetailCommandes = query;
                lstDetailsProduitsCommandes.Add(commande, lstDetailCommandes);
            }

            //Aller chercher les paniers du vendeur
            var paniers = (from panier in db.GetTable<PPArticlesEnPanier>()
                           where panier.NoVendeur.Equals(noVendeur)
                           group panier by panier.PPClients
                           );

            //TODO : Aller chercher le nombres de visites quotidienne
            var visites = (from vendeurClient in db.GetTable<PPVendeursClients>()
                           where vendeurClient.NoVendeur.Equals(noVendeur)
                           select vendeurClient
                           ).ToList();

         //Aller chercher les clients actifs, potentiel et visiteurs du profil connecté.
         int nbClientsActif = 0;
         int nbClientsPotentiel = 0;
         int nbClientsVisiteurs = 0;

         var clients = (from c in db.GetTable<PPClients>()
                        select c
                        ).ToList();

         foreach (var client in clients)
         {
            var dejaCommande = (from commande in db.GetTable<PPCommandes>()
                                where (commande.NoClient.Equals(client.NoClient)) &&
                                (commande.NoVendeur.Equals(noVendeur))
                                select commande
                         ).ToList();
            var possedePanier = (from panier in db.GetTable<PPArticlesEnPanier>()
                                 where (panier.NoClient.Equals(client.NoClient)) && (panier.NoVendeur.Equals(noVendeur))
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

         //Créer un object AccueilVendeurViewModel afin de l'envoyer a ma vue
         AccueilVendeurViewModel model = new AccueilVendeurViewModel(lstDetailsProduitsCommandes, paniers,visites.Count());
         model.lstStatsClients = lstStats;
         db.Connection.Close();

            return View(model);
        }

        public ActionResult AjouterProduit()
        {
            Models.DataClasses1DataContext db = new Models.DataClasses1DataContext();
            db.Connection.Open();
            var categories = (from cat in db.GetTable<PPCategories>()
                              select cat
                              ).ToList();

            ViewBag.ListeCategories = new SelectList(categories, "NoCategorie", "Description");

            PPProduits produit = new PPProduits();
            produit.Disponibilité = true;
            produit.NoVendeur = (Session["vendeurObj"] as PPVendeurs).NoVendeur;
            produit.NoCategorie = 70;

            GestionProduitViewModel gestionProduit = new GestionProduitViewModel();
            gestionProduit.produit = produit;

            ViewBag.Message = "";
            ViewBag.PrixVenteErreur = "";
            ViewBag.Action = "Ajouter";
            ViewBag.Form = "AjouterProduit";
            return View("GestionProduit", gestionProduit);
        }

        [HttpPost]
        public ActionResult AjouterProduit(GestionProduitViewModel model)
        {
            ViewBag.Message = "";
            ViewBag.PrixVenteErreur = "";
            List<string> parts = new List<string>();
            Models.DataClasses1DataContext db = new Models.DataClasses1DataContext();
            db.Connection.Open();
            //ValidateModel(model.produit);
            //Pour les dropdownlist aller voir le fichier : ---------> vers la ligne 300 du clientController et dans les InformationPersonel
            //TODO: Ajouter le produit dans la BASE DE DONNÉES
            PPProduits prod = model.produit;

            //Ajouter le produit dans la base de donnée
            if (ModelState.IsValid)
            {
                if (model.produit.PrixDemande > model.produit.PrixVente)
                {
                    if (DateVentePrixVenteValide(model.produit.PrixVente, model.produit.DateVente))
                    {

                        if (model.file != null && model.file.ContentLength > 0)
                            try
                            {
                                var postedFileExtension = Path.GetExtension(model.file.FileName);
                                //Validation si c'est un image 
                                if (!string.Equals(model.file.ContentType, "image/jpg", StringComparison.OrdinalIgnoreCase) &&
                                   !string.Equals(model.file.ContentType, "image/jpeg", StringComparison.OrdinalIgnoreCase) &&
                                   !string.Equals(model.file.ContentType, "image/pjpeg", StringComparison.OrdinalIgnoreCase) &&
                                   !string.Equals(model.file.ContentType, "image/gif", StringComparison.OrdinalIgnoreCase) &&
                                   !string.Equals(model.file.ContentType, "image/x-png", StringComparison.OrdinalIgnoreCase) &&
                                   !string.Equals(model.file.ContentType, "image/png", StringComparison.OrdinalIgnoreCase))
                                {
                                    ViewBag.Message("Le fichier doit être une image");
                                }
                                else if (!string.Equals(postedFileExtension, ".jpg", StringComparison.OrdinalIgnoreCase)
                                        && !string.Equals(postedFileExtension, ".png", StringComparison.OrdinalIgnoreCase)
                                        && !string.Equals(postedFileExtension, ".gif", StringComparison.OrdinalIgnoreCase)
                                        && !string.Equals(postedFileExtension, ".jpeg", StringComparison.OrdinalIgnoreCase))
                                {
                                    ViewBag.Message("Le fichier doit être une image");
                                }
                                //Le nom de l'image sur le serveur sera le numeros du produit + l'extension de l'image
                                if (!ViewBag.Message.Equals("Le fichier doit être une image"))
                                {
                                    parts = model.file.FileName.Split('.').Select(f => f.Trim()).ToList();

                                    var nbProduit = (from produits in db.GetTable<PPProduits>()
                                                     select prod
                                    ).ToList();
                                    //Le pattern de num produit va être à retravailler.
                                    //prod.NoProduit = (nbProduit.Count() + 1) * 10;
                                    var maxNoProduitVendeur = (from pMax in db.GetTable<PPProduits>()
                                                               where pMax.NoVendeur.Equals((Session["vendeurObj"] as PPVendeurs).NoVendeur)
                                                               select pMax
                                                               ).ToList();
                                    long noProduit = maxNoProduitVendeur.Last().NoProduit + 1;
                                    prod.NoProduit = noProduit;
                                    prod.DateCreation = DateTime.Now;

                                    string path = Path.Combine(Server.MapPath("~/Content/images"),
                                                          (prod.NoProduit.ToString() + '.' + parts.ElementAt(1)));
                                    model.file.SaveAs(path);
                                    prod.Photo = (prod.NoProduit.ToString() + Path.GetExtension(path));


                                    db.PPProduits.InsertOnSubmit(prod);
                                    try
                                    {
                                        db.SubmitChanges();
                                        ModelState.Clear();
                                    }
                                    catch (Exception e)
                                    {

                                    }

                                    ViewBag.Message = "File uploaded successfully";
                                }
                            }
                            catch (Exception ex)
                            {
                                ViewBag.Message = "ERROR:" + ex.Message.ToString();
                            }
                        else
                        {
                            ViewBag.Message = "You have not specified a file.";
                        }
                    }
                    else
                    {
                        ViewBag.PrixVenteErreur = "Le prix de vente et la date doivent être rempli si il y a un rabais.";
                    }

                }
                else
                {
                    ViewBag.PrixVenteErreur = "Le prix de vente doit être plus bas que le prix demandé.";
                }

            }

            PPProduits p = new PPProduits();
            p.Disponibilité = true;
            p.NoVendeur = (Session["vendeurObj"] as PPVendeurs).NoVendeur;
            p.NoCategorie = 70;

            var categories = (from cat in db.GetTable<PPCategories>()
                              select cat
                              ).ToList();

            ViewBag.ListeCategories = new SelectList(categories, "NoCategorie", "Description");
            db.Connection.Close();
            GestionProduitViewModel gestionP = new GestionProduitViewModel();
            gestionP.produit = p;
            ViewBag.Action = "Ajouter";
            ViewBag.Form = "AjouterProduit";
            return View("GestionProduit", gestionP);
        }

        /// <summary>
        /// Cette fonction est appeler par la page catalogue du vendeur quand le vendeur tente de modifier un des produits
        /// de son catalogue
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ModifierProduit(int id)
        {
            Models.DataClasses1DataContext db = new Models.DataClasses1DataContext();
            db.Connection.Open();

            var produit = (from prod in db.GetTable<PPProduits>()
                           where prod.NoProduit.Equals(id)
                           select prod
                           ).ToList();

            PPProduits produitAModifier = produit.First();
            produitAModifier.Disponibilité = true;
            var categories = (from cat in db.GetTable<PPCategories>()
                              select cat
                              ).ToList();

            ViewBag.ListeCategories = new SelectList(categories, "NoCategorie", "Description");
            GestionProduitViewModel gestionProduit = new GestionProduitViewModel();
            gestionProduit.produit = produitAModifier;

            ViewBag.Message = "";
            ViewBag.PrixVenteErreur = "";
            ViewBag.Action = "Modifier";
            ViewBag.Form = "ModifierProduit";
            return View("GestionProduit", gestionProduit);
        }

        [HttpPost]
        public ActionResult ModifierProduit(GestionProduitViewModel model)
        {
            ViewBag.PrixVenteErreur = "";
            ViewBag.Message = "";
            Models.DataClasses1DataContext db = new Models.DataClasses1DataContext();
            db.Connection.Open();
            //ValidateModel(model.produit);
            //Pour les dropdownlist aller voir le fichier : ---------> vers la ligne 300 du clientController et dans les InformationPersonel
            //TODO: Ajouter le produit dans la BASE DE DONNÉES
            List<string> parts = new List<string>();

            if (ModelState.IsValid)
            {
                if ((model.produit.PrixDemande > model.produit.PrixVente) || (model.produit.PrixVente.Equals(null)))
                {
                    if (DateVentePrixVenteValide(model.produit.PrixVente, model.produit.DateVente))
                    {
                        if (model.file != null && model.file.ContentLength > 0)
                            try
                            {
                                var postedFileExtension = Path.GetExtension(model.file.FileName);
                                //Validation si c'est un image 
                                if (!string.Equals(model.file.ContentType, "image/jpg", StringComparison.OrdinalIgnoreCase) &&
                                   !string.Equals(model.file.ContentType, "image/jpeg", StringComparison.OrdinalIgnoreCase) &&
                                   !string.Equals(model.file.ContentType, "image/pjpeg", StringComparison.OrdinalIgnoreCase) &&
                                   !string.Equals(model.file.ContentType, "image/gif", StringComparison.OrdinalIgnoreCase) &&
                                   !string.Equals(model.file.ContentType, "image/x-png", StringComparison.OrdinalIgnoreCase) &&
                                   !string.Equals(model.file.ContentType, "image/png", StringComparison.OrdinalIgnoreCase))
                                {
                                    ViewBag.Message("Le fichier doit être une image");
                                }
                                else if (!string.Equals(postedFileExtension, ".jpg", StringComparison.OrdinalIgnoreCase)
                                        && !string.Equals(postedFileExtension, ".png", StringComparison.OrdinalIgnoreCase)
                                        && !string.Equals(postedFileExtension, ".gif", StringComparison.OrdinalIgnoreCase)
                                        && !string.Equals(postedFileExtension, ".jpeg", StringComparison.OrdinalIgnoreCase))
                                {
                                    ViewBag.Message("Le fichier doit être une image");
                                }
                                //Le nom de l'image sur le serveur sera le numeros du produit + l'extension de l'image
                                if (!ViewBag.Message.Equals("Le fichier doit être une image"))
                                {
                                    parts = model.file.FileName.Split('.').Select(p => p.Trim()).ToList();

                                    string path = Path.Combine(Server.MapPath("~/Content/images"),
                                                          (model.produit.NoProduit.ToString() + '.' + parts.ElementAt(1)));
                                    model.file.SaveAs(path);
                                    model.produit.Photo = model.produit.NoProduit + '.' + Path.GetExtension(path);
                                    ViewBag.Message = "File uploaded successfully";
                                }
                            }
                            catch (Exception ex)
                            {
                                ViewBag.Message = "ERROR:" + ex.Message.ToString();
                            }
                        else
                        {
                            //ViewBag.Message = "You have not specified a file.";
                        }
                        if (ViewBag.Message.Equals("File uploaded successfully") || ViewBag.Message.Equals(""))
                        {
                            var produitAModifier = (from prodAMod in db.GetTable<PPProduits>()
                                                    where prodAMod.NoProduit.Equals(model.produit.NoProduit)
                                                    select prodAMod
                               ).ToList();

                            PPProduits pAModifier = produitAModifier.First();

                            //Modifier les valeurs
                            pAModifier.Nom = model.produit.Nom;
                            pAModifier.PrixDemande = model.produit.PrixDemande;
                            pAModifier.Description = model.produit.Description;
                            pAModifier.DateCreation = model.produit.DateCreation;
                            pAModifier.NombreItems = model.produit.NombreItems;
                            pAModifier.PrixVente = model.produit.PrixVente;
                            pAModifier.DateVente = model.produit.DateVente;
                            pAModifier.Poids = model.produit.Poids;
                            pAModifier.Disponibilité = model.produit.Disponibilité;


                            if (ViewBag.Message.Equals("File uploaded successfully"))
                            {
                                pAModifier.Photo = (model.produit.NoProduit.ToString() + '.' + parts.ElementAt(1));
                            }
                            try
                            {
                                db.SubmitChanges();
                                ModelState.Clear();
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }
                        }
                    }
                    else
                    {
                        ViewBag.PrixVenteErreur = "Le prix de vente et la date doivent être rempli si il y a un rabais.";
                    }
                }
                else
                {
                    ViewBag.PrixVenteErreur = "Le prix de vente doit être plus petit que le prix demandé.";
                }
            }


            if ((ModelState.IsValid) && ((ViewBag.Message.Equals("File uploaded successfully")) || (ViewBag.Message.Equals(""))) && (ViewBag.PrixVenteErreur.Equals("")))
            {
                Dictionary<PPCommandes, List<PPDetailsCommandes>> lstDetailsProduitsCommandes = new Dictionary<PPCommandes, List<PPDetailsCommandes>>();
                //Aller chercher les commandes non traités
                var commandesNonTraite = (from commande in db.GetTable<PPCommandes>()
                                          where commande.NoVendeur.Equals((Session["vendeurObj"] as PPVendeurs).NoVendeur) && commande.Statut.Equals('N')
                                          select commande
                                          ).ToList();

                foreach (var commande in commandesNonTraite)
                {
                    List<PPDetailsCommandes> lstDetailCommandes = new List<PPDetailsCommandes>();
                    var query = (from detailsCommande in db.PPDetailsCommandes
                                 where detailsCommande.NoCommande.Equals(commande.NoCommande)
                                 select detailsCommande
                                 ).ToList();
                    lstDetailCommandes = query;
                    lstDetailsProduitsCommandes.Add(commande, lstDetailCommandes);
                }

                //Aller chercher les paniers du vendeur
                var paniers = (from panier in db.GetTable<PPArticlesEnPanier>()
                               where panier.NoVendeur.Equals((Session["vendeurObj"] as PPVendeurs).NoVendeur)
                               group panier by panier.PPClients
                               );

                //TODO : Aller chercher le nombres de visites quotidienne

                //Créer un object AccueilVendeurViewModel afin de l'envoyer a ma vue
                AccueilVendeurViewModel model2 = new AccueilVendeurViewModel(lstDetailsProduitsCommandes, paniers, 10);
                db.Connection.Close();
                return View("AccueilVendeur", model2);
            }
            else
            {
                var produit = (from prod in db.GetTable<PPProduits>()
                               where prod.NoProduit.Equals(model.produit.NoProduit)
                               select prod
                   ).ToList();

                PPProduits produitAModifier = produit.First();
                //produitAModifier.Disponibilité = true;
                var categories = (from cat in db.GetTable<PPCategories>()
                                  select cat
                                  ).ToList();

                ViewBag.ListeCategories = new SelectList(categories, "NoCategorie", "Description");
                GestionProduitViewModel gestionProduit = new GestionProduitViewModel();
                gestionProduit.produit = produitAModifier;


                ViewBag.Action = "Modifier";
                ViewBag.Form = "ModifierProduit";
                db.Connection.Close();
                return View("GestionProduit", gestionProduit);
            }
        }

        public ActionResult SupprimerProduit(int id)
        {
            Models.DataClasses1DataContext db = new Models.DataClasses1DataContext();
            db.Connection.Open();
            //1-Aller chercher le produit à supprimer
            var produitASupprimer = (from prod in db.GetTable<PPProduits>()
                                     where prod.NoProduit.Equals(id)
                                     select prod
                                     ).ToList();

            PPProduits prodASupprimer = produitASupprimer.First();

            //2- Vérifier si le produit est dans une commande
            var produitCommande = (from dc in db.GetTable<PPDetailsCommandes>()
                                   where dc.NoProduit.Equals(prodASupprimer.NoProduit)
                                   select dc
                                   ).ToList();

            //3-Vérifier si le produit est dans des paniers de client
            var produitPanier = (from articleEnPanier in db.GetTable<PPArticlesEnPanier>()
                                 where articleEnPanier.NoProduit.Equals(prodASupprimer.NoProduit)
                                 select articleEnPanier
                                 ).ToList();
            //Vérifier si le produit est dans des paniers
            if (produitPanier.Count() > 0)
            {
                //Vider le produit des paniers
                foreach (var item in produitPanier)
                {
                    db.PPArticlesEnPanier.DeleteOnSubmit(item);
                }

            }

            //Suppression du produit s'il n'est pas dans une commande
            if (produitCommande.Count <= 0)
            {
                //Supprimer le produit définitivement de la BD
                db.PPProduits.DeleteOnSubmit(prodASupprimer);
            }
            else
            {
                //Si le produit est présent dans une commande le champs Disponilité devient à False et
                //le champ quantité devient à 0.
                prodASupprimer.Disponibilité = false;
                prodASupprimer.NombreItems = 0;
            }

            //Add changes to DATABASE
            try
            {
                db.SubmitChanges();
            }
            catch (Exception e)
            {

            }

            //Retourner la page d'accueil client.
            Dictionary<PPCommandes, List<PPDetailsCommandes>> lstDetailsProduitsCommandes = new Dictionary<PPCommandes, List<PPDetailsCommandes>>();
            //Aller chercher les commandes non traités
            var commandesNonTraite = (from commande in db.GetTable<PPCommandes>()
                                      where commande.NoVendeur.Equals((Session["vendeurObj"] as PPVendeurs).NoVendeur) && commande.Statut.Equals('N')
                                      select commande
                                      ).ToList();

            foreach (var commande in commandesNonTraite)
            {
                List<PPDetailsCommandes> lstDetailCommandes = new List<PPDetailsCommandes>();
                var query = (from detailsCommande in db.PPDetailsCommandes
                             where detailsCommande.NoCommande.Equals(commande.NoCommande)
                             select detailsCommande
                             ).ToList();
                lstDetailCommandes = query;
                lstDetailsProduitsCommandes.Add(commande, lstDetailCommandes);
            }

            //Aller chercher les paniers du vendeur
            var paniers = (from panier in db.GetTable<PPArticlesEnPanier>()
                           where panier.NoVendeur.Equals((Session["vendeurObj"] as PPVendeurs).NoVendeur)
                           group panier by panier.PPClients
                           );

         
         //TODO : Aller chercher le nombres de visites quotidienne
         var visites = (from vendeurClient in db.GetTable<PPVendeursClients>()
                        where vendeurClient.NoVendeur.Equals((Session["vendeurObj"] as PPVendeurs).NoVendeur)
                        select vendeurClient
                        ).ToList();
         //Aller chercher les clients actifs, potentiel et visiteurs du profil connecté.
         int nbClientsActif = 0;
         int nbClientsPotentiel = 0;
         int nbClientsVisiteurs = 0;

         var clients = (from c in db.GetTable<PPClients>()
                        select c
                        ).ToList();

         foreach (var client in clients)
         {
            var dejaCommande = (from commande in db.GetTable<PPCommandes>()
                                where (commande.NoClient.Equals(client.NoClient)) &&
                                (commande.NoVendeur.Equals((Session["vendeurObj"] as PPVendeurs).NoVendeur))
                                select commande
                         ).ToList();
            var possedePanier = (from panier in db.GetTable<PPArticlesEnPanier>()
                                 where (panier.NoClient.Equals(client.NoClient)) && (panier.NoVendeur.Equals((Session["vendeurObj"] as PPVendeurs).NoVendeur))
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

         //Créer un object AccueilVendeurViewModel afin de l'envoyer a ma vue
         AccueilVendeurViewModel model2 = new AccueilVendeurViewModel(lstDetailsProduitsCommandes, paniers, visites.Count());
         model2.lstStatsClients = lstStats;
            db.Connection.Close();
            return View("AccueilVendeur", model2);
        }

        public ActionResult ProduitDansUnPanier(int id)
        {
            Models.DataClasses1DataContext db = new Models.DataClasses1DataContext();
            db.Connection.Open();
            //3-Vérifier si le produit est dans des paniers de client
            var produitPanier = (from articleEnPanier in db.GetTable<PPArticlesEnPanier>()
                                 where articleEnPanier.NoProduit.Equals(id)
                                 select articleEnPanier
                                 ).ToList();

            if (produitPanier.Count > 0)
            {
                db.Connection.Close();
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            else
            {
                db.Connection.Close();
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }

        }

        // GET: Vendeur
        public ActionResult GestionCommande()
        {
            List<GererCommandeViewModel.GererCommande> lstAGerer = new List<GererCommandeViewModel.GererCommande>();
            if (Session["vendeurObj"] == null)
            {
                return PartialView("index");
            }
            long noVendeur = (Session["vendeurObj"] as PPVendeurs).NoVendeur;

            // Action qui va sevir à marqué les commandes cochez comme L au lieu de N

            List<PPCommandes> lstCommandeEnAttente = (from uneCommande in contextPP.GetTable<PPCommandes>()
                                                      where uneCommande.NoVendeur.Equals(noVendeur) && uneCommande.Statut.Equals('N')
                                                      orderby uneCommande.DateCommande descending
                                                      select uneCommande).ToList();
            foreach (PPCommandes comm in lstCommandeEnAttente)
            {
                GererCommandeViewModel.GererCommande unComm = new GererCommandeViewModel.GererCommande
                {
                    commande = comm,
                    isChecked = false
                };
                lstAGerer.Add(unComm);
            }

            List<PPCommandes> lstCommandeLivré = (from uneCommande in contextPP.GetTable<PPCommandes>()
                                                  where uneCommande.NoVendeur.Equals(noVendeur) && uneCommande.Statut.Equals('L')
                                                  orderby uneCommande.DateCommande descending
                                                  select uneCommande).ToList();
            GererCommandeViewModel gererComm = new GererCommandeViewModel
            {
                lstCommandeLivrer = lstCommandeLivré,
                lstCommandeNonLivrer = lstAGerer
            };

            return View(gererComm);
        }

        // Lorsqu'une commande est marquée comme livrée
        [HttpPost]
        public ActionResult GestionCommande(string bidon)
        {
            if (Session["vendeurObj"] == null)
            {
                return PartialView("index");
            }
            long noVendeur = (Session["vendeurObj"] as PPVendeurs).NoVendeur;

            // Action qui va sevir à marqué les commandes cochez comme L au lieu de N

            List<PPCommandes> lstCommandeEnAttente = (from uneCommande in contextPP.GetTable<PPCommandes>()
                                                      where uneCommande.NoVendeur.Equals(noVendeur) && uneCommande.Statut.Equals('N')
                                                      orderby uneCommande.DateCommande descending
                                                      select uneCommande).ToList();

            List<PPCommandes> lstCommandeLivré = (from uneCommande in contextPP.GetTable<PPCommandes>()
                                                  where uneCommande.NoVendeur.Equals(noVendeur) && uneCommande.Statut.Equals('L')
                                                  orderby uneCommande.DateCommande descending
                                                  select uneCommande).ToList();
            List<List<PPCommandes>> lstDeuxCommandes = new List<List<PPCommandes>>();
            lstDeuxCommandes.Add(lstCommandeEnAttente);
            lstDeuxCommandes.Add(lstCommandeLivré);

            return View(lstDeuxCommandes);
        }
        //GET: GestionProduit
        public ActionResult GestionProduit() => View();

        //GET: CatalogueVendeur
        public ActionResult CatalogueVendeur(string tri, string categorie, string recherche, string recherche2, int? typeRech, int? page, int pageDimension = 5, int noPage = 1)
        {
            const String strTriNum = "numero";
            const String strTriCat = "categorie";
            const String strTriDate = "date";
            ViewBag.TriActuel = tri;

            bool booOrdre = false;
            if (!String.IsNullOrEmpty(tri))
            {
                booOrdre = tri[0] == '!';
            }

            ViewBag.TriNum = !String.IsNullOrEmpty(tri) && tri.Contains(strTriNum) ? (booOrdre ? strTriNum : "!" + strTriNum) : ViewBag.TriNum ?? strTriNum;
            ViewBag.TriCat = !String.IsNullOrEmpty(tri) && tri.Contains(strTriCat) ? (booOrdre ? strTriCat : "!" + strTriCat) : ViewBag.TriCat ?? strTriCat;
            ViewBag.TriDate = !String.IsNullOrEmpty(tri) && tri.Contains(strTriDate) ? (booOrdre ? strTriDate : "!" + strTriDate) : ViewBag.TriDate ?? strTriDate;
            //System.Diagnostics.Debug.WriteLine("tri1: " + (ViewBag.TriNum as String) + " Tri2: " + (ViewBag.TriCat as String) + " Tri3: " + (ViewBag.TriDate as String));

            List<PPProduits> lstDesProduits = contextPP.PPProduits.ToList();

            vendeurDao = new VendeurDao((Session["vendeurObj"] as PPVendeurs).NoVendeur);
            PPVendeurs vendeurOriginel = vendeurDao.rechecheVendeurParNo((Session["vendeurObj"] as PPVendeurs).NoVendeur);

            //Produit du vendeur en particulier (pas encore tester)

            lstDesProduits = lstDesProduits
                .Where(predicate: pro => pro.NoVendeur == vendeurOriginel.NoVendeur)
                .ToList();

            //tri
            switch (tri)
            {
                case strTriNum:
                    lstDesProduits = lstDesProduits.OrderBy(pro => pro.NoProduit)
                        .ToList();
                    break;
                case "!" + strTriNum:
                    lstDesProduits = lstDesProduits.OrderByDescending(pro => pro.NoProduit)
                        .ToList();
                    break;
                case strTriDate:
                    lstDesProduits = lstDesProduits.OrderBy(pro => pro.DateCreation)
                        .ThenBy(pro => pro.Nom)
                        .ToList();
                    break;
                case "!" + strTriDate:
                    lstDesProduits = lstDesProduits.OrderByDescending(pro => pro.DateCreation)
                        .ThenBy(pro => pro.Nom)
                        .ToList();
                    break;
                case strTriCat:
                    lstDesProduits = lstDesProduits.OrderBy(pro => pro.PPCategories.Description)
                        .ThenBy(pro => pro.Nom)
                        .ToList();
                    break;
                case "!" + strTriCat:
                    lstDesProduits = lstDesProduits.OrderByDescending(pro => pro.PPCategories.Description)
                        .ThenBy(pro => pro.Nom)
                        .ToList();
                    break;
            }

            //Si affichage d'une catégorie en particulier
            if (!String.IsNullOrWhiteSpace(categorie) &&
                contextPP.PPCategories.FirstOrDefault(predicate: cat => cat.Description.ToLower() == categorie.ToLower()) != null)
            {
                lstDesProduits = lstDesProduits
                    .Where(pro => String.Equals(pro.PPCategories.Description, categorie, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            //Si recherche dans le catalogue
            if (!String.IsNullOrWhiteSpace(recherche) && typeRech.HasValue && typeRech != 0)
            {
                string strMotRecherche = recherche.ToLower();

                switch (typeRech)
                {
                    case 1:
                        lstDesProduits = lstDesProduits
                            .Where(predicate: pro => pro.Nom.ToString().ToLower().Contains(strMotRecherche))
                            .ToList();
                        break;
                    case 2:
                        lstDesProduits = lstDesProduits
                            .Where(predicate: pro => pro.NoProduit.ToString().Contains(strMotRecherche))
                            .ToList();
                        break;
                    case 3:
                        lstDesProduits = lstDesProduits
                            .Where(predicate: pro => pro.Description.ToString().ToLower().Contains(strMotRecherche))
                            .ToList();
                        break;
                    case 4:
                        DateTime dtDebut;
                        DateTime dtFin;
                        /*
                        System.Diagnostics.Debug.WriteLine("Liste des produits par dates (Avant)");
                        lstDesProduits.ForEach(pro => System.Diagnostics.Debug.WriteLine(pro.DateCreation.Value.ToString("dd-MM-yyyy")));*/

                        try
                        {
                            dtDebut = Convert.ToDateTime(recherche);
                            dtFin = Convert.ToDateTime(recherche2);

                            lstDesProduits = lstDesProduits
                                .FindAll(pro => pro.DateCreation.Value >= dtDebut && pro.DateCreation.Value <= dtFin);
                            /*
                            System.Diagnostics.Debug.WriteLine("Liste des produits par dates (Après)");
                            lstDesProduits.ForEach(pro => System.Diagnostics.Debug.WriteLine(pro.DateCreation.Value.ToString("dd-MM-yyyy")));*/
                        }
                        catch (FormatException fe)
                        {
                            System.Diagnostics.Debug.WriteLine(fe);
                        }

                        break;
                    case 5:
                        lstDesProduits = lstDesProduits
                            .Where(predicate: pro => pro.PPCategories.Description.ToLower().Contains(strMotRecherche))
                            .ToList();
                        break;
                }
            }

            //Pagination
            List<string> lstSelectionNbItems = new List<string>
            {
                "5","10","15","20","25","tous"
            };

            int intNumeroPage = (page ?? 1);


            ViewModels.CatalogueViewModel catVM = new ViewModels.CatalogueViewModel
            {
                lstCategorie = contextPP.PPCategories.ToList(),
                strTri = tri,
                strCategorie = categorie,
                recherche = recherche,
                recherche2 = recherche2,
                pageDimension = pageDimension,
                intNoPage = noPage,
                iplProduits = lstDesProduits.ToPagedList(intNumeroPage, pageDimension)
            };

            if (typeRech.HasValue)
            {
                catVM.typeRech = typeRech.Value;
            }

            ViewBag.ListeNbItems = new SelectList(lstSelectionNbItems, pageDimension);


            return View(catVM);
        }

        public ActionResult GestionProfilVendeur()
        {
            //String strAdresseCourrielVendeur = "L.CHAPLEAU@TOTO.COM";
            vendeurDao = new VendeurDao((Session["vendeurObj"] as PPVendeurs).NoVendeur);

            List<Province> lstProvinces = new List<Province>
            {
                new Province { Abreviation = "NB", Nom = "Nouveau-Brunswick"},
                new Province { Abreviation = "ON", Nom = "Ontario"},
                new Province { Abreviation = "QC", Nom = "Québec"},
            };

            ViewBag.ListeProvinces = new SelectList(lstProvinces, "Abreviation", "Nom");

            PPVendeurs unVendeur = vendeurDao.rechecheVendeurParNo((Session["vendeurObj"] as PPVendeurs).NoVendeur);


            return View(unVendeur);
        }

        [HttpPost]
        public ActionResult GestionProfilVendeur(PPVendeurs vendeur, String strProvenence, String police, String fond, String baniere )
        {
            List<Province> lstProvinces = new List<Province>
            {
                new Province { Abreviation = "NB", Nom = "Nouveau-Brunswick"},
                new Province { Abreviation = "ON", Nom = "Ontario"},
                new Province { Abreviation = "QC", Nom = "Québec"},
            };

            List<string> lstChampsInfoPersonnel = new List<string>
            {
                nameof(vendeur.Prenom),
                nameof(vendeur.Nom),
                nameof(vendeur.Rue),
                nameof(vendeur.Ville),
                nameof(vendeur.Pays),
                nameof(vendeur.Province),
                nameof(vendeur.Tel1),
                nameof(vendeur.CodePostal)
            };

            List<string> lstChampsSectionVendeur = new List<string>
            {
                nameof(vendeur.NomAffaires),
                nameof(vendeur.PoidsMaxLivraison),
                nameof(vendeur.Taxes),
                nameof(vendeur.LivraisonGratuite)
            };

            ViewBag.ListeProvinces = new SelectList(lstProvinces, "Abreviation", "Nom");

            vendeurDao = new VendeurDao(vendeur.NoVendeur);

            PPVendeurs vendeurOriginel = vendeurDao.rechecheVendeurParNo(vendeur.NoVendeur);

            //Retire les validations selon la section appelé
            if (string.Equals(strProvenence, "informationpersonnel", StringComparison.OrdinalIgnoreCase))
            {
                lstChampsSectionVendeur.ForEach(x => ModelState[x].Errors.Clear());
            }
            else if (string.Equals(strProvenence, "modificationmdp", StringComparison.OrdinalIgnoreCase))
            {
                lstChampsInfoPersonnel.ForEach(x => ModelState[x].Errors.Clear());
                lstChampsSectionVendeur.ForEach(x => ModelState[x].Errors.Clear());
            }
            else if (string.Equals(strProvenence, "informationVendeur", StringComparison.OrdinalIgnoreCase))
            {
                lstChampsInfoPersonnel.ForEach(x => ModelState[x].Errors.Clear());
            }
            else
            {
                lstChampsInfoPersonnel.ForEach(x => ModelState[x].Errors.Clear());
                lstChampsSectionVendeur.ForEach(x => ModelState[x].Errors.Clear());
            }

            ModelState[nameof(vendeur.MotDePasse)].Errors.Clear();


            if (ModelState.IsValid)
            {
                
                if (string.Equals(strProvenence, "informationpersonnel", StringComparison.OrdinalIgnoreCase))
                {
                    vendeurDao.modifierProfilInformationPersonnel(vendeur);
                    ViewBag.uneErreur = "succes";
                }
                else if (string.Equals(strProvenence, "modificationmdp", StringComparison.OrdinalIgnoreCase))
                {
                    String strAncientMDP = Request["tbAncienMdp"];
                    String strNouveauMDP = Request["tbNouveauMdp"];
                    String strConfirmationMDP = Request["tbConfirmationMdp"];

                    String strMessageErreurVide = "Le champs doit être rempli!";
                    bool booValide = true;
                    if (string.IsNullOrWhiteSpace(strAncientMDP))
                    {
                        ViewBag.MessageErreurAncient = strMessageErreurVide;
                        booValide = false;
                    }

                    if (string.IsNullOrWhiteSpace(strNouveauMDP))
                    {
                        ViewBag.MessageErreurNouveau = strMessageErreurVide;
                        booValide = false;
                    }

                    if (string.IsNullOrWhiteSpace(strConfirmationMDP))
                    {
                        ViewBag.MessageErreurConfirmation = strMessageErreurVide;
                        booValide = false;
                    }

                    //Tous les champs ne sont pas vide
                    if (booValide)
                    {
                        //Valide que le mot de passe est bien l'ancien mdp.
                        if (vendeurOriginel.MotDePasse.Equals(strAncientMDP))
                        {
                            //Valide que le nouveau mdp est identique a celui de confirmation
                            if (strNouveauMDP.Equals(strConfirmationMDP))
                            {
                                vendeurDao.modifierProfilMDP(strNouveauMDP);
                                ViewBag.uneErreur = "succes";
                            }
                            else
                            {
                                ViewBag.MessageErreurConfirmation = "La confirmation doit être identique au nouveau mot de passe!";
                                ViewBag.uneErreur = "echec";
                            }
                        }
                        else
                        {
                            ViewBag.MessageErreurNouveau = "Le nouveau mot de passe doit être différent de celui actuel";
                            ViewBag.uneErreur = "echec";
                        }
                    }
                    else
                    {
                        ViewBag.uneErreur = "echec";
                    }
                }
                else if (string.Equals(strProvenence, "informationVendeur", StringComparison.OrdinalIgnoreCase))
                {
                    vendeurDao.modifierProfilSpecificVendeur(vendeur);
                    ViewBag.uneErreur = "succes";
                }
                else
                {
                    if (Request.Files.Count > 0)
                    {
                        HttpPostedFileBase fichier = Request.Files[0];
                        string strNouveauNomFichier = "";

                        if (fichier != null && fichier.ContentLength > 0)
                        {
                            try
                            {
                                if (fichier.ContentType == "image/jpeg" || fichier.ContentType == "image/jpg" ||
                                    fichier.ContentType == "image/pjep" || fichier.ContentType == "image/gif" ||
                                    fichier.ContentType == "image/x-png" || fichier.ContentType == "image/png")
                                {
                                    //Le nom sera l'adresse courriel du vendeur
                                    string path = Path.Combine(Server.MapPath("~/Content/images"),
                                           vendeurOriginel.AdresseEmail + Path.GetExtension(fichier.FileName));
                                    fichier.SaveAs(path);
                                    strNouveauNomFichier = vendeurOriginel.AdresseEmail + Path.GetExtension(fichier.FileName);
                                }
                                else
                                {
                                    ViewBag.messageErreurFichier = "Seulement les formats jpeg, jpg, pjep, gif, x-png, png sont abceptés!";
                                    ViewBag.uneErreur = "echec";
                                }
                            }
                            catch (Exception ex)
                            {
                                ViewBag.messageErreurFichier = "Le fichier n'a pas été téléversé!";
                                ViewBag.uneErreur = "echec";
                            }
                        }

                        if (ViewBag.uneErreur != "echec")
                        {
                            vendeurDao.modifierProfilConfiguration(police, fond, strNouveauNomFichier);
                            ViewBag.uneErreur = "succes";
                        }
                    } 
                }
            }
            else
            {
                ViewBag.uneErreur = "echec";
            }

            //est-ce qu'il y a la mise a jour des donnée (meme si variable local?)
            return View(vendeurOriginel);
        }



        public ActionResult CommandeDetail(int id)
        {
            Models.DataClasses1DataContext db = new Models.DataClasses1DataContext();
            db.Connection.Open();

            //Aller chercher la commande
            var commandes = (from commande in db.GetTable<PPCommandes>()
                             where commande.NoCommande.Equals(id)
                             select commande
                             ).ToList();

            //Aller chercher les details cette commande
            var detailsCommandes = (from details in db.GetTable<PPDetailsCommandes>()
                                    where details.NoCommande.Equals(commandes.First().NoCommande)
                                    select details
                                    ).ToList();

            Dictionary<PPCommandes, List<PPDetailsCommandes>> dictionnaire = new Dictionary<PPCommandes, List<PPDetailsCommandes>>();
            dictionnaire.Add(commandes.First(), detailsCommandes);

            //Aller chercher l'historique de commande
            var historique = (from histoCommande in db.GetTable<PPHistoriquePaiements>()
                              where histoCommande.NoCommande.Equals(id)
                              select histoCommande
                              ).ToList();

            PPHistoriquePaiements histo = historique.First();
            AccueilVendeurViewModel accueilVendeurViewModel = new AccueilVendeurViewModel(dictionnaire, histo);
            db.Connection.Close();
            return View(accueilVendeurViewModel);
        }

        public ActionResult Livrer(int id)
        {
            Models.DataClasses1DataContext db = new Models.DataClasses1DataContext();
            db.Connection.Open();
            /*
             * Aller changer le statut de commande du id passé en paramètre pour enlever la commande
             * de la liste des commandes non traités 
            */

            var commandes = (from com in db.GetTable<PPCommandes>()
                             where com.NoCommande.Equals(id)
                             select com
                            );

            //Changer le status de la commande 
            commandes.First().Statut = 'L';

            //Changer la valeur dans la base de données
            try
            {
                db.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            //Model page d'accueil vendeur
            long noVendeur = (Session["vendeurObj"] as PPVendeurs).NoVendeur;

            Dictionary<PPCommandes, List<PPDetailsCommandes>> lstDetailsProduitsCommandes = new Dictionary<PPCommandes, List<PPDetailsCommandes>>();
            //Aller chercher les commandes non traités
            var commandesNonTraite = (from commande in db.GetTable<PPCommandes>()
                                      where commande.NoVendeur.Equals(noVendeur) && commande.Statut.Equals('N')
                                      select commande
                                      ).ToList();

            foreach (var commande in commandesNonTraite)
            {
                List<PPDetailsCommandes> lstDetailCommandes = new List<PPDetailsCommandes>();
                var query = (from detailsCommande in db.PPDetailsCommandes
                             where detailsCommande.NoCommande.Equals(commande.NoCommande)
                             select detailsCommande
                             ).ToList();
                lstDetailCommandes = query;
                lstDetailsProduitsCommandes.Add(commande, lstDetailCommandes);
            }

            //Aller chercher les paniers du vendeur
            var paniers = (from panier in db.GetTable<PPArticlesEnPanier>()
                           where panier.NoVendeur.Equals(noVendeur)
                           group panier by panier.PPClients
                           );

         //TODO : Aller chercher le nombres de visites quotidienne
         var visites = (from vendeurClient in db.GetTable<PPVendeursClients>()
                        where vendeurClient.NoVendeur.Equals((Session["vendeurObj"] as PPVendeurs).NoVendeur)
                        select vendeurClient
                        ).ToList();

         //Aller chercher les clients actifs, potentiel et visiteurs du profil connecté.
         int nbClientsActif = 0;
         int nbClientsPotentiel = 0;
         int nbClientsVisiteurs = 0;

         var clients = (from c in db.GetTable<PPClients>()
                        select c
                        ).ToList();

         foreach (var client in clients)
         {
            var dejaCommande = (from commande in db.GetTable<PPCommandes>()
                                where (commande.NoClient.Equals(client.NoClient)) &&
                                (commande.NoVendeur.Equals(noVendeur))
                                select commande
                         ).ToList();
            var possedePanier = (from panier in db.GetTable<PPArticlesEnPanier>()
                                 where (panier.NoClient.Equals(client.NoClient)) && (panier.NoVendeur.Equals(noVendeur))
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

         //Créer un object AccueilVendeurViewModel afin de l'envoyer a ma vue
         AccueilVendeurViewModel model = new AccueilVendeurViewModel(lstDetailsProduitsCommandes, paniers, visites.Count());
         model.lstStatsClients = lstStats;

         db.Connection.Close();

         return View("AccueilVendeur", model);
        }

        public bool DateVentePrixVenteValide(decimal? prixVente, DateTime? dateVente)
        {
            bool retour = false;
            if ((dateVente.Equals(null)) && (prixVente.Equals(null)))
            {
                retour = true;
            }
            else if (!(dateVente.Equals(null)) && !(prixVente.Equals(null)))
            {
                retour = true;
            }

            return retour;
        }

        public ActionResult PanierDetailVendeur(int id)
        {
            Models.DataClasses1DataContext db = new Models.DataClasses1DataContext();
            db.Connection.Open();
            //requête pour aller chercher les produits à l'aide d'un vendeur
            List<PPArticlesEnPanier> items = (from panier in db.GetTable<Models.PPArticlesEnPanier>()
                                              where panier.NoClient.Equals(id) && panier.NoVendeur.Equals((Session["vendeurObj"] as PPVendeurs).NoVendeur)
                                              select panier).ToList();
            db.Connection.Close();
            return View(items);
        }

        [ChildActionOnly]
        public ActionResult InformationPersonnel()
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
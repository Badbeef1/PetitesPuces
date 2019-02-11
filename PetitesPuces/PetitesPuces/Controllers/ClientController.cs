using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PetitesPuces.Models;
using PagedList;
using System.Transactions;
using System.Globalization;

namespace PetitesPuces.Controllers
{
    public class ClientController : Controller
    {
        DataClasses1DataContext contextPP = new DataClasses1DataContext();
        ClientDao clientDao;

        public ActionResult Index()
        {
            String noClient = ((PPClients)Session["clientObj"]).NoClient.ToString();

            //long noClient = ((Models.PPClients)Session["clientObj"]).NoClient;
            /* Compare data with Database */
            Models.DataClasses1DataContext db = new Models.DataClasses1DataContext();
            db.Connection.Open();
            
            //Requête qui va permettre d'aller chercher les paniers du client
            var paniers = from panier in db.GetTable<Models.PPArticlesEnPanier>()
                          where panier.NoClient.Equals(noClient)
                          group panier by panier.PPVendeurs;

            db.Connection.Close();

            return View("AccueilPanier", paniers);
        }


        public ActionResult AccueilClient()
        {
            List<Models.EntrepriseCategorie> lstEntreCate = new List<Models.EntrepriseCategorie>();
            String noClient = ((PPClients)Session["clientObj"]).NoClient.ToString();

            //long noClient = ((Models.PPClients)Session["clientObj"]).NoClient;
            /* Compare data with Database */
            Models.DataClasses1DataContext db = new Models.DataClasses1DataContext();
            db.Connection.Open();

            //Requête qui va permettre d'aller chercher les paniers du client
            var paniers = from panier in db.GetTable<Models.PPArticlesEnPanier>()
                          where panier.NoClient.Equals(noClient)
                          group panier by panier.PPVendeurs;
            var toutesCategories = (from cat in db.GetTable<Models.PPCategories>()
                                    select cat
                                 );
            foreach (var cat in toutesCategories)
            {
                List<PPVendeurs> lstVendeurs = new List<PPVendeurs>();
                var query = (from prod in db.GetTable<Models.PPProduits>()
                             where prod.NoCategorie.Equals(cat.NoCategorie)
                             select prod
                             );
                foreach (var item in query)
                {
                    if (!lstVendeurs.Contains(item.PPVendeurs))
                    {
                        lstVendeurs.Add(item.PPVendeurs);
                    }
                }
                lstEntreCate.Add(new Models.EntrepriseCategorie(cat, lstVendeurs));
            }

            AccueilClientViewModel items = new AccueilClientViewModel(lstEntreCate, paniers);

            db.Connection.Close();

            return View(items);
        }

        // GET: Client
        public ActionResult SaisieCommande(List<PPArticlesEnPanier> lst)
        {
            List<Province> lstProvinces = new List<Province>
            {
                new Province { Abreviation = "NB", Nom = "Nouveau-Brunswick"},
                new Province { Abreviation = "ON", Nom = "Ontario"},
                new Province { Abreviation = "QC", Nom = "Québec"},
            };

            ViewBag.ListeProvinces = new SelectList(lstProvinces, "Abreviation", "Nom");


            List < PPArticlesEnPanier > items = new List<PPArticlesEnPanier>();
            items = (from panier in contextPP.GetTable<Models.PPArticlesEnPanier>()
                    where panier.NoClient.Equals(lst[0].NoClient) && panier.NoVendeur.Equals(lst[0].NoVendeur)
                    select panier).ToList();

            var client = from unClient in contextPP.GetTable<PPClients>()
                               where unClient.NoClient.Equals(items[0].PPClients.NoClient)
                               select unClient;

            PPClients clientSaisie = new PPClients
            {
                NoClient = client.First().NoClient,
                AdresseEmail = client.First().AdresseEmail,
                Nom = client.First().Nom,
                Prenom = client.First().Prenom,
                Rue = client.First().Rue,
                Ville = client.First().Ville,
                Province = client.First().Province,
                CodePostal = client.First().CodePostal,
                Pays = client.First().Pays,
                Tel1 = client.First().Tel1,
                Tel2 = client.First().Tel2
            };

            SaisieCommandeViewModel sViewModel = new SaisieCommandeViewModel
            {
                lstArticlePanier = items,
                client = clientSaisie,
                vendeur = items[0].PPVendeurs
            };

            return View(sViewModel);
        }


        //Panier Détaillé du client
        public ActionResult PanierDetail(int id)
        {
            Models.DataClasses1DataContext db = new Models.DataClasses1DataContext();
            db.Connection.Open();
            //requête pour aller chercher les produits à l'aide d'un vendeur
            List<PPArticlesEnPanier> items = (from panier in db.GetTable<Models.PPArticlesEnPanier>()
                                              where panier.NoClient.Equals(((PPClients)Session["clientObj"]).NoClient) && panier.NoVendeur.Equals(id)
                                              select panier).ToList();

            return View(items);
        }

        /// <summary>
        /// Cette fonction du controleur permet d'actualiser 
        /// la quantité d'item d'un produit dans le panier.
        /// </summary>
        /// <param name="noPanier"></param>
        /// <param name="quantite"></param>
        /// <returns></returns>

        public ActionResult UpdatePanier(int noPanier, int quantite)
        {
            Models.DataClasses1DataContext db = new Models.DataClasses1DataContext();
            db.Connection.Open();
            String noClient = ((PPClients)Session["clientObj"]).NoClient.ToString();
            int noVendeur;
            //long noClient = ((Models.PPClients)Session["clientObj"]).NoClient;

            var articlesPanier = (from articlePanier in db.GetTable<PPArticlesEnPanier>()
                                  where articlePanier.NoPanier.Equals(noPanier)
                                  select articlePanier
                                  );

            noVendeur = (int)articlesPanier.First().NoVendeur;
            articlesPanier.First().NbItems = (short)quantite;

            // Submit the changes to the database.
            try
            {
                db.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            //Requête qui va permettre d'aller chercher les paniers du client
            List<PPArticlesEnPanier> items = (from panier in db.GetTable<Models.PPArticlesEnPanier>()
                                              where panier.NoClient.Equals(((PPClients)Session["clientObj"]).NoClient.ToString()) && panier.NoVendeur.Equals(noVendeur)
                                              select panier).ToList();

            db.Connection.Close();
            return PartialView("Client/Panier", items);
        }

        public ActionResult UpdatePanierSaisieCommande(int noPanier, int quantite)
        {
            if (ModelState.IsValid)
            {
                String noClient = ((PPClients)Session["clientObj"]).NoClient.ToString();
                long noVendeur = 0;
                try
                {
                    var articlesPanier = from articlePanier in contextPP.GetTable<PPArticlesEnPanier>()
                                         where articlePanier.NoPanier.Equals(noPanier)
                                         select articlePanier;

                    noVendeur = (long)articlesPanier.First().NoVendeur;
                    articlesPanier.First().NbItems = (short)quantite;
                    contextPP.SubmitChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    ViewData["errorBD"] = "Une erreur s'est produite au changement de quantité";
                }
                //Requête qui va permettre d'aller chercher les paniers du client
                List<PPArticlesEnPanier> items = (from panier in contextPP.GetTable<Models.PPArticlesEnPanier>()
                                                  where panier.NoClient.Equals(noClient) && panier.NoVendeur.Equals(noVendeur)
                                                  select panier).ToList();
                
                if (items.Count > 0)
                {
                    SaisieCommandeViewModel sViewModel = new SaisieCommandeViewModel
                    {
                        client = contextPP.GetTable<PPClients>().Where(m => m.NoClient.Equals(noClient)).First(),
                        vendeur = contextPP.GetTable<PPVendeurs>().Where(m => m.NoVendeur.Equals(noVendeur)).First(),
                        lstArticlePanier = items
                    };
                    
                    return PartialView("SaisieCommande", sViewModel);
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }
            }
            else
            {
                // Est-ce que je veux vraiment retourner ceci???
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

        }
        /// <summary>
        /// Permet de supprimer le produit passé en paramètre
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult SupprimerProduit(int id)
        {
            Models.DataClasses1DataContext db = new Models.DataClasses1DataContext();
            db.Connection.Open();
            long noVendeur;
            String noClient = ((PPClients)Session["clientObj"]).NoClient.ToString();
            //long noClient = ((Models.PPClients)Session["clientObj"]).NoClient;

            //Aller chercher le panier à supprimer
            var query = (from articlePanier in db.GetTable<Models.PPArticlesEnPanier>()
                         where articlePanier.NoPanier.Equals(id)
                         select articlePanier
                         );
            noVendeur = (long)query.First().NoVendeur;

            db.PPArticlesEnPanier.DeleteOnSubmit(query.First());

            try
            {
                db.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            //Requête qui va permettre d'aller chercher les paniers du client
            List<PPArticlesEnPanier> items = (from panier in db.GetTable<Models.PPArticlesEnPanier>()
                                              where panier.NoClient.Equals(((PPClients)Session["clientObj"]).NoClient.ToString()) && panier.NoVendeur.Equals(noVendeur)
                                              select panier).ToList();
            if (items.Count() == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            db.Connection.Close();
            return PartialView("Client/Panier", items);
        }

        public ActionResult SupprimerProduitSaisieCommande(int id)
        {
            if (ModelState.IsValid)
            {
                long noVendeur = 0;
                long noClient = ((PPClients)Session["clientObj"]).NoClient;
                try
                {

                    //Aller chercher le panier à supprimer
                    var query = (from articlePanier in contextPP.GetTable<Models.PPArticlesEnPanier>()
                                 where articlePanier.NoPanier.Equals(id)
                                 select articlePanier
                                 );

                    noVendeur = (long)query.First().NoVendeur;

                    contextPP.PPArticlesEnPanier.DeleteOnSubmit(query.First());

                    contextPP.SubmitChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                //Requête qui va permettre d'aller chercher les paniers du client
                List<PPArticlesEnPanier> items = (from panier in contextPP.GetTable<Models.PPArticlesEnPanier>()
                                                  where panier.NoClient.Equals(noClient) && panier.NoVendeur.Equals(noVendeur)
                                                  select panier).ToList();
                if (items.Count > 0)
                {
                    SaisieCommandeViewModel sViewModel = new SaisieCommandeViewModel
                    {
                        client = contextPP.GetTable<PPClients>().Where(m => m.NoClient.Equals(noClient)).First(),
                        vendeur = contextPP.GetTable<PPVendeurs>().Where(m => m.NoVendeur.Equals(noVendeur)).First(),
                        lstArticlePanier = items
                    };

                    return PartialView("SaisieCommande", sViewModel);
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }
            }
            else
            {
                // Est-ce que je veux vraiment retourner ceci???
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

        }

        public ActionResult GestionProfilClient()
        {
            //HttpContext.User.Identity.Name
            String strAdresseCourrielClient = ((PPClients)Session["clientObj"]).AdresseEmail;

            clientDao = new ClientDao((Session["clientObj"] as PPClients).NoClient);

            PPClients leClient = clientDao.rechecheClientParCourriel(strAdresseCourrielClient);

            List<Province> lstProvinces = new List<Province>
            {
                new Province { Abreviation = "NB", Nom = "Nouveau-Brunswick"},
                new Province { Abreviation = "ON", Nom = "Ontario"},
                new Province { Abreviation = "QC", Nom = "Québec"},
            };

            ViewBag.ListeProvinces = new SelectList(lstProvinces, "Abreviation", "Nom");
            return View(leClient);
        }

        //Vue partiel Information personnel
        [ChildActionOnly]
        public ActionResult InformationPersonnel()
        {
            return PartialView();
        }

        //Vue partiel modification du mot de passe
        [ChildActionOnly]
        public ActionResult ModificationMDP() => PartialView();

        [HttpPost]
        public ActionResult GestionProfilClient(PPClients client, String strProvenence)
        {
            List<Province> lstProvinces = new List<Province>
            {
                new Province { Abreviation = "NB", Nom = "Nouveau-Brunswick"},
                new Province { Abreviation = "ON", Nom = "Ontario"},
                new Province { Abreviation = "QC", Nom = "Québec"},
            };

            ViewBag.ListeProvinces = new SelectList(lstProvinces, "Abreviation", "Nom");
            clientDao = new ClientDao(client.NoClient);

            PPClients clientOriginal = clientDao.rechecheClientParNo(client.NoClient);

            if (ModelState.IsValid)
            {
                if (string.Equals(strProvenence, "informationpersonnel", StringComparison.OrdinalIgnoreCase))
                {
                    clientDao.modifierProfilInformationPersonnel(client);
                }
                else
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
                        if (clientOriginal.MotDePasse.Equals(strAncientMDP))
                        {
                            //Valide que le nouveau mdp est identique a celui de confirmation
                            if (strNouveauMDP.Equals(strConfirmationMDP))
                            {
                                clientDao.modifierProfilMDP(strNouveauMDP);
                            }
                            else
                            {
                                ViewBag.MessageErreurConfirmation = "La confirmation doit être identique au nouveau mot de passe!";
                            }
                        }
                        else
                        {
                            ViewBag.MessageErreurNouveau = "Le nouveau mot de passe doit être différent de celui actuel";
                        }
                    }

                    TempData["msgConfirmation"] = clientOriginal.MotDePasse != strAncientMDP ? "succes" : "echec";
                }
            }

            return View(clientOriginal);
        }

        // Tout les produits (15 par pages).
        public ActionResult Cataloguesss()
        {
            ViewModels.CatalogueViewModel catVM = new ViewModels.CatalogueViewModel
            {
                lstCategorie = contextPP.PPCategories.ToList(),
                lstproduits = contextPP.PPProduits.ToList()
            };

            return View(catVM);
        }


        //Les produits avec une quantité défini par page

        public ActionResult Catalogue(string tri, string categorie, string vendeur, string recherche , string recherche2, int? typeRech, int? page, int pageDimension = 5,int noPage = 1 )
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
            System.Diagnostics.Debug.WriteLine("tri1: " + (ViewBag.TriNum as String)+ " Tri2: " + (ViewBag.TriCat as String) + " Tri3: " + (ViewBag.TriDate as String));

            List<PPProduits> lstDesProduits = contextPP.PPProduits.ToList();

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

            PPVendeurs objVendeurCatalogue = null;

            //Si affichage d'un vendeur en particulier
            if (!String.IsNullOrWhiteSpace(vendeur) && 
                ((objVendeurCatalogue = contextPP.PPVendeurs.FirstOrDefault(predicate: ven => ven.NomAffaires.ToLower() == vendeur.ToLower())) != null))
            {
                lstDesProduits = lstDesProduits
                    .Where(pro => String.Equals(pro.PPVendeurs.NomAffaires, vendeur, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            //Si affichage d'une catégorie en particulier
            else if (!String.IsNullOrWhiteSpace(categorie) && 
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
                                .FindAll( pro => pro.DateCreation.Value >= dtDebut && pro.DateCreation.Value <= dtFin);
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

            //Vendeur par catégorie
            Dictionary<string,List<string>> dicCateAvecVendeur = contextPP.PPProduits
                .GroupBy(pro => pro.PPCategories.Description.ToString(), pro => pro.PPVendeurs.NomAffaires.ToString())
                .Distinct()
                .ToDictionary(pro => pro.Key, pro => pro.Distinct().ToList(), StringComparer.OrdinalIgnoreCase);

            //Liste des vendeurs
            List<PPVendeurs> lstVendeur = contextPP.PPVendeurs
                .Where(predicate: ven => ven.PPProduits.Count() > 0)
                .OrderBy(ven => ven.NomAffaires)
                .ToList(); 

            ViewModels.CatalogueViewModel catVM = new ViewModels.CatalogueViewModel
            {
                lstCategorie = contextPP.PPCategories.ToList(),
                strTri = tri,
                strCategorie = categorie,
                recherche = recherche,
                recherche2 = recherche2,
                pageDimension = pageDimension,
                intNoPage = noPage,
                iplProduits = lstDesProduits.ToPagedList(intNumeroPage, pageDimension),
                dicVendeur = dicCateAvecVendeur,
                vendeur = vendeur,
                lstVendeur = lstVendeur,
                vendeurCatalogue = objVendeurCatalogue
            };

            if (typeRech.HasValue)
            {
                catVM.typeRech = typeRech.Value;
            }

            ViewBag.ListeNbItems = new SelectList(lstSelectionNbItems, pageDimension);
            
            return View(catVM);
        }



        public ActionResult RecevoirPrixLivraison(string poids, string panier, string tarif)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    List<Province> lstProvinces = new List<Province>
                        {
                            new Province { Abreviation = "NB", Nom = "Nouveau-Brunswick"},
                            new Province { Abreviation = "ON", Nom = "Ontario"},
                            new Province { Abreviation = "QC", Nom = "Québec"},
                        };

                    ViewBag.ListeProvinces = new SelectList(lstProvinces, "Abreviation", "Nom");


                    ViewData["cbChecked"] = tarif;
                    Decimal dclPoids = Decimal.Parse(poids.Replace(".", ","));

                    // On trouve le code poids
                    var poidsLivraison = from pLiv in contextPP.GetTable<PPTypesPoids>()
                                         where pLiv.PoidsMin <= dclPoids && pLiv.PoidsMax >= dclPoids
                                         orderby pLiv.CodePoids
                                         select pLiv;

                    // On checher la liste des livraisons offertes pour ce code poids
                    var montantLivraison = from monLivraison in contextPP.GetTable<PPPoidsLivraisons>()
                                           where monLivraison.CodePoids.Equals(poidsLivraison.First().CodePoids)
                                           select monLivraison;

                    // On envoie le résultats des type de livraison
                    ViewData["MontantLivraison"] = montantLivraison.ToList();

                    var typeLivraison = from typeLiv in contextPP.GetTable<PPTypesLivraison>()
                                        select typeLiv;

                    ViewData["TypeLivraison"] = typeLivraison.ToList();

                    var numPourPanierList = from ppArtEnPan in contextPP.GetTable<PPArticlesEnPanier>()
                                            where ppArtEnPan.NoPanier.ToString().Equals(panier)
                                            select new { ppArtEnPan.PPClients, ppArtEnPan.PPVendeurs };

                    var panierList = from ppArtEnPan in contextPP.GetTable<PPArticlesEnPanier>()
                                     where ppArtEnPan.NoClient.Equals(numPourPanierList.First().PPClients.NoClient)
                                     && ppArtEnPan.NoVendeur.Equals(numPourPanierList.First().PPVendeurs.NoVendeur)
                                     select ppArtEnPan;
                    if(panierList.Count() > 0)
                    {
                        SaisieCommandeViewModel sViewModel = new SaisieCommandeViewModel
                        {
                            lstArticlePanier = panierList.ToList(),
                            client = panierList.ToList()[0].PPClients,
                            vendeur = panierList.ToList()[0].PPVendeurs
                        };

                        return View("SaisieCommande", sViewModel);
                    }
                    else
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }
                
            }
            else
            {
                // Est-ce que je veux vraiment retourner ceci???
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
        }


        // GET: ProduitDetail
        public ActionResult ProduitDetaille(long numero)
        {

            PPProduits produit = contextPP.PPProduits.FirstOrDefault(pro => pro.NoProduit == numero);

            return View(produit);
        }

        public ActionResult InfoClientSaisieCommande(PPClients client)
        {
            return View(client);
        }
        public ActionResult test() => View();

        public ActionResult ConfirmationTransaction() => View();

        [HttpPost]
        public ActionResult ConfirmationTransaction(string NoAutorisation, string DateAutorisation, string FraisMarchand, string InfoSuppl)
        {

            ViewData["CheckPoint"] = "-A";
            List<PPDetailsCommandes> lstDetCommandeEnCours = new List<PPDetailsCommandes>();

            if (NoAutorisation != null && NoAutorisation.Trim() != "")
            {
                TempData["NoAutorisation"] = int.Parse(NoAutorisation); 
            }
            if (DateAutorisation != null && DateAutorisation.Trim() != "")
            {
                TempData["DateAutorisation"] = DateAutorisation;
            }
            if (FraisMarchand != null && FraisMarchand.Trim() != "")
            {
                TempData["FraisMarchand"] = FraisMarchand;
            }
            if (InfoSuppl != null && InfoSuppl.Trim() != "N/A")
            {
                TempData["InfoSuppl"] = InfoSuppl;
                    var panierCommander = from unPanier in contextPP.GetTable<PPArticlesEnPanier>()
                                          where unPanier.NoClient.Equals(InfoSuppl.Split('-')[0]) &&
                                          unPanier.NoVendeur.Equals(InfoSuppl.Split('-')[1])
                                          select unPanier;
                
                // Poids de ma livraison
                var poidsLivraison = from pLiv in contextPP.GetTable<PPTypesPoids>()
                                         where pLiv.PoidsMin <= Decimal.Parse(InfoSuppl.Split('-')[2].Replace(".", ",")) && pLiv.PoidsMax >= Decimal.Parse(InfoSuppl.Split('-')[2].Replace(".", ","))
                                         orderby pLiv.CodePoids
                                         select pLiv;
                
                // Type de livraison
                var typeLivraison = from typeLiv in contextPP.GetTable<PPPoidsLivraisons>()
                                        where typeLiv.CodePoids.Equals(poidsLivraison.First().CodePoids) &&
                                        typeLiv.Tarif.Equals(Decimal.Parse(InfoSuppl.Split('-')[3])) select typeLiv;

                // Trouver prochain numéro de commande
                var numCommande = from commandeTrouver in contextPP.GetTable<PPCommandes>()
                                  orderby commandeTrouver.NoCommande descending
                                  select commandeTrouver;
                long maxCommande = numCommande.First().NoCommande + 1;
                
                using (var trans = new TransactionScope())
                    {
                    try
                    {
                        Char c = new Char();
                        c = 'N';
                        // Création de la commande
                        PPCommandes commande = new PPCommandes
                        {
                            NoCommande = maxCommande,
                            NoClient = panierCommander.First().NoClient,
                            NoVendeur = panierCommander.First().NoVendeur,
                            DateCommande = DateTime.ParseExact(DateAutorisation,"yyyy-MM-dd",CultureInfo.InvariantCulture),
                            CoutLivraison = Decimal.Parse(InfoSuppl.Split('-')[3].Replace(".", ",")),
                            TypeLivraison = typeLivraison.First().CodeLivraison,
                            MontantTotAvantTaxes = Decimal.Parse(InfoSuppl.Split('-')[4].Replace(".", ",")),
                            TPS = Decimal.Parse(InfoSuppl.Split('-')[5].Replace(".", ",")),
                            TVQ = Decimal.Parse(InfoSuppl.Split('-')[6].Replace(".", ",")),
                            PoidsTotal = Decimal.Parse(InfoSuppl.Split('-')[2].Replace(".", ",")),
                            Statut = c,
                            NoAutorisation = NoAutorisation
                        };
                        contextPP.GetTable<PPCommandes>().InsertOnSubmit(commande);
                        contextPP.SubmitChanges();
                        
                        // Création des détails de commande

                        var numDetComm = from detTrouver in contextPP.GetTable<PPDetailsCommandes>()
                                         orderby detTrouver.NoDetailCommandes descending
                                         select detTrouver;
                        long maxDetComm = numDetComm.First().NoDetailCommandes + 1;
                        foreach (PPArticlesEnPanier artPan in panierCommander)
                        {
                            // Trouver prochain numéro de commande

                            decimal prix = 0;
                            var produit = from unProduit in contextPP.GetTable<PPProduits>()
                                          where unProduit.NoProduit.Equals(artPan.NoProduit)
                                          select unProduit;
                            if(DateTime.Now <= produit.First().DateVente)
                            {
                                prix = (decimal) produit.First().PrixVente;
                            }
                            PPDetailsCommandes detCommande = new PPDetailsCommandes
                            {
                                NoDetailCommandes = maxDetComm,
                                NoCommande = commande.NoCommande,
                                NoProduit = artPan.NoProduit,
                                PrixVente = prix,
                                Quantité = artPan.NbItems
                            };
                            lstDetCommandeEnCours.Add(detCommande);
                            maxDetComm++;
                        }
                        contextPP.GetTable<PPDetailsCommandes>().InsertAllOnSubmit(lstDetCommandeEnCours);
                        contextPP.SubmitChanges();
                        
                        // Vider le panier
                        contextPP.GetTable<PPArticlesEnPanier>().DeleteAllOnSubmit(panierCommander);
                        contextPP.SubmitChanges();

                        // Mettre à jour nbItems
                        foreach(PPDetailsCommandes detComm in lstDetCommandeEnCours)
                        {
                            var produitModifier = from unProduit in contextPP.GetTable<PPProduits>()
                                                  where unProduit.NoProduit.Equals(detComm.NoProduit)
                                                  select unProduit;
                            PPProduits prodModifier = produitModifier.First();
                            prodModifier.NombreItems -= detComm.Quantité;
                            contextPP.SubmitChanges();
                            
                        }
                        
                        // On cherche le vendeur
                        var vendeur = from unVendeur in contextPP.GetTable<PPVendeurs>()
                                      where unVendeur.NoVendeur.Equals(commande.NoVendeur)
                                      select unVendeur;

                        // Mettre à jour l'historique de paiement
                        PPHistoriquePaiements histoPaiement = new PPHistoriquePaiements
                        {
                            MontantVenteAvantLivraison = commande.MontantTotAvantTaxes,
                            NoVendeur = commande.NoVendeur,
                            NoClient = commande.NoClient,
                            NoCommande = commande.NoCommande,
                            DateVente = commande.DateCommande,
                            NoAutorisation = commande.NoAutorisation,
                            FraisLesi = Decimal.Parse(FraisMarchand.Replace(".", ",")),
                            Redevance = commande.MontantTotAvantTaxes*(vendeur.First().Pourcentage/100),
                            FraisLivraison = commande.CoutLivraison,
                            FraisTPS = commande.TPS,
                            FraisTVQ = commande.TVQ
                        };
                        
                        contextPP.GetTable<PPHistoriquePaiements>().InsertOnSubmit(histoPaiement);
                        contextPP.SubmitChanges();
                        trans.Complete();
                    }
                    catch (Exception e)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                    }

                }
            }
            return View();
        }
    }
}
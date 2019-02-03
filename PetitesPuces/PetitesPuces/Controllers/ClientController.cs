using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PetitesPuces.Models;

namespace PetitesPuces.Controllers
{
    public class ClientController : Controller
    {
        DataClasses1DataContext contextPP = new DataClasses1DataContext();
        ClientDao clientDao;

        public ActionResult Index()
        {
            String noClient = "10000";

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
            String noClient = "10000";

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
            Models.DataClasses1DataContext db = new Models.DataClasses1DataContext();
            List<PPArticlesEnPanier> items = new List<PPArticlesEnPanier>();
            foreach (PPArticlesEnPanier pppArtPan in lst)
            {
                items = ((from panier in db.GetTable<Models.PPArticlesEnPanier>()
                           where panier.NoClient.Equals(pppArtPan.NoClient) && panier.NoVendeur.Equals(pppArtPan.NoVendeur)
                           select panier).ToList());
            } 
            return View(items);
        }


        //Panier Détaillé du client
        public ActionResult PanierDetail(int id)
        {
            Models.DataClasses1DataContext db = new Models.DataClasses1DataContext();
            db.Connection.Open();
            //requête pour aller chercher les produits à l'aide d'un vendeur
            List<PPArticlesEnPanier> items = (from panier in db.GetTable<Models.PPArticlesEnPanier>()
                                              where panier.NoClient.Equals(10000) && panier.NoVendeur.Equals(id)
                                              select panier).ToList();

            return View(items);
        }


        /* Fonction qui servait a rediriger par le form à la page SaisieCommande 
         * Je l'ai enlever pcq ça servait à rien en fait (NJ 2019-02-02)
         * [HttpPost]
        public void PanierDetail(long id, List<PPArticlesEnPanier> model)
        {
            
            Models.DataClasses1DataContext db = new Models.DataClasses1DataContext();
            db.Connection.Open();
            long noClient = 10000;
            //long noClient = ((Models.PPClients)Session["clientObj"]).NoClient;

            //Requête qui va permettre d'aller chercher les paniers du client
            List<PPArticlesEnPanier> items = (from panier in db.GetTable<PPArticlesEnPanier>()
                                              where panier.NoClient.Equals(noClient) && panier.NoVendeur.Equals(id)
                                              select panier).ToList();
            db.Connection.Close();
            
            RedirectToAction("SaisieCommande", items);
        }*/

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
            String noClient = "10000";
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
                                              where panier.NoClient.Equals(10000) && panier.NoVendeur.Equals(noVendeur)
                                              select panier).ToList();

            db.Connection.Close();
            return PartialView("Client/Panier", items);
        }

        public ActionResult UpdatePanierSaisieCommande(int noPanier, int quantite)
        {
            Models.DataClasses1DataContext db = new Models.DataClasses1DataContext();
            db.Connection.Open();
            String noClient = "10000";
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
                                              where panier.NoClient.Equals(10000) && panier.NoVendeur.Equals(noVendeur)
                                              select panier).ToList();

            db.Connection.Close();
            return PartialView("SaisieCommande", items);
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
            String noClient = "10000";
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
                                              where panier.NoClient.Equals(10000) && panier.NoVendeur.Equals(noVendeur)
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
            Models.DataClasses1DataContext db = new Models.DataClasses1DataContext();
            db.Connection.Open();
            long noVendeur;
            String noClient = "10000";
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
                                              where panier.NoClient.Equals(10000) && panier.NoVendeur.Equals(noVendeur)
                                              select panier).ToList();
            if (items.Count() == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            db.Connection.Close();
            return View("SaisieCommande", items);
        }

        public ActionResult GestionProfilClient()
        {
            //HttpContext.User.Identity.Name
            String strAdresseCourrielClient = "Client10000@cgodin.qc.ca";

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

            clientDao = new ClientDao();

            PPClients clientOriginal = clientDao.rechecheClientParNo(client.NoClient);

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

        public ActionResult Catalogue(string tri, string categorie, string vendeur,string nbPage = "15",int noPage = 1)
        {
            List<PPProduits> lstDesProduits = contextPP.PPProduits.ToList();

            //Si affichage d'un vendeur en particulier (pas encore tester)
            if (!String.IsNullOrWhiteSpace(vendeur))
            {
                lstDesProduits = lstDesProduits.Where(pro => String.Equals(pro.PPVendeurs.NomAffaires, vendeur, StringComparison.OrdinalIgnoreCase)).ToList();

            }
            
            //Si affichage d'une catégorie en particulier
            if (!String.IsNullOrWhiteSpace(categorie))
            {
                lstDesProduits = lstDesProduits.Where(pro => String.Equals(pro.PPCategories.Description, categorie, StringComparison.OrdinalIgnoreCase)).ToList();
                //ViewBag.infini = "parfait";
            }

            //tri
            switch (tri)
            {
                case "numero":
                    lstDesProduits = lstDesProduits.OrderBy(pro => pro.NoProduit).ToList();
                    break;
                case "!numero":
                    lstDesProduits = lstDesProduits.OrderByDescending(pro => pro.NoProduit).ToList();
                    break;
                case "date":
                    lstDesProduits = lstDesProduits.OrderBy(pro => pro.DateCreation).ToList();
                    break;
                case "!date":
                    lstDesProduits = lstDesProduits.OrderByDescending(pro => pro.DateCreation).ToList();
                    break;
                case "categorie":
                    lstDesProduits = lstDesProduits.OrderBy(pro => pro.PPCategories.Description).ToList();
                    break;
                case "!categorie":
                    lstDesProduits = lstDesProduits.OrderByDescending(pro => pro.PPCategories.Description).ToList();
                    break;
            }

            //Pagination
            List<string> lstSelectionNbItems = new List<string>
            {
                "5","10","15","20","25","tous"
            };
            
            ViewModels.CatalogueViewModel catVM = new ViewModels.CatalogueViewModel
            {
                lstCategorie = contextPP.PPCategories.ToList()
            };

            if (nbPage != lstSelectionNbItems.Last())
            {
                List<List<PPProduits>> lstProdDiv = lstDesProduits.Separe(Convert.ToInt32(nbPage));
                catVM.lstproduits = lstProdDiv[noPage - 1];
            }
            else
            {
                catVM.lstproduits = lstDesProduits;
            }

            //ViewBag.ListeProvinces = new SelectList(lstProvinces, "Abreviation", "Nom");
            ViewBag.ListeNbItems = new SelectList(lstSelectionNbItems, nbPage);




            
            return View(catVM);
        }



        public ActionResult RecevoirPrixLivraison(string poids, string panier)
        {
            Models.DataClasses1DataContext db = new Models.DataClasses1DataContext();
            Decimal dclPoids = Decimal.Parse(poids);
            db.Connection.Open();
            var poidsLivraison = from pLiv in db.GetTable<PPTypesPoids>()
                                  where pLiv.PoidsMin <= dclPoids && pLiv.PoidsMax >= dclPoids
                                 orderby pLiv.CodePoids
                                  select pLiv;

            var numPourPanierList = from ppArtEnPan in db.GetTable<PPArticlesEnPanier>()
                             where ppArtEnPan.NoPanier.ToString().Equals(panier)
                             select new { ppArtEnPan.PPClients, ppArtEnPan.PPVendeurs };

            var panierList = from ppArtEnPan in db.GetTable<PPArticlesEnPanier>()
                             where ppArtEnPan.NoClient.Equals(numPourPanierList.First().PPClients)
                             && ppArtEnPan.NoVendeur.Equals(numPourPanierList.First().PPVendeurs)
                             select ppArtEnPan;

                ViewData["CodePoids"] = poidsLivraison.First().CodePoids;
            return View("SaisieCommande",panierList);
        }


        // GET: ProduitDetail
        public ActionResult ProduitDetaille() => View();

        public ActionResult test() => View();
    }
}
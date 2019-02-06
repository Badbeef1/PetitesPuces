using PetitesPuces.Models;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
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
         int noVendeur = 10;
         Models.DataClasses1DataContext db = new Models.DataClasses1DataContext();
         db.Connection.Open();
         Dictionary<PPCommandes, List<PPDetailsCommandes>> lstDetailsProduitsCommandes = new Dictionary<PPCommandes, List<PPDetailsCommandes>>();
         //Aller chercher les commandes non traités
         var commandesNonTraite = (from commande in db.GetTable<PPCommandes>()
                                   where commande.NoVendeur.Equals(10) && commande.Statut.Equals('N')
                                   select commande
                                   ).ToList();

         foreach(var commande in commandesNonTraite)
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

         //Créer un object AccueilVendeurViewModel afin de l'envoyer a ma vue
         AccueilVendeurViewModel model = new AccueilVendeurViewModel(lstDetailsProduitsCommandes, paniers, 10);

         db.Connection.Close();
         return View(model);
      }

      // GET: Vendeur
      public ActionResult GestionCommande() => View();

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
            if (!String.IsNullOrWhiteSpace(categorie))
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
         return View(vendeurDao.rechecheVendeurParNo((Session["vendeurObj"] as PPVendeurs).NoVendeur));
      }

      [HttpPost]
      public ActionResult GestionProfilVendeur(PPVendeurs vendeur, String strProvenence)
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

         if (string.Equals(strProvenence, "informationpersonnel", StringComparison.OrdinalIgnoreCase))
         {
            lstChampsSectionVendeur.ForEach(x => ModelState[x].Errors.Clear());

            vendeurDao.modifierProfilInformationPersonnel(vendeur);
         }
         else if (string.Equals(strProvenence, "modificationmdp", StringComparison.OrdinalIgnoreCase))
         {
            lstChampsInfoPersonnel.ForEach(x => ModelState[x].Errors.Clear());
            lstChampsSectionVendeur.ForEach(x => ModelState[x].Errors.Clear());

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
         }
         else
         {
            lstChampsInfoPersonnel.ForEach(x => ModelState[x].Errors.Clear());

            vendeurDao.modifierProfilSpecificVendeur(vendeur);
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
         int noVendeur = 10;
         Dictionary<PPCommandes, List<PPDetailsCommandes>> lstDetailsProduitsCommandes = new Dictionary<PPCommandes, List<PPDetailsCommandes>>();
         //Aller chercher les commandes non traités
         var commandesNonTraite = (from commande in db.GetTable<PPCommandes>()
                                   where commande.NoVendeur.Equals(10) && commande.Statut.Equals('N')
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

         //Créer un object AccueilVendeurViewModel afin de l'envoyer a ma vue
         AccueilVendeurViewModel model = new AccueilVendeurViewModel(lstDetailsProduitsCommandes, paniers, 10);


         db.Connection.Close();

         return View("AccueilVendeur",model);
      }

      public ActionResult PanierDetailVendeur(int id)
      {
         Models.DataClasses1DataContext db = new Models.DataClasses1DataContext();
         db.Connection.Open();
         //requête pour aller chercher les produits à l'aide d'un vendeur
         List<PPArticlesEnPanier> items = (from panier in db.GetTable<Models.PPArticlesEnPanier>()
                                           where panier.NoClient.Equals(id) && panier.NoVendeur.Equals(10)
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
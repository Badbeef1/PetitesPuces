using PetitesPuces.Models;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace PetitesPuces.Controllers
{
    public class VendeurController : Controller
    {
        DataClasses1DataContext contextPP = new DataClasses1DataContext();
        VendeurDao vendeurDao;

        public ActionResult Index() => View("AccueilVendeur");

        public ActionResult AccueilVendeur() => View();

        // GET: Vendeur
        public ActionResult GestionCommande() => View();

        //GET: GestionProduit
        public ActionResult GestionProduit() => View();

        //GET: CatalogueVendeur
        public ActionResult CatalogueVendeur() => View();

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
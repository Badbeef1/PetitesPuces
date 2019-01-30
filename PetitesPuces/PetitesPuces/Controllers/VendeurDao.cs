using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PetitesPuces.Models;

namespace PetitesPuces.Controllers
{
    public class VendeurDao : IVendeur
    {
        private DataClasses1DataContext contextPP = new DataClasses1DataContext();
        public PPVendeurs unVendeur { get; }

        public VendeurDao()
        {
            String strAdresseCourrielVendeur = "L.CHAPLEAU@TOTO.COM";

            unVendeur = contextPP.PPVendeurs.FirstOrDefault(vendeur => vendeur.AdresseEmail == strAdresseCourrielVendeur);
        }

        public VendeurDao(long lngNoVendeur)
        {
            unVendeur = contextPP.PPVendeurs.FirstOrDefault(vendeur => vendeur.NoVendeur == lngNoVendeur);
        }

        public void modifierProfilInformationPersonnel(PPVendeurs nouveauVendeur)
        {
            if (!string.IsNullOrWhiteSpace(nouveauVendeur.Prenom))
            {
                unVendeur.Prenom = nouveauVendeur.Prenom;
            }

            if (!string.IsNullOrWhiteSpace(nouveauVendeur.Nom))
            {
                unVendeur.Nom = nouveauVendeur.Nom;
            }

            if (!string.IsNullOrWhiteSpace(nouveauVendeur.Rue))
            {
                unVendeur.Rue = nouveauVendeur.Rue;
            }

            if (!string.IsNullOrWhiteSpace(nouveauVendeur.Ville))
            {
                unVendeur.Ville = nouveauVendeur.Ville;
            }

            if (!string.IsNullOrWhiteSpace(nouveauVendeur.Province))
            {
                unVendeur.Province = nouveauVendeur.Province;
            }

            if (!string.IsNullOrWhiteSpace(nouveauVendeur.CodePostal))
            {
                unVendeur.CodePostal = nouveauVendeur.CodePostal;
            }

            if (!string.IsNullOrWhiteSpace(nouveauVendeur.Tel1))
            {
                unVendeur.Tel1 = nouveauVendeur.Tel1;
            }

            if (!string.IsNullOrWhiteSpace(nouveauVendeur.Tel2))
            {
                unVendeur.Tel2 = nouveauVendeur.Tel2;
            }

            unVendeur.DateMAJ = DateTime.Now;

            try
            {
                contextPP.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void modifierProfilMDP(string strNouveauMDP)
        {
            unVendeur.MotDePasse = strNouveauMDP;

            unVendeur.DateMAJ = DateTime.Now;

            try
            {
                contextPP.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void modifierProfilSpecificVendeur(PPVendeurs nouveauVendeur)
        {
            if (!string.IsNullOrWhiteSpace(nouveauVendeur.NomAffaires))
            {
                unVendeur.NomAffaires = nouveauVendeur.NomAffaires;
            }

            if (nouveauVendeur.PoidsMaxLivraison != null)
            {
                unVendeur.PoidsMaxLivraison = nouveauVendeur.PoidsMaxLivraison;
            }

            if (nouveauVendeur.LivraisonGratuite != null)
            {
                unVendeur.LivraisonGratuite = nouveauVendeur.LivraisonGratuite;
            }

            if (nouveauVendeur.Taxes != null)
            {
                unVendeur.Taxes = nouveauVendeur.Taxes;
            }

            unVendeur.DateMAJ = DateTime.Now;

            try
            {
                contextPP.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public PPVendeurs rechecheVendeurParNo(long lngNoVendeur)
        {
            return contextPP.PPVendeurs.FirstOrDefault(vendeur => vendeur.NoVendeur == lngNoVendeur);
        }

        public PPVendeurs rechercheVendeurParCourriel(string strCourriel)
        {
            return contextPP.PPVendeurs.FirstOrDefault(vendeur => vendeur.AdresseEmail == strCourriel);
        }
    }
}
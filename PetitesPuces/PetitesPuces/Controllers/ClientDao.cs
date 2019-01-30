using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PetitesPuces.Models;

namespace PetitesPuces.Controllers
{
    public class ClientDao : IClient
    {
        private DataClasses1DataContext contextPP = new DataClasses1DataContext();
        private PPClients unClient;

        public ClientDao(int intNoClient)
        {
            unClient = (from unClient in contextPP.PPClients
                        where unClient.NoClient == intNoClient
                        select unClient).First();
        }

        public ClientDao()
        {
            String strAdresseCourrielClient = "Client10000@cgodin.qc.ca";
            unClient = (from unClient in contextPP.PPClients
                        where unClient.AdresseEmail == strAdresseCourrielClient
                        select unClient).First();
        }

        public void modifierProfilInformationPersonnel(PPClients nouveauClient)
        {
            //Penser a si le client veux juste retirer une information sans la remplacer

            //penser aussi si les données envoyer sont identique (pas de modification)


            if (!string.IsNullOrWhiteSpace(nouveauClient.Prenom))
            {
                unClient.Prenom = nouveauClient.Prenom;
            }

            if (!string.IsNullOrWhiteSpace(nouveauClient.Nom))
            {
                unClient.Nom = nouveauClient.Nom;
            }

            if (!string.IsNullOrWhiteSpace(nouveauClient.Rue))
            {
                unClient.Rue = nouveauClient.Rue;
            }

            if (!string.IsNullOrWhiteSpace(nouveauClient.Ville))
            {
                unClient.Ville = nouveauClient.Ville;
            }

            if (!string.IsNullOrWhiteSpace(nouveauClient.Province))
            {
                unClient.Province = nouveauClient.Province;
            }

            if (!string.IsNullOrWhiteSpace(nouveauClient.CodePostal))
            {
                unClient.CodePostal = nouveauClient.CodePostal;
            }

            if (!string.IsNullOrWhiteSpace(nouveauClient.Tel1))
            {
                unClient.Tel1 = nouveauClient.Tel1;
            }

            if (!string.IsNullOrWhiteSpace(nouveauClient.Tel2))
            {
                unClient.Tel2 = nouveauClient.Tel2;
            }

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
            unClient.MotDePasse = strNouveauMDP;

            try
            {
                contextPP.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public PPClients rechecheClientParCourriel(string strCourriel)
        {
            return contextPP.PPClients.FirstOrDefault(client => client.AdresseEmail == strCourriel);
        }

        public PPClients rechecheClientParNo(long lngNoClient)
        {
            return contextPP.PPClients.FirstOrDefault(client => client.NoClient == lngNoClient);
        }
    }
}
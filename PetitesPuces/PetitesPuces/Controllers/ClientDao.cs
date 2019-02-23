using System;
using System.Linq;
using PetitesPuces.Models;

namespace PetitesPuces.Controllers
{
    public class ClientDao : IClient
    {
        private DataClasses1DataContext contextPP = new DataClasses1DataContext();
        private PPClients unClient;

        public ClientDao(long lngNoClient)
        {
            unClient = contextPP.PPClients.FirstOrDefault(client => client.NoClient == lngNoClient);
        }

        public ClientDao()
        {
            String strAdresseCourrielClient = "Client10001@gmail.com";
            unClient = (from unClient in contextPP.PPClients
                        where unClient.AdresseEmail == strAdresseCourrielClient
                        select unClient).First();
        }

        public void modifierProfilInformationPersonnel(PPClients nouveauClient)
        {
            //penser aussi si les données envoyer sont identique (pas de modification)

            unClient.Prenom = nouveauClient.Prenom;

            unClient.Nom = nouveauClient.Nom;

            unClient.Rue = nouveauClient.Rue;

            unClient.Ville = nouveauClient.Ville;

            unClient.Province = nouveauClient.Province;

            unClient.CodePostal = nouveauClient.CodePostal;

            unClient.Tel1 = nouveauClient.Tel1;

            unClient.Tel2 = nouveauClient.Tel2;

            unClient.DateMAJ = DateTime.Now;

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

            unClient.DateMAJ = DateTime.Now;

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
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

        public void modifierProfilInformationPersonnel(PPClients unClient)
        {
            throw new NotImplementedException();
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
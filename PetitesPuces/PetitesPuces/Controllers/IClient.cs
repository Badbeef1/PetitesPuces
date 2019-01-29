using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetitesPuces.Controllers
{
    interface IClient
    {
        void modifierProfilInformationPersonnel(Models.PPClients unClient);
        void modifierProfilMDP(String strNouveauMDP);
        Models.PPClients rechecheClientParCourriel(string strCourriel);
        Models.PPClients rechecheClientParNo(long lngNoClient);
    }
}

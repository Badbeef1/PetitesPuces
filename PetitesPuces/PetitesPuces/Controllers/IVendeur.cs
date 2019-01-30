using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetitesPuces.Controllers
{
    interface IVendeur
    {
        void modifierProfilInformationPersonnel(Models.PPVendeurs unVendeur);
        void modifierProfilMDP(String strNouveauMDP);
        void modifierProfilSpecificVendeur(Models.PPVendeurs nouveauVendeur);
        Models.PPVendeurs rechercheVendeurParCourriel(string strCourriel);
        Models.PPVendeurs rechecheVendeurParNo(long lngNoVendeur);
    }
}

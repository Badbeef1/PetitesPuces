using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetitesPuces.Models
{
    public class SaisieCommandeViewModel
    {
        public List<PPArticlesEnPanier> lstArticlePanier = new List<PPArticlesEnPanier>();

        public PPClients client = new PPClients();

        public PPVendeurs vendeur = new PPVendeurs();
    }
}
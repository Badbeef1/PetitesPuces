using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetitesPuces.Models
{
    public class SaisieCommandeViewModel
    {
        public List<PPArticlesEnPanier> lstArticlePanier { get; set; }

        public PPClients client { get; set; }

        public PPVendeurs vendeur { get; set; }
    }
}
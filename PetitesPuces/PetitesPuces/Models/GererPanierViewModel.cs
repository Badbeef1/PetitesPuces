using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetitesPuces.Models
{
    public class GererPanierViewModel
    {
        public List<GererPanier> lstPanierAncien { get; set; }
        public List<List<PPArticlesEnPanier>> lstPanierRecent { get; set; }

        public int valeurDdl { get; set; }
        public class GererPanier
        {
            public List<PPArticlesEnPanier> ppArtPan { get; set; }

            public bool isChecked { get; set; }
        }
    }

}
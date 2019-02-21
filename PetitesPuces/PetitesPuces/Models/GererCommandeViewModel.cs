using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetitesPuces.Models
{
    public class GererCommandeViewModel
    {
        public List<GererCommande> lstCommandeNonLivrer { get; set; }
        public List<PPCommandes> lstCommandeLivrer { get; set; }

        public List<PPHistoriquePaiements> lstPaiement { get; set; }
        public class GererCommande
        {
            public PPCommandes commande { get; set; }

            public bool isChecked { get; set; }
        }
    }

}
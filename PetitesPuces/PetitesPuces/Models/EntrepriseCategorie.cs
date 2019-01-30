using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetitesPuces.Models
{
    public class EntrepriseCategorie
    {
        Models.PPCategories categorie { get; set; }
        List<Models.PPVendeurs> lstVendeurs { get; set;}

        public EntrepriseCategorie(PPCategories categorie, List<PPVendeurs> lstVendeurs)
        {
            this.categorie = categorie;
            this.lstVendeurs = lstVendeurs;
        }
    }
}
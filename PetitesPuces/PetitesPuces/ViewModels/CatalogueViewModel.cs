using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetitesPuces.ViewModels
{
    public class CatalogueViewModel
    {
        public List<Models.PPCategories> lstCategorie { get; set; }
        public List<Models.PPProduits> lstproduits { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;

namespace PetitesPuces.ViewModels
{
    public class CatalogueViewModel
    {
        public List<Models.PPCategories> lstCategorie { get; set; }
        public List<Models.PPProduits> lstproduits { get; set; }
        public String strTri { get; set; }
        public String strCategorie { get; set; }
        public String strRecherche { get; set; }
        public int pageDimension { get; set; }
        public int intNoPage { get; set; }
        public int intTypeRecherche { get; set; }
        public IPagedList<Models.PPProduits> iplProduits { get; set; }
    }
}
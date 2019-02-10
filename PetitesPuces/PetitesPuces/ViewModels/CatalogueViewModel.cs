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
        public String recherche { get; set; }
        public String recherche2 { get; set; }
        public int pageDimension { get; set; }
        public int intNoPage { get; set; }
        public int typeRech { get; set; }
        public IPagedList<Models.PPProduits> iplProduits { get; set; }
        public String strParent { get; set; }
        public Dictionary<string, List<string>> dicVendeur { get; set; }
        public String vendeur { get; set; }
        public List<Models.PPVendeurs> lstVendeur { get; set; }
        public Models.PPVendeurs vendeurCatalogue { get; set; }

        public HttpPostedFile test { get; set; }
    }
}
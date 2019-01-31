using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetitesPuces.Models
{
    public class AccueilClientViewModel
    {
        public List<EntrepriseCategorie> lstEntreCategories { get; set; }
        public IQueryable<IGrouping<PetitesPuces.Models.PPVendeurs, PetitesPuces.Models.PPArticlesEnPanier>> lstPanierVendeur { get; set; }
        public AccueilClientViewModel(List<EntrepriseCategorie> lstEntreCategories, IQueryable<IGrouping<PPVendeurs, PPArticlesEnPanier>> lstPanierVendeur)
        {
            this.lstEntreCategories = lstEntreCategories;
            this.lstPanierVendeur = lstPanierVendeur;
        }
    }

}
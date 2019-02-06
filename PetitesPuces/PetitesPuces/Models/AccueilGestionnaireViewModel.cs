using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetitesPuces.Models
{
   public class AccueilGestionnaireViewModel
   {
      public List<PPVendeurs> lstDemandesVendeurs { get; set; }
      public Dictionary<PPCategories,bool> lstCategories { get; set; }

      public AccueilGestionnaireViewModel(List<PPVendeurs> lstDemandesVendeurs, Dictionary<PPCategories, bool> lstCategories)
      {
         this.lstDemandesVendeurs = lstDemandesVendeurs;
         this.lstCategories = lstCategories;
      }
   }
}
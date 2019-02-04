using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetitesPuces.Models
{
   public class AccueilGestionnaireViewModel
   {
      public List<PPVendeurs> lstDemandesVendeurs { get; set; }

      public AccueilGestionnaireViewModel(List<PPVendeurs> lstDemandesVendeurs)
      {
         this.lstDemandesVendeurs = lstDemandesVendeurs;
      }
   }
}
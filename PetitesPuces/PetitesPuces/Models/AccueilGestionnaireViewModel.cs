using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetitesPuces.Models
{
   public class AccueilGestionnaireViewModel
   {
      public List<PPVendeurs> lstDemandesVendeurs { get; set; }
      public Dictionary<PPCategories, bool> lstCategories { get; set; }
      public PPCategories categorie { get; set; }
      public Dictionary<PPVendeurs,bool> lstVendeurs { get; set; }
      public Dictionary<PPHistoriquePaiements,PPVendeurs> lstRedevances { get; set; }
      
      
      public AccueilGestionnaireViewModel(List<PPVendeurs> lstDemandesVendeurs, Dictionary<PPCategories, bool> lstCategories, PPCategories categorie)
      {
         this.lstDemandesVendeurs = lstDemandesVendeurs;
         this.lstCategories = lstCategories;
         this.categorie = categorie;
      }
      public AccueilGestionnaireViewModel()
      {

      }
   }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetitesPuces.Models
{
   public class GestionProduitViewModel
   {
      public PPProduits produit { get; set; }
      public HttpPostedFileBase file { get; set; }

      public GestionProduitViewModel()
      {
         
      }
   }
}
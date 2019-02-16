using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetitesPuces.Models
{
   public class TotalCommandesClientParVendeurViewModel
   {
      public PPClients client { get; set; }
      public PPVendeurs vendeur { get; set; }
      public double dblTotalCommande { get; set; }
      public DateTime dateDerniereCommande { get; set; }

      public TotalCommandesClientParVendeurViewModel(PPClients client, PPVendeurs vendeur, double dblTotalCommande, DateTime dateDerniereCommande)
      {
         this.client = client;
         this.vendeur = vendeur;
         this.dblTotalCommande = dblTotalCommande;
         this.dateDerniereCommande = dateDerniereCommande;
      }

      public TotalCommandesClientParVendeurViewModel()
      {
      }
   }
}
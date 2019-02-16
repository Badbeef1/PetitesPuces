using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetitesPuces.Models
{
   public class VisitesClients
   {
      public string noClient { get; set; }
      public int nbVisites { get; set; }

      public VisitesClients(string noClient, int nbVisites)
      {
         this.noClient = noClient;
         this.nbVisites = nbVisites;
      }

      public VisitesClients()
      {
      }
   }
}
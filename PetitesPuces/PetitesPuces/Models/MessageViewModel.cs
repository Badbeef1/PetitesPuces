using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetitesPuces.Models
{
   public class MessageViewModel
   {
      public int noExpediteur { get; set; }
      public int noDestinataire { get; set; }
      public string nomExpediteur { get; set; }
      public string nomDestinataire { get; set; }
      public string message { get; set; }

      public MessageViewModel(int noExpediteur, int noDestinataire, string nomExpediteur, string nomDestinataire, string message)
      {
         this.noExpediteur = noExpediteur;
         this.noDestinataire = noDestinataire;
         this.nomExpediteur = nomExpediteur;
         this.nomDestinataire = nomDestinataire;
         this.message = message;
      }
   }
}
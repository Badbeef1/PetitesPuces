using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PetitesPuces.Models
{
   public class NouveauMDP
   {
      [Required(ErrorMessage = "Le champ mot de passe est requis.")]
      public string password1 { get; set; }
      [Required(ErrorMessage = "Le champ mot de passe est requis.")]
      public string password2 { get; set; }
      public string courriel { get; set; }

      public NouveauMDP(string password1, string password2,string courriel)
      {
         this.password1 = password1;
         this.password2 = password2;
         this.courriel = courriel;
      }

      public NouveauMDP()
      {
      }
   }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PetitesPuces.Models
{
   public class ValiderMotPasse
   {
      [Required(ErrorMessage = "Vous devez saisir un courriel.")]
      [DataType(DataType.EmailAddress, ErrorMessage = "Ce n'est pas un format de courriel valide!")]
      public string courriel { get; set; }
   }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PetitesPuces.Models
{
   public class DateVenteEtPrixVenteValidation: ValidationAttribute
   {
      public DateTime dateVente { get; set; }
      public decimal prixVente { get; set; }

      public DateVenteEtPrixVenteValidation(DateTime dateVente, decimal prixVente)
      {
         this.dateVente = dateVente;
         this.prixVente = prixVente;
      }

      protected override ValidationResult IsValid(object value, ValidationContext validationContext)
      {
         //Vérifier si les deux sont vides
         if((dateVente.Equals(null)) && (prixVente.Equals(null)))
         {
            return null;
         }
         else if(!(dateVente.Equals(null)) && !(prixVente.Equals(null)))
         {
            return null;
         }
         else
         {
            return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
         }
      }
   }
}
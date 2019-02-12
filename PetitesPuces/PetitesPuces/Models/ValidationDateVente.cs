using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PetitesPuces.Models
{
   public class ValidationDateVente: ValidationAttribute
   {

      public override object TypeId => base.TypeId;

      public override bool RequiresValidationContext => base.RequiresValidationContext;
      
      public override string ToString()
      {
         return base.ToString();
      }

      public override bool Equals(object obj)
      {
         return base.Equals(obj);
      }

      public override int GetHashCode()
      {
         return base.GetHashCode();
      }

      public override bool Match(object obj)
      {
         return base.Match(obj);
      }

      public override bool IsDefaultAttribute()
      {
         return base.IsDefaultAttribute();
      }

      public override string FormatErrorMessage(string name)
      {
         return base.FormatErrorMessage(name);
      }

      public override bool IsValid(object value)
      {
         return base.IsValid(value);
      }

      protected override ValidationResult IsValid(object value, ValidationContext validationContext)
      {
         if(value == null)
         {
            return null;
         }
         if((DateTime)value < DateTime.Now)
         {
            return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
         }
         return null;
         //return base.IsValid(value, validationContext);
      }
   }
}
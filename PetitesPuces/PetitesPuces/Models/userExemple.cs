using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PetitesPuces.Models
{
   public class userExemple
   {
      public int userId { get; set; }

      public string username { get; set; }

      [DataType(DataType.Password)]
      public string password { get; set; }

      public string errorMessage { get; set; }
      
      [NotMapped] // Does not effect with your database
      [Compare("password")]
      public string confirmUsername { get; set; }

      [NotMapped]
      [DataType(DataType.Password)]
      [Compare("password")]
      public string confirmPassword { get; set; }
      

      //test vendeurs

      public string nomAffaires { get; set; }

      public string nom { get; set; }
      public string prenom { get; set; }
      public string rue { get; set; }
      public string ville { get; set; }
      public string province { get; set; }
      public string codePostal { get; set; }
      public string tel1 { get; set; }
      public string tel2 { get; set; }

      public int poidsMaxLivraison { get; set; }
      public bool libraisonGratuite { get; set; }
      public int taxes { get; set; }
      public float pourcentage { get; set; }
   }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PetitesPuces.Models
{
    public class PPClientViewModel
    {
        public PPClients client { get; set; }
        public PPVendeurs vendeur { get; set; }

        public string errorMessage { get; set; }

        [NotMapped] // Does not effect with your database
        //[Compare("client.AdresseEmail")]
        public string confirmUsername { get; set; }

        [NotMapped]
        [DataType(DataType.Password)]
        //[Compare("client.MotDePasse")]
        public string confirmPassword { get; set; }

        public bool boolVendeur { get; set; }
    }
}
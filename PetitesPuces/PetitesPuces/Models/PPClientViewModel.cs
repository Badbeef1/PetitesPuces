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

        [Required]
        public string confirmUsername { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        public string confirmPassword { get; set; }
        

        public string errorMessage { get; set; }

        public bool boolVendeur { get; set; }

    }
}
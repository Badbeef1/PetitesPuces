using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        
    }
}
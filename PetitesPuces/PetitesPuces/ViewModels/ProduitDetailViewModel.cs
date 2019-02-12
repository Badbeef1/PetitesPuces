using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PetitesPuces.ViewModels;
using PetitesPuces.Models;
using System.ComponentModel.DataAnnotations;

namespace PetitesPuces.ViewModels
{
    public class ProduitDetailViewModel
    {
        public PPProduits Produit { get; set; }
        
        public PPEvaluations Evaluation { get; set; }

    }
}
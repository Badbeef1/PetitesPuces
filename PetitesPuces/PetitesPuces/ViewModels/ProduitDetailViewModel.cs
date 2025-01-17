﻿using System;
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
        public decimal FormattedRating { get; set; }
        public PPEvaluations Evaluation { get; set; }
        public bool ClientARecuCeProduit { get; set; }
        public int nbEvaluateurs { get; set; }
    }
}
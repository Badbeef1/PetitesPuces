using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;
using PetitesPuces.Models;

namespace PetitesPuces.ViewModels
{
   public class EvaluationsViewModel
   {

      public List<PPEvaluations> LstEvaluations { get; set; }
      public List<Tuple<PPEvaluations, string>> LstEvalEtNomClient { get; set; }
      public decimal FormattedRating { get; set; }
      public List<int> LstPourcentage { get; set; }

      public PPEvaluations Evaluation { get; set; }

      public PPProduits Produit { get; set; }
   }
}
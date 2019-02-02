using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetitesPuces.Models
{
   public class AccueilVendeurViewModel
   {
      public Dictionary<PPCommandes, List<PPDetailsCommandes>> lstCommandesNonTraites { get; set; }
      public PPHistoriquePaiements historique { get; set; }
      public IQueryable<IGrouping<PPClients,PPArticlesEnPanier>> lstClientPanier { get; set; }
      public int nbVisites { get; set; }

      //Constructeur pour la page d'accueil vendeur
      public AccueilVendeurViewModel(Dictionary<PPCommandes, List<PPDetailsCommandes>> lstCommandesNonTraites, IQueryable<IGrouping<PPClients, PPArticlesEnPanier>> lstClientPanier, int nbVisites)
      {
         this.lstCommandesNonTraites = lstCommandesNonTraites;
         this.lstClientPanier = lstClientPanier;
         this.nbVisites = nbVisites;
      }

      //Constructeur pour la page Detail Commande
      public AccueilVendeurViewModel(Dictionary<PPCommandes, List<PPDetailsCommandes>> lstCommandesNonTraites, PPHistoriquePaiements historique)
      {
         this.lstCommandesNonTraites = lstCommandesNonTraites;
         this.historique = historique;
      }
   }
}
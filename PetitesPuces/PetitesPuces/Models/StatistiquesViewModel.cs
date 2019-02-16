using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetitesPuces.Models
{
   public class StatistiquesViewModel
   {
      //Pour les statisques total de vendeurs
      public int nbVendeurAccepte { get; set; }
      public int nbVendeurRefuse { get; set; }

      //Nombre total de clients (actif, potentiel et visiteur)
      public int nbClientActif { get; set; }
      public int nbClientPotentiel { get; set; }
      public int nbClientVisiteurs { get; set; }

      //Nombre total des nouveau vendeur/client entre 1,3,6,9,12 mois 
      public int nbClientsUnMois { get; set; }
      public int nbClientsTroisMois { get; set; }
      public int nbClientsSixMois { get; set; }
      public int nbClientsNeufMois { get; set; }
      public int nbClientsDouzeMois { get; set; }

      public int nbVendeurUnMois { get; set; }
      public int nbVendeurTroisMois { get; set; }
      public int nbVendeurSixMois { get; set; }
      public int nbVendeurNeufMois { get; set; }
      public int nbVendeurDouzeMois { get; set; }

      //Nombre total de connexions clients
      public int nbConnexionsClient { get; set; }

      //Listes des dernière connexion
      public List<PPClients> lstDereniereConnexion { get; set; } 

      //Listes de tout les vendeurs
      public List<PPVendeurs> lstVendeurs { get; set; }

      //Liste des stats client par vendeur pour le premier vendeur
      public List<int> lstClientsVendeur { get; set; }

      //Dictionnaire pour le graphique du nombre de visites d'un client pour un vendeur
      public Dictionary<PPClients,int> dicVisitesClientsVendeurs { get; set; }

      public StatistiquesViewModel()
      {

      }
   }
}
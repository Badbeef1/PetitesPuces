using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PetitesPuces.Models;
using PagedList;

namespace PetitesPuces.Views
{
    public class CourrielController : Controller
    {
        DataClasses1DataContext contextPP = new DataClasses1DataContext();

        // GET: Courriel
        public ActionResult Index(short? lieu)
        {
            const String strClient = "Client";
            const String strVendeur = "Vendeur";
            const String strGestionnaire = "Gestionnaire";
            
            
            //Liste de tout les lieux pour la bar de navigation
            List<PPLieu> lstLieu = contextPP.PPLieu.ToList();

            //Type d'utilisateur
            String strTypeUtilisateur = "";
            dynamic utilisateur;

            if (Session["clientObj"] != null)
            {
                strTypeUtilisateur = strClient;
                utilisateur = contextPP.PPClients
                    .FirstOrDefault(client => client.NoClient == (Session["clientObj"] as PPClients).NoClient);
            }
            else if (Session["vendeurObj"] != null)
            {
                strTypeUtilisateur = strVendeur;
                utilisateur = contextPP.PPVendeurs
                    .FirstOrDefault(vendeur => vendeur.NoVendeur == (Session["vendeurObj"] as PPVendeurs).NoVendeur);
            }
            else
            {
                strTypeUtilisateur = strGestionnaire;
                utilisateur = contextPP.PPGestionnaire
                    .FirstOrDefault(gestionnaire => gestionnaire.AdresseEmail == (Session["gestionnaireObj"] as PPGestionnaire).AdresseEmail);
            }


            //Notification par dossier
            Dictionary<short, int> dicNotificationLieu = new Dictionary<short, int>();

            switch (utilisateur)
            {
                
            }




            ViewModels.CourrielVM courrielVM = new ViewModels.CourrielVM
            {
                lstLieu = lstLieu,
                lieu = lieu ?? 1
            };


            return View(courrielVM);
        }
    }
}
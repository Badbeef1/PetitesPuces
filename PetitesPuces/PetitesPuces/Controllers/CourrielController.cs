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
        public ActionResult Index(string id, short? lieu)
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

            List<PPDestinataires> lstDestinataires01 = new List<PPDestinataires>();

            switch (utilisateur)
            {
                case PPClients x:

                    /*
                     * Code purement théorique, rien n'a été tester
                     */

                    
                    //Metton lieu 1 Boite de reception
                    var unLieu = lstLieu[0];

                    //Parcour les destinataires pour trouver les messages de l'utilisateur
                    lstDestinataires01 = contextPP.PPDestinataires
                        .Where(predicate: des => des.Lieu == unLieu.NoLieu && des.NoDestinataire == x.NoClient)
                        .ToList();

                    //Metton que etat non lu == 0
                    int intMessageNonLu = lstDestinataires01
                        .Where(predicate: des => des.EtatLu.Value == 0)
                        .Count();

                    dicNotificationLieu.Add(unLieu.NoLieu, intMessageNonLu);
                    //---
                    //Metton lieu 3 Message supprimer
                    unLieu = lstLieu[2];

                    //Parcour les destinataires pour trouver les messages de l'utilisateur supprimer
                    List<PPDestinataires> lstDestinataires03 = contextPP.PPDestinataires
                        .Where(predicate: des => des.Lieu == unLieu.NoLieu && des.NoDestinataire == x.NoClient)
                        .ToList();

                    dicNotificationLieu.Add(unLieu.NoLieu, lstDestinataires03.Count);
                    //---
                    //Metton lieu 4 Bouillon
                    unLieu = lstLieu[3];

                    //Parcour les destinataires pour trouver les messages de l'utilisateur brouillon
                    List<PPMessages> lstMessage04 = contextPP.PPMessages
                        .Where(predicate: mess => mess.NoExpediteur == x.NoClient && mess.dateEnvoi.HasValue == false)
                        .ToList();

                    dicNotificationLieu.Add(unLieu.NoLieu, lstMessage04.Count);



                    break;
                case PPVendeurs y:
                    
                    break;
                case PPGestionnaire z:

                    break;
            }


            ViewModels.CourrielVM courrielVM = new ViewModels.CourrielVM
            {
                lstLieu = lstLieu,
                lieu = lieu ?? 1
            };


            if (id == "Reception")
            {
                SectionBoiteReception(ref courrielVM, lstDestinataires01);
            }



            


            return View(courrielVM);
        }

        /*
        *  Rien n'a été tester
        */
        private void SectionBoiteReception(ref ViewModels.CourrielVM courrielVM, List<PPDestinataires> lstDestinataires)
        {
            List<Tuple<PPDestinataires, String>> lstDestinataireEtExpediteur = new List<Tuple<PPDestinataires, string>>();
            
            //parcour les destinataires pour recuperer le nom a afficher 
            lstDestinataires.ForEach(dest => {
                dynamic dynExpediteur;
                int intNoExpediteur = dest.PPMessages.NoExpediteur.Value;

                //Si le client n'a pas inscrit de prenom ou de nom, l'adresse courriel va etre afficher au lieu du nom
                if (dynExpediteur = contextPP.PPClients.FirstOrDefault(predicate: client => client.NoClient == intNoExpediteur) != null)
                {
                    PPClients unClient = (dynExpediteur as PPClients);

                    String strNomAffichage = (unClient.Nom is null || unClient.Prenom is null) ? unClient.AdresseEmail : unClient.Prenom + " " + unClient.Nom;

                    lstDestinataireEtExpediteur.Add(new Tuple<PPDestinataires, string>(dest, strNomAffichage));
                }
                else if (dynExpediteur = contextPP.PPVendeurs.FirstOrDefault(predicate: vendeur => vendeur.NoVendeur == intNoExpediteur) != null)
                {
                    lstDestinataireEtExpediteur.Add(new Tuple<PPDestinataires, string>(dest, (dynExpediteur as PPVendeurs).NomAffaires));
                }
                else
                {
                    //Quand le gestionnaire aura un numero identification
                }

            });

            courrielVM.iplDestionataireBoiteReception = lstDestinataireEtExpediteur.ToPagedList(1, 20);
            
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PetitesPuces.Models;
using PagedList;
using System.IO;

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
                /*
                utilisateur = contextPP.PPGestionnaire
                    .FirstOrDefault(gestionnaire => gestionnaire.AdresseEmail == (Session["gestionnaireObj"] as PPGestionnaire).AdresseEmail);
                    */
                utilisateur = contextPP.PPVendeurs.FirstOrDefault();
            }


            //Notification par dossier
            Dictionary<short, int> dicNotificationLieu = new Dictionary<short, int>();

            List<PPDestinataires> lstDestinataires01 = new List<PPDestinataires>();

            ViewModels.CourrielVM courrielVM = new ViewModels.CourrielVM();


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


                    courrielVM.addresseExpediteur = x.AdresseEmail;

                    //Get vendeurs
                    courrielVM.lstVendeursCourriels = contextPP.PPVendeurs.Select(m => m.AdresseEmail).ToList();
                    //Get gestionnaires
                    courrielVM.lstGestionnairesCourriels = contextPP.PPGestionnaire.Select(m => m.AdresseEmail).ToList();
                    break;
                case PPVendeurs y:


                    courrielVM.addresseExpediteur = y.AdresseEmail;

                    //Get soi-même
                    courrielVM.lstVendeursCourriels = contextPP.PPVendeurs.Where(v => v.AdresseEmail == y.AdresseEmail).Select(m => m.AdresseEmail).ToList();
                    //Get clients
                    courrielVM.lstClientsCourriels = contextPP.PPClients.Select(m => m.AdresseEmail).ToList();
                    //Get gestionnaires
                    courrielVM.lstGestionnairesCourriels = contextPP.PPGestionnaire.Select(m => m.AdresseEmail).ToList();
                    break;
                case PPGestionnaire z:


                    courrielVM.addresseExpediteur = z.AdresseEmail;

                    //Get gestionnaires
                    courrielVM.lstGestionnairesCourriels = contextPP.PPGestionnaire.Select(m => m.AdresseEmail).ToList();
                    //Get clients
                    courrielVM.lstClientsCourriels = contextPP.PPClients.Select(m => m.AdresseEmail).ToList();
                    //Get vendeurs
                    courrielVM.lstVendeursCourriels = contextPP.PPVendeurs.Select(m => m.AdresseEmail).ToList();
                    break;
            }



            courrielVM.lstLieu = lstLieu;
            courrielVM.lieu = lieu ?? 1;


            getNoDestinataireEtAdresse(contextPP);


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
            lstDestinataires.ForEach(dest =>
            {
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

        [HttpPost]
        public ActionResult Soumettre(string submit, ViewModels.CourrielVM model)
        {
            //Parce que le model veut pas garder la liste
            model.lstLieu = contextPP.PPLieu.ToList();

            //Liste de toutes les IDs et leur courriel
            var listeNoDestEtAdresse = getNoDestinataireEtAdresse(contextPP);
            
            //ID de l'expediteur (utilisateur courant)
            var noExpediteur = listeNoDestEtAdresse.Where(m => m.Item2.Equals(model.addresseExpediteur)).Select(s => s.Item1).FirstOrDefault();
            //Nb messages present
            var noMessage = contextPP.PPMessages.Count() + 1;
            

            switch (submit)
            {
                case "Envoyer":
                    if (!ModelState.IsValid || Request["ddlDestinataires"] == null)
                    {
                        model.msgErreurCourriel = "Erreur d'envoi: Aucun champ ne doit être vide sauf pour le fichier joint.";
                        return View("Index", model);
                    }

                    var lstCourriels = Request["ddlDestinataires"]?.Split(',');
                    var listeNoDestinataires = listeNoDestEtAdresse.Where(m => lstCourriels.Contains(m.Item2)).Select(s => s.Item1);

                    var message = new PPMessages()
                    {
                        NoMsg = noMessage,
                        NoExpediteur = int.Parse(noExpediteur.ToString()),
                        DescMsg = model.messageCourriel,
                        FichierJoint = model.fichierJoint.FileName,
                        Lieu = 2,
                        dateEnvoi = DateTime.Now,
                        objet = model.objetMessage
                    };

                    // Sauvegarde le fichier sur le serveur
                    string path = Server.MapPath("~/Uploads/");
                    if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                    model.fichierJoint.SaveAs(path + Path.GetFileName(model.fichierJoint.FileName));

                    contextPP.PPMessages.InsertOnSubmit(message);

                    var lstEnvoi = new List<PPDestinataires>();

                    foreach (var destinataire in listeNoDestinataires)
                    {
                        lstEnvoi.Add(new PPDestinataires()
                        {
                            NoMsg = noMessage,
                            NoDestinataire = int.Parse(destinataire.ToString()),
                            EtatLu = 0,
                            Lieu = 1
                        });
                    }

                    contextPP.PPDestinataires.InsertAllOnSubmit(lstEnvoi);

                    contextPP.SubmitChanges();
                    model.msgSuccesCourriel = "Le courriel a été envoyé à tous les destinataires.";
                    break;
                case "Enregistrer":

                    if (!ModelState.IsValid)
                    {
                        model.msgErreurCourriel = "Erreur d'enregistrement: L'objet du message ni le message ne peuvent pas être vides.";
                        return View("Index", model);
                    }
                    
                    contextPP.PPMessages.InsertOnSubmit(new PPMessages()
                    {
                        NoMsg = noMessage,
                        NoExpediteur = int.Parse(noExpediteur.ToString()),
                        DescMsg = model.messageCourriel,
                        FichierJoint = model.fichierJoint.FileName,
                        Lieu = 4,
                        //dateEnvoi = DateTime.Now,
                        objet = model.objetMessage
                    });

                    contextPP.SubmitChanges();

                    string path1 = Server.MapPath("~/Uploads/");
                    if (!Directory.Exists(path1))
                        Directory.CreateDirectory(path1);
                    model.fichierJoint.SaveAs(path1 + Path.GetFileName(model.fichierJoint.FileName));
                    model.msgSuccesCourriel = "Le courriel a été enregistré.";
                    break;
            }

            return View("Index", model);
        }

        private List<Tuple<long, string>> getNoDestinataireEtAdresse(DataClasses1DataContext context)
        {
            var clients = from c in context.PPClients select new { c.NoClient, c.AdresseEmail };
            var vendeurs = from v in context.PPVendeurs select new { v.NoVendeur, v.AdresseEmail };
            var gestionnaire = (from g in context.PPGestionnaire select new { g.NoGestionnaire, g.AdresseEmail });

            List<Tuple<long, string>> liste = new List<Tuple<long, string>>();

            foreach (var c in clients)
                liste.Add(new Tuple<long, string>(c.NoClient, c.AdresseEmail));
            foreach (var v in vendeurs)
                liste.Add(new Tuple<long, string>(v.NoVendeur, v.AdresseEmail));
            foreach (var g in gestionnaire)
                liste.Add(new Tuple<long, string>(g.NoGestionnaire, g.AdresseEmail));

            return liste;
        }
    }
}
﻿using System;
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

        const String strClient = "Client";
        const String strVendeur = "Vendeur";
        const String strGestionnaire = "Gestionnaire";

        const String actionLu = "lu";
        const String actionNonLu = "nonlu";
        const String actionSupprimer = "supp";
        const String actionSupprimerDefinitivement = "suppdef";
        const String actionRestaurer = "restau";

        const string strBoiteReception = "Reception";
        const string strAffichageMessage = "AffichageMessage";
        const string strBoiteSupprime = "SupprimePartiel";
        const string strSupprimeDefinitivement = "SupprimeTotal";
        const string strBrouillon = "Brouillon";
        const string strNouveauMessage = "NouveauMessage";
        const string strEnvoye = "Envoyer";

        // GET: Courriel
        public ActionResult Index(string id, short? lieu, int? message, String ElementSelectionner, String uneAction)
        {
            
            //Liste de tout les lieux pour la bar de navigation
            List<PPLieu> lstLieu = contextPP.PPLieu.ToList();


            //Type d'utilisateur
            //String strTypeUtilisateur = "";
            dynamic utilisateur;
            long lngNoUtilisateur = 0;

            if (Session["clientObj"] != null)
            {
                //strTypeUtilisateur = strClient;
                utilisateur = contextPP.PPClients
                    .FirstOrDefault(client => client.NoClient == (Session["clientObj"] as PPClients).NoClient);
                lngNoUtilisateur = (utilisateur as PPClients).NoClient;
            }
            else if (Session["vendeurObj"] != null)
            {
                //strTypeUtilisateur = strVendeur;
                utilisateur = contextPP.PPVendeurs
                    .FirstOrDefault(vendeur => vendeur.NoVendeur == (Session["vendeurObj"] as PPVendeurs).NoVendeur);

                lngNoUtilisateur = (utilisateur as PPVendeurs).NoVendeur;
            }
            else
            {
                //strTypeUtilisateur = strGestionnaire;
                /*
                utilisateur = contextPP.PPGestionnaire
                .FirstOrDefault(gestionnaire => gestionnaire.NoGestionnaire == (Session["gestionnaireObj"] as PPGestionnaire).NoGestionnaire);
                */



                utilisateur = contextPP.PPVendeurs.FirstOrDefault();

                lngNoUtilisateur = (utilisateur as PPVendeurs).NoVendeur;

                //lngNoUtilisateur = (utilisateur as PPGestionnaire).NoGestionnaire;
            }

            //Si des modifications
            if (ElementSelectionner != null && uneAction != null)
            {
                optionListeMessages(new List<string>(ElementSelectionner.Split(',')), lngNoUtilisateur, uneAction);
            }


            //Notification par dossier
            Dictionary<short, int> dicNotificationLieu = new Dictionary<short, int>();

            List<PPDestinataires> lstDestinatairesBoiteReception = new List<PPDestinataires>();
            
            switch (utilisateur)
            {
                case PPClients c:
                    Tuple­<Dictionary<short, int>, List<PPDestinataires>> tupNotification = notificationParLieu(lstLieu, c.NoClient);

                    dicNotificationLieu = tupNotification.Item1;
                    lstDestinatairesBoiteReception = tupNotification.Item2;

                    //lngNoUtilisateur = c.NoClient;
                    break;
                case PPVendeurs v:
                    Tuple­<Dictionary<short, int>, List<PPDestinataires>> tupNotification1 = notificationParLieu(lstLieu, v.NoVendeur);

                    dicNotificationLieu = tupNotification1.Item1;
                    lstDestinatairesBoiteReception = tupNotification1.Item2;

                    //lngNoUtilisateur = v.NoVendeur;
                    break;
                case PPGestionnaire g:
                    Tuple­<Dictionary<short, int>, List<PPDestinataires>> tupNotification2 = notificationParLieu(lstLieu, g.NoGestionnaire);

                    dicNotificationLieu = tupNotification2.Item1;
                    lstDestinatairesBoiteReception = tupNotification2.Item2;

                    //lngNoUtilisateur = g.NoGestionnaire;
                    break;
            }

            ViewModels.CourrielVM courrielVM = new ViewModels.CourrielVM
            {
                lstLieu = lstLieu,
                lieu = lieu ?? 1,
                dicNotificationLieu = dicNotificationLieu,
                strPage = id ?? "Reception"
            };

            //Init lstDestinataires et l'adresse de l'expediteur
            GetListePourRedactionMessage(utilisateur, null, courrielVM);

            List<PPDestinataires> lstDestinataire = new List<PPDestinataires>();
            List<PPMessages> lstMessage = new List<PPMessages>();

            switch (id)
            {
                case strBoiteReception:
                case null:
                    lstDestinataire = contextPP.PPDestinataires
                        .Where(predicate: des => des.Lieu == 1 && des.NoDestinataire == lngNoUtilisateur)
                        .ToList();
                    break;
                case strEnvoye:
                case strBrouillon:
                    lstMessage = contextPP.PPMessages
                        .Where(predicate: mess => mess.Lieu == ((id == strBrouillon) ? 4 : 2) && mess.NoExpediteur == lngNoUtilisateur)
                        .ToList();
                    

                    break;
                case strBoiteSupprime:
                case strSupprimeDefinitivement:
                    lstDestinataire = contextPP.PPDestinataires
                        .Where(predicate: des => des.Lieu == ((id == strSupprimeDefinitivement) ? 5 : 3) && des.NoDestinataire == lngNoUtilisateur)
                        .ToList();

                    lstMessage = contextPP.PPMessages
                        .Where(predicate: mess => mess.Lieu == ((id == strSupprimeDefinitivement) ? 5 : 3) && mess.NoExpediteur == lngNoUtilisateur)
                        .ToList();
                    break;
            }


            if (id == "AffichageMessage" && message.HasValue)
            {
                courrielVM.valtupAfficheMessage = AffichageMessage(message.Value, utilisateur);
            }
            else if (id == strBoiteSupprime || id == strSupprimeDefinitivement)
            {
                courrielVM.iplListeMessageAffiche = ListeCourrielDestinataire(lstDestinataire).Concat(ListeCourrielMessage(lstMessage)).ToPagedList(1, 20);
            }
            else if (id == strEnvoye || id == strBrouillon)
            {
                courrielVM.iplListeMessageAffiche = ListeCourrielMessage(lstMessage).ToPagedList(1, 20);
            }
            else if(id == strNouveauMessage && message != null)
            {
                var msgObj = contextPP.PPMessages.FirstOrDefault(x => x.NoMsg == message);
                courrielVM.noMessageOuvert = msgObj.NoMsg;
                courrielVM.messageCourriel = msgObj.DescMsg;
                courrielVM.objetMessage = msgObj.objet;
                courrielVM = GetListePourRedactionMessage(utilisateur,message,courrielVM);
            }
            else
            {

                courrielVM.iplListeMessageAffiche = ListeCourrielDestinataire(lstDestinataire).ToPagedList(1, 20); //1,20 tempo
            }



            return View(courrielVM);
        }

        private List<ViewModels.MessageAfficheVM> ListeCourrielDestinataire(List<PPDestinataires> lstDestinataires)
        {
            List<ViewModels.MessageAfficheVM> lstMessageAfficher = new List<ViewModels.MessageAfficheVM>();

            //parcour les destinataires pour recuperer le nom a afficher 
            lstDestinataires.ForEach(dest =>
            {
                dynamic dynExpediteur;
                int intNoExpediteur = dest.PPMessages.NoExpediteur.Value;

                ViewModels.MessageAfficheVM messVM = new ViewModels.MessageAfficheVM
                {
                    Destinataire = dest,
                    Message = dest.PPMessages,
                    ShrEtat = 0
                };


                //Si le client n'a pas inscrit de prenom ou de nom, l'adresse courriel va etre afficher au lieu du nom
                if ((dynExpediteur = contextPP.PPClients.FirstOrDefault(predicate: client => client.NoClient == intNoExpediteur)) != null)
                {
                    PPClients unClient = (dynExpediteur as PPClients);

                    String strNomAffichage = (unClient.Nom is null || unClient.Prenom is null) ? unClient.AdresseEmail : unClient.Prenom + " " + unClient.Nom;

                    messVM.StrNomAffichage = strNomAffichage;

                    lstMessageAfficher.Add(messVM);
                }
                else if ((dynExpediteur = contextPP.PPVendeurs.FirstOrDefault(predicate: vendeur => vendeur.NoVendeur == intNoExpediteur)) != null)
                {
                    messVM.StrNomAffichage = (dynExpediteur as PPVendeurs).NomAffaires;

                    lstMessageAfficher.Add(messVM);
                }
                else
                {
                    messVM.StrNomAffichage = (dynExpediteur as PPGestionnaire).AdresseEmail;

                    lstMessageAfficher.Add(messVM);
                }

            });
            
            return lstMessageAfficher;
        }

        private List<ViewModels.MessageAfficheVM> ListeCourrielMessage(List<PPMessages> lstMessages) 
        {
            List<ViewModels.MessageAfficheVM> lstMessageAfficher = new List<ViewModels.MessageAfficheVM>();

            //parcour les messages pour recuperer le nom a afficher 
            lstMessages.ForEach(mess =>
            {
                dynamic dynDestinataire;
                int intNoExpediteur = mess.NoExpediteur.Value;


                ViewModels.MessageAfficheVM messVM = new ViewModels.MessageAfficheVM
                {
                    Message = mess,
                    ShrEtat = 0
                };

                List<PPDestinataires> lstDestinataires = contextPP.PPDestinataires
                    .Where(dest => dest.NoMsg == mess.NoMsg)
                    .ToList();

                int intNbDestinataire = lstDestinataires.Count;
                if (intNbDestinataire > 1)
                {
                    messVM.StrNomAffichage = intNbDestinataire.ToString() + " destinataires ...";

                }
                else if (intNbDestinataire == 1)
                {
                    int intNoDestinataire = lstDestinataires[0].NoDestinataire;

                    //Si le client n'a pas inscrit de prenom ou de nom, l'adresse courriel va etre afficher au lieu du nom
                    if ((dynDestinataire = contextPP.PPClients.FirstOrDefault(predicate: client => client.NoClient == intNoDestinataire)) != null)
                    {
                        PPClients unClient = (dynDestinataire as PPClients);

                        String strNomAffichage = (unClient.Nom is null || unClient.Prenom is null) ? unClient.AdresseEmail : unClient.Prenom + " " + unClient.Nom;

                        messVM.StrNomAffichage = strNomAffichage;

                        //lstMessageAfficher.Add(messVM);
                    }
                    else if ((dynDestinataire = contextPP.PPVendeurs.FirstOrDefault(predicate: vendeur => vendeur.NoVendeur == intNoDestinataire)) != null)
                    {
                        messVM.StrNomAffichage = (dynDestinataire as PPVendeurs).NomAffaires;

                        //lstMessageAfficher.Add(messVM);
                    }
                    else
                    {
                        messVM.StrNomAffichage = (dynDestinataire as PPGestionnaire).AdresseEmail;

                        //lstMessageAfficher.Add(messVM);
                    }
                }

                lstMessageAfficher.Add(messVM);
            });

            return lstMessageAfficher;
        }


        private (PPDestinataires,string,string) AffichageMessage(int intNoMessage, dynamic utilisateur)
        {
            String strDestinataire = "";
            String strExpediteur = "";
            long lngNoDestinataire = 0;

            switch (utilisateur)
            {
                case PPClients c:
                    strDestinataire = c.AdresseEmail;
                    lngNoDestinataire = c.NoClient ;
                    break;
                case PPVendeurs v:
                    strDestinataire = v.AdresseEmail;
                    lngNoDestinataire = v.NoVendeur;
                    break;
                case PPGestionnaire g:
                    strDestinataire = g.AdresseEmail;
                    lngNoDestinataire = g.NoGestionnaire;
                    break;
            }

            PPDestinataires destinataires = contextPP.PPDestinataires
                .FirstOrDefault(predicate: dest => dest.NoDestinataire == lngNoDestinataire && dest.NoMsg == intNoMessage);

            dynamic expediteur;
            int intNoExpediteur = destinataires.PPMessages.NoExpediteur.Value;
            if ((expediteur = contextPP.PPClients.FirstOrDefault(predicate: client => client.NoClient == intNoExpediteur)) != null)
            {
                PPClients unClient = (expediteur as PPClients);

                strExpediteur = (unClient.Nom is null || unClient.Prenom is null) ? unClient.AdresseEmail : unClient.Prenom + " " + unClient.Nom + " <" + unClient.AdresseEmail + ">";
            }
            else if ((expediteur = contextPP.PPVendeurs.FirstOrDefault(predicate: vendeur => vendeur.NoVendeur == intNoExpediteur)) != null)
            {
                PPVendeurs unVendeur = (expediteur as PPVendeurs);

                strExpediteur = unVendeur.NomAffaires + " <" + unVendeur.AdresseEmail + ">";
            }
            else
            {
                strExpediteur = (expediteur as PPGestionnaire).AdresseEmail;
            }

            //Change l'état de non lu à lu
            if (destinataires.EtatLu == 0)
            {
                destinataires.EtatLu = 1;

                try
                {
                    contextPP.SubmitChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);

                }
            }


                return (destinataires, strDestinataire, strExpediteur);
        }

        //Touve le nombre de notification par lieu
        //La liste des destinataire est utile dans une autre méthode, donc je l'exporte ...
        private Tuple­<Dictionary<short, int>, List<PPDestinataires>> notificationParLieu(List<PPLieu> lstDesLieux, long lngNoUtilisateur)
        {
            Dictionary<short, int> dicNbNotification = new Dictionary<short, int>();
            List<PPDestinataires> lstDestinatairesBR = new List<PPDestinataires>();

            //Prend en compet que les lieux n'ont pas été altéré 
            lstDesLieux.ForEach(unLieu => {

                //Boite de reception
                if (unLieu.NoLieu == 1)
                {
                    //Parcour les destinataires pour trouver les messages de l'utilisateur
                    lstDestinatairesBR = contextPP.PPDestinataires
                        .Where(predicate: des => des.Lieu == unLieu.NoLieu && des.NoDestinataire == lngNoUtilisateur)
                        .ToList();

                    //Metton que etat non lu == 0
                    int intMessageNonLu = lstDestinatairesBR
                        .Where(predicate: des => des.EtatLu.Value == 0)
                        .Count();

                    dicNbNotification.Add(unLieu.NoLieu, intMessageNonLu);
                }
                //Boite supprimer
                else if (unLieu.NoLieu == 3)
                {
                    //Parcour les destinataires pour trouver les messages de l'utilisateur supprimer
                    List<PPDestinataires> lstDestinataires03 = contextPP.PPDestinataires
                        .Where(predicate: des => des.Lieu == unLieu.NoLieu && des.NoDestinataire == lngNoUtilisateur)
                        .ToList();

                    dicNbNotification.Add(unLieu.NoLieu, lstDestinataires03.Count);
                }
                //Brouillon
                else if (unLieu.NoLieu == 4)
                {
                    //Parcour les destinataires pour trouver les messages de l'utilisateur brouillon
                    List<PPMessages> lstMessage04 = contextPP.PPMessages
                        .Where(predicate: mess => mess.NoExpediteur == lngNoUtilisateur && mess.dateEnvoi.HasValue == false)
                        .ToList();

                    dicNbNotification.Add(unLieu.NoLieu, lstMessage04.Count);
                }
            });

            return new Tuple<Dictionary<short, int>, List<PPDestinataires>>(dicNbNotification, lstDestinatairesBR);
        }

        //Applique changement au destinataire
        private void optionListeMessages(List<String> lstElementATraiter,long lngNoUtilisateur, string strAction)
        {
            List<PPDestinataires> lstTempoDestinataire = new List<PPDestinataires>();

            lstElementATraiter.ForEach(element =>
            {
                PPDestinataires destinataires = contextPP.PPDestinataires
                    .FirstOrDefault(predicate: desti => desti.NoDestinataire == lngNoUtilisateur && desti.NoMsg == int.Parse(element));

                switch (strAction)
                {
                    case actionLu:
                        destinataires.EtatLu = 1;
                        break;
                    case actionNonLu:
                        destinataires.EtatLu = 0;
                        break;
                    case actionSupprimer:
                        destinataires.Lieu = 3;
                        break;
                    case actionRestaurer:
                        destinataires.Lieu = 1;
                        break;
                    case actionSupprimerDefinitivement:
                        destinataires.Lieu = 5;
                        break;
                }

                lstTempoDestinataire.Add(destinataires);
            });

            try
            {
                contextPP.SubmitChanges();
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }


        [HttpPost]
        public ActionResult Soumettre(string submit, ViewModels.CourrielVM model)
        {
            //Parce que le model veut pas garder la liste
            model.lstLieu = contextPP.PPLieu.ToList();

            //Liste de toutes les IDs et leur courriel
            var listeNoDestEtAdresse = GetNoDestinataireEtAdresse(contextPP);
            
            //ID de l'expediteur (utilisateur courant)
            var noExpediteur = listeNoDestEtAdresse.Where(m => m.Item2.Equals(model.addresseExpediteur)).Select(s => s.Item1).FirstOrDefault();
            //Nb messages present
           var messageOuvert = contextPP.PPMessages.FirstOrDefault(x => x.NoMsg == model.noMessageOuvert);
            var noMessage = messageOuvert?.NoMsg?? contextPP.PPMessages.Count() + 1;

         switch (submit)
         {
            case "Envoyer":
               if (!ModelState.IsValid || Request["ddlDestinataires"] == null)
               {
                  model.msgErreurCourriel = "Erreur d'envoi: Aucun champ ne doit être vide sauf pour le fichier joint.";
                  return RedirectToAction("Index", model);
               }

               if(messageOuvert == null) { 
                  //Cree un nouveau message
                  var message = new PPMessages()
                  {
                     NoMsg = noMessage,
                     NoExpediteur = int.Parse(noExpediteur.ToString()),
                     DescMsg = model.messageCourriel,
                     FichierJoint = model.fichierJoint?.FileName,
                     Lieu = 2,
                     dateEnvoi = DateTime.Now,
                     objet = model.objetMessage
                  };
                  contextPP.PPMessages.InsertOnSubmit(message);
               }
               else
               {
                  //Update le brouillon
                  messageOuvert.DescMsg = model.messageCourriel;
                  messageOuvert.objet = model.objetMessage;
                  messageOuvert.Lieu = 2;
                  messageOuvert.dateEnvoi = DateTime.Now;
                  messageOuvert.FichierJoint = model.fichierJoint?.FileName;
                  contextPP.SubmitChanges();
               }


               // Sauvegarde le fichier sur le serveur
               var path = Server.MapPath("~/Uploads/");
               if (!Directory.Exists(path)) Directory.CreateDirectory(path);
               model.fichierJoint?.SaveAs(path + Path.GetFileName(model.fichierJoint?.FileName));

               if(messageOuvert?.PPDestinataires.Count > 0) { 
                  contextPP.PPDestinataires.DeleteAllOnSubmit(messageOuvert.PPDestinataires);
                  contextPP.SubmitChanges();
               }
               var lstNoDest = Request["ddlDestinataires"]?.Split(',');
               //Liste des destinataires à insérer dans la table PPDestinataires
               var lstEnvoi = lstNoDest.Select(destinataire => new PPDestinataires()
               {
                  NoMsg = noMessage,
                  NoDestinataire = int.Parse(destinataire),
                  EtatLu = 0,
                  Lieu = 1
               }).ToList();

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


               // var listeNoDestinataires1 = listeNoDestEtAdresse.Where(m => lstCourriels1.Contains(m.Item2)).Select(s => s.Item1);

               contextPP.PPMessages.InsertOnSubmit(new PPMessages()
               {
                  NoMsg = noMessage,
                  NoExpediteur = int.Parse(noExpediteur.ToString()),
                  DescMsg = model.messageCourriel,
                  FichierJoint = model.fichierJoint?.FileName,
                  Lieu = 4,
                  //dateEnvoi = DateTime.Now,
                  objet = model.objetMessage
               });

               var lstCourriels1 = Request["ddlDestinataires"]?.Split(',');
               if (Request["ddlDestinataires"] != null)
               {
                  var lstEnvoi1 = new List<PPDestinataires>();
                  foreach (var destinataire in lstCourriels1)
                  {
                     lstEnvoi1.Add(new PPDestinataires()
                     {
                        NoMsg = noMessage,
                        NoDestinataire = int.Parse(destinataire),
                        EtatLu = -1,
                        Lieu = 1
                     });
                  }
                  contextPP.PPDestinataires.InsertAllOnSubmit(lstEnvoi1);
               }

               try
               {
                  contextPP.SubmitChanges();
               }
               catch (Exception ex)
               {

               }

               if (model.fichierJoint != null)
               {
                  var path1 = Server.MapPath("~/Uploads/");
                  if (!Directory.Exists(path1))
                     Directory.CreateDirectory(path1);
                  model.fichierJoint?.SaveAs(path1 + Path.GetFileName(model.fichierJoint?.FileName));
               }
               model.msgSuccesCourriel = "Le courriel a été enregistré.";
               break;
            default:
               model.msgErreurCourriel = "Action invalide !";
               return RedirectToAction("Index", model);

         }

         return RedirectToAction("Index", model);
      }

      private List<Tuple<long, string>> GetNoDestinataireEtAdresse(DataClasses1DataContext context)
        {
            var clients = from c in context.PPClients select new { c.NoClient, c.AdresseEmail };
            var vendeurs = from v in context.PPVendeurs select new { v.NoVendeur, v.AdresseEmail };
            var gestionnaire = from g in context.PPGestionnaire select new { g.NoGestionnaire, g.AdresseEmail };

            List<Tuple<long, string>> liste = new List<Tuple<long, string>>();

            foreach (var c in clients)
                liste.Add(new Tuple<long, string>(c.NoClient, c.AdresseEmail));
            foreach (var v in vendeurs)
                liste.Add(new Tuple<long, string>(v.NoVendeur, v.AdresseEmail));
            foreach (var g in gestionnaire)
                liste.Add(new Tuple<long, string>(g.NoGestionnaire, g.AdresseEmail));

            return liste;
        }

        //Categorie :   0=admin, 1=vendeur, 2=client
        //Tuple<Categorie utilisateur, noUtilisateur, adresseEmail, nom + prenom/ NomAffaires, selectionné?>
        public ViewModels.CourrielVM GetListePourRedactionMessage(dynamic util,int? noMsg, ViewModels.CourrielVM model)
        {
            var objMsg = contextPP.PPMessages.FirstOrDefault(x => x.NoMsg == noMsg);
            var destinataires = objMsg?.PPDestinataires;

            var liste = new List<Tuple<short, long, string, string, bool>>();

            switch (util)
            {
                case PPClients c:
                    model.addresseExpediteur = c.AdresseEmail;

                    //Get tous les vendeurs
                    foreach(var vendeur in contextPP.PPVendeurs) { 
                        liste.Add(new Tuple<short, long, string,string, bool>
                            (1,vendeur.NoVendeur,vendeur.AdresseEmail,vendeur.NomAffaires,
                            destinataires?.Any(x=> x.NoDestinataire == vendeur.NoVendeur)?? false));
                    }

                    //Get tous les gestionnaires
                    foreach (var gest in contextPP.PPGestionnaire)
                    {
                        liste.Add(new Tuple<short, long, string, string, bool>
                            (0, gest.NoGestionnaire, gest.AdresseEmail, null,destinataires?.Any(x => x.NoDestinataire == gest.NoGestionnaire)?? false));
                    }

                    //Get tous les clients
                    foreach(var cli in contextPP.PPClients)
                    {
                        liste.Add(new Tuple<short, long, string, string, bool>
                            (2, cli.NoClient, cli.AdresseEmail, (cli.Nom + " " + cli.Prenom).Trim(),
                            destinataires?.Any(x => x.NoDestinataire == cli.NoClient)?? false));
                    }
                    
                    break;
                case PPVendeurs v:
                    model.addresseExpediteur = v.AdresseEmail;
                    
                    //Get soi-même
                    //model.lstVendeursCourriels = contextPP.PPVendeurs.Where(a => a.AdresseEmail == v.AdresseEmail).Select(m => m.AdresseEmail).ToList();
                    liste.Add(new Tuple<short, long, string, string, bool>
                        (1,v.NoVendeur,v.AdresseEmail,v.NomAffaires, destinataires?.Any(x => x.NoDestinataire == v.NoVendeur)??false));
                    
                    
                    //Get clients
                    foreach (var cli in contextPP.PPClients)
                    {
                        liste.Add(new Tuple<short, long, string, string, bool>
                            (2, cli.NoClient, cli.AdresseEmail, (cli.Nom + " " + cli.Prenom).Trim(),
                            destinataires?.Any(x => x.NoDestinataire == cli.NoClient)??false));
                    }

                    //Get gestionnaires
                    foreach (var gest in contextPP.PPGestionnaire)
                    {
                        liste.Add(new Tuple<short, long, string, string, bool>
                            (0, gest.NoGestionnaire, gest.AdresseEmail, null, destinataires?.Any(x => x.NoDestinataire == gest.NoGestionnaire)??false));
                    }

                    break;

                case PPGestionnaire g:
                    model.addresseExpediteur = g.AdresseEmail;

                    //Get tous les vendeurs
                    foreach (var vendeur in contextPP.PPVendeurs)
                    {
                        liste.Add(new Tuple<short, long, string, string, bool>
                            (1, vendeur.NoVendeur, vendeur.AdresseEmail, vendeur.NomAffaires,
                            destinataires?.Any(x => x.NoDestinataire == vendeur.NoVendeur)??false));
                    }

                    //Get tous les gestionnaires
                    foreach (var gest in contextPP.PPGestionnaire)
                    {
                        liste.Add(new Tuple<short, long, string, string, bool>
                            (0, gest.NoGestionnaire, gest.AdresseEmail, null, destinataires?.Any(x => x.NoDestinataire == gest.NoGestionnaire)??false));
                    }

                    //Get tous les clients
                    foreach (var cli in contextPP.PPClients)
                    {
                        liste.Add(new Tuple<short, long, string, string, bool>
                            (2, cli.NoClient, cli.AdresseEmail, (cli.Nom + " " + cli.Prenom).Trim(),
                            destinataires?.Any(x => x.NoDestinataire == cli.NoClient)??false));
                    }
                    
                    break;
            }
            model.lstDestinataires = liste;

            return model;
        }
        
        [HttpGet]
        public FileResult TelechargeFichierJoin(object nom)
        {
            //Vérifie si le fichier existe dans la base de donnée
            //...





            return null;
        }
   }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using PetitesPuces.Models;


namespace PetitesPuces.Controllers
{
    public class InternauteController : Controller
    {
        // GET: Inscription
        public ActionResult Index() => View("AccueilInternaute");

        public ActionResult AccueilInternaute()
        {
            List <Models.EntrepriseCategorie> lstEntreCate = new List<Models.EntrepriseCategorie>();
            /* Compare data with Database */
            Models.DataClasses1DataContext db = new Models.DataClasses1DataContext();
            db.Connection.Open();
            var toutesCategories = (from cat in db.GetTable<Models.PPCategories>()
                                    select cat
                                    );
            foreach(var cat in toutesCategories)
            {
                List<PPVendeurs> lstVendeurs = new List<PPVendeurs>();
                var query = (from prod in db.GetTable<Models.PPProduits>()
                             where prod.NoCategorie.Equals(cat.NoCategorie)
                             select prod
                             );
                foreach(var item in query)
                {
                    if (!lstVendeurs.Contains(item.PPVendeurs))
                    {
                        lstVendeurs.Add(item.PPVendeurs);
                    }
                }
                lstEntreCate.Add(new Models.EntrepriseCategorie(cat, lstVendeurs));
            }
            return View(lstEntreCate);
        }

        public ActionResult CatalogueNouveaute() => View();


        //GET
        public ActionResult Inscription() => View();


        [HttpPost] //POST
        public ActionResult Inscription(Models.PPClientViewModel model)
        {

            /* Variables required for client registration */
            var username = model.vendeur.AdresseEmail;
            var confUsername = model.confirmUsername;
            var password = model.vendeur.MotDePasse;
            var confPassword = model.confirmPassword;
            var clientSectionValid = (username != null && password != null);
            var usernameConfirmValid = username == confUsername;
            var passwordConfirmValid = password == confPassword;

            /* Variables required for seller registration */
            var etreVendeur = model.boolVendeur;

            //reset
            model.errorMessage = "";
            model.okMessage = "";

            /* Some validations */
            try
            { MailAddress ma = new MailAddress(username); }
            catch (Exception ex) { model.errorMessage = "Le format de l'adresse courriel n'est pas valide !"; }

            model.errorMessage = usernameConfirmValid ?
                (passwordConfirmValid ? model.errorMessage : "Le deuxième mot de passe doit correspondre au premier.") : "Le deuxième courriel doit correspondre au premier.";


            if (!etreVendeur) // "Je veux etre vendeur" is not checked
            {
                //Clear errors in Vendeur section
                foreach (var item in ModelState.Keys.Where(s => !s.Equals("vendeur.AdresseEmail") &&
                        !s.Equals("vendeur.MotDePasse") && !s.Equals("confirmUsername") && !s.Equals("confirmPassword")))
                {
                    ModelState[item].Errors.Clear();
                }
            }

            /* Database section */
            var context = new DataClasses1DataContext();

            if (clientSectionValid && !etreVendeur && usernameConfirmValid && passwordConfirmValid)
            {
                //Register client  
                context.Connection.Open();
                var max = (context.PPClients.Max(x => x.NoClient) + 1);

                //Maximum amount of clients reached
                if (max >= 99999)
                    model.errorMessage = "Nous avons atteint le nombre maximum de vendeurs (100).";

                //Email exists              "model.vendeur" part is ok
                else if (context.PPClients.Any(x => x.AdresseEmail.ToLower().Equals(model.vendeur.AdresseEmail.ToLower())))
                    model.errorMessage = "Ce courriel est déjà inscrit !";

                //else if(ModelState["vendeur.AdresseEmail"]?.Errors != null)
                //    model.errorMessage = "Le format de courriel est invalide !";

                //Stop right there if theres an error
                if (model.errorMessage != "") return View(model);

                //Transaction
                using (var transaction = new TransactionScope())
                {
                    try
                    {
                        context.PPClients.InsertOnSubmit(new PPClients()
                        {
                            NoClient = max > 10000 ? max : 10001,
                            AdresseEmail = model.vendeur.AdresseEmail,
                            MotDePasse = model.vendeur.MotDePasse,
                            DateCreation = DateTime.Now,
                            Statut = 1
                        });

                        context.SubmitChanges();
                        model.okMessage = "L'ajout dans la base de données a réussi. ";
                        transaction.Complete();
                    }
                    catch (Exception ex)
                    {
                        model.errorMessage = "L'ajout dans la base de données a échoué. " + ex.Message;
                    }
                }

                context.Connection.Close();

                model = new PPClientViewModel() { errorMessage = model.errorMessage, okMessage = model.okMessage, vendeur = null };
            }
            else if (clientSectionValid && usernameConfirmValid && passwordConfirmValid && etreVendeur && ModelState.IsValid)
            {
                //Register seller
                context.Connection.Open();

                var max = (context.PPVendeurs.Max(x => x.NoVendeur) + 1);

                //Maximum amount of sellers reached
                if (max >= 100)
                    model.errorMessage = "Nous avons atteint le nombre maximum de vendeurs (100).";

                //Email exists
                else if (context.PPVendeurs.Any(x => x.AdresseEmail.ToLower().Equals(model.vendeur.AdresseEmail.ToLower())))
                    model.errorMessage = "Ce courriel est déjà inscrit !";

                //Company name exists
                else if (context.PPVendeurs.Any(x => x.NomAffaires.ToLower().Equals(model.vendeur.NomAffaires.ToLower())))
                    model.errorMessage = "Le nom d'entreprise existe déjà !";

                //Stop right there if theres an error
                if (model.errorMessage != "") return View(model);

                //Transaction
                using (var transaction = new TransactionScope())
                {
                    try
                    {
                        model.vendeur.DateCreation = DateTime.Now;

                        model.vendeur.NoVendeur = max > 10 ? max : 11;
                        model.vendeur.Statut = 0;
                        context.PPVendeurs.InsertOnSubmit(model.vendeur);

                        context.SubmitChanges();
                        model.okMessage = "L'ajout dans la base de données a réussi. ";
                        transaction.Complete();
                    }
                    catch (Exception ex)
                    {
                        model.errorMessage = "L'ajout dans la base de données a échoué. " + ex.Message;
                    }
                }

                context.Connection.Close();

                model = new PPClientViewModel() { errorMessage = model.errorMessage, okMessage = model.okMessage, vendeur = null };
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult VerifyEntry() => null;


    }
}
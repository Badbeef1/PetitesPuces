using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using PetitesPuces.Models;

namespace PetitesPuces.Controllers
{
    public class BDController : Controller
    {
        // GET: BD
        public ActionResult Index()
        {
            if (Session["gestionnaireObj"] != null)
            {

                ViewModels.XmlDataViewModel model = new ViewModels.XmlDataViewModel();
                DataClasses1DataContext context = new DataClasses1DataContext();

                context.Connection.ConnectionString = Properties.Settings.Default.BD6B8_424R_TESTSConnectionString;
                try
                {
                    context.Connection.Open();
                    model.tabCount = GetTableCount(context, true);
                    setBtnStatus(model);
                    model.tabCount = GetTableCount(context, model.enableDeleteButton);

                }
                catch (Exception ex)
                {
                    model.errorMessage = ex.Message;
                }

                context.Connection.Close();
                return View(model);
            }
            else return new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden);
        }

        //Insert
        [HttpPost]
        public ActionResult Ajouter(ViewModels.XmlDataViewModel model)
        {
            if (Session["gestionnaireObj"] == null) return View("Index", model);

            List<PPClients> lstClients = new List<PPClients>();
            List<PPVendeurs> lstVendeurs = new List<PPVendeurs>();
            List<PPProduits> lstProduits = new List<PPProduits>();

            List<PPDetailsCommandes> lstDetailsCommandes = new List<PPDetailsCommandes>();
            List<PPCommandes> lstCommandes = new List<PPCommandes>();
            List<PPArticlesEnPanier> lstArticlesEnPanier = new List<PPArticlesEnPanier>();
            List<PPHistoriquePaiements> lstHistoriquePaiements = new List<PPHistoriquePaiements>();

            List<PPCategories> lstCategories = new List<PPCategories>();
            List<PPVendeursClients> lstVendeursClients = new List<PPVendeursClients>();
            List<PPGestionnaire> lstGestionnaire = new List<PPGestionnaire>();
            List<PPTaxeFederale> lstTaxeFederale = new List<PPTaxeFederale>();
            List<PPTaxeProvinciale> lstTaxeProvinciale = new List<PPTaxeProvinciale>();
            List<PPTypesLivraison> lstTypesLivraison = new List<PPTypesLivraison>();
            List<PPTypesPoids> lstTypesPoids = new List<PPTypesPoids>();
            List<PPPoidsLivraisons> lstPoidsLivraisons = new List<PPPoidsLivraisons>();


            XDocument doc = XDocument.Load(HostingEnvironment.MapPath("~/Content/fichierXML.xml"));
            XNamespace ss = "urn:schemas-microsoft-com:office:spreadsheet";

            foreach (var v in doc.DescendantNodes())
            {
                XElement elem = v.NextNode as XElement;

                if (elem?.Name == ss + "Workbook") //Workbook
                {
                    var workSheets = elem?.Elements().Where(t => t.Name == ss + "Worksheet");
                    if (workSheets == null) return View();

                    foreach (var sheet in workSheets)
                    {
                        var tables = sheet.Elements().Where(n => n.Name == ss + "Table");
                        foreach (var table in tables)
                        {

                            /*
                             *   Table PPClients
                             */
                            if (sheet.Attribute(ss + "Name").Value == "PPClients")
                            {
                                var tabObjs = GetTableCellStrings(table);
                                foreach (var obj in tabObjs)
                                {
                                    if (obj[0] != null)
                                    {
                                        lstClients.Add(new PPClients()
                                        {
                                            NoClient = CheckLong(obj[0]),
                                            AdresseEmail = obj[1],
                                            MotDePasse = obj[2],
                                            Nom = obj[3],
                                            Prenom = obj[4],
                                            Rue = obj[5],
                                            Ville = obj[6],
                                            Province = obj[7],
                                            CodePostal = obj[8],
                                            Pays = obj[9],
                                            Tel1 = obj[10],
                                            Tel2 = obj[11],
                                            DateCreation = CheckDate(obj[12]),
                                            DateMAJ = CheckDate(obj[13]),
                                            NbConnexions = CheckShort(obj[14]),
                                            DateDerniereConnexion = CheckDate(obj[15]),
                                            Statut = CheckShort(obj[16])
                                        });
                                    }
                                }
                            }


                            /*
                             *   Table PPVendeurs
                             */
                            if (sheet.Attribute(ss + "Name").Value == "PPVendeurs")
                            {
                                var tabObjs = GetTableCellStrings(table);
                                foreach (var obj in tabObjs)
                                {
                                    if (obj[0] != null)
                                    {
                                        lstVendeurs.Add(new PPVendeurs()
                                        {
                                            NoVendeur = CheckLong(obj[0]),
                                            NomAffaires = obj[1],
                                            Nom = obj[2],
                                            Prenom = obj[3],
                                            Rue = obj[4],
                                            Ville = obj[5],
                                            Province = obj[6],
                                            CodePostal = obj[7],
                                            Pays = obj[8],
                                            Tel1 = obj[9],
                                            Tel2 = obj[10],
                                            AdresseEmail = obj[11],
                                            MotDePasse = obj[12],
                                            PoidsMaxLivraison = CheckInt(obj[13]),
                                            LivraisonGratuite = CheckDecimal(obj[14]),
                                            Taxes = CheckInt(obj[15]) == 1,
                                            Pourcentage = CheckDecimal(obj[16]),
                                            Configuration = obj[17],
                                            DateCreation = CheckDate(obj[18]),
                                            DateMAJ = CheckDate(obj[19]),
                                            Statut = CheckShort(obj[20])
                                        });
                                    }
                                }
                            }


                            /*
                             *   Table PPProduits
                             */
                            if (sheet.Attribute(ss + "Name").Value == "PPProduits")
                            {
                                var tabObjs = GetTableCellStrings(table);
                                foreach (var obj in tabObjs)
                                {
                                    if (obj[0] != null)
                                    {
                                        lstProduits.Add(new PPProduits()
                                        {
                                            NoProduit = CheckLong(obj[0]),
                                            NoVendeur = CheckLong(obj[1]),
                                            NoCategorie = CheckInt(obj[2]),
                                            Nom = obj[3],
                                            Description = obj[4],
                                            Photo = obj[5],
                                            PrixDemande = CheckDecimal(obj[6]),
                                            NombreItems = CheckShort(obj[7]),
                                            Disponibilité = CheckInt(obj[8]) == 1,
                                            DateVente = CheckDate(obj[9]),
                                            PrixVente = CheckDecimal(obj[10]),
                                            Poids = CheckDecimal(obj[11]),
                                            DateCreation = CheckDate(obj[12]),
                                            DateMAJ = CheckDate(obj[13])
                                        });
                                    }
                                }
                            }


                            /*
                             *   Table PPDetailsCommandes
                             */
                            if (sheet.Attribute(ss + "Name").Value == "PPDetailsCommandes")
                            {
                                var tabObjs = GetTableCellStrings(table);
                                foreach (var obj in tabObjs)
                                {
                                    if (obj[0] != null)
                                    {
                                        lstDetailsCommandes.Add(new PPDetailsCommandes()
                                        {
                                            NoDetailCommandes = CheckLong(obj[0]),
                                            NoCommande = CheckLong(obj[1]),
                                            NoProduit = CheckLong(obj[2]),
                                            PrixVente = CheckDecimal(obj[3]),
                                            Quantité = CheckShort(obj[4])
                                        });
                                    }
                                }
                            }


                            /*
                             *   Table PPCommandes
                             */
                            if (sheet.Attribute(ss + "Name").Value == "PPCommandes")
                            {
                                var tabObjs = GetTableCellStrings(table);
                                foreach (var obj in tabObjs)
                                {
                                    if (obj[0] != null)
                                    {
                                        lstCommandes.Add(new PPCommandes()
                                        {
                                            NoCommande = CheckLong(obj[0]),
                                            NoClient = CheckLong(obj[1]),
                                            NoVendeur = CheckLong(obj[2]),
                                            DateCommande = CheckDate(obj[3]),
                                            CoutLivraison = CheckDecimal(obj[4]),
                                            TypeLivraison = CheckShort(obj[5]),
                                            MontantTotAvantTaxes = CheckDecimal(obj[6]),
                                            TPS = CheckDecimal(obj[7]),
                                            TVQ = CheckDecimal(obj[8]),
                                            PoidsTotal = CheckDecimal(obj[9]),
                                            Statut = char.Parse(obj[10]),
                                            NoAutorisation = obj[11]
                                        });
                                    }
                                }
                            }


                            /*
                             *   Table PPArticlesEnPanier
                             */
                            if (sheet.Attribute(ss + "Name").Value == "PPArticlesEnPanier")
                            {
                                var tabObjs = GetTableCellStrings(table);
                                foreach (var obj in tabObjs)
                                {
                                    if (obj[0] != null)
                                    {
                                        lstArticlesEnPanier.Add(new PPArticlesEnPanier()
                                        {
                                            NoPanier = CheckLong(obj[0]),
                                            NoClient = CheckLong(obj[1]),
                                            NoVendeur = CheckLong(obj[2]),
                                            NoProduit = CheckLong(obj[3]),
                                            DateCreation = CheckDate(obj[4]),
                                            NbItems = CheckShort(obj[5])
                                        });
                                    }
                                }
                            }


                            /*
                             *   Table PPHistoriquePaiements
                             */
                            if (sheet.Attribute(ss + "Name").Value == "PPHistoriquePaiements")
                            {
                                var tabObjs = GetTableCellStrings(table);
                                foreach (var obj in tabObjs)
                                {
                                    if (obj[0] != null)
                                    {
                                        lstHistoriquePaiements.Add(new PPHistoriquePaiements()
                                        {
                                            NoHistorique = CheckLong(obj[0]),
                                            MontantVenteAvantLivraison = CheckDecimal(obj[1]),
                                            NoVendeur = CheckLong(obj[2]),
                                            NoClient = CheckLong(obj[3]),
                                            NoCommande = CheckLong(obj[4]),
                                            DateVente = CheckDate(obj[5]),
                                            NoAutorisation = obj[6],
                                            FraisLesi = CheckDecimal(obj[7]),
                                            Redevance = CheckDecimal(obj[8]),
                                            FraisLivraison = CheckDecimal(obj[9]),
                                            FraisTPS = CheckDecimal(obj[10]),
                                            FraisTVQ = CheckDecimal(obj[11]),
                                        });
                                    }
                                }
                            }


                            /*
                             *   Table PPCategories
                             */
                            if (sheet.Attribute(ss + "Name").Value == "PPCategories")
                            {
                                var tabObjs = GetTableCellStrings(table);
                                foreach (var obj in tabObjs)
                                {
                                    if (obj[0] != null)
                                    {
                                        lstCategories.Add(new PPCategories()
                                        {
                                            NoCategorie = CheckInt(obj[0]),
                                            Description = obj[1],
                                            Details = obj[2]
                                        });
                                    }
                                }
                            }


                            /*
                             *   Table PPVendeursClients
                             */
                            if (sheet.Attribute(ss + "Name").Value == "PPVendeursClients")
                            {
                                var tabObjs = GetTableCellStrings(table);
                                foreach (var obj in tabObjs)
                                {
                                    if (obj[0] != null)
                                    {
                                        lstVendeursClients.Add(new PPVendeursClients()
                                        {
                                            NoVendeur = CheckLong(obj[0]),
                                            NoClient = CheckLong(obj[1]),
                                            DateVisite = CheckDate(obj[2])
                                        });
                                    }
                                }
                            }


                            /*
                             *   Table PPGestionnaire
                             */

                            if (sheet.Attribute(ss + "Name").Value == "PPGestionnaires")
                            {
                                var tabObjs = GetTableCellStrings(table);
                                foreach (var obj in tabObjs)
                                {
                                    if (obj[0] != null)
                                    {
                                        lstGestionnaire.Add(new PPGestionnaire()
                                        {
                                            NoGestionnaire = CheckInt(obj[0]),
                                            AdresseEmail = obj[1],
                                            MotDePasse = obj[2]
                                        });
                                    }
                                }
                            }


                            /*
                             *   Table PPTaxeFederale
                             */
                            if (sheet.Attribute(ss + "Name").Value == "PPTaxeFederale")
                            {
                                var tabObjs = GetTableCellStrings(table);
                                foreach (var obj in tabObjs)
                                {
                                    if (obj[0] != null)
                                    {
                                        lstTaxeFederale.Add(new PPTaxeFederale()
                                        {
                                            NoTPS = byte.Parse(obj[0]),
                                            DateEffectiveTPS = CheckDate(obj[1]),
                                            TauxTPS = CheckDecimal(obj[2])
                                        });
                                    }
                                }
                            }


                            /*
                             *   Table PPTaxeProvinciale
                             */
                            if (sheet.Attribute(ss + "Name").Value == "PPTaxeProvinciale")
                            {
                                var tabObjs = GetTableCellStrings(table);
                                foreach (var obj in tabObjs)
                                {
                                    if (obj[0] != null)
                                    {
                                        lstTaxeProvinciale.Add(new PPTaxeProvinciale()
                                        {
                                            NoTVQ = byte.Parse(obj[0]),
                                            DateEffectiveTVQ = CheckDate(obj[1]),
                                            TauxTVQ = CheckDecimal(obj[2])
                                        });
                                    }
                                }
                            }


                            /*
                             *   Table PPTypesLivraisons
                             */
                            if (sheet.Attribute(ss + "Name").Value == "PPTypesLivraisons")
                            {
                                var tabObjs = GetTableCellStrings(table);
                                foreach (var obj in tabObjs)
                                {
                                    if (obj[0] != null)
                                    {
                                        lstTypesLivraison.Add(new PPTypesLivraison()
                                        {
                                            CodeLivraison = CheckShort(obj[0]),
                                            Description = obj[1]
                                        });
                                    }
                                }
                            }


                            /*
                             *   Table PPTypesPoids
                             */
                            if (sheet.Attribute(ss + "Name").Value == "PPTypesPoids")
                            {
                                var tabObjs = GetTableCellStrings(table);
                                foreach (var obj in tabObjs)
                                {
                                    if (obj[0] != null)
                                    {
                                        lstTypesPoids.Add(new PPTypesPoids()
                                        {
                                            CodePoids = CheckShort(obj[0]),
                                            PoidsMin = CheckDecimal(obj[1]),
                                            PoidsMax = CheckDecimal(obj[2])
                                        });
                                    }
                                }
                            }


                            /*
                             *   Table PPPoidsLivraisons
                             */
                            if (sheet.Attribute(ss + "Name").Value == "PPPoidsLivraisons")
                            {
                                var tabObjs = GetTableCellStrings(table);
                                foreach (var obj in tabObjs)
                                {
                                    if (obj[0] != null)
                                    {
                                        lstPoidsLivraisons.Add(new PPPoidsLivraisons()
                                        {
                                            CodeLivraison = CheckShort(obj[0]),
                                            CodePoids = CheckShort(obj[1]),
                                            Tarif = CheckDecimal(obj[2])
                                        });
                                    }
                                }
                            }

                        }
                    }
                }
            }


            DataClasses1DataContext context = new DataClasses1DataContext(Properties.Settings.Default.BD6B8_424R_TESTSConnectionString);

            try
            {
                context.Connection.Open();
                context.Transaction = context.Connection.BeginTransaction();


                //Tables sans liens
                context.PPCategories.InsertAllOnSubmit(lstCategories);
                context.PPTypesLivraison.InsertAllOnSubmit(lstTypesLivraison);
                context.PPTypesPoids.InsertAllOnSubmit(lstTypesPoids);
                context.PPTaxeProvinciale.InsertAllOnSubmit(lstTaxeProvinciale);
                context.PPTaxeFederale.InsertAllOnSubmit(lstTaxeFederale);
                context.PPGestionnaire.InsertAllOnSubmit(lstGestionnaire);
                context.PPClients.InsertAllOnSubmit(lstClients);
                context.PPVendeurs.InsertAllOnSubmit(lstVendeurs);

                //Tables avec liens
                context.PPPoidsLivraisons.InsertAllOnSubmit(lstPoidsLivraisons);
                context.PPHistoriquePaiements.InsertAllOnSubmit(lstHistoriquePaiements);
                context.PPProduits.InsertAllOnSubmit(lstProduits);
                context.PPVendeursClients.InsertAllOnSubmit(lstVendeursClients);

                context.PPDetailsCommandes.InsertAllOnSubmit(lstDetailsCommandes);
                context.PPArticlesEnPanier.InsertAllOnSubmit(lstArticlesEnPanier);
                context.PPCommandes.InsertAllOnSubmit(lstCommandes);

                context.SubmitChanges(ConflictMode.ContinueOnConflict);
                context.Transaction.Commit();
                model.successMessage = "Les tables ont étés remplies.";
            }
            catch (Exception ex)
            {
                context.Transaction.Rollback();
                model.errorMessage = ex.Message;
            }

            model.tabCount = GetTableCount(context, true);
            setBtnStatus(model);
            context.Connection.Close();

            ModelState.Clear();
            return View("Index", model);
        }

        [HttpPost]
        public ActionResult Retirer(ViewModels.XmlDataViewModel model)
        {
            if (Session["gestionnaireObj"] == null) return View("Index", model);

            DataClasses1DataContext context = new DataClasses1DataContext();
            context.Connection.ConnectionString = Properties.Settings.Default.BD6B8_424R_TESTSConnectionString;
            try
            {
                context.Connection.Open();
                context.Transaction = context.Connection.BeginTransaction();

                context.PPArticlesEnPanier.DeleteAllOnSubmit(context.PPArticlesEnPanier);
                context.PPDetailsCommandes.DeleteAllOnSubmit(context.PPDetailsCommandes);
                context.PPCommandes.DeleteAllOnSubmit(context.PPCommandes);
                context.PPVendeursClients.DeleteAllOnSubmit(context.PPVendeursClients);
                context.PPHistoriquePaiements.DeleteAllOnSubmit(context.PPHistoriquePaiements);
                context.PPEvaluations.DeleteAllOnSubmit(context.PPEvaluations);
                context.SubmitChanges(ConflictMode.ContinueOnConflict);
                context.PPProduits.DeleteAllOnSubmit(context.PPProduits);

                context.PPDestinataires.DeleteAllOnSubmit(context.PPDestinataires);
                context.PPMessages.DeleteAllOnSubmit(context.PPMessages);
                context.PPLieu.DeleteAllOnSubmit(context.PPLieu);

                context.PPCategories.DeleteAllOnSubmit(context.PPCategories);
                context.PPTaxeFederale.DeleteAllOnSubmit(context.PPTaxeFederale);
                context.PPTaxeProvinciale.DeleteAllOnSubmit(context.PPTaxeProvinciale);
                context.PPPoidsLivraisons.DeleteAllOnSubmit(context.PPPoidsLivraisons);
                context.PPTypesLivraison.DeleteAllOnSubmit(context.PPTypesLivraison);
                context.PPTypesPoids.DeleteAllOnSubmit(context.PPTypesPoids);

                context.PPVendeurs.DeleteAllOnSubmit(context.PPVendeurs);
                context.PPClients.DeleteAllOnSubmit(context.PPClients);
                context.PPGestionnaire.DeleteAllOnSubmit(context.PPGestionnaire);

                context.SubmitChanges(ConflictMode.ContinueOnConflict);
                context.Transaction.Commit();

                model.successMessage = "Les tables ont étés vidées.";
            }
            catch (Exception ex)
            {
                context.Transaction.Rollback();
                model.errorMessage = ex.Message;
            }

            model.tabCount = GetTableCount(context, false);
            setBtnStatus(model);

            context.Connection.Close();
            return View("Index", model);
        }

        public string[,] GetTableCount(DataClasses1DataContext context, bool inverse)
        {
            string[,] countInsertion = new string[,] { };

            if (!inverse)
            {
                countInsertion = new string[,]
                {
               {"PPClients", context.PPClients.Count().ToString()},
               {"PPVendeurs", context.PPVendeurs.Count().ToString()},
               {"PPProduits", context.PPProduits.Count().ToString()},

               {"PPDetailsCommandes", context.PPDetailsCommandes.Count().ToString()},
               {"PPCommandes", context.PPCommandes.Count().ToString()},
               {"PPArticlesEnPanier", context.PPArticlesEnPanier.Count().ToString()},
               {"PPHistoriquePaiements", context.PPHistoriquePaiements.Count().ToString()},

               {"PPCategories", context.PPCategories.Count().ToString()},
               {"PPVendeursClients", context.PPVendeursClients.Count().ToString()},
               {"PPGestionnaires", context.PPGestionnaire.Count().ToString()},
               {"PPTaxeFederale", context.PPTaxeFederale.Count().ToString()},
               {"PPTaxeProvinciale", context.PPTaxeProvinciale.Count().ToString()},
               {"PPTypesLivraison", context.PPTypesLivraison.Count().ToString()},
               {"PPTypesPoids", context.PPTypesPoids.Count().ToString()},
               {"PPPoidsLivraisons", context.PPPoidsLivraisons.Count().ToString()}
                };
            }
            else
            {
                countInsertion = new string[,]
                {

               {"PPArticlesEnPanier", context.PPArticlesEnPanier.Count().ToString()},
               {"PPDetailsCommandes", context.PPDetailsCommandes.Count().ToString()},
               {"PPCommandes", context.PPCommandes.Count().ToString()},
               {"PPVendeursClients", context.PPVendeursClients.Count().ToString()},
               {"PPHistoriquePaiements", context.PPHistoriquePaiements.Count().ToString()},

               {"PPDestinataires", context.PPDestinataires.Count().ToString()},
               {"PPMessages", context.PPMessages.Count().ToString()},
               {"PPLieu", context.PPLieu.Count().ToString()},

               {"PPCategories", context.PPCategories.Count().ToString()},
               {"PPTaxeFederale", context.PPTaxeFederale.Count().ToString()},
               {"PPTaxeProvinciale", context.PPTaxeProvinciale.Count().ToString()},
               {"PPPoidsLivraisons", context.PPPoidsLivraisons.Count().ToString()},
               {"PPTypesLivraison", context.PPTypesLivraison.Count().ToString()},
               {"PPTypesPoids", context.PPTypesPoids.Count().ToString()},

               {"PPProduits", context.PPProduits.Count().ToString()},
               {"PPVendeurs", context.PPVendeurs.Count().ToString()},
               {"PPClients", context.PPClients.Count().ToString()},
               {"PPGestionnaires", context.PPGestionnaire.Count().ToString()},
                };
            }

            return countInsertion;
        }

        public List<dynamic[]> GetTableCellStrings(XElement table)
        {

            XNamespace ss = "urn:schemas-microsoft-com:office:spreadsheet";

            List<dynamic[]> tab = new List<dynamic[]>();
            var rows = table.Elements().Where(r => r.Name == ss + "Row");

            foreach (var row in rows.Skip(1))
            {
                var cells = row.Elements().Where(c => c.Name == ss + "Cell");

                dynamic[] tabObj = new dynamic[cells.Count()];

                int counter = 0;
                foreach (var cell in cells)
                {
                    tabObj[counter] = (cell.DescendantNodes().FirstOrDefault() as XElement)?.Value;
                    counter++;
                }

                tab.Add(tabObj);

            }

            return tab;
        }

        public DateTime? CheckDate(string strDate)
        {
            if (strDate != null && strDate != "")
                return DateTime.ParseExact(strDate.Substring(0, 10), "yyyy-mm-dd", null);
            else
                return null;
        }

        public long? CheckLong(string strNo)
        {
            if (long.TryParse(strNo, out long x))
                return x;

            return null;
        }

        public short? CheckShort(string strNo)
        {
            if (short.TryParse(strNo, out short x))
                return x;

            return null;
        }

        public decimal? CheckDecimal(string strNo)
        {
            if (decimal.TryParse(strNo, out decimal x))
                return x;

            return null;
        }

        public int? CheckInt(string strNo)
        {
            if (int.TryParse(strNo, out int x))
                return x;

            return null;
        }

        public ViewModels.XmlDataViewModel setBtnStatus(ViewModels.XmlDataViewModel model)
        {

            model.enableAddButton = true;

            for (var i = 0; i < model.tabCount.Length / 2; i++)
            {
                if (int.Parse(model.tabCount[i, 1]) != 0)
                    model.enableAddButton = false;
            }
            model.enableDeleteButton = !model.enableAddButton;

            return model;
        }
    }
}
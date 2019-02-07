using System;
using System.Collections.Generic;
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
         List<PPClients> lstClients = new List<PPClients>();
         List<PPVendeurs> lstVendeurs = new List<PPVendeurs>();

         XDocument doc = XDocument.Load(HostingEnvironment.MapPath("~/Content/xml2003Speadsheet.xml"));
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
                  
                  foreach(var table in tables)
                  {

                     /*
                      *   Table PPClients
                      */
                     if (sheet.Attribute(ss + "Name").Value == "PPClients")
                     {
                        var tabObjs = GetTableCellStrings(table);
                        foreach (var obj in tabObjs)
                        {
                           lstClients.Add(new PPClients()
                           {
                              NoClient = obj[1],
                              AdresseEmail = obj[2],
                              MotDePasse = obj[3],
                              Nom = obj[4],
                              Prenom = obj[5],
                              Rue = obj[6],
                              Ville = obj[7],
                              Province = obj[8],
                              CodePostal = obj[9],
                              Pays = obj[10],
                              Tel1 = obj[11],
                              Tel2 = obj[12],
                              DateCreation = obj[13],
                              DateMAJ = obj[14],
                              NbConnexions = obj[15],
                              DateDerniereConnexion = obj[16],
                              Statut = obj[17]
                           });
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
                           lstClients.Add(new PPClients()
                           {
                              NoClient = obj[1],
                              AdresseEmail = obj[2],
                              MotDePasse = obj[3],
                              Nom = obj[4],
                              Prenom = obj[5],
                              Rue = obj[6],
                              Ville = obj[7],
                              Province = obj[8],
                              CodePostal = obj[9],
                              Pays = obj[10],
                              Tel1 = obj[11],
                              Tel2 = obj[12],
                              DateCreation = obj[13],
                              DateMAJ = obj[14],
                              NbConnexions = obj[15],
                              DateDerniereConnexion = obj[16],
                              Statut = obj[17]
                           });
                        }
                     }


                  }
                  }

            }
         }
         return View();
      }


      [HttpPost]
      public ActionResult Index(int i)
      {



         return View();
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


   }
}
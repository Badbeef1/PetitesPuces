using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PetitesPuces.Controllers
{
   public class InternauteController : Controller
   {
      // GET: Inscription
      public ActionResult Index() => View("AccueilInternaute");
      
      public ActionResult AccueilInternaute()
      {
         /* Compare data with Database */
         Models.DataClasses1DataContext db = new Models.DataClasses1DataContext();
         db.Connection.Open();
         var categories = (from cat in db.GetTable<Models.PPCategories>() select cat);

         db.Connection.Close();

         return View(categories);
      }

      
      public ActionResult Inscription() => View();

      [HttpPost]
      public ActionResult VerifyEntry() => null;
      public ActionResult CatalogueNouveaute => View();

   }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PetitesPuces.Controllers
{
    public class InscriptionController : Controller
    {
        // GET: Inscription
        public ActionResult Index()
        {
            return View();
        }

       [HttpPost]
       public ActionResult VerifyEntry()
       {

          return null;
       } 

    }
}
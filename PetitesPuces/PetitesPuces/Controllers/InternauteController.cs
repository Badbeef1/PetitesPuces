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

        public ActionResult AccueilInternaute() => View();

        public ActionResult Inscription() => View();

        [HttpPost]
        public ActionResult VerifyEntry() => null;

    }
}
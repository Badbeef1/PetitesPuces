﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PetitesPuces.Controllers
{
    public class AdministrateurController : Controller
    {
        // GET: Administrateur
        public ActionResult GestionInactivite()
        {
            return View();
        }

       public ActionResult Statistiques()
       {
          return View();
       }
   }
}
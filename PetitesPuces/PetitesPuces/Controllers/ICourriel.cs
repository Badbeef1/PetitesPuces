using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetitesPuces.Controllers
{
    interface ICourriel
    {
        Models.PPMessages rechercheMessageParId(long id);
    }
}
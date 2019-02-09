using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;

namespace PetitesPuces.ViewModels
{
    public class CourrielVM
    {
        public List<Models.PPLieu> lstLieu { get; set; }
        public short lieu { get; set; }
        public IPagedList<Tuple<Models.PPDestinataires, String>> iplDestionataireBoiteReception { get; set; }
        
        public string addresseExpediteur { get; set; }
        public List<Models.PPDestinataires> lstDestinataires { get; set; }
        public string objetMessage { get; set; }
        public string messageCourriel { get; set; }

        public HttpPostedFileBase fichierJoint { get; set; }
    }
}
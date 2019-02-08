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
    }
}
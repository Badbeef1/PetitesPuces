using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PetitesPuces.Models
{
    public class InactiviteViewModel
    {
        public List<PPClients> clients { get; set; }
        public List<PPVendeurs> vendeurs { get; set; }

        public IList<SelectListItem> cbClients { get; set; }
        public IList<SelectListItem> cbVendeurs { get; set; }
    }



}
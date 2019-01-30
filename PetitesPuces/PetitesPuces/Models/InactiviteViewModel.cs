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

        public List<Inactiver> cbClients { get; set; }
        public List<Inactiver> cbVendeurs { get; set; }
    }


    public class Inactiver
    {
        public int ID { get; set; }
        public string idClient { get; set; }
        public bool IsSelected { get; set; }
    }



}
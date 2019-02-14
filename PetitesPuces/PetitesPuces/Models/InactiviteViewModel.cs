using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PetitesPuces.Models
{
    public class InactiviteViewModel
    {
        public List<Inactiver> cbClients { get; set; }
        public List<PPClients> lstClientsRetirer { get; set; }
        public List<Inactiver> cbVendeurs { get; set; }

        public String valDdlClient { get; set; }
        public String valDdlVendeur { get; set; }
        
    }


    public class Inactiver
    {
        public int ID { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string idClient { get; set; }
        public DateTime dernierPresence { get; set; }
        public bool IsSelected { get; set; }


    }



}
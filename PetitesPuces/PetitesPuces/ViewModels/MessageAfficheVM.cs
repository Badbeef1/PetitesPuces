using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetitesPuces.ViewModels
{
    public class MessageAfficheVM
    {
        public String StrNomAffichageDestinataire { get; set; }
        public String StrNomAffichageExpediteur { get; set; }
        public short ShrEtat { get; set; }
        public Models.PPDestinataires Destinataire { get; set; }
        public Models.PPMessages Message { get; set; }

        public DateTime dtDatePourTri { get; set; }
        public string strNomdestinataireExpediteurPourTri { get; set; }
    }
}
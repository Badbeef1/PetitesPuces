﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetitesPuces.ViewModels
{
    public class MessageAfficheVM
    {
        public String StrNomAffichage { get; set; }
        public short ShrEtat { get; set; }
        public Models.PPDestinataires Destinataire { get; set; }
        public Models.PPMessages Message { get; set; }
    }
}
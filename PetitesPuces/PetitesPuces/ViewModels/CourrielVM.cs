﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using PagedList;

namespace PetitesPuces.ViewModels
{
    public class CourrielVM
    {
        public List<Models.PPLieu> lstLieu { get; set; }
        public short lieu { get; set; }
        public IPagedList<MessageAfficheVM> iplListeMessageAffiche { get; set; }
        public Dictionary<short, int> dicNotificationLieu { get; set; }
        public MessageAfficheVM AfficheMessage { get; set; }
        public String strPage { get; set; }
        public string strTriActuel { get; set; }
        
        public List<Tuple<short, long, string, string, bool>> lstDestinataires { get; set; }

        [Required]
        public string addresseExpediteur { get; set; }
        [Required]
        public string objetMessage { get; set; }
        [Required]
        public string messageCourriel { get; set; }

        public string nomFichierJoin { get; set; }

        public int noMessageOuvert { get; set; }

        public string msgErreurCourriel { get; set; }
        public string msgSuccesCourriel { get; set; }

        public HttpPostedFileBase fichierJoint { get; set; }
    }
}
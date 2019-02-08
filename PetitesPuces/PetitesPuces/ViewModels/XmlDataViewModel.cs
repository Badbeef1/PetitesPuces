using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetitesPuces.ViewModels
{
    public class XmlDataViewModel
    {
        public string[,] tabCount { get; set; }
        public bool enableAddButton { get; set; }
        public bool enableDeleteButton { get; set; }
        public string errorMessage { get; set; }
    }
}
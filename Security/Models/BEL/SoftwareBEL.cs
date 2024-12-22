using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Security.Models.BEL
{
    public class SoftwareBEL 
    {
        public string SoftwareID { get; set; }
        public string SoftwareShortName { get; set; }
        public string SoftwareFullName { get; set; }
        public Boolean IsActive { get; set; }
    }
}

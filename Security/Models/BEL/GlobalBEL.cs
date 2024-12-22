using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Security.Models.BEL
{
   public class GlobalBEL
    {
        private String creator;
        private DateTime creationdate;
        private String modifier;
        private DateTime modificationdate;
        public String Creator
        {
            get { return creator; }
            set { creator = value; }
        }
        public DateTime CreationDate
        {
            get { return creationdate; }
            set { creationdate = value; }
        }
        public String Modifier
        {
            get { return modifier; }
            set { modifier = value; }
        }
        public DateTime ModificationDate
        {
            get { return modificationdate; }
            set { modificationdate = value; }
        }
    }
}

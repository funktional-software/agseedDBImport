using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace agSeedMarshaller
{
    class Product
    {
        public string id;
        public DateTime modDate;
        public string crop;
        public string name;
        public string tid;
        public string keyStrengths;
        public bool isNew;

        public Product(string id, DateTime modDate, string crop, string name, string keystrength, bool isNew)
        {
            this.crop = crop;
            this.id = id;
            this.modDate = modDate;
            this.name = name;
            this.keyStrengths = keystrength;
            this.isNew = isNew;
        }
    }
}

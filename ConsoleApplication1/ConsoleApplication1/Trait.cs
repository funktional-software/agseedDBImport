using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace agSeedMarshaller
{
    class Trait
    {
        public string id;
        public DateTime created;
        public DateTime modified;
        public string name;
        public string code;
        public string image;

        public string cropname { get; set; }
    }
}

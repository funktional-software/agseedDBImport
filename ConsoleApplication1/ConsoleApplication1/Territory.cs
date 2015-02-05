using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace agSeedMarshaller
{
    class Territory
    {
        public string id { get; set; }

        public string name { get; set; }

        public DateTime created { get; set; }

        public DateTime modified { get; set; }

        public string zone { get; set; }

        public string zipcode { get; set; }

        public string teamName { get; set; }

        public string guideId { get; set; }
    }
}

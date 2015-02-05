using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace agSeedMarshaller
{
    class User
    {

        public string id;
        public DateTime created;
        public DateTime modified;
        public string firstName;
        public string lastName;

        public string email;
        public string password;
        public string phone;
        public string address;

        public string profileImage;
        public string role;

        public User()
        {
            this.phone = string.Empty;
        }
    }
}

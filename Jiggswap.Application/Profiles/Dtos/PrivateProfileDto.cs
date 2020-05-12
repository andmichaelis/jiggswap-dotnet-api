using System;
using System.Collections.Generic;
using System.Text;

namespace Jiggswap.Application.Profiles.Dtos
{
    public class PrivateProfileDto : IAddress
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string StreetAddress { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Zip { get; set; }
    }
}

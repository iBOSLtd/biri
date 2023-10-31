using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class Url
    {
        public long IntUrlId { get; set; }
        public string StrDomainName { get; set; }
        public string StrUrl { get; set; }
        public string StrIpaddress { get; set; }
        public bool? IsActive { get; set; }
    }
}

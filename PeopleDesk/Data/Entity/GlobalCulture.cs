using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class GlobalCulture
    {
        public long IntId { get; set; }
        public long IntAccountId { get; set; }
        public string StrKey { get; set; }
        public string StrLanguage { get; set; }
        public string StrLabel { get; set; }
        public bool? IsActive { get; set; }
    }
}

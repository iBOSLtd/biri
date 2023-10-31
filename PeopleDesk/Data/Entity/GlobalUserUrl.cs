using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class GlobalUserUrl
    {
        public long IntAutoId { get; set; }
        public string StrLoginId { get; set; }
        public long IntUrlId { get; set; }
        public bool IsActive { get; set; }
    }
}

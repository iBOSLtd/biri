using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class BackgroundJobSuccessHistory
    {
        public long IntId { get; set; }
        public string StrModuleName { get; set; }
        public DateTime DteLastExcute { get; set; }
        public string StrStatus { get; set; }
    }
}

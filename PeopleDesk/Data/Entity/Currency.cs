using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class Currency
    {
        public long IntCurrencyId { get; set; }
        public string StrCurrency { get; set; }
        public string StrCurrencyCode { get; set; }
        public bool? IsActive { get; set; }
    }
}

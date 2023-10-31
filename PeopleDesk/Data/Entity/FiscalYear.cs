using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class FiscalYear
    {
        public long IntAutoId { get; set; }
        public long? IntYearId { get; set; }
        public string StrFiscalYear { get; set; }
        public DateTime DteFiscalFromDate { get; set; }
        public DateTime DteFiscalToDate { get; set; }
        public bool? IsActive { get; set; }
        public long? IntCreateBy { get; set; }
        public DateTime? DteCreatedAt { get; set; }
    }
}

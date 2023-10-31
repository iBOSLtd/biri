using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class MasterTaxChallanConfig
    {
        public long IntTaxChallanConfigId { get; set; }
        public int IntYear { get; set; }
        public DateTime DteFiscalFromDate { get; set; }
        public DateTime DteFiscalToDate { get; set; }
        public string StrCircle { get; set; }
        public string StrZone { get; set; }
        public string StrChallanNo { get; set; }
        public DateTime DteChallanDate { get; set; }
        public string StrBankName { get; set; }
        public long? IntBankId { get; set; }
        public long IntAccountId { get; set; }
        public long IntFiscalYearId { get; set; }
        public long? IntActionBy { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }
}

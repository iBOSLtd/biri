using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class EmpPfInvestmentHeader
    {
        public long IntInvenstmentHeaderId { get; set; }
        public long IntAccountId { get; set; }
        public long IntBusinessUnitId { get; set; }
        public DateTime DtePfPeriodFromMonthYear { get; set; }
        public DateTime DtePfPeriodToMonthYear { get; set; }
        public string StrInvestmentCode { get; set; }
        public string StrInvestmentReffNo { get; set; }
        public DateTime DteInvestmentDate { get; set; }
        public DateTime? DteMatureDate { get; set; }
        public decimal NumInterestRate { get; set; }
        public long? IntBankId { get; set; }
        public string StrBankName { get; set; }
        public long? IntBankBranchId { get; set; }
        public string StrBankBranchName { get; set; }
        public string StrRoutingNo { get; set; }
        public string StrAccountName { get; set; }
        public string StrAccountNumber { get; set; }
        public bool IsActive { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }
}

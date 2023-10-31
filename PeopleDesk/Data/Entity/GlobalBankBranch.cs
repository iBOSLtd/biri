using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class GlobalBankBranch
    {
        public long IntBankBranchId { get; set; }
        public string StrBankBranchCode { get; set; }
        public string StrBankBranchName { get; set; }
        public string StrBankBranchAddress { get; set; }
        public long? IntAccountId { get; set; }
        public long? IntDistrictId { get; set; }
        public long IntCountryId { get; set; }
        public long IntBankId { get; set; }
        public string StrBankName { get; set; }
        public string StrBankShortName { get; set; }
        public string StrBankCode { get; set; }
        public string StrRoutingNo { get; set; }
        public bool IsActive { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class AccountBankDetail
    {
        public long IntAccountBankDetailsId { get; set; }
        public long IntAccountId { get; set; }
        public long? IntBusinessUnitId { get; set; }
        public long IntBankOrWalletType { get; set; }
        public long IntBankWalletId { get; set; }
        public string StrBankWalletName { get; set; }
        public string StrDistrict { get; set; }
        public long? IntBankBranchId { get; set; }
        public string StrBranchName { get; set; }
        public string StrRoutingNo { get; set; }
        public string StrSwiftCode { get; set; }
        public string StrAccountName { get; set; }
        public string StrAccountNo { get; set; }
        public bool? IsActive { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }
}

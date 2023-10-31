using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class BankWallet
    {
        public long IntBankWalletId { get; set; }
        public string StrBankWalletName { get; set; }
        public string StrShortName { get; set; }
        public string StrCode { get; set; }
        public long IntBankOrWalletType { get; set; }
        public bool? IsActive { get; set; }
    }
}

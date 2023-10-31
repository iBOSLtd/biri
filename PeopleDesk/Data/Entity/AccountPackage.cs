using System;
using System.Collections.Generic;

namespace PeopleDesk.Data.Entity
{
    public partial class AccountPackage
    {
        public long IntAccountPackageId { get; set; }
        public string StrAccountPackageName { get; set; }
        public long IntMinEmployee { get; set; }
        public long IntMaxEmployee { get; set; }
        public long IntExpireInDays { get; set; }
        public decimal NumPrice { get; set; }
        public decimal? NumFileStorageQuaota { get; set; }
        public bool IsFree { get; set; }
        public bool? IsActive { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }
}

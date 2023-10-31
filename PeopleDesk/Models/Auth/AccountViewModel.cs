using PeopleDesk.Data.Entity;

namespace PeopleDesk.Models.Auth
{
    public class AccountViewModel
    {

        public Account? Account { get; set; }
        public AccountPackage? AccountPackage { get; set; }
        public Url? Url { get; set; }
    }

    public class AccountCreateViewModel
    {
        public long IntUrlId { get; set; }
        public string? StrDomainName { get; set; } = null!;

        public long IntAccountPackageId { get; set; }
        public string? StrAccountPackageName { get; set; } = null!;
        public long IntMinEmployee { get; set; }
        public long IntMaxEmployee { get; set; }
        public long IntExpireInDays { get; set; }
        public decimal NumPrice { get; set; }
        public decimal? NumFileStorageQuaota { get; set; }
        public bool IsFree { get; set; }

        public long IntAccountId { get; set; }
        public string StrAccountName { get; set; } = null!;
        public string? StrShortCode { get; set; }
        public string StrOwnerName { get; set; } = null!;
        public string? StrAddress { get; set; }
        public string StrMobileNumber { get; set; } = null!;
        public string? StrNid { get; set; }
        public string? StrBin { get; set; }
        public string? StrEmail { get; set; }
        public string? StrWebsite { get; set; }
        public long? IntLogoUrlId { get; set; }
        public long? IntCountryId { get; set; }
        public string StrCurrency { get; set; }
        public bool IsBlock { get; set; }

        public bool? IsTax { get; set; }
        public bool? IsProvidentFund { get; set; }
        public bool? IsLoan { get; set; }

        public bool? IsActive { get; set; }
        public DateTime DteExpireDate { get; set; }
        public decimal? NumPackageFileStorageQuaota { get; set; }
        public DateTime DteDateOfOnboard { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
        public bool IsLoggedInWithOtp { get; set; } = false;

    }

}
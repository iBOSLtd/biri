namespace PeopleDesk.Models.MasterData
{
    public class MasterBusinessUnitDWithAccount
    {
        public long IntBusinessUnitId { get; set; }
        public string StrBusinessUnit { get; set; } = null!;
        public string? StrShortCode { get; set; }
        public string? StrBusinessUnitAddress { get; set; }
        public long? StrBusinessUnitLogoUrlId { get; set; }
        public long? IntDistrictId { get; set; }
        public string? StrDistrict { get; set; }
        public string? StrEmail { get; set; }
        public string? StrWebsiteUrl { get; set; }
        public string? StrCurrency { get; set; }
        public bool? IsActive { get; set; }
        public long IntAccountId { get; set; }
        public string StrAccountName { get; set; } = null!;
        public string? StrAccountAddress { get; set; }
        public long? IntAccountLogoUrlId { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }
}

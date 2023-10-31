namespace PeopleDesk.Data.Entity
{
    public partial class PyrSalaryGenerateRequest
    {
        public long IntSalaryGenerateId { get; set; }
        public string? StrSalaryGenerateCode { get; set; }
        public long? IntAccountId { get; set; }
        public long? IntBusinessUnitId { get; set; }
        public string? StrBusinessUnit { get; set; }
        public int? IntWorkplaceGroupId { get; set; }
        public string? StrWorkplaceGroup { get; set; }
        public int? IntWorkplace { get; set; }
        public string? StrWorkplace { get; set; }
        public int? IntPayrollGroupId { get; set; }
        public string? StrPayrollGroup { get; set; }
        public int? IntMonthId { get; set; }
        public int? IntYearId { get; set; }
        public DateTime? DtePayrollGenerateFrom { get; set; }
        public DateTime? DtePayrollGenerateTo { get; set; }
        public decimal? NumNetPayable { get; set; }
        public bool? IsProcessingStatus { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime DteCreatedDateTime { get; set; }
        public bool? IsApprove { get; set; }
        public long? IntApprovedBy { get; set; }
        public DateTime? DteApprovedDateTime { get; set; }
        public bool? IsReject { get; set; }
        public string? IntRejectBy { get; set; }
        public DateTime? DteRejectDateTime { get; set; }
    }
}

namespace PeopleDesk.Data.Entity
{
    public partial class PyrPayrollElement
    {
        public int IntPayrollElementId { get; set; }
        public string? StrPayrollElementName { get; set; }
        public string? StrPayrollElementCode { get; set; }
        public int? IntPayrollElementTypeId { get; set; }
        public string? StrPayrollElementTypeCode { get; set; }
        public long IntAccountId { get; set; }
        public long IntBusinessUnitId { get; set; }
        public bool? IsActive { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }
}

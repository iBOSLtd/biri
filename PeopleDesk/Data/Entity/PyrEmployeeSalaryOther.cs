namespace PeopleDesk.Data.Entity
{
    public partial class PyrEmployeeSalaryOther
    {
        public long IntEmployeeSalaryOtherId { get; set; }
        public long IntEmployeeId { get; set; }
        public long IntPayrollElementId { get; set; }
        public string StrPayrollElementTypeCode { get; set; } = null!;
        public decimal? NumAmount { get; set; }
        public decimal? ReqNumAmount { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }
}

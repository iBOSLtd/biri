namespace PeopleDesk.Data.Entity
{
    public partial class ElezableForJobConfig
    {
        public long IntJobConfigId { get; set; }
        public long IntAccountId { get; set; }
        public long IntFromEmploymentTypeId { get; set; }
        public long? IntToEmploymentTypeId { get; set; }
        public long IntServiceLengthDays { get; set; }
        public bool IsActive { get; set; }
        public long IntCreatedBy { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
    }
}

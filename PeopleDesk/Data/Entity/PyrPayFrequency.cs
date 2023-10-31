namespace PeopleDesk.Data.Entity
{
    public partial class PyrPayFrequency
    {
        public long IntPayFrequencyId { get; set; }
        public string? StrPayFrequencyName { get; set; }
        public string? StrPayFrequencyCode { get; set; }
        public bool? IsActive { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
    }
}

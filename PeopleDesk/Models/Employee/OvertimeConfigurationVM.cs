namespace PeopleDesk.Models.Employee
{
    public class OvertimeConfigurationVM
    {
    }
    public partial class GetOverTimeConfigurationVM
    {
        public long IntOtconfigId { get; set; }
        public long IntAccountId { get; set; }
        public bool? IsOvertimeAutoCalculate { get; set; }
        public long IntOtdependOn { get; set; }
        public decimal? NumFixedAmount { get; set; }
        public long IntOverTimeCountFrom { get; set; }
        public long? IntOtbenefitsHour { get; set; }
        public long? IntMaxOverTimeDaily { get; set; }
        public long? IntMaxOverTimeMonthly { get; set; }
        public long? IntOtcalculationShouldBe { get; set; }
        public long? IntOtAmountShouldBe { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? DteCreateAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedDate { get; set; }
        public long? IntUpdatedBy { get; set; }
        public long? Value { get; set; }
        public string? Label { get; set; }
    }
}

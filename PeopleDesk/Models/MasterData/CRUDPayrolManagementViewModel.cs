namespace PeopleDesk.Models.MasterData
{
    public class CRUDPayrolManagementViewModel
    {
        // ================= payroll group  ============================
        public string PartType { get; set; }
        public long AutoId { get; set; }
        public string PayrollElementName { get; set; }
        public string PayrollElementCode { get; set; }
        public long PayrollElementTypeId { get; set; }
        public string PayrollElementTypeCode { get; set; }
        public bool isActive { get; set; }
        public long IntCreatedBy { get; set; }
        public long IntAccountId { get; set; }

        /// ================== payroll grade ============================
        public string PayrollGradeName { get; set; }
        public decimal LowerLimit { get; set; }
        public decimal UpperLimit { get; set; }

        // ================= payroll group  ============================
        public string PayrollGroupName { get; set; }
        public string PayrollGroupCode { get; set; }
        public string StartDateOfMonth { get; set; }
        public string EndDateOfMonth { get; set; }
        public string PayFrequencyName { get; set; }
        public long PayFrequencyId { get; set; }
    }
}

namespace PeopleDesk.Models.MasterData
{
    public class OrganogramViewModel
    {
    }
    public class OrganogramTreeViewModel
    {
        public long AutoId { get; set; }
        public long ParentId { get; set; }
        public long Sequence { get; set; }
        public long? PositionId { get; set; }
        public string PositionName { get; set; }
        public long? EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeNameDetails { get; set; }
        public long? DesignationId { get; set; }
        public string? DesignationName { get; set; }
        public long? EmployeeImageUrlId { get; set; }
        public string? Email { get; set; }
        public List<OrganogramTreeViewModel> ChildList { get; set; }
    }
    public class OrganogramReConstructViewModel
    {
        public long AutoId { get; set; }
        public long ParentId { get; set; }
        public long Sequence { get; set; }
        public long? PositionId { get; set; }
        public string PositionName { get; set; }
        public long? EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public long CreatedBy { get; set; }
        public long BusinessUnitId { get; set; }
        public bool IsActive { get; set; }
    }
}

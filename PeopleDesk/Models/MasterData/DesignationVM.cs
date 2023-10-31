namespace PeopleDesk.Models.MasterData
{
    public class DesignationVM
    {
        public long IntDesignationId { get; set; }
        public string StrDesignation { get; set; } = null!;
        public string? StrDesignationCode { get; set; }
        public long? IntPositionId { get; set; }
        public bool? IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public long?[] IntBusinessUnitIdList { get; set; }
        public long?[] IntUserRoleIdList { get; set; }
        public List<BusinessUnitValuLabelVM>? BusinessUnitValuLabelVMList { get; set; }
        public List<RoleValuLabelVM>? RoleValuLabelVMList { get; set; }
        public long IntAccountId { get; set; }
        public DateTime DteCreatedAt { get; set; }
        public long? IntCreatedBy { get; set; }
        public DateTime? DteUpdatedAt { get; set; }
        public long? IntUpdatedBy { get; set; }
        public long? IntPayscaleGradeId { get; set; }
        public string? StrPayscaleGradeName { get; set; }
    }

    public class BusinessUnitValuLabelVM
    {
        public long Value { get; set; }
        public string Label { get; set; }
    }
    public class RoleValuLabelVM
    {
        public long Value { get; set; }
        public string Label { get; set; }
    }
    public class RoleAssignToUserVM
    {
        public long? AccountId { get; set; }
        public long? BusinessUnitId { get; set; }
        public long? WorkplaceGroupId { get; set; }
        public long? EmployeeId { get; set; }
        public long?[] RoleIdList { get; set; }
        public long? UpdatedBy { get; set; }
    }


}

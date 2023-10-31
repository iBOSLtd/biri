namespace PeopleDesk.Models.Employee
{
    public class EmployeeLandingVM
    {
    }

    public class EmployeeBasicForConfirmationVM
    {
        public long EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public DateTime? ConfirmationDateRaw { get; set; }
        public long? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public long? DesignationId { get; set; }
        public string DesignationName { get; set; }
        public DateTime? JoiningDate { get; set; }
        public long? SupervisorId { get; set; }
        public string? SupervisorName { get; set;}
        public long? LineManagerId { get; set;}
        public string? LineManager { get; set;}
        public long? WorkplaceGroupId { get; set;}
        public string? WorkplaceGroupName { get; set;}
        public long? WorkplaceId { get; set;}
        public string? WorkplaceName { get; set;}
        public long? EmploymentStatusId { get; set; }
        public string EmploymentStatus { get;set; }
        public long? ProfilePicUrl { get; set;}
        public long BusinessUnitId { get;set;}
        public long AccountId { get;set;}
        public long? PayrollGroupId { get; set;}
        public string? PayrollGroupName { get; set;}
        public long? PayscaleGradeId { get; set;}
        public string? PayscaleGradeName { get;set;}
        public string? ConfirmationDate { get;set;}
        public DateTime? DateOfBirth { get; set;}
        public long? EmploymentTypeId { get; set;}
        public string? EmploymentType { get; set;}
        public long? WingId { get; set;}
        public string? WingName { get; set;}
        public long? SoleDepoId { get; set;}
        public string? SoleDepoName { get; set;}
        public long? RegionId { get; set;}
        public string? RegionName { get; set;}
        public long? AreaId { get; set;}
        public string? AreaName { get; set;}
        public long? TerritoryId { get; set;}
        public string? TerritoryName { get; set;}
        public DateTime? dteInternCloseDate { get; set;}
        public DateTime? dteProbationaryCloseDate { get; set;}
        public string? PinNo { get; set; }
        public string? JoiningDateFormated { get; set;}
        public string? ServiceLength { get; set;}
        public string? ConfirmationStatus { get; set;}

    }

    public class ConfirmationEmployeeLandingPaginationViewModelWithHeader
    {
        public dynamic CurrentPage { get; set; }
        public dynamic TotalCount { get; set; }
        public dynamic PageSize { get; set; }
        public dynamic Data { get; set; }
        //public ConfirmationEmployeeHeader EmployeeHeader { get; set; }
    }
    public class ConfirmationEmployeeHeader
    {
        public dynamic StrDesignationList { get; set; }
        public dynamic StrDepartmentList { get; set; }
        public dynamic StrSupervisorNameList { get; set; }
        public dynamic StrLinemanagerList { get; set; }
        public dynamic StrEmploymentTypeList { get; set; }
        public dynamic strWingList { get; set; }
        public dynamic StrSoleDepoList { get; set; }
        public dynamic StrRegionList { get; set; }
        public dynamic StrAreaList { get; set; }
        public dynamic StrTerritoryList { get; set; }
    }
    public class InactiveEmployeeListLanding : PaginationBaseVM
    {
        public dynamic Data { get; set; }
        public EmployeeHeader EmployeeHeader { get; set; }
    }
    public class InactiveEmployeeListLandingVM
    {
        public long? intAccountId { get; set; }
        public long? intBusinessUnitId { get; set; }
        public string? BusinessUnit { get; set; }
        public long? intWorkplaceGroupId { get; set; }
        public long? intEmployeeId { get; set; }
        public string StrEmployeeCode { get; set; }
        public string StrEmployeeName { get; set; }
        public long? IntDesignationId { get; set; }
        public string? StrDesignation { get; set; }
        public long? IntDepartmentId { get; set; }
        public string? StrDepartment { get; set; }
        public string? StrWorkplaceGroup { get; set; }
        public string? StrWorkplace { get; set; }
        public string? DteJoiningDate { get; set; }
        public string? ServiceLength { get; set; }
        public bool? isActive { get; set; }
        public string StrStatus { get; set; }
    }
}

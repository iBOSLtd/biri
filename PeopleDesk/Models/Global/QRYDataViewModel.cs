namespace PeopleDesk.Models.Global
{
    public class QRYDataViewModel
    {
    }
    public class CustomDTO
    {
        public long? IntEmployeeId { get; set; }
        public long? IntEmploymentTypeId { get; set; }
        public string? EmploymentTypeName { get; set; }
        public string? StrEmployeeName { get; set; }
        public string? StrEmployeeCode { get; set; }
        public long? IntBusinessUnitId { get; set; }
        public string? StrBusinessUnitName { get; set; }
        public long? IntWorkplaceGroupId { get; set; }
        public string? StrWorkplaceGroupName { get; set; }
        public long? IntDepartmentId { get; set; }
        public string? StrDepartmentName { get; set; }
        public long? IntDesignationId { get; set; }
        public string? StrDesignationName { get; set; }
        public long? IntSupervisorId { get; set; }
        public string? SupervisorName { get; set; }
    }
    public class CustomDTO2
    {
        public long? IntBusinessUnitId { get; set; }
        public string? StrBusinessUnitName { get; set; }
        public long? IntWorkplaceGroupId { get; set; }
        public string? StrWorkplaceGroupName { get; set; }
        public long? IntWorkplaceId { get; set; }
        public string? StrWorkplaceName { get; set; }
        public long? IntDepartmentId { get; set; }
        public string? StrDepartmentName { get; set; }
        public long? IntDesignationId { get; set; }
        public string? StrDesignationName { get; set; }
        public long? IntEmploymentTypeId { get; set; }
        public string? EmploymentTypeName { get; set; }
    }
    public class CombineDataSetByWorkplaceGDeptDesigSupEmpType
    {
        public List<WorkplaceGroupList> WorkplaceGroupList { get; set; }
        public List<DepartmentList> DepartmentList { get; set; }
        public List<DesignationList> DesignationList { get; set; }
        public List<SupervisorList> SupervisorList { get; set; }
        public List<EmploymentTypeList> EmploymentTypeList { get; set; }
        public List<EmployeeList> EmployeeList { get; set; }
    }
    public class CombineDataSetByWorkplaceGDeptDesigSupEmpTypeByAccountId
    {
        public List<BusinessUnitList> BusinessUnitList { get; set; }
        public List<WorkplaceGroupList> WorkplaceGroupList { get; set; }
        public List<WorkplaceList> WorkplaceList { get; set; }
        public List<DepartmentList> DepartmentList { get; set; }
        public List<DesignationList> DesignationList { get; set; }
        public List<EmploymentTypeList> EmploymentTypeList { get; set; }
    }
    public class BusinessUnitList
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
    public class WorkplaceGroupList
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
    public class WorkplaceList
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
    public class DepartmentList
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
    public class DesignationList
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
    public class EmploymentTypeList
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
    public class SupervisorList
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
    public class EmployeeList
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }

}

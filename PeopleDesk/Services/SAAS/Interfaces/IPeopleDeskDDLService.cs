using System.Data;
namespace PeopleDesk.Services.SAAS.Interfaces
{
    public interface IPeopleDeskDDLService
    {
        Task<DataTable> PeopleDeskAllDDL(string? DDLType, long? AccountId, long? BusinessUnitId, long? WorkplaceGroupId, long? intId, string? AutoEmployeeCode, bool? IsView, int? IntMonth, long? IntYear, long? IntWorkplaceId, long? IntPayrollGroupId, long? ParentTerritoryId, string? SearchTxt);
        Task<DataTable> PeopleDeskAllLanding(string? TableName, long? AccountId, long? BusinessUnitId, long? intId, long? intStatusId, DateTime? FromDate, DateTime? ToDate,
            long? LoanTypeId, long? DeptId, long? DesigId, long? EmpId, long? MinimumAmount, long? MaximumAmount, long? MovementTypeId, DateTime? ApplicationDate, int? StatusId, long? LoggedEmployeeId);
    }
}

using PeopleDesk.Models;
using PeopleDesk.Models.Employee;
using System.Data;

namespace PeopleDesk.Services.SAAS.Interfaces
{
    public interface ILeaveMovementService
    {
        #region ===> LEAVE <===
        Task<DataTable> GetEmployeeLeaveBalanceAndHistory(long? EmployeeId, string? ViewType, long? LeaveTypeId, DateTime? ApplicationDate, long? IntYear, int? StatusId);
        Task<List<LeaveDataSetDTO>> GetAllLeaveApplicatonListForApprove(LeaveDTO model);
        Task<LeaveDataSetDTO> GetSingleLeaveApplicatonListForApprove(long? ViewType, long? EmployeeId, long? ApplicationId);
        Task<MessageHelper> CRUDLeaveApplication(LeaveApplicationDTO obj);
        Task<MessageHelper> LeaveApprove(List<LeaveAndMovementApprovedDTO> obj);
        #endregion

        #region ===< MOVEMENT >===
        Task<MessageHelper> CRUDMovementApplication(MovementApplicationDTO obj);
        Task<List<MovementDataSetDTO>> GetAllMovementApplicatonListForApprove(LeaveDTO model);
        Task<MovementDataSetDTO> GetSingleMovementApplicatonListForApprove(long? ViewType, long? EmployeeId, long? MovementId);
        Task<MessageHelper> MovementApprove(List<LeaveAndMovementApprovedDTO> obj);
        #endregion

        #region ===> LEAVE MOVEMENT TYPE <===
        Task<MessageHelper> CRUDLeaveMovementType(LeaveMovementTypeDTO obj);
        #endregion

        #region ===> Employment Type Wise Leave Balance <===
        Task<MessageHelper> CRUDEmploymentTypeWiseLeaveBalance(CRUDEmploymentTypeWiseLeaveBalanceDTO obj);
        #endregion
    }
}

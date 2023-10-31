using PeopleDesk.Data.Entity;
using PeopleDesk.Models.Employee;
using PeopleDesk.Services.Helper.Interfaces;

namespace PeopleDesk.Services.Helper
{
    public class ConvertEntityToViewModelService : IConvertEntityToViewModelService
    {

        public async Task<EmployeeBasicInfoViewModel> ConvertEmployeeBasicInfo(EmpEmployeeBasicInfo obj)
        {
            EmployeeBasicInfoViewModel model = new EmployeeBasicInfoViewModel
            {
                IntEmployeeBasicInfoId = obj.IntEmployeeBasicInfoId,
                StrEmployeeCode = obj.StrEmployeeCode,
                StrCardNumber = obj.StrCardNumber,
                StrEmployeeName = obj.StrEmployeeName,
                IntGenderId = obj.IntGenderId,
                StrGender = obj.StrGender,
                IntReligionId = obj.IntReligionId,
                StrMaritalStatus = obj.StrMaritalStatus,
                StrBloodGroup = obj.StrBloodGroup,
                IntDepartmentId = obj.IntDepartmentId,
                IntDesignationId = obj.IntDesignationId,
                DteDateOfBirth = obj.DteDateOfBirth,
                DteJoiningDate = obj.DteJoiningDate,
                DteConfirmationDate = obj.DteConfirmationDate,
                DteLastWorkingDate = obj.DteLastWorkingDate,
                IntSupervisorId = obj.IntSupervisorId,
                IntLineManagerId = obj.IntLineManagerId,
                IntDottedSupervisorId = obj.IntDottedSupervisorId,
                IsSalaryHold = obj.IsSalaryHold,
                IsActive = obj.IsActive,
                IsUserInactive = obj.IsUserInactive,
                IsRemoteAttendance = obj.IsRemoteAttendance,
                IntEmploymentTypeId = obj.IntEmploymentTypeId,
                StrEmploymentType= obj.StrEmploymentType,
                IntWorkplaceId = obj.IntWorkplaceId,
                IntWorkplaceGroupId = (long)obj.IntWorkplaceGroupId,
                IntAccountId = obj.IntAccountId,
                IntBusinessUnitId = obj.IntBusinessUnitId,
                DteCreatedAt = obj.DteCreatedAt,
                IntCreatedBy = obj.IntCreatedBy,
                DteUpdatedAt = obj.DteUpdatedAt,
                IntUpdatedBy = obj.IntUpdatedBy
            };

            return model;
        }
    }
}

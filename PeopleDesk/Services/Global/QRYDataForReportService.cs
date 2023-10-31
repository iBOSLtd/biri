using Microsoft.EntityFrameworkCore;
using PeopleDesk.Data;
using PeopleDesk.Models.Employee;
using PeopleDesk.Services.Global.Interface;

namespace PeopleDesk.Services.Global
{
    public class QRYDataForReportService : IQRYDataForReportService
    {
        private readonly PeopleDeskContext _contex;

        public QRYDataForReportService(PeopleDeskContext _contex)
        {
            this._contex = _contex;
        }

        public async Task<EmployeeQryProfileAllViewModel> EmployeeQryProfileAll(long intEmployeeId)
        {

            EmployeeQryProfileAllViewModel data = await (from emp in _contex.EmpEmployeeBasicInfos
                                                         where emp.IntEmployeeBasicInfoId == intEmployeeId
                                                         join d in _contex.MasterDepartments on emp.IntDepartmentId equals d.IntDepartmentId into dd
                                                         from dept in dd.DefaultIfEmpty()
                                                         join de in _contex.MasterDesignations on emp.IntDesignationId equals de.IntDesignationId into dess
                                                         from des in dess.DefaultIfEmpty()
                                                         join s in _contex.EmpEmployeeBasicInfos on emp.IntSupervisorId equals s.IntEmployeeBasicInfoId into ss
                                                         from sup in ss.DefaultIfEmpty()
                                                         join dotteds in _contex.EmpEmployeeBasicInfos on emp.IntDottedSupervisorId equals dotteds.IntEmployeeBasicInfoId into dottedss
                                                         from dottedSup in dottedss.DefaultIfEmpty()
                                                         join l in _contex.EmpEmployeeBasicInfos on emp.IntLineManagerId equals l.IntEmployeeBasicInfoId into ll
                                                         from man in ll.DefaultIfEmpty()
                                                         join et in _contex.MasterEmploymentTypes on emp.IntEmploymentTypeId equals et.IntEmploymentTypeId into ett
                                                         from empT in ett.DefaultIfEmpty()
                                                         join b in _contex.MasterBusinessUnits on emp.IntBusinessUnitId equals b.IntBusinessUnitId into bb
                                                         from bus in bb.DefaultIfEmpty()

                                                         join wg in _contex.MasterWorkplaces on emp.IntWorkplaceId equals wg.IntWorkplaceId into pp
                                                         from wppg in pp.DefaultIfEmpty()

                                                         join w in _contex.MasterWorkplaceGroups on wppg.IntWorkplaceGroupId equals w.IntWorkplaceGroupId into ww
                                                         from wpg in ww.DefaultIfEmpty()

                                                         join DSupPhoto1 in _contex.EmpEmployeePhotoIdentities on emp.IntDottedSupervisorId equals DSupPhoto1.IntEmployeeBasicInfoId into DSupPhoto2
                                                         from DSupPhoto in DSupPhoto2.DefaultIfEmpty()
                                                         join supPhoto1 in _contex.EmpEmployeePhotoIdentities on emp.IntSupervisorId equals supPhoto1.IntEmployeeBasicInfoId into supPhoto2
                                                         from supPhoto in supPhoto2.DefaultIfEmpty()
                                                         join manPhoto1 in _contex.EmpEmployeePhotoIdentities on emp.IntLineManagerId equals manPhoto1.IntEmployeeBasicInfoId into manPhoto2
                                                         from manPhoto in manPhoto2.DefaultIfEmpty()

                                                         select new EmployeeQryProfileAllViewModel
                                                         {
                                                             EmployeeBasicInfo = emp,
                                                             BusinessUnit = bus,
                                                             DepartmentId = dept.IntDepartmentId,
                                                             DepartmentName = dept.StrDepartment,
                                                             DesignationId = des.IntDesignationId,
                                                             DesignationName = des.StrDesignation,
                                                             SupervisorId = sup.IntEmployeeBasicInfoId,
                                                             SupervisorName = sup.StrEmployeeName,
                                                             IntSupervisorImageUrlId = supPhoto == null ? 0 : supPhoto.IntProfilePicFileUrlId,
                                                             DottedSupervisorId = emp.IntDottedSupervisorId,
                                                             DottedSupervisorName = dottedSup.StrEmployeeName,
                                                             IntDottedSupervisorImageUrlId = DSupPhoto == null ? 0 : DSupPhoto.IntProfilePicFileUrlId,
                                                             LineManagerId = man.IntEmployeeBasicInfoId,
                                                             LineManagerName = man.StrEmployeeName,
                                                             IntLineManagerImageUrlId = manPhoto == null ? 0 : manPhoto.IntProfilePicFileUrlId,
                                                             EmploymentTypeId = emp.IntEmploymentTypeId,
                                                             EmploymentTypeName = empT.StrEmploymentType,
                                                             WorkplaceGroupId = wpg.IntWorkplaceGroupId,
                                                             WorkplaceGroupName = wpg.StrWorkplaceGroup

                                                         }).AsNoTracking().AsQueryable().FirstOrDefaultAsync();

            return data;


        }


        public async Task<List<EmployeeQryProfileAllViewModel>> EmployeeQryProfileAllList(long? BusinessUnitId, long? WorkplaceGroupId, long? DeptId, long? DesigId, long? EmployeeId)
        {

            List<EmployeeQryProfileAllViewModel> data = await (from emp in _contex.EmpEmployeeBasicInfos
                                                               join d in _contex.MasterDepartments on emp.IntDepartmentId equals d.IntDepartmentId into dd
                                                               from dept in dd.DefaultIfEmpty()
                                                               join de in _contex.MasterDesignations on emp.IntDesignationId equals de.IntDesignationId into dess
                                                               from des in dess.DefaultIfEmpty()
                                                               join s in _contex.EmpEmployeeBasicInfos on emp.IntSupervisorId equals s.IntEmployeeBasicInfoId into ss
                                                               from sup in ss.DefaultIfEmpty()
                                                               join l in _contex.EmpEmployeeBasicInfos on emp.IntLineManagerId equals l.IntEmployeeBasicInfoId into ll
                                                               from man in ll.DefaultIfEmpty()
                                                               join et in _contex.MasterEmploymentTypes on emp.IntEmploymentTypeId equals et.IntEmploymentTypeId into ett
                                                               from empT in ett.DefaultIfEmpty()
                                                               join b in _contex.MasterBusinessUnits on emp.IntBusinessUnitId equals b.IntBusinessUnitId into bb
                                                               from bus in bb.DefaultIfEmpty()

                                                               join wg in _contex.MasterWorkplaces on emp.IntWorkplaceId equals wg.IntWorkplaceId into pp
                                                               from wppg in pp.DefaultIfEmpty()

                                                               join w in _contex.MasterWorkplaceGroups on wppg.IntWorkplaceGroupId equals w.IntWorkplaceGroupId into ww
                                                               from wpg in ww.DefaultIfEmpty()
                                                               where emp.IsActive == true &&
                                                               (BusinessUnitId > 0 ? emp.IntBusinessUnitId == BusinessUnitId : true) &&
                                                               (WorkplaceGroupId > 0 ? wppg.IntWorkplaceGroupId == WorkplaceGroupId : true) &&
                                                               (DeptId > 0 ? emp.IntDepartmentId == DeptId : true) &&
                                                               (DesigId > 0 ? emp.IntDesignationId == DesigId : true) &&
                                                               (EmployeeId > 0 ? emp.IntEmployeeBasicInfoId == EmployeeId : true)
                                                               select new EmployeeQryProfileAllViewModel
                                                               {
                                                                   EmployeeBasicInfo = emp,
                                                                   BusinessUnit = bus,
                                                                   DepartmentId = dept.IntDepartmentId,
                                                                   DepartmentName = dept.StrDepartment,
                                                                   DesignationId = des.IntDesignationId,
                                                                   DesignationName = des.StrDesignation,
                                                                   SupervisorId = sup.IntEmployeeBasicInfoId,
                                                                   SupervisorName = sup.StrEmployeeName,
                                                                   LineManagerId = man.IntEmployeeBasicInfoId,
                                                                   LineManagerName = man.StrEmployeeName,
                                                                   EmploymentTypeId = emp.IntEmploymentTypeId,
                                                                   EmploymentTypeName = empT.StrEmploymentType,
                                                                   WorkplaceGroupId = wpg.IntWorkplaceGroupId,
                                                                   WorkplaceGroupName = wpg.StrWorkplaceGroup

                                                               }).AsNoTracking().ToListAsync();

            return data;


        }

        public async Task<List<EmployeeQryProfileAllViewModel>> EmployeeQryProfileAllListBySupervisorORLineManagerId(long? SupervisorORLineManagerEmployeeId)
        {

            List<EmployeeQryProfileAllViewModel> data = await (from emp in _contex.EmpEmployeeBasicInfos
                                                               join d in _contex.MasterDepartments on emp.IntDepartmentId equals d.IntDepartmentId into dd
                                                               from dept in dd.DefaultIfEmpty()
                                                               join de in _contex.MasterDesignations on emp.IntDesignationId equals de.IntDesignationId into dess
                                                               from des in dess.DefaultIfEmpty()
                                                               join s in _contex.EmpEmployeeBasicInfos on emp.IntSupervisorId equals s.IntEmployeeBasicInfoId into ss
                                                               from sup in ss.DefaultIfEmpty()
                                                               join dotteds in _contex.EmpEmployeeBasicInfos on emp.IntDottedSupervisorId equals dotteds.IntEmployeeBasicInfoId into dottedss
                                                               from dottedSup in dottedss.DefaultIfEmpty()
                                                               join l in _contex.EmpEmployeeBasicInfos on emp.IntLineManagerId equals l.IntEmployeeBasicInfoId into ll
                                                               from man in ll.DefaultIfEmpty()
                                                               join et in _contex.MasterEmploymentTypes on emp.IntEmploymentTypeId equals et.IntEmploymentTypeId into ett
                                                               from empT in ett.DefaultIfEmpty()
                                                               join b in _contex.MasterBusinessUnits on emp.IntBusinessUnitId equals b.IntBusinessUnitId into bb
                                                               from bus in bb.DefaultIfEmpty()

                                                               join wg in _contex.MasterWorkplaces on emp.IntWorkplaceId equals wg.IntWorkplaceId into pp
                                                               from wppg in pp.DefaultIfEmpty()

                                                               join w in _contex.MasterWorkplaceGroups on wppg.IntWorkplaceGroupId equals w.IntWorkplaceGroupId into ww
                                                               from wpg in ww.DefaultIfEmpty()

                                                               where emp.IsActive == true
                                                               && (emp.IntEmployeeBasicInfoId == SupervisorORLineManagerEmployeeId
                                                                    || sup.IntEmployeeBasicInfoId == SupervisorORLineManagerEmployeeId
                                                                    || dottedSup.IntEmployeeBasicInfoId == SupervisorORLineManagerEmployeeId
                                                                    || man.IntEmployeeBasicInfoId == SupervisorORLineManagerEmployeeId)
                                                               select new EmployeeQryProfileAllViewModel
                                                               {
                                                                   EmployeeBasicInfo = emp,
                                                                   BusinessUnit = bus,
                                                                   DepartmentId = dept.IntDepartmentId,
                                                                   DepartmentName = dept.StrDepartment,
                                                                   DesignationId = des.IntDesignationId,
                                                                   DesignationName = des.StrDesignation,
                                                                   SupervisorId = sup.IntEmployeeBasicInfoId,
                                                                   SupervisorName = sup.StrEmployeeName,
                                                                   LineManagerId = man.IntEmployeeBasicInfoId,
                                                                   LineManagerName = man.StrEmployeeName,
                                                                   EmploymentTypeId = emp.IntEmploymentTypeId,
                                                                   EmploymentTypeName = empT.StrEmploymentType,
                                                                   WorkplaceGroupId = wpg.IntWorkplaceGroupId,
                                                                   WorkplaceGroupName = wpg.StrWorkplaceGroup

                                                               }).AsNoTracking().OrderBy(x => x.EmployeeBasicInfo.IntEmployeeBasicInfoId).ToListAsync();

            return data;


        }

    }
}

using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Wordprocessing;
using LanguageExt.ClassInstances;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PeopleDesk.Data;
using PeopleDesk.Data.Entity;
using PeopleDesk.Models.AssetManagement;
using PeopleDesk.Models.Employee;
using PeopleDesk.Services.Auth;
using PeopleDesk.Services.SAAS.Interfaces;
using System.Globalization;
using System.Linq;
using static System.Drawing.FontConverter;

namespace PeopleDesk.Services.SAAS
{
    public class PayrollService : IPayrollService
    {
        private readonly PeopleDeskContext _context;
        private readonly IAuthService _authService;
        private readonly IApprovalPipelineService _approvalPipelineService;

        public PayrollService(IApprovalPipelineService _approvalPipelineService, PeopleDeskContext _context, IAuthService _authService)
        {
            this._context = _context;
            this._authService = _authService;
            this._approvalPipelineService = _approvalPipelineService;
        }

        public async Task<BankAdviceLanding> BankAdvices(BankAdviceHeader obj, long AccountId)
        {
            try
            {
                BankAdviceLanding retObj = new BankAdviceLanding();

                obj.searchTxt = obj.searchTxt.ToLower();

                IQueryable<BankAdvice> query = (from sr in _context.PyrPayrollSalaryGenerateRequests
                                               join sh in _context.PyrSalaryGenerateHeaders on sr.IntSalaryGenerateRequestId equals sh.IntSalaryGenerateRequestId into shJoin
                                               from sh in shJoin.DefaultIfEmpty()
                                               join des in _context.MasterDesignations on sh.IntDesignationId equals des.IntDesignationId into RankJoin
                                               from des in RankJoin.DefaultIfEmpty()
                                               join empD in _context.EmpEmployeeBasicInfoDetails on sh.IntEmployeeId equals empD.IntEmployeeId into empJoin
                                               from empD in empJoin.DefaultIfEmpty()
                                               join wi in _context.TerritorySetups on empD.IntWingId equals wi.IntTerritoryId into wi1
                                               from wing in wi1.DefaultIfEmpty()
                                               join soleD in _context.TerritorySetups on empD.IntSoleDepo equals soleD.IntTerritoryId into soleD1
                                               from soleDp in soleD1.DefaultIfEmpty()
                                               join regn in _context.TerritorySetups on empD.IntRegionId equals regn.IntTerritoryId into regn1
                                               from region in regn1.DefaultIfEmpty()
                                               join area1 in _context.TerritorySetups on empD.IntAreaId equals area1.IntTerritoryId into area2
                                               from area in area2.DefaultIfEmpty()
                                               join terrty in _context.TerritorySetups on empD.IntTerritoryId equals terrty.IntTerritoryId into terrty1
                                               from Territory in terrty1.DefaultIfEmpty()
                                               where sr.IsActive == true && sh.IsActive == true && sr.IntSalaryGenerateRequestId == obj.intSalaryGenerateRequestId
                                                   && sr.IntAccountId == AccountId && sr.IntYear == obj.intYearId && sr.IntMonth == obj.intMonthId
                                                   && (!string.IsNullOrEmpty(obj.searchTxt)? (sh.StrEmployeeCode.ToLower().Contains(obj.searchTxt) || sh.StrEmployeeName.ToLower().Contains(obj.searchTxt) || sh.StrAccountName.ToLower().Contains(obj.searchTxt)):true )
                                               orderby sh.StrEmployeeName ascending
                                               select new BankAdvice
                                               {
                                                   EmployeeCode=sh.StrEmployeeCode,
                                                   EmployeeName = sh.StrEmployeeName,
                                                   Designation=sh.StrDesignation==null?"":sh.StrDesignation,
                                                   DesignationRank= des.IntRankingId,
                                                   Territory= Territory.StrTerritoryName == null?"": Territory.StrTerritoryName,
                                                   Area= area.StrTerritoryName == null ? "" : area.StrTerritoryName,
                                                   Region=region.StrTerritoryName == null ? "" : region.StrTerritoryName,
                                                   MobileNumber=empD.StrPersonalMobile == null ? "" : empD.StrPersonalMobile ,
                                                   AccountNo = sh.StrAccountNo==null?"": sh.StrAccountNo,
                                                   NetSalary= sh.NumNetPayableSalary,
                                                   Remarks= "",
                                                   Wing=wing.StrTerritoryName == null ? "" : wing.StrTerritoryName,
                                                   SoleDepo=soleDp.StrTerritoryName ==null ? "" : soleDp.StrTerritoryName
                                               }).OrderBy(x => x.DesignationRank).AsNoTracking().AsQueryable();

               

                if (!obj.IsForXl)
                {
                    int maxSize = 1000;
                    obj.PageSize = obj.PageSize > maxSize ? maxSize : obj.PageSize;
                    obj.PageNo = obj.PageNo < 1 ? 1 : obj.PageNo;
                    retObj.Data=await query.Skip(obj.PageSize * (obj.PageNo - 1)).Take(obj.PageSize).ToListAsync();
                }
                else
                {
                    retObj.Data = await query.ToListAsync();
                }


                retObj.BankAdviceVM= query.GroupBy(x => new { x.SoleDepo, x.Wing }).Select(x => new BankAdviceHeaderVM { SoleDepoName= x.Key.SoleDepo,WingName=x.Key.Wing  }).ToList();
                retObj.TotalCount = query.Count();            
                retObj.PageSize = obj.PageSize;
                retObj.CurrentPage = obj.PageNo;
                retObj.MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName((int)obj.intMonthId)+"-"+obj.intYearId;

                return retObj;

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        #region ==== sole to area ddl ====
        public async Task<dynamic> GetSoledepoDdlByWorkplaceGroup(long AccountId, long BusinessUnitId, long WorkplaceGroup, BaseVM tokenData)
        {
            try
            {
                var sole = await (from s in _context.TerritorySetups
                                  where s.IntAccountId == AccountId && s.IntBusinessUnitId == BusinessUnitId && s.IntWorkplaceGroupId == WorkplaceGroup
                                  && (tokenData.isOfficeAdmin == true ? true : (tokenData.soleDepoList.Contains(0)  || tokenData.soleDepoList.Contains(s.IntTerritoryId)))
                                  join type in _context.OrganizationTypes on s.IntTerritoryTypeId equals type.IntOrganizationTypeId
                                  where type.IntOrganizationTypeId == 5
                                  select new
                                  {
                                      value = s.IntTerritoryId,
                                      label = s.StrTerritoryName,
                                      type = type.StrOrganizationTypeName
                                  }).AsNoTracking().ToListAsync();
                return sole;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<dynamic> GetAreaBySoleDepoId(long AccountId, long BusinessUnitId, long WorkplaceGroup, long soleDepo, BaseVM tokenData)
        {
            try
            {
                var sole = await (from s in _context.TerritorySetups
                                  where s.IntAccountId == AccountId && s.IntBusinessUnitId == BusinessUnitId
                                  && s.IntWorkplaceGroupId == WorkplaceGroup && s.IntTerritoryId == soleDepo
                                  join r in _context.TerritorySetups on
                                  new { a = s.IntTerritoryId, b = true } equals new { a = r.IntParentTerritoryId, b = r.IsActive }
                                  join a in _context.TerritorySetups on
                                  new { a = r.IntTerritoryId, b = true } equals new { a = a.IntParentTerritoryId, b = a.IsActive }
                                  join type in _context.OrganizationTypes on
                                  new { a = s.IntTerritoryTypeId, c = "Sole Depo" } equals new { a = type.IntOrganizationTypeId, c = type.StrOrganizationTypeName }
                                  where (tokenData.isOfficeAdmin == true ? true : (tokenData.areaList.Contains(0) || tokenData.areaList.Contains(a.IntTerritoryId)))
                                  select new
                                  {
                                      value = a.IntTerritoryId,
                                      label = a.StrTerritoryName

                                  }).AsNoTracking().ToListAsync();
                return sole;
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion
    }
}

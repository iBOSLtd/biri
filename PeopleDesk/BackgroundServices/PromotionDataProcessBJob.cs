using Azure.Storage.Blobs.Models;
using Microsoft.EntityFrameworkCore;
using PeopleDesk.Data;
using PeopleDesk.Data.Entity;
using PeopleDesk.Models.Employee;
using PeopleDesk.Models.Global;
using PeopleDesk.Services.SignalR.Interfaces;
using System;

namespace PeopleDesk.BackgroundServices
{
    public class PromotionDataProcessBJob : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        public PromotionDataProcessBJob(IServiceProvider serviceProvider, ILogger<PromotionDataProcessBJob> logger)
        {
            _serviceProvider = serviceProvider;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var _context = scope.ServiceProvider.GetRequiredService<PeopleDeskContext>();

                        List<EmpTransferNpromotion> promotionList = await _context.EmpTransferNpromotions.Where(x => x.StrTransferNpromotionType.ToLower() == "Promotion".ToLower()
                        && x.IsPipelineClosed == true && x.IsReject == false && x.IsProcess == false && x.IsActive == true).ToListAsync();

                        if (promotionList != null)
                        {
                            foreach (var promo in promotionList)
                            {

                                EmpTransferNpromotion promotion = await _context.EmpTransferNpromotions.Where(x => x.IntTransferNpromotionId == promo.IntTransferNpromotionId).FirstOrDefaultAsync();

                                if (promotion.DteEffectiveDate.Date <= DateTime.Now)
                                {
                                    promotion.IsProcess = true;
                                    promotion.IntUpdatedBy = 1;
                                    promotion.DteUpdatedAt = DateTime.Now;

                                    _context.EmpTransferNpromotions.Update(promotion);
                                    await _context.SaveChangesAsync();

                                    EmpEmployeeBasicInfo empEmployeeBasic = await _context.EmpEmployeeBasicInfos.FirstOrDefaultAsync(x => x.IntEmployeeBasicInfoId == promotion.IntEmployeeId);
                                    if (empEmployeeBasic != null)
                                    {
                                        empEmployeeBasic.IntBusinessUnitId = promotion.IntBusinessUnitId;
                                        empEmployeeBasic.IntWorkplaceGroupId = promotion.IntWorkplaceGroupId;
                                        empEmployeeBasic.IntWorkplaceId = promotion.IntWorkplaceId;
                                        empEmployeeBasic.IntSupervisorId = promotion.IntSupervisorId;
                                        empEmployeeBasic.IntLineManagerId = promotion.IntLineManagerId;
                                        empEmployeeBasic.IntDottedSupervisorId = promotion.IntDottedSupervisorId;
                                        empEmployeeBasic.IntDepartmentId = promotion.IntDepartmentId;
                                        empEmployeeBasic.IntDesignationId = promotion.IntDesignationId;

                                        _context.EmpEmployeeBasicInfos.Update(empEmployeeBasic);
                                        await _context.SaveChangesAsync();

                                        List<RoleBridgeWithDesignation> deleteExistingRoleList = await _context.RoleBridgeWithDesignations
                                            .Where(x => x.StrIsFor.ToLower() == "Employee".ToLower() && x.IntAccountId == promotion.IntAccountId && x.IntDesignationOrEmployeeId == promotion.IntEmployeeId && x.IsActive == true).ToListAsync();

                                        List<RoleBridgeWithDesignation> roleExistWithNewDesignationList = await _context.RoleBridgeWithDesignations
                                            .Where(x => (x.StrIsFor.ToLower() == "Designation".ToLower() && x.IntAccountId == promotion.IntAccountId && x.IntDesignationOrEmployeeId == promotion.IntDesignationId && x.IsActive == true)).ToListAsync();

                                        List<EmpTransferNpromotionUserRole> transferNpromotionUserRoleList = await _context.EmpTransferNpromotionUserRoles.Where(x => x.IntTransferNpromotionId == promotion.IntTransferNpromotionId && x.IsActive == true).ToListAsync();
                                        List<RoleBridgeWithDesignation> newRoleAssignToUser = new List<RoleBridgeWithDesignation>();

                                        foreach (EmpTransferNpromotionUserRole item in transferNpromotionUserRoleList)
                                        {
                                            bool isDesignationExists = false;

                                            if (roleExistWithNewDesignationList.Select(x => x.IntRoleId).Contains(item.IntUserRoleId))
                                            {
                                                isDesignationExists = true;
                                            }

                                            if (deleteExistingRoleList.Select(x => x.IntRoleId).Contains(item.IntUserRoleId) && isDesignationExists == false)
                                            {
                                                deleteExistingRoleList = deleteExistingRoleList.Where(x => x.IntRoleId != item.IntUserRoleId).ToList();
                                            }
                                            else if (!deleteExistingRoleList.Select(x => x.IntRoleId).Contains(item.IntUserRoleId) && isDesignationExists == false)
                                            {
                                                newRoleAssignToUser.Add(new RoleBridgeWithDesignation
                                                {
                                                    IntAccountId = promotion.IntAccountId,
                                                    StrIsFor = "Employee",
                                                    IntDesignationOrEmployeeId = promotion.IntEmployeeId,
                                                    IntRoleId = item.IntUserRoleId,
                                                    IntCreatedBy = (long)promotion.IntCreatedBy,
                                                    DteCreatedDateTime = DateTime.Now,
                                                    IsActive = true
                                                });
                                            }
                                        }

                                        List<RoleExtensionRow> roleExtensionList = await _context.RoleExtensionRows
                                            .Where(x => x.IntEmployeeId == promotion.IntEmployeeId && x.IsActive == true).ToListAsync();

                                        List<EmpTransferNpromotionRoleExtension> transferNpromotionRoleExtensionList = await _context.EmpTransferNpromotionRoleExtensions.Where(x => x.IntTransferNpromotionId == promotion.IntTransferNpromotionId && x.IsActive == true).ToListAsync();
                                        List<RoleExtensionRow> newRoleExtensionList = new List<RoleExtensionRow>();

                                        RoleExtensionHeader header = await _context.RoleExtensionHeaders.FirstOrDefaultAsync(x => x.IntEmployeeId == promotion.IntEmployeeId && x.IsActive == true);

                                        if (header == null)
                                        {
                                            header = new RoleExtensionHeader();
                                            header.IntEmployeeId = promotion.IntEmployeeId;
                                            header.IntCreatedBy = (long)promotion.IntCreatedBy;
                                            header.DteCreatedDateTime = DateTime.Now;
                                            header.IsActive = true;

                                            await _context.RoleExtensionHeaders.AddAsync(header);
                                            await _context.SaveChangesAsync();
                                        }

                                        foreach (EmpTransferNpromotionRoleExtension item in transferNpromotionRoleExtensionList)
                                        {
                                            if (roleExtensionList.Where(x => x.IntEmployeeId == item.IntEmployeeId && x.IntOrganizationTypeId == item.IntOrganizationTypeId && x.IntOrganizationReffId == item.IntOrganizationReffId).Count() <= 0)
                                            {
                                                newRoleExtensionList.Add(new RoleExtensionRow
                                                {
                                                    IntRoleExtensionHeaderId = header.IntRoleExtensionHeaderId,
                                                    IntEmployeeId = promotion.IntEmployeeId,
                                                    IntOrganizationTypeId = item.IntOrganizationTypeId,
                                                    StrOrganizationTypeName = item.StrOrganizationTypeName,
                                                    IntOrganizationReffId = item.IntOrganizationReffId,
                                                    StrOrganizationReffName = item.StrOrganizationReffName,
                                                    IntCreatedBy = (long)promotion.IntCreatedBy,
                                                    DteCreatedDateTime = DateTime.Now,
                                                    IsActive = true
                                                });
                                            }
                                            else
                                            {
                                                roleExtensionList = roleExtensionList.Where(x => x.IntRoleExtensionRowId != roleExtensionList.Where(x => x.IntEmployeeId == item.IntEmployeeId && x.IntOrganizationTypeId == item.IntOrganizationTypeId && x.IntOrganizationReffId == item.IntOrganizationReffId).FirstOrDefault().IntRoleExtensionRowId).ToList();
                                            }
                                        }

                                        if (deleteExistingRoleList.Count() > 0)
                                        {
                                            deleteExistingRoleList.ForEach(d =>
                                            {
                                                d.IsActive = false;
                                                d.IntUpdatedBy = promotion.IntEmployeeId;
                                                d.DteUpdateDateTime = DateTime.Now;
                                            });
                                            _context.RoleBridgeWithDesignations.UpdateRange(deleteExistingRoleList);
                                            await _context.SaveChangesAsync();
                                        }
                                        if (newRoleAssignToUser.Count() > 0)
                                        {
                                            await _context.RoleBridgeWithDesignations.AddRangeAsync(newRoleAssignToUser);
                                            await _context.SaveChangesAsync();
                                        }

                                        if (roleExtensionList.Count() > 0)
                                        {
                                            roleExtensionList.ForEach(d =>
                                            {
                                                d.IsActive = false;
                                            });
                                            _context.RoleExtensionRows.UpdateRange(roleExtensionList);
                                            await _context.SaveChangesAsync();
                                        }
                                        if (newRoleExtensionList.Count() > 0)
                                        {
                                            await _context.RoleExtensionRows.AddRangeAsync(newRoleExtensionList);
                                            await _context.SaveChangesAsync();
                                        }
                                    }
                                }
                            }
                        }
                    }
                    await Task.Delay(1000 * 60 * 60, stoppingToken);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}

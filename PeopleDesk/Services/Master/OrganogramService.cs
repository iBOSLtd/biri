using Microsoft.EntityFrameworkCore;
using PeopleDesk.Data;
using PeopleDesk.Data.Entity;
using PeopleDesk.Models;
using PeopleDesk.Models.MasterData;
using PeopleDesk.Services.MasterData.Interfaces;

namespace PeopleDesk.Services.MasterData
{
    public class OrganogramService : IOrganogramService
    {
        private readonly PeopleDeskContext _context;
        public OrganogramService(PeopleDeskContext context)
        {
            _context = context;
        }

        public async Task<OrganogramTreeViewModel> GetOrganogramTree(long businessUnitId)
        {
            try
            {
                //emp.strEmployeeName,' [',emp.strEmployeeCode,'], ',dsg.strDesignationName
                List<OrganogramTreeViewModel> res = await (from org in _context.GlobalOrganogramTrees
                                                           join emp in _context.EmpEmployeeBasicInfos on org.IntEmployeeId equals emp.IntEmployeeBasicInfoId into LeftEmp
                                                           from empInfo in LeftEmp.DefaultIfEmpty()
                                                           join des in _context.MasterDesignations on empInfo.IntDesignationId equals des.IntDesignationId into LeftDes
                                                           from desInfo in LeftDes.DefaultIfEmpty()
                                                           join dett in _context.EmpEmployeeBasicInfoDetails on empInfo.IntEmployeeBasicInfoId equals dett.IntEmployeeId into dett2
                                                           from details in dett2.DefaultIfEmpty()
                                                           where org.IntBusinessUnitId == businessUnitId && org.IsActive == true
                                                           orderby org.IntParentId, org.IntSequence
                                                           let pho = empInfo != null ? _context.EmpEmployeePhotoIdentities.Where(x => x.IntEmployeeBasicInfoId == empInfo.IntEmployeeBasicInfoId && x.IntProfilePicFileUrlId > 0).FirstOrDefault() : null
                                                           select new OrganogramTreeViewModel
                                                           {
                                                               AutoId = org.IntAutoId,
                                                               ParentId = org.IntParentId,
                                                               Sequence = org.IntSequence,
                                                               PositionId = org.IntPositionId,
                                                               PositionName = org.StrPositionName,
                                                               EmployeeId = org.IntEmployeeId,
                                                               EmployeeName = org.StrEmployeeName,
                                                               EmployeeNameDetails = org.StrEmployeeName + empInfo != null ? " [" + empInfo.StrEmployeeCode + "], " : "" + desInfo != null ? desInfo.StrDesignation : "",
                                                               DesignationId = empInfo != null ? empInfo.IntDesignationId : null,
                                                               DesignationName = desInfo != null ? desInfo.StrDesignation : "",
                                                               EmployeeImageUrlId = pho != null ? pho.IntProfilePicFileUrlId : null,
                                                               Email = details != null ? details.StrPersonalMail : ""
                                                           }).ToListAsync();

                if (res.Count > 0)
                {
                    var response = OrganogramService.MakeOrganogramTree(res, 0);
                    return response.FirstOrDefault();
                }
                else
                {
                    return new OrganogramTreeViewModel();//with null value in it : must return as it is
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static List<OrganogramTreeViewModel> MakeOrganogramTree(List<OrganogramTreeViewModel> objList, long parentId)
        {   //Recursive function
            return objList.Where(x => x.ParentId.Equals(parentId)).Select(item => new OrganogramTreeViewModel
            {
                AutoId = item.AutoId,
                ParentId = item.ParentId,
                Sequence = item.Sequence,
                PositionId = item.PositionId,
                PositionName = item.PositionName,
                EmployeeId = item.EmployeeId,
                EmployeeName = item.EmployeeName,
                EmployeeNameDetails = item.EmployeeNameDetails,
                DesignationId = item.DesignationId,
                DesignationName = item.DesignationName,
                EmployeeImageUrlId = item.EmployeeImageUrlId,
                Email = item.Email,
                ChildList = MakeOrganogramTree(objList, item.AutoId)
            }).ToList();
        }

        public async Task<MessageHelper> OrganogramReConstruct(List<OrganogramReConstructViewModel> objList)
        {
            try
            {
                var newItemList = new List<GlobalOrganogramTree>();
                var updateItemList = new List<GlobalOrganogramTree>();
                foreach (var item in objList)
                {
                    if (item.AutoId > 0)
                    {
                        var existsObj = await Task.FromResult((from org in _context.GlobalOrganogramTrees
                                                               where org.IntBusinessUnitId == item.BusinessUnitId
                                                               && (org.IntEmployeeId == item.EmployeeId && org.IntEmployeeId != 0)
                                                               && org.IntPositionId == item.PositionId
                                                               && (org.IntParentId == item.ParentId)
                                                               && org.IsActive == true
                                                               && org.IntAutoId != item.AutoId
                                                               select org).FirstOrDefault());
                        if (existsObj != null)
                        {
                            return new MessageHelper()
                            {
                                Message = $"{item.PositionName}-{item.EmployeeName} already exist",
                                StatusCode = 500
                            };
                        }

                        GlobalOrganogramTree updateObj = await _context.GlobalOrganogramTrees.Where(x => x.IntAutoId == item.AutoId).FirstOrDefaultAsync();
                        if (updateObj != null)
                        {
                            updateObj.IntPositionId = item.PositionId;
                            updateObj.StrPositionName = item.PositionName;
                            updateObj.IntEmployeeId = item.EmployeeId;
                            updateObj.StrEmployeeName = item.EmployeeName;
                            updateObj.IntParentId = item.ParentId;
                            updateObj.IntSequence = item.Sequence;
                            updateObj.IntBusinessUnitId = item.BusinessUnitId;
                            updateObj.IsActive = item.IsActive;
                            updateObj.DteCreatedAt = DateTime.Now;
                            updateObj.IntCreatedBy = item.CreatedBy;

                            updateItemList.Add(updateObj);

                            if (item.IsActive == false)//inactive the childs
                            {
                                var inactiveChilds = await Task.FromResult(_context.GlobalOrganogramTrees.Where(x => x.IntParentId == item.AutoId && x.IsActive == true).ToList());
                                if (inactiveChilds.Count > 0)
                                {
                                    inactiveChilds.ForEach(x =>
                                    {
                                        x.IsActive = false;
                                    });
                                    updateItemList.AddRange(inactiveChilds);
                                }
                            }
                        }
                    }
                    else
                    {
                        GlobalOrganogramTree existsObj = await (from org in _context.GlobalOrganogramTrees
                                                                where org.IntBusinessUnitId == item.BusinessUnitId
                                                                && (org.IntEmployeeId == item.EmployeeId && org.IntEmployeeId != 0)
                                                                && org.IntPositionId == item.PositionId
                                                                && (org.IntParentId == item.ParentId)
                                                                && org.IsActive == true
                                                                select org).FirstOrDefaultAsync();
                        if (existsObj != null)
                        {
                            return new MessageHelper()
                            {
                                Message = $"{item.PositionName}-{item.EmployeeName} already exist",
                                StatusCode = 500
                            };
                        }

                        GlobalOrganogramTree newObj = new GlobalOrganogramTree
                        {
                            IntPositionId = item.PositionId,
                            StrPositionName = item.PositionName,
                            IntEmployeeId = item.EmployeeId,
                            StrEmployeeName = item.EmployeeName,
                            IntParentId = item.ParentId,
                            IntSequence = item.Sequence,
                            IntBusinessUnitId = item.BusinessUnitId,
                            IsActive = true,
                            DteCreatedAt = DateTime.Now,
                            IntCreatedBy = item.CreatedBy
                        };
                        newItemList.Add(newObj);

                    }
                }

                if (updateItemList.Count > 0)
                {
                    _context.GlobalOrganogramTrees.UpdateRange(updateItemList);
                    await _context.SaveChangesAsync();
                }
                if (newItemList.Count > 0)
                {
                    await _context.GlobalOrganogramTrees.AddRangeAsync(newItemList);
                    await _context.SaveChangesAsync();
                }

                return new MessageHelper()
                {
                    Message = "Processing",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

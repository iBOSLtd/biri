using Microsoft.EntityFrameworkCore;
using PeopleDesk.Data;
using PeopleDesk.Data.Entity;
using PeopleDesk.Models;
using PeopleDesk.Models.Task;
using PeopleDesk.Services.SAAS.Interfaces;

namespace PeopleDesk.Services.SAAS
{
    public class TaskManagementService : ITaskManagementService
    {

        private readonly PeopleDeskContext _context;
        MessageHelper res = new MessageHelper();

        public TaskManagementService(PeopleDeskContext _context)
        {

            this._context = _context;

        }

        #region    =================  Task Project  =================
        public async Task<MessageHelper> TskProjectCreateAndUpdate(TskProject obj)
        {

            try
            {
                if (obj.IntProjectId > 0)
                {
                    _context.Entry(obj).State = EntityState.Modified;
                    _context.Entry(obj).Property(x => x.IntCreatedBy).IsModified = false;
                    _context.Entry(obj).Property(x => x.DteCreatedAt).IsModified = false;

                    _context.TskProjects.Update(obj);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    await _context.TskProjects.AddAsync(obj);
                    await _context.SaveChangesAsync();

                }
                res.Message = "Create Successfull";
                res.StatusCode = 200;
                res.AutoId = obj.IntProjectId;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.StatusCode = 500;
            }
            return res;
        }
        public async Task<List<TskProject>> TskProjectLanding(long accountId, long businessId)
        {
            return await _context.TskProjects.Where(x => x.IsActivate == true && x.IntAccountId == accountId && x.IntProjectId == businessId).ToListAsync();

        }
        public async Task<TskProject> TskProjectById(long Id)
        {
            return await _context.TskProjects.Where(x => x.IsActivate == true && x.IntProjectId == Id).FirstOrDefaultAsync();
        }
        public async Task<MessageHelper> TskProjectDeleteById(long Id)
        {
            try
            {
                TskProject obj = await _context.TskProjects.Where(x => x.IsActivate == true && x.IntProjectId == Id).FirstOrDefaultAsync();
                obj.IsActivate = false;

                _context.TskProjects.Update(obj);
                await _context.SaveChangesAsync();

                res.Message = "Delete Successfull";
                res.StatusCode = 200;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.StatusCode = 500;

            }
            return res;
        }

        #endregion

        #region       ================  Task Group Members  ===============
        public async Task<MessageHelper> TskGroupMemberCreateAndUpdate(TskGroupMember obj)
        {
            try
            {
                if (obj.IntGroupMemberId > 0)
                {
                    _context.Entry(obj).State = EntityState.Modified;
                    _context.Entry(obj).Property(x => x.IntCreatedBy).IsModified = false;
                    _context.Entry(obj).Property(x => x.DteCreatedAt).IsModified = false;

                    _context.TskGroupMembers.Update(obj);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    await _context.TskGroupMembers.AddAsync(obj);
                    await _context.SaveChangesAsync();

                }
                res.Message = "Create Successfull";
                res.StatusCode = 200;
                res.AutoId = obj.IntGroupMemberId;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.StatusCode = 500;
            }
            return res;
        }
        public async Task<List<TskGroupMember>> TskGroupMemberLandingByProjectId(long Id)
        {
            return await _context.TskGroupMembers.Where(x => x.IsActive == true && x.IntAutoId == Id && x.IntGroupMemberTypeId == 1).ToListAsync();
        }
        public async Task<TskGroupMember> TskGroupMemberById(long Id)
        {
            return await _context.TskGroupMembers.Where(x => x.IsActive == true && x.IntGroupMemberId == Id).FirstOrDefaultAsync();
        }
        public async Task<MessageHelper> TskGroupMemberDelete(long Id)
        {
            try
            {
                TskGroupMember obj = await _context.TskGroupMembers.Where(x => x.IsActive == true && x.IntGroupMemberId == Id).FirstOrDefaultAsync();
                obj.IsActive = false;

                _context.TskGroupMembers.Update(obj);
                await _context.SaveChangesAsync();

                res.Message = "Delete Successfull";
                res.StatusCode = 200;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.StatusCode = 500;

            }
            return res;
        }

        #endregion

        #region  =====================  Task Board  ======================
        public async Task<MessageHelper> TskBoardCreateAndUpdate(TskBoard obj)
        {
            try
            {
                if (obj.IntBoardId > 0)
                {
                    _context.Entry(obj).State = EntityState.Modified;
                    _context.Entry(obj).Property(x => x.IntCreatedBy).IsModified = false;
                    _context.Entry(obj).Property(x => x.DteCreatedAt).IsModified = false;

                    _context.TskBoards.Update(obj);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    await _context.TskBoards.AddAsync(obj);
                    await _context.SaveChangesAsync();
                }

                res.Message = "Created Successful";
                res.StatusCode = 200;
                res.AutoId = obj.IntBoardId;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.StatusCode = 500;
            }

            return res;
        }

        public async Task<List<TskBoard>> TskBoardLandingByProjectId()
        {
            return await _context.TskBoards.Where(x => x.IsActive == true && x.IntBoardId == x.IntProjectId).ToListAsync();
        }
        public async Task<TskBoard> TskBoardById(long Id)
        {
            return await _context.TskBoards.Where(x => x.IsActive == true && x.IntBoardId == Id).FirstOrDefaultAsync();
        }

        public async Task<MessageHelper> TskBoardDeleteById(long Id)
        {
            try
            {
                TskBoard obj = await _context.TskBoards.Where(x => x.IsActive == true && x.IntBoardId == Id).FirstOrDefaultAsync();
                obj.IsActive = false;

                _context.TskBoards.Update(obj);
                await _context.SaveChangesAsync();

                res.Message = "Delete Successful";
                res.StatusCode = 200;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.StatusCode = 500;
            }
            return res;
        }

        #endregion

        #region  ======================  Task Details  ========================

        public async Task<MessageHelper> TskTaskDetailsCreateAndUpdate(TskTaskDetail obj)
        {
            try
            {
                if (obj.IntTaskDetailsId > 0)
                {
                    _context.Entry(obj).State = EntityState.Modified;
                    _context.Entry(obj).Property(x => x.IntCreatedBy).IsModified = false;
                    _context.Entry(obj).Property(x => x.DteCreatedAt).IsModified = false;

                    _context.TskTaskDetails.Update(obj);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    await _context.TskTaskDetails.AddAsync(obj);
                    await _context.SaveChangesAsync();
                }

                res.Message = "Created Successful";
                res.StatusCode = 200;
                res.AutoId = obj.IntTaskDetailsId;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.StatusCode = 500;
            }

            return res;
        }

        public async Task<List<TskTaskDetail>> TskTaskDetailsLandingByBoardId()
        {
            return await _context.TskTaskDetails.Where(x => x.IsActive == true && x.IntTaskDetailsId == x.IntBoardId).ToListAsync();
        }

        public async Task<TskTaskDetail> TskTaskDetailsById(long Id)
        {
            return await _context.TskTaskDetails.Where(x => x.IsActive == true && x.IntTaskDetailsId == Id).FirstOrDefaultAsync();
        }

        public async Task<MessageHelper> TskTaskDetailsDeleteById(long Id)
        {
            try
            {
                TskTaskDetail obj = await _context.TskTaskDetails.Where(x => x.IsActive == true && x.IntTaskDetailsId == Id).FirstOrDefaultAsync();
                obj.IsActive = false;

                _context.TskTaskDetails.Update(obj);
                await _context.SaveChangesAsync();

                res.Message = "Delete Successful";
                res.StatusCode = 200;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.StatusCode = 500;
            }
            return res;
        }

        #endregion


        #region ==================== Query Data =====================

        public async Task<TskProjectViewModel> ProjectDetailsInformation(long Id)
        {
            try
            {
                TskProjectViewModel data = await (from pro in _context.TskProjects
                                                      //join borad1 in _context.TskBoards on pro.IntProjectId equals borad1.IntProjectId into borad2
                                                      //from borad in borad2.DefaultIfEmpty()
                                                  where pro.IntProjectId == Id
                                                  select new TskProjectViewModel
                                                  {
                                                      IntProjectId = pro.IntProjectId,
                                                      StrProjectName = pro.StrProjectName,
                                                      TskBoardViewModelList = (from b in _context.TskBoards
                                                                               where b.IntProjectId == pro.IntProjectId
                                                                               select new TskBoardViewModel
                                                                               {
                                                                                   IntBoardId = b.IntBoardId,
                                                                                   StrBoardName = b.StrBoardName,
                                                                                   TskTaskDetailViewModelList = (from tsk in _context.TskTaskDetails
                                                                                                                 where tsk.IntBoardId == b.IntBoardId
                                                                                                                 select new TskTaskDetailViewModel
                                                                                                                 {
                                                                                                                     IntTaskDetailsId = tsk.IntTaskDetailsId,
                                                                                                                     StrTaskTitle = tsk.StrTaskTitle,
                                                                                                                     TskGroupMemberViewModelList = (from m in _context.TskGroupMembers
                                                                                                                                                    where m.IntGroupMemberTypeId == 3 && m.IntAutoId == tsk.IntTaskDetailsId
                                                                                                                                                    select new TskGroupMemberViewModel
                                                                                                                                                    {
                                                                                                                                                        IntGroupMemberId = m.IntGroupMemberId,
                                                                                                                                                        StrEmployeeName = m.StrEmployeeName
                                                                                                                                                    }).ToList(),
                                                                                                                 }).ToList(),

                                                                               }).ToList()

                                                  }).FirstOrDefaultAsync();

                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public async Task<TskProjectViewModel> ProjectDetailsInformationJoin(long Id)
        //{
        //    try
        //    {
        //        TskProjectViewModel data = await (from p in _context.TskProjects
        //                                          join b1 in _context.TskBoards
        //                                          on p.IntProjectId equals b1.IntProjectId into b2
        //                                          from b in b2.DefaultIfEmpty()
        //                                          select new TskProjectViewModel
        //                                          {
        //                                              IntProjectId = p.IntProjectId,
        //                                              StrProjectName = p.StrProjectName,
        //                                              TskBoardViewModelList = (from b3 in _context.TskBoards 
        //                                                                       join t in _context.TskTaskDetails
        //                                                                       on b3.IntBoardId equals t.IntBoardId into t4

        //                                                                       select new TskBoardViewModel
        //                                                                       {
        //                                                                           IntBoardId = b3.IntBoardId,
        //                                                                           StrBoardName = b3.StrBoardName,
        //                                                                           TskTaskDetailViewModelList = (from b4 in _context.TskBoards
        //                                                                                                         join t1 in _context.TskTaskDetails
        //                                                                                                         on b4.IntBoardId equals t1.IntBoardId
        //                                                                                                         on 
        //                                                                                                         select new
        //                                                                                                         {

        //                                                                                                         }).ToList()

        //                                                                       }).ToList(),

        //                                          }).FirstOrDefaultAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        #endregion


        #region  ===================  Project Creation =================
        public async Task<MessageHelper> ProjectCreaete(TskProjectCreateViewModel obj)
        {
            try
            {
                TskProject proj = new TskProject()
                {
                    StrProjectName = obj.StrProjectName,
                    DteStartDate = obj.DteStartDate,
                    DteEndDate = obj.DteEndDate,
                    IntFileUrlId = obj.IntFileUrlId,
                    IsActivate = true,
                    StrDescription = obj.StrDescription,
                    IntAccountId = obj.IntAccountId,
                    StrStatus = obj.StrStatus,
                    DteCreatedAt = DateTime.Now,
                    IntCreatedBy = obj.IntCreatedBy,
                };
                if (obj.IntProjectId > 0)
                {
                    proj.IntUpdatedBy = obj.IntUpdatedBy;
                    proj.DteUpdatedAt = DateTime.Now;

                    _context.Entry(proj).State = EntityState.Modified;
                    _context.Entry(proj).Property(x => x.IntCreatedBy).IsModified = false;
                    _context.Entry(proj).Property(x => x.DteCreatedAt).IsModified = false;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    proj.IntCreatedBy = obj.IntCreatedBy;
                    proj.DteCreatedAt = DateTime.Now;

                    await _context.TskProjects.AddAsync(proj);
                    await _context.SaveChangesAsync();
                }

                List<TskGroupMember> memberList = new List<TskGroupMember>();
                foreach (var item in obj.GroupMemberIdNameList)
                {
                    TskGroupMember member = new TskGroupMember
                    {
                        IntEmployeeId = item.IntEmployeeId,
                        StrEmployeeName = item.StrEmployeeName,
                        IntAutoId = proj.IntProjectId,
                        IntGroupMemberTypeId = 1,
                        IsActive = true,
                        IsDelete = false,
                        DteCreatedAt = DateTime.Now,
                        IntCreatedBy = obj.IntCreatedBy
                    };
                    memberList.Add(member);
                }

                await _context.TskGroupMembers.AddRangeAsync(memberList);
                await _context.SaveChangesAsync();


                //TskProjectCreateViewModel data = (from m in _context.TskGroupMembers
                //                                  where m.IntGroupMemberTypeId == 1 && m.IntAutoId == m.IntGroupMemberId
                //                                  select new TskProjectCreateViewModel
                //                                  {
                //                                      GroupMemberIdList = m.IntEmployeeId
                //                                  }).ToList();

                res.Message = "Create Successful";
                res.StatusCode = 200;

            }

            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.StatusCode = 500;

            }
            return res;
        }

        #endregion

        #region  ==================  Board Creation  ================
        public async Task<MessageHelper> BoardCreate(TskBoardCreateViewModel obj)
        {
            try
            {
                TskBoard board = new TskBoard
                {
                    IntProjectId = obj.IntProjectId,
                    StrBoardName = obj.StrBoardName,
                    StrDescription = obj.StrDescription,
                    DteStartDate = obj.DteStartDate,
                    DteEndDate = obj.DteEndDate,
                    IntFileUrlId = obj.IntFileUrlId,
                    IntReporterId = obj.IntReporterId,
                    StrPriority = obj.StrPriority,
                    StrBackgroundColor = obj.StrBackgroundColor,
                    StrHtmlColorCode = obj.StrHtmlColorCode,
                    StrStatus = obj.StrStatus,
                    DteCreatedAt = DateTime.Now,
                    IntCreatedBy = obj.IntCreatedBy,
                    IsActive = true

                };

                if (obj.IntBoardId > 0)
                {
                    board.IntUpdatedBy = obj.IntUpdatedBy;
                    board.DteUpdatedAt = DateTime.Now;

                    _context.Entry(board).State = EntityState.Modified;
                    _context.Entry(board).Property(x => x.IntCreatedBy).IsModified = false;
                    _context.Entry(board).Property(x => x.DteCreatedAt).IsModified = false;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    board.IntCreatedBy = obj.IntCreatedBy;
                    board.DteCreatedAt = DateTime.Now;

                    await _context.TskBoards.AddAsync(board);
                    await _context.SaveChangesAsync();
                }

                List<TskGroupMember> memberList = new List<TskGroupMember>();
                foreach (var item in obj.GroupMemberIdNameList)
                {
                    TskGroupMember member = new TskGroupMember
                    {
                        IntEmployeeId = item.IntEmployeeId,
                        StrEmployeeName = item.StrEmployeeName,
                        IntAutoId = board.IntBoardId,
                        IntGroupMemberTypeId = 2,
                        IsActive = true,
                        IsDelete = false,
                        DteCreatedAt = DateTime.Now,
                        IntCreatedBy = obj.IntCreatedBy
                    };
                    memberList.Add(member);
                }

                await _context.TskGroupMembers.AddRangeAsync(memberList);
                await _context.SaveChangesAsync();

                res.Message = "Create Successful";
                res.StatusCode = 200;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.StatusCode = 500;
            }
            return res;
        }

        #endregion

        #region  ===============  Task Creation  ===============
        public async Task<MessageHelper> TaskCreate(TskTaskCreateViewModel obj)
        {
            try
            {
                TskTaskDetail task = new TskTaskDetail
                {
                    IntProjectId = obj.IntProjectId,
                    IntBoardId = obj.IntBoardId,
                    StrTaskTitle = obj.StrTaskTitle,
                    StrTaskDescription = obj.StrTaskDescription,
                    StrStatus = obj.StrStatus,
                    DteCreatedAt = DateTime.Now,
                    IntCreatedBy = obj.IntCreatedBy,
                    IsActive = true

                };
                await _context.TskTaskDetails.AddAsync(task);
                await _context.SaveChangesAsync();

                List<TskGroupMember> memberList = new List<TskGroupMember>();
                foreach (var item in obj.GroupMemberIdNameList)
                {
                    TskGroupMember member = new TskGroupMember
                    {
                        IntEmployeeId = item.IntEmployeeId,
                        StrEmployeeName = item.StrEmployeeName,
                        IntAutoId = task.IntTaskDetailsId,
                        IntGroupMemberTypeId = 3,
                        IsActive = true,
                        IsDelete = false,
                        DteCreatedAt = DateTime.Now,
                        IntCreatedBy = obj.IntCreatedBy
                    };
                    memberList.Add(member);
                }

                if (obj.IntTaskDetailsId > 0)
                {
                    task.IntUpdatedBy = obj.IntUpdatedBy;
                    task.DteUpdatedAt = DateTime.Now;

                    _context.Entry(task).State = EntityState.Modified;
                    _context.Entry(task).Property(x => x.IntCreatedBy).IsModified = false;
                    _context.Entry(task).Property(x => x.DteCreatedAt).IsModified = false;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    task.IntCreatedBy = obj.IntCreatedBy;
                    task.DteCreatedAt = DateTime.Now;

                    await _context.TskTaskDetails.AddAsync(task);
                    await _context.SaveChangesAsync();
                }

                await _context.TskGroupMembers.AddRangeAsync(memberList);
                await _context.SaveChangesAsync();


                res.Message = "Create Successfull";
                res.StatusCode = 200;
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.StatusCode = 500;
            }

            return res;
        }

        #endregion



    }
}

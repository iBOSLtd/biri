using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PeopleDesk.Data;
using PeopleDesk.Data.Entity;
using PeopleDesk.Extensions;
using PeopleDesk.Helper;
using PeopleDesk.Models;
using PeopleDesk.Models.MasterData;
using PeopleDesk.Services.Master.Interfaces;
using System.Data;
using System.Data.SqlClient;

namespace PeopleDesk.Controllers.MasterData
{
    [Route("api/[controller]")]
    [ApiController]
    public class MasterDataController : ControllerBase
    {
        private readonly IMasterService _masterService;
        private readonly PeopleDeskContext _context;

        public MasterDataController(IMasterService _masterService, PeopleDeskContext _context)
        {
            this._masterService = _masterService;
            this._context = _context;
        }

        #region ========== User Type ==================
        [HttpPost]
        [Route("SaveUserType")]
        public async Task<IActionResult> SaveUserType(UserType model)
        {
            MessageHelperCreate res = new MessageHelperCreate();

            try
            {
                await _masterService.SaveUserType(model);

                return Ok(res);
            }
            catch (Exception ex)
            {
                res.StatusCode = 500;
                res.Message = ex.Message;
                return BadRequest(res);
            }
        }

        [HttpGet]
        [Route("GetAllUserType")]
        public async Task<IActionResult> GetAllUserType()
        {
            try
            {
                return Ok(await _masterService.GetAllUserType());
            }
            catch (Exception ex)
            {
                MessageHelper res = new MessageHelper
                {
                    StatusCode = 500,
                    Message = ex.Message
                };
                return BadRequest(res);
            }
        }

        [HttpGet]
        [Route("GetUserTypeById")]
        public async Task<IActionResult> GetUserTypeById(long id)
        {
            try
            {
                return Ok(await _masterService.GetUserTypeById(id));
            }
            catch (Exception ex)
            {
                MessageHelper res = new MessageHelper
                {
                    StatusCode = 500,
                    Message = ex.Message
                };
                return BadRequest(res);
            }
        }

        [HttpGet]
        [Route("DeleteUserTypeById")]
        public async Task<IActionResult> DeleteUserTypeById(long id)
        {
            try
            {
                MessageHelper res = new MessageHelper
                {
                    StatusCode = 200,
                    Message = "Delete Successfully !!!"
                };
                await _masterService.DeleteUserTypeById(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                MessageHelper res = new MessageHelper
                {
                    StatusCode = 500,
                    Message = ex.Message
                };
                return BadRequest(res);
            }
        }
        #endregion

        #region ========== Account ==================
        [HttpPost]
        [Route("SaveAccount")]
        public async Task<IActionResult> SaveAccount(Account model)
        {
            MessageHelperCreate res = new MessageHelperCreate();

            try
            {
                await _masterService.SaveAccount(model);

                return Ok(res);
            }
            catch (Exception ex)
            {
                res.StatusCode = 500;
                res.Message = ex.Message;
                return BadRequest(res);
            }
        }

        [HttpGet]
        [Route("GetAllAccount")]
        public async Task<IActionResult> GetAllAccount()
        {
            try
            {
                return Ok(await _masterService.GetAllAccount());
            }
            catch (Exception ex)
            {
                MessageHelper res = new MessageHelper
                {
                    StatusCode = 500,
                    Message = ex.Message
                };
                return BadRequest(res);
            }
        }

        [HttpGet]
        [Route("GetAccountById")]
        public async Task<IActionResult> GetAccountById(long id)
        {
            try
            {
                return Ok(await _masterService.GetAccountById(id));
            }
            catch (Exception ex)
            {
                MessageHelper res = new MessageHelper
                {
                    StatusCode = 500,
                    Message = ex.Message
                };
                return BadRequest(res);
            }
        }

        [HttpGet]
        [Route("DeleteAccountById")]
        public async Task<IActionResult> DeleteAccountById(long id)
        {
            try
            {
                MessageHelper res = new MessageHelper
                {
                    StatusCode = 200,
                    Message = "Delete Successfully !!!"
                };
                await _masterService.DeleteAccountById(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                MessageHelper res = new MessageHelper
                {
                    StatusCode = 500,
                    Message = ex.Message
                };
                return BadRequest(res);
            }
        }
        #endregion

        #region ========== Account Package ==================
        [HttpPost]
        [Route("SaveAccountPackage")]
        public async Task<IActionResult> SaveAccountPackage(AccountPackage model)
        {
            MessageHelperCreate res = new MessageHelperCreate();

            try
            {
                await _masterService.SaveAccountPackage(model);

                return Ok(res);
            }
            catch (Exception ex)
            {
                res.StatusCode = 500;
                res.Message = ex.Message;
                return BadRequest(res);
            }
        }

        [HttpGet]
        [Route("GetAllAccountPackage")]
        public async Task<IActionResult> GetAllAccountPackage()
        {
            try
            {
                return Ok(await _masterService.GetAllAccountPackage());
            }
            catch (Exception ex)
            {
                MessageHelper res = new MessageHelper
                {
                    StatusCode = 500,
                    Message = ex.Message
                };
                return BadRequest(res);
            }
        }

        [HttpGet]
        [Route("GetAccountPackageById")]
        public async Task<IActionResult> GetAccountPackageById(long id)
        {
            try
            {
                return Ok(await _masterService.GetAccountPackageById(id));
            }
            catch (Exception ex)
            {
                MessageHelper res = new MessageHelper
                {
                    StatusCode = 500,
                    Message = ex.Message
                };
                return BadRequest(res);
            }
        }

        [HttpGet]
        [Route("DeleteAccountPackageById")]
        public async Task<IActionResult> DeleteAccountPackageById(long id)
        {
            try
            {
                MessageHelper res = new MessageHelper
                {
                    StatusCode = 200,
                    Message = "Delete Successfully !!!"
                };
                await _masterService.DeleteAccountPackageById(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                MessageHelper res = new MessageHelper
                {
                    StatusCode = 500,
                    Message = ex.Message
                };
                return BadRequest(res);
            }
        }
        #endregion

        #region ============== District ==============
        [HttpPost]
        [Route("SaveDistrict")]
        public async Task<IActionResult> SaveDistrict(District model)
        {
            MessageHelperCreate res = new MessageHelperCreate();
            try
            {
                await _masterService.SaveDistrict(model);

                return Ok(res);
            }
            catch (Exception ex)
            {
                res.StatusCode = 500;
                res.Message = ex.Message;
                return BadRequest(res);
            }
        }

        [HttpGet]
        [Route("GetAllDistrict")]
        public async Task<IActionResult> GetAllDistrict()
        {
            try
            {
                return Ok(await _masterService.GetAllDistrict());
            }
            catch (Exception ex)
            {
                MessageHelper res = new MessageHelper
                {
                    StatusCode = 500,
                    Message = ex.Message
                };
                return BadRequest(res);
            }
        }

        [HttpGet]
        [Route("GetDistrictById")]
        public async Task<IActionResult> GetDistrictById(long id)
        {
            try
            {
                return Ok(await _masterService.GetDistrictById(id));
            }
            catch (Exception ex)
            {
                MessageHelper res = new MessageHelper
                {
                    StatusCode = 500,
                    Message = ex.Message
                };
                return BadRequest(res);
            }
        }

        [HttpGet]
        [Route("DeleteDistrictById")]
        public async Task<IActionResult> DeleteDistrictById(long id)
        {
            try
            {
                MessageHelper res = new MessageHelper
                {
                    StatusCode = 200,
                    Message = "Delete Successfully !!!"
                };
                await _masterService.DeleteDistrictById(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                MessageHelper res = new MessageHelper
                {
                    StatusCode = 500,
                    Message = ex.Message
                };
                return BadRequest(res);
            }
        }

        #endregion

        #region ============== Division ==============
        [HttpPost]
        [Route("SaveDivision")]
        public async Task<IActionResult> SaveDivision(Division model)
        {
            MessageHelperCreate res = new MessageHelperCreate();
            try
            {
                await _masterService.SaveDivision(model);

                return Ok(res);
            }
            catch (Exception ex)
            {
                res.StatusCode = 500;
                res.Message = ex.Message;
                return BadRequest(res);
            }
        }

        [HttpGet]
        [Route("GetAllDivision")]
        public async Task<IActionResult> GetAllDivision()
        {
            try
            {
                return Ok(await _masterService.GetAllDivision());
            }
            catch (Exception ex)
            {
                MessageHelper res = new MessageHelper
                {
                    StatusCode = 500,
                    Message = ex.Message
                };
                return BadRequest(res);
            }

        }
        [HttpGet]
        [Route("GetDivisionById")]
        public async Task<IActionResult> GetDivisionById(long id)
        {
            try
            {
                return Ok(await _masterService.GetDivisionById(id));
            }
            catch (Exception ex)
            {
                MessageHelper res = new MessageHelper
                {
                    StatusCode = 500,
                    Message = ex.Message
                };
                return BadRequest(res);
            }
        }

        [HttpGet]
        [Route("DeleteDivisionById")]
        public async Task<IActionResult> DeleteDivisionById(long id)
        {
            try
            {
                MessageHelper res = new MessageHelper
                {
                    StatusCode = 200,
                    Message = "Delete Successfully !!!"
                };
                await _masterService.DeleteDivisionById(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                MessageHelper res = new MessageHelper
                {
                    StatusCode = 500,
                    Message = ex.Message
                };
                return BadRequest(res);
            }
        }
        #endregion

        #region ============== Gender ==============
        [HttpPost]
        [Route("SaveGender")]
        public async Task<IActionResult> SaveGender(Gender model)
        {
            MessageHelperCreate res = new MessageHelperCreate();
            try
            {
                await _masterService.SaveGender(model);

                return Ok(res);
            }
            catch (Exception ex)
            {
                res.StatusCode = 500;
                res.Message = ex.Message;
                return BadRequest(res);
            }
        }

        [HttpGet]
        [Route("GetAllGender")]
        public async Task<IActionResult> GetAllGender()
        {
            try
            {
                return Ok(await _masterService.GetAllGender());
            }
            catch (Exception ex)
            {
                MessageHelper res = new MessageHelper
                {
                    StatusCode = 500,
                    Message = ex.Message
                };
                return BadRequest(res);
            }

        }
        [HttpGet]
        [Route("GetGenderById")]
        public async Task<IActionResult> GetGenderById(long id)
        {
            try
            {
                return Ok(await _masterService.GetGenderById(id));
            }
            catch (Exception ex)
            {
                MessageHelper res = new MessageHelper
                {
                    StatusCode = 500,
                    Message = ex.Message
                };
                return BadRequest(res);
            }
        }
        [HttpGet]
        [Route("DeleteGenderById")]
        public async Task<IActionResult> DeleteGenderById(long id)
        {
            try
            {
                MessageHelper res = new MessageHelper
                {
                    StatusCode = 200,
                    Message = "Delete Successfully !!!"
                };
                await _masterService.DeleteGenderById(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                MessageHelper res = new MessageHelper
                {
                    StatusCode = 500,
                    Message = ex.Message
                };
                return BadRequest(res);
            }
        }
        #endregion

        #region ============== Religion ==============
        [HttpPost]
        [Route("SaveReligion")]
        public async Task<IActionResult> SaveReligion(Religion model)
        {
            MessageHelperCreate res = new MessageHelperCreate();
            MessageHelperUpdate resUpdate = new MessageHelperUpdate();
            try
            {
                await _masterService.SaveReligion(model);
                if (model.IntReligionId > 0)
                {
                    resUpdate.AutoId = model.IntReligionId;
                    return Ok(resUpdate);
                }
                else
                {
                    res.AutoId = model.IntReligionId;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                res.StatusCode = 500;
                res.Message = ex.Message;
                return BadRequest(res);
            }
        }
        [HttpGet]
        [Route("GetAllReligion")]
        public async Task<IActionResult> GetAllReligion()
        {
            try
            {
                return Ok(await _masterService.GetAllReligion());
            }
            catch (Exception ex)
            {
                MessageHelper res = new MessageHelper
                {
                    StatusCode = 500,
                    Message = ex.Message
                };
                return BadRequest(res);
            }

        }

        [HttpGet]
        [Route("GetReligionById")]
        public async Task<IActionResult> GetReligionById(long id)
        {
            try
            {
                return Ok(await _masterService.GetReligionById(id));
            }
            catch (Exception ex)
            {
                MessageHelper res = new MessageHelper
                {
                    StatusCode = 500,
                    Message = ex.Message
                };
                return BadRequest(res);
            }
        }

        [HttpGet]
        [Route("DeleteReligionById")]
        public async Task<IActionResult> DeleteReligionById(long id)
        {
            try
            {
                MessageHelper res = new MessageHelper
                {
                    StatusCode = 200,
                    Message = "Delete Successfully !!!"
                };
                await _masterService.DeleteReligionById(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                MessageHelper res = new MessageHelper
                {
                    StatusCode = 500,
                    Message = ex.Message
                };
                return BadRequest(res);
            }
        }
        #endregion

        #region ============== ONLY GET ALL ==============
        [HttpGet]
        [Route("GetAllBankWallet")]
        public async Task<IActionResult> GetAllBankWallet()
        {
            try
            {
                return Ok(await _masterService.GetAllBankWallet());
            }
            catch (Exception ex)
            {
                MessageHelper res = new MessageHelper
                {
                    StatusCode = 500,
                    Message = ex.Message
                };
                return BadRequest(res);
            }

        }
        [HttpGet]
        [Route("GetAllBloodGroup")]
        public async Task<IActionResult> GetAllBloodGroup()
        {
            try
            {
                return Ok(await _masterService.GetAllBloodGroup());
            }
            catch (Exception ex)
            {
                MessageHelper res = new MessageHelper
                {
                    StatusCode = 500,
                    Message = ex.Message
                };
                return BadRequest(res);
            }

        }
        [HttpGet]
        [Route("GetAllCountry")]
        public async Task<IActionResult> GetAllCountry()
        {
            try
            {
                return Ok(await _masterService.GetAllCountry());
            }
            catch (Exception ex)
            {
                MessageHelper res = new MessageHelper
                {
                    StatusCode = 500,
                    Message = ex.Message
                };
                return BadRequest(res);
            }

        }
        [HttpGet]
        [Route("GetAllCurrency")]
        public async Task<IActionResult> GetAllCurrency()
        {
            try
            {
                return Ok(await _masterService.GetAllCurrency());
            }
            catch (Exception ex)
            {
                MessageHelper res = new MessageHelper
                {
                    StatusCode = 500,
                    Message = ex.Message
                };
                return BadRequest(res);
            }

        }
        [HttpGet]
        [Route("GetAllEducationDegree")]
        public async Task<IActionResult> GetAllEducationDegree()
        {
            try
            {
                return Ok(await _masterService.GetAllEducationDegree());
            }
            catch (Exception ex)
            {
                MessageHelper res = new MessageHelper
                {
                    StatusCode = 500,
                    Message = ex.Message
                };
                return BadRequest(res);
            }

        }
        [HttpGet]
        [Route("GetAllEducationFieldOfStudy")]
        public async Task<IActionResult> GetAllEducationFieldOfStudy()
        {
            try
            {
                return Ok(await _masterService.GetAllEducationFieldOfStudy());
            }
            catch (Exception ex)
            {
                MessageHelper res = new MessageHelper
                {
                    StatusCode = 500,
                    Message = ex.Message
                };
                return BadRequest(res);
            }

        }
        [HttpGet]
        [Route("GetAllURLs")]
        public async Task<IActionResult> GetAllURLs()
        {
            try
            {
                return Ok(await _masterService.GetAllURLs());
            }
            catch (Exception ex)
            {
                MessageHelper res = new MessageHelper
                {
                    StatusCode = 500,
                    Message = ex.Message
                };
                return BadRequest(res);
            }

        }

        #endregion

        #region ================ Announcement ===============
        [HttpPost]
        [Route("CreateAnnouncement")]
        public async Task<IActionResult> CreateAnnouncement(AnnouncementViewModel obj)
        {
            try
            {
                var data = new Announcement()
                {
                    IntAnnouncementId = obj.IntAnnouncementId,
                    IntAccountId = obj.IntAccountId,
                    IntBusinessUnitId = obj.IntBusinessUnitId,
                    StrTitle = obj.StrTitle,
                    StrDetails = obj.StrDetails,
                    IntTypeId = obj.IntTypeId,
                    StrTypeName = obj.StrTypeName,
                    DteExpiredDate = obj.DteExpiredDate,
                    DteCreatedAt = obj.DteCreatedAt,
                    IntCreatedBy = obj.IntCreatedBy,
                    IsActive = obj.IsActive
                };

                var res = await _masterService.CRUDTblAnnouncement(data);

                return Ok(res);

            }
            catch (Exception EX)
            {
                throw EX;
            }
        }

        [HttpPost]
        [Route("CreateEditAnnouncement")]
        public async Task<IActionResult> CreateEditAnnouncement(AnnouncementCommonDTO model)
        {
            try
            {
                return Ok(await _masterService.CreateEditAnnouncement(model));
            }
            catch (Exception EX)
            {
                return BadRequest(EX.Message);
            }
        }

        [HttpGet]
        [Route("GetAllAnnouncement")]
        public async Task<IActionResult> GetAllAnnouncement(int BusinessUnitId, int year)
        {
            try
            {
                var res = await _masterService.GetAllAnnouncement(BusinessUnitId, year);
                return Ok(res);

            }
            catch (Exception EX)
            {
                throw EX;
            }
        }

        [HttpGet]
        [Route("GetAnnouncementById")]
        public async Task<IActionResult> GetAnnouncementById(int intId)
        {
            try
            {
                var res = await _masterService.GetAnnouncementById(intId);
                return Ok(res);

            }
            catch (Exception EX)
            {
                throw EX;
            }
        }

        [HttpGet]
        [Route("GetAnnouncementListById")]
        public async Task<IActionResult> GetAnnouncementListById(int intId)
        {
            try
            {
                var res = await _masterService.GetAnnouncementListById(intId);
                return Ok(res);

            }
            catch (Exception EX)
            {
                throw EX;
            }
        }

        [HttpGet]
        [Route("GetAnnouncement")]
        public async Task<IActionResult> GetAnnouncement(int YearId)
        {
            try
            {
                BaseVM tokenData = await AuthExtension.GetDataFromJwtToken(GetDataFromJwtTokenRequestType.TokenData, null, null);
                if (tokenData.accountId == -1)
                {
                    return BadRequest(new MessageHelperAccessDenied());
                }

                var res = await _masterService.GetAnnouncement(tokenData.employeeId, tokenData.accountId, tokenData.businessUnitId, YearId);
                return Ok(res);

            }
            catch (Exception EX)
            {
                return BadRequest(new MessageHelperError{Message = EX.Message});
            }
        }

        [HttpPut]
        [Route("DeleteAnnouncement")]
        public async Task<IActionResult> DeleteAnnouncement(AnnouncementCommonDTO obj)
        {
            try
            {
                var res = await _masterService.DeleteAnnouncement(obj);
                return Ok(res);
            }
            catch (Exception EX)
            {
                throw EX;
            }
        }
        #endregion

        #region =========== All Landing ============
        [HttpGet]
        [Route("PeopleDeskAllLanding")]
        public async Task<IActionResult> PeopleDeskAllLanding(string? TableName, long? AccountId, long? BusinessUnitId, long? intId, long? intStatusId, DateTime? FromDate, DateTime? ToDate,
            long? LoanTypeId, long? DeptId, long? DesigId, long? EmpId, long? MinimumAmount, long? MaximumAmount, long? MovementTypeId, DateTime? ApplicationDate, int? StatusId,
            long? LoggedEmployeeId)
        {
            try
            {
                /*
                 * BusinessUnit 
                 * * Department 
                 * * * Designation 
                 */
                if (!string.IsNullOrEmpty(TableName))
                {
                    return Ok(JsonConvert.SerializeObject(await _masterService.PeopleDeskAllLanding(TableName, AccountId, BusinessUnitId, intId, intStatusId, FromDate, ToDate,
                        LoanTypeId, DeptId, DesigId, EmpId, MinimumAmount, MaximumAmount, MovementTypeId, ApplicationDate, StatusId, LoggedEmployeeId)));
                }
                else
                {
                    throw new Exception("invalid data");
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion




        [AllowAnonymous]
        [HttpGet]
        [Route("GetAllDistrictAllowAnonymous")]
        public async Task<IActionResult> GetAllDistrictAllowAnonymous()
        {
            try
            {
                return Ok(await _masterService.GetAllDistrict());
            }
            catch (Exception ex)
            {
                MessageHelper res = new MessageHelper
                {
                    StatusCode = 500,
                    Message = ex.Message
                };
                return BadRequest(res);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("POST_TO_DATA_SAVE")]
        public async Task<IActionResult> POST_TO_DATA_SAVE(POST_TO_DATA_VM model)
        {
            try
            {
                //DataTable dt = new DataTable();

                using (var connection = new SqlConnection(Connection.iPEOPLE_HCM))
                {
                    string sql = "dbo.sprPOST_TO_DATA_SAVE";
                    using (SqlCommand sqlCmd = new SqlCommand(sql, connection))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;

                        sqlCmd.Parameters.AddWithValue("@Name", model.Name);
                        sqlCmd.Parameters.AddWithValue("@Email", model.Email);

                       

                        connection.Open();

                        await sqlCmd.ExecuteNonQueryAsync();

                        connection.Close();
                    }
                }

                return Ok(true);
            }
            catch (Exception ex)
            {
                MessageHelper res = new MessageHelper
                {
                    StatusCode = 500,
                    Message = ex.Message
                };
                return BadRequest(res);
            }
        }

        public class POST_TO_DATA_VM
        {
            public string Name { get; set; }
            public string Email { get; set; }
        }

    }
}
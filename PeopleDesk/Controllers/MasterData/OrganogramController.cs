using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PeopleDesk.Models.MasterData;
using PeopleDesk.Services.MasterData.Interfaces;

namespace PeopleDesk.Controllers.Master
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganogramController : ControllerBase
    {
        private readonly IOrganogramService _organogram;
        public OrganogramController(IOrganogramService organogram)
        {
            _organogram = organogram;
        }

        [HttpGet]
        [Route("GetOrganogramTree")]
        public async Task<IActionResult> GetOrganogramTree(long businessUnitId)
        {
            try
            {
                var res = await _organogram.GetOrganogramTree(businessUnitId);
                return Ok(res);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [Route("OrganogramReConstruct")]
        public async Task<IActionResult> OrganogramReConstruct(List<OrganogramReConstructViewModel> objList)
        {
            try
            {
                var res = await _organogram.OrganogramReConstruct(objList);

                if (res.StatusCode == 200)
                {
                    return Ok(res);
                }
                else
                {
                    return BadRequest(res);
                }

            }
            catch (Exception ex)
            {
                return BadRequest(JsonConvert.SerializeObject(ex.Message));
            }
        }
    }
}


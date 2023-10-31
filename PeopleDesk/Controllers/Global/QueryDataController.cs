using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeopleDesk.Data;
using PeopleDesk.Data.Entity;
using PeopleDesk.Models.Global;
using System.Data;
using System.Dynamic;
using System.Linq.Expressions;

namespace PeopleDesk.Controllers.Global
{
    [Route("api/[controller]")]
    [ApiController]
    public class QueryDataController : ControllerBase
    {
        private readonly IQueryable<SampleModel> _sampleData;
        private readonly PeopleDeskContext _context;

        public QueryDataController(PeopleDeskContext _context)
        {
            this._context = _context;
        }

        [HttpGet]
        [Route("HashCodeGenerator")]
        public async Task<IActionResult> HashCodeGenerator(long employeeId, int totalGenerate)
        {
            await Task.Delay(1 * 1000 * 60);

            string[] Alphabet = {"A", "N", "B", "O", "C", "P", "D", "Q", "E", "R", "F", "S", "G",
                                "T", "H", "U", "I", "V", "J", "W", "K", "X", "L",  "Y", "M", "Z",

                                "Z", "Y", "X", "W", "V", "U", "T", "S", "R", "Q", "P", "O", "N",
                                "M", "L", "K", "J", "I", "H", "G", "F", "E", "D", "C", "B", "A",

                                "A", "N", "B", "O", "C", "P", "D", "Q", "E", "R", "F", "S", "G",
                                "T", "H", "U", "I", "V", "J", "W", "K", "X", "L",  "Y", "M", "Z",

                                "Z", "Y", "X", "W", "V", "U", "T", "S", "R", "Q", "P", "O", "N",
                                "M", "L", "K", "J", "I", "H", "G", "F", "E", "D", "C", "B", "A",};

            string dateTimes = DateTime.Now.ToString("yyyyMMddHHmm");
            List<GeneratedHashCode> generatedCode = new List<GeneratedHashCode>();
            List<string> generatedCodeString = new List<string>();
            string codeHead = string.Empty;
            char[] dteChars = Convert.ToString(dateTimes).ToCharArray();

            for (int i = 0; i < dateTimes.Length; i++)
            {
                codeHead = codeHead + Alphabet[Convert.ToInt32(dteChars[i].ToString())];
            }

            totalGenerate = totalGenerate + 1000;

            for (int i = 1000; i < totalGenerate; i++)
            {
                string code = string.Empty;
                char[] chars = Convert.ToString(i).ToCharArray();

                code = codeHead + Alphabet[Convert.ToInt32(chars[0].ToString())] + Alphabet[Convert.ToInt32(chars[1].ToString())]
                                + Alphabet[Convert.ToInt32(chars[2].ToString())] + Alphabet[Convert.ToInt32(chars[3].ToString())];

                generatedCodeString.Add(code);
                generatedCode.Add(new GeneratedHashCode
                {
                    StrHashCode = code,
                    DteCreatedAt = DateTime.Now,
                    IntCreatedBy = employeeId,
                });
            }

            generatedCode = (from gc in generatedCode
                             where _context.GeneratedHashCodes.Where(x => x.StrHashCode == gc.StrHashCode).Count() == 0
                             select gc).ToList();

            if (generatedCode.Count > 0)
            {
                await _context.GeneratedHashCodes.AddRangeAsync(generatedCode);
                await _context.SaveChangesAsync();
            }

            string response = "Total: " + generatedCode.Count() + " numbers of HashCode has been generated successfully";

            return Ok(response);
        }

        [HttpGet]
        [Route("CombineDataSetByWorkplaceGDeptDesigSupEmpType")]
        public async Task<IActionResult> CombineDataSetByWorkplaceGDeptDesigSupEmpType(long? BusinessUnitId, long? WorkplaceGroupId, long? DeptId, long? DesigId, long? SupervisorId, long? EmpType)
        {
            try
            {
                List<CustomDTO> data = await (from E in _context.EmpEmployeeBasicInfos
                                              join ed in _context.EmpEmployeeBasicInfoDetails on E.IntEmployeeBasicInfoId equals ed.IntEmployeeId
                                              join B in _context.MasterBusinessUnits on E.IntBusinessUnitId equals B.IntBusinessUnitId into BB
                                              from b in BB.DefaultIfEmpty()
                                              join W in _context.MasterWorkplaceGroups on E.IntWorkplaceId equals W.IntWorkplaceGroupId into WW
                                              from w in WW.DefaultIfEmpty()
                                              join WPG in _context.MasterWorkplaces on w.IntWorkplaceGroupId equals WPG.IntWorkplaceGroupId into WWPG
                                              from ww in WWPG.DefaultIfEmpty()
                                              join D in _context.MasterDepartments on E.IntDepartmentId equals D.IntDepartmentId into DD
                                              from d in DD.DefaultIfEmpty()
                                              join DES in _context.MasterDesignations on E.IntDesignationId equals DES.IntDesignationId into DESS
                                              from des in DESS.DefaultIfEmpty()
                                              join S in _context.EmpEmployeeBasicInfos on E.IntSupervisorId equals S.IntEmployeeBasicInfoId into SS
                                              from s in SS.DefaultIfEmpty()
                                              join ET in _context.MasterEmploymentTypes on E.IntEmploymentTypeId equals ET.IntEmploymentTypeId into ETT
                                              from et in ETT.DefaultIfEmpty()
                                              where (ed.IntEmployeeStatusId == 1 || ed.IntEmployeeStatusId == 4)
                                                  && (BusinessUnitId > 0 ? BusinessUnitId == E.IntBusinessUnitId : true)
                                                  && (WorkplaceGroupId > 0 ? WorkplaceGroupId == w.IntWorkplaceGroupId : true)
                                                  && (DeptId > 0 ? DeptId == E.IntDepartmentId : true)
                                                  && (DesigId > 0 ? DesigId == E.IntDesignationId : true)
                                                  && (SupervisorId > 0 ? SupervisorId == E.IntSupervisorId : true)
                                                  && (EmpType > 0 ? EmpType == E.IntEmploymentTypeId : true)


                                              select new CustomDTO
                                              {
                                                  IntEmployeeId = E.IntEmployeeBasicInfoId,
                                                  IntEmploymentTypeId = E.IntEmploymentTypeId,
                                                  EmploymentTypeName = et.StrEmploymentType,
                                                  StrEmployeeName = E.StrEmployeeName,
                                                  StrEmployeeCode = E.StrEmployeeCode,
                                                  IntBusinessUnitId = b.IntBusinessUnitId,
                                                  StrBusinessUnitName = b.StrBusinessUnit,
                                                  IntWorkplaceGroupId = w.IntWorkplaceGroupId,
                                                  StrWorkplaceGroupName = w.StrWorkplaceGroup,
                                                  IntDepartmentId = d.IntDepartmentId,
                                                  StrDepartmentName = d.StrDepartment,
                                                  IntDesignationId = des.IntDesignationId,
                                                  StrDesignationName = des.StrDesignation,
                                                  IntSupervisorId = E.IntSupervisorId,
                                                  SupervisorName = s.StrEmployeeName
                                              }).AsNoTracking().AsQueryable().ToListAsync();

                CombineDataSetByWorkplaceGDeptDesigSupEmpType groupResult = new CombineDataSetByWorkplaceGDeptDesigSupEmpType
                {
                    WorkplaceGroupList = data.Where(x => x.IntWorkplaceGroupId > 0).GroupBy(x => x.IntWorkplaceGroupId).Select(x => new WorkplaceGroupList { Id = (long)x.Key, Name = x?.FirstOrDefault()?.StrWorkplaceGroupName }).ToList(),
                    DepartmentList = data.Where(x => x.IntDepartmentId > 0).GroupBy(x => x.IntDepartmentId).Select(x => new DepartmentList { Id = (long)x.Key, Name = x?.FirstOrDefault()?.StrDepartmentName }).ToList(),
                    DesignationList = data.Where(x => x.IntDesignationId > 0).GroupBy(x => x.IntDesignationId).Select(x => new DesignationList { Id = (long)x.Key, Name = x?.FirstOrDefault()?.StrDesignationName }).ToList(),
                    SupervisorList = data.Where(x => x.IntSupervisorId > 0).GroupBy(x => x.IntSupervisorId).Select(x => new SupervisorList { Id = (long)x.Key, Name = x?.FirstOrDefault()?.SupervisorName }).ToList(),
                    EmploymentTypeList = data.Where(x => x.IntEmploymentTypeId > 0).GroupBy(x => x.IntEmploymentTypeId).Select(x => new EmploymentTypeList { Id = (long)x.Key, Name = x?.FirstOrDefault()?.EmploymentTypeName }).ToList(),
                    EmployeeList = data.Where(x => x.IntEmployeeId > 0).GroupBy(x => x.IntEmployeeId).Select(x => new EmployeeList { Id = (long)x.Key, Name = (x?.FirstOrDefault()?.StrEmployeeName + " (" + x?.FirstOrDefault()?.StrEmployeeCode + ")") }).ToList()
                };
                return Ok(groupResult);
            }
            catch (Exception ex)
            {
                return StatusCode(404, ex.Message);
            }
        }

        [HttpGet]
        [Route("CombineDataSetByWorkplaceGDeptDesigSupEmpTypeByAccountId")]
        public async Task<IActionResult> CombineDataSetByWorkplaceGDeptDesigSupEmpTypeByAccountId(long AccountId, long? BusinessUnitId, long? WorkplaceGroupId, long? WorkplaceId, long? DeptId, long? DesigId, long? EmpType)
        {
            try
            {
                List<CustomDTO2> data = await (from A in _context.Accounts
                                               where A.IntAccountId == AccountId && A.IsActive == true
                                               join B in _context.MasterBusinessUnits on A.IntAccountId equals B.IntAccountId into BB
                                               from b in BB.DefaultIfEmpty()
                                               join WG in _context.MasterWorkplaceGroups on A.IntAccountId equals WG.IntAccountId into WW2
                                               from wg in WW2.DefaultIfEmpty()
                                               join W in _context.MasterWorkplaces on wg.IntWorkplaceGroupId equals W.IntWorkplaceGroupId into WW
                                               from w in WW.DefaultIfEmpty()
                                               join D in _context.MasterDepartments on b.IntBusinessUnitId equals D.IntBusinessUnitId into DD
                                               from d in DD.DefaultIfEmpty()
                                               join DES in _context.MasterDesignations on b.IntBusinessUnitId equals DES.IntBusinessUnitId into DESS
                                               from des in DESS.DefaultIfEmpty()
                                               from et in _context.MasterEmploymentTypes.Where(x => x.IsActive == true && (x.IntAccountId == AccountId || x.IntAccountId == 0)).ToList()
                                               where (b != null ? b.IsActive == true : true)
                                               && (wg != null ? wg.IsActive == true : true)
                                               && (w != null ? w.IsActive == true : true)
                                               && (d != null ? d.IsActive == true : true)
                                               && (des != null ? des.IsActive == true : true)
                                               && (BusinessUnitId > 0 ? BusinessUnitId == b.IntBusinessUnitId : true
                                                       && WorkplaceGroupId > 0 ? WorkplaceGroupId == wg.IntWorkplaceGroupId : true
                                                       && WorkplaceId > 0 ? WorkplaceId == w.IntWorkplaceId : true
                                                       && DeptId > 0 ? DeptId == d.IntDepartmentId : true
                                                       && DesigId > 0 ? DesigId == des.IntDesignationId : true
                                                       && EmpType > 0 ? EmpType == et.IntEmploymentTypeId : true)

                                               select new CustomDTO2
                                               {
                                                   IntBusinessUnitId = b.IntBusinessUnitId,
                                                   StrBusinessUnitName = b.StrBusinessUnit,
                                                   IntWorkplaceGroupId = wg.IntWorkplaceGroupId,
                                                   StrWorkplaceGroupName = wg.StrWorkplaceGroup,
                                                   IntWorkplaceId = w.IntWorkplaceId,
                                                   StrWorkplaceName = w.StrWorkplace,
                                                   IntDepartmentId = d.IntDepartmentId,
                                                   StrDepartmentName = d.StrDepartment,
                                                   IntDesignationId = des.IntDesignationId,
                                                   StrDesignationName = des.StrDesignation,
                                                   IntEmploymentTypeId = et.IntEmploymentTypeId,
                                                   EmploymentTypeName = et.StrEmploymentType
                                               }).AsNoTracking().ToListAsync();

                CombineDataSetByWorkplaceGDeptDesigSupEmpTypeByAccountId groupResult = new CombineDataSetByWorkplaceGDeptDesigSupEmpTypeByAccountId
                {
                    BusinessUnitList = data.Where(x => x.IntBusinessUnitId > 0).GroupBy(x => x.IntBusinessUnitId).Select(x => new BusinessUnitList { Id = (long)x.Key, Name = x?.FirstOrDefault()?.StrBusinessUnitName }).ToList(),
                    WorkplaceGroupList = data.Where(x => x.IntWorkplaceGroupId > 0).GroupBy(x => x.IntWorkplaceGroupId).Select(x => new WorkplaceGroupList { Id = (long)x.Key, Name = x?.FirstOrDefault()?.StrWorkplaceGroupName }).ToList(),
                    WorkplaceList = data.Where(x => x.IntWorkplaceId > 0).GroupBy(x => x.IntWorkplaceId).Select(x => new WorkplaceList { Id = (long)x.Key, Name = x?.FirstOrDefault()?.StrWorkplaceName }).ToList(),
                    DepartmentList = data.Where(x => x.IntDepartmentId > 0).GroupBy(x => x.IntDepartmentId).Select(x => new DepartmentList { Id = (long)x.Key, Name = x?.FirstOrDefault()?.StrDepartmentName }).ToList(),
                    DesignationList = data.Where(x => x.IntDesignationId > 0).GroupBy(x => x.IntDesignationId).Select(x => new DesignationList { Id = (long)x.Key, Name = x?.FirstOrDefault()?.StrDesignationName }).ToList(),
                    EmploymentTypeList = data.Where(x => x.IntEmploymentTypeId > 0).GroupBy(x => x.IntEmploymentTypeId).Select(x => new EmploymentTypeList { Id = (long)x.Key, Name = x?.FirstOrDefault()?.EmploymentTypeName }).ToList()
                };

                return Ok(groupResult);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("GetGlobalCultures")]
        public async Task<IActionResult> GetGlobalCultures(long AccountId, string Language)
        {
            try
            {
                var dt = await _context.GlobalCultures.Where(x => x.IntAccountId == AccountId && x.StrLanguage.ToLower() == Language.ToLower() && x.IsActive == true).AsNoTracking().AsQueryable().ToListAsync();

                var dynamicObject = new ExpandoObject() as IDictionary<string, Object>;
                foreach (var property in dt)
                {
                    dynamicObject.Add(property.StrKey, property.StrLabel);
                }
                return Ok(dynamicObject);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }





    private class SampleModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string City { get; set; }
        // Add other properties as needed
    }
        [HttpGet]
        [Route("SampleRepository")]
        public IActionResult SampleRepository()
        {
            // Replace this with your actual data source
            var _sampleData = new List<SampleModel>
                                {
                                    new SampleModel { Id = 1, Name = "John", Age = 25, City = "New York" },
                                    new SampleModel { Id = 2, Name = "Jane", Age = 30, City = "New York" },
                                    new SampleModel { Id = 3, Name = "Bob", Age = 35, City = "New York" },
                                }.AsQueryable();

            string propertyName = "Age";
            string comparisonOperator = ">=";
            int filterValue = 18;

            var filterExpression = _sampleData.Where(CreateFilterExpression(propertyName, comparisonOperator, filterValue));

            return Ok(filterExpression);
        } 
        
        [HttpGet]
        [Route("SampleRepositoryMultiple")]
        public IActionResult SampleRepositoryMultiple()
        {
            // Replace this with your actual data source
            var _sampleData = new List<SampleModel>
                                {
                                    new SampleModel { Id = 1, Name = "John", Age = 25 },
                                    new SampleModel { Id = 2, Name = "Jane", Age = 30 },
                                    new SampleModel { Id = 3, Name = "Bob", Age = 35 },
                                }.AsQueryable();


            var nameFilterExpression = CreateFilterExpression("Name", "==", "John");
            var ageFilterExpression = CreateFilterExpression("Age", ">=", 18);
            var cityFilterExpression = CreateFilterExpression("City", "==", "New York");

            var combinedExpression = Expression.AndAlso(
                Expression.AndAlso(nameFilterExpression.Body, ageFilterExpression.Body),
                cityFilterExpression.Body
            );

            var finalExpression = Expression.Lambda<Func<SampleModel, bool>>(combinedExpression, nameFilterExpression.Parameters);

            var filterExpression = _sampleData.Where(finalExpression);

            return Ok(filterExpression);
        }

        private static Expression<Func<SampleModel, bool>> CreateFilterExpression(string propertyName, string comparisonOperator, object filterValue)
        {
            var parameter = Expression.Parameter(typeof(SampleModel), "x");
            var property = Expression.Property(parameter, propertyName);
            var constant = Expression.Constant(filterValue);

            Expression comparison;
            switch (comparisonOperator)
            {
                case "==":
                    comparison = Expression.Equal(property, constant);
                    break;
                case "!=":
                    comparison = Expression.NotEqual(property, constant);
                    break;
                case ">":
                    comparison = Expression.GreaterThan(property, constant);
                    break;
                case "<":
                    comparison = Expression.LessThan(property, constant);
                    break;
                case ">=":
                    comparison = Expression.GreaterThanOrEqual(property, constant);
                    break;
                case "<=":
                    comparison = Expression.LessThanOrEqual(property, constant);
                    break;
                default:
                    throw new NotSupportedException($"Comparison operator '{comparisonOperator}' is not supported.");
            }

            var lambda = Expression.Lambda<Func<SampleModel, bool>>(comparison, parameter);
            return lambda;
        }


    }
}
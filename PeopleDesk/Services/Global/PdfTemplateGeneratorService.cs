using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Wordprocessing;
using HCM.Helper;
using LanguageExt;
using LanguageExt.UnitsOfMeasure;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using PeopleDesk.Data;
using PeopleDesk.Data.Entity;
using PeopleDesk.Models.Employee;
using PeopleDesk.Services.Global.Interface;
using System.Globalization;
using System.Net;
using System.Text;
using static PeopleDesk.Models.Employee.TransferReportViewModel;
using static PeopleDesk.Models.Global.PdfViewModel;

namespace PeopleDesk.Services.Global
{
    public class PdfTemplateGeneratorService : IPdfTemplateGeneratorService
    {
        private string companyLogoUrl;
        private readonly string commonJsUrl;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly PeopleDeskContext _context;

        private readonly IHttpContextAccessor _httpContextAccessor;

        // private readonly IShippingMgmt _shipping;
        private string documentUrl;

        public PdfTemplateGeneratorService(PeopleDeskContext _context, IWebHostEnvironment _hostingEnvironment, IHttpContextAccessor _httpContextAccessor/*, IShippingMgmt _shipping*/)
        {
            this._hostingEnvironment = _hostingEnvironment;
            companyLogoUrl = _hostingEnvironment.ContentRootPath + "/wwwroot/ibos_logo.svg";
            commonJsUrl = _hostingEnvironment.ContentRootPath + "/wwwroot/pdf_common.js";
            this._context = _context;
            this._httpContextAccessor = _httpContextAccessor;
            documentUrl = "https://" + _httpContextAccessor.HttpContext.Request.Host.Value + "/api/Document/DownloadFile?id=";
            //this._shipping = _shipping;
        }

        public async Task<string> DailyAttendanceReportByEmployeeId(PdfDailyAttendanceReportViewModel obj)
        {
            EmployeeQryProfileAllViewModel employee = obj.EmployeeQryProfileAll;

            var sb = new StringBuilder();
            string cmyLogoUrl = _context.GlobalFileUrls.Where(x => x.IsActive == true && x.IntDocumentId == employee.BusinessUnit.StrLogoUrlId).Select(s => s.StrFileServerId).FirstOrDefault();

            if (!string.IsNullOrEmpty(cmyLogoUrl))
            {
                var webclient = new WebClient();
                byte[] img = webclient.DownloadData($"{documentUrl + employee.BusinessUnit.StrLogoUrlId}");
                Stream image = new MemoryStream(img);
                companyLogoUrl = Convert.ToBase64String(img);
            }

            sb.Append(@"<html>
                             <head>
                                <link rel='stylesheet' type='text/css' href='{0}' />
                             </head>
                                 <body>
                        <div class='print-page-main-body'>
                          <div class='print-page-body'>
                            <div>
                              <div  style='position:relative; width:100%'>
                              <div style='width:80px; position:absolute'>");

            sb.AppendFormat(@"<img class='companyImg' src='data:image/png;base64, {0}' alt='Pineapple' style='width:100%;' />", companyLogoUrl);

            sb.AppendFormat(@"
                            </div>
                            <div style='width:100%;display:block' >
                                <p  style='width:70%;text-align: center;display:block;font-style: normal;font-weight: bold;font-size: 30px;color: rgba(0, 0, 0, 0.7); margin:0 auto'>{0}</p>
                                <p  style='width:70%;text-align: center;font-style: normal;font-size: 20px;color: rgba(0, 0, 0, 0.7);margin:0 auto'>{1}</p>
                            </div>
                             </div>
                             <div class='company-social-info'>
                                <p>{2}</p>
                                <p>{3}</p>
                             </div>
                             </div>

                             <div class='main-report-body' style='margin-top: 40px'>
                             <div class='body-header'>
                                 <div class='employee_personal_info'>
                                 <table>
                                     <tbody>
                                     <tr>
                                         <td>Employee Name: </td>
                                         <td>{4} [<span>{5}</span>]</td>
                                     </tr>
                                     <tr>
                                         <td>Designation: </td>
                                         <td>{6}, {7}</td>
                                     </tr>
                                     <tr>
                                         <td>Department: </td>
                                         <td>{8}</td>
                                     </tr>
                                     </tbody>
                                 </table>
                                 </div>
                                 <div class='emp_other_info'>
                                 <table>
                                     <tbody>
                                     <tr>
                                         <td>Workplace Group: </td>
                                         <td>{9}</td>
                                     </tr>
                                     <tr>
                                         <td>Business Unit: </td>
                                         <td>{10}</td>
                                     </tr>
                                     <tr>
                                         <td>Supervisor: </td>
                                         <td>{11}</td>
                                     </tr>
                                     </tbody>
                                 </table>
                                 </div>
                             </div>",
                            employee?.BusinessUnit?.StrBusinessUnit,
                            employee?.BusinessUnit?.StrAddress,
                            employee?.BusinessUnit?.StrWebsiteUrl,
                            "",//employee?.BusinessUnit?.StrMobile,
                            employee?.EmployeeBasicInfo.StrEmployeeName,
                            employee?.EmployeeBasicInfo.StrEmployeeCode,
                            employee?.DesignationName,
                            employee?.EmploymentTypeName,
                            employee?.DepartmentName,
                            employee?.WorkplaceGroupName,
                            employee?.BusinessUnit.StrBusinessUnit,
                            employee?.SupervisorName);

            sb.AppendFormat(@"<div class='body_main_table' style='margin-top:40px'>
                                   <div class='table_header''>Attendance Details From {0} To {1}</div>
                                   <div class='table'>
                                     <table>
                                       <thead>
                                         <tr>
                                           <th>SL</th>
                                           <th>Date</th>
                                           <th>In-time</th>
                                           <th>Out-time</th>
                                           <th>Status</th>
                                           <th>Reason</th>
                                         </tr>
                                       </thead>
                                       <tbody>", obj.FromDate, obj.ToDate);

            long count = 1;
            foreach (var item in obj.DailyAttendanceReportViewModelList)
            {
                sb.AppendFormat(@"<tr style='page-break-inside: avoid;'>
                                     <td>{5}</td>
                                     <td>{0}</td>
                                     <td>{1}</td>
                                     <td>{2}</td>
                                     <td>{3}</td>
                                     <td>{4}</td>
                                 </tr>", Convert.ToDateTime(item?.Attendance).ToString("dd-MMMM-yyyy"), item?.InTime, item?.OutTime, item?.AttStatus, item?.Remarks, count);
                count++;
            }
            sb.Append(@"               </tbody>
                                     </table>
                                    </div>
                                </div>
                             </div>");
            sb.Append(@"<div class='footer'>
                                     <table class='footer_table'>
                                         <tbody>
                                         <tr>
                                             <td class='authorName'>Department Head</td>
                                             <td>Admin</td>
                                             <td>Accounts</td>
                                         </tr>
                                         </tbody>
                                     </table>
                                     <div class='footerNote'></div>");

            sb.AppendFormat(@"              </div>
                                        </div>
                                     </div>
                                 <script src='{0}'></script>
                             </body>
                         </html>", commonJsUrl);

            return sb.ToString();
        }

        public async Task<string> SeparationReportByEmployee(MasterBusinessUnit businessUnit, EmployeeSeparationReportViewModel SeperationObj)
        {
            var sb = new StringBuilder();
            string cmyLogoUrl = _context.GlobalFileUrls.Where(x => x.IsActive == true && x.IntDocumentId == businessUnit.StrLogoUrlId).Select(s => s.StrFileServerId).FirstOrDefault();

            if (!string.IsNullOrEmpty(cmyLogoUrl))
            {
                var webclient = new WebClient();
                byte[] img = webclient.DownloadData($"{documentUrl + businessUnit.StrLogoUrlId}");
                Stream image = new MemoryStream(img);
                companyLogoUrl = Convert.ToBase64String(img);
            }

            decimal? PF = SeperationObj?.pfAccountViewModel?.EmployeePFContribution == null ? 0 : SeperationObj?.pfAccountViewModel?.EmployeePFContribution;
            decimal? PFWithdraw = SeperationObj?.pfAccountViewModel?.TotalPFWithdraw == null ? 0 : SeperationObj?.pfAccountViewModel?.TotalPFWithdraw;
            decimal? Gratuity = SeperationObj?.pfAccountViewModel?.Gratuity == null ? 0 : SeperationObj?.pfAccountViewModel?.Gratuity;
            decimal? TotalPF = PF - PFWithdraw;

            string? inWord = await AmountInWords.ConvertToWords((TotalPF + Gratuity).ToString());

            sb.Append(@"<html>
                             <head>
                             </head>
                                 <body>
                                  <div class='print-page-main-body'>
                                     <div class='print-page-body'>
                                       <div>
                                         <div style='position:relative'>
                                           <div style='width:80px; position:absolute'>");

            sb.AppendFormat(@"<img src='data:image/png;base64, {0}' alt='Pineapple' style= 'width:100%' />", companyLogoUrl);

            sb.AppendFormat(@"
                                    </div>
                                    <div style='text-align: center; font-size: 25px; font-weight: 600;'>{0}</div>
                                    <div style='text-align: center'>{1}</div>
                                  </div>
                                  <div class='company-social-info'>
                                    <p>{2}</p>
                                    <p>{3}</p>
                                  </div>
                                </div>
                              </div>
                              <div class='main-report-body'>
                                <div class='body-header'>
                                </div>
                                <div class='body_main_table'>
                                  <div class='table_header'>Employee Separation Report</div>
                                  <table class='separation_table'>
                                    <tbody>
                                      <tr>
                                        <td width='15%' class='name'>Name</td>
                                        <td width='5%' style='float-right'>:{4}</td>
                                        <td width='30%' class='dotted_line'>{5}</td>
                                        <td width='15%' class='id'>ID</td>
                                        <td width='5%' style='float-right'>:</td>
                                        <td width='30%' class='dotted_line'>{6} </td>
                                      </tr>
                                      <tr>
                                        <td width='15%' class='name'>Department</td>
                                        <td width='5%' style='float-right'>:</td>
                                        <td width='30%' class='dotted_line'>{7}</td>
                                        <td width='15%' class='id'>Designation</td>
                                        <td width='5%' style='float-right'>:</td>
                                        <td width='30%' class='dotted_line'>{8} </td>
                                      </tr>

                                      <tr>
                                        <td width='15%' class='name'>Supervisor</td>
                                        <td width='5%' style='float-right'>:</td>
                                        <td width='30%' class='dotted_line'>{9}</td>
                                        <td width='15%' class='id'>Line Manager</td>
                                        <td width='5%' style='float-right'>:</td>
                                        <td width='30%' class='dotted_line'>{10} </td>
                                      </tr>
                                      <tr>
                                        <td width='15%' class='name'>Date of Joining</td>
                                        <td width='5%' style='float-right'>:</td>
                                        <td width='30%' class='dotted_line'>{11}</td>
                                        <td width='15%' class='id'>Confirmation Date</td>
                                        <td width='5%' style='float-right'>:</td>
                                        <td width='30%' class='dotted_line'>{12} </td>
                                      </tr>
                                      <tr>
                                        <td width='15%' class='name'>Separation Date</td>
                                        <td width='5%' style='float-right'>:</td>
                                        <td width='30%' class='dotted_line'>{13}</td>
                                        <td width='15%' class='id'>Last Working Date</td>
                                        <td width='5%' style='float-right'>:</td>
                                        <td width='30%' class='dotted_line'>{14} </td>
                                      </tr>
                                       <tr>
                                            <td height='15px'></td>
                                       </tr>
                                      <tr>
                                        <td  width='15%' class='name' style='vertical-align: top;'>Reason</td>
                                        <td width='5%' style='float-right;vertical-align: top;'>:</td>
                                        <td width='80%' colspan='4'>{15}</td>
                                      </tr>
                                      <tr>
                                        <td width='15%' class='name'>Provident Fund</td>
                                        <td width='5%' style='float-right'>:</td>
                                        <td width='80%' class='dotted_line' colspan='4'>{16}</td>
                                      </tr>
                                       <tr>
                                        <td width='15%' class='name'>Gratuity</td>
                                        <td width='5%' style='float-right'>:</td>
                                        <td width='80%' class='dotted_line' colspan='4'>{17}</td>
                                      </tr>
                                      <tr>
                                        <td width='15%' class='name'>Leave Encashment</td>
                                        <td width='5%' style='float-right'>:</td>
                                        <td width='80%' class='dotted_line' colspan='4'>{18}</td>
                                      </tr>
                                      <tr>
                                        <td width='15%' class='name'>Net Pay</td>
                                        <td width='5%' style='float-right'>:</td>
                                        <td width='80%' class='dotted_line' colspan='4'>{19}</td>
                                      </tr>
                                      <tr>
                                        <td width='15%' class='name'>In Words</td>
                                        <td width='5%' style='float-right'>:</td>
                                        <td width='80%' class='dotted_line' colspan='4'>{20}</td>
                                      </tr>
                                    </tbody>
                                  </table>
                                </div>
                              </div>

                             <div class='footer'>
                                <table class='footer_table'>
                                    <tbody>
                                    <tr>
                                        <td class='authorName'>Department Head</td>
                                        <td>Admin</td>
                                        <td>Accounts</td>
                                    </tr>
                                    </tbody>
                                </table>
                                <div class='footerNote'></div>
                                </div>
                            </div>
                        </div>
                        <script src='{21}'></script>
                        </body>

                        </html>",
                        businessUnit?.StrBusinessUnit,
                        businessUnit?.StrAddress,
                        businessUnit?.StrWebsiteUrl,
                        "",
                        "",
                        SeperationObj?.EmployeeInfoObj?.StrEmployeeName,
                        SeperationObj?.EmployeeInfoObj?.StrEmployeeCode,
                        SeperationObj?.DepartmentObj?.StrDepartment,
                        SeperationObj?.DesignationObj?.StrDesignation,
                        SeperationObj?.SupervisorObj?.StrEmployeeName,
                        SeperationObj?.LineManagerObj?.StrEmployeeName,
                        SeperationObj?.EmployeeInfoObj?.DteJoiningDate.Value.ToString("dd-MMM-yyyy hh:mm tt"),
                        SeperationObj?.EmployeeInfoObj?.DteConfirmationDate == null ? "" : SeperationObj?.EmployeeInfoObj?.DteConfirmationDate?.ToString("dd-MMM-yyyy hh:mm tt"),
                        SeperationObj?.SeperationObj?.DteCreatedAt.ToString("dd-MMM-yyyy hh:mm tt"),
                        SeperationObj?.SeperationObj?.DteLastWorkingDate?.ToString("dd-MMM-yyyy hh:mm tt"),
                        SeperationObj?.SeperationObj?.StrReason,
                        TotalPF,
                        Gratuity,
                        "0",
                        TotalPF + Gratuity,
                        inWord,
                        commonJsUrl);

            return sb.ToString();
        }

        public async Task<string> LoanDetailsReportByEmployee(LoanDetailsReportViewModel obj)
        {
            var sb = new StringBuilder();
            //companyLogoUrl = _hostingEnvironment.ContentRootPath + "/wwwroot/" + obj?.BusinessUnit?.StrLogoUrlforPdfReport;

            sb.AppendFormat(@"<html>
                        <head>
                        </head>
                        <body>
                          <div class='print-page-main-body'>
                            <div class='print-page-body'>
                              <div class='clearfix'>
                                <div class='company-info'>
                                  <div class='companyName'>
                                    <img class='companyImg' src='{0}' alt='Pineapple' width='22' height='17' />
                                    <p class='companyTitle'>{1}</p>
                                  </div>
                                  <div class='companyAddress'>{2}</div>
                                </div>
                                <div class='company-social-info'>
                                  <p>{3}</p>
                                  <p>{4}</p>
                                </div>
                              </div>
                              <div class='main-report-body'>
                                <div class='body-header'>
                                  <div class='employee_personal_info'>
                                    <table>
                                      <tbody>
                                        <tr>
                                          <td>Employee name</td>
                                          <td>{5} [<span>{6}</span>]</td>
                                        </tr>
                                        <tr>
                                          <td>Designation</td>
                                          <td>{7}, {8}</td>
                                        </tr>
                                        <tr>
                                          <td>Department</td>
                                          <td>{9}</td>
                                        </tr>

                                      </tbody>
                                    </table>
                                  </div>
                                  <div class='emp_other_info_part_One'>
                                    <table>
                                      <tbody>
                                        <tr>
                                          <td>Joining Date</td>
                                          <td>{10}</td>
                                        </tr>
                                        <tr>
                                          <td>Total Loan Amount</td>
                                          <td>{11}</td>
                                        </tr>
                                        <tr>
                                          <td>Installment Amount</td>
                                          <td>{12}</td>
                                        </tr>
                                        <tr>
                                          <td>No Of Installment</td>
                                          <td>{13}</td>
                                        </tr>
                                      </tbody>
                                    </table>
                                  </div>
                                  <div class='emp_other_info_part_two'>
                                    <table>
                                      <tbody>
                                        <tr>
                                          <td>Gross Salary</td>
                                          <td>{14}</td>
                                        </tr>
                                        <tr>
                                          <td>Basic Salary</td>
                                          <td>{15}</td>
                                        </tr>
                                        <tr>
                                          <td>Loan Type</td>
                                          <td>{16}</td>
                                        </tr>
                                        <tr>
                                          <td>Due Amount</td>
                                          <td>{17}</td>
                                        </tr>
                                        <tr>
                                          <td>Due Installment</td>
                                          <td>{18}</td>
                                        </tr>
                                      </tbody>
                                    </table>
                                  </div>
                                </div>", companyLogoUrl,
                                obj?.BusinessUnit?.StrBusinessUnit,
                                obj?.BusinessUnit?.StrAddress,
                                obj?.BusinessUnit?.StrWebsiteUrl,
                                "",//obj?.BusinessUnit?.StrMobile,
                                obj?.employeeName, obj?.employeeCode,
                                obj?.designationName, obj?.employmentTypeName,
                                obj?.departmentName, Convert.ToDateTime(obj?.joiningDate).ToString("dd-MM-yyyy"),
                                obj?.approveLoanAmount, obj?.approveNumberOfInstallmentAmount,
                                obj?.approveNumberOfInstallment,
                                obj?.EmployeeSalaryDefault?.NumGrossSalary, obj?.EmployeeSalaryDefault?.NumBasic,
                                obj?.loanType, obj?.remainingBalance, obj?.dueInstallment);

            sb.Append(@"<div class='body_main_table'>
                                  <div class='table'>
                                    <table>
                                      <thead>
                                        <tr>
                                          <th>SL</th>
                                          <th>Month</th>
                                          <th>Year</th>
                                          <th>No of Installment</th>
                                          <th>Installment Amount</th>
                                          <th>Installment Date</th>
                                        </tr>
                                      </thead>
                                      <tbody>");

            int index = 1;
            foreach (var item in obj.LoanReScheduleList.OrderBy(x => x.IntScheduleId))
            {
                sb.AppendFormat(@"<tr style='page-break-inside: avoid;'>
                                <td>{5}</td>
                                <td>{0}</td>
                                <td>{1}</td>
                                <td>{2}</td>
                                <td>{3}</td>
                                <td>{4}</td>
                            </tr>", item.IntMonth == "1" ? "Jan" : item.IntMonth == "2" ? "Feb" : item.IntMonth == "3" ? "March"
                                    : item.IntMonth == "4" ? "April" : item.IntMonth == "5" ? "May" : item.IntMonth == "6" ? "June"
                                    : item.IntMonth == "7" ? "July" : item.IntMonth == "8" ? "Aug" : item.IntMonth == "9" ? "Aug"
                                    : item.IntMonth == "10" ? "Oct" : item.IntMonth == "11" ? "Nov" : item.IntMonth == "12" ? "Dec" : "",
                                    item.IntYear, index, item.IntInstallmentAmount, item.DteInstallmentDate, index);
                index++;
            }

            sb.AppendFormat(@"                </tbody>
                                    </table>
                                  </div>
                                </div>
                              </div>
                                    <div class='footer'>
                                            <table class='footer_table'>
                                                <tbody>
                                                <tr>
                                                    <td class='authorName'>Department Head</td>
                                                    <td>Admin</td>
                                                    <td>Accounts</td>
                                                </tr>
                                                </tbody>
                                            </table>
                                            <div class='footerNote'></div>
                                            </div>
                                        </div>
                                    </div>
                        <script src='{0}'></script>
                        </body>

                        </html>", commonJsUrl);

            return sb.ToString();
        }

        public async Task<string> LoanReportAll(List<LoanReportByAdvanceFilterViewModel> obj, MasterBusinessUnit BusinessUnit, string? FromDate, string? ToDate)
        {
            var sb = new StringBuilder();
            // companyLogoUrl = _hostingEnvironment.ContentRootPath + "/wwwroot/" + BusinessUnit?.StrLogoUrlforPdfReport;

            sb.AppendFormat(@"<html>
                                <head>
                                </head>
                                <body>
                                    <div class='print-page-main-body'>
                                    <div class='print-page-body'>
                                        <div>
                                        <div style='position:relative; width:100%'>
                                            <div  style='width: 80px; position: absolute'>
                                            <img  style='width:100%' src='{6}' alt='Pineapple' />
                                            </div>
                                            <div style='width:100%;display:block'>
                                                <p style='width:100%;text-align: center;display:block;font-style: normal;font-weight: bold;font-size: 24px;color: rgba(0, 0, 0, 0.7);'>{0}</p>
                                                <p style='text-align: center;font-style: normal;font-weight: bold;font-size: 24px;color: rgba(0, 0, 0, 0.7);'>{1}</p>
                                            </div>
                                        </div>
                                        <div class='company-social-info'>
                                            <p>{2}</p>
                                            <p>{3}</p>
                                        </div>
                                        </div>

                                        <div class='main-report-body'>

                                        <div class='body_main_table'>
                                            <div class='table_header'>Loan Details From {4} To {5}</div>
                                            <div class='table'>
                                            <table>
                                                <thead>
                                                <tr>
                                                    <th>SL</th>
                                                    <th>Employee</th>
                                                    <th>Loan Type</th>
                                                    <th>Loan Amount</th>
                                                    <th>Installment</th>
                                                    <th>No of Installments</th>
                                                    <th>Application Date</th>
                                                    <th>Approval</th>
                                                    <th>Status</th>
                                                </tr>
                                                </thead>
                                                <tbody>",
                                                BusinessUnit?.StrBusinessUnit,
                                                BusinessUnit?.StrAddress,
                                                BusinessUnit?.StrWebsiteUrl,
                                                "",//BusinessUnit?.StrMobile,
                                                !string.IsNullOrEmpty(FromDate) ? Convert.ToDateTime(FromDate).ToString("dd-MMM-yyyy") : "",
                                                !string.IsNullOrEmpty(ToDate) ? Convert.ToDateTime(ToDate).ToString("dd-MMM-yyyy") : "",
                                                companyLogoUrl);
            long count = 1;
            foreach (var item in obj)
            {
                sb.AppendFormat(@"<tr style='page-break-inside: avoid;'>
                                <td>{0}</td>
                                <td>{1} [{2}] <br /><span class='line-gap'> {3}, {4}</span></td>
                                <td>{5}</td>
                                <td>BDT {6}<br /></td>
                                <td>BDT {7}<br /></td>
                                <td>{8}</td>
                                <td>{9}</td>
                                <td>{10}</td>
                                <td>{11}</td>
                            </tr>", count, item?.StrEmployeeName, item?.StrEmployeeCode, item?.StrDesignation,
                            item?.StrDepartment, item?.LoanType,
                            (item?.ApproveLoanAmount == null || item?.ApproveLoanAmount <= 0) ? item?.LoanAmount : item?.ApproveLoanAmount,
                            item?.NumberOfInstallmentAmount,
                            (item?.ApproveNumberOfInstallment == null || item?.ApproveNumberOfInstallment <= 0) ? item?.NumberOfInstallment : item?.ApproveNumberOfInstallment,
                            item?.ApplicationDate.Value.ToString("dd-MM-yyyy"),
                            item?.ApplicationStatus, item?.InstallmentStatus);
                count++;
            }

            sb.AppendFormat(@"
                                                </tbody>
                                            </table>
                                            </div>
                                        </div>
                                        </div>
                                        <div class='footer'>
                                                <table class='footer_table'>
                                                    <tbody>
                                                    <tr>
                                                        <td class='authorName'>Department Head</td>
                                                        <td>Admin</td>
                                                        <td>Accounts</td>
                                                    </tr>
                                                    </tbody>
                                                </table>
                                                <div class='footerNote'></div>
                                                </div>
                                            </div>
                                        </div>
                                    <script src='{0}'></script>
                                </body>
                                </html>", commonJsUrl);

            return sb.ToString();
        }

        public async Task<string> LeaveReportByEmployee(LeaveHistoryViewModel obj)
        {
            var sb = new StringBuilder();
            //companyLogoUrl = _hostingEnvironment.ContentRootPath + "/wwwroot/" + obj?.BusinessUnit?.StrLogoUrlforPdfReport;

            sb.Append(@"<html>
                             <head>
                             </head>
                                 <body>
                                  <div class='print-page-main-body'>
                                     <div class='print-page-body'>
                                       <div class='clearfix'>
                                         <div style='position:relative'>
                                           <div style='position:absolute;width:100px'>");

            sb.AppendFormat(@"<img  src='{0}' alt='company-logo' style='width:100%' />", companyLogoUrl);

            sb.AppendFormat(@"
                                 </div>
                                     <p style='font-size:30px;display:block;text-align:center;font-weight:bold'>{0}</p>
                                     <div style='display:block; text-align:center'> {1} </div>
                             </div>
                             <div class='company-social-info'>
                                 <p> {2} </p>
                                 <p>{3}</p>
                             </div>
                             </div>

                             <div class='main-report-body'>
                             <div class='body-header'>
                                 <div class='employee_personal_info'>
                                 <table>
                                     <tbody>
                                     <tr>
                                         <td>Employee Name</td>
                                         <td>{4} [<span>{5}</span>]</td>
                                     </tr>
                                     <tr>
                                         <td>Designation</td>
                                         <td>{6}, {7}</td>
                                     </tr>
                                     <tr>
                                         <td>Department</td>
                                         <td>{8}</td>
                                     </tr>
                                     </tbody>
                                 </table>
                                 </div>
                                 <div class='emp_other_info'>
                                 <table>
                                     <tbody>
                                     <tr>
                                         <td>Workplace Group</td>
                                         <td>{9}</td>
                                     </tr>
                                     <tr>
                                         <td>Business Unit</td>
                                         <td>{10}</td>
                                     </tr>
                                     <tr>
                                         <td>Supervisor</td>
                                         <td>{11}</td>
                                     </tr>
                                     </tbody>
                                 </table>
                                 </div>
                             </div>",
                            obj?.BusinessUnit?.StrBusinessUnit,
                            obj?.BusinessUnit?.StrAddress,
                            obj?.BusinessUnit?.StrWebsiteUrl,
                            "",//obj?.BusinessUnit?.StrMobile,
                            obj?.EmployeeDetailsViewModel?.EmployeeInfoObj?.StrEmployeeName,
                            obj?.EmployeeDetailsViewModel?.EmployeeInfoObj?.StrEmployeeCode,
                            obj?.EmployeeDetailsViewModel?.DepartmentObj?.StrDepartment,
                            obj?.EmployeeDetailsViewModel?.EmployeeTypeObj?.StrEmploymentType,
                            obj?.EmployeeDetailsViewModel?.DesignationObj?.StrDesignation,
                            obj?.EmployeeDetailsViewModel?.WorkplaceGroupObj?.StrWorkplaceGroup,
                            obj?.BusinessUnit?.StrBusinessUnit,
                            obj?.EmployeeDetailsViewModel.SupervisorObj?.StrEmployeeName);

            sb.AppendFormat(@"<div class='body_main_table'>
                              <div class='table_header'>Leave Details From {0} To {1}</div>
                              <div class='table'>
                                <table>
                                  <thead>
                                    <tr>
                                      <th width='5%'>SL</th>
                                      <th width='17%'>Leave Type</th>
                                      <th width='17%'>Date Range</th>
                                      <th width='17%' class='text_center'>Application Date</th>
                                      <th width='17%' class='text_center'>Duration</th>
                                      <th width='27%'>Reason</th>
                                    </tr>
                                  </thead>
                                  <tbody>", obj?.FromDate.Value.ToString("dd-MMM-yyyy"), obj?.ToDate.Value.ToString("dd-MMM-yyyy"));

            int count = 1;
            foreach (var item in obj.LeaveApplicationList)
            {
                LocalDate start = new LocalDate(item.LeaveApplication.DteFromDate.Year, item.LeaveApplication.DteFromDate.Month, item.LeaveApplication.DteFromDate.Day);
                LocalDate end = new LocalDate(item.LeaveApplication.DteToDate.Year, item.LeaveApplication.DteToDate.Month, item.LeaveApplication.DteToDate.Day);
                Period period = Period.Between(start, end.PlusDays(1));

                string leaveDuration = (period.Years > 0 && period.Months > 0 && period.Days > 0) ? $"{period.Years} years, {period.Months} months, {period.Days} days"
                    : (period.Months > 0 && period.Days > 0) ? $"{period.Months} months, {period.Days} days"
                    : $"{period.Days} days";

                sb.AppendFormat(@"<tr style='page-break-inside: avoid;'>
                                  <td>{0}</td>
                                  <td>{1}</td>
                                  <td>{2} <span>-</span> {3}</td>
                                  <td class='text_center'>{4}</td>
                                  <td class='text_center'>{5}</td>
                                  <td>{6}</td>
                                </tr>", count, item?.LeaveType?.StrLeaveType,
                                item?.LeaveApplication?.DteFromDate.ToString("dd-MM-yyyy"),
                                item?.LeaveApplication?.DteToDate.ToString("dd-MM-yyyy"),
                                item?.LeaveApplication?.DteApplicationDate.ToString("dd-MM-yyyy"),
                                leaveDuration,
                                item?.LeaveApplication?.StrReason);
                count++;
            }

            sb.AppendFormat(@"
                                              </tbody>
                                            </table>
                                          </div>
                                        </div>
                                      </div>
                                        <div class='footer'>
                                                <table class='footer_table'>
                                                    <tbody>
                                                    <tr>
                                                        <td class='authorName'>Department Head</td>
                                                        <td>Admin</td>
                                                        <td>Accounts</td>
                                                    </tr>
                                                    </tbody>
                                                </table>
                                                <div class='footerNote'></div>
                                                </div>
                                            </div>
                                        </div>
                                    <script src='{0}'></script>
                                </body>
                                </html>", commonJsUrl);

            return sb.ToString();
        }

        public async Task<string> MonthlySalaryReport(MonthlySalaryReportViewModel obj)
        {
            var sb = new StringBuilder();
            //companyLogoUrl = _hostingEnvironment.ContentRootPath + "/wwwroot/" + obj?.BusinessUnit?.StrLogoUrlforPdfReport;

            sb.AppendFormat(@"<html>
                                <head>
                                </head>
                                <body>
                                  <div class='print-page-main-body'>
                                    <div class='print-page-body'>
                                      <div class='clearfix'>
                                        <div class='company-info'>
                                          <div class='companyName'>
                                            <img class='companyImg' src='{0}' alt='Pineapple' width='22' height='17' />
                                            <p class='companyTitle'>{1}</p>
                                          </div>
                                          <div class='companyAddress'>{2}</div>
                                        </div>
                                        <div class='company-social-info'>
                                          <p>{3}</p>
                                          <p>{4}</p>
                                        </div>
                                      </div>
                                      <div class='main-report-body'>
                                        <div class='body-header'>
                                        </div>
                                        <div class='body_main_table'>
                                          <div class='table_header'>Salary Report of {5}</div>
                                          <table>
                                            <tr class='numbering_tr'>
                                              <td rowspan='3' class='main_header'>SL</td>
                                              <td colspan='2' class='header main_header'>Personal</td>
                                              <td colspan='2' class='header main_header'>Salary</td>
                                              <td colspan='6' class='header main_header'>Leave</td>
                                              <td colspan='5' class='header main_header'>Attendance</td>
                                              <td colspan='3' class='header main_header'>Overtime</td>
                                              <td rowspan='2' class='rotate header main_header'>Att. Bouns</td>
                                              <td colspan='5' class='header main_header'>Allowance</td>
                                              <td rowspan='2' class='rotate header main_header'>Deduction</td>
                                              <td rowspan='2' class='rotate header main_header'>Loan</td>
                                              <td rowspan='2' class='rotate header main_header'>CBA</td>
                                              <td rowspan='2' class='rotate header main_header'><b>Net Payable</b></td>
                                              <td rowspan='2' class='header signture_td main_header'>Signature</td>
                                            </tr>
                                            <tr>
                                              <td class='header'>Employee</td>
                                              <td class='header main_header'><b>Code No</b></td>
                                              <td class='header'>Gross</td>
                                              <td class='header main_header'>Basic</td>
                                              <td class='header'>C/L</td>
                                              <td class='header'>S/L</td>
                                              <td class='header'>E/L</td>
                                              <td class='header'>SUS</td>
                                              <td class='header'>M/L</td>
                                              <td class='header main_header'>LWP</td>
                                              <td class='rotate header'>Working</br> Day</td>
                                              <td class='rotate header'>Present</td>
                                              <td class='rotate header'>Absent</td>
                                              <td class='rotate header'>Offday</td>
                                              <td class='rotate header main_header'>Holiday</td>
                                              <td class='rotate header'>OT Rate</td>
                                              <td class='rotate header'>OT Amt.</br> (tk)</td>
                                              <td class='rotate header main_header'>OT Hr.</td>
                                              <td class='rotate header'>Arrear</td>
                                              <td class='rotate header'>N/A</td>
                                              <td class='rotate header'>N/A amt.</td>
                                              <td class='rotate header'>E/S</td>
                                              <td class='rotate header'>E/S amt.</td>
                                            </tr>
                                            <tr class='numbering_tr'>

                                              <td>1</td>
                                              <td class='main_header'><b>2</b></td>
                                              <td>3</td>
                                              <td class='main_header'>4</td>
                                              <td>5</td>
                                              <td>6</td>
                                              <td>7</td>
                                              <td>8</td>
                                              <td>9</td>
                                              <td class='main_header'>10</td>
                                              <td>11</td>
                                              <td>12</td>
                                              <td>13</td>
                                              <td>14</td>
                                              <td class='main_header'>15</td>
                                              <td>16</td>
                                              <td>17</td>
                                              <td class='main_header'>18</td>
                                              <td class='main_header'>19</td>
                                              <td>20</td>
                                              <td>21</td>
                                              <td>22</td>
                                              <td>23</td>
                                              <td class='main_header'>24</td>
                                              <td class='main_header'>25</td>
                                              <td class='main_header'>26</td>
                                              <td class='main_header'>27</td>
                                              <td class='main_header'><b>28</b></td>
                                              <td class='main_header'>29</td>
                                            </tr>", companyLogoUrl,
                                        obj?.BusinessUnit?.StrBusinessUnit,
                                        obj?.BusinessUnit?.StrAddress,
                                        obj?.BusinessUnit?.StrWebsiteUrl,
                                        "",//obj?.BusinessUnit?.StrMobile,
                                        obj?.MonthName);

            long count = 1;
            foreach (var item in obj.SalaryGenerateHeaderList)
            {
                sb.AppendFormat(@"<tr style='page-break-inside: avoid;'>
                                      <td class='main_header'>{0}</td>
                                      <td>{1} <br />{2}, {3}</td>
                                      <td class='main_header'><b>{2}</b></td>
                                      <td>{6}</td>
                                      <td class='main_header'>{7}</td>
                                      <td>{8}</td>
                                      <td>{9}</td>
                                      <td>{10}</td>
                                      <td>{11}</td>
                                      <td>{12}</td>
                                      <td class='main_header'>{13}</td>
                                      <td>{14}</td>
                                      <td>{15}</td>
                                      <td>{16}</td>
                                      <td>{17}</td>
                                      <td class='main_header'>{18}</td>
                                      <td>{19}</td>
                                      <td>{20}</td>
                                      <td class='main_header'>{21}</td>
                                      <td class='main_header'>{22}</td>
                                      <td>{23}</td>
                                      <td>{24}</td>
                                      <td>{25}</td>
                                      <td>{26}</td>
                                      <td class='main_header'>{27}</td>
                                      <td class='main_header'>{28}</td>
                                      <td class='main_header'>{29}</td>
                                      <td class='main_header'>{30}</td>
                                      <td class='main_header'><b>{31}</b></td>
                                      <td class='signature main_header'></td>
                                    </tr>", count,

                                    item?.EmployeeBasicInfoObj?.StrEmployeeName,
                                    item?.EmployeeBasicInfoObj?.StrEmployeeCode,
                                    item?.EmpDesignationObj?.StrDesignation,
                                    item?.EmpDepartmentObj?.StrDepartment,
                                    //item?.PayscaleGradeObj?.StrGrade,
                                    item?.SalaryGenerateHeaderObj?.NumGrossSalary,
                                    //item?.SalaryGenerateHeaderObj?.NumBasicSalary,
                                    //item?.SalaryGenerateHeaderObj?.IntCl,
                                    //item?.SalaryGenerateHeaderObj?.IntSl,
                                    //item?.SalaryGenerateHeaderObj?.IntEl,
                                    //item?.SalaryGenerateHeaderObj?.IntPl,
                                    //item?.SalaryGenerateHeaderObj?.IntMl,
                                    //item?.SalaryGenerateHeaderObj?.IntLwp,
                                    item?.SalaryGenerateHeaderObj?.IntTotalWorkingDays,
                                    //item?.SalaryGenerateHeaderObj?.IntPresent,
                                    //item?.SalaryGenerateHeaderObj?.IntAbsent,
                                    //item?.SalaryGenerateHeaderObj?.IntOffday,
                                    //item?.SalaryGenerateHeaderObj?.IntHolyday,
                                    //item?.SalaryGenerateHeaderObj?.NumOverTimePerHoursRate,
                                    //item?.SalaryGenerateHeaderObj?.NumOverTimeAmount,
                                    //item?.SalaryGenerateHeaderObj?.NumOverTimeTotalHours,
                                    //item?.SalaryGenerateHeaderObj?.NumAttendanceBonusAllowance,
                                    //0,
                                    //item?.SalaryGenerateHeaderObj?.NumNightAllowance,
                                    //0,
                                    //item?.SalaryGenerateHeaderObj?.NumExtraSideDutyAllowance,
                                    //0,
                                    //item?.SalaryGenerateHeaderObj.NumTotalDeductionCal,
                                    //item?.SalaryGenerateHeaderObj.NumLoanAmount,
                                    0);
                //item?.SalaryGenerateHeaderObj.NumNetPayableAmountCal);
                count++;
            }

            sb.AppendFormat(@"      </table>
                                </div>
                              </div>
                              <div class='footer'>
                                <table class='footer_table'>
                                  <tbody>

                                    <tr>
                                      <td class='authorName'>Department Head</td>
                                      <td>Admin</td>
                                      <td>Accounts</td>
                                    </tr>
                                  </tbody>
                                </table>
                              </div>
                            </div>
                          </div>
                        <script src='{0}'></script>
                    </body>
                </html>", commonJsUrl);

            return sb.ToString();
        }

        public async Task<string> PdfSalaryDetailsReport(SalaryMasyterVM obj, MasterBusinessUnit businessUnit,long intWorkplaceGroupId, long IntMonth, long intYear)
        {
            var sb = new StringBuilder();
            //companyLogoUrl = _hostingEnvironment.ContentRootPath + "/wwwroot/" + obj?.BusinessUnit?.StrLogoUrlforPdfReport;

            var pres = obj.salaryDetailsReportVM.Select(x => x.intTotalWorkingDays).FirstOrDefault();
            var countList = obj.salaryDetailsReportVM.Count();

            if (intWorkplaceGroupId == 2)
            {
                sb.AppendFormat(@"<!DOCTYPE html>
                                <html lang=""en"">
                                <head>
                                </head>

                                <body>
                                    <div style=""margin-top: 50px;"">
                                        <div class=""header"">
                                            <h3 style=""text-align: center; font-size: 35px; margin-bottom: 3px"">" + businessUnit?.StrBusinessUnit + @"</h3>
                                            <p style=""text-align: center; font-size: 20px;margin-top:0px"">" + businessUnit?.StrAddress + @"</p>
                                            <div style=""margin: auto; text-align: center;"">
                                                <span style=""border-bottom: 1px solid gray; font-size: 25px; font-weight:700"">Salary Sheet of " + obj.WorkplaceGroupName + @"</span>
                                            </div>
                                        </div>
                                        <div class=""table-container"">
                                            <div class=""table-head"">
                                                <table style=""width: 100%"">
                                                    <tbody>
                                                        <tr>
                                                            <td style=""width: 25%;""></td>
                                                            <td><span>Present Month: <span></span>" + pres + @" Days</span></td>
                                                            <td><span>Month: <span>" + obj.Month + @"/" + intYear + @"</span></span></td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </div>
                                            <div class=""Salary-Sheet-pdf-table"">
                                                <table style=""width: 100%;"">
                                                    <thead>
                                                        <tr>
                                                            <th rowspan=""2"">SN</th>
                                                            <th rowspan=""2"">Employee's ID</th>
                                                            <th rowspan=""2"">Employee's Name</th>
                                                            <th rowspan=""2"" style='min-width:100px !important'>Designation</th>
                                                            <th rowspan=""2"">PIN</th>
                                                            <th rowspan=""2"">Joining Date</th>
                                                            <th rowspan=""2"">Present</th>
                                                            <th rowspan=""2"">Leave</th>
                                                            <th rowspan=""2"">Absent</th>
                                                            <th colspan=""5"">Earnings</th> <!-- <<---  -->

                                                            <th rowspan=""2"">Gross Salary</th>
                                                            <th rowspan=""2"">Arrear Salary</th>
                                                            <th rowspan=""2"">Extra House Rent</th>
                                                            <th rowspan=""2"">City Allow.</th>
                                                            <th rowspan=""2"">Total Salary</th>
                                                            <th colspan=""5"">Deductions</th> <!-- <<---  -->

                                                            <th rowspan=""2"">Net Salary</th>
                                                            <th rowspan=""2"">PF (Com.)</th>
                                                            <th rowspan=""2"">Gratuity</th>
                                                            <th rowspan=""2"">Total PF & Gratuity</th>
                                                            <th rowspan=""2"">Total Cost at Salary</th>
                                                            <th rowspan=""2"">Remarks</th>
                                                        </tr>
                                                        <tr>
                                                            <th>Basic Salary</th>
                                                            <th>House Rent</th>
                                                            <th>Medical</th>
                                                            <th>Conv.</th>
                                                            <th>Special Allow.</th>

                                                            <th>PF (Own)</th>
                                                            <th>Absent</th>
                                                            <th>Tax</th>
                                                            <th>Adj.</th>
                                                            <th>Fine</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                        <tr style='text-align: center'>
                                                            <td>1</td>
                                                            <td>2</td>
                                                            <td>3</td>
                                                            <td>4</td>
                                                            <td>5</td>
                                                            <td>6</td>
                                                            <td>7</td>
                                                            <td>8</td>
                                                            <td>9</td>
                                                            <td>10</td>
                                                            <td>11</td>
                                                            <td>12</td>
                                                            <td>13</td>
                                                            <td>14</td>
                                                            <td>15</td>
                                                            <td>16</td>
                                                            <td>17</td>
                                                            <td>18</td>
                                                            <td>19</td>
                                                            <td>20</td>
                                                            <td>21</td>
                                                            <td>22</td>
                                                            <td>23</td>
                                                            <td>24</td>
                                                            <td>25</td>
                                                            <td>26</td>
                                                            <td>27</td>
                                                            <td>28</td>
                                                            <td>29</td>
                                                            <td>30</td>
                                                        </tr>");

                int count = 1;
                //long deptId = 0;
                //long lastDeptId = (long)obj.salaryDetailsReportVM.OrderByDescending(x => x.intDepartmentId).FirstOrDefault().intDepartmentId;
                //long lastSL = obj.salaryDetailsReportVM.OrderByDescending(x => x.SL).FirstOrDefault().SL;

                foreach (var dept in obj.DepartmentVM)
                {
                    sb.AppendFormat(@"<tr>
                                        <td colspan='9' style='font-weight: 700;padding:10px; font-size:25px; background-color: rgb(239, 236, 236);'>Department: " + dept?.deptName + @"</td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                    </tr>");

                    //var deptWiseDetails = obj.salaryDetailsReportVM.Where(x => x.intDepartmentId == dept.deptId).OrderBy(a => a.strEmployeeName).ToList();
                    
                    var deptWiseDetails = (from sal in obj.salaryDetailsReportVM
                                           where sal.intDepartmentId == dept.deptId
                                           orderby sal.depRankId ascending
                                           select sal).ToList();

                    foreach (var item in deptWiseDetails)
                    {
                        if (!string.IsNullOrEmpty(item.strDepartment))
                        {
                            var joinDate = Convert.ToDateTime(item?.dteJoiningDate).ToString("dd MMM, yyyy");
                            var totalPF = item?.numPFAmount + item?.numPfCompany + item?.numGratuity;
                            var tax = Convert.ToDecimal(item?.numTaxAmount).ToString("0");
                            var leaveSum = item?.intAnnualLeave + item?.intCasualLeave + item?.intSickLeave + item?.intMaternityLeave + item?.intPrivilegeLeave;
                            sb.AppendFormat(@"<tr style='text-align:right;'>
                                            <td style='text-align:center; padding:6px; font-size:20px;'>" + count++ + @"</td>
                                            <td style='text-align:left; padding:6px; font-size:20px;'>" + item?.strEmployeeCode + @"</td>
                                            <td style='min-width:200px !important; text-align:left; padding:6px; font-size:20px;'>" + item?.strEmployeeName + @"</td>
                                            <td style='text-align:left; padding:6px; font-size:20px;'>" + item?.strDesignation + @"</td>
                                            <td style='text-align:center; padding:6px; font-size:20px;'>" + item?.intPIN + @"</td>
                                            <td style='text-align:center; min-width:120px !important; padding:6px; font-size:20px;'>" + joinDate + @"</td>
                                            <td style='text-align:center; padding:6px; font-size:20px;'>" + (item?.intPresent) + @"</td>
                                            <td style='text-align:center; padding:6px; font-size:20px;'>" + leaveSum + @"</td>
                                            <td style='text-align:center; padding:6px; font-size:20px;'>" + item?.intAbsent + @"</td>
                                            <td style='padding:6px; font-size:20px;'>" + Convert.ToDecimal(item?.numBasic).ToString("0") + @"</td>
                                            <td style='padding:6px; font-size:20px;'>" + Convert.ToDecimal(item?.numHouseRent).ToString("0") + @"</td>
                                            <td style='padding:6px; font-size:20px;'>" + Convert.ToDecimal(item?.numMedical).ToString("0") + @"</td>
                                            <td style='padding:6px; font-size:20px;'>" + Convert.ToDecimal(item?.numConvence).ToString("0") + @"</td>
                                            <td style='padding:6px; font-size:20px;'>" + Convert.ToDecimal(item?.numSpecialAllowence).ToString("0") + @"</td>
                                            <td style='padding:6px; font-size:20px;'>" + Convert.ToDecimal(item?.numGrossSalary).ToString("0") + @"</td>
                                            <td style='padding:6px; font-size:20px;'>" + Convert.ToDecimal(item?.numArearSalary).ToString("0") + @"</td>
                                            <td style='padding:6px; font-size:20px;'>" + Convert.ToDecimal(item?.numExtraRouseRent).ToString("0") + @"</td>
                                            <td style='padding:6px; font-size:20px;'>" + Convert.ToDecimal(item?.numCityAllowence).ToString("0") + @"</td>
                                            <td style='padding:6px; font-size:20px;'>" + Convert.ToDecimal(item?.numTotalAllowance + item?.numArearSalary + item?.numGrossSalary).ToString("0") + @"</td>
                                            <td style='padding:6px; font-size:20px;'>" + Convert.ToDecimal(item?.numPFAmount).ToString("0") + @"</td>
                                            <td style='padding:6px; font-size:20px;'>" + Convert.ToDecimal(item?.AbsentAmount).ToString("0") + @"</td>
                                            <td style='padding:6px; font-size:20px;'>" + tax + @"</td>
                                            <td style='padding:6px; font-size:20px;'>" + Convert.ToDecimal(item?.numAdjustment).ToString("0") + @"</td>
                                            <td style='padding:6px; font-size:20px;'>" + Convert.ToDecimal(item?.numFine).ToString("0") + @"</td>
                                            <td style='padding:6px; font-size:20px;'>" + Convert.ToDecimal(item?.numNetPayableSalary).ToString("0") + @"</td>
                                            <td style='padding:6px; font-size:20px;'>" + Convert.ToDecimal(item?.numPfCompany).ToString("0") + @"</td>
                                            <td style='padding:6px; font-size:20px;'>" + Convert.ToDecimal(item?.numGratuity).ToString("0") + @"</td>
                                            <td style='padding:6px; font-size:20px;'>" + Convert.ToDecimal(item?.TotalPfNGratuity).ToString("0") + @"</td>
                                            <td style='padding:6px; font-size:20px;'>" + Convert.ToDecimal(item?.numNetPayableSalary + totalPF).ToString("0") + @"</td>
                                            <td style='padding:6px; font-size:20px;'></td>
                                        </tr>");


                        }


                    }
                    if (dept.deptId > 0)
                    {
                        //count = 1;
                        sb.AppendFormat(@"<tr style='text-align:right;'>
                                        <td colspan='9' style='font-weight: 700;text-align:right; background-color: rgb(239, 236, 236);padding:6px;font-size:20px;'>Sub Total:</td>
                                        <td style='padding:6px;font-weight: 700; font-size:20px;'>{1}</td>
                                        <td style='padding:6px;font-weight: 700; font-size:20px;'>{2}</td>
                                        <td style='padding:6px;font-weight: 700; font-size:20px;'>{3}</td>
                                        <td style='padding:6px;font-weight: 700; font-size:20px;'>{4}</td>
                                        <td style='padding:6px;font-weight: 700; font-size:20px;'>{5}</td>
                                        <td style='padding:6px;font-weight: 700; font-size:20px;'>{6}</td>
                                        <td style='padding:6px;font-weight: 700; font-size:20px;'>{7}</td>
                                        <td style='padding:6px;font-weight: 700; font-size:20px;'>{8}</td>
                                        <td style='padding:6px;font-weight: 700; font-size:20px;'>{9}</td>
                                        <td style='padding:6px;font-weight: 700; font-size:20px;'>{10}</td>
                                        <td style='padding:6px;font-weight: 700; font-size:20px;'>{11}</td>
                                        <td style='padding:6px;font-weight: 700; font-size:20px;'>{12}</td>
                                        <td style='padding:6px;font-weight: 700; font-size:20px;'>{13}</td>
                                        <td style='padding:6px;font-weight: 700; font-size:20px;'>{14}</td>
                                        <td style='padding:6px;font-weight: 700; font-size:20px;'>{15}</td>
                                        <td style='padding:6px;font-weight: 700; font-size:20px;'>{16}</td>
                                        <td style='padding:6px;font-weight: 700; font-size:20px;'>{17}</td>
                                        <td style='padding:6px;font-weight: 700; font-size:20px;'>{18}</td>
                                        <td style='padding:6px;font-weight: 700; font-size:20px;'>{19}</td>
                                        <td style='padding:6px;font-weight: 700; font-size:20px;'>{20}</td>
                                        <td style='padding:6px;font-weight: 700; font-size:20px;'>{21}</td>
                                    </tr>", dept?.deptName,
                                    String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intDepartmentId == dept.deptId && x.intEmployeeId > 0).Sum(x => x?.numBasic)),
                                    String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intDepartmentId == dept.deptId && x.intEmployeeId > 0).Sum(x => x?.numHouseRent)),
                                    String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intDepartmentId == dept.deptId && x.intEmployeeId > 0).Sum(x => x?.numMedical)),
                                    String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intDepartmentId == dept.deptId && x.intEmployeeId > 0).Sum(x => x?.numConvence)),
                                    String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intDepartmentId == dept.deptId && x.intEmployeeId > 0).Sum(x => x?.numSpecialAllowence)),
                                    String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intDepartmentId == dept.deptId && x.intEmployeeId > 0).Sum(x => x?.numGrossSalary)),
                                    String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intDepartmentId == dept.deptId && x.intEmployeeId > 0).Sum(x => x?.numArearSalary)),
                                    String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intDepartmentId == dept.deptId && x.intEmployeeId > 0).Sum(x => x?.numExtraRouseRent)),
                                    String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intDepartmentId == dept.deptId && x.intEmployeeId > 0).Sum(x => x?.numCityAllowence)),
                                    String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intDepartmentId == dept.deptId && x.intEmployeeId > 0).Sum(x => x.numTotalAllowance + x.numArearSalary + x.numGrossSalary)),
                                    String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intDepartmentId == dept.deptId && x.intEmployeeId > 0).Sum(x => x?.numPFAmount)),
                                    String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intDepartmentId == dept.deptId && x.intEmployeeId > 0).Sum(x => x?.AbsentAmount)),
                                    String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intDepartmentId == dept.deptId && x.intEmployeeId > 0).Sum(x => x?.numTaxAmount)),
                                    String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intDepartmentId == dept.deptId && x.intEmployeeId > 0).Sum(x => x?.numAdjustment)),
                                    String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intDepartmentId == dept.deptId && x.intEmployeeId > 0).Sum(x => x?.numFine)),
                                    String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intDepartmentId == dept.deptId && x.intEmployeeId > 0).Sum(x => x?.numNetPayableSalary)),
                                    String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intDepartmentId == dept.deptId && x.intEmployeeId > 0).Sum(x => x?.numPfCompany)),
                                    String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intDepartmentId == dept.deptId && x.intEmployeeId > 0).Sum(x => x?.numGratuity)),
                                    String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intDepartmentId == dept.deptId && x.intEmployeeId > 0).Sum(x => x?.TotalPfNGratuity)),
                                    String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intDepartmentId == dept.deptId && x.intEmployeeId > 0).Sum(x => x?.numNetPayableSalary + x?.TotalPfNGratuity)),
                                    ""
                                    );



                    }

                    if (count == countList + 1)
                    {
                        //count = 1;
                        sb.AppendFormat(@"<tr style='text-align:right;'>
                                        <td colspan='9' style='font-weight: 700;text-align:right;padding:6px;font-size:20px; background-color: rgb(239, 236, 236);'>Grand Total:</td>
                                        <td style=' padding:6px; font-size:20px;font-weight: 700;'>"+ String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numBasic)) + @"</td>
                                        <td style=' padding:6px; font-size:20px;font-weight: 700;'>"+String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numHouseRent))+@"</td>
                                        <td style=' padding:6px; font-size:20px;font-weight: 700;'>"+String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numMedical))+@"</td>
                                        <td style=' padding:6px; font-size:20px;font-weight: 700;'>"+String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numConvence))+@"</td>
                                        <td style=' padding:6px; font-size:20px;font-weight: 700;'>"+String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numSpecialAllowence))+@"</td>
                                        <td style=' padding:6px; font-size:20px;font-weight: 700;'>"+String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numGrossSalary))+@"</td>
                                        <td style=' padding:6px; font-size:20px;font-weight: 700;'>"+String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numArearSalary))+@"</td>
                                        <td style=' padding:6px; font-size:20px;font-weight: 700;'>"+String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numExtraRouseRent))+@"</td>
                                        <td style=' padding:6px; font-size:20px;font-weight: 700;'>"+ String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numCityAllowence)) + @"</td>
                                        <td style=' padding:6px; font-size:20px;font-weight: 700;'>"+String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x.numTotalAllowance + x.numArearSalary + x.numGrossSalary))+@"</td>
                                        <td style=' padding:6px; font-size:20px;font-weight: 700;'>"+String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numPFAmount))+@"</td>
                                        <td style=' padding:6px; font-size:20px;font-weight: 700;'>"+String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.AbsentAmount))+@"</td>
                                        <td style=' padding:6px; font-size:20px;font-weight: 700;'>"+String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numTaxAmount))+@"</td>
                                        <td style=' padding:6px; font-size:20px;font-weight: 700;'>"+String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numAdjustment))+@"</td>
                                        <td style=' padding:6px; font-size:20px;font-weight: 700;'>"+String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numFine))+@"</td>
                                        <td style=' padding:6px; font-size:20px;font-weight: 700;'>"+String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numNetPayableSalary))+@"</td>
                                        <td style=' padding:6px; font-size:20px;font-weight: 700;'>"+String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numPfCompany))+@"</td>
                                        <td style=' padding:6px; font-size:20px;font-weight: 700;'>"+String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numGratuity))+@"</td>
                                        <td style=' padding:6px; font-size:20px;font-weight: 700;'>"+String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.TotalPfNGratuity))+ @"</td>
                                        <td style=' padding:6px; font-size:20px;font-weight: 700;'>" + String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numNetPayableSalary + x?.TotalPfNGratuity))+@"</td>
                                        <td style=' padding:6px; font-size:20px;'></td>
                                    </tr>"
                                    );
                    }
                }

                string? inWord = await AmountInWords.ConvertToWords(obj.salaryDetailsReportVM.Sum(x => x.numNetPayableSalary).ToString()) + " only";


                sb.AppendFormat(@" 
                                                    </tbody>
                                                </table>
                                            </div>
                                            <div>
                                                <h4>In Words (Total Cost at Salary) : " + inWord + @"</h4>
                                            </div>
                                        </div>
                                        <div class=""signature-table"">
                                            <table style=""width: 95%; margin: auto; border-collapse: collapse; margin-top: 130px;"">
                                                <tbody style=""text-align: center;"">
                                                    <tr>
                                                        <td>
                                                            <p>Human Resource & Administrations
                                                                Department</p>
                                                        </td>
                                                        <td>
                                                            <p>Audit Department</p>
                                                        </td>
                                                        <td>
                                                            <p>Deputy Manager-Cash & Banking</p>
                                                        </td>
                                                        <td>
                                                            <p>Senior Manager-Accounts & Finance</p>
                                                        </td>
                                                        <td>
                                                            <p>Director-Finance</p>
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </div>
                                    </div>
                                </body>

                                </html>");
            }
            else if(intWorkplaceGroupId == 3)
            {
                sb.AppendFormat(@"<!DOCTYPE html>
                                <html lang=""en"">
                                <head>
                                </head>

                                <body>
                                    <div style=""margin-top: 50px;"">
                                        <div class=""header"">
                                            <h3 style=""text-align: center; font-size: 35px; margin-bottom: 3px"">" + businessUnit?.StrBusinessUnit + @"</h3>
                                            <p style=""text-align: center; font-size: 20px;margin-top:0px"">" + businessUnit?.StrAddress + @"</p>
                                            <div style=""margin: auto; text-align: center;"">
                                                <span style=""border-bottom: 1px solid gray; font-size: 25px; font-weight:700"">Salary Sheet of " + obj.WorkplaceGroupName + @" (For "+obj.SoleDipoName+@")</span>
                                            </div>
                                        </div>
                                        <div class=""table-container"" style='margin-top:5px;'>
                                            <div class=""table-head"">
                                                <table style=""width: 100%"">
                                                    <tbody>
                                                        <tr>
                                                            <td><span>Sole Depot: <span></span>" + obj.SoleDipoName + @"</span></td>
                                                            <td><span>Wing: <span></span>" + obj.WingName + @"</span></td>
                                                            <td><span>Present Month: <span></span>" + pres + @" Days</span></td>
                                                            <td style='text-align:right'><span>Month: <span>" + obj.Month + @"/" + intYear + @"</span></span></td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </div>
                                            <div class=""Salary-Sheet-pdf-table"">
                                                <table style=""width: 100%;"">
                                                    <thead>
                                                        <tr>
                                                            <th rowspan=""2"">SN</th>
                                                            <th rowspan=""2"">Employee's ID</th>
                                                            <th rowspan=""2"" style='min-width:100px !important'>Employee's Name</th>
                                                            <th rowspan=""2"">Designation</th>
                                                            <th rowspan=""2"">PIN</th>
                                                            <th rowspan=""2"" style='min-width:100px !important'>Joining Date</th>
                                                            <th rowspan=""2""><span style=\""writing-mode: vertical-rl; transform: rotate(180deg);\"">Present</span></th>
                                                            <th rowspan=""2"">Leave</th>
                                                            <th rowspan=""2"">Absent</th>
                                                            <th colspan=""5"">Earnings</th> <!-- <<---  -->

                                                            <th rowspan=""2"">Gross Salary</th>
                                                            <th rowspan=""2"">Arrear Salary</th>
                                                            <th rowspan=""2"">Extra Allow.</th>
                                                            <th rowspan=""2"">Total Salary</th>
                                                            <th colspan=""6"">Deductions</th> <!-- <<---  -->

                                                            <th rowspan=""2"">Net Salary</th>
                                                            <th rowspan=""2"">PF (Com.)</th>
                                                            <th rowspan=""2"">Gratuity</th>
                                                            <th rowspan=""2"">Total PF & Gratuity</th>
                                                            <th rowspan=""2"">Total Cost at Salary</th>
                                                            <th rowspan=""2"">Remarks</th>
                                                        </tr>
                                                        <tr>
                                                            <th>Basic Salary</th>
                                                            <th>House Rent</th>
                                                            <th>Medical</th>
                                                            <th>Conv.</th>
                                                            <th>Special Allow.</th>

                                                            <th>PF (Own)</th>
                                                            <th>Absent</th>
                                                            <th>Tax</th>
                                                            <th>Adj.</th>
                                                            <th>Motor Cycle</th>
                                                            <th>Fine</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                        <tr style='text-align: center'>
                                                            <td>1</td>
                                                            <td>2</td>
                                                            <td>3</td>
                                                            <td>4</td>
                                                            <td>5</td>
                                                            <td>6</td>
                                                            <td>7</td>
                                                            <td>8</td>
                                                            <td>9</td>
                                                            <td>10</td>
                                                            <td>11</td>
                                                            <td>12</td>
                                                            <td>13</td>
                                                            <td>14</td>
                                                            <td>15</td>
                                                            <td>16</td>
                                                            <td>17</td>
                                                            <td>18</td>
                                                            <td>19</td>
                                                            <td>20</td>
                                                            <td>21</td>
                                                            <td>22</td>
                                                            <td>23</td>
                                                            <td>24</td>
                                                            <td>25</td>
                                                            <td>26</td>
                                                            <td>27</td>
                                                            <td>28</td>
                                                            <td>29</td>
                                                            <td>30</td>
                                                        </tr>");


                int count = 1;

                /*FOR SOLEDEPO SALARY*/
                foreach (var area in obj.SoleDepoVM)
                {
                    sb.AppendFormat(@"<tr>
                                        <td colspan='9' style='font-weight: 700; padding:10px; font-size:25px; background-color: rgb(239, 236, 236);'>Sole Depo: " + area?.SoleDepoName + @"</td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                    </tr>");

                    //var areaWiseDetails = obj.salaryDetailsReportVM.Where(x => x.intAreaId == area.AreaId).OrderBy(a => a.intRankingId).ToList();
                    var areaWiseDetails = (from sal in obj.salaryDetailsReportVM
                                           where sal.intSoleDepoId == area.SoleDepoId && sal.intAreaId <= 0
                                           orderby sal.RankId ascending
                                           select sal).ToList();

                    foreach (var item in areaWiseDetails)
                    {
                        if (!string.IsNullOrEmpty(item.SoleDipoName))
                        {
                            var joinDate = Convert.ToDateTime(item?.dteJoiningDate).ToString("dd MMM, yyyy");
                            var totalPF = item?.numPFAmount + item?.numPfCompany + item?.numGratuity + item?.numTaxAmount + item?.numAdjustment + item?.numBikeLoan;
                            var tax = Convert.ToDecimal(item?.numTaxAmount).ToString("0");
                            var leaveSum = item?.intAnnualLeave + item?.intCasualLeave + item?.intSickLeave + item?.intMaternityLeave + item?.intPrivilegeLeave;
                            sb.AppendFormat(@"<tr style='text-align:right;'>
                                                            <td style='text-align:center; padding:6px; font-size:20px;'>" + count++ + @"</td>
                                                            <td style='text-align:left; padding:6px; font-size:20px;'>" + item?.strEmployeeCode + @"</td>
                                                            <td style='min-width:200px !important; text-align:left; padding:6px; font-size:20px;'>" + item?.strEmployeeName + @"</td>
                                                            <td style='text-align:center; font-size:20px;'>" + item?.strDesignation + @"</td>
                                                            <td style='text-align:center; padding:6px; font-size:20px;'>" + item?.intPIN + @"</td>
                                                            <td style='text-align:center; min-width:120px !important; padding:6px; font-size:20px;'>" + joinDate + @"</td>
                                                            <td style='text-align:center; padding:6px; font-size:20px;'>" + (item?.intPresent) + @"</td>
                                                            <td style='text-align:center; padding:6px; font-size:20px;'>" + leaveSum + @"</td>
                                                            <td style='text-align:center; padding:6px; font-size:20px;'>" + item?.intAbsent + @"</td>
                                                            <td style=' padding:6px; font-size:20px;'>" + Convert.ToDecimal(item?.numBasic).ToString("0") + @"</td>
                                                            <td style=' padding:6px; font-size:20px;'>" + Convert.ToDecimal(item?.numHouseRent).ToString("0") + @"</td>
                                                            <td style=' padding:6px; font-size:20px;'>" + Convert.ToDecimal(item?.numMedical).ToString("0") + @"</td>
                                                            <td style=' padding:6px; font-size:20px;'>" + Convert.ToDecimal(item?.numConvence).ToString("0") + @"</td>
                                                            <td style=' padding:6px; font-size:20px;'>" + Convert.ToDecimal(item?.numSpecialAllowence).ToString("0") + @"</td>
                                                            <td style=' padding:6px; font-size:20px;'>" + Convert.ToDecimal(item?.numGrossSalary).ToString("0") + @"</td>
                                                            <td style=' padding:6px; font-size:20px;'>" + Convert.ToDecimal(item?.numArearSalary).ToString("0") + @"</td>
                                                            <td style=' padding:6px; font-size:20px;'>" + Convert.ToDecimal(item?.numExtraAllowance).ToString("0") + @"</td>
                                                            <td style=' padding:6px; font-size:20px;'>" + Convert.ToDecimal(item?.numTotalAllowance + item?.numArearSalary + item?.numGrossSalary).ToString("0") + @"</td>
                                                            <td style=' padding:6px; font-size:20px;'>" + Convert.ToDecimal(item?.numPFAmount).ToString("0") + @"</td>
                                                            <td style=' padding:6px; font-size:20px;'>" + Convert.ToDecimal(item?.AbsentAmount).ToString("0") + @"</td>
                                                            <td style=' padding:6px; font-size:20px;'>" + tax + @"</td>
                                                            <td style=' padding:6px; font-size:20px;'>" + Convert.ToDecimal(item?.numAdjustment).ToString("0") + @"</td>
                                                            <td style=' padding:6px; font-size:20px;'>" + Convert.ToDecimal(item?.numBikeLoan).ToString("0") + @"</td>
                                                            <td style=' padding:6px; font-size:20px;'>" + Convert.ToDecimal(item?.numFine).ToString("0") + @"</td>
                                                            <td style=' padding:6px; font-size:20px;'>" + Convert.ToDecimal(item?.numNetPayableSalary).ToString("0") + @"</td>
                                                            <td style=' padding:6px; font-size:20px;'>" + Convert.ToDecimal(item?.numPfCompany).ToString("0") + @"</td>
                                                            <td style=' padding:6px; font-size:20px;'>" + Convert.ToDecimal(item?.numGratuity).ToString("0") + @"</td>
                                                            <td style=' padding:6px; font-size:20px;'>" + Convert.ToDecimal(item?.TotalPfNGratuity).ToString("0") + @"</td>
                                                            <td style=' padding:6px; font-size:20px;'>" + Convert.ToDecimal(item?.numNetPayableSalary + totalPF).ToString("0") + @"</td>
                                                            <td style=' padding:6px'></td>
                                                        </tr>");


                        }

                    }
                    if (area.SoleDepoId > 0)
                    {
                        //count = 1;
                        sb.AppendFormat(@"<tr style='text-align:right;'>
                                        <td colspan='9' style='font-weight: 700;text-align:right;padding:6px;font-size:20px; background-color: rgb(239, 236, 236);'>Sub Total:</td>
                                        <td style=' padding:6px;font-weight: 700; font-size:20px;'>{1}</td>
                                        <td style=' padding:6px;font-weight: 700; font-size:20px;'>{2}</td>
                                        <td style=' padding:6px;font-weight: 700; font-size:20px;'>{3}</td>
                                        <td style=' padding:6px;font-weight: 700; font-size:20px;'>{4}</td>
                                        <td style=' padding:6px;font-weight: 700; font-size:20px;'>{5}</td>
                                        <td style=' padding:6px;font-weight: 700; font-size:20px;'>{6}</td>
                                        <td style=' padding:6px;font-weight: 700; font-size:20px;'>{7}</td>
                                        <td style=' padding:6px;font-weight: 700; font-size:20px;'>{8}</td>
                                        <td style=' padding:6px;font-weight: 700; font-size:20px;'>{9}</td>
                                        <td style=' padding:6px;font-weight: 700; font-size:20px;'>{10}</td>
                                        <td style=' padding:6px;font-weight: 700; font-size:20px;'>{11}</td>
                                        <td style=' padding:6px;font-weight: 700; font-size:20px;'>{12}</td>
                                        <td style=' padding:6px;font-weight: 700; font-size:20px;'>{13}</td>
                                        <td style=' padding:6px;font-weight: 700; font-size:20px;'>{14}</td>
                                        <td style=' padding:6px;font-weight: 700; font-size:20px;'>{15}</td>
                                        <td style=' padding:6px;font-weight: 700; font-size:20px;'>{16}</td>
                                        <td style=' padding:6px;font-weight: 700; font-size:20px;'>{17}</td>
                                        <td style=' padding:6px;font-weight: 700; font-size:20px;'>{18}</td>
                                        <td style=' padding:6px;font-weight: 700; font-size:20px;'>{19}</td>
                                        <td style=' padding:6px;font-weight: 700; font-size:20px;'>{20}</td>
                                        <td style=' padding:6px;font-weight: 700; font-size:20px;'>{21}</td>
                                    </tr>", area.SoleDepoName,
                                    String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intSoleDepoId == area.SoleDepoId && x.intAreaId == 0 && x.intEmployeeId > 0).Sum(x => x?.numBasic)),
                                    String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intSoleDepoId == area.SoleDepoId && x.intAreaId == 0 && x.intEmployeeId > 0).Sum(x => x?.numHouseRent)),
                                    String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intSoleDepoId == area.SoleDepoId && x.intAreaId == 0 && x.intEmployeeId > 0).Sum(x => x?.numMedical)),
                                    String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intSoleDepoId == area.SoleDepoId && x.intAreaId == 0 && x.intEmployeeId > 0).Sum(x => x?.numConvence)),
                                    String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intSoleDepoId == area.SoleDepoId && x.intAreaId == 0 && x.intEmployeeId > 0).Sum(x => x?.numSpecialAllowence)),
                                    String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intSoleDepoId == area.SoleDepoId && x.intAreaId == 0 && x.intEmployeeId > 0).Sum(x => x?.numGrossSalary)),
                                    String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intSoleDepoId == area.SoleDepoId && x.intAreaId == 0 && x.intEmployeeId > 0).Sum(x => x?.numArearSalary)),
                                    String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intSoleDepoId == area.SoleDepoId && x.intAreaId == 0 && x.intEmployeeId > 0).Sum(x => x?.numExtraAllowance)),
                                    String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intSoleDepoId == area.SoleDepoId && x.intAreaId == 0 && x.intEmployeeId > 0).Sum(x => x.numTotalAllowance + x.numArearSalary + x.numGrossSalary)),
                                    String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intSoleDepoId == area.SoleDepoId && x.intAreaId == 0 && x.intEmployeeId > 0).Sum(x => x?.numPFAmount)),
                                    String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intSoleDepoId == area.SoleDepoId && x.intAreaId == 0 && x.intEmployeeId > 0).Sum(x => x?.AbsentAmount)),
                                    String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intSoleDepoId == area.SoleDepoId && x.intAreaId == 0 && x.intEmployeeId > 0).Sum(x => x?.numTaxAmount)),
                                    String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intSoleDepoId == area.SoleDepoId && x.intAreaId == 0 && x.intEmployeeId > 0).Sum(x => x?.numAdjustment)),
                                    String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intSoleDepoId == area.SoleDepoId && x.intAreaId == 0 && x.intEmployeeId > 0).Sum(x => x?.numBikeLoan)),
                                    String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intSoleDepoId == area.SoleDepoId && x.intAreaId == 0 && x.intEmployeeId > 0).Sum(x => x?.numFine)),
                                    String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intSoleDepoId == area.SoleDepoId && x.intAreaId == 0 && x.intEmployeeId > 0).Sum(x => x?.numNetPayableSalary)),
                                    String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intSoleDepoId == area.SoleDepoId && x.intAreaId == 0 && x.intEmployeeId > 0).Sum(x => x?.numPfCompany)),
                                    String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intSoleDepoId == area.SoleDepoId && x.intAreaId == 0 && x.intEmployeeId > 0).Sum(x => x?.numGratuity)),
                                    String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intSoleDepoId == area.SoleDepoId && x.intAreaId == 0 && x.intEmployeeId > 0).Sum(x => x?.TotalPfNGratuity)),
                                    String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intSoleDepoId == area.SoleDepoId && x.intAreaId == 0 && x.intEmployeeId > 0).Sum(x => x?.numNetPayableSalary + x?.TotalPfNGratuity + x?.numTaxAmount + x?.numAdjustment + x?.numBikeLoan)),
                                    ""
                                    );

                    }
                }

                /*FOR AREAR SALARY*/
                foreach (var area in obj.AreaVM)
                {
                    sb.AppendFormat(@"<tr>
                                        <td colspan='9' style='font-weight: 700; padding:10px; font-size:25px; background-color: rgb(239, 236, 236);'>Area: " + area?.AreaName + @"</td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                    </tr>");

                    //var areaWiseDetails = obj.salaryDetailsReportVM.Where(x => x.intAreaId == area.AreaId).OrderBy(a => a.intRankingId).ToList();
                    var areaWiseDetails = (from sal in obj.salaryDetailsReportVM
                                           where sal.intAreaId == area.AreaId && sal.intAreaId > 0
                                           orderby sal.RankId ascending
                                           select sal).ToList();

                    foreach (var item in areaWiseDetails)
                    {
                        if (!string.IsNullOrEmpty(item.AreaName))
                        {
                            var joinDate = Convert.ToDateTime(item?.dteJoiningDate).ToString("dd MMM, yyyy");
                            var totalPF = item?.numPFAmount + item?.numPfCompany + item?.numGratuity + item?.numTaxAmount + item?.numAdjustment + item?.numBikeLoan;
                            var tax = Convert.ToDecimal(item?.numTaxAmount).ToString("0");
                            var leaveSum = item?.intAnnualLeave + item?.intCasualLeave + item?.intSickLeave + item?.intMaternityLeave + item?.intPrivilegeLeave;
                            sb.AppendFormat(@"<tr style='text-align:right;'>
                                                            <td style='text-align:center; padding:6px; font-size:20px;'>" + count++ + @"</td>
                                                            <td style='text-align:left; padding:6px; font-size:20px;'>" + item?.strEmployeeCode + @"</td>
                                                            <td style='min-width:200px !important; text-align:left; padding:6px; font-size:20px;'>" + item?.strEmployeeName + @"</td>
                                                            <td style='text-align:center; font-size:20px;'>" + item?.strDesignation + @"</td>
                                                            <td style='text-align:center; padding:6px; font-size:20px;'>" + item?.intPIN + @"</td>
                                                            <td style='text-align:center; min-width:120px !important; padding:6px; font-size:20px;'>" + joinDate + @"</td>
                                                            <td style='text-align:center; padding:6px; font-size:20px;'>" + (item?.intPresent) + @"</td>
                                                            <td style='text-align:center; padding:6px; font-size:20px;'>" + leaveSum + @"</td>
                                                            <td style='text-align:center; padding:6px; font-size:20px;'>" + item?.intAbsent + @"</td>
                                                            <td style=' padding:6px; font-size:20px;'>" + Convert.ToDecimal(item?.numBasic).ToString("0") + @"</td>
                                                            <td style=' padding:6px; font-size:20px;'>" + Convert.ToDecimal(item?.numHouseRent).ToString("0") + @"</td>
                                                            <td style=' padding:6px; font-size:20px;'>" + Convert.ToDecimal(item?.numMedical).ToString("0") + @"</td>
                                                            <td style=' padding:6px; font-size:20px;'>" + Convert.ToDecimal(item?.numConvence).ToString("0") + @"</td>
                                                            <td style=' padding:6px; font-size:20px;'>" + Convert.ToDecimal(item?.numSpecialAllowence).ToString("0") + @"</td>
                                                            <td style=' padding:6px; font-size:20px;'>" + Convert.ToDecimal(item?.numGrossSalary).ToString("0") + @"</td>
                                                            <td style=' padding:6px; font-size:20px;'>" + Convert.ToDecimal(item?.numArearSalary).ToString("0") + @"</td>
                                                            <td style=' padding:6px; font-size:20px;'>" + Convert.ToDecimal(item?.numExtraAllowance).ToString("0") + @"</td>
                                                            <td style=' padding:6px; font-size:20px;'>" + Convert.ToDecimal(item?.numTotalAllowance + item?.numArearSalary + item?.numGrossSalary).ToString("0") + @"</td>
                                                            <td style=' padding:6px; font-size:20px;'>" + Convert.ToDecimal(item?.numPFAmount).ToString("0") + @"</td>
                                                            <td style=' padding:6px; font-size:20px;'>" + Convert.ToDecimal(item?.AbsentAmount).ToString("0") + @"</td>
                                                            <td style=' padding:6px; font-size:20px;'>" + tax + @"</td>
                                                            <td style=' padding:6px; font-size:20px;'>" + Convert.ToDecimal(item?.numAdjustment).ToString("0") + @"</td>
                                                            <td style=' padding:6px; font-size:20px;'>" + Convert.ToDecimal(item?.numBikeLoan).ToString("0") + @"</td>
                                                            <td style=' padding:6px; font-size:20px;'>" + Convert.ToDecimal(item?.numFine).ToString("0") + @"</td>
                                                            <td style=' padding:6px; font-size:20px;'>" + Convert.ToDecimal(item?.numNetPayableSalary).ToString("0") + @"</td>
                                                            <td style=' padding:6px; font-size:20px;'>" + Convert.ToDecimal(item?.numPfCompany).ToString("0") + @"</td>
                                                            <td style=' padding:6px; font-size:20px;'>" + Convert.ToDecimal(item?.numGratuity).ToString("0") + @"</td>
                                                            <td style=' padding:6px; font-size:20px;'>" + Convert.ToDecimal(item?.TotalPfNGratuity).ToString("0") + @"</td>
                                                            <td style=' padding:6px; font-size:20px;'>" + Convert.ToDecimal(item?.numNetPayableSalary + totalPF).ToString("0") + @"</td>
                                                            <td style=' padding:6px'></td>
                                                        </tr>");


                        }

                    }
                    if (area.AreaId > 0)
                    {
                        //count = 1;
                        sb.AppendFormat(@"<tr style='text-align:right;'>
                                        <td colspan='9' style='font-weight: 700;text-align:right;padding:6px;font-size:20px; background-color: rgb(239, 236, 236);'>Sub Total:</td>
                                        <td style=' padding:6px;font-weight: 700; font-size:20px;'>{1}</td>
                                        <td style=' padding:6px;font-weight: 700; font-size:20px;'>{2}</td>
                                        <td style=' padding:6px;font-weight: 700; font-size:20px;'>{3}</td>
                                        <td style=' padding:6px;font-weight: 700; font-size:20px;'>{4}</td>
                                        <td style=' padding:6px;font-weight: 700; font-size:20px;'>{5}</td>
                                        <td style=' padding:6px;font-weight: 700; font-size:20px;'>{6}</td>
                                        <td style=' padding:6px;font-weight: 700; font-size:20px;'>{7}</td>
                                        <td style=' padding:6px;font-weight: 700; font-size:20px;'>{8}</td>
                                        <td style=' padding:6px;font-weight: 700; font-size:20px;'>{9}</td>
                                        <td style=' padding:6px;font-weight: 700; font-size:20px;'>{10}</td>
                                        <td style=' padding:6px;font-weight: 700; font-size:20px;'>{11}</td>
                                        <td style=' padding:6px;font-weight: 700; font-size:20px;'>{12}</td>
                                        <td style=' padding:6px;font-weight: 700; font-size:20px;'>{13}</td>
                                        <td style=' padding:6px;font-weight: 700; font-size:20px;'>{14}</td>
                                        <td style=' padding:6px;font-weight: 700; font-size:20px;'>{15}</td>
                                        <td style=' padding:6px;font-weight: 700; font-size:20px;'>{16}</td>
                                        <td style=' padding:6px;font-weight: 700; font-size:20px;'>{17}</td>
                                        <td style=' padding:6px;font-weight: 700; font-size:20px;'>{18}</td>
                                        <td style=' padding:6px;font-weight: 700; font-size:20px;'>{19}</td>
                                        <td style=' padding:6px;font-weight: 700; font-size:20px;'>{20}</td>
                                        <td style=' padding:6px;font-weight: 700; font-size:20px;'>{21}</td>
                                    </tr>", area.AreaName,
                                    String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intAreaId == area.AreaId && x.intEmployeeId > 0).Sum(x => x?.numBasic)),
                                    String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intAreaId == area.AreaId && x.intEmployeeId > 0).Sum(x => x?.numHouseRent)),
                                    String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intAreaId == area.AreaId && x.intEmployeeId > 0).Sum(x => x?.numMedical)),
                                    String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intAreaId == area.AreaId && x.intEmployeeId > 0).Sum(x => x?.numConvence)),
                                    String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intAreaId == area.AreaId && x.intEmployeeId > 0).Sum(x => x?.numSpecialAllowence)),
                                    String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intAreaId == area.AreaId && x.intEmployeeId > 0).Sum(x => x?.numGrossSalary)),
                                    String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intAreaId == area.AreaId && x.intEmployeeId > 0).Sum(x => x?.numArearSalary)),
                                    String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intAreaId == area.AreaId && x.intEmployeeId > 0).Sum(x => x?.numExtraAllowance)),
                                    String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intAreaId == area.AreaId && x.intEmployeeId > 0).Sum(x => x.numTotalAllowance + x.numArearSalary + x.numGrossSalary)),
                                    String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intAreaId == area.AreaId && x.intEmployeeId > 0).Sum(x => x?.numPFAmount)),
                                    String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intAreaId == area.AreaId && x.intEmployeeId > 0).Sum(x => x?.AbsentAmount)),
                                    String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intAreaId == area.AreaId && x.intEmployeeId > 0).Sum(x => x?.numTaxAmount)),
                                    String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intAreaId == area.AreaId && x.intEmployeeId > 0).Sum(x => x?.numAdjustment)),
                                    String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intAreaId == area.AreaId && x.intEmployeeId > 0).Sum(x => x?.numBikeLoan)),
                                    String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intAreaId == area.AreaId && x.intEmployeeId > 0).Sum(x => x?.numFine)),
                                    String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intAreaId == area.AreaId && x.intEmployeeId > 0).Sum(x => x?.numNetPayableSalary)),
                                    String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intAreaId == area.AreaId && x.intEmployeeId > 0).Sum(x => x?.numPfCompany)),
                                    String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intAreaId == area.AreaId && x.intEmployeeId > 0).Sum(x => x?.numGratuity)),
                                    String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intAreaId == area.AreaId && x.intEmployeeId > 0).Sum(x => x?.TotalPfNGratuity)),
                                    String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intAreaId == area.AreaId && x.intEmployeeId > 0).Sum(x => x?.numNetPayableSalary + x?.TotalPfNGratuity + x?.numTaxAmount + x?.numAdjustment + x?.numBikeLoan)),
                                    ""
                                    );

                    }                    
                }

                if (count == countList + 1)
                {
                    //count = 1;
                    sb.AppendFormat(@"<tr style='text-align:right;'>
                                        <td colspan='9' style='font-weight: 700;text-align:right;padding:6px;font-size:20px; background-color: rgb(239, 236, 236);'>Grand Total:</td>
                                        <td style=' padding:6px; font-size:20px;font-weight: 700;'>{1}</td>
                                        <td style=' padding:6px; font-size:20px;font-weight: 700;'>{2}</td>
                                        <td style=' padding:6px; font-size:20px;font-weight: 700;'>{3}</td>
                                        <td style=' padding:6px; font-size:20px;font-weight: 700;'>{4}</td>
                                        <td style=' padding:6px; font-size:20px;font-weight: 700;'>{5}</td>
                                        <td style=' padding:6px; font-size:20px;font-weight: 700;'>{6}</td>
                                        <td style=' padding:6px; font-size:20px;font-weight: 700;'>{7}</td>
                                        <td style=' padding:6px; font-size:20px;font-weight: 700;'>{8}</td>
                                        <td style=' padding:6px; font-size:20px;font-weight: 700;'>{9}</td>
                                        <td style=' padding:6px; font-size:20px;font-weight: 700;'>{10}</td>
                                        <td style=' padding:6px; font-size:20px;font-weight: 700;'>{11}</td>
                                        <td style=' padding:6px; font-size:20px;font-weight: 700;'>{12}</td>
                                        <td style=' padding:6px; font-size:20px;font-weight: 700;'>{13}</td>
                                        <td style=' padding:6px; font-size:20px;font-weight: 700;'>{14}</td>
                                        <td style=' padding:6px; font-size:20px;font-weight: 700;'>{15}</td>
                                        <td style=' padding:6px; font-size:20px;font-weight: 700;'>{16}</td>
                                        <td style=' padding:6px; font-size:20px;font-weight: 700;'>{17}</td>
                                        <td style=' padding:6px; font-size:20px;font-weight: 700;'>{18}</td>
                                        <td style=' padding:6px; font-size:20px;font-weight: 700;'>{19}</td>
                                        <td style=' padding:6px; font-size:20px;font-weight: 700;'>{20}</td>
                                        <td style=' padding:6px; font-size:20px;font-weight: 700;'>{21}</td>
                                    </tr>", "",
                                String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numBasic)),
                                String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numHouseRent)),
                                String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numMedical)),
                                String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numConvence)),
                                String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numSpecialAllowence)),
                                String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numGrossSalary)),
                                String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numArearSalary)),
                                String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numExtraAllowance)),
                                String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x.numTotalAllowance + x.numArearSalary + x.numGrossSalary)),
                                String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numPFAmount)),
                                String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.AbsentAmount)),
                                String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numTaxAmount)),
                                String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numAdjustment)),
                                String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numBikeLoan)),
                                String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numFine)),
                                String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numNetPayableSalary)),
                                String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numPfCompany)),
                                String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numGratuity)),
                                String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.TotalPfNGratuity)),
                                String.Format("{0:N0}", obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numNetPayableSalary + x?.TotalPfNGratuity + x?.numTaxAmount + x?.numAdjustment + x?.numBikeLoan)),
                                ""
                                );
                }

                string? inWord = await AmountInWords.ConvertToWords(obj.salaryDetailsReportVM.Sum(x => x.numNetPayableSalary).ToString()) + " only";


                sb.AppendFormat(@" 
                                                    </tbody>
                                                </table>
                                            </div>
                                            <div>
                                                <h4>In Words (Total Cost at Salary) : " + inWord + @"</h4>
                                            </div>
                                        </div>
                                        <div class=""signature-table"">
                                            <table style=""width: 95%; margin: auto; border-collapse: collapse; margin-top: 130px;"">
                                                <tbody style=""text-align: center;"">
                                                    <tr>
                                                        <td>
                                                            <p>Sole Depo Head</p>
                                                        </td>
                                                        <td>
                                                            <p>Region Head</p>
                                                        </td>
                                                        <td>
                                                            <p>Wing Head</p>
                                                        </td>
                                                        <td>
                                                            <p>Audit Department</p>
                                                        </td>
                                                        <td>
                                                            <p>HR & Admin Department</p>
                                                        </td>
                                                        <td>
                                                            <p>Accounts Department</p>
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </div>
                                    </div>
                                </body>

                                </html>");
            }

            return sb.ToString();
        }

        public async Task<string> EmployeeBasedSalaryReport(EmployeePayslipViewModel obj)
        {
            var sb = new StringBuilder();
            //companyLogoUrl = _hostingEnvironment.ContentRootPath + "/wwwroot/" + obj?.BusinessUnit?.StrLogoUrlforPdfReport;

            sb.AppendFormat(@"<html>
                                    <head>
                                    </head>
                                    <body>
                                      <div class='print-page-main-body'>
                                        <div class='print-page-body'>
                                          <div class='clearfix'>
                                            <div class='company-info'>
                                              <div class='companyName'>
                                                <img class='companyImg' src='{0}' alt='Pineapple' width='22' height='17' />
                                                <p class='companyTitle'>{1}</p>
                                              </div>
                                              <div class='companyAddress'>{2}</div>
                                            </div>
                                            <div class='company-social-info'>
                                              <p>{3}</p>
                                              <p>{4}</p>
                                            </div>
                                          </div>
                                          <div class='main-report-body'>
                                            <div class='body-header'>
                                              <div class='emp_other_info_part_one emp_other_info'>
                                                <table>
                                                  <tbody>
                                                    <tr>
                                                      <td>Employee Name</td>
                                                      <td>{5}, [{6}]</td>
                                                    </tr>
                                                    <tr>
                                                      <td>Designation</td>
                                                      <td>{7}</td>
                                                    </tr>
                                                    <tr>
                                                      <td>Employment Type</td>
                                                      <td>{8}</td>
                                                    </tr>
                                                    <tr>
                                                      <td>Department</td>
                                                      <td>{9}</td>
                                                    </tr>
                                                    <tr>
                                                      <td>Workplace Group</td>
                                                      <td>{10}</td>
                                                    </tr>
                                                    <tr>
                                                      <td>Business Unit</td>
                                                      <td>{11}</td>
                                                    </tr>
                                                  </tbody>
                                                </table>
                                              </div>
                                              <div class='emp_other_info_part_two emp_other_info'>
                                                <table>
                                                  <tbody>
                                                    <tr>
                                                      <td>Payroll Group</td>
                                                      <td>{12}</td>
                                                    </tr>
                                                    <tr>
                                                      <td>Payscale Grade</td>
                                                      <td>{13}</td>
                                                    </tr>
                                                    <tr>
                                                      <td>From Date</td>
                                                      <td>{14}</td>
                                                    </tr>
                                                    <tr>
                                                      <td>To Date</td>
                                                      <td>{15}</td>
                                                    </tr>
                                                  </tbody>
                                                </table>
                                              </div>
                                            </div>
                                            <div class='body_main_table'>
                                              <div class='table_header'>Pay Slip Of {16} &nbsp; &#8226; &nbsp; {17}</div>

                                              <div class='table'>
                                                <table>
                                                  <thead>
                                                    <tr>
                                                      <th>Earnings</th>
                                                      <th>Amount in Taka</th>
                                                    </tr>
                                                  </thead>
                                                  <tbody>", companyLogoUrl,
                                                  obj?.BusinessUnit?.StrBusinessUnit,
                                                  obj?.BusinessUnit?.StrAddress,
                                                  obj?.BusinessUnit?.StrWebsiteUrl,
                                                  "",//obj?.BusinessUnit?.StrMobile,
                                                  obj?.EmployeeDetails?.EmployeeInfoObj?.StrEmployeeName,
                                                  obj?.EmployeeDetails?.EmployeeInfoObj?.StrEmployeeCode,
                                                  obj?.EmployeeDetails?.DesignationObj?.StrDesignation,
                                                  obj?.EmployeeDetails?.EmployeeTypeObj?.StrEmploymentType,
                                                  obj?.EmployeeDetails?.DepartmentObj?.StrDepartment,
                                                  obj?.EmployeeDetails?.WorkplaceGroupObj?.StrWorkplaceGroup,
                                                  obj?.BusinessUnit?.StrBusinessUnit,
                                                  "",//obj?.EmployeeDetails?.EmployeeInfoObj?.StrPayrollGroupName,
                                                     //obj?.EmployeeDetails?.PayscaleGradeObj?.StrGrade,
                                                  Convert.ToDateTime(obj?.SalaryGenerateHeader?.DtePayrollGenerateFrom).ToString("dd-MMM-yyy"),
                                                  Convert.ToDateTime(obj?.SalaryGenerateHeader?.DtePayrollGenerateTo).ToString("dd-MMM-yyy"),
                                                  obj?.EmployeeDetails?.EmployeeInfoObj?.StrEmployeeName,
                                                  obj?.MonthName);

            foreach (var item in obj.AdditionList)
            {
                sb.AppendFormat(@"<tr style='page-break-inside: avoid;'>
                                    <td>{0}</td>
                                    <td>{1}</td>
                                </tr>", item?.SalaryPortionName, item?.PortionAmount);
            }

            sb.AppendFormat(@"<tr>
                                <td>Gross Earnings</td>
                                <td>{0}</td>
                            </tr>
                            </tbody>
                        </table>
                        </div>
                        <div class='table table_deduction'>
                        <table>
                            <thead>
                            <tr>
                                <th>Deduction</th>
                                <th>Amount in Taka</th>
                            </tr>
                            </thead>
                            <tbody>", obj?.GrossEarnings);

            foreach (var item in obj.DeductionList)
            {
                sb.AppendFormat(@"<tr style='page-break-inside: avoid;'>
                                    <td>{0}</td>
                                    <td>{1}</td>
                                </tr>", item?.SalaryPortionName, item?.PortionAmount);
            }

            string amountInWords = await AmountInWords.ConvertToWords(obj?.NetPayable.ToString());

            sb.AppendFormat(@"<tr>
                                <td>Gross Deduction</td>
                                <td>{0}</td>
                            </tr>
                            </tbody>
                        </table>
                        </div>
                    </div>
                    <div class=' body_main_table footer-table'>
                        <div class='table'>
                        <table>
                            <tbody>
                            <tr>
                                <td>Net Pay</td>
                                <td>{1} <br />({2})</td>
                            </tr>
                            <tr>
                                <td>Provident Fund Balance</td>
                                <td>{3}</td>
                            </tr>
                            <tr>
                                <td>Gratuity Balance</td>
                                <td>{4}</td>
                            </tr>
                            </tbody>
                        </table>
                        </div>
                    </div>
                    </div>

                    <div class='footer'>
                    <table class='footer_table'>
                        <tbody>
                        <tr>
                            <td class='authorName'>Department Head</td>
                            <td>Admin</td>
                            <td>Accounts</td>
                        </tr>
                        </tbody>
                    </table>
                    </div>
                </div>
                </div>
            <script src='{5}'></script>
            </body>
            </html>", obj?.GrossEarnings,
             obj?.NetPayable, amountInWords,
             obj?.ProvidentFund,
             obj?.Gratuity,
             commonJsUrl);

            return sb.ToString();
        }

        public async Task<string> MonthlySalaryDepartmentWiseReport(List<MonthlySalaryDepartmentWiseReportViewModel> obj, MasterBusinessUnit BusinessUnit, string monthName)
        {
            var sb = new StringBuilder();
            //companyLogoUrl = _hostingEnvironment.ContentRootPath + "/wwwroot/" + BusinessUnit?.StrLogoUrlforPdfReport;

            sb.AppendFormat(@"<html>
                                    <head>
                                    </head>
                                    <body>
                                      <div class='print-page-main-body'>
                                        <div class='print-page-body'>
                                          <div class='clearfix'>
                                            <div class='company-info'>
                                              <div class='companyName'>
                                                <img class='companyImg' src='{0}' alt='Pineapple' width='22' height='17' />
                                                <p class='companyTitle'>{1}</p>
                                              </div>
                                              <div class='companyAddress'>{2}</div>
                                            </div>
                                            <div class='company-social-info'>
                                              <p>{3}</p>
                                              <p>{4}</p>
                                            </div>
                                          </div>

                                          <div class='main-report-body'>

                                            <div class='body_main_table'>
                                              <div class='table_header'>Summary Salary & Wages for the month of {5}</div>

                                              <div class='table'>
                                                <table>
                                                  <thead>
                                                    <tr>
                                                      <th class='text-left'>Department</th>
                                                      <th>Man Power</th>
                                                      <th>O.T AMT</th>
                                                      <th>E.SIDE</th>
                                                      <th>N/A AMT</th>
                                                      <th>ATT. Bonus</th>
                                                      <th>Salary</th>
                                                      <th>Gross Salary</th>
                                                      <th>C.B.A Subscription</th>
                                                      <th>Deduct AMT</th>
                                                      <th>Net Payable</th>
                                                    </tr>
                                                  </thead>
                                                  <tbody>", companyLogoUrl,
                                                  BusinessUnit?.StrBusinessUnit,
                                                  BusinessUnit?.StrAddress,
                                                  BusinessUnit?.StrWebsiteUrl,
                                                  "",//BusinessUnit?.StrMobile,
                                                  monthName);

            foreach (var item in obj)
            {
                sb.AppendFormat(@"<tr style='page-break-inside: avoid;'>
                                      <td class='text-left'>{0}</td>
                                      <td>{1}</td>
                                      <td>{2}</td>
                                      <td>{3}</td>
                                      <td>{4}</td>
                                      <td>{5}</td>
                                      <td>{6}</td>
                                      <td>{7}</td>
                                      <td>{8}</td>
                                      <td>{9}</td>
                                      <td>{10}</td>
                                    </tr>", item?.DepartmentName,
                                    item?.ManPowerCount,
                                    item?.OverTimeAmount,
                                    item?.ExtraSideDutyAmount,
                                    item?.NightDutyAmount,
                                    item?.AttendanceBonusAmount,
                                    item?.Salary,
                                    item?.GrossSalary,
                                    item?.CbaAmount,
                                    item?.DeductAmount,
                                    item?.NetPayable);
            }

            sb.AppendFormat(@"<tr class='total_row'>
                                  <td class='text-left'>Total</td>
                                  <td>{0}</td>
                                  <td>{1}</td>
                                  <td>{2}</td>
                                  <td>{3}</td>
                                  <td>{4}</td>
                                  <td>{5}</td>
                                  <td>{6}</td>
                                  <td>{7}</td>
                                  <td>{8}</td>
                                  <td>{9}</td>
                                </tr>",
                                obj.Sum(x => x.ManPowerCount),
                                obj.Sum(x => x.OverTimeAmount),
                                obj.Sum(x => x.ExtraSideDutyAmount),
                                obj.Sum(x => x.NightDutyAmount),
                                obj.Sum(x => x.AttendanceBonusAmount),
                                obj.Sum(x => x.Salary),
                                obj.Sum(x => x.GrossSalary),
                                obj.Sum(x => x.CbaAmount),
                                obj.Sum(x => x.DeductAmount),
                                obj.Sum(x => x.NetPayable));

            string amountInWords = await AmountInWords.ConvertToWords(obj.Sum(x => x.NetPayable).Value.ToString());

            sb.AppendFormat(@"      </tbody>
                                </table>
                              </div>
                              <div class='in_words'>In Words:{0}</div>

                            </div>
                          </div>

                          <div class='footer'>
                            <table class='footer_table'>
                              <tbody>
                                <tr>
                                  <td class='authorName'>Department Head</td>
                                  <td>Admin</td>
                                  <td>Accounts</td>
                                </tr>
                              </tbody>
                            </table>
                          </div>
                        </div>
                      </div>
                    <script src='{1}'></script>
                    </body>
                    </html>", amountInWords, commonJsUrl);

            return sb.ToString();
        }

        public async Task<string> RosterReport(List<RosterReportViewModel> obj, MasterBusinessUnit BusinessUnit, string date)
        {
            var sb = new StringBuilder();
            //companyLogoUrl = _hostingEnvironment.ContentRootPath + "/wwwroot/" + BusinessUnit?.StrLogoUrlforPdfReport;

            sb.AppendFormat(@"<html>
                                <head>
                                </head>
                                <body>
                                  <div class='print-page-main-body'>
                                    <div class='print-page-body'>
                                      <div class='clearfix'>
                                        <div class='company-info'>
                                          <div class='companyName'>
                                            <img class='companyImg' src='{0}' alt='Pineapple' width='22' height='17' />
                                            <p class='companyTitle'>{1}</p>
                                          </div>
                                          <div class='companyAddress'>{2}</div>
                                        </div>
                                        <div class='company-social-info'>
                                          <p>{3}</p>
                                          <p>{4}</p>
                                        </div>
                                      </div>

                                      <div class='main-report-body'>

                                        <div class='body_main_table'>
                                          <div class='table_header'>Roster Report </div>
                                          <div class='currDate'>
                                            Date:{5}
                                          </div>
                                          <Div class='table'>
                                            <table>
                                              <thead>
                                                <tr>
                                                  <th>SL</th>
                                                  <th>Employee</th>
                                                  <th>Designation</th>
                                                  <th>Department</th>
                                                  <th>Roster Group Name</th>
                                                  <th>Calendar Name</th>
                                                </tr>
                                              </thead>
                                              <tbody>", companyLogoUrl,
                                              BusinessUnit?.StrBusinessUnit,
                                              BusinessUnit?.StrAddress,
                                              BusinessUnit?.StrWebsiteUrl,
                                              "",//BusinessUnit?.StrMobile,
                                              date);
            int count = 1;
            foreach (var item in obj)
            {
                sb.AppendFormat(@"<tr style='page-break-inside: avoid;'>
                                      <td>{0}</td>
                                      <td>{1} [{2}]</td>
                                      <td>{3}</td>
                                      <td>{4}</td>
                                      <td>{5}</td>
                                      <td>{6}</td>
                                    </tr>", count, item?.strEmployeeName,
                                    item?.strEmployeeCode,
                                    item?.strDesignationName,
                                    item?.strDepartmentName,
                                    item?.strRosterGroupName,
                                    item?.strCalendarName);
                count++;
            }

            sb.AppendFormat(@"               </tbody>
                                        </table>
                                      </Div>
                                    </div>
                                  </div>

                                  <div class='footer'>
                                    <table class='footer_table'>
                                      <tbody>
                                        <tr>
                                          <td class='authorName'>Department Head</td>
                                          <td>Admin</td>
                                          <td>Accounts</td>
                                        </tr>
                                      </tbody>
                                    </table>
                                  </div>
                                </div>
                              </div>
                            <script src='{0}'></script>
                            </body>
                            </html>", commonJsUrl);

            return sb.ToString();
        }

        public async Task<string> MovementReport(List<MovementReportViewModel> obj, MasterBusinessUnit BusinessUnit, string date)
        {
            var sb = new StringBuilder();

            string cmyLogoUrl = _context.GlobalFileUrls.Where(x => x.IsActive == true && x.IntDocumentId == BusinessUnit.StrLogoUrlId).Select(s => s.StrFileServerId).FirstOrDefault();

            if (!string.IsNullOrEmpty(cmyLogoUrl))
            {
                var webclient = new WebClient();
                byte[] img = webclient.DownloadData($"{documentUrl + BusinessUnit.StrLogoUrlId}");
                Stream image = new MemoryStream(img);
                companyLogoUrl = Convert.ToBase64String(img);
            }

            sb.AppendFormat(@"<html>
                                <head>
                                </head>
                                <body>
                                  <div class='print-page-main-body'>
                                    <div class='print-page-body'>
                                      <div class=""clearfix"">
                                          <div class=""company-info"">
                                            <div class=""companyImg-wrapper"">
                                              <img
                                                class=""companyImg""
                                                src='data:image/png;base64, {0}'
                                                alt=""Pineapple""/>
                                            </div>
                                            <p class=""companyTitle"">{1}</p>
                                            <p class=""companyAddress"">{2}</p>
                                          </div>
                                           <div class='company-social-info'>
                                              <p>{3}</p>
                                              <p>{4}</p>
                                            </div>
                                        </div>
                                      <div class='main-report-body'>
                                        <div class='body_main_table'>
                                          <div class='table_header'>Movement Details From {5}</div>
                                          <div class='table'>
                                            <table>
                                              <thead>
                                                <tr>
                                                  <th>SL</th>
                                                  <th>Employee</th>
                                                  <th>Department</th>
                                                  <th>Designation </th>
                                                  <th>Employment Type</th>
                                                  <th class='text-center'>Duration</th>
                                                </tr>
                                              </thead>
                                              <tbody>", companyLogoUrl,
                                              BusinessUnit?.StrBusinessUnit,
                                              BusinessUnit?.StrAddress,
                                              BusinessUnit?.StrWebsiteUrl,
                                              "",//BusinessUnit?.StrMobile,
                                              date);
            int count = 1;
            foreach (var item in obj)
            {
                sb.AppendFormat(@"<tr style='page-break-inside: avoid;'>
                                      <td>{0}</td>
                                      <td>{1}</td>
                                      <td>{2}</td>
                                      <td>{3}</td>
                                      <td>{4}</td>
                                      <td class='text-center'>{5}</td>
                                    </tr>", count, item.EmployeeName,
                                    item?.DepartmentName,
                                    item?.DesignationName,
                                    item?.EmploymentTypeName,
                                    item?.Duration);
                count++;
            }

            sb.AppendFormat(@" </tbody>
                            </table>
                          </div>
                        </div>
                      </div>

                      <div class='footer'>
                        <table class='footer_table'>
                          <tbody>
                            <tr>
                              <td class='authorName'>Department Head</td>
                              <td>Admin</td>
                              <td>Accounts</td>
                            </tr>
                          </tbody>
                        </table>
                      </div>
                    </div>
                  </div>
                <script src='{0}'></script>
                </body>
                </html>", commonJsUrl);

            return sb.ToString();
        }

        public async Task<string> LeaveApplicationApprovalReport(List<LeaveDataSetViewModel> obj)
        {
            var sb = new StringBuilder();
            EmpEmployeeBasicInfo emp = await _context.EmpEmployeeBasicInfos.FirstOrDefaultAsync(x => x.IntEmployeeBasicInfoId == obj.FirstOrDefault().intEmployeeId);
            MasterBusinessUnit BusinessUnit = await _context.MasterBusinessUnits.FirstOrDefaultAsync(x => x.IntBusinessUnitId == emp.IntBusinessUnitId);

            //companyLogoUrl = _hostingEnvironment.ContentRootPath + "/wwwroot/" + BusinessUnit?.StrLogoUrlforPdfReport;

            sb.AppendFormat(@"<html>
                                <head>
                                </head>
                                <body>
                                  <div class='print-page-main-body'>
                                    <div class='print-page-body'>
                                      <div class='clearfix'>
                                        <div class='company-info'>
                                          <div class='companyName'>
                                            <img class='companyImg' src='{0}' alt='Pineapple' width='22' height='17' />
                                            <p class='companyTitle'>{1}</p>
                                          </div>
                                          <div class='companyAddress'>{2}</div>
                                        </div>
                                        <div class='company-social-info'>
                                          <p>{3}</p>
                                          <p>{4}</p>
                                        </div>
                                      </div>
                                      <div class='main-report-body'>
                                        <div class='body_main_table'>
                                          <div class='table_header'>List of Pending Leave Application</div>
                                          <div class='table'>
                                            <table>
                                              <thead>
                                                <tr>
                                                      <th>SL</th>
                                                      <th>Employee</th>
                                                      <th>Designation</th>
                                                      <th>Department</th>
                                                      <th>Leave Type</th>
                                                      <th class='text-center'>Date Range</th>
                                                      <th class='text-center'>Duration</th>
                                                      <th>Reason</th>
                                                      <th>Location</th>
                                                      <th class='text-center'>Application Date</th>
                                                </tr>
                                              </thead>
                                              <tbody>", companyLogoUrl,
                                              BusinessUnit?.StrBusinessUnit,
                                              BusinessUnit?.StrAddress,
                                              BusinessUnit?.StrWebsiteUrl,
                                              "");//BusinessUnit?.StrMobile);
            int count = 1;
            foreach (var item in obj)
            {
                sb.AppendFormat(@"<tr style='page-break-inside: avoid;'>
                                      <td styte='width:50px'>{0}</td>
                                      <td styte='width:150px'>{1}</td>
                                      <td styte='width:150px'>{2}</td>
                                      <td styte='width:100px'>{3}</td>
                                      <td styte='width:100px'>{4}</td>
                                      <td styte='width:100px'>{5}</td>
                                      <td styte='width:250px'>{6}</td>
                                      <td styte='width:50px'>{7}</td>
                                      <td styte='width:100px'>{8}</td>
                                      <td styte='width:100px' class='text-center'>{9}</td>
                                    </tr>", count, item?.strEmployeeName + " [" + item?.strEmployeeCode + "]",
                                    item?.strDesignationName,
                                    item?.strDepartmentName,
                                    item?.strLeaveType,
                                    item?.dteAppliedFromDate?.ToString("dd-MMM-yyyy") + " to " + item?.dteAppliedToDate?.ToString("dd-MMM-yyyy"),
                                    item?.totalDays,
                                    item?.strLeaveReason,
                                    item?.strAddressDuetoLeave,
                                    item?.dteApplicationDate?.ToString("dd-MMM-yyyy"));
                count++;
            }

            sb.AppendFormat(@" </tbody>
                            </table>
                          </div>
                        </div>
                      </div>

                      <div class='footer'>
                        <table class='footer_table'>
                          <tbody>
                            <tr>
                              <td class='authorName'>Department Head</td>
                              <td>Admin</td>
                              <td>Accounts</td>
                            </tr>
                          </tbody>
                        </table>
                      </div>
                    </div>
                  </div>
                <script src='{0}'></script>
                </body>
                </html>", commonJsUrl);

            return sb.ToString();
        }

        public async Task<string> BanglaPdfGenerate()
        {
            var sb = new StringBuilder();

            MasterBusinessUnit BusinessUnit = await _context.MasterBusinessUnits.FirstOrDefaultAsync(x => x.IntBusinessUnitId == 11);

            //companyLogoUrl = _hostingEnvironment.ContentRootPath + "/wwwroot/" + BusinessUnit?.StrLogoUrlforPdfReport;

            sb.AppendFormat(@"<html>
                                <head>
                                </head>
                                <body>
                                  <div class='print-page-main-body'>
                                    <div class='print-page-body'>
                                      <div class='clearfix'>
                                        <div class='company-info'>
                                          <div class='companyName'>
                                            <img class='companyImg' src='{0}' alt='Pineapple' width='22' height='17' />
                                            <p class='companyTitle'>{1}</p>
                                          </div>
                                          <div class='companyAddress'>{2}</div>
                                        </div>
                                        <div class='company-social-info'>
                                          <p>{3}</p>
                                          <p>{4}</p>
                                        </div>
                                      </div>
                                      <div class='main-report-body'>
                                        <div class='body_main_table'>
                                          <div class='table_header'>List of Pending Leave Application</div>",
                                             companyLogoUrl,
                                              BusinessUnit?.StrBusinessUnit,
                                              BusinessUnit?.StrAddress,
                                              BusinessUnit?.StrWebsiteUrl,
                                              "");//BusinessUnit?.StrMobile);

            sb.Append(@"<p>আমাদের গ্রামের নাম রোদডুবি ৷ গ্রামটি হুগলি জেলার সরস্বতী নদীর ভীরে অবস্থিত ৷ আমাদের গ্রামে বহু লোক বাস করেন ৷ এই গ্রামে অনেক পাড়া আছে ৷ প্রতি পড়াের মানুষ মিলেমিশে বাস  করেন ৷ আমাদের গ্রামে একটি মাধ্যমিক বিদ্যালয় ও একটি প্রাথমিক বিদ্যালয় আছে ৷ সেখানে ছেলেমেয়েরা লেখাপড়া শেখে ৷ গ্রামের বেশিরভাগ মানুষই চাষবাস করেন ৷ তবে কেউ কেউ শহরে চাকরিও করতে যান ৷ অনেকে ব্যাবসাও করেন ৷ গ্রামে কাঁচা ও পাকা দু-ধরনের রাস্তাই আছে ৷ বর্ষায়  কাঁচা রাস্তায় খুব কাদা হয় ৷ গ্রামে কাঁচাবাড়ির পাশাপাশি কিছু পাকাবাড়ি ও আছে ৷ তবে আমাদের গ্রামে কোনো হাসপাতাল নেই ৷ আমি আমার গ্রামকে খুব ভালোবাসি ৷</p>");

            sb.AppendFormat(@" </div>
                            </div>
                      <div class='footer'>
                        <table class='footer_table'>
                          <tbody>
                            <tr>
                              <td class='authorName'>Department Head</td>
                              <td>Admin</td>
                              <td>Accounts</td>
                            </tr>
                          </tbody>
                        </table>
                      </div>
                    </div>
                  </div>
                <script src='{0}'></script>
                </body>
                </html>", commonJsUrl);

            return sb.ToString();
        }

        public async Task<string> SalaryPaySlipReportByEmployee(EmployeeSalaryPaySlipViewModel obj)
        {
            var sb = new StringBuilder();
            string PaySlipStyle = _hostingEnvironment.ContentRootPath + "/wwwroot/paySlipStyle.css";

            sb.AppendFormat(@"<html lang='en'>
                              <head>
                                <meta charset='UTF-8' />
                                <meta http-equiv='X-UA-Compatible' content='IE=edge' />
                                <meta name='viewport' content='width=device-width, initial-scale=1.0' />
                                <title>Pay Slip</title>
                                <link rel='stylesheet' href='{0}' />
                              </head>
                              <body style='border:none'>
                                 <div class='salaray-heading' style='border:none; padding-top:150px; line-height: 10px;'>
                                    <div style='float: left; width: 50%;'>
                                        <p>
                                            <span style='width: 200px; display: inline-block'>Date</span>:
                                            <span>{1}</span>
                                        </p>
                                        <p>
                                            <span style='width: 200px; display: inline-block'>Employee ID</span>:
                                            <span>{2}</span>
                                        </p>
                                        <p>
                                            <span style='width: 200px; display: inline-block'>Name of the Employee </span>:
                                            <span>{3}</span>
                                        </p>
                                        <p>
                                            <span style='width: 200px; display: inline-block'>Designation</span>:
                                            <span>{4}</span>
                                        </p>
                                        <p>
                                            <span style='width: 200px; display: inline-block'>Department</span>:
                                            <span>{5}</span>
                                        </p>
                                        <p>
                                            <span style='width: 200px; display: inline-block'>Joining Date</span>:
                                            <span>{6}</span>
                                        </p>
                                        <p>
                                            <span style='width: 200px; display: inline-block'>Employment Type</span>:
                                            <span>{7}</span>
                                        </p>
                                    </div>
                                    <div style='float: right; width: 50%;'>
                                        <p>
                                            <span style='width: 200px; display: inline-block'>Bank Name</span>:
                                            <span>"+obj.strFinancialInstitution+ @"</span>
                                        </p>
                                        <p>
                                            <span style='width: 200px; display: inline-block'>Branch Name</span>:
                                            <span>"+obj.strBankBranchName+ @"</span>
                                        </p>
                                        <p>
                                            <span style='width: 200px; display: inline-block'>Account Name </span>:
                                            <span>"+obj.strAccountName+ @"</span>
                                        </p>
                                        <p>
                                            <span style='width: 200px; display: inline-block'>Account No</span>:
                                            <span>"+obj.strAccountNo+@"</span>
                                        </p>
           
                                    </div>
                                </div>
                                <div style=""padding-top:50px;"">
                                  <div style='text-align: center; line-height: 8px; margin-top: 200px;'>
                                    <div>
                                        <span style='padding:0 5px 5px 5px; margin-bottom: 10px; border-bottom:1px solid black; font-size:20px !important; font-weight:bold'>  Salary Pay Slip</span>
                                     
                                    </div>
                                    
                                    <p>for the month of &nbsp;<b>{8}</b></p>
                                  </div>
                                  <h4>Particulars of Salary</h4>
                                  <table style='width: 100%'>
                                    <tr>
                                      <th style='text-align: left'>Income Head</th>
                                      <th colspan='3'>Amount in BDT</th>
                                    </tr>
                                    <tr>
                                      <th style='text-align: left'>A. Benefits:</th>
                                      <th style='text-align: right'>Current</th>
                                      <th style='text-align: right'>Arrear</th>
                                      <th style='text-align: right'>Total</th>
                                    </tr>", PaySlipStyle,
                                    obj?.GeneratDate,
                                    obj?.EmployeeCode,
                                    obj?.Employee,
                                    obj?.Designation,
                                    obj?.Department,
                                    Convert.ToDateTime(obj?.JoiningDate).ToString("dd-MMM-yyy"),
                                    obj?.EmploymentType,
                                    Convert.ToDateTime(obj?.DateOfPayment).ToString("MMM-yyy"));

            List<PaySlipViewModel> AllowanceAmount = obj.paySlipViewModels.Where(x => x.PayrollElementTypeId == 1.ToString()).ToList();

            decimal currentSalary;
            decimal arrearSalary;
            decimal totalSalary;

            foreach (var item in AllowanceAmount)
            {
                currentSalary = Math.Round((decimal)item?.NumAmount, 2);
                arrearSalary = Math.Round((decimal)item?.Arrear, 2);
                totalSalary = Math.Round((decimal)item?.Total, 2);

                sb.AppendFormat(@"<tr>
                                  <td>{0}</td>
                                  <td style='text-align: right'>{1}</td>
                                  <td style='text-align: right'>{2}</td>
                                  <td style='text-align: right'>{3}</td>
                                </tr>", item?.PayrollElement, currentSalary, arrearSalary, totalSalary);
            }

            currentSalary = Math.Round((decimal)AllowanceAmount?.Sum(x => x.NumAmount), 2);
            arrearSalary = Math.Round((decimal)AllowanceAmount?.Sum(x => x.Arrear), 2);
            totalSalary = Math.Round((decimal)AllowanceAmount?.Sum(x => x.Total), 2);

            sb.AppendFormat(@"
                            <tr>
                                <th style='text-align: left'>Total benefits</th>
                                <th style='text-align: right'>{0}</th>
                                <th style='text-align: right'>{1}</th>
                                <th style='text-align: right'>{2}</th>
                            </tr>
                            <tr>
                                <td colspan='4'></td>
                            </tr>
                            <tr>
                                <th style='text-align: left' colspan='4'>B. Deductions:</th>
                            </tr>",
                            currentSalary,
                            arrearSalary,
                            totalSalary);

            List<PaySlipViewModel> DeductionAmount = obj.paySlipViewModels.Where(x => x.PayrollElementTypeId == 0.ToString()).ToList();

            //foreach (var item in DeductionAmount)
            //{
            //    sb.AppendFormat(@"<tr>
            //                        <td>{0}</td>
            //                        <td colspan='3' style='text-align: right'>{1}</td>
            //                    </tr>", item?.PayrollElement, item?.NumAmount);
            //}


            Account account = _context.Accounts.FirstOrDefault(x => x.IntAccountId == obj.AccountId);
            decimal tax = account.IsTax == true ? Math.Round((decimal)obj.numTaxAmount, 2) : 0;
            decimal loan = account.IsLoan == true ? Math.Round((decimal)obj.numLoanAmount, 2) : 0;
            decimal pfund = account.IsProvidentFund == true ? Math.Round((decimal)obj.numPFAmount, 2) : 0;

            if (account.IsTax == true)
            {
                sb.AppendFormat(@"<tr>
                                <td>Tax</td>
                                <td colspan='3' style='text-align: right'>{0}</td>
                            </tr>", tax);
            }
            if (account.IsLoan == true)
            {
                sb.AppendFormat(@"<tr>
                                <td>Loan</td>
                                <td colspan='3' style='text-align: right'>{0}</td>
                            </tr>", loan);
            }
            if (account.IsProvidentFund == true)
            {
                sb.AppendFormat(@"<tr>
                                <td>Provident Fund</td>
                                <td colspan='3' style='text-align: right'>{0}</td>
                            </tr>", pfund);
            }


            string? inWord = await AmountInWords.ConvertToWords((AllowanceAmount?.Sum(x => x.NumAmount) - Math.Round(tax + loan + pfund, 2)).ToString()) + " only";
            sb.AppendFormat(@"<tr>
                                <th style='text-align: left'>Total deductions</th>
                                <th colspan='3' style='text-align: right'>{0}</th>
                            </tr>
                            <tr>
                                <td colspan='4'></td>
                            </tr>
                            <tr>
                                <th style='text-align: left'>Net Take Home (A-B)</th>
                                <th colspan='3' style='text-align: right'>{1}</th>
                            </tr>
                            </table>
                            <p style='margin-top: 10px'><span>In Word: </span><b>{2}</b> </p>
                                  <p>
                                    <span style='width: 200px; display: inline-block'>Mode of Payment</span>:
		                            <span>{3}</span>
                                  </p>
                                  <p>
                                    <span style='width: 200px; display: inline-block'>Date of Payment</span>:
                                    <span>{4}</span>
                                  </p>

                                  <p style='margin-top: 30px'>Certified by</p>

                                  <div style='margin-top: 100px; line-height: 10px; font-weight: 600'>
                                    <p>{5}</p>
                                  </div>
                                </div>
                              </body>
                            </html>",
                            Math.Round(tax + loan + pfund, 2),
                            Math.Round((decimal)(AllowanceAmount?.Sum(x => x.NumAmount) - (tax + loan + pfund)), 2), inWord == null ? "" : inWord,
                            obj.ModeOfPayment, Convert.ToDateTime(obj?.DateOfPayment).ToString("MMM-yyy"), obj.BusinessUnitName);

            return sb.ToString();
        }

        public async Task<string> SalaryCertificateReportByEmployeeId(EmployeeSalaryPaySlipViewModel obj)
        {
            var sb = new StringBuilder();
            string salaryCertificate = _hostingEnvironment.ContentRootPath + "/wwwroot/salaryCertificate.css";

            sb.AppendFormat(@"<html lang='en'>
                                <head>
                                <meta charset='UTF-8' />
                                <meta http-equiv='X-UA-Compatible' content='IE=edge' />
                                <meta name='viewport' content='width=device-width, initial-scale=1.0' />
                                <title>Salary Certificate</title>
                                <link rel='stylesheet' href='{0}' />
                                </head>
                                <body>
                                <div class='salaray-heading'>
                                    <div class='salaray-heading-table' style='width: 100%; line-height: 10px; padding-top:150px'>
            <div style='float: left; width: 50%;'>
                <p>
                    <span style='width: 200px; display: inline-block'>Date</span>:
                    <span>{1}</span>
                </p>
                <p>
                    <span style='width: 200px; display: inline-block'>Name of the Employee</span>:
                    <span>{2}</span>
                </p>
                <p>
                    <span style='width: 200px; display: inline-block'>Designation</span>:
                    <span>{3}</span>
                </p>
                <p>
                    <span style='width: 200px; display: inline-block'>Joining Date</span>:
                    <span>{4}</span>
                </p>
                <p>
                    <span style='width: 200px; display: inline-block'>Employment Type</span>:
                    <span>{5}</span>
                </p>
            </div>
            <div style='float: right; width: 50%;'>
                <p>
                    <span style='width: 200px; display: inline-block'>Bank Name</span>:
                    <span>"+obj.strFinancialInstitution+ @"</span>
                </p>
                <p>
                    <span style='width: 200px; display: inline-block'>Branch Name</span>:
                    <span>"+obj.strBankBranchName+ @"</span>
                </p>
                <p>
                    <span style='width: 200px; display: inline-block'>Account Name </span>:
                    <span>"+obj.strAccountName+ @"</span>
                </p>
                <p>
                    <span style='width: 200px; display: inline-block'>Account No</span>:
                    <span>"+obj.strAccountNo+@"</span>
                </p>

            </div>
            </div>
            </div>
                <div style='margin-top: 180px !important;'>
                     <div style='margin-top: 100px; text-align:center;'>
                        <span style='padding-bottom:5px; margin-bottom: 10px; border-bottom:1px solid black; font-size:20px !important; font-weight:bold; text-align:center;'>Salary Certificate
                        </span>
                    </div>
                                   
                                    <p>
                                    This is to certify that Mr./Mrs./Ms. <span>{6}</span>, E-TIN
                                    <span>{7}</span>, is an employee of {8} in the position of
                                    {9} of <span>{10}</span>. During the period from
                                    <span>{11}</span> to <span>{12}</span>, his salary and benefits were as
                                    follows:
                                    </p>
                                    <h4>Particulars of Salary</h4>
                                    <table style='width: 100%'>
                                    <tr>
                                        <th>Income Head</th>
                                        <th>Amount in BDT</th>
                                    </tr>
                                    <tr>
                                        <th>A. Benefits:</th>
                                        <th></th>
                                    </tr>", salaryCertificate,
                                    obj?.GeneratDate,
                                    obj?.Employee,
                                    obj?.Designation,
                                    Convert.ToDateTime(obj?.JoiningDate).ToString("dd-MMM-yyy"),
                                    obj?.EmploymentType,
                                    obj?.Employee, "", obj?.BusinessUnitName, obj?.HRPostionName, obj?.Designation,
                                    Convert.ToDateTime(obj?.JoiningDate).ToString("dd-MMM-yyy"),
                                    DateTime.Now.Date.ToString("dd-MMM-yyy"));

            List<PaySlipViewModel> AllowanceAmount = obj.paySlipViewModels.Where(x => x.PayrollElementTypeId == 1.ToString()).ToList();

            foreach (var item in AllowanceAmount)
            {
                sb.AppendFormat(@"<tr>
                                  <td>{0}</td>
                                  <td>{1}</td>
                                </tr>", item?.PayrollElement, Math.Round((decimal)item?.NumAmount, 2));
            }

            sb.AppendFormat(@"<tr>
                                <th>Total benefits</th>
                                <th>{0}</th>
                            </tr>
                            <tr>
                                <td colspan='2'></td>
                            </tr>
                            <tr>
                                <th>B. Deductions:</th>
                                <th></th>
                            </tr>", Math.Round((decimal)AllowanceAmount?.Sum(x => x.NumAmount), 2));

            Account account = _context.Accounts.FirstOrDefault(x => x.IntAccountId == obj.AccountId);
            decimal tax = account.IsTax == true ? Math.Round((decimal)obj.numTaxAmount, 2) : 0;
            decimal loan = account.IsLoan == true ? Math.Round((decimal)obj.numLoanAmount, 2) : 0;
            decimal pfund = account.IsProvidentFund == true ? Math.Round((decimal)obj.numPFAmount, 2) : 0;

            if (account.IsTax == true)
            {
                sb.AppendFormat(@"<tr>
                                <td>Tax</td>
                                <td colspan='3' style='text-align: right'>{0}</td>
                            </tr>", tax);
            }
            if (account.IsLoan == true)
            {
                sb.AppendFormat(@"<tr>
                                <td>Loan</td>
                                <td colspan='3' style='text-align: right'>{0}</td>
                            </tr>", loan);
            }
            if (account.IsProvidentFund == true)
            {
                sb.AppendFormat(@"<tr>
                                <td>Provident Fund</td>
                                <td colspan='3' style='text-align: right'>{0}</td>
                            </tr>", pfund);
            }

            string? inWord = await AmountInWords.ConvertToWords((AllowanceAmount?.Sum(x => x.NumAmount) - Math.Round(tax + loan + pfund, 2)).ToString()) + " only";

            sb.AppendFormat(@"<tr>
                                <th>Total deductions</th>
                                <th>{0}</th>
                            </tr>
                            <tr>
                                <td colspan='2'></td>
                            </tr>
                            <tr>
                                <th>Net Take Home (A-B)</th>
                                <th>{1}</th>
                            </tr>
                            </table>

                            <p style='margin-top: 20px'><span>In Word: </span><b>{2}</b></p>

                            <p style='margin-top: 50px'>Certified by</p>

                            <div style='margin-top: 100px; line-height: 10px; font-weight: 600;'>
                            <p>{3}</p>
                            </div>
                        </div>
                        </body>
                    </html>", Math.Round(tax + loan + pfund, 2),
                             Math.Round((decimal)(AllowanceAmount?.Sum(x => x.NumAmount) - (tax + loan + pfund)), 2),
                            inWord == null ? "" : inWord,
                            obj.BusinessUnitName);

            return sb.ToString();
        }

        public async Task<string> AllTypeOfSalaryReport(SalaryAllReportViewModel obj, int IntBankOrWalletType)
        {
            var sb = new StringBuilder();
            string salaryReportAll = _hostingEnvironment.ContentRootPath + "/wwwroot/salaryReportAllStyle.css";

            sb.AppendFormat(@"<html lang='en'>
                                <head>
                                  <meta charset='UTF-8' />
                                  <meta http-equiv='X-UA-Compatible' content='IE=edge' />
                                  <meta name='viewport' content='width=device-width, initial-scale=1.0' />
                                  <title>Bond Report</title>
                                  <link rel='stylesheet' href='{0}' />
                                </head>
                                <body>
                                  <div class='report-header'>
                                    <h5>{1}</h5>
                                    <h6>
                                      {2}
                                    </h6>
                                    <h5>Salary report for the month of {3}</h5>
                                  </div>
                                  <div class='report-body'>
                                    <table style='width: 100%'>
                                      <tbody>"
                                    , salaryReportAll,
                                    obj?.BusinessUnit,
                                    obj?.Address,
                                    Convert.ToDateTime(obj?.GenerateDate).ToString("MMM-yyy"));

            if (IntBankOrWalletType == 0)
            {
                sb.AppendFormat(@"<tr class='table-head'>
                                    <td>SL</td>
                                    <td>Employee Name</td>
                                    <td>Employee Id</td>
                                    <td>Salary</td>
                                    <td>Total Allowance</td>
                                    <td>Total Deduction</td>
                                    <td>Net Pay</td>
                                    <td>Bank Pay</td>
                                    <td>Digital Bank Pay</td>
                                    <td>Cash Pay</td>
                                    <td>Workplace</td>
                                    <td>Workplace Groupe</td>
                                    <td>Payroll Groupe</td>
                                </tr>
                                </tbody>
                                <tbody>");

                long sl = 1;
                foreach (var item in obj.salaryReportHeaderViewModels)
                {
                    sb.AppendFormat(@"<tr>
                                        <td>{0}</td>
                                        <td>{1}</td>
                                        <td>{2}</td>
                                        <td style='text-align:right'>{3}</td>
                                        <td style='text-align:right'>{4}</td>
                                        <td style='text-align:right'>{5}</td>
                                        <td style='text-align:right'>{6}</td>
                                        <td style='text-align:right'>{7}</td>
                                        <td style='text-align:right'>{8}</td>
                                        <td style='text-align:right'>{9}</td>
                                        <td>{10}</td>
                                        <td>{11}</td>
                                        <td>{12}</td>
                                    </tr>"
                                    , sl, item.EmployeeName, item.EmployeeId, item.Salary, item.TotalAllowance, item.TotalDecuction,
                                    item.Netpay, item.BankPay, item.DigitalBank, item.CashPay, item.Workplace, item.WorkplaceGroupe, item.PayrollGroupe);
                    sl++;
                }

                sb.AppendFormat(@"<tr class='table-head'>
                                    <td colspan='3' style='text-align:right'>Total</td>
                                    <td style='text-align:right'>{0}</td>
                                    <td style='text-align:right'>{1}</td>
                                    <td style='text-align:right'>{2}</td>
                                    <td style='text-align:right'>{3}</td>
                                    <td style='text-align:right'>{4}</td>
                                    <td style='text-align:right'>{5}</td>
                                    <td style='text-align:right'>{6}</td>
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                </tr>
                                </table>
                              </div>
                            </body>
                            </html>", obj.salaryReportHeaderViewModels.Sum(x => x.Salary),
                                obj.salaryReportHeaderViewModels.Sum(x => x.TotalAllowance),
                                obj.salaryReportHeaderViewModels.Sum(x => x.TotalDecuction),
                                obj.salaryReportHeaderViewModels.Sum(x => x.Netpay),
                                obj.salaryReportHeaderViewModels.Sum(x => x.BankPay),
                                obj.salaryReportHeaderViewModels.Sum(x => x.DigitalBank),
                                obj.salaryReportHeaderViewModels.Sum(x => x.CashPay));
            }
            else if (IntBankOrWalletType == 1)
            {
                sb.AppendFormat(@"<tr class='table-head'>
                                    <td>SL</td>
                                    <td>Account Name</td>
                                    <td>Employee Id</td>
                                    <td>Bank Name</td>
                                    <td>Branch</td>
                                    <td>Account No</td>
                                    <td>Routing No</td>
                                    <td>Net Pay</td>
                                    <td>Bank Pay</td>
                                    <td>Cash Pay</td>
                                    <td>Workplace</td>
                                    <td>Workplace Groupe</td>
                                    <td>Payroll Groupe</td>
                                </tr>
                                </tbody>
                                <tbody>");

                long sl = 1;
                foreach (var item in obj.salaryReportHeaderViewModels)
                {
                    sb.AppendFormat(@"<tr>
                                        <td>{0}</td>
                                        <td>{1}</td>
                                        <td>{2}</td>
                                        <td>{3}</td>
                                        <td>{4}</td>
                                        <td>{5}</td>
                                        <td>{6}</td>
                                        <td style='text-align:right'>{7}</td>
                                        <td style='text-align:right'>{8}</td>
                                        <td style='text-align:right'>{9}</td>
                                        <td>{10}</td>
                                        <td>{11}</td>
                                        <td>{12}</td>
                                    </tr>"
                                    , sl, item.AccountName, item.EmployeeId, item.BankName, item.Branch, item.AccountNo,
                                    item.RoutingNo, item.Netpay, item.BankPay, item.CashPay, item.Workplace, item.WorkplaceGroupe, item.PayrollGroupe);
                    sl++;
                }

                sb.AppendFormat(@"<tr class='table-head'>
                                    <td colspan='7' style='text-align:right'>Total</td>
                                    <td style='text-align:right'>{0}</td>
                                    <td style='text-align:right'>{1}</td>
                                    <td style='text-align:right'>{2}</td>
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                </tr>
                            </table>
                            </div>
                            </body>
                            </html>"
                                , obj.salaryReportHeaderViewModels.Sum(x => x.Netpay),
                                obj.salaryReportHeaderViewModels.Sum(x => x.BankPay),
                                obj.salaryReportHeaderViewModels.Sum(x => x.CashPay));
            }
            else if (IntBankOrWalletType == 2)
            {
                sb.AppendFormat(@"<tr class='table-head'>
                                    <td>SL</td>
                                    <td>Employee Name</td>
                                    <td>Employee Id</td>
                                    <td>GateWay</td>
                                    <td>Mobile No</td>
                                    <td>Net Payable</td>
                                    <td>Workplace</td>
                                    <td>Workplace Groupe</td>
                                    <td>Payroll Groupe</td>
                                </tr>
                                </tbody>
                                <tbody>");

                long sl = 1;
                foreach (var item in obj.salaryReportHeaderViewModels)
                {
                    sb.AppendFormat(@"<tr>
                                        <td>{0}</td>
                                        <td>{1}</td>
                                        <td>{2}</td>
                                        <td>{3}</td>
                                        <td>{4}</td>
                                        <td style='text-align:right'>{5}</td>
                                        <td>{6}</td>
                                        <td>{7}</td>
                                        <td>{8}</td>
                                    </tr>"
                                    , sl, item.EmployeeName, item.EmployeeId, item.BankName, item.AccountNo, item.Netpay,
                                    item.Workplace, item.WorkplaceGroupe, item.PayrollGroupe);
                    sl++;
                }

                sb.AppendFormat(@"<tr class='table-head'>
                                    <td colspan='5' style='text-align:right'>Total</td>
                                    <td style='text-align:right'>{0}</td>
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                </tr>
                                </table>
                                </div>
                                </body>
                                </html>", obj.salaryReportHeaderViewModels.Sum(x => x.Netpay));
            }
            else
            {
                sb.AppendFormat(@"<tr class='table-head'>
                                    <td>SL</td>
                                    <td>Employee Name</td>
                                    <td>Employee Id</td>
                                    <td>Net Payable</td>
                                    <td>Workplace</td>
                                    <td>Workplace Groupe</td>
                                    <td>Payroll Groupe</td>

                                </tr>
                                </tbody>
                                <tbody>");

                long sl = 1;
                foreach (var item in obj.salaryReportHeaderViewModels)
                {
                    sb.AppendFormat(@"<tr>
                                        <td>{0}</td>
                                        <td>{1}</td>
                                        <td>{2}</td>
                                        <td style='text-align:right'>{3}</td>
                                        <td>{4}</td>
                                        <td>{5}</td>
                                        <td>{6}</td>

                                    </tr>"
                                    , sl, item.EmployeeName, item.EmployeeId, item.Netpay,
                                    item.Workplace, item.WorkplaceGroupe, item.PayrollGroupe);
                    sl++;
                }

                sb.AppendFormat(@"<tr class='table-head'>
                                    <td colspan='3' style='text-align:right'>Total</td>
                                    <td style='text-align:right'>{0}</td>
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                </tr>
                            </table>
                            </div>
                            </body>
                            </html>", obj.salaryReportHeaderViewModels.Sum(x => x.Netpay));
            }

            return sb.ToString();
        }

        public async Task<string> BankAdviceReport(BankAdviceViewModel obj)
        {
            var sb = new StringBuilder();
            string bankAdvice = _hostingEnvironment.ContentRootPath + "/wwwroot/bankAdviceStyle.css";

            string? inWord = await AmountInWords.ConvertToWords(obj?.TotalCreditAmount.ToString()) + " only"; ;

            sb.AppendFormat(@"<html lang='en'>
                                <head>
                                <meta charset='UTF-8' />
                                <meta http-equiv='X-UA-Compatible' content='IE=edge' />
                                <meta name='viewport' content='width=device-width, initial-scale=1.0' />
                                <link rel='stylesheet' href='{0}' />
                                <title>Salary Advice</title>
                                </head>
                                <body>
                                <div>
                                    <div class='heading' style='margin-top: 192px'>
                                    <p class='aplication-date'>{1}</p>
                                    <div style='line-height: 10px; margin: 30px 0'>
                                        <p class='to'>The Head Of Branch</p>
                                        <p>{2}</p>
                                        <p>{3}</p>
                                        <p>{4}</p>
                                    </div>
                                    </div>

                                    <div class='subject-section'>
                                    <p>
                                        Subject:
                                        <span>Bulk Electronic Fund Transfer (Credit Instruction) requested
                                        through BEFTN (Bangladesh Electronic Fund Transfer Network){5}</span>
                                    </p>
                                    </div>
                                    <div class='application-body'>
                                    <p>Dear Sir/Madam,</p>
                                    <p>
                                        We want to transfer fund through BEFTN facility from your branch. We
                                        have read and understood and shall abide by all the terms and
                                        conditions of UCBL governing the set <b>BEFTN Operating{6}</b>. The
                                        number details are attached herewith duly signed by the signatory/signatories
                                    </p>

                                    <div class='application-table-body'>
                                        <table style='width: 100%'>
                                        <tr>
                                            <th style='text-align: left' colspan='2'>Bulk BFT Details</th>
                                        </tr>
                                        <tr>
                                            <td>Total Credit Amount is BDT:</td>
                                            <td style='text-align: right'>{7}</td>
                                        </tr>
                                        <tr>
                                            <td>Amount in words:</td>
                                            <td>{8}</td>
                                        </tr>
                                        <tr>
                                            <td>Total number of transactions:</td>
                                            <td style='text-align: right'>{9}</td>
                                        </tr>
                                        <tr>
                                            <td>Attachment (no. sheets):</td>
                                            <td style='text-align: right'>{10}</td>
                                        </tr>
                                        </table>
                                    </div>
                                    <p>
                                        We would like to request you to kindly transfer fund through BEFTN for
                                        Salary of <span>{11}</span> of {12}, payment of BDT
                                        <span>{13}</span> (<span>in words:</span> {14}) by debiting our current
                                        account of {15}, Account Number
                                        <span>{16}</span> maintained with you.
                                    </p>
                                    <p>
                                        Please also note that detailed salary sheet along with accounts name,
                                        accounts no., routing number and amount is also enclosed herewith this
                                        salary advice.
                                    </p>
                                    <p style='margin-top: 30px'>Thanking you</p>
                                    </div>
                                </div>
                                </body>
                            </html>", bankAdvice,
                            Convert.ToDateTime(obj?.GeneratDate).ToString("dd MMMM, yyyy"),
                            obj?.CompanyBank, obj?.BankAddress, "", "", "", obj?.TotalCreditAmount, inWord, obj?.TotalTransactions,
                            obj?.TotalAttachment, Convert.ToDateTime(obj?.GeneratDate).ToString("MMM, yyyy"),
                            obj?.BusinessUnit, obj?.TotalCreditAmount, inWord, obj?.TotalCreditAmount, obj?.BankAccountNo);

            return sb.ToString();
        }
        public async Task<string> BonusReport(BonusReportVM obj, MasterBusinessUnit businessUnit)
        {
            var sb = new StringBuilder();
            //< td class=""bold_font font_18"">Sole Depo: </td>
            //<td align = ""right"" class=""bold_font font_18"">Wing: </td>
            sb.AppendFormat(@"<!DOCTYPE html>
                                <html lang=""en"">
                                  <head>
                                  </head>
                                  <body>
                                    <div>
                                      <div class=""center_text"">
                                        <h1>" + businessUnit.StrBusinessUnit + @"</h1>
                                        <p class=""mt_5"">" + businessUnit.StrAddress + @"</p>
                                        <h3 class=""text_underlined mt_5"">
                                          Bonus Sheet of "+obj.strWorkplaceGroupName+@" (" + obj.strBonusName + @")
                                        </h3>
                                      </div>
                                      <div class=""mt_26"">
                                        <table class=""width_100"">
                                          <tbody>
                                            <tr>
                                              
                                              <td align=""right"" class=""bold_font font_18"">
                                                Effective Date: " + Convert.ToDateTime(obj.EffectiveDate).ToString("dd MMM, yyyy") + @"
                                              </td>
                                            </tr>
                                          </tbody>
                                        </table>
                                        <div class=""bonusGen"">
                                          <table class=""width_100"">
                                            <thead>
                                              <tr>
                                                <td class=""bold_font font_18"" align=""center"" style='padding-left:2px;'>SL</td>
                                                <td class=""bold_font font_18"" align=""center"">Employee ID</td>
                                                <td class=""bold_font font_18"" align=""center"">Employee Name</td>
                                                <td class=""bold_font font_18"" align=""center"">Designation</td>
                                                <td class=""bold_font font_18"" align=""center"">Joining Date</td>
                                                <td class=""bold_font font_18"" align=""center"">Job Duration</td>
                                                <td class=""bold_font font_18"" align=""center"">Gross Salary (TK.)</td>
                                                <td class=""bold_font font_18"" align=""center"">Bonus Percentage</td>
                                                <td class=""bold_font font_18"" align=""center"">Bonus Amount (TK.)</td>
                                                <td class=""bold_font font_18"" align=""center"">Remarks/ Signature</td>
                                              </tr>
                                            </thead>
                                            <tbody>");
            var tCount = obj.data.Count();
            var count = 0;
            string? inWord = "";
            foreach (var item in obj.data)
            {
                count++;

                if (!String.IsNullOrEmpty(item.StrDepartmentName))
                {
                    sb.AppendFormat(@"<tr style=""text-align:center;"">
                                                <td class='font_td'>{0}</td>
                                                <td class='font_td'>{1}</td>
                                                <td class='font_td'>{2}</td>
                                                <td class='font_td'>{3}</td>
                                                <td class='font_td'>{4}</td>
                                                <td class='font_td'>{5}</td>
                                                <td class='font_td'>{6}</td>
                                                <td class='font_td'>{7}</td>
                                                <td class='font_td'>{8}</td>
                                                <td class='font_td'>{9}</td>
                                              </tr>",
                                              item.SL, item.StrEmployeeCode, item.StrEmployeeName, item.StrDesignationName,
                                              Convert.ToDateTime(item.DteJoiningDate).ToString("dd MMM, yyyy"), item.StrServiceLength, item.NumSalary,
                                              Convert.ToInt32(item.NumBonusPercentage) +"%", item.NumBonusAmount, "");
                }
                else if(!String.IsNullOrEmpty(item.DeptName))
                {
                    
                    sb.AppendFormat(@"<tr>
                                    <td class=""bold_font font_18"" colspan=""10""> Dept: {0}</td>
                                    
                                    </tr>", item.DeptName);
                }
                else if(String.IsNullOrEmpty(item.DeptName) && String.IsNullOrEmpty(item.SL) && count != tCount)
                {
                    sb.AppendFormat(@"<tr>
                                    <td colspan=""6"" class=""bold_font font_18 center_text"">
                                        Sub Total
                                    </td>
                                    <td style=""text-align:center"" class=""font_18 bold_font"">{0}</td>
                                    <td></td>
                                    <td style=""text-align:center"" class=""font_18 bold_font"">{1}</td>
                                    <td></td>
                                    </tr>", item.NumSalary, item.NumBonusAmount);

                }
                else
                {
                    sb.AppendFormat(@" <tr>
                                <td class=""bold_font font_18 center_text"" colspan=""6"">Total</td>
                                <td style=""text-align:center"" class=""font_18 bold_font"">{0}</td>
                                <td></td>
                                <td style=""text-align:center"" class=""font_18 bold_font"">{1}</td>
                                <td></td>
                                </tr>", item.NumSalary, item.NumBonusAmount);
                    inWord = await AmountInWords.ConvertToWords(item.NumBonusAmount.ToString()) + " only";
                }
                
            }

            
            sb.AppendFormat(@"
                            </tbody>
                            </table>
                        </div>
                        <div style=""margin-top: 20px"" class=""bold_font font_18"">In Words: "+inWord+@"</div>
                        <div style=""margin-top: 80px"">
                          <table class=""width_100"">
                            <tbody>
                              <tr>
                                <td align=""center"">
                                  <button
                                    class=""bold_font""
                                    style=""
                                      background-color: transparent;
                                      border: none;
                                      border-top: 1px solid black;
                                      padding: 0 20px;
                                      font-size: 10px;
                                    ""
                                  >
                                    Prepared By
                                  </button>
                                </td>
                                <td align=""center"">
                                  <button
                                    class=""bold_font""
                                    style=""
                                      background-color: transparent;
                                      border: none;
                                      border-top: 1px solid black;
                                      padding: 0 20px;
                                      font-size: 10px;
                                    ""
                                  >
                                    Deputy Manager (C&B)
                                  </button>
                                </td>
                                <td align=""center"">
                                  <button
                                    class=""bold_font""
                                    style=""
                                      background-color: transparent;
                                      border: none;
                                      border-top: 1px solid black;
                                      padding: 0 20px;
                                      font-size: 10px;
                                    ""
                                  >
                                    Human Resource Dept.
                                  </button>
                                </td>
                                <td align=""center"">
                                  <button
                                    class=""bold_font""
                                    style=""
                                      background-color: transparent;
                                      border: none;
                                      border-top: 1px solid black;
                                      padding: 0 20px;
                                      font-size: 10px;
                                    ""
                                  >
                                    Audit Department
                                  </button>
                                </td>
                                <td align=""center"">
                                  <button
                                    class=""bold_font""
                                    style=""
                                      background-color: transparent;
                                      border: none;
                                      border-top: 1px solid black;
                                      padding: 0 20px;
                                      font-size: 10px;
                                    ""
                                  >
                                    Sr. Manager (F&A)
                                  </button>
                                </td>
                              </tr>
                              <tr>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td style=""padding-top: 80px"" align=""center"">
                                  <button
                                    class=""bold_font""
                                    style=""
                                      background-color: transparent;
                                      border: none;
                                      border-top: 1px solid black;
                                      padding: 0 20px;
                                      font-size: 10px;
                                    ""
                                  >
                                    Director-Audit (Sales & Marketing)
                                  </button>
                                </td>
                                <td style=""padding-top: 80px"" align=""center"">
                                  <button
                                    class=""bold_font""
                                    style=""
                                      background-color: transparent;
                                      border: none;
                                      border-top: 1px solid black;
                                      padding: 0 20px;
                                      font-size: 10px;
                                    ""
                                  >
                                    Director(Finance)
                                  </button>
                                </td>
                              </tr>
                            </tbody>
                          </table>
                        </div>
                        </div>
                    </div>
                    </body>
                </html>
                ");


            return sb.ToString();
        }
        public async Task<string> IncrementLetterReport(IncrementLetterViewModel obj)
        {
            var sb = new StringBuilder();
            string incrementLetterStyle = _hostingEnvironment.ContentRootPath + "/wwwroot/increment.css";

            sb.AppendFormat(@"<!DOCTYPE html>
                        <html lang='en'>
                          <head>
                            <meta charset='UTF-8' />
                            <meta http-equiv='X-UA-Compatible' content='IE=edge' />
                            <meta name='viewport' content='width=device-width, initial-scale=1.0' />
                            <link rel='stylesheet' href='{0}' />
                            <title>Increment Letter</title>
                          </head>
                          <body>
                            <div class='increment'>
                              <div>
                                <div>Date: {1}</div>
                                <div>To,</div>
                                <div class='font-bold'>{2}</div>
                                <div>{3}, {4}</div>
                                <div>Business Unit: {5}.</div>
                              </div>
                              <div>
                                <br />
                                <p class='font-bold center-text'>Letter of Increment</p>
                                <p>
                                  We take this opportunity to let you know, your performance during the
                                  lane year was good. Based on the current inflation rate, comparative
                                  salary structure, and performance evaluation, we revised your salary
                                  structure. As such, your salary structure stands as follows which will
                                  effect from {6}.
                                </p>
                              </div>

                              <div>
                                <p class='font-bold'>Information:</p>
                                <div class='table_section_container'>
                                  <table>
                                    <thead>
                                      <tr>
                                        <td colspan='3'>Existing Salary</td>
                                      </tr>
                                    </thead>
                                    <tbody>",
                                       incrementLetterStyle, Convert.ToDateTime(DateTime.Now).ToString("dd MMMM, yyyy"),
                                       obj?.EmployeeName, obj?.Designation, obj?.Department, obj?.BusinessUnit,
                                       Convert.ToDateTime(obj?.IncrementSalaryDate).ToString("MMMM dd, yyyy"));

            foreach (var item in obj.existingSalaryViewModels)
            {
                sb.AppendFormat(@"<tr>
                <td>{0}</td>
                <td>=</td>
                <td>{1}</td>
              </tr>", item?.SalaryElement, item?.numAmount);
            }

            double existingSalary = (double)obj.existingSalaryViewModels.Sum(x => x.numAmount);

            sb.AppendFormat(@" <tr class='font-bold'>
                               <td>Total (BDT)</td>
                               <td>=</td>
                               <td>{0}</td>
                             </tr>
                             <tr>
                               <td colspan='3'></td>
                             </tr>
                             <tr>
                               <td>Annual Pay</td>
                               <td>=</td>
                               <td>{1}</td>
                             </tr>
                             <tr>
                               <td>Total Festival Bonus</td>
                               <td>=</td>
                               <td>{2}</td>
                             </tr>
                             <tr class='font-bold'>
                               <td>Total Annual Pay</td>
                               <td>=</td>
                               <td>{3}</td>
                             </tr>
                           </tbody>
                         </table>

                         <table class='table__1'>
                           <thead>
                             <tr>
                               <td colspan='3'>New Salary</td>
                             </tr>
                           </thead>
                           <tbody>", existingSalary,
                              existingSalary * 12,
                              existingSalary * 50 * 0.01 * 2,
                              (existingSalary * 12) + (existingSalary * 50 * 0.01 * 2));

            foreach (var item in obj.incrementSalaryViewModels)
            {
                sb.AppendFormat(@"<tr>
                <td>{0}</td>
                <td>=</td>
                <td>{1}</td>
              </tr>", item?.SalaryElement, item?.numAmount);
            }

            double incrementedSalary = (double)obj.incrementSalaryViewModels.Sum(x => x.numAmount);
            string? inWord = await AmountInWords.ConvertToWords(incrementedSalary.ToString()) + " only";

            sb.AppendFormat(@"<tr class='font-bold'>
                               <td>Total (BDT)</td>
                               <td>=</td>
                               <td>{0}</td>
                             </tr>

                             <tr>
                               <td colspan='3'></td>
                             </tr>
                             <tr>
                               <td>Annual Pay</td>
                               <td>=</td>
                               <td>{1}</td>
                             </tr>
                             <tr>
                               <td>Total Festival Bonus</td>
                               <td>=</td>
                               <td>{2}</td>
                             </tr>
                             <tr class='font-bold'>
                               <td>Total Annual Pay</td>
                               <td>=</td>
                               <td>{3}</td>
                             </tr>
                           </tbody>
                         </table>
                       </div>
                       <p class='font-bold'>
                         Amount in Words: {4}(Monthly)
                       </p>

                       <p>
                         The above is subject to further enhancement, if you awarded special
                         increment.
                       </p>
                       <p>
                         The year ahead is going to pose several challenges. We demand
                         exceptional performance from you. We are confident that you will rise
                         to this challenge and pool in your energies, competencies and
                         commitment to achieve further heights.
                       </p>
                       <br />
                       <div>
                         <p>Behalf of the Management</p>
                       </div>

                       <div class='signit'>
                         <div class='font-bold'>{5}</div>
                         <div>{6}, {7}</div>
                         <div>{8}</div>
                       </div>
                     </div>
                   </div>
                 </body>
               </html>
               ", incrementedSalary, incrementedSalary * 12,
                  incrementedSalary * 50 * 0.01 * 2, (incrementedSalary * 12) + (incrementedSalary * 50 * 0.01 * 2),
                  inWord, obj?.ApprovarName, obj?.ApprovarDesignation, obj?.ApprovarDepartment, obj?.ApprovarBusinessUnit);

            return sb.ToString();
        }

        public async Task<string> TransferAndPromotionReport(TransferAndPromotionReportPDFViewModel obj)
        {
            var sb = new StringBuilder();
            string transferAndPromotionStyle = _hostingEnvironment.ContentRootPath + "/wwwroot/transferAndPromotion.css";

            //string? inWord = await AmountInWords.ConvertToWords(obj?.AmountInWords.ToString());

            sb.AppendFormat(@"<!DOCTYPE html>
               <html lang='en'>
                 <head>
                   <meta charset='UTF-8' />
                   <meta http-equiv='X-UA-Compatible' content='IE=edge' />
                   <meta name='viewport' content='width=device-width, initial-scale=1.0' />
                   <link rel='stylesheet' href='{0}' />
                   <title>Transfer and Promotion Letter</title>
                 </head>
                 <body>
                   <div class='promotion'>
                    <p style='width:100%; height:230px'></p>
                     <div>
                       <div>Date: {1}</div>
                       <div>To,</div>
                       <div class='font-bold'>{2}</div>
                       <div>{3}</div>
                       <div>{4}</div>
                     </div>
                     <div>
                       <br />
                       <p class='font-bold underlined-text'>Transfer and Promotion Letter</p>
                       <p>Dear {5},</p>
                       <p>
                         It is our pleasure to inform you that the management has decided to
                         transfer you to our {6}. You are here by advised to report to the
                         Manager-HR & Admin as per management decision. And we are also pleased
                         to inform you that with the transfer, you are also being promoted from
                         {7} to {8} at {9}.,
                         effective {10}.
                       </p>
                       <p>
                         With this transfer and promotion, you’ll be assigned new roles and
                         responsibilities, which you'll need to adhere to. I hope you continue
                         to work with the same dedication in your new position in the future.
                       </p>
                     </div>

                     <div>
                       <p>We congratulate you on your achievement.</p>
                       <p>Behalf of the Management</p>

                       <div class='signit'>
                         <div class='font-bold'>{11}</div>
                         ", transferAndPromotionStyle, Convert.ToDateTime(DateTime.Now).ToString("dd MMMM, yyyy"),
               obj?.EmployeeName, obj?.Designation, obj?.Department, obj?.EmployeeName, obj?.BusinessUnit, obj?.PreviousDesignation,
               obj?.PromotedDesignation, obj?.PromotedBusinessUnit, Convert.ToDateTime(obj?.PromotedDate).ToString("MMMM dd, yyyy"),
               obj?.ApprovarName);

            if ((obj.ApprovarDesignation == null || obj.ApprovarDesignation == "") && (obj.ApprovarDepartment != null || obj.ApprovarDepartment != ""))
            {
                sb.AppendFormat(@"<div>{0}</div>
                         <div>{1}</div>
                       </div>
                     </div>
                   </div>
                 </body>
               </html>", obj?.ApprovarDepartment,
               obj?.ApprovarBusinessUnit);
            }
            else if ((obj.ApprovarDepartment == null || obj.ApprovarDepartment == "") && (obj.ApprovarDesignation != null || obj.ApprovarDesignation != ""))
            {
                sb.AppendFormat(@"<div>{0}</div>
                         <div>{1}</div>
                       </div>
                     </div>
                   </div>
                 </body>
               </html>", obj?.ApprovarDesignation,
               obj?.ApprovarBusinessUnit);
            }
            else
            {
                sb.AppendFormat(@"<div>{0}</div>
                       </div>
                     </div>
                   </div>
                 </body>
               </html>", obj?.ApprovarBusinessUnit);
            }
            return sb.ToString();
        }

        public async Task<string> TransferReport(TransferReportViewModel obj)
        {
            var sb = new StringBuilder();
            string transferReportStyle = _hostingEnvironment.ContentRootPath + "/wwwroot/transfer.css";

            //string? inWord = await AmountInWords.ConvertToWords(obj?.AmountInWords.ToString());

            sb.AppendFormat(@"<!DOCTYPE html>
                        <html lang='en'>
                           <head>
                              <meta charset='UTF-8' />
                              <meta http-equiv='X-UA-Compatible' content='IE=edge' />
                              <meta name='viewport' content='width=device-width, initial-scale=1.0' />
                              <link rel='stylesheet' href='{0}' />
                              <title>Transfer Letter</title>
                           </head>
                           <body>
                              <p style='width:100%;height:200px'></p>
                              <div class='transfer'>
                              <div>
                                 <div>Date: {1}</div>
                                 <div>To,</div>
                                 <div class='font-bold'>{2}</div>
                                 <div>{3}</div>
                                 <div>{4}</div>
                              </div>
                              <div>
                                 <p class='font-bold'>Subject: Transfer Letter.</p>
                              </div>
                              <div>
                                 <p>
                                    Dear {5}, <br />
                                    <br />

                                    It is our pleasure to inform you that the management has decided to
                                    Transfer you in our {6}. You are here by advised to report
                                    Manager- HR & Admin as per management Decision.
                                 </p>
                              </div>
                              <br />

                              <div>
                                 <p class='font-bold'>Information:</p>
                                 <table>
                                    <thead>
                                    <tr>
                                       <td>Name</td>
                                       <td>Designation</td>
                                       <td>Present Salary</td>
                                       <td>Joining Date</td>
                                       <td>From</td>
                                       <td>To</td>
                                       <td>Effective Date</td>
                                    </tr>
                                    </thead>
                                    <tbody>
                                    <td>{7}</td>
                                    <td>{8}</td>
                                    <td>{9}</td>
                                    <td>{10}</td>
                                    <td>{11}</td>
                                    <td>{12}</td>
                                    <td>{13}</td>
                                    </tbody>
                                 </table>
                                 <br>
                                 <p>We wish you success in your endeavour.</p>

                                 <div class='signit'>
                                    <div>Best Regards.</div>
                                    <div>On behalf of</div>
                                    <div>{14}</div>
                                    <div>{15}</div>
                                    <div>{16}</div>
                                 </div>
                              </div>
                              </div>
                           </body>
                        </html>

                  ", transferReportStyle, Convert.ToDateTime(DateTime.Now).ToString("dd MMMM, yyyy"),
             obj?.EmployeeName, obj?.FromDesignation, obj?.FromDepartment, obj?.EmployeeName, obj?.ToBusinessUnit, obj?.EmployeeName,
             obj?.ToDesignation, obj?.PresenetSalary, obj?.JoiningDate, obj?.FromBusinessUnit, obj?.ToBusinessUnit,
             obj?.EffectiveDate, obj?.Approvarname, obj?.ApprovarDesignation, obj?.ApprovarBusinessUnit);
            return sb.ToString();
        }

        public async Task<string> PromotionLetterReport(PromotionReportViewModel obj)
        {
            var sb = new StringBuilder();
            string promotionLetterStyle = _hostingEnvironment.ContentRootPath + "/wwwroot/PromotionLetter.css";

            //string? inWord = await AmountInWords.ConvertToWords(obj?.TotalCreditAmount.ToString());

            sb.AppendFormat(@"<!DOCTYPE html>
                                  <html lang='en'>
                                      <head>
                                      <meta charset='UTF-8' />
                                      <meta http-equiv='X-UA-Compatible' content='IE=edge' />
                                      <meta name='viewport' content='width=device-width, initial-scale=1.0' />
                                      <link rel='stylesheet' href='{0}' />
                                      <title>Promotion Letter</title>
                                      </head>
                                      <body>
                                      <div class='promotion'>
                                          <p style='width:100%; height:230px'></p>
                                          <div>
                                          <div>Date: {1}</div>
                                          <div>To,</div>
                                          <div class='font-bold'>{2}</div>
                                          <div>{3}, {4}</div>
                                          <div>Business Unit: {5}</div>
                                          </div>
                                          <div>
                                          <br />
                                          <p class='font-bold center-text'>Letter of Promotion</p>
                                          <p>
                                              We take this opportunity to let you know that your performance during
                                              the last lane year was good. We are pleased to inform you that you
                                              have been promoted from {6} to {7} in {8}
                                              effective on {9}
                                          </p>
                                          <p>
                                              With this promotion, you’ll be assigned new roles and
                                              responsibilities, which you need to adhere to. I hope you continue to
                                              work with the same dedication in your new position in the future.
                                          </p>
                                          <p>
                                              The year ahead is going to pose several challenges. We demand
                                              exceptional performance from you. We are confident that you will rise
                                              to this challenge and pool your energies, competencies, and commitment
                                              to achieve further heights.
                                          </p>
                                          </div>

                                          <div>
                                          <p>We congratulate you on your achievement.</p>
                                          <p>Behalf of the Management</p>
                                          <div class='signit'>
                                          <div class='font-bold'>{10}</div>
                                          ", promotionLetterStyle, Convert.ToDateTime(DateTime.Now).ToString("dd mm, yyyy"), obj.EmployeeName,
                            obj.FromDesignation, obj.FromDepartment, obj.FromBusinessUnit, obj.FromDesignation, obj.ToDesignation, obj.ToBusinessUnit,
                            Convert.ToDateTime(obj.EffectiveDate).ToString("MMMM dd, yyyy"),
                            obj.ApproverName, obj.ApproverDesignation, obj.ApproverDepartment, obj.ApproverBusinessUnit);

            if ((obj.ApproverDesignation == null || obj.ApproverDesignation == "") && (obj.ApproverDepartment != null || obj.ApproverDepartment != ""))
            {
                sb.AppendFormat(@"<div>{0}</div>
                         <div>{1}</div>
                       </div>
                     </div>
                   </div>
                 </body>
               </html>", obj?.ApproverDepartment,
               obj?.ApproverBusinessUnit);
            }
            else if ((obj.ApproverDepartment == null || obj.ApproverDepartment == "") && (obj.ApproverDesignation != null || obj.ApproverDesignation != ""))
            {
                sb.AppendFormat(@"<div>{0}</div>
                         <div>{1}</div>
                       </div>
                     </div>
                   </div>
                 </body>
               </html>", obj?.ApproverDesignation,
               obj?.ApproverBusinessUnit);
            }
            else
            {
                sb.AppendFormat(@"<div>{0}</div>
                       </div>
                     </div>
                   </div>
                 </body>
               </html>", obj?.ApproverBusinessUnit);
            }

            return sb.ToString();
        }

        public async Task<string> BankAdviceReportForIBBL(BankAdviceReportForIBBLViewModel obj)
        {
            var sb = new StringBuilder();

            string cmyLogoUrl = _context.GlobalFileUrls.Where(x => x.IsActive == true && x.IntDocumentId == obj.intLogoUrlId).Select(s => s.StrFileServerId).FirstOrDefault();

            if (!string.IsNullOrEmpty(cmyLogoUrl))
            {
                var webclient = new WebClient();
                byte[] img = webclient.DownloadData($"{documentUrl + obj.intLogoUrlId}");
                Stream image = new MemoryStream(img);
                companyLogoUrl = Convert.ToBase64String(img);
            }
            string ibblReportAll = _hostingEnvironment.ContentRootPath + "/wwwroot/ibbl.styles.css";

            sb.AppendFormat(@"<!DOCTYPE html>
                  <html lang='en'>
                    <head>
                      <meta charset='UTF-8' />
                      <meta http-equiv='X-UA-Compatible' content='IE=edge' />
                      <meta name='viewport' content='width=device-width, initial-scale=1.0' />
                      <link rel='stylesheet' href='{0}' type='text/css' />
                      <title>Payment Information</title>
                    </head>
                    <body>
                      <div class='container'>
                        <p style='width:100%;height:100px'></p>

                        <div class='header'>
                            <div class='logo-container'>
                                <img alt='ibos-logo' src='data:image/png;base64, {8}' class='logo' />
                            </div>
                        </div>

                        <section class='info-header'>
                          <div class='left'>
                            <h3>To,</h3>
                            <h3>The Manager</h3>
                            <p>{3}</p>
                            <p>{4}</p>
                          </div>
                          <div class='right'>
                            <h3>Date : {6}</h3>
                          </div>
                        </section>
                        <section class='sub-headline'>
                          <h3>Subject : Bank Advice Of &nbsp{5}</h3>
                          <h3>Dear Sir,</h3>
                          <h3>
                            <span style='font-weight:400'> We do hereby requesting you to make payment by transferring the amount
                            to the respective account holder as shown below in detailed by
                            debiting our </span> CD Account No. {7}
                          </h3>
                        </section>
                        <section class='table-container'>
                          <h3 style='margin-bottom:5px'>Detailed particulars of each account holder :</h3>
                          <table class='table' style='border: 1px solid rgb(167, 167, 167); border-collapse: collapse;'>
                            <thead>
                             
                            ", ibblReportAll,
                                    obj?.BusinessUnit,
                                    obj?.CompanyAddress,
                                    obj?.CompanyBankName,
                                    obj?.CompanyBranchName,
                                    obj?.ReportGenerateDate.Value.ToString("MMMM-yyyy"),
                                    DateTime.Now.ToString("dd-MMMM-yyyy"),
                                    obj?.CompanyAccountNo, companyLogoUrl);

            long sl = 1;

           
            string? inWord = await AmountInWords.ConvertToWords(obj?.Data.Sum(x => x.NetSalary).ToString()) + " only"; ;
            if (obj.Data.Count > 0)
            {
                foreach(var i in obj.BankAdviceVM)
                {
                    var filteredData= obj.Data.Where(x => x.SoleDepo == i.SoleDepoName && x.Wing == i.WingName).ToList();

                    sb.AppendFormat(@"

                                <tr style='background-color: #ffffe4'>
                                    <th colspan='4'  style='border: 1px solid rgb(167, 167, 167); font-size:20px; border-collapse: collapse;  padding: 5px; text-align:left;' >Sole Depo: {0}</th>
                                    <th colspan='4'  style='border: 1px solid rgb(167, 167, 167); font-size:20px; border-collapse: collapse;  padding: 5px; text-align:center;' >Wing: {1}</th>
                                    <th colspan='4'  style='border: 1px solid rgb(167, 167, 167); font-size:20px; border-collapse: collapse;  padding: 5px; text-align:right;' >Month: {2}</th>
                                 </tr>

                             <tr>
                                <th style=' border: 1px solid rgb(167, 167, 167); border-collapse: collapse;  padding: 5px;text-align:center' >SN</th>
                                <th style='border: 1px solid rgb(167, 167, 167); border-collapse: collapse;  padding: 5px;text-align:center' >Employee ID</th>
                                <th style='border: 1px solid rgb(167, 167, 167); border-collapse: collapse;  padding: 5px;text-align:center' >Employees' Full Name</th>
                                <th style='border: 1px solid rgb(167, 167, 167); border-collapse: collapse;  padding: 5px;text-align:center' >Designation</th>
                                <th style='border: 1px solid rgb(167, 167, 167); border-collapse: collapse;  padding: 5px;text-align:center' >Territory</th>
                                <th style='border: 1px solid rgb(167, 167, 167); border-collapse: collapse;  padding: 5px;text-align:center' >Area</th>
                                <th style='border: 1px solid rgb(167, 167, 167); border-collapse: collapse;  padding: 5px;text-align:center' >Region</th>
                                <th style='border: 1px solid rgb(167, 167, 167); border-collapse: collapse;  padding: 5px;text-align:center' >Mobile Number</th>
                                <th style='border: 1px solid rgb(167, 167, 167); border-collapse: collapse;  padding: 5px;text-align:center' >Bank Account Number</th>
                                <th style='border: 1px solid rgb(167, 167, 167); border-collapse: collapse;  padding: 5px;text-align:center' >Net Salary</th>
                                <th style='border: 1px solid rgb(167, 167, 167); border-collapse: collapse;  padding: 5px;text-align:center' >Remarks</th>                           
                              </tr>



                                </thead>
                            <tbody>",  i.SoleDepoName, i.WingName, obj?.ReportGenerateDate.Value.ToString("MMMM-yyyy"));

                    foreach (var item in filteredData)
                    {
                        sb.AppendFormat(@"<tr>
                                    <td style='border: 1px solid rgb(167, 167, 167); border-collapse: collapse;  padding: 5px;text-align:center' >{0}</td>
                                    <td style='border: 1px solid rgb(167, 167, 167); border-collapse: collapse;  padding: 5px;'>{1}</td>
                                    <td style='border: 1px solid rgb(167, 167, 167); border-collapse: collapse;  padding: 5px;'>{2}</td>
                                    <td style='border: 1px solid rgb(167, 167, 167); border-collapse: collapse;  padding: 5px; text-align: center;'>{3}</td>
                                    <td style='border: 1px solid rgb(167, 167, 167); border-collapse: collapse;  padding: 5px;'>{4}</td>
                                    <td style='border: 1px solid rgb(167, 167, 167); border-collapse: collapse;  padding: 5px;'>{5}</td>
                                    <td style='border: 1px solid rgb(167, 167, 167); border-collapse: collapse;  padding: 5px;text-align:center'>{6}</td>
                                    <td style='border: 1px solid rgb(167, 167, 167); border-collapse: collapse;  padding: 5px;'>{7}</td>
                                    <td style='border: 1px solid rgb(167, 167, 167); border-collapse: collapse;  padding: 5px;'>{8}</td>
                                    <td style='border: 1px solid rgb(167, 167, 167); border-collapse: collapse;  padding: 5px; text-align: center;'>{9}</td>
                                    <td style='border: 1px solid rgb(167, 167, 167); border-collapse: collapse;  padding: 5px;'>{10}</td>
                                    
                                 </tr>", sl, item?.EmployeeCode, item?.EmployeeName, item?.Designation, item?.Territory, item?.Area, item?.Region, 
                                 item?.MobileNumber, item?.AccountNo, Math.Round((decimal)item?.NetSalary, 0), item?.Remarks);
                        sl++;
                    }
                }            
            }
         
            sb.AppendFormat(@"<tr>
                             <td style=' border: 1px solid rgb(167, 167, 167); border-collapse: collapse; text-align:center; font-weight: bold;' colspan='9'>
                               <h4 style='padding:2px 0;margin:0'>Total</h4>
                             </td>
                             <td style='border: 1px solid rgb(167, 167, 167); border-collapse: collapse;text-align:right;'><span style='margin-right:2px'>{0}</span></td>
                              <td></td>
                              <td></td>
                            </tr>
                         </tbody>
                       </table>
                     </section>
                     <section style='margin-top:5px'>
                       <span>In Word :<h3 style='display:inline'>
                          {1}
                       </h3></span>

                       <h2  style='margin-top:50px'>{2}</h2>
                     </section>
                     <section class='signature-container'>
                        <div class='signature-holder'>
                           <h4 style='margin-top:100px'>Authorize Signature</h4>
                        </div>
                     </section>
                     </div>
                  </body>
               </html>", Math.Round((decimal)obj?.Data.Sum(x => x.NetSalary), 0), inWord, obj?.BusinessUnit);
            return sb.ToString();
        }

        public async Task<string> BankAdviceReportForBEFTN(BankAdviceReportForBEFTNViewModel obj)
        {
            var sb = new StringBuilder();
            string cmyLogoUrl = _context.GlobalFileUrls.Where(x => x.IsActive == true && x.IntDocumentId == obj.intLogoUrlId).Select(s => s.StrFileServerId).FirstOrDefault();

            if (!string.IsNullOrEmpty(cmyLogoUrl))
            {
                var webclient = new WebClient();
                byte[] img = webclient.DownloadData($"{documentUrl + obj.intLogoUrlId}");
                Stream image = new MemoryStream(img);
                companyLogoUrl = Convert.ToBase64String(img);
            }
            string beftnReportAll = _hostingEnvironment.ContentRootPath + "/wwwroot/beftn.styles.css";

            sb.AppendFormat(@"<!DOCTYPE html>
            <html lang='en'>
              <head>
                <meta charset='UTF-8'/>
                <meta http-equiv='X-UA-Compatible' content='IE=edge' />
                <meta name='viewport' content='width=device-width, initial-scale=1.0' />
                <link rel='stylesheet' type='text/css' href='{0}' />
                <title>BEFTN Form</title>
              </head>
              <body>
                <div class='container'>
                  <div class='header'>
                    <div class='logo-container'>
                      <img alt='ibos-logo' src='data:image/png;base64, {7}' class='logo' />
                    </div>
                    <div class='title-container'>
                      <h1>{1}</h1>
                      <h3>
                        {2}.
                      </h3>
                    </div>
                  </div>
                  <section class='info-header'>
                    <div class='left'>
                      <h3>To</h3>
                      <h3>The Manager</h3>
                      <p>{3}</p>
                      <p>{4}</p>
                    </div>
                    <div class='right'>
                      <h3>Date : {5}</h3>
                    </div>
                  </section>
                  <section class='sub-headline'>
                    <h3>Subject : Payment Instruction by BEFTN.</h3>
                    <h3>Dear Sir</h3>
                    <h3>
                      We do hereby requesting you to make payment by transferring the amount
                      to the respective Account Holder as shown below in detailed by
                      debiting our CD Account No. {6}
                    </h3>
                  </section>
                  <section class='table-container'>
                    <h3>Detailed particulars of each account holder :</h3> <br>
                    <table class='table'>
                      <thead> ", beftnReportAll,
                                    obj?.BusinessUnit,
                                    obj?.CompanyAddress,
                                    obj?.CompanyBankName,
                                    obj?.CompanyBranchName,
                                    DateTime.Now.ToString("dd-MMMM-yyyy"), obj?.CompanyAccountNo, companyLogoUrl);

            long sl = 1;

            foreach (var i in obj.BankAdviceVM)
            {
                var filteredData = obj.Data.Where(x => x.SoleDepo == i.SoleDepoName && x.Wing == i.WingName).ToList();

                sb.AppendFormat(@"<tr style='background-color: #ffffe4'>
                                    <td colspan='4'  style='border: 1px solid rgb(167, 167, 167); font-size:20px; border-collapse: collapse;  padding: 5px; text-align:left;'  >Sole Depo: {0}</td>
                                    <td colspan='4'  style='border: 1px solid rgb(167, 167, 167); font-size:20px; border-collapse: collapse;  padding: 5px; text-align:center;' >Wing: {1}</td>
                                    <td colspan='4'  style='border: 1px solid rgb(167, 167, 167); font-size:20px; border-collapse: collapse;  padding: 5px; text-align:right;' >Month: {2}</td>

                                 </tr>
                        <tr>
                          <th style=' border: 1px solid rgb(167, 167, 167); border-collapse: collapse;  padding: 5px;text-align:center' >SN</th>
                          <th style=' border: 1px solid rgb(167, 167, 167); border-collapse: collapse;  padding: 5px;text-align:center' >Employee ID</th>
                          <th style=' border: 1px solid rgb(167, 167, 167); border-collapse: collapse;  padding: 5px;text-align:center'>Employees' Full Name</th>
                          <th style=' border: 1px solid rgb(167, 167, 167); border-collapse: collapse;  padding: 5px;text-align:center' >Designation</th>
                          <th style=' border: 1px solid rgb(167, 167, 167); border-collapse: collapse;  padding: 5px;text-align:center' >Territory</th>
                          <th style=' border: 1px solid rgb(167, 167, 167); border-collapse: collapse;  padding: 5px;text-align:center' >Area</th>
                          <th style=' border: 1px solid rgb(167, 167, 167); border-collapse: collapse;  padding: 5px;text-align:center' >Region</th>
                          <th style=' border: 1px solid rgb(167, 167, 167); border-collapse: collapse;  padding: 5px;text-align:center' >Mobile Number</th>
                          <th style=' border: 1px solid rgb(167, 167, 167); border-collapse: collapse;  padding: 5px;text-align:center' >Bank Account Number</th>
                          <th style=' border: 1px solid rgb(167, 167, 167); border-collapse: collapse;  padding: 5px;text-align:center' >Net Salary</th>
                          <th style=' border: 1px solid rgb(167, 167, 167); border-collapse: collapse;  padding: 5px;text-align:center' >Remarks</th>
                        </tr> 
                       </thead>
                      <tbody>", i.SoleDepoName, i.WingName, obj?.ReportGenerateDate.Value.ToString("MMMM-yyyy"));

                foreach (var item in filteredData)
                {
                    sb.AppendFormat(@"<tr>
                                <td  style='border: 1px solid rgb(167, 167, 167); border-collapse: collapse;  padding: 5px;text-align:center'  >{0}</td>
                                <td  style='border: 1px solid rgb(167, 167, 167); border-collapse: collapse;  padding: 5px;text-align:center'  >{1}</td>
                                <td  style='border: 1px solid rgb(167, 167, 167); border-collapse: collapse;  padding: 5px;text-align:center'  >{2}</td>
                                <td  style='border: 1px solid rgb(167, 167, 167); border-collapse: collapse;  padding: 5px;text-align:center'  >{3}</td>
                                <td  style='border: 1px solid rgb(167, 167, 167); border-collapse: collapse;  padding: 5px;text-align:center'  >{4}</td>
                                <td  style='border: 1px solid rgb(167, 167, 167); border-collapse: collapse;  padding: 5px;text-align:center'  >{5}</td>
                                <td  style='border: 1px solid rgb(167, 167, 167); border-collapse: collapse;  padding: 5px;text-align:center'  >{6}</td>
                                <td  style='border: 1px solid rgb(167, 167, 167); border-collapse: collapse;  padding: 5px;text-align:center'  >{7}</td>
                                <td  style='border: 1px solid rgb(167, 167, 167); border-collapse: collapse;  padding: 5px;text-align:center'  >{8}</td>
                                <td  style='border: 1px solid rgb(167, 167, 167); border-collapse: collapse;  padding: 5px;text-align:center'  >{9}</td>
                                <td  style='border: 1px solid rgb(167, 167, 167); border-collapse: collapse;  padding: 5px;text-align:center' >{10}</td>
                              </tr>", sl, item?.EmployeeCode, item?.EmployeeName, item?.Designation, item?.Territory, item?.Area, item?.Region,
                             item?.MobileNumber, item?.AccountNo, Math.Round((decimal)item?.NetSalary, 0), item?.Remarks);
                    sl++;
                }

            }

            string? inWord = await AmountInWords.ConvertToWords(obj?.Data.Sum(x => x.NetSalary).ToString()) + " only"; ;
            sb.AppendFormat(@"<tr>
                              <td style=' border: 1px solid rgb(167, 167, 167); border-collapse: collapse; text-align:center; font-weight: bold;' colspan='9' ><h4>Total</h4></td>
                              <td style='border: 1px solid rgb(167, 167, 167); border-collapse: collapse;text-align:center;'  >{0}</td>
                              <td style='border: 1px solid rgb(167, 167, 167); '  ></td>
                            
                           </tr>
                           </tbody>
                        </table>
                     </section>
                     <section>
                        <h3>
                           In Word : {1}
                        </h3>
                        <h2>{2}</h2>
                     </section>
                     <section class='signature-container'>
                        <div class='signature-holder'>
                           <h4>Authorize Signature</h4>
                        </div>
                     </section>
                     </div>
                  </body>
               </html>", Math.Round((decimal)obj?.Data.Sum(x => x.NetSalary), 0) , inWord, obj?.BusinessUnit);

            return sb.ToString();
        }

        public async Task<string> DailyAttendanceReportPDF(EmployeeDaylyAttendanceReportLanding obj)
        {
            var sb = new StringBuilder();

            string cmyLogoUrl = _context.GlobalFileUrls.Where(x => x.IsActive == true && x.IntDocumentId == obj.CompanyLogoUrlId).Select(s => s.StrFileServerId).FirstOrDefault();

            //if (!string.IsNullOrEmpty(cmyLogoUrl))
            //{
            //    var webclient = new WebClient();
            //    byte[] img = webclient.DownloadData($"{documentUrl + obj.CompanyLogoUrlId}");
            //    Stream image = new MemoryStream(img);
            //    companyLogoUrl = Convert.ToBase64String(img);
            //}
            
                sb.AppendFormat(@"<!DOCTYPE html>
                                <html lang='en'>
                                  <head>
                                    <meta charset='UTF-8' />
                                    <meta http-equiv='X-UA-Compatible' content='IE=edge' />
                                    <meta name='viewport' content='width=device-width, initial-scale=1.0' />
                                    <title>Daily Attendance Report</title>
                                    <style></style>
                                  </head>
                                  <body>
                                    <div style='padding: 0px; position: relative; min-height: 100vh'>
                                      <!-- heading top -->
                                      <div style='position: relative; padding: 10px 0'>
                                        <div style='width: 100px; position: absolute; top: 20%'>
                                          <img
                                            src='data:image/png;base64, {0}'
                                            alt='company_logo' style='width: 100%'
                                          />
                                        </div>
                                        <div
                                          style='
                                            display: block;
                                            width: 100%;
                                            text-align: center;
                                            padding-bottom: 20px;
                                            border-bottom: 1px solid rgb(188, 188, 188);'>
                                          <p style='font-size: 20px; font-weight: bold'>{1}</p>
                                          <p style='max-width: 70%; margin: auto; color: rgb(86, 86, 86)'>
                                            {2}
                                          </p>
                                        </div>
                                      </div>
                                      <div style='width: 100%'>
                                       <div>
                                          <p style='text-align: center; font-size: 20px; font-weight: bold'>
                                            Attendance Report ({3})
                                          </p>
                                          <div style='margin-bottom:5px'>
                                            <p style='font-size: 18px; font-weight: 500'>General Employees</p>
                                            <div>", (companyLogoUrl == null? "": companyLogoUrl), obj.BusinessUnitName, obj.CompanyAddress, obj.AttendanceDate);//.Value.ToString("dd MMMM yyyy")


            if (obj.BusinessUnitId > 0)
            {
                sb.AppendFormat(@"<p style='font-weight: 500; font-size: 16px;display:inline'>
                                      <span style='font-weight: 500'>Business Unit:</span> {0} &nbsp&nbsp&nbsp
                                    </p>", obj.BusinessUnitName);
            }
            if (obj.AttendanceDate != null)
            {
                sb.AppendFormat(@"<p style='font-weight: 500; font-size: 16px;display:inline'>
                                      <span style='font-weight: 500'>Date:</span> {0} &nbsp&nbsp&nbsp
                                    </p>", obj.AttendanceDate  );
                //obj.AttendanceDate.Value.ToString("dd MMMM yyyy")
                
            }
            if (obj.WorkplaceGroup != "" && obj.WorkplaceGroup != null)
            {
                sb.AppendFormat(@"<p style='font-weight: 500; font-size: 16px;display:inline'>
                                      <span style='font-weight: 500'>Workplace Group:</span> {0} &nbsp&nbsp&nbsp
                                    </p>", obj.WorkplaceGroup);
            }
            if (obj.Workplace != "" && obj.Workplace != null)
            {
                sb.AppendFormat(@"<p style='font-weight: 500; font-size: 16px;display:inline'>
                                      <span style='font-weight: 500'>Workplace:</span> {0} &nbsp&nbsp&nbsp
                                    </p>", obj.Workplace);
            }

            sb.AppendFormat(@"</div>
                              </div>
                            </div>
                            <!-- attendance summary -->
                            <div>
                              <table
                                style='
                                  border: 1px solid rgb(167, 167, 167);
                                  border-collapse: collapse;
                                  padding: 5px;
                                  width: 100%;'>
                                <tbody>
                                  <tr style='width: 100%'>
                                    <td
                                      style='
                                        border: 1px solid rgb(167, 167, 167);
                                        border-collapse: collapse;
                                        padding: 5px;
                                        font-size: 12px;'>
                                      Total Employee
                                    </td>
                                    <td
                                      style='
                                        border: 1px solid rgb(167, 167, 167);
                                        border-collapse: collapse;
                                        padding: 5px;
                                        font-size: 12px;'>
                                      {0}
                                    </td>
                                    <td
                                      style='
                                        border: 1px solid rgb(167, 167, 167);
                                        border-collapse: collapse;
                                        padding: 5px;
                                        font-size: 12px;'>
                                      Present
                                    </td>
                                    <td
                                      style='
                                        border: 1px solid rgb(167, 167, 167);
                                        border-collapse: collapse;
                                        padding: 5px;
                                        font-size: 12px;'>
                                      {1} &nbsp; [&lsquo;{9}&rsquo;]
                                    </td>
                                    <td
                                      style='
                                        border: 1px solid rgb(167, 167, 167);
                                        border-collapse: collapse;
                                        padding: 5px;
                                        font-size: 12px;'>
                                      Absent
                                    </td>
                                    <td
                                      style='
                                        border: 1px solid rgb(167, 167, 167);
                                        border-collapse: collapse;
                                        padding: 5px;
                                        font-size: 12px;'>
                                      {2}
                                    </td>
                                  </tr>
                                  <tr style='width: 100%'>
                                    <td
                                      style='
                                        border: 1px solid rgb(167, 167, 167);
                                        border-collapse: collapse;
                                        padding: 5px;
                                        font-size: 12px;'>
                                      Delay
                                    </td>
                                    <td
                                      style='
                                        border: 1px solid rgb(167, 167, 167);
                                        border-collapse: collapse;
                                        padding: 5px;
                                        font-size: 12px;'>
                                      {3}
                                    </td>
                                    <td
                                      style='
                                        border: 1px solid rgb(167, 167, 167);
                                        border-collapse: collapse;
                                        padding: 5px;
                                        font-size: 12px;'>
                                      Extra Delay
                                    </td>
                                    <td
                                      style='
                                        border: 1px solid rgb(167, 167, 167);
                                        border-collapse: collapse;
                                        padding: 5px;
                                        font-size: 12px;'>
                                      {4}
                                    </td>
                                    <td
                                      style='
                                        border: 1px solid rgb(167, 167, 167);
                                        border-collapse: collapse;
                                        padding: 5px;
                                        font-size: 12px;'>
                                      Leave
                                    </td>
                                    <td
                                      style='
                                        border: 1px solid rgb(167, 167, 167);
                                        border-collapse: collapse;
                                        padding: 5px;
                                        font-size: 12px;'>
                                      {5}
                                    </td>
                                  </tr>
                                  <tr style='width: 100%'>
                                    <td
                                      style='
                                        border: 1px solid rgb(167, 167, 167);
                                        border-collapse: collapse;
                                        padding: 5px;
                                        font-size: 12px;'>
                                      Movement
                                    </td>
                                    <td
                                      style='
                                        border: 1px solid rgb(167, 167, 167);
                                        border-collapse: collapse;
                                        padding: 5px;
                                        font-size: 12px;'>
                                      {6}
                                    </td>
                                    <td
                                      style='
                                        border: 1px solid rgb(167, 167, 167);
                                        border-collapse: collapse;
                                        padding: 5px;
                                        font-size: 12px;'>
                                      Weekend
                                    </td>
                                    <td
                                      style='
                                        border: 1px solid rgb(167, 167, 167);
                                        border-collapse: collapse;
                                        padding: 5px;
                                        font-size: 12px;'>
                                      {7}
                                    </td>
                                    <td
                                      style='
                                        border: 1px solid rgb(167, 167, 167);
                                        border-collapse: collapse;
                                        padding: 5px;
                                        font-size: 12px;'>
                                      Holiday
                                    </td>
                                    <td
                                      style='
                                        border: 1px solid rgb(167, 167, 167);
                                        border-collapse: collapse;
                                        padding: 5px;
                                        font-size: 12px;'>
                                      {8}
                                    </td>
                                  </tr>
                                </tbody>
                              </table>
                            </div>
                            <!-- attendance table -->
                            <div style='margin-top: 20px'>
                              <table
                                style='
                                  border: 1px solid rgb(167, 167, 167);
                                  border-collapse: collapse;
                                  padding: 5px;
                                  width: 100%;
                                  margin-bottom: 10px'>
                                <thead>
                                  <tr>
                                    <th
                                      style='
                                        border: 1px solid rgb(167, 167, 167);
                                        border-collapse: collapse;
                                        padding: 5px;
                                        font-size: 12px;'>
                                      SL
                                    </th>
                                    <th
                                      style='
                                        border: 1px solid rgb(167, 167, 167);
                                        border-collapse: collapse;
                                        padding: 5px;
                                        text-align: left;
                                        font-size: 12px;'>
                                      Employee Name
                                    </th>
                                    <th
                                      style='
                                        border: 1px solid rgb(167, 167, 167);
                                        border-collapse: collapse;
                                        padding: 5px;
                                        text-align: center;
                                        font-size: 12px;'>
                                      Employee Code
                                    </th>
                                    <th
                                      style='
                                        border: 1px solid rgb(167, 167, 167);
                                        border-collapse: collapse;
                                        padding: 5px;
                                        text-align: left;
                                        font-size: 12px;'>
                                      Designation
                                    </th>
                                    <th
                                      style='
                                        border: 1px solid rgb(167, 167, 167);
                                        border-collapse: collapse;
                                        padding: 5px;
                                        text-align: left;
                                        font-size: 12px;'>
                                      Employment Type
                                    </th>
                                    <th
                                      style='
                                        border: 1px solid rgb(167, 167, 167);
                                        border-collapse: collapse;
                                        padding: 5px;
                                        text-align: left;
                                        font-size: 12px;'>
                                      Calender Name
                                    </th>
                                    <th
                                      style='
                                        border: 1px solid rgb(167, 167, 167);
                                        border-collapse: collapse;
                                        padding: 5px;
                                        text-align: center;
                                        font-size: 12px;'>
                                      In Time
                                    </th>
                                    <th
                                      style='
                                        border: 1px solid rgb(167, 167, 167);
                                        border-collapse: collapse;
                                        padding: 5px;
                                        text-align: center;
                                        font-size: 12px;'>
                                      Out Time
                                    </th>
                                    <th
                                      style='
                                        border: 1px solid rgb(167, 167, 167);
                                        border-collapse: collapse;
                                        padding: 5px;
                                        text-align: center;
                                        font-size: 12px;'>
                                      Duration
                                    </th>
                                    <th
                                      style='
                                        border: 1px solid rgb(167, 167, 167);
                                        border-collapse: collapse;
                                        padding: 5px;
                                        text-align: center;
                                        font-size: 12px;'>
                                      Actual Status
                                    </th>
                                    <th
                                      style='
                                        border: 1px solid rgb(167, 167, 167);
                                        border-collapse: collapse;
                                        padding: 5px;
                                        text-align: center;
                                        font-size: 12px;'>
                                      Manual Status
                                    </th>
                                    <th
                                      style='
                                        border: 1px solid rgb(167, 167, 167);
                                        border-collapse: collapse;
                                        padding: 5px;
                                        text-align: left;
                                        font-size: 12px;'>
                                      Address
                                    </th>
                                    <th
                                      style='
                                        border: 1px solid rgb(167, 167, 167);
                                        border-collapse: collapse;
                                        padding: 5px;
                                        text-align: left;
                                        font-size: 12px;'>
                                      Remarks
                                    </th>
                                  </tr>
                                </thead>
                                <tbody>", obj.TotalEmployee, obj.PresentCount, obj.AbsentCount, obj.LateCount, 0, obj.LeaveCount, obj.MovementCount, obj.WeekendCount, obj.HolidayCount, obj.ManualPresentCount);

            int sl = 1;

            foreach( var dept in obj.departmentVM )
            {
                sb.AppendFormat(@"<tr>
                                        <td colspan='13' style='font-weight: 700;padding:5px; font-size:16px; background-color: rgb(239, 236, 236);'>Department: " + dept.deptName + @"</td>                                       

                                   </tr>");

                var deptWiseDetails = (from sal in obj.Data
                                       where sal.DepartmentId == dept.deptId
                                       orderby sal.DepartmentId ascending
                                       select sal).ToList();

                foreach (var item in deptWiseDetails)
                {


                    sb.AppendFormat(@"<tr>
                <td
                  style='
                    border: 1px solid rgb(167, 167, 167);
                    border-collapse: collapse;
                    padding: 5px;
                    text-align: center;
                    font-size: 12px;'>
                  {0}
                </td>
                <td
                  style='
                    border: 1px solid rgb(167, 167, 167);
                    border-collapse: collapse;
                    padding: 5px;
                    font-size: 12px;'>
                  {1}
                </td>
                <td
                  style='
                    border: 1px solid rgb(167, 167, 167);
                    border-collapse: collapse;
                    padding: 5px;
                    text-align: center;
                    font-size: 12px;'>
                  {2}
                </td>
                <td
                  style='
                    border: 1px solid rgb(167, 167, 167);
                    border-collapse: collapse;
                    padding: 5px;
                    font-size: 12px;'>
                  {3}
                </td>
                <td
                  style='
                    border: 1px solid rgb(167, 167, 167);
                    border-collapse: collapse;
                    padding: 5px;
                    font-size: 12px;'>
                  {4}
                </td>
                <td
                  style='
                    border: 1px solid rgb(167, 167, 167);
                    border-collapse: collapse;
                    padding: 5px;
                    text-align: center;
                    font-size: 12px;'>
                  {5}
                </td>
                <td
                  style='
                    border: 1px solid rgb(167, 167, 167);
                    border-collapse: collapse;
                    padding: 5px;
                    text-align: center;
                    font-size: 12px;'>
                  {6}
                </td>
                <td
                  style='
                    border: 1px solid rgb(167, 167, 167);
                    border-collapse: collapse;
                    padding: 5px;
                    text-align: center;
                    font-size: 12px;'>
                  {7}
                </td>
                <td
                  style='
                    border: 1px solid rgb(167, 167, 167);
                    border-collapse: collapse;
                    padding: 5px;
                    text-align: center;
                    font-size: 12px;'>
                  {8}
                </td>
                <td
                  style='
                    border: 1px solid rgb(167, 167, 167);
                    border-collapse: collapse;
                    padding: 5px;
                    text-align: center;
                    font-size: 12px;'>
                  {9}
                </td>
                <td
                  style='
                    border: 1px solid rgb(167, 167, 167);
                    border-collapse: collapse;
                    padding: 5px;
                    font-size: 12px;'>
                  {10}
                </td>
                <td
                  style='
                    border: 1px solid rgb(167, 167, 167);
                    border-collapse: collapse;
                    padding: 5px;
                    font-size: 12px;'>
                  {11}
                </td>
                <td style='
                    border: 1px solid rgb(167, 167, 167);
                    border-collapse: collapse;
                    padding: 5px;
                    font-size: 12px;'>
                  {12}
                </td>
              </tr>", sl, item.EmployeeName, item.EmployeeCode, item.Designation, item.EmploymentType, item.CalendarName, item.InTime, item.OutTime, item.DutyHours, item.ActualStatus, item.ManualStatus, item.Location, item.Remarks);

                    sl = sl + 1;
                }
            }

           

            sb.AppendFormat(@"</tbody>
                            </table>
                        </div>
                        </div>
                       
                    </div>
                    </body>
                </html>");

            return sb.ToString();
        }

        public async Task<string> SalaryTaxCertificateReportPDF(SalaryTaxCertificateViewModel obj)
        {
            var sb = new StringBuilder();

            decimal? NetSalary = Math.Round((decimal)(obj.payrollElementVMs.Select(x => x.NumAmount).Sum()), 2);

            sb.AppendFormat(@"<!DOCTYPE html>
                            <html lang='en'>
                                <head>
                                <meta charset='UTF-8' />
                                <meta http-equiv='X-UA-Compatible' content='IE=edge' />
                                <meta name='viewport' content='width=device-width, initial-scale=1.0' />
                                <title>Salary Tax Certificate</title>
                                </head>
                                <body>
                                <div>
                                    <p style='width:100%;height:200px'></p>
                                    <h3 style='text-align: center; margin-bottom: 25px'>
                                    Salary TAX Certificate
                                    </h3>
                                    <p>Date: {0}</p>
                                    <p style='margin: 20px 0'>
                                    This is to certify that {1}, Employee ID. {2}, is working
                                    with {3}. He is a employee of the company and is serving as
                                    '{4}'. He has been paid total tk. {5} as salary and the
                                    allowances during the financial year {6} from "+obj.Fiscalyear+@"
                                    </p>
                                    <table style='width: 100%; border-collapse: collapse; border: 1px solid black;'>
                                      <tr>
                                        <!-- income head -->
                                        <td style='padding: 0px;border-collapse: collapse;'>
                                          <table style='border-collapse: collapse; width: 100%;'>
                                            <thead>
                                              <tr>
                                                <th style='
                                                border-bottom: 1px solid black;
                                                border-right: 1px solid black;
                                                border-collapse: collapse;
                                                text-align: left;
                                                padding: 5px;'>
                                                  Income Head (Tk.)
                                                </th>
                                                <th style='
                                                border-bottom: 1px solid black;
                                                border-right: 1px solid black;
                                                border-collapse: collapse;
                                                text-align: right;
                                                padding: 5px;'>
                                                  Amount
                                                </th>
                                              </tr>
                                            </thead>
                                            <tbody>", DateTime.Now.Date.ToString("dd MMMM yyyy"), obj.EmployeeName,
                                    obj.EmployeeCode, obj.BusinessUnit, obj.Designation, NetSalary,
                                    obj.FiscalyearName);

            foreach (var item in obj.payrollElementVMs)
            {
                sb.AppendFormat(@"<tr>
                                    <td style='
                                    border-bottom: 1px solid black;
                                    border-right: 1px solid black;
                                      border-collapse: collapse;
                                      text-align: left;
                                      padding: 5px;'>
                                      {0}
                                    </td>
                                    <td style='
                                    border-bottom: 1px solid black;
                                    border-right: 1px solid black;
                                      border-collapse: collapse;
                                      text-align: right;
                                      padding: 5px;'>
                                      {1}
                                    </td>
                                  </tr>", item?.PayrollElement, Math.Round((decimal)item?.NumAmount, 2));
            }
            string? inWord = await AmountInWords.ConvertToWords(obj.payrollElementVMs.Select(x => x.NumAmount).Sum().ToString()) + " only"; ;

            sb.AppendFormat(@"<tr>
                                <td style='
                                border-right: 1px solid black;
                                border-collapse: collapse;
                                text-align: left;
                                padding: 5px;'>
                                  Net Salary
                                </td>
                                <td style='
                                border-right: 1px solid black;
                                border-collapse: collapse;
                                text-align: right;
                                padding: 5px;'>
                                  {0}
                                </td>
                              </tr>
                            </tbody>
                          </table>
                        </td>
                        <!-- gap -->
                        <td style='min-width: 20px; border: 0px solid black; border-collapse: collapse;padding: 0px;'>
                        </td>
                        <!-- deduction  -->
                        <td style='padding: 0px;'>
                          <table style='border-collapse: collapse; width: 100%'>
                            <thead>
                              <tr>
                                <th style='
                                  border: 1px solid black;
                                  border-top: 0px solid black;
                                  border-collapse: collapse;
                                  text-align: left;
                                  padding: 5px;'>
                                  Deductions (Tk.)
                                </th>
                                <th style='
                                  border: 1px solid black;
                                  border-top: 0px solid black;
                                  border-right: 0px solid black;
                                  border-collapse: collapse;
                                  text-align: right;
                                  padding: 5px;'>
                                  Amount
                                </th>
                              </tr>
                            </thead>
                            <tbody>
                              <tr>
                                <td style='
                                border: 1px solid black;
                                border-top: 0px solid black;
                                border-collapse: collapse;
                                text-align: left;
                                padding: 5px;'>
                                  Income Tax (TDS to deducted)
                                </td>
                                <td style='
                                border-bottom: 1px solid black;
                                border-collapse: collapse;
                                text-align: right;
                                padding: 5px;'>
                                  {1}
                                </td>
                              </tr>", NetSalary, Math.Round((decimal)obj.TaxAmount, 2));

            for (int i = 2; i <= obj.payrollElementVMs.Count(); i++)
            {
                sb.AppendFormat(@"<tr>
                                    <td style='
                                    border: 1px solid black;
                                    border-top: 0px solid black;
                                    border-collapse: collapse;
                                    text-align: left;
                                    padding: 5px;
                                    height: 20px;'>

                                    </td>
                                    <td style='
                                    border-bottom: 1px solid black;
                                    border-collapse: collapse;
                                    text-align: right;
                                    padding: 5px;
                                    height: 20px;'>

                                    </td>
                                    </tr>");
            }

            sb.AppendFormat(@"<tr>
                                <td style='
                                border-left: 1px solid black;
                                border-right: 1px solid black;
                                border-collapse: collapse;
                                text-align: left;
                                padding: 5px;'>
                                  Total Dedcution
                                </td>
                                <td style='
                                border-collapse: collapse;
                                text-align: right;
                                padding: 5px;'>
                                  {0}
                                </td>
                              </tr>
                            </tbody>
                          </table>
                        </td>
                      </tr>
                    </table>
                    <p style='margin: 20px 0'>
                        Net Salary in Taka: <b>{1}</b>
                    </p>
                    <p style='margin: 0'>
                        Salary Certificate issued for the TAX Year: &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{2}
                    </p>
                    <p style='margin: 0'>
                        Date of Joining in the present employment: &nbsp;&nbsp;&nbsp;&nbsp;{3}
                    </p>
                    <p style='margin: 20px 0'>
                        Tk. {4} has been deducted from his salary as Source Tax and paid to
                        the Deputy Commissioner of Taxes, Circle - {5} (Companies), Zone - {6}, 
                        through NBR {7} e-Payment A-Challan No- {8},
                        Date: {9}
                    </p>
                    <h4 style='margin-top: 120px; margin-bottom: 0; padding-bottom: 0'>
                        <b>Certified by</b>
                    </h4>
                    <h4 style='margin: 0'><b></b></h4>
                    </div>
                </body>
                </html>", Math.Round((decimal)obj.TaxAmount, 2), inWord, obj?.FiscalyearName,
                (obj.ConfirmationDate != null ? obj?.ConfirmationDate.Value.Date.ToString("dd MMMM yyyy") : ""),
                Math.Round((decimal)obj.TaxAmount, 2), obj?.Circle, obj?.ZoneName, obj?.BankName, obj?.ChallanNo,
                (obj.TaxPaidDate != null ? obj?.TaxPaidDate.Value.Date.ToString("dd MMMM yyyy") : ""));




            return sb.ToString();
        }
    }
}
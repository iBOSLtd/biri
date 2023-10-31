using ClosedXML.Excel;
using HCM.Helper;
using LanguageExt.UnitsOfMeasure;
using Microsoft.AspNetCore.Mvc;
using PeopleDesk.Data.Entity;
using PeopleDesk.Models.Employee;

namespace PeopleDesk.Services.Helper
{
    public class DownloadExcel
    {
        public static async Task<IActionResult> GetDailyCafeteriaReport(List<GetDailyCafeteriaReportViewModel> dt)
        {
            int TotalRowCount = dt.Count();
            XLWorkbook xLWorkbook = new XLWorkbook();

            var worksheet = xLWorkbook.Worksheets.Add("Daily Meal");
            //Sub Title
            var subTitle = worksheet.Range(2, 1, 2, 8).SetValue($"Daily Meal Of {dt.Select(x => x.MealDate).FirstOrDefault()}");
            subTitle.Merge().Style.Font.SetBold().Font.FontSize = 16;
            subTitle.Style.Font.SetFontColor(XLColor.CoolBlack);
            subTitle.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            subTitle.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            //Table Header
            var header = worksheet.Range(4, 1, 4, 8);
            header.Style.Font.SetBold();
            header.Style.Fill.SetBackgroundColor(XLColor.CoolBlack);
            header.Style.Font.SetFontColor(XLColor.White);
            header.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            header.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            header.Style.Border.TopBorder = XLBorderStyleValues.Thin;
            header.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            header.Style.Border.RightBorder = XLBorderStyleValues.Thin;

            int hSl = 1;

            header.Cell(1, hSl++).SetValue("Sl");
            header.Cell(1, hSl++).SetValue("PeopleDesk Enroll");
            header.Cell(1, hSl++).SetValue("iBOS Enroll");
            header.Cell(1, hSl++).SetValue("Employee Code");
            header.Cell(1, hSl++).SetValue("Employee Name");
            header.Cell(1, hSl++).SetValue("Designation");
            header.Cell(1, hSl++).SetValue("Department");
            header.Cell(1, hSl++).SetValue("Meal Count");

            //Table Data
            var dataArray = worksheet.Range(5, 1, TotalRowCount + 4, 8);
            dataArray.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            dataArray.Style.Border.TopBorder = XLBorderStyleValues.Thin;
            dataArray.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            dataArray.Style.Border.RightBorder = XLBorderStyleValues.Thin;

            var rowIndex = 1;
            foreach (var row in dt)
            {
                int dSl = 1;
                dataArray.Cell(rowIndex, dSl++).SetValue(rowIndex);
                dataArray.Cell(rowIndex, dSl++).SetValue(row.EmployeeId);
                dataArray.Cell(rowIndex, dSl++).SetValue(row.Referenceid);
                dataArray.Cell(rowIndex, dSl++).SetValue(row.EmployeeCode);
                dataArray.Cell(rowIndex, dSl++).SetValue(row.EmployeeFullName);
                dataArray.Cell(rowIndex, dSl++).SetValue(row.DesignationName);
                dataArray.Cell(rowIndex, dSl++).SetValue(row.DepartmentName);
                dataArray.Cell(rowIndex, dSl++).SetValue(row.MealCount);

                rowIndex++;
            }
            worksheet.Columns().AdjustToContents();

            MemoryStream MS = new MemoryStream();
            xLWorkbook.SaveAs(MS);
            MS.Position = 0;

            return new FileStreamResult(MS, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = $"DailyMeal_{dt.Select(x => x.MealDate).FirstOrDefault()}.xlsx"
            };

        }

        public static async Task<IActionResult> MonthlyCafeteriaReport(List<GetDailyCafeteriaReportViewModel> dt)
        {
            int TotalRowCount = dt.Count();
            XLWorkbook xLWorkbook = new XLWorkbook();

            var worksheet = xLWorkbook.Worksheets.Add("Monthly Meal");
            //Sub Title
            var subTitle = worksheet.Range(2, 1, 2, 8).SetValue($"Monthly Meal Of {dt.Select(x => x.MealDate).FirstOrDefault()}");
            subTitle.Merge().Style.Font.SetBold().Font.FontSize = 16;
            subTitle.Style.Font.SetFontColor(XLColor.CoolBlack);
            subTitle.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            subTitle.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            //Table Header
            var header = worksheet.Range(4, 1, 4, 8);
            header.Style.Font.SetBold();
            header.Style.Fill.SetBackgroundColor(XLColor.CoolBlack);
            header.Style.Font.SetFontColor(XLColor.White);
            header.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            header.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            header.Style.Border.TopBorder = XLBorderStyleValues.Thin;
            header.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            header.Style.Border.RightBorder = XLBorderStyleValues.Thin;

            int hSl = 1;

            header.Cell(1, hSl++).SetValue("Sl");
            header.Cell(1, hSl++).SetValue("PeopleDesk Enroll");
            header.Cell(1, hSl++).SetValue("iBOS Enroll");
            header.Cell(1, hSl++).SetValue("Employee Code");
            header.Cell(1, hSl++).SetValue("Employee Name");
            header.Cell(1, hSl++).SetValue("Designation");
            header.Cell(1, hSl++).SetValue("Department");
            header.Cell(1, hSl++).SetValue("Meal Count");

            //Table Data
            var dataArray = worksheet.Range(5, 1, TotalRowCount + 4, 8);
            dataArray.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            dataArray.Style.Border.TopBorder = XLBorderStyleValues.Thin;
            dataArray.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            dataArray.Style.Border.RightBorder = XLBorderStyleValues.Thin;

            var rowIndex = 1;
            foreach (var row in dt)
            {
                int dSl = 1;
                dataArray.Cell(rowIndex, dSl++).SetValue(rowIndex);
                dataArray.Cell(rowIndex, dSl++).SetValue(row.EmployeeId);
                dataArray.Cell(rowIndex, dSl++).SetValue(row.Referenceid);
                dataArray.Cell(rowIndex, dSl++).SetValue(row.EmployeeCode);
                dataArray.Cell(rowIndex, dSl++).SetValue(row.EmployeeFullName);
                dataArray.Cell(rowIndex, dSl++).SetValue(row.DesignationName);
                dataArray.Cell(rowIndex, dSl++).SetValue(row.DepartmentName);
                dataArray.Cell(rowIndex, dSl++).SetValue(row.MealCount);

                rowIndex++;
            }
            worksheet.Columns().AdjustToContents();

            MemoryStream MS = new MemoryStream();
            xLWorkbook.SaveAs(MS);
            MS.Position = 0;

            return new FileStreamResult(MS, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = $"Monthly_{dt.Select(x => x.MealDate).FirstOrDefault()}.xlsx"
            };

        }
        public static async Task<XLWorkbook> SalaryDetailsExcellReport(SalaryMasyterVM obj, MasterBusinessUnit businessUnit)
        {
            int TotalRowCount = obj.salaryDetailsReportVM.Count();
            XLWorkbook xLWorkbook = new XLWorkbook();
            var pres = obj.salaryDetailsReportVM.Select(x => x.intTotalWorkingDays).FirstOrDefault();
            var countList = obj.salaryDetailsReportVM.Count();

            var worksheet = xLWorkbook.Worksheets.Add("Salary Report");
            //Sub Title
            var buName = worksheet.Range(1, 1, 1, 30).SetValue($""+obj.salaryDetailsReportVM.Select(x=> x.strBusinessUnitName).FirstOrDefault()+ "");
            buName.Merge().Style.Font.SetBold().Font.FontSize = 16;
            buName.Style.Font.SetFontColor(XLColor.CoolBlack);
            buName.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            buName.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            var buAdd = worksheet.Range(2, 1, 2, 30).SetValue($"" + businessUnit.StrAddress + "");
            buAdd.Merge().Style.Font.SetBold().Font.FontSize = 10;
            buAdd.Style.Font.SetFontColor(XLColor.CoolBlack);
            buAdd.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            buAdd.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            if(obj.WorkplaceGroupName == "Head Office")
            {

                var subTitle = worksheet.Range(3, 1, 3, 30).SetValue($"Salary Sheet of " + obj.WorkplaceGroupName + "");
                subTitle.Merge().Style.Font.SetBold().Font.FontSize = 12;
                subTitle.Style.Font.SetFontColor(XLColor.CoolBlack);
                subTitle.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                subTitle.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                
                var wing = worksheet.Range(4, 7, 4, 9).SetValue($"Present Month: " + pres + " Days");
                wing.Merge().Style.Font.SetBold().Font.FontSize = 10;
                wing.Style.Font.SetFontColor(XLColor.CoolBlack);
                wing.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                wing.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                var pMonth = worksheet.Range(4, 20, 4, 22).SetValue($"Month: " + obj.Month + "/" + obj.Year + "");
                pMonth.Merge().Style.Font.SetBold().Font.FontSize = 10;
                pMonth.Style.Font.SetFontColor(XLColor.CoolBlack);
                pMonth.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                pMonth.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            }
            else if(obj.WorkplaceGroupName == "Marketing")
            {

                var subTitle = worksheet.Range(3, 1, 3, 30).SetValue($"Salary Sheet of " + obj.WorkplaceGroupName + " (For " + obj.SoleDipoName + ")");
                subTitle.Merge().Style.Font.SetBold().Font.FontSize = 12;
                subTitle.Style.Font.SetFontColor(XLColor.CoolBlack);
                subTitle.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                subTitle.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                var soleDipo = worksheet.Range(4, 1, 4, 3).SetValue($"Sole Depot: " + obj.SoleDipoName + "");
                soleDipo.Merge().Style.Font.SetBold().Font.FontSize = 10;
                soleDipo.Style.Font.SetFontColor(XLColor.CoolBlack);
                soleDipo.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                soleDipo.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                var wing = worksheet.Range(4, 7, 4, 9).SetValue($"Wing: " + obj.WingName + "");
                wing.Merge().Style.Font.SetBold().Font.FontSize = 10;
                wing.Style.Font.SetFontColor(XLColor.CoolBlack);
                wing.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                wing.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                var pMonth = worksheet.Range(4, 15, 4, 17).SetValue($"Present Month: " + pres + " Days");
                pMonth.Merge().Style.Font.SetBold().Font.FontSize = 10;
                pMonth.Style.Font.SetFontColor(XLColor.CoolBlack);
                pMonth.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                pMonth.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;


                var month = worksheet.Range(4, 28, 4, 30).SetValue($"Month: " + obj.Month + "/" + obj.Year + "");
                month.Merge().Style.Font.SetBold().Font.FontSize = 10;
                month.Style.Font.SetFontColor(XLColor.CoolBlack);
                month.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                month.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            }

            

            //Table Head
            var header = worksheet.Range(5, 1, 5, 8);
            header.Style.Font.SetBold();
            header.Style.Fill.SetBackgroundColor(XLColor.CoolBlack);
            header.Style.Font.SetFontColor(XLColor.White);
            header.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            header.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            header.Style.Border.TopBorder = XLBorderStyleValues.Thin;
            header.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            header.Style.Border.RightBorder = XLBorderStyleValues.Thin;

            int hSl = 1;
            int SL = 1;
            if (obj.WorkplaceGroupName == "Head Office")
            {

                header.Cell(1, hSl++).SetValue("SN");
                header.Cell(1, hSl++).SetValue("Employee ID");
                header.Cell(1, hSl++).SetValue("Employee Name");
                header.Cell(1, hSl++).SetValue("Designation");
                header.Cell(1, hSl++).SetValue("PIN");
                header.Cell(1, hSl++).SetValue("Joining Date");
                header.Cell(1, hSl++).SetValue("Present");
                header.Cell(1, hSl++).SetValue("Leave");
                header.Cell(1, hSl++).SetValue("Absent");
                header.Cell(1, hSl++).SetValue("Basic Salary");
                header.Cell(1, hSl++).SetValue("House Rent");
                header.Cell(1, hSl++).SetValue("Medical");
                header.Cell(1, hSl++).SetValue("Conv.");
                header.Cell(1, hSl++).SetValue("Special Allow.");
                header.Cell(1, hSl++).SetValue("Gross Salary");
                header.Cell(1, hSl++).SetValue("Arrear Salary");
                header.Cell(1, hSl++).SetValue("Extra House Rent");
                header.Cell(1, hSl++).SetValue("City Allow.");
                header.Cell(1, hSl++).SetValue("Total Salary");
                header.Cell(1, hSl++).SetValue("PF (Own)");
                header.Cell(1, hSl++).SetValue("Absent");
                header.Cell(1, hSl++).SetValue("Tax");
                header.Cell(1, hSl++).SetValue("Adj.");
                //header.Cell(1, hSl++).SetValue("Motor Cycle Loan");
                header.Cell(1, hSl++).SetValue("Fine");
                header.Cell(1, hSl++).SetValue("Net Salary");
                header.Cell(1, hSl++).SetValue("PF (Com.)");
                header.Cell(1, hSl++).SetValue("Gratuity");
                header.Cell(1, hSl++).SetValue("Total PF & Gratuity");
                header.Cell(1, hSl++).SetValue("Total Cost at Salary");
                header.Cell(1, hSl++).SetValue("Remarks");

                for (int i = 1; i <= 30; i++)
                {
                    worksheet.Cell(6, i).SetValue(i).Style
                        .Fill.SetBackgroundColor(XLColor.Gray)
                        .Font.SetFontColor(XLColor.White)
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                }

                //Table Data
                var dataArray = worksheet.Range(7, 1, TotalRowCount + 4, 8);
                dataArray.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                dataArray.Style.Border.TopBorder = XLBorderStyleValues.Thin;
                dataArray.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                dataArray.Style.Border.RightBorder = XLBorderStyleValues.Thin;
                
                int count = 1;
                var rowIndex = 1;


                foreach (var dept in obj.DepartmentVM)
                {
                    int gSl = 1;
                    //header.Cell(2, gSl++).SetValue("Area: "+area.AreaName+"");
                    dataArray.Cell(rowIndex, gSl).SetValue("Department: " + dept.deptName + "").IsMerged();
                    dataArray.Cell(rowIndex, gSl).Style.Font.SetBold(true);
                    dataArray.Cell(rowIndex, gSl).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    dataArray.Cell(rowIndex, gSl).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    dataArray.Cell(rowIndex, gSl).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    dataArray.Cell(rowIndex, gSl).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    dataArray.Rows(rowIndex, rowIndex).Style.Fill.SetBackgroundColor(XLColor.BlueGray);
                    dataArray.Cell(rowIndex, gSl).Style.Font.FontColor = XLColor.White;
                    dataArray.Rows(rowIndex, rowIndex).FirstOrDefault().Merge();

                    rowIndex++;

                    var deptWiseDetails = (from sal in obj.salaryDetailsReportVM
                                           where sal.intDepartmentId == dept.deptId
                                           orderby sal.depRankId ascending
                                           select sal).ToList();

                    foreach (var row in deptWiseDetails)
                    {
                        if (!string.IsNullOrEmpty(row.strDepartment))
                        {
                            int dSl = 1;
                            count++;
                            dataArray.Cell(rowIndex, dSl++).SetValue(SL).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            dataArray.Cell(rowIndex, dSl++).SetValue(row.strEmployeeCode).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            dataArray.Cell(rowIndex, dSl++).SetValue(row.strEmployeeName);
                            dataArray.Cell(rowIndex, dSl++).SetValue(row.strDesignation);
                            dataArray.Cell(rowIndex, dSl++).SetValue(row.intPIN).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            dataArray.Cell(rowIndex, dSl++).SetValue(Convert.ToDateTime(row.dteJoiningDate).ToString("dd MMM,yyyy")).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            dataArray.Cell(rowIndex, dSl++).SetValue(row.intPresent).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            dataArray.Cell(rowIndex, dSl++).SetValue(row.ApprovedLeave).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            dataArray.Cell(rowIndex, dSl++).SetValue(row.intAbsent).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            dataArray.Cell(rowIndex, dSl++).SetValue(row.numBasic);
                            dataArray.Cell(rowIndex, dSl++).SetValue(row.numHouseRent);
                            dataArray.Cell(rowIndex, dSl++).SetValue(row.numMedical);
                            dataArray.Cell(rowIndex, dSl++).SetValue(row.numConvence);
                            dataArray.Cell(rowIndex, dSl++).SetValue(row.numSpecialAllowence);
                            dataArray.Cell(rowIndex, dSl++).SetValue(row.numGrossSalary);
                            dataArray.Cell(rowIndex, dSl++).SetValue(row.numArearSalary);
                            dataArray.Cell(rowIndex, dSl++).SetValue(row.numExtraRouseRent);
                            dataArray.Cell(rowIndex, dSl++).SetValue(row.numCityAllowence);
                            dataArray.Cell(rowIndex, dSl++).SetValue(row?.numGrossSalary + row?.numTotalAllowance + row?.numArearSalary);
                            dataArray.Cell(rowIndex, dSl++).SetValue(row.numPFAmount);
                            dataArray.Cell(rowIndex, dSl++).SetValue(Convert.ToDecimal(row.AbsentAmount).ToString("0")).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            dataArray.Cell(rowIndex, dSl++).SetValue(row.numTaxAmount);
                            dataArray.Cell(rowIndex, dSl++).SetValue(row.numAdjustment);
                            //dataArray.Cell(rowIndex, dSl++).SetValue(row.numBikeLoan);
                            dataArray.Cell(rowIndex, dSl++).SetValue(row.numFine);
                            dataArray.Cell(rowIndex, dSl++).SetValue(row.numNetPayableSalary);
                            dataArray.Cell(rowIndex, dSl++).SetValue(Convert.ToDecimal(row.numPfCompany).ToString("0")).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            dataArray.Cell(rowIndex, dSl++).SetValue(Convert.ToDecimal(row.numGratuity).ToString("0")).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            dataArray.Cell(rowIndex, dSl++).SetValue(Convert.ToDecimal(row.TotalPfNGratuity).ToString("0")).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            dataArray.Cell(rowIndex, dSl++).SetValue(Convert.ToDecimal(row.TotalPfNGratuity + row.numNetPayableSalary).ToString("0")).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            dataArray.Cell(rowIndex, dSl++).SetValue("");

                            rowIndex++;
                        }

                        SL++;
                    }
                    if (dept.deptId > 0)
                    {
                        int dSl = 1;
                        dataArray.Cell(rowIndex, dSl++).SetValue("");
                        dataArray.Cell(rowIndex, dSl++).SetValue("");
                        dataArray.Cell(rowIndex, dSl++).SetValue("");
                        dataArray.Cell(rowIndex, dSl++).SetValue("");
                        dataArray.Cell(rowIndex, dSl++).SetValue("");
                        dataArray.Cell(rowIndex, dSl++).SetValue("");
                        dataArray.Cell(rowIndex, dSl++).SetValue("");
                        dataArray.Cell(rowIndex, dSl++).SetValue("");
                        dataArray.Cell(rowIndex, dSl++).SetValue("Sub Total:").Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intDepartmentId == dept.deptId && x.intEmployeeId > 0).Sum(x => x?.numBasic)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intDepartmentId == dept.deptId && x.intEmployeeId > 0).Sum(x => x?.numHouseRent)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intDepartmentId == dept.deptId && x.intEmployeeId > 0).Sum(x => x?.numMedical)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intDepartmentId == dept.deptId && x.intEmployeeId > 0).Sum(x => x?.numConvence)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intDepartmentId == dept.deptId && x.intEmployeeId > 0).Sum(x => x?.numSpecialAllowence)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intDepartmentId == dept.deptId && x.intEmployeeId > 0).Sum(x => x?.numGrossSalary)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intDepartmentId == dept.deptId && x.intEmployeeId > 0).Sum(x => x?.numArearSalary)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intDepartmentId == dept.deptId && x.intEmployeeId > 0).Sum(x => x?.numExtraAllowance)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intDepartmentId == dept.deptId && x.intEmployeeId > 0).Sum(x => x?.numCityAllowence)).Style.Font.SetBold(true);
                        //dataArray.Cell(rowIndex, dSl++).SetValue(row.numCityAllowence);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intDepartmentId == dept.deptId && x.intEmployeeId > 0).Sum(x => x?.numGrossSalary + x.numTotalAllowance + x.numArearSalary)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intDepartmentId == dept.deptId && x.intEmployeeId > 0).Sum(x => x?.numPFAmount)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(Convert.ToDecimal(obj.salaryDetailsReportVM.Where(x => x.intDepartmentId == dept.deptId && x.intEmployeeId > 0).Sum(x => x?.AbsentAmount)).ToString("0")).Style.Font.SetBold(true).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intDepartmentId == dept.deptId && x.intEmployeeId > 0).Sum(x => x?.numTaxAmount)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intDepartmentId == dept.deptId && x.intEmployeeId > 0).Sum(x => x?.numAdjustment)).Style.Font.SetBold(true);
                        //dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intDepartmentId == dept.deptId && x.intEmployeeId > 0).Sum(x => x?.numBikeLoan)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intDepartmentId == dept.deptId && x.intEmployeeId > 0).Sum(x => x?.numFine)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intDepartmentId == dept.deptId && x.intEmployeeId > 0).Sum(x => x?.numNetPayableSalary)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intDepartmentId == dept.deptId && x.intEmployeeId > 0).Sum(x => x?.numPfCompany)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(Convert.ToDecimal(obj.salaryDetailsReportVM.Where(x => x.intDepartmentId == dept.deptId && x.intEmployeeId > 0).Sum(x => x?.numGratuity)).ToString("0")).Style.Font.SetBold(true).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        dataArray.Cell(rowIndex, dSl++).SetValue(Convert.ToDecimal(obj.salaryDetailsReportVM.Where(x => x.intDepartmentId == dept.deptId && x.intEmployeeId > 0).Sum(x => x?.TotalPfNGratuity)).ToString("0")).Style.Font.SetBold(true).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        dataArray.Cell(rowIndex, dSl++).SetValue(Convert.ToDecimal(obj.salaryDetailsReportVM.Where(x => x.intDepartmentId == dept.deptId && x.intEmployeeId > 0).Sum(x => x?.TotalPfNGratuity + x?.numNetPayableSalary)).ToString("0")).Style.Font.SetBold(true).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        dataArray.Cell(rowIndex, dSl++).SetValue("");
                        rowIndex++;
                    }
                    if(count == countList + 1)
                    {
                        int dSl = 1;
                        dataArray.Cell(rowIndex, dSl++).SetValue("");
                        dataArray.Cell(rowIndex, dSl++).SetValue("");
                        dataArray.Cell(rowIndex, dSl++).SetValue("");
                        dataArray.Cell(rowIndex, dSl++).SetValue("");
                        dataArray.Cell(rowIndex, dSl++).SetValue("");
                        dataArray.Cell(rowIndex, dSl++).SetValue("");
                        dataArray.Cell(rowIndex, dSl++).SetValue("");
                        dataArray.Cell(rowIndex, dSl++).SetValue("");
                        dataArray.Cell(rowIndex, dSl++).SetValue("Grand Total:").Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numBasic)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numHouseRent)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numMedical)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numConvence)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numSpecialAllowence)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numGrossSalary)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numArearSalary)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numExtraAllowance)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numCityAllowence)).Style.Font.SetBold(true);
                        //dataArray.Cell(rowIndex, dSl++).SetValue(row.numCityAllowence);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numGrossSalary + x.numTotalAllowance + x.numArearSalary)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numPFAmount)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(Convert.ToDecimal(obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.AbsentAmount)).ToString("0")).Style.Font.SetBold(true).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numTaxAmount)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numAdjustment)).Style.Font.SetBold(true);
                        //dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intDepartmentId == dept.deptId && x.intEmployeeId > 0).Sum(x => x?.numBikeLoan)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numFine)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numNetPayableSalary)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numPfCompany)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(Convert.ToDecimal(obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numGratuity)).ToString("0")).Style.Font.SetBold(true).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        dataArray.Cell(rowIndex, dSl++).SetValue(Convert.ToDecimal(obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.TotalPfNGratuity)).ToString("0")).Style.Font.SetBold(true).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        dataArray.Cell(rowIndex, dSl++).SetValue(Convert.ToDecimal(obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.TotalPfNGratuity + x?.numNetPayableSalary)).ToString("0")).Style.Font.SetBold(true).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        dataArray.Cell(rowIndex, dSl++).SetValue("");
                        rowIndex++;

                        string? inWord = await AmountInWords.ConvertToWords(obj.salaryDetailsReportVM.Sum(x => x.numNetPayableSalary + x?.TotalPfNGratuity).ToString()) + " only";
                        dataArray.Rows(rowIndex, rowIndex).FirstOrDefault().Merge().SetValue("InWord: " + inWord + "").Style.Font.SetBold(true);

                    }
                }
                
                worksheet.Columns().AdjustToContents();

            }
            else if(obj.WorkplaceGroupName == "Marketing")
            {
                header.Cell(1, hSl++).SetValue("SN");
                header.Cell(1, hSl++).SetValue("Employee ID");
                header.Cell(1, hSl++).SetValue("Employee Name");
                header.Cell(1, hSl++).SetValue("Designation");
                header.Cell(1, hSl++).SetValue("PIN");
                header.Cell(1, hSl++).SetValue("Joining Date");
                header.Cell(1, hSl++).SetValue("Present");
                header.Cell(1, hSl++).SetValue("Leave");
                header.Cell(1, hSl++).SetValue("Absent");
                header.Cell(1, hSl++).SetValue("Basic Salary");
                header.Cell(1, hSl++).SetValue("House Rent");
                header.Cell(1, hSl++).SetValue("Medical");
                header.Cell(1, hSl++).SetValue("Conv.");
                header.Cell(1, hSl++).SetValue("Special Allow.");
                header.Cell(1, hSl++).SetValue("Gross Salary");
                header.Cell(1, hSl++).SetValue("Arrear Salary");
                header.Cell(1, hSl++).SetValue("Extra Allow.");
                //header.Cell(1, hSl++).SetValue("City Allow.");
                header.Cell(1, hSl++).SetValue("Total Salary");
                header.Cell(1, hSl++).SetValue("PF (Own)");
                header.Cell(1, hSl++).SetValue("Absent");
                header.Cell(1, hSl++).SetValue("Tax");
                header.Cell(1, hSl++).SetValue("Adj.");
                header.Cell(1, hSl++).SetValue("Motor Cycle");
                header.Cell(1, hSl++).SetValue("Fine");
                header.Cell(1, hSl++).SetValue("Net Salary");
                header.Cell(1, hSl++).SetValue("PF (Com.)");
                header.Cell(1, hSl++).SetValue("Gratuity");
                header.Cell(1, hSl++).SetValue("Total PF & Gratuity");
                header.Cell(1, hSl++).SetValue("Total Cost at Salary");
                header.Cell(1, hSl++).SetValue("Remarks");

                for (int i = 1; i <= 30; i++)
                {
                    worksheet.Cell(6, i).SetValue(i).Style
                        .Fill.SetBackgroundColor(XLColor.Gray)
                        .Font.SetFontColor(XLColor.White)
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                }

                //Table Data
                var dataArray = worksheet.Range(7, 1, TotalRowCount + 4, 8);
                dataArray.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                dataArray.Style.Border.TopBorder = XLBorderStyleValues.Thin;
                dataArray.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                dataArray.Style.Border.RightBorder = XLBorderStyleValues.Thin;

                int count = 1;
                    var rowIndex = 1;
                foreach (var area in obj.SoleDepoVM)
                {
                    int gSl = 1;
                    //header.Cell(2, gSl++).SetValue("Area: "+area.AreaName+"");
                    dataArray.Cell(rowIndex, gSl).SetValue("Sole Depo: " + area.SoleDepoName + "").IsMerged();
                    dataArray.Cell(rowIndex, gSl).Style.Font.SetBold(true);
                    dataArray.Cell(rowIndex, gSl).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    dataArray.Cell(rowIndex, gSl).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    dataArray.Cell(rowIndex, gSl).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    dataArray.Cell(rowIndex, gSl).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    dataArray.Rows(rowIndex, rowIndex).Style.Fill.SetBackgroundColor(XLColor.BlueGray);
                    dataArray.Cell(rowIndex, gSl).Style.Font.FontColor = XLColor.White;
                    dataArray.Rows(rowIndex, rowIndex).FirstOrDefault().Merge();
                    //dataArray.Range("A5:AD5").Merge();

                    rowIndex++;

                    var areaWiseDetails = (from sal in obj.salaryDetailsReportVM
                                           where sal.intSoleDepoId == area.SoleDepoId && sal.intAreaId <= 0
                                           orderby sal.RankId ascending
                                           select sal).ToList();


                    foreach (var row in areaWiseDetails)
                    {
                        if (!string.IsNullOrEmpty(row.SoleDipoName))
                        {
                            int dSl = 1;
                            count++;
                            dataArray.Cell(rowIndex, dSl++).SetValue(SL);
                            dataArray.Cell(rowIndex, dSl++).SetValue(row.strEmployeeCode);
                            dataArray.Cell(rowIndex, dSl++).SetValue(row.strEmployeeName);
                            dataArray.Cell(rowIndex, dSl++).SetValue(row.strDesignation);
                            dataArray.Cell(rowIndex, dSl++).SetValue(row.intPIN);
                            dataArray.Cell(rowIndex, dSl++).SetValue(Convert.ToDateTime(row.dteJoiningDate).ToString("dd MMM,yyyy"));
                            dataArray.Cell(rowIndex, dSl++).SetValue(row.intPresent);
                            dataArray.Cell(rowIndex, dSl++).SetValue(row.ApprovedLeave);
                            dataArray.Cell(rowIndex, dSl++).SetValue(row.intAbsent);
                            dataArray.Cell(rowIndex, dSl++).SetValue(row.numBasic);
                            dataArray.Cell(rowIndex, dSl++).SetValue(row.numHouseRent);
                            dataArray.Cell(rowIndex, dSl++).SetValue(row.numMedical);
                            dataArray.Cell(rowIndex, dSl++).SetValue(row.numConvence);
                            dataArray.Cell(rowIndex, dSl++).SetValue(row.numSpecialAllowence);
                            dataArray.Cell(rowIndex, dSl++).SetValue(row.numGrossSalary);
                            dataArray.Cell(rowIndex, dSl++).SetValue(row.numArearSalary);
                            dataArray.Cell(rowIndex, dSl++).SetValue(row.numExtraAllowance);
                            //dataArray.Cell(rowIndex, dSl++).SetValue(row.numCityAllowence);
                            dataArray.Cell(rowIndex, dSl++).SetValue(row?.numGrossSalary + row?.numTotalAllowance + row?.numArearSalary);
                            dataArray.Cell(rowIndex, dSl++).SetValue(row.numPFAmount);
                            dataArray.Cell(rowIndex, dSl++).SetValue(Convert.ToDecimal(row.AbsentAmount).ToString("0")).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            dataArray.Cell(rowIndex, dSl++).SetValue(row.numTaxAmount);
                            dataArray.Cell(rowIndex, dSl++).SetValue(row.numAdjustment);
                            dataArray.Cell(rowIndex, dSl++).SetValue(row.numBikeLoan);
                            dataArray.Cell(rowIndex, dSl++).SetValue(row.numFine);
                            dataArray.Cell(rowIndex, dSl++).SetValue(row.numNetPayableSalary);
                            dataArray.Cell(rowIndex, dSl++).SetValue(row.numPfCompany);
                            dataArray.Cell(rowIndex, dSl++).SetValue(Convert.ToDecimal(row.numGratuity).ToString("0")).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            dataArray.Cell(rowIndex, dSl++).SetValue(Convert.ToDecimal(row.TotalPfNGratuity).ToString("0")).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            dataArray.Cell(rowIndex, dSl++).SetValue(Convert.ToDecimal(row.TotalPfNGratuity + row.numNetPayableSalary + row?.numTaxAmount + row?.numAdjustment + row?.numBikeLoan).ToString("0")).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            dataArray.Cell(rowIndex, dSl++).SetValue("");

                            rowIndex++;
                            SL++;
                        }
                    }
                    if (area.SoleDepoId > 0)
                    {
                        int dSl = 1;
                        dataArray.Cell(rowIndex, dSl++).SetValue("");
                        dataArray.Cell(rowIndex, dSl++).SetValue("");
                        dataArray.Cell(rowIndex, dSl++).SetValue("");
                        dataArray.Cell(rowIndex, dSl++).SetValue("");
                        dataArray.Cell(rowIndex, dSl++).SetValue("");
                        dataArray.Cell(rowIndex, dSl++).SetValue("");
                        dataArray.Cell(rowIndex, dSl++).SetValue("");
                        dataArray.Cell(rowIndex, dSl++).SetValue("");
                        dataArray.Cell(rowIndex, dSl++).SetValue("Sub Total:").Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intAreaId == area.AreaId && x.intEmployeeId > 0).Sum(x => x?.numBasic)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intAreaId == area.AreaId && x.intEmployeeId > 0).Sum(x => x?.numHouseRent)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intAreaId == area.AreaId && x.intEmployeeId > 0).Sum(x => x?.numMedical)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intAreaId == area.AreaId && x.intEmployeeId > 0).Sum(x => x?.numConvence)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intAreaId == area.AreaId && x.intEmployeeId > 0).Sum(x => x?.numSpecialAllowence)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intAreaId == area.AreaId && x.intEmployeeId > 0).Sum(x => x?.numGrossSalary)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intAreaId == area.AreaId && x.intEmployeeId > 0).Sum(x => x?.numArearSalary)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intAreaId == area.AreaId && x.intEmployeeId > 0).Sum(x => x?.numExtraAllowance)).Style.Font.SetBold(true);
                        //dataArray.Cell(rowIndex, dSl++).SetValue(row.numCityAllowence);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intAreaId == area.AreaId && x.intEmployeeId > 0).Sum(x => x?.numGrossSalary + x.numTotalAllowance + x.numArearSalary)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intAreaId == area.AreaId && x.intEmployeeId > 0).Sum(x => x?.numPFAmount)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(Convert.ToDecimal(obj.salaryDetailsReportVM.Where(x => x.intAreaId == area.AreaId && x.intEmployeeId > 0).Sum(x => x?.AbsentAmount)).ToString("0")).Style.Font.SetBold(true).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intAreaId == area.AreaId && x.intEmployeeId > 0).Sum(x => x?.numTaxAmount)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intAreaId == area.AreaId && x.intEmployeeId > 0).Sum(x => x?.numAdjustment)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intAreaId == area.AreaId && x.intEmployeeId > 0).Sum(x => x?.numBikeLoan)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intAreaId == area.AreaId && x.intEmployeeId > 0).Sum(x => x?.numFine)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intAreaId == area.AreaId && x.intEmployeeId > 0).Sum(x => x?.numNetPayableSalary)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intAreaId == area.AreaId && x.intEmployeeId > 0).Sum(x => x?.numPfCompany)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(Convert.ToDecimal(obj.salaryDetailsReportVM.Where(x => x.intAreaId == area.AreaId && x.intEmployeeId > 0).Sum(x => x?.numGratuity)).ToString("0")).Style.Font.SetBold(true).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        dataArray.Cell(rowIndex, dSl++).SetValue(Convert.ToDecimal(obj.salaryDetailsReportVM.Where(x => x.intAreaId == area.AreaId && x.intEmployeeId > 0).Sum(x => x?.TotalPfNGratuity)).ToString("0")).Style.Font.SetBold(true).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        dataArray.Cell(rowIndex, dSl++).SetValue(Convert.ToDecimal(obj.salaryDetailsReportVM.Where(x => x.intAreaId == area.AreaId && x.intEmployeeId > 0).Sum(x => x?.TotalPfNGratuity + x?.numNetPayableSalary + x?.numTaxAmount + x?.numAdjustment + x?.numBikeLoan)).ToString("0")).Style.Font.SetBold(true).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        dataArray.Cell(rowIndex, dSl++).SetValue("");
                        rowIndex++;
                    }
                    

                }
                foreach (var area in obj.AreaVM)
                {
                    int gSl = 1;
                    //header.Cell(2, gSl++).SetValue("Area: "+area.AreaName+"");
                    dataArray.Cell(rowIndex, gSl).SetValue("Area: " + area.AreaName + "").IsMerged();
                    dataArray.Cell(rowIndex, gSl).Style.Font.SetBold(true);
                    dataArray.Cell(rowIndex, gSl).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    dataArray.Cell(rowIndex, gSl).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    dataArray.Cell(rowIndex, gSl).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    dataArray.Cell(rowIndex, gSl).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    dataArray.Rows(rowIndex, rowIndex).Style.Fill.SetBackgroundColor(XLColor.BlueGray);
                    dataArray.Cell(rowIndex, gSl).Style.Font.FontColor = XLColor.White;
                    dataArray.Rows(rowIndex, rowIndex).FirstOrDefault().Merge();
                    //dataArray.Range("A5:AD5").Merge();

                    rowIndex++;

                    var areaWiseDetails = (from sal in obj.salaryDetailsReportVM
                                           where sal.intAreaId == area.AreaId && sal.intAreaId > 0
                                           orderby sal.RankId ascending
                                           select sal).ToList();


                    foreach (var row in areaWiseDetails)
                    {
                        if (!string.IsNullOrEmpty(row.AreaName))
                        {
                            int dSl = 1;
                            count++;
                            dataArray.Cell(rowIndex, dSl++).SetValue(SL);
                            dataArray.Cell(rowIndex, dSl++).SetValue(row.strEmployeeCode);
                            dataArray.Cell(rowIndex, dSl++).SetValue(row.strEmployeeName);
                            dataArray.Cell(rowIndex, dSl++).SetValue(row.strDesignation);
                            dataArray.Cell(rowIndex, dSl++).SetValue(row.intPIN);
                            dataArray.Cell(rowIndex, dSl++).SetValue(Convert.ToDateTime(row.dteJoiningDate).ToString("dd MMM,yyyy"));
                            dataArray.Cell(rowIndex, dSl++).SetValue(row.intPresent);
                            dataArray.Cell(rowIndex, dSl++).SetValue(row.ApprovedLeave);
                            dataArray.Cell(rowIndex, dSl++).SetValue(row.intAbsent);
                            dataArray.Cell(rowIndex, dSl++).SetValue(row.numBasic);
                            dataArray.Cell(rowIndex, dSl++).SetValue(row.numHouseRent);
                            dataArray.Cell(rowIndex, dSl++).SetValue(row.numMedical);
                            dataArray.Cell(rowIndex, dSl++).SetValue(row.numConvence);
                            dataArray.Cell(rowIndex, dSl++).SetValue(row.numSpecialAllowence);
                            dataArray.Cell(rowIndex, dSl++).SetValue(row.numGrossSalary);
                            dataArray.Cell(rowIndex, dSl++).SetValue(row.numArearSalary);
                            dataArray.Cell(rowIndex, dSl++).SetValue(row.numExtraAllowance);
                            //dataArray.Cell(rowIndex, dSl++).SetValue(row.numCityAllowence);
                            dataArray.Cell(rowIndex, dSl++).SetValue(row?.numGrossSalary + row?.numTotalAllowance + row?.numArearSalary);
                            dataArray.Cell(rowIndex, dSl++).SetValue(row.numPFAmount);
                            dataArray.Cell(rowIndex, dSl++).SetValue(Convert.ToDecimal(row.AbsentAmount).ToString("0")).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            dataArray.Cell(rowIndex, dSl++).SetValue(row.numTaxAmount);
                            dataArray.Cell(rowIndex, dSl++).SetValue(row.numAdjustment);
                            dataArray.Cell(rowIndex, dSl++).SetValue(row.numBikeLoan);
                            dataArray.Cell(rowIndex, dSl++).SetValue(row.numFine);
                            dataArray.Cell(rowIndex, dSl++).SetValue(row.numNetPayableSalary);
                            dataArray.Cell(rowIndex, dSl++).SetValue(row.numPfCompany);
                            dataArray.Cell(rowIndex, dSl++).SetValue(Convert.ToDecimal(row.numGratuity).ToString("0")).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            dataArray.Cell(rowIndex, dSl++).SetValue(Convert.ToDecimal(row.TotalPfNGratuity).ToString("0")).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            dataArray.Cell(rowIndex, dSl++).SetValue(Convert.ToDecimal(row.TotalPfNGratuity + row.numNetPayableSalary + row?.numTaxAmount + row?.numAdjustment + row?.numBikeLoan).ToString("0")).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            dataArray.Cell(rowIndex, dSl++).SetValue("");

                            rowIndex++;
                            SL++;
                        }
                    }
                    if (area.AreaId > 0)
                    {
                        int dSl = 1;
                        dataArray.Cell(rowIndex, dSl++).SetValue("");
                        dataArray.Cell(rowIndex, dSl++).SetValue("");
                        dataArray.Cell(rowIndex, dSl++).SetValue("");
                        dataArray.Cell(rowIndex, dSl++).SetValue("");
                        dataArray.Cell(rowIndex, dSl++).SetValue("");
                        dataArray.Cell(rowIndex, dSl++).SetValue("");
                        dataArray.Cell(rowIndex, dSl++).SetValue("");
                        dataArray.Cell(rowIndex, dSl++).SetValue("");
                        dataArray.Cell(rowIndex, dSl++).SetValue("Sub Total:").Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intAreaId == area.AreaId && x.intEmployeeId > 0).Sum(x => x?.numBasic)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intAreaId == area.AreaId && x.intEmployeeId > 0).Sum(x => x?.numHouseRent)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intAreaId == area.AreaId && x.intEmployeeId > 0).Sum(x => x?.numMedical)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intAreaId == area.AreaId && x.intEmployeeId > 0).Sum(x => x?.numConvence)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intAreaId == area.AreaId && x.intEmployeeId > 0).Sum(x => x?.numSpecialAllowence)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intAreaId == area.AreaId && x.intEmployeeId > 0).Sum(x => x?.numGrossSalary)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intAreaId == area.AreaId && x.intEmployeeId > 0).Sum(x => x?.numArearSalary)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intAreaId == area.AreaId && x.intEmployeeId > 0).Sum(x => x?.numExtraAllowance)).Style.Font.SetBold(true);
                        //dataArray.Cell(rowIndex, dSl++).SetValue(row.numCityAllowence);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intAreaId == area.AreaId && x.intEmployeeId > 0).Sum(x => x?.numGrossSalary + x?.numTotalAllowance + x?.numArearSalary)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intAreaId == area.AreaId && x.intEmployeeId > 0).Sum(x => x?.numPFAmount)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(Convert.ToDecimal(obj.salaryDetailsReportVM.Where(x => x.intAreaId == area.AreaId && x.intEmployeeId > 0).Sum(x => x?.AbsentAmount)).ToString("0")).Style.Font.SetBold(true).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intAreaId == area.AreaId && x.intEmployeeId > 0).Sum(x => x?.numTaxAmount)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intAreaId == area.AreaId && x.intEmployeeId > 0).Sum(x => x?.numAdjustment)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intAreaId == area.AreaId && x.intEmployeeId > 0).Sum(x => x?.numBikeLoan)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intAreaId == area.AreaId && x.intEmployeeId > 0).Sum(x => x?.numFine)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intAreaId == area.AreaId && x.intEmployeeId > 0).Sum(x => x?.numNetPayableSalary)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intAreaId == area.AreaId && x.intEmployeeId > 0).Sum(x => x?.numPfCompany)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(Convert.ToDecimal(obj.salaryDetailsReportVM.Where(x => x.intAreaId == area.AreaId && x.intEmployeeId > 0).Sum(x => x?.numGratuity)).ToString("0")).Style.Font.SetBold(true).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        dataArray.Cell(rowIndex, dSl++).SetValue(Convert.ToDecimal(obj.salaryDetailsReportVM.Where(x => x.intAreaId == area.AreaId && x.intEmployeeId > 0).Sum(x => x?.TotalPfNGratuity)).ToString("0")).Style.Font.SetBold(true).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        dataArray.Cell(rowIndex, dSl++).SetValue(Convert.ToDecimal(obj.salaryDetailsReportVM.Where(x => x.intAreaId == area.AreaId && x.intEmployeeId > 0).Sum(x => x?.TotalPfNGratuity + x?.numNetPayableSalary + x?.numTaxAmount + x?.numAdjustment + x?.numBikeLoan)).ToString("0")).Style.Font.SetBold(true).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        dataArray.Cell(rowIndex, dSl++).SetValue("");
                        rowIndex++;
                    }
                    if (count == countList + 1)
                    {
                        int dSl = 1;
                        dataArray.Cell(rowIndex, dSl++).SetValue("");
                        dataArray.Cell(rowIndex, dSl++).SetValue("");
                        dataArray.Cell(rowIndex, dSl++).SetValue("");
                        dataArray.Cell(rowIndex, dSl++).SetValue("");
                        dataArray.Cell(rowIndex, dSl++).SetValue("");
                        dataArray.Cell(rowIndex, dSl++).SetValue("");
                        dataArray.Cell(rowIndex, dSl++).SetValue("");
                        dataArray.Cell(rowIndex, dSl++).SetValue("");
                        dataArray.Cell(rowIndex, dSl++).SetValue("Grand Total:").Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numBasic)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numHouseRent)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numMedical)).Style.Font.SetBold(true); ;
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numConvence)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numSpecialAllowence)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numGrossSalary)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numArearSalary)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numExtraAllowance)).Style.Font.SetBold(true);
                        //dataArray.Cell(rowIndex, dSl++).SetValue(row.numCityAllowence);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numGrossSalary + x?.numTotalAllowance + x?.numArearSalary)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numPFAmount)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(Convert.ToDecimal(obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.AbsentAmount)).ToString("0")).Style.Font.SetBold(true).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numTaxAmount)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numAdjustment)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numBikeLoan)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numFine)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numNetPayableSalary)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numPfCompany)).Style.Font.SetBold(true);
                        dataArray.Cell(rowIndex, dSl++).SetValue(Convert.ToDecimal(obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.numGratuity)).ToString("0")).Style.Font.SetBold(true).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        dataArray.Cell(rowIndex, dSl++).SetValue(Convert.ToDecimal(obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.TotalPfNGratuity)).ToString("0")).Style.Font.SetBold(true).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        dataArray.Cell(rowIndex, dSl++).SetValue(Convert.ToDecimal(obj.salaryDetailsReportVM.Where(x => x.intEmployeeId > 0).Sum(x => x?.TotalPfNGratuity + x?.numNetPayableSalary + x?.numTaxAmount + x?.numAdjustment + x?.numBikeLoan)).ToString("0")).Style.Font.SetBold(true).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        dataArray.Cell(rowIndex, dSl++).SetValue("");
                        rowIndex++;

                        string? inWord = await AmountInWords.ConvertToWords(obj.salaryDetailsReportVM.Sum(x => x.numNetPayableSalary).ToString()) + " only";
                        dataArray.Rows(rowIndex, rowIndex).FirstOrDefault().Merge().SetValue("InWord: " + inWord + "").Style.Font.SetBold(true);

                    }

                }
                worksheet.Columns().AdjustToContents();


            }


            //MemoryStream MS = new MemoryStream();
            //xLWorkbook.SaveAs(MS);
            //MS.Position = 0;

            //return new FileStreamResult(MS, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            //{
            //    FileDownloadName = $"MonthlySalary_{obj.Month}.xlsx"
            //};
            return xLWorkbook;

        }
    }
}

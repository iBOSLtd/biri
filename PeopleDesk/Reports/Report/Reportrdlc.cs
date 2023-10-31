using Microsoft.Reporting.NETCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Data;
using System.Linq;
using DinkToPdf.Contracts;
using DinkToPdf;

 

namespace PeopleDesk
{
    public class Reportrdlc : IReportrdlc
    {
        private readonly string DailyAttendanceReportCSSUrl;
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly IConverter _converter;
        public Reportrdlc(IConverter converter, IWebHostEnvironment hostingEnvironment)
        {
               _converter= converter;
            this.hostingEnvironment = hostingEnvironment;
            this.DailyAttendanceReportCSSUrl = hostingEnvironment.ContentRootPath + "/wwwroot/report.css";
        }
        private void Load<T>(LocalReport report, string reportPath, IList<T> items, string dataSetName, Dictionary<string, object> reportParameters = null)
        {
  
            using var rs = Assembly.GetExecutingAssembly().GetManifestResourceStream(reportPath);
            report.LoadReportDefinition(rs);
            //var data = ListtoDataTableConverter.ToDataTable(items);
            report.DataSources.Add(new ReportDataSource(dataSetName, items));
            var reportParamCollection = new List<ReportParameter>();
            if (reportParameters != null)
            {
                reportParamCollection.AddRange(reportParameters.Select(parameter =>
                    new ReportParameter(parameter.Key, parameter.Value?.ToString())));
                report.SetParameters(reportParamCollection);
            }
        
        }
        private void Load(LocalReport report, string reportPath, DataTable dt, string dataSetName, Dictionary<string, object> reportParameters = null)
        {
  
            using var rs = Assembly.GetExecutingAssembly().GetManifestResourceStream(reportPath);
            report.LoadReportDefinition(rs);
            report.DataSources.Add(new ReportDataSource(dataSetName, dt));
            var reportParamCollection = new List<ReportParameter>();
            if (reportParameters != null)
            {
                reportParamCollection.AddRange(reportParameters.Select(parameter =>
                    new ReportParameter(parameter.Key, parameter.Value?.ToString())));
                report.SetParameters(reportParamCollection);
            }
        
        }

        private byte[] PrepareReport(string reportPath, DataTable dt, string dataSetName, string renderFormat, Dictionary<string, object> reportParameters = null)
        {
            using var report = new LocalReport();
            Load(report,  reportPath, dt, dataSetName, reportParameters);
            var pdf = report.Render(renderFormat);
            return pdf;
        }

        private byte[] PrepareReport<T>(string reportPath, IList<T> items, string dataSetName, string renderFormat, Dictionary<string, object> reportParameters = null)
        {
            try
            {
                using var report = new LocalReport();
                Load(report, reportPath, items, dataSetName, reportParameters);
                var pdf = report.Render(renderFormat);
                return pdf;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        public byte[] GetPDF<T>(string reportPath, IList<T> items, string dataSetName, string renderFormat, int PageOrientation=1, Dictionary<string, object> reportParameters = null) {
            //"PDF", "pdf", "application/pdf"
            // reportPath ="ReportViewerCore.Sample.AspNetCore.Reports.Report.rdlc"
            // return File(data, "application/octet-stream", "Customer Information.xlsx");
         try
            {
                var data= PrepareReport(reportPath, items, dataSetName, "HTML5", reportParameters);
                var htmlString = System.Text.Encoding.Default.GetString(data);
                var globalSettings = new GlobalSettings
                {
                    ColorMode = ColorMode.Color,
                    Orientation = PageOrientation == 1? Orientation.Portrait: Orientation.Landscape,
                    PaperSize = PaperKind.LegalExtra,
                    DocumentTitle = "",
                };
                var objectSettings = new ObjectSettings
                {
                    PagesCount = true,
                    HtmlContent = htmlString,
                    FooterSettings = { FontName = "Arial", FontSize = 7, Line = false, Left = "Page [page] of [toPage]", Right = "System generated report. printed on " + DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt") }
                };

                HtmlToPdfDocument pdf = new HtmlToPdfDocument()
                {
                    GlobalSettings = globalSettings,
                    Objects = { objectSettings }
                };
                var file = _converter.Convert(pdf);
                return file;
             }
            catch (Exception ex)
            {
                throw ex;
            }
          
        }
        public byte[] GetPDF(string reportPath, DataTable dt, string dataSetName, string renderFormat, int PageOrientation=1, Dictionary<string, object> reportParameters = null)
        {
            //"PDF", "pdf", "application/pdf"
            // reportPath ="ReportViewerCore.Sample.AspNetCore.Reports.Report.rdlc"
            // return File(data, "application/octet-stream", "Customer Information.xlsx");
            try
            {
                var data = PrepareReport(reportPath, dt, dataSetName, "HTML5", reportParameters);
                var htmlString = System.Text.Encoding.Default.GetString(data);
                var globalSettings = new GlobalSettings
                {
                    ColorMode = ColorMode.Color,
                    Orientation = PageOrientation == 1 ? Orientation.Portrait : Orientation.Landscape,
                    PaperSize = PaperKind.A4,
                    DocumentTitle = "",
                };
                var objectSettings = new ObjectSettings
                {
                    PagesCount = true,
                    HtmlContent = htmlString,
                    WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = "" }, //DailyAttendanceReportCSSUrl                  
                    FooterSettings = { FontName = "Arial", FontSize = 7, Line = false, Left = "Page [page] of [toPage]", Right = "System generated report. printed on " + DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt") }
                };

                HtmlToPdfDocument pdf = new HtmlToPdfDocument()
                {
                    GlobalSettings = globalSettings,
                    Objects = { objectSettings }
                };
                var file = _converter.Convert(pdf);
                return file;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public byte[] GetHTML<T>(string reportPath, IList<T> items, string dataSetName, string renderFormat, Dictionary<string, object> reportParameters = null)
        {
            //"HTML5", "html", "text/html"
            // reportPath ="ReportViewerCore.Sample.AspNetCore.Reports.Report.rdlc"
            // return File(data, "application/octet-stream", "Customer Information.xlsx");
            return PrepareReport(reportPath, items, dataSetName, "HTML5", reportParameters);

        }
        public byte[] GetHTML(string reportPath, DataTable dt, string dataSetName, string renderFormat, Dictionary<string, object> reportParameters = null)
        {
            //"HTML5", "html", "text/html"
            // reportPath ="ReportViewerCore.Sample.AspNetCore.Reports.Report.rdlc"
            // return File(data, "application/octet-stream", "Customer Information.xlsx");
            return PrepareReport(reportPath, dt, dataSetName, "HTML5", reportParameters);

        }

        public byte[] GetXLSX<T>(string reportPath, IList<T> items, string dataSetName, string renderFormat, Dictionary<string, object> reportParameters = null)
        {
            //"EXCELOPENXML", "xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            // reportPath ="ReportViewerCore.Sample.AspNetCore.Reports.Report.rdlc"
            // return File(data, "application/octet-stream", "Customer Information.xlsx");
            return PrepareReport(reportPath, items, dataSetName, "EXCELOPENXML", reportParameters);

        }
        public byte[] GetXLSX(string reportPath, DataTable dt, string dataSetName, string renderFormat, Dictionary<string, object> reportParameters = null)
        {
            //"EXCELOPENXML", "xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            // reportPath ="ReportViewerCore.Sample.AspNetCore.Reports.Report.rdlc"
            // return File(data, "application/octet-stream", "Customer Information.xlsx");
            return PrepareReport(reportPath, dt, dataSetName, "EXCELOPENXML", reportParameters);

        }
 
 
    }
}
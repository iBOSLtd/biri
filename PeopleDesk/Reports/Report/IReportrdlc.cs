using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeopleDesk
{
    public interface IReportrdlc
    {
        byte[] GetPDF<T>(string reportPath, IList<T> items, string dataSetName, string renderFormat,int PageOrientation, Dictionary<string, object> reportParameters = null);
        byte[] GetPDF(string reportPath, DataTable dt, string dataSetName, string renderFormat,int PageOrientation, Dictionary<string, object> reportParameters = null);
        byte[] GetHTML<T>(string reportPath, IList<T> items, string dataSetName, string renderFormat, Dictionary<string, object> reportParameters = null);
        byte[] GetHTML(string reportPath, DataTable dt, string dataSetName, string renderFormat, Dictionary<string, object> reportParameters = null);
        byte[] GetXLSX<T>(string reportPath, IList<T> items, string dataSetName, string renderFormat, Dictionary<string, object> reportParameters = null);
        byte[] GetXLSX(string reportPath, DataTable dt, string dataSetName, string renderFormat, Dictionary<string, object> reportParameters = null);


    }
}
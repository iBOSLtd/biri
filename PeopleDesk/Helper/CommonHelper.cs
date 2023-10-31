using System.Data;
using System.Text;

namespace PeopleDesk.Helper
{
    public class CommonHelper
    {
        public static string ConcateStringWithComma(List<string?> ObjList)
        {
            StringBuilder builder = new StringBuilder();
            foreach (string? s in ObjList)
            {
                if (s != null && s != "")
                    builder.Append(s).Append(", ");
            }
            return builder.ToString().TrimEnd(new char[] { ',', ' ' });
        }
        public static async Task<GetDynamicDataTableDTO> GetDynamicDataTable(DataTable dt)
        {
            var headerNameList = new List<string>();
            var dynamicTableRow = new List<DynamicTableRowDTO>();

            for (int i = 0; i <= dt.Rows.Count - 1; i++)
            {
                var rowDataList = new List<string>();
                for (int j = 0; j <= dt.Columns.Count - 1; j++)
                {
                    rowDataList.Add(dt.Rows[i][j].ToString());
                }
                dynamicTableRow.Add(new DynamicTableRowDTO()
                {
                    TableData = rowDataList
                });
            }

            foreach (DataColumn column in dt.Columns)
            {
                headerNameList.Add(column.ColumnName);
            }

            return new GetDynamicDataTableDTO()
            {
                HeadingNames = headerNameList,
                TableRow = dynamicTableRow
            };
        }

        public static string Encode(string password)
        {
            var utfPassword = Encoding.UTF8.GetBytes(password);
            string encryptedPassword = Convert.ToBase64String(utfPassword);

            return encryptedPassword;
        }
        public static string Decode(string password)
        {
            var utfPassword = System.Convert.FromBase64String(password);
            string decryptedPassword = Encoding.UTF8.GetString(utfPassword);

            return decryptedPassword;
        }
    }

    public class GetDynamicDataTableDTO
    {
        public List<string> HeadingNames { get; set; }
        public List<DynamicTableRowDTO> TableRow { get; set; }
    }
    public class DynamicTableRowDTO
    {
        public List<string> TableData { get; set; }
    }
}

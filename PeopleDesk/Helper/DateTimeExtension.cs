using System.Runtime.InteropServices;
namespace PeopleDesk.Helper
{
    public class DateTimeExtension
    {
        public static DateTime GetCurrentDateTimeBD()
        {
            TimeZoneInfo timeZoneInfo;
            DateTime dateTime;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Bangladesh Standard Time");
                dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, timeZoneInfo);
                return dateTime;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Asia/Dhaka");
                dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, timeZoneInfo);
                return dateTime;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                throw new Exception("I don't know how to do a lookup on a Mac TimeZone");
            }
            else
            {
                throw new Exception("I don't know how to do a lookup TimeZone");
            }
        }
        public static DateTime GetFirstDayOfMonth(DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }
        public static DateTime GetLastDayOfMonth(DateTime date)
        {
            return GetFirstDayOfMonth(date).AddMonths(1).AddDays(-1);
        }
        public static int GetMonthDifference(DateTime start, DateTime end)
        {
            if (start > end)
            {
                var swapper = start;
                start = end;
                end = swapper;
            }

            return ((end.Year * 12) + end.Month) - ((start.Year * 12) + start.Month);
        }
    }
}

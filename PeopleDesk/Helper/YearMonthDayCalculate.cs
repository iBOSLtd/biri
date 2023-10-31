using NodaTime;

namespace PeopleDesk.Helper
{
    public class YearMonthDayCalculate
    {
        public static string YearMonthDayShortFormCal(DateTime fromDate, DateTime toDate)
        {
            string serviceLength = String.Empty;

            if (fromDate != null && toDate != null)
            {
                LocalDate joiningDate = new LocalDate(fromDate.Year, fromDate.Month, fromDate.Day);
                LocalDate currentDate = new LocalDate(toDate.Year, toDate.Month, toDate.Day);
                Period period = Period.Between(joiningDate, currentDate.PlusDays(1));

                serviceLength += period.Years != 0 ? period.Years + "Y " : "";
                serviceLength += period.Months != 0 ? period.Months + "M " : "";
                serviceLength += period.Days != 0 ? period.Days + "D" : "";

            }
            else { serviceLength = "invalid data"; }
            return serviceLength;
        }
        public static string YearMonthDayLongFormCal(DateTime fromDate, DateTime toDate)
        {
            string serviceLength = String.Empty;

            if (fromDate != null && toDate != null)
            {
                LocalDate joiningDate = new LocalDate(fromDate.Year, fromDate.Month, fromDate.Day);
                LocalDate currentDate = new LocalDate(toDate.Year, toDate.Month, toDate.Day);
                Period period = Period.Between(joiningDate, currentDate.PlusDays(1));

                serviceLength += period.Years != 0 ? period.Years + "years " : "";
                serviceLength += period.Months != 0 ? period.Months + "months " : "";
                serviceLength += period.Days != 0 ? period.Days + "days" : "";

            }
            else { serviceLength = "invalid data"; }
            return serviceLength;
        }
        public static int CalculateDaysBetweenTwoDate(DateTime fromDate, DateTime toDate)
        {
            int serviceLength = 0;

            if (fromDate != null && toDate != null)
            {
                LocalDate joiningDate = new LocalDate(fromDate.Year, fromDate.Month, fromDate.Day);
                LocalDate currentDate = new LocalDate(toDate.Year, toDate.Month, toDate.Day);
                Period period = Period.Between(joiningDate, currentDate.PlusDays(1));

                serviceLength += period.Years != 0 ? period.Years * 365 : 0;
                serviceLength += period.Months != 0 ? period.Months * 30 : 0;
                serviceLength += period.Days != 0 ? period.Days : 0;

            }
            return serviceLength;
        }
    }
}

using System.Text;

namespace PeopleDesk.Helper
{
    public static class StringExtend
    {
        public static string LeftPad(this string value, string delimeter, int size = 6)
        {
            var baseValue = new StringBuilder();
            for (var i = 0; i < size; i++)
            {
                baseValue.Append(delimeter);
            }

            var max = baseValue.Length;

            if (max < value.Length)
            {
                return value.ToString();
            }

            StringBuilder sb = new StringBuilder(baseValue.ToString());
            var index = max - value.Length;
            var start = 0;
            for (int i = index; i < index + value.Length; i++)
            { sb[i] = value[start++]; }

            return sb.ToString();
        }
    }
}

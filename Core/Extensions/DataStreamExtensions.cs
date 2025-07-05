using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Extensions
{
    public static  class DataStreamExtensions
    {
        public static List<decimal> ParseToDecimalList(this string values, params char[] separators)
        {
            var decimalList = values.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            return decimalList.Select(decimal.Parse).ToList();
        }

        public static string ToValuesString(this IEnumerable<decimal> values, char separator = ';')
        {
            return string.Join(separator, values);
        }
    }
}

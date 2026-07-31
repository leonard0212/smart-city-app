using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Domain.Extensions
{
    public static class DateTimeExtensions
    {

        public static string ToPrintDate(this DateTime date)
        {
            return date.ToString("dd.MM.yyyy");
        }

        public static string ToFullPrintDate(this DateTime date)
        {
            return date.ToString("dd.MM.yyyy hh:mm:ss");
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Domain.Utils
{
    public static class StringUtils
    {

        public static string? Coalesce(params string[] items)
        {
            var firstOrDefaultNotNull = items?.FirstOrDefault(x => !string.IsNullOrEmpty(x));
            return firstOrDefaultNotNull;
        }
    }
}

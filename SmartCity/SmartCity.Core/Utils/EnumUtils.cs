using Microsoft.AspNetCore.Mvc.Rendering;
using SmartCity.Domain.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Core.Utils
{
    public static class EnumUtils
    {
        public static IEnumerable<SelectListItem> GetSelectListWithDefault(Type enumType)
        {
            return GetSelectListWithDefault(enumType, null);
        }

        public static IEnumerable<SelectListItem> GetSelectListWithDefault(Type enumType, int? enumIntValue, bool orderAlphabetically = false)
        {
            var selectList = GetSelectList(enumType, enumIntValue, orderAlphabetically);
            var selectListWithDefault = selectList.Prepend(new SelectListItem { Text = "--Selectează--", Value = "" });

            return selectListWithDefault;
        }

        public static IEnumerable<SelectListItem> GetSelectList(Type enumType)
        {
            return GetSelectList(enumType, null);
        }

        public static IEnumerable<SelectListItem> GetSelectList(Type enumType, int? enumIntValue, bool orderAlphabetically = false)
        {
            if (!enumType.GetTypeInfo().IsEnum)
                throw new ArgumentException("Only enum allowed.", nameof(enumType));

            var elements = Enum.GetValues(enumType).Cast<Enum>().ToList();
            var items = elements.Select(
                item => new SelectListItem
                {
                    Text = item.Description(),
                    Value = item.ToString(),
                    Selected = Convert.ToInt32(item) == enumIntValue
                }).ToList();

            if (orderAlphabetically)
                items = items.OrderBy(x => x.Text).ToList();

            return items;
        }

        public static TEnum? TryParse<TEnum>(string value)
            where TEnum : struct
        {
            TEnum outEnumValue;
            var enumValue = Enum.TryParse(value, out outEnumValue) ? outEnumValue : (TEnum?)null;

            return enumValue;
        }
    }
}

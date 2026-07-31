using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using SmartCity.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Core.TagHelpers
{
    [HtmlTargetElement("select", Attributes = ForAttributeName)]
    public class EnumSelectTagHelper : SelectTagHelper
    {
        private const string ForAttributeName = "asp-for-enum";
        private const string WithDefaultAttributeName = "asp-with-default";
        private const string OrderAlphabeticallyAttributeName = "asp-order-alphabetically";

        public EnumSelectTagHelper(IHtmlGenerator generator) : base(generator) { }

        [HtmlAttributeName(ForAttributeName)]
        public ModelExpression ForEnum { get; set; }

        [HtmlAttributeName(WithDefaultAttributeName)]
        public bool? WithDefault { get; set; }

        [HtmlAttributeName(OrderAlphabeticallyAttributeName)]
        public bool? OrderAlphabetically { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (output == null)
                throw new ArgumentNullException(nameof(output));

            if (!ForEnum.Metadata.IsEnum)
                throw new Exception("Only enum allowed.");

            var enumIntValue = ForEnum.Model != null ? Convert.ToInt32(ForEnum.Model) : (int?)null;
            var enumType = ForEnum.Metadata.UnderlyingOrModelType;

            var items = WithDefault == true
                ? EnumUtils.GetSelectListWithDefault(enumType, enumIntValue, OrderAlphabetically ?? false)
                : EnumUtils.GetSelectList(enumType, enumIntValue, OrderAlphabetically ?? false);

            For = ForEnum;
            Items = items;

            base.Process(context, output);
        }
    }
}

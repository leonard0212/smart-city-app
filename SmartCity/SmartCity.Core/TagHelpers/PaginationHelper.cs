using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Core.TagHelpers
{
    [HtmlTargetElement("pagination")]
    public class PaginationHelper : TagHelper
    {
        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName("hide-on-single-page")]
        public bool HideOnSinglePage { get; set; }

        [HtmlAttributeName("show-total-count")]
        public bool ShowTotalCount { get; set; }

        [HtmlAttributeName("has-previous-page")]
        public bool HasPreviousPage { get; set; }

        [HtmlAttributeName("has-next-page")]
        public bool HasNextPage { get; set; }

        [HtmlAttributeName("page-count")]
        public int PageCount { get; set; }

        [HtmlAttributeName("page-index")]
        public int PageIndex { get; set; }

        [HtmlAttributeName("max-items")]
        public int MaxItems { get; set; }

        [HtmlAttributeName("total-count")]
        public int TotalCount { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (HideOnSinglePage && PageCount < 2)
            {
                output.SuppressOutput();
                return;
            }

            var min = Math.Max(1, PageIndex - MaxItems / 2);
            var max = Math.Min(min + MaxItems, PageCount);

            var html = new StringBuilder();
         
            html.Append("<span class=\"btn-group\">");
            if (HasPreviousPage)
            {
                html.AppendFormat("<a class=\"btn btn-white btn-pagination\" data-ref-page=\"{1}\" rel =\"prev\" href=\"{0}\">&laquo;</a>",
                    GetUrlForPage(PageIndex - 1), PageIndex - 1);
            }
            else
            {
                html.AppendFormat("<a class=\"btn btn-white btn-pagination disabled\" rel=\"prev\" href=\"{0}\">&laquo;</a>", "javascript:void(0)");
            }

            foreach (var page in Enumerable.Range(min, max - min + 1))
            {
                if (page == PageIndex)
                {
                    html.AppendFormat("<a class=\"btn btn-pagination sc-pagination-btn\" data-ref-page=\"{2}\" href=\"{0}\">{1}</a>", "javascript:void(0)", page, page);
                }
                else
                {
                    html.AppendFormat("<a class=\"btn btn-white btn-pagination\" data-ref-page=\"{2}\" href=\"{0}\">{1}</a>",
                        GetUrlForPage(page), page, page);
                }
            }

            if (HasNextPage)
            {
                html.AppendFormat("<a class=\"btn btn-white btn-pagination\" data-ref-page=\"{1}\" rel=\"next\" href=\"{0}\">&raquo;</a>",
                    GetUrlForPage(PageIndex + 1), PageIndex + 1);
            }
            else
            {
                html.AppendFormat("<a class=\"btn btn-white btn-pagination disabled\" rel=\"next\" href=\"{0}\">&raquo;</a>", "javascript:void(0)");
            }

            html.Append("</span>");

            if (ShowTotalCount)
                html.AppendFormat("<span><i>count: {0}</i></span>", TotalCount);

            output.Content.SetHtmlContent(html.ToString());
        }

        private string GetUrlForPage(int pageIndex)
        {
            var temp = ViewContext.ViewBag.Params as RouteValueDictionary;
            if (temp == null)
            {
                var routeData = ViewContext.RouteData.Values;
                var queryString = ViewContext.HttpContext.Request.Query;
                temp = new RouteValueDictionary(routeData);

                if (queryString.Count > 0)
                {
                    foreach (var key in queryString.Keys)
                    {
                        if (!temp.ContainsKey(key))
                            temp.Add(key, queryString[key]);
                    }
                }
            }
            else
            {
                temp = new RouteValueDictionary(temp);
            }

            if (temp.ContainsKey("page"))
            {
                temp["page"] = pageIndex;
            }
            else
            {
                temp.Add("page", pageIndex);
            }

            var url = QueryHelpers.AddQueryString(ViewContext.HttpContext.Request.Path.Value, temp.ToDictionary(x => x.Key, x => x.Value.ToString()));
            return url;
        }
    }
}

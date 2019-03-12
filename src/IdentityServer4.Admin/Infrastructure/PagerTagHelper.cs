using System;
using System.Collections;
using System.Text;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace IdentityServer4.Admin.Infrastructure
{
    public class PagerOption
    {
        public int Page { get; set; }

        public int Total { get; set; }

        public int Size { get; set; }

        public string RouteUrl { get; set; } = "/";

        public int PagerCount { get; set; } = 4;
    }

    public class PagerTagHelper : TagHelper
    {
        public PagerOption PagerOption { get; set; }


        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";

            if (PagerOption.Size <= 0)
            {
                PagerOption.Size = 15;
            }

            if (PagerOption.Page <= 0)
            {
                PagerOption.Page = 1;
            }

            if (PagerOption.Total <= 0)
            {
                return;
            }

            // 计算分页
            var totalPage = PagerOption.Total / PagerOption.Size +
                            (PagerOption.Total % PagerOption.Size > 0 ? 1 : 0);
            if (totalPage <= 0)
            {
                return;
            }

            var pageNumbers = new ArrayList();
            int start = 1;
            bool isShowStart = false;
            bool isShowEnd = false;
            if (PagerOption.Page >= PagerOption.PagerCount)
            {
                start = PagerOption.Page - PagerOption.PagerCount / 2;
                isShowStart = true;
            }
            else
            {
                isShowStart = false;
            }

            var end = start + PagerOption.PagerCount - 1;
            if (end > totalPage)
            {
                end = totalPage;
                isShowEnd = false;
            }
            else
            {
                isShowEnd = true;
            }

            ;
            for (var i = start; i <= end; i++)
            {
                pageNumbers.Add(i);
            }

            //当前路由地址
            if (string.IsNullOrEmpty(PagerOption.RouteUrl))
            {
                //PagerOption.RouteUrl = helper.ViewContext.HttpContext.Request.RawUrl;
                if (!string.IsNullOrEmpty(PagerOption.RouteUrl))
                {
                    var lastIndex = PagerOption.RouteUrl.LastIndexOf("/", StringComparison.Ordinal);
                    PagerOption.RouteUrl = PagerOption.RouteUrl.Substring(0, lastIndex);
                }
            }

            PagerOption.RouteUrl = PagerOption.RouteUrl.TrimEnd('/');

            //构造分页样式
            var pagerBuilder = new StringBuilder(string.Empty);
            pagerBuilder.Append("<ul id=\"pagination\" class=\"pagination pagination-sm no-margin pull-left\">");

            if (isShowStart)
            {
                pagerBuilder.AppendFormat(
                    "<a class=\"page-item \" href=\"{0}/{1}\" aria-label=\"Previous\"><i class=\"icon-arrow-left\"></i></a>",
                    PagerOption.RouteUrl,
                    PagerOption.Page - 1 <= 0 ? 1 : PagerOption.Page - 1);
            }

            foreach (var i in pageNumbers)
            {
                if (Convert.ToInt32(i) == PagerOption.Page)
                {
                    pagerBuilder.AppendFormat("<span class=\"page-number current\">{0}</span>",
                        i,
                        PagerOption.RouteUrl);
                }
                else
                {
                    pagerBuilder.AppendFormat("<a class=\"page-number\" href=\"{1}/{0}\">{0}</a>",
                        i,
                        PagerOption.RouteUrl);
                }
            }

            if (isShowEnd)
            {
                pagerBuilder.AppendFormat(
                    "<a class=\"page-number\" href=\"{0}/{1}\" aria-label=\"Next\"><i class=\"icon-arrow-right\"></i></a>",
                    PagerOption.RouteUrl,
                    PagerOption.Page + 1 > totalPage ? PagerOption.Page : PagerOption.Page + 1);
            }

            pagerBuilder.Append("</ul>");

            output.Content.SetHtmlContent(pagerBuilder.ToString());
        }
    }
}
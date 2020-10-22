﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;

using R8.Lib.AspNetCore.Base;

namespace R8.Lib.AspNetCore.TagHelpers
{
    [HtmlTargetElement("a", Attributes = ActionAttributeName)]
    [HtmlTargetElement("a", Attributes = ControllerAttributeName)]
    [HtmlTargetElement("a", Attributes = AreaAttributeName)]
    [HtmlTargetElement("a", Attributes = PageAttributeName)]
    [HtmlTargetElement("a", Attributes = PageHandlerAttributeName)]
    [HtmlTargetElement("a", Attributes = FragmentAttributeName)]
    [HtmlTargetElement("a", Attributes = HostAttributeName)]
    [HtmlTargetElement("a", Attributes = ProtocolAttributeName)]
    [HtmlTargetElement("a", Attributes = RouteAttributeName)]
    [HtmlTargetElement("a", Attributes = RouteValuesDictionaryName)]
    [HtmlTargetElement("a", Attributes = RouteValuesPrefix + "*")]
    public class R8AnchorTagHelper : AnchorTagHelper
    {
        private readonly IOptions<RequestLocalizationOptions> _options;

        [HtmlAttributeName("asp-disabled")]
        public bool Disabled { get; set; }

        public R8AnchorTagHelper(IHtmlGenerator generator, IOptions<RequestLocalizationOptions> options) : base(generator)
        {
            _options = options;
        }

        private const string ActionAttributeName = "asp-action";
        private const string ControllerAttributeName = "asp-controller";
        private const string AreaAttributeName = "asp-area";
        private const string PageAttributeName = "asp-page";
        private const string PageHandlerAttributeName = "asp-page-handler";
        private const string FragmentAttributeName = "asp-fragment";
        private const string HostAttributeName = "asp-host";
        private const string ProtocolAttributeName = "asp-protocol";
        private const string RouteAttributeName = "asp-route";
        private const string RouteValuesDictionaryName = "asp-all-route-data";
        private const string RouteValuesPrefix = "asp-route-";

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var culture = (string)ViewContext.HttpContext.Request.RouteValues[LanguageRouteConstraint.Key];
            if (culture != _options.Value.DefaultRequestCulture.Culture.Name)
                RouteValues[LanguageRouteConstraint.Key] = culture;

            if (Disabled)
                output.Attributes.Add("disabled", "");

            base.Process(context, output);
        }
    }
}
using System;
using System.Collections.Generic;
using jcode.util;
using Microsoft.AspNetCore.Mvc.Routing;

namespace jcode.web.util
{
    public class HttpDeleteToPostAttribute : HttpMethodAttribute
    {
        private static readonly IEnumerable<string> _supportedMethods = new [] { "POST" };

        /// <summary>
        /// Creates a new <see cref="HttpDeleteToPostAttribute"/>.
        /// </summary>
        public HttpDeleteToPostAttribute()
            : base(_supportedMethods)
        {
        }

        /// <summary>
        /// Creates a new <see cref="HttpDeleteToPostAttribute"/> with the given route template.
        /// </summary>
        /// <param name="template">The route template. May not be null.</param>
        public HttpDeleteToPostAttribute(string template)
            : base(_supportedMethods, GetTemplate(template))
        {
            if (template == null)
            {
                throw new ArgumentNullException(nameof(template));
            }
        }

        private static string GetTemplate(string template)
        {
            if (ConfigHelper.GetConfig().IsSass)// 只是测试，后续改下属性
            {
                return $"jcode_delete/{template}";
            }
            else
            {
                return template;
            }
        }
    }
}
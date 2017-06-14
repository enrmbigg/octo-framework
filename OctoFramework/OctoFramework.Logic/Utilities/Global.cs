using System.Web;
using Umbraco.Core;

namespace OctoFramework.Logic.Utilities
{
    public class Global : Umbraco.Web.UmbracoApplication
    {
        public override string GetVaryByCustomString(HttpContext context, string custom)
        {
            if (custom.InvariantEquals("url"))
            {
                return "url=" + context.Request.Url.AbsoluteUri;
            }

            return base.GetVaryByCustomString(context, custom);
        }
    }
}
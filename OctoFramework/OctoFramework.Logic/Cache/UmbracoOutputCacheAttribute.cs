using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.UI;
using DevTrends.MvcDonutCaching;
using Umbraco.Web;

namespace OctoFramework.Logic.Cache
{
    public class UmbracoOutputCacheAttribute : DonutOutputCacheAttribute
    {
        public UmbracoOutputCacheAttribute()
        {
            Duration = Duration.Equals(0) ? 0 : int.Parse(WebConfigurationManager.AppSettings["UmbracoOutputCacheDuration"]);
            VaryByCustom = string.IsNullOrWhiteSpace(VaryByCustom) ? "url" : VaryByCustom;
            Location = Location == (OutputCacheLocation)(-1) ? OutputCacheLocation.Server : Location;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var filtered = HttpUtility.ParseQueryString(filterContext.HttpContext.Request.QueryString.ToString());
            var previewMode = UmbracoContext.Current.InPreviewMode;
            var debugMode = filterContext.HttpContext.IsDebuggingEnabled;
            var umbDebugMode = filtered["umbDebug"] != null;
            /*
            if (filterContext.HttpContext.Request.Browser.Crawler)
            {
                return;
            }*/

            // If not in preview mode or Umbraco debug mode then return cached output
            if (!debugMode && !previewMode && !umbDebugMode)
            {
                base.OnActionExecuting(filterContext);
            }
            // If compilation debug mode is false but Umbraco debug mode is requested return cached output
            else if (!debugMode && umbDebugMode)
            {
                base.OnActionExecuting(filterContext);
            }
        }
    }
}
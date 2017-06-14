using System.Web.Configuration;
using System.Web.Mvc;
using DevTrends.MvcDonutCaching;
using OctoFramework.Logic.Cache;
using Elmah;
using umbraco;
using Umbraco.Core.Logging;
using Umbraco.Web;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;

namespace OctoFramework.Logic.Controllers
{
    public class DefaultController : SurfaceController, IRenderController
    {
        //Constructors
        public DefaultController() { }

        public DefaultController(UmbracoContext context) : base(context)
        {
        }

        public DefaultController(UmbracoContext context, UmbracoHelper helper) : base(context, helper)
        {
        }

        protected UmbracoContext Context { get; private set; }

        public override UmbracoHelper Umbraco
        {
            get { return new UmbracoHelper(UmbracoContext); }
        }

        /// <summary>
        /// If the route hijacking doesn't find a controller this default controller will be used.
        /// That way a each page will always go through a controller and we can always have a MasterModel for the masterpage.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [UmbracoOutputCache]
        public virtual ActionResult Index(RenderModel model)
        {
            return View(ControllerContext.RouteData.Values["action"].ToString(), model.Content);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            //Log the exception.
            LogHelper.Error<RenderMvcController>("Exception On Render", filterContext.Exception);
            ErrorSignal.FromCurrentContext().Raise(filterContext.Exception);
            //Email out
            var message = $"<p>Url: {filterContext.HttpContext.Request.Url}  <br/><br/>" + $"Exception occured: {filterContext.Exception}</p>";
            var user = WebConfigurationManager.AppSettings["ErrorEmailAddress"];
            library.SendMail("info@JuraWhiskey.com", user, "Error Occurred", message, true);

            //Check if its been handled
            if (filterContext.ExceptionHandled)
            {
                return;
            }

            //Clear the cache if an error occurs.
            var cacheManager = new OutputCacheManager();
            cacheManager.RemoveItems();

            //Show the view error.
            filterContext.Result = View("Error");
            filterContext.ExceptionHandled = true;
        }

        protected ActionResult CurrentTemplate<T>(T model)
        {
            var template = ControllerContext.RouteData.Values["action"].ToString();
            if (!EnsurePhsyicalViewExists(template))
            {
                return HttpNotFound();
            }
            return View(template, model);
        }

        protected bool EnsurePhsyicalViewExists(string template)
        {
            var result = ViewEngines.Engines.FindView(ControllerContext, template, null);
            if (result.View == null)
            {
                LogHelper.Warn<DefaultController>("No physical template file was found for template " + template);
                return false;
            }
            return true;
        }
    }
}
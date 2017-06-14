using System;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using Umbraco.Core;
using Umbraco.Core.Security;

namespace OctoFramework.Logic.HttpModules
{
    public class ElmahSecurityModule : IHttpModule
    {
        private HttpApplication _context;

        public void Dispose()
        {
        }

        public void Init(HttpApplication context)
        {
            _context = context;
            _context.BeginRequest += BeginRequest;
        }

        private void BeginRequest(object sender, EventArgs e)
        {
            var handlerPath = string.Empty;
            var systemWebServerSection = (HttpHandlersSection)ConfigurationManager.GetSection("system.web/httpHandlers");

            foreach (HttpHandlerAction handler in systemWebServerSection.Handlers)
            {
                if (handler.Type.Trim() == "Elmah.ErrorLogPageFactory, Elmah")
                {
                    handlerPath = handler.Path.ToLower();
                    break;
                }
            }

            if (string.IsNullOrWhiteSpace(handlerPath) || !_context.Request.Path.ToLower().Contains(handlerPath))
            {
                return;
            }

            var userService = ApplicationContext.Current.Services.UserService;
            var auth = new HttpContextWrapper(HttpContext.Current).GetUmbracoAuthTicket();
            if (auth != null)
            {
                var user = userService.GetByUsername(auth.Name);

                if (user != null)
                {
                    var isAllowed = user != null && user.AllowedSections.Any(x => x.Equals("developer", StringComparison.InvariantCultureIgnoreCase));
                    if (isAllowed)
                    {
                        return;
                    }
                }
            }

            var customErrorsSection = (CustomErrorsSection)ConfigurationManager.GetSection("system.web/customErrors");
            var defaultRedirect = customErrorsSection.DefaultRedirect ?? "/umbraco";
            _context.Context.Response.Redirect(defaultRedirect);
        }
    }
}
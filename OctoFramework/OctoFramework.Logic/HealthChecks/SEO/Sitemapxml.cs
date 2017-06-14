using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Hosting;
using Umbraco.Core.Logging;
using Umbraco.Web.HealthCheck;

namespace OctoFramework.Logic.HealthChecks.SEO
{
    [HealthCheck("72125b16-7cb9-485b-9065-a4e2c6c76625", "Sitemap.xml",
    Description = "Create a Sitemapxml file to help bots index the site.",
    Group = "SEO")]
    public class Sitemapxml : HealthCheck
    {
        public Sitemapxml(HealthCheckContext healthCheckContext) : base(healthCheckContext)
        {
        }

        public override IEnumerable<HealthCheckStatus> GetStatus()
        {
            return new[] { CheckForSiteMapxmlFile() };
        }

        public override HealthCheckStatus ExecuteAction(HealthCheckAction action)
        {
            switch (action.Alias)
            {
                case "addDefaultSitemapxmlFile":
                    return addDefaultSitemapxmlFile();

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private HealthCheckStatus CheckForSiteMapxmlFile()
        {
            var success = File.Exists(HttpContext.Current.Server.MapPath("~/Sitemap.xml"));
            var message = success
                ? string.Format("Sitemap.xml file exists")
                : string.Format("Cannot find sitemap.xml file");
            var actions = new List<HealthCheckAction>();

            if (success == false)
                actions.Add(new HealthCheckAction("addDefaultSitemapxmlFile", Id)
                // Override the "Rectify" button name and describe what this action will do
                {
                    Name = string.Format("Create"),
                    Description = string.Format("This will create a Sitemap.xml file at the root of your site")
                });

            return
                new HealthCheckStatus(message)
                {
                    ResultType = success ? StatusResultType.Success : StatusResultType.Error,
                    Actions = actions
                };
        }

        private HealthCheckStatus addDefaultSitemapxmlFile()
        {
            var success = false;
            var message = string.Format("Sitemap.xml file created!");
            try
            {
                File.WriteAllText(HostingEnvironment.MapPath("~/sitemap.xml"), "");
                success = true;
            }
            catch (Exception exception)
            {
                LogHelper.Error<Sitemapxml>("Could not write sitemap.xml to the root of the site", exception);
            }

            return
                new HealthCheckStatus(message)
                {
                    ResultType = success ? StatusResultType.Success : StatusResultType.Error,
                    Actions = new List<HealthCheckAction>()
                };
        }
    }
}
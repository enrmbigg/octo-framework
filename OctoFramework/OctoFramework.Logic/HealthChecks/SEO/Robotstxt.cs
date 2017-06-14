using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Hosting;
using Umbraco.Core.Logging;
using Umbraco.Web.HealthCheck;

namespace OctoFramework.Logic.HealthChecks.SEO
{
    [HealthCheck("3A482719-3D90-4BC1-B9F8-910CD9CF5B32", "Robots.txt",
    Description = "Create a robots.txt file to block access to system folders.",
    Group = "SEO")]
    public class RobotsTxt : HealthCheck
    {
        public RobotsTxt(HealthCheckContext healthCheckContext) : base(healthCheckContext)
        {
        }

        public override IEnumerable<HealthCheckStatus> GetStatus()
        {
            return new[] { CheckForRobotsTxtFile() };
        }

        public override HealthCheckStatus ExecuteAction(HealthCheckAction action)
        {
            switch (action.Alias)
            {
                case "addDefaultRobotsTxtFile":
                    return AddDefaultRobotsTxtFile();

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private HealthCheckStatus CheckForRobotsTxtFile()
        {
            var request = HttpContext.Current.Request;
            var url = request.Url.Scheme + "://" + request.Url.Authority +
            request.ApplicationPath + "/robots.txt";
            HttpWebResponse response = null;
            var webrequest = (HttpWebRequest)WebRequest.Create(url);
            webrequest.Method = "HEAD";
            try
            {
                response = (HttpWebResponse)webrequest.GetResponse();
            }
            catch (WebException ex)
            {
                /* A WebException will be thrown if the status of the response is not `200 OK` */
                LogHelper.Error<HealthCheck>("Failed to find Robots", ex);
            }
            finally
            {
                // Don't forget to close your response.
                if (response != null)
                {
                    response.Close();
                }
            }
            var success = response.StatusCode == HttpStatusCode.OK;
            var message = success
                ? string.Format("Robots.txt file exists")
                : string.Format("Cannot find robots.txt file");

            var actions = new List<HealthCheckAction>();

            if (success == false)
                actions.Add(new HealthCheckAction("addDefaultRobotsTxtFile", Id)
                {
                    Name = string.Format("Create"),
                    Description = string.Format("This will create a Robots.txt file at the root of your site")
                });
            return
                new HealthCheckStatus(message)
                {
                    ResultType = success ? StatusResultType.Success : StatusResultType.Error,
                    Actions = actions
                };
        }

        private HealthCheckStatus AddDefaultRobotsTxtFile()
        {
            var success = false;
            var message = string.Format("Robots.txt file created!");
            const string content = @"# robots.txt for Umbraco
User-agent: *
Disallow: /umbraco/";

            try
            {
                File.WriteAllText(HostingEnvironment.MapPath("~/robots.txt"), content);
                success = true;
            }
            catch (Exception exception)
            {
                LogHelper.Error<RobotsTxt>("Could not write robots.txt to the root of the site", exception);
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
using System.Collections.Generic;
using System.Xml;
using Umbraco.Core.IO;
using Umbraco.Web.HealthCheck;

namespace OctoFramework.Logic.HealthChecks.ClientDependency
{
    [HealthCheck("B806521A-C82C-4135-86EC-7E3CBC195FD3", "Client Dependency Version Number", Description = "Change the Client Dependency framwork version number", Group = "Client Dependency")]
    public class ClientDependency : HealthCheck
    {
        public ClientDependency(HealthCheckContext healthCheckContext) : base(healthCheckContext)
        {
        }

        public override HealthCheckStatus ExecuteAction(HealthCheckAction action)
        {
            if (action.Alias == "Increment")
                return IncrementCdf();

            return new HealthCheckStatus("Unknown")
            {
                ResultType = StatusResultType.Error,
                Description = $"Unknown Action {action.Alias}"
            };
        }

        public override IEnumerable<HealthCheckStatus> GetStatus()
        {
            return new List<HealthCheckStatus> { GetCdfVersion() };
        }

        private HealthCheckStatus GetCdfVersion()
        {
            var path = IOHelper.MapPath("~/config/clientdependency.config");
            var xmlDocument = new XmlDocument { PreserveWhitespace = true };
            xmlDocument.Load(path);

            var node = xmlDocument.SelectSingleNode("//clientDependency/@version");
            if (node == null) return new HealthCheckStatus("Failed to load cdf version");

            if (!int.TryParse(node.Value, out var version))
                return new HealthCheckStatus("Failed to load cdf version");
            var message = $"CDF Version: {version}";

            var status = new HealthCheckStatus(message)
            {
                ResultType = StatusResultType.Info,
                Description = $"The client dependency framework is running on version {version}, increment this number to generate a new set of cached files",
                Actions = new List<HealthCheckAction>
                {
                    new HealthCheckAction("Increment", Id)
                    {
                        Name = "Increment"
                    }
                }
            };
            return status;
        }

        private static HealthCheckStatus IncrementCdf()
        {
            var path = IOHelper.MapPath("~/config/clientdependency.config");
            var xmlDocument = new XmlDocument { PreserveWhitespace = true };
            xmlDocument.Load(path);

            var node = xmlDocument.SelectSingleNode("//clientDependency/@version");
            if (node == null)
            {
                return new HealthCheckStatus("Failed")
                {
                    ResultType = StatusResultType.Error
                };
            }

            if (int.TryParse(node.Value, out var version))
            {
                node.Value = (version + 1).ToString();
            }

            xmlDocument.Save(path);

            return new HealthCheckStatus($"Incremented to {version + 1}");
        }
    }
}
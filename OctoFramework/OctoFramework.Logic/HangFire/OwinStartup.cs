using OctoFramework.Logic.Tasks;
using Hangfire;
using Hangfire.Dashboard;
using Owin;
using Umbraco.Web;

namespace OctoFramework.Logic.HangFire
{
    public class OwinStartup : UmbracoDefaultOwinStartup
    {
        public override void Configuration(IAppBuilder app)
        {
            base.Configuration(app);

            GlobalConfiguration.Configuration.UseSqlServerStorage("umbracoDbDSN");

            var dashboardOptions = new DashboardOptions
            {
                Authorization = new[]
                {
                    new HangFireAuthorizationFilter()
                }
            };

            app.UseHangfireDashboard("/umbraco/hangfire", dashboardOptions);
            app.UseHangfireServer();

            RecurringJob.AddOrUpdate("LinkChecker", () => HttpTasks.RequestUrl("/Umbraco/Api/LinkCheckerTask/CheckPage/1048"), Cron.Hourly(30));
        }
    }
}
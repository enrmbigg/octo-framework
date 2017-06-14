using System.Web.Mvc;
using OctoFramework.Logic.Cache;
using OctoFramework.Logic.Models.ViewModels;
using Umbraco.Web.Models;

namespace OctoFramework.Logic.Controllers
{
    public class HomeController : DefaultController
    {
        [UmbracoOutputCache]
        public override ActionResult Index(RenderModel model)
        {
            var homeModel = new HomeViewModel(model.Content);
            return CurrentTemplate(homeModel);
        }
    }
}
using System.Web.Mvc;
using OctoFramework.Logic.Cache;
using OctoFramework.Logic.Models.ViewModels;
using Umbraco.Web;
using Umbraco.Web.Models;

namespace OctoFramework.Logic.Controllers
{
    public class BlogHomeController : DefaultController
    {
        public BlogHomeController()
        {
        }

        public BlogHomeController(UmbracoContext context) : base(context)
        {
        }

        public BlogHomeController(UmbracoContext context, UmbracoHelper helper) : base(context, helper)
        {
        }

        [UmbracoOutputCache]
        public new ActionResult Index(RenderModel model)
        {
            var blogHomeModel = new BlogHomeViewModel(model.Content);
            return View("BlogHome", blogHomeModel);
        }
    }
}
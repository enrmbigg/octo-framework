using System.Web.Mvc;
using OctoFramework.Logic.Cache;
using OctoFramework.Logic.Models.ViewModels;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;

namespace OctoFramework.Logic.Controllers
{
    public class ContactController : RenderMvcController
    {
        [UmbracoOutputCache]
        public override ActionResult Index(RenderModel model)
        {
            var contactViewModel = new ContactViewModel(model.Content);
            return CurrentTemplate(contactViewModel);
        }
    }
}
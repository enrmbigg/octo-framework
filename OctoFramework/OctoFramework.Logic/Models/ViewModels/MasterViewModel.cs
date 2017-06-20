using System.Web;
using OctoFramework.Logic.Models.Custom;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace OctoFramework.Logic.Models.ViewModels
{
    public class MasterViewModel
    {
        public MasterViewModel(IPublishedContent content)
        {
            Content = content;
        }

        internal UmbracoHelper Umbraco = new UmbracoHelper(UmbracoContext.Current);
        public IPublishedContent Content;
        public HomeViewModel Home { get { return new HomeViewModel(Content.AncestorOrSelf(1)); } }
        public SeoMetaData SEO { get { return Content.GetPropertyValue<SeoMetaData>("SEO", null); } }
        public string PageIntro { get { return Content.GetPropertyValue<string>("pageIntro", ""); } }
        public Image PageImage { get { return new Image(Umbraco.TypedMedia(Content.GetPropertyValue<string>("pageImage"))); } }
        public HtmlString BodyContent { get { return Content.GetPropertyValue("bodyContent", new HtmlString("")); } }
        public string SocialTitle { get { return Content.GetPropertyValue<string>("openGraphTitle", ""); } }
        public string SocialDescription { get { return Content.GetPropertyValue<string>("openGraphDescription", ""); } }
        public Image SocialImage { get { return new Image(Umbraco.TypedMedia(Content.GetPropertyValue<string>("openGraphDefaultImage"))); } }
    }
}
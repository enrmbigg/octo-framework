using System;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace OctoFramework.Logic.Models.ViewModels
{
    public partial class BlogPostViewModel : MasterViewModel
    {
        public BlogPostViewModel(IPublishedContent content) : base(content)
        {
        }

        public DateTime ArticlePublishedDate { get { return Content.GetPropertyValue<DateTime>("createTime", Content.CreateDate); } }
        public string[] BlogTags { get { return Content.GetPropertyValue<string>("tag", "").Split(','); } }
        public string Description { get { return Content.GetPropertyValue<string>("pageIntro"); } }
        public DateTime PostCreation { get { return Content.GetPropertyValue<DateTime>("createTime"); } }
        public HtmlString Markdown { get { return Content.GetPropertyValue<HtmlString>("markdown"); } }
    }
}
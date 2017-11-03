using System;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace OctoFramework.Logic.Models.ViewModels
{
    public class BlogPostViewModel : MasterViewModel
    {
        public BlogPostViewModel(IPublishedContent content) : base(content)
        {
            Content = content;
        }

        public DateTime ArticlePublishedDate => Content.GetPropertyValue("createTime", Content.CreateDate);
        public string[] BlogTags { get; set; }
        public string Description => Content.GetPropertyValue<string>("pageIntro");
        public DateTime PostCreation => Content.GetPropertyValue<DateTime>("createTime");
        public HtmlString Markdown => Content.GetPropertyValue<HtmlString>("markdown");
        public bool ShowComments { get; set; }
    }
}
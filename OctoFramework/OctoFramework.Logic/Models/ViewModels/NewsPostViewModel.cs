using System;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace OctoFramework.Logic.Models.ViewModels
{
    public partial class NewsPostViewModel : MasterViewModel
    {
        public NewsPostViewModel(IPublishedContent content) : base(content)
        {
        }

        public DateTime ArticlePublishedDate { get { return Content.GetPropertyValue("createTime", Content.CreateDate); } }
        public string[] NewsTags { get { return Content.GetPropertyValue<string>("tag", "").Split(','); } }
        public string Description { get { return Content.GetPropertyValue<string>("pageIntro"); } }
        public DateTime PostCreation { get { return Content.GetPropertyValue<DateTime>("createTime"); } }
    }
}
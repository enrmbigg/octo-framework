using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace OctoFramework.Logic.Models.ViewModels
{
    public partial class HomeViewModel : MasterViewModel
    {
        public HomeViewModel(IPublishedContent content) : base(content)
        {
        }

        public IEnumerable<BlogPostViewModel> BlogPosts { get { return Umbraco.TypedContentAtXPath("//blogPost").OrderByDescending(x => x.GetPropertyValue<DateTime>("createTime")).Take(4).Select(x => new BlogPostViewModel(x)); } }
    }
}
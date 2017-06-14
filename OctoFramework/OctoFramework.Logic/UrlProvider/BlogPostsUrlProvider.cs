using System;
using System.Collections.Generic;
using System.Linq;
using OctoFramework.Logic.Models.ViewModels;
using Umbraco.Web;
using Umbraco.Web.Routing;

namespace OctoFramework.Logic.UrlProvider
{
    public class BlogPostsUrlProvider : IUrlProvider
    {
        public string GetUrl(UmbracoContext umbracoContext, int id, Uri current, UrlProviderMode mode)
        {
            var content = umbracoContext.ContentCache.GetById(id);
            if (content.DocumentTypeAlias == "blogPost")
            {
                var model = new BlogPostViewModel(content);
                if (model != null)
                {
                    var date = model.PostCreation;
                    if (date != null)
                    {
                        //This will add the selected date before the node name.
                        //For example /blog/item1/ becomes /blog/28-07-2014/item1/.
                        var url = model.Content.Parent.Url;
                        if (!(url.EndsWith("/")))
                        {
                            url += "/";
                        }
                        var publishUrl = url + date.ToString("dd-MM-yyyy") + "/" + content.UrlName + "/";
                        return publishUrl;
                    }
                }
            }

            return null;
        }

        public IEnumerable<string> GetOtherUrls(UmbracoContext umbracoContext, int id, Uri current)
        {
            return Enumerable.Empty<string>();
        }
    }
}
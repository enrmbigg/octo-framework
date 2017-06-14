using System;
using System.Collections.Generic;
using System.Linq;
using OctoFramework.Logic.Constants;
using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Core.Logging;
using Umbraco.Web;
using Umbraco.Web.Routing;

namespace OctoFramework.Logic.ContentFinder
{
    /// <summary>
    /// The multilingual content finder.
    /// </summary>
    public class MultilingualContentFinder : IContentFinder
    {
        /// <summary>
        /// The try find content.
        /// </summary>
        /// <param name="contentRequest">
        /// The content request.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool TryFindContent(PublishedContentRequest contentRequest)
        {
            if (contentRequest == null)
            {
                return false;
            }

            try
            {
                using (ApplicationContext.Current.ProfilingLogger.TraceDuration<MultilingualContentFinder>("Started TryFindContent", "Completed TryFindContent"))
                {
                    var helper = new UmbracoHelper(UmbracoContext.Current);
                    var urls =
                        ApplicationContext.Current.ApplicationCache.RuntimeCache.GetCacheItem<List<Tuple<int, string>>>(
                            "MultilingualContentFinder-Urls",
                        () =>
                        {
                            var contentUrls = new List<Tuple<int, string>>();

                            // Get all the nodes in the website.
                            var home = helper.TypedContentSingleAtXPath("//" + DocumentTypes.HomeAlias);
                            var allNodes = home.DescendantsOrSelf().ToList();
                            //Add home
                            allNodes.Add(home);

                            foreach (var node in allNodes)
                            {
                                // Get all the urls in the website.
                                // With UrlProvider.GetOtherUrls we also get the urls of the other languages.
                                var temp = new List<Tuple<int, string>>
                                {
                                    // if its the current domain make it relative Url
                                    new Tuple<int, string>(node.Id, node.Url)
                                };
                                temp.AddRange(UmbracoContext.Current.UrlProvider.GetOtherUrls(node.Id).Select(x => new Tuple<int, string>(node.Id, x)));
                                contentUrls.AddRange(temp);
                            }

                            return contentUrls;
                        });

                    if (urls.Any())
                    {
                        // Get the current url without querystring.
                        var url = RemoveQueryFromUrl(contentRequest.Uri.ToString()).EnsureEndsWith("/");

                        var currentUrlItem = urls.FirstOrDefault(x => url.InvariantEquals(x.Item2));

                        if (currentUrlItem != null)
                        {
                            var contentItem = UmbracoContext.Current.ContentCache.GetById(currentUrlItem.Item1);

                            if (contentItem != null)
                            {
                                contentRequest.PublishedContent = contentItem;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error<MultilingualContentFinder>("Error in contenfinder MultilingualContentFinder", ex);
            }

            return contentRequest.PublishedContent != null;
        }

        /// <summary>
        /// Remove the querystring from a url.
        /// </summary>
        /// <param name="url">
        /// The url.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string RemoveQueryFromUrl(string url)
        {
            var index = url.IndexOf('?');

            return index > 0 ? url.Substring(0, index) : url;
        }
    }
}
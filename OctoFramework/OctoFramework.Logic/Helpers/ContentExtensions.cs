using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Strings;

namespace OctoFramework.Logic.Helpers
{
    public static class ContentExtensions
    {
        public static string GetUrlSegment(this IPublishedContent content, string culture)
        {
            var urlSegment = "";//content.GetVortoValue<string>("urlSegment", culture);

            if (string.IsNullOrEmpty(urlSegment))
            {
                urlSegment = content.UrlName;
            }

            return urlSegment.ToUrlSegment().EnsureEndsWith("/");
        }

        /// <summary>
        /// Gets the default url segment for a specified content.
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        /// <returns>
        /// The url segment.
        /// </returns>
        public static string GetUrlSegment(this IContentBase content)
        {
            var url = UrlSegmentProviders.Select(p => p.GetUrlSegment(content)).First(u => u != null);
            url = url ?? new DefaultUrlSegmentProvider().GetUrlSegment(content); // be safe
            return url;
        }

        /// <summary>
        /// Gets the url segment for a specified content and culture.
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        /// <param name="culture">
        /// The culture.
        /// </param>
        /// <returns>
        /// The url segment.
        /// </returns>
        public static string GetUrlSegment(this IContentBase content, CultureInfo culture)
        {
            var url = UrlSegmentProviders.Select(p => p.GetUrlSegment(content, culture)).First(u => u != null);
            url = url ?? new DefaultUrlSegmentProvider().GetUrlSegment(content, culture); // be safe
            return url;
        }

        private static IEnumerable<IUrlSegmentProvider> UrlSegmentProviders
        {
            get
            {
                return UrlSegmentProviderResolver.HasCurrent
                           ? UrlSegmentProviderResolver.Current.Providers
                           : new IUrlSegmentProvider[] { new DefaultUrlSegmentProvider() };
            }
        }
    }
}
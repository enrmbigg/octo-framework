using System.Configuration;
using System.Globalization;
using Newtonsoft.Json;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Strings;
using OctoFramework.Logic.Models.Custom;

namespace OctoFramework.Logic.UrlProvider
{
    public class SeoMetaDataUrlSegmentProvider : IUrlSegmentProvider
    {
        private static readonly string PropertyName = "metadata";

        static SeoMetaDataUrlSegmentProvider()
        {
            PropertyName = "EO101.PropertyName";
        }

        public string GetUrlSegment(IContentBase content)
        {
            if (!content.HasProperty(PropertyName))
                return null;
            try
            {
                var seoMetadata = JsonConvert.DeserializeObject<SeoMetaData>(content.GetValue<string>(PropertyName));
                return string.IsNullOrWhiteSpace(seoMetadata?.UrlName) ? null : seoMetadata.UrlName.ToUrlSegment();
            }
            catch
            {
                return null;
            }
        }

        public string GetUrlSegment(IContentBase content, CultureInfo culture)
        {
            return GetUrlSegment(content);
        }
    }
}
using System;
using Newtonsoft.Json;
using Umbraco.Core.Logging;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;
using OctoFramework.Logic.Models.Custom;

namespace OctoFramework.Logic.PropertyValueConverter
{
    [PropertyValueType(typeof(SeoMetaData))]
    [PropertyValueCache(PropertyCacheValue.All, PropertyCacheLevel.Content)]
    public class SeoMetaDataConverter : PropertyValueConverterBase
    {
        public override bool IsConverter(PublishedPropertyType propertyType)
        {
            return propertyType.PropertyEditorAlias != null && propertyType.PropertyEditorAlias.Equals("SEO101");
        }

        public override object ConvertDataToSource(PublishedPropertyType propertyType, object source, bool preview)
        {
            var sourceString = source?.ToString();
            if (string.IsNullOrWhiteSpace(sourceString)) return null;

            try
            {
                var md = JsonConvert.DeserializeObject<SeoMetaData>(sourceString);

                if (!string.IsNullOrWhiteSpace(md.Tags))
                {
                    md.TagsArray = md.Tags.Split(',');
                }
                return md;
            }
            catch (Exception e)
            {
                LogHelper.Warn<SeoMetaDataConverter>($"Cannot deserialize SeoMetaData - {e.GetType().Name} - {e.Message}");
                return null;
            }
        }
    }
}

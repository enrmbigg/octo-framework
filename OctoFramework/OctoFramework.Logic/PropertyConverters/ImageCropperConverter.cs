using System;
using Newtonsoft.Json;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web.Models;

namespace OctoFramework.Logic.PropertyConverters
{
    public class ImageCropperConverter : PropertyValueConverterBase, IPropertyValueConverterMeta
    {
        public override object ConvertSourceToObject(PublishedPropertyType propertyType, object source, bool preview)
        {
            if (source != null)
            {
                var json = source.ToString();
                if (json.DetectIsJson())
                {
                    try
                    {
                        return JsonConvert.DeserializeObject<ImageCropDataSet>(json);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error(typeof(ImageCropperConverter), "Could not parse the json string: " + json, ex);
                    }
                }
            }

            return null;
        }

        public override bool IsConverter(PublishedPropertyType propertyType)
        {
            return Umbraco.Core.Constants.PropertyEditors.ImageCropperAlias.InvariantEquals(propertyType.PropertyEditorAlias);
        }

        public PropertyCacheLevel GetPropertyCacheLevel(PublishedPropertyType propertyType, PropertyCacheValue cacheValue)
        {
            return PropertyCacheLevel.Content;
        }

        public Type GetPropertyValueType(PublishedPropertyType propertyType)
        {
            return typeof(ImageCropDataSet);
        }
    }
}
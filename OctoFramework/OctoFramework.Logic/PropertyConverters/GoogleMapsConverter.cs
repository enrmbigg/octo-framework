using System;
using System.Globalization;
using OctoFramework.Logic.Models.Custom;
using Umbraco.Core;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;

namespace OctoFramework.Logic.PropertyConverters
{
    public class GoogleMapsConverter : PropertyValueConverterBase, IPropertyValueConverterMeta
    {
        public override object ConvertSourceToObject(PublishedPropertyType propertyType, object source, bool preview)
        {
            if (source != null && !string.IsNullOrWhiteSpace(source.ToString()))
            {
                var coordinates = source.ToString().Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (coordinates.Length == 3)
                {
                    return new GoogleMaps
                    {
                        Lat = double.Parse(coordinates[0], CultureInfo.InvariantCulture),
                        Lng = double.Parse(coordinates[1], CultureInfo.InvariantCulture),
                        Zoom = int.Parse(coordinates[2], CultureInfo.InvariantCulture)
                    };
                }
            }

            return null;
        }

        public override bool IsConverter(PublishedPropertyType propertyType)
        {
            return "AngularGoogleMaps".InvariantEquals(propertyType.PropertyEditorAlias);
        }

        public PropertyCacheLevel GetPropertyCacheLevel(PublishedPropertyType propertyType, PropertyCacheValue cacheValue)
        {
            return PropertyCacheLevel.Content;
        }

        public Type GetPropertyValueType(PublishedPropertyType propertyType)
        {
            return typeof(GoogleMaps);
        }
    }
}
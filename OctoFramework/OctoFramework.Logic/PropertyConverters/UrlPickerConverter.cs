using System;
using Newtonsoft.Json;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web;

namespace OctoFramework.Logic.Converters
{
    /// <summary>
    /// Summary description for E360 UrlPicker
    /// </summary>

    public class UrlPicker
    {
        public UrlPicker.UrlPickerTypes Type { get; set; }

        public Meta Meta { get; set; }

        public TypeData TypeData { get; set; }

        public string Url { get; set; }

        public string Name { get; set; }

        public enum UrlPickerTypes
        {
            Url,
            Content,
            Media,
            Email
        }
    }

    public class TypeData
    {
        public string Url { get; set; }

        public string Email { get; set; }

        public int? ContentId { get; set; }

        public IPublishedContent Content { get; set; }

        public int? MediaId { get; set; }

        public IPublishedContent Media { get; set; }
    }

    public class Meta
    {
        public string Title { get; set; }

        public bool NewWindow { get; set; }
    }

    public class LinkInfo
    {
        public string LinkCaption { get; set; }

        public string LinkTitle { get; set; }

        public string LinkURL { get; set; }

        public string LinkTarget { get; set; }

        public string LinkIcon { get; set; }

        public UrlPicker.UrlPickerTypes LinkType { get; set; }

        public IPublishedContent LinkUmbracoNode { get; set; }

        public LinkInfo()
        {
            LinkCaption = string.Empty;
            LinkTitle = string.Empty;
            LinkURL = string.Empty;
            LinkTarget = string.Empty;
            LinkIcon = string.Empty;
            LinkUmbracoNode = null;
        }
    }

    [PropertyValueType(typeof(UrlPicker))]
    [PropertyValueCache(PropertyCacheValue.All, PropertyCacheLevel.Content)]
    public class UrlPickerValueConverter : PropertyValueConverterBase
    {
        internal readonly UmbracoHelper umbracoHelper = new UmbracoHelper(UmbracoContext.Current);

        public override bool IsConverter(PublishedPropertyType propertyType)
        {
            return propertyType.PropertyEditorAlias.Equals("E360.UrlPicker");
        }

        public override object ConvertDataToSource(PublishedPropertyType propertyType, object source, bool preview)
        {
            if (source == null)
            {
                return new UrlPicker();
            }

            var input = source.ToString();
            if (!DetectIsJson(input))
            {
                return input;
            }
            try
            {
                UrlPicker urlPicker = JsonConvert.DeserializeObject<UrlPicker>(input);

                if (urlPicker.TypeData.ContentId.HasValue)
                    urlPicker.TypeData.Content = umbracoHelper.TypedContent(urlPicker.TypeData.ContentId);
                if (urlPicker.TypeData.MediaId.HasValue)
                    urlPicker.TypeData.Media = umbracoHelper.TypedMedia(urlPicker.TypeData.MediaId);
                switch (urlPicker.Type)
                {
                    case UrlPicker.UrlPickerTypes.Content:
                        if (urlPicker.TypeData.Content != null)
                        {
                            urlPicker.Url = urlPicker.TypeData.Content.Url;
                            urlPicker.Name = string.IsNullOrWhiteSpace(urlPicker.Meta.Title) ? urlPicker.TypeData.Content.Name : urlPicker.Meta.Title;
                            break;
                        }
                        break;

                    case UrlPicker.UrlPickerTypes.Media:
                        if (urlPicker.TypeData.Media != null)
                        {
                            urlPicker.Url = urlPicker.TypeData.Media.Url;
                            urlPicker.Name = string.IsNullOrWhiteSpace(urlPicker.Meta.Title) ? urlPicker.TypeData.Media.Name : urlPicker.Meta.Title;
                            break;
                        }
                        break;

                    case UrlPicker.UrlPickerTypes.Email:
                        if (urlPicker.TypeData.Email != null)
                        {
                            urlPicker.Url = "mailto:" + urlPicker.TypeData.Email;
                            urlPicker.Name = string.IsNullOrWhiteSpace(urlPicker.Meta.Title) ? urlPicker.TypeData.Email : urlPicker.Meta.Title;
                            break;
                        }
                        break;

                    default:
                        urlPicker.Url = urlPicker.TypeData.Url;
                        urlPicker.Name = string.IsNullOrWhiteSpace(urlPicker.Meta.Title) ? urlPicker.TypeData.Url : urlPicker.Meta.Title;
                        break;
                }
                return urlPicker;
            }
            catch (Exception ex)
            {
                LogHelper.Error<UrlPickerValueConverter>(ex.Message, ex);
                return new UrlPicker();
            }
        }

        private static bool DetectIsJson(string input)
        {
            input = input.Trim();
            if (input.StartsWith("{") && input.EndsWith("}"))
                return true;
            if (input.StartsWith("["))
                return input.EndsWith("]");
            return false;
        }
    }
}
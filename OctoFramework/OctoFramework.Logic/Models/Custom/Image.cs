using Umbraco.Core.Models;
using Umbraco.Web;

namespace OctoFramework.Logic.Models.Custom
{
    public class Image
    {
        public Image(IPublishedContent image)
        {
            if (image != null)
            {
                Name = image.Name;
                Url = image.Url;
                AltText = image.GetPropertyValue<string>("altText", Name);
                Node = image;
                Width = image.GetPropertyValue<int>("umbracoWidth");
                Height = image.GetPropertyValue<int>("umbracoHeight");
                ThumbnailUrl = image.GetCropUrl("Thumbnail");
                BannerUrl = image.GetCropUrl("Banner");
            }
        }

        public string Name { get; set; }
        public string Url { get; set; }
        public string ThumbnailUrl { get; set; }
        public string AltText { get; set; }
        public IPublishedContent Node { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string BannerUrl { get; set; }
    }
}
namespace OctoFramework.Logic.Models.Custom
{
    public class SeoMetaData
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string Index { get; set; }

        public string UrlName { get; set; }

        public Sitemap Sitemap { get; set; }

        public string Url { get; set; }

        public string Tags { get; set; }

        public string[] TagsArray { get; set; }
    }
}
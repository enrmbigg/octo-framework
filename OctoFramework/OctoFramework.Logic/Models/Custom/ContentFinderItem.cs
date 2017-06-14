using Umbraco.Core.Models;

namespace OctoFramework.Logic.Models.Custom
{
    public class ContentFinderItem
    {
        public string Template { get; set; }
        public IPublishedContent Content { get; set; }
    }
}
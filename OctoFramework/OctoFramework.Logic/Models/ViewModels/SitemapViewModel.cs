using Umbraco.Core.Models;

namespace OctoFramework.Logic.Models.ViewModels
{
    public partial class SitemapViewModel : MasterViewModel
    {
        public SitemapViewModel(IPublishedContent content) : base(content)
        {
        }
    }
}
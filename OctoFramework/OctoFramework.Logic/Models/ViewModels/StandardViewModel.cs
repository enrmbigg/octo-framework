using Umbraco.Core.Models;

namespace OctoFramework.Logic.Models.ViewModels
{
    public partial class StandardViewModel : MasterViewModel
    {
        public StandardViewModel(IPublishedContent content) : base(content)
        {
        }
    }
}
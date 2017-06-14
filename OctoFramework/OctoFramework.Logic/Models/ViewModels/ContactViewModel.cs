using Umbraco.Core.Models;

namespace OctoFramework.Logic.Models.ViewModels
{
    public partial class ContactViewModel : MasterViewModel
    {
        public ContactViewModel(IPublishedContent content) : base(content)
        {
        }
    }
}
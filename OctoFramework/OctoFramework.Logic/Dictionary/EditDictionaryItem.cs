using Umbraco.Core;

namespace OctoFramework.Logic.Dictionary
{
    public class EditDictionaryItem : umbraco.settings.EditDictionaryItem
    {
        public EditDictionaryItem()
        {
            CurrentApp = Umbraco.Core.Constants.Applications.Translation;
        }
    }
}
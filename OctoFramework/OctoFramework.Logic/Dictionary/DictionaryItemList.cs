using System;
using Umbraco.Core;

namespace OctoFramework.Logic.Dictionary
{
    public class DictionaryItemList : umbraco.presentation.settings.DictionaryItemList
    {
        public DictionaryItemList()
        {
            CurrentApp = Umbraco.Core.Constants.Applications.Translation;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            lt_table.Text = lt_table.Text.Replace("../", "/umbraco/");
        }
    }
}
using umbraco;
using umbraco.businesslogic;
using umbraco.cms.presentation.Trees;
using Umbraco.Core;

namespace OctoFramework.Logic.Dictionary
{
    [Tree(Umbraco.Core.Constants.Applications.Translation, "dictionary2", "Dictionary")]
    public class DictionaryTree : loadDictionary
    {
        public DictionaryTree(string application) : base(application)
        {
        }

        protected override void CreateRootNode(ref XmlTreeNode rootNode)
        {
            base.CreateRootNode(ref rootNode);

            rootNode.Action = "UmbClientMgr.contentFrame('../app_plugins/Dictionary/DictionaryItemList.aspx');";
        }

        public override void Render(ref XmlTree tree)
        {
            base.Render(ref tree);

            foreach (XmlTreeNode node in tree)
            {
                node.Action = string.Format("javascript:UmbClientMgr.contentFrame('../app_plugins/Dictionary/EditDictionaryItem.aspx?id={0}');", node.NodeID);
            }
        }
    }
}
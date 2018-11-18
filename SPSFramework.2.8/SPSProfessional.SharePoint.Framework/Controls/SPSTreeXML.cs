using System;
using System.Data;
using System.Diagnostics;
using System.Web.UI.WebControls;
using System.Xml.Serialization;
using SPSProfessional.SharePoint.Framework.Controls;

namespace SPSProfessional.SharePoint.Framework.Controls
{
    [Serializable]
    [XmlRoot("SPSTree")]
    public class SPSTreeXML
    {
        private const string FIELD_SEPARATOR = ",";

        private SPSBranchItem[] _branchItems;
        private SPSLeafItem _leafItem;
        private int _expandDepth;
        private bool _expandAll;
        private TreeViewImageSet _imageSet;
        private int _nodeIndent;
        private bool _showLines;
        private bool _showExpandCollapse;
        private char _pathSeparator;

        #region Properties

        [XmlElementAttribute("SPSBranchItem")]
        public SPSBranchItem[] BranchItems
        {
            get { return _branchItems; }
            set { _branchItems = value; }
        }

        [XmlElement("SPSLeafItem")]
        public SPSLeafItem LeafItem
        {
            get { return _leafItem; }
            set { _leafItem = value; }
        }

        [XmlAttribute]
        public int ExpandDepth
        {
            get { return _expandDepth; }
            set { _expandDepth = value; }
        }

        [XmlAttribute]
        public bool ExpandAll
        {
            get { return _expandAll; }
            set { _expandAll = value; }
        }

        [XmlAttribute]
        public TreeViewImageSet ImageSet
        {
            get { return _imageSet; }
            set { _imageSet = value; }
        }

        [XmlAttribute]
        public int NodeIndent
        {
            get { return _nodeIndent; }
            set { _nodeIndent = value; }
        }

        [XmlAttribute]
        public bool ShowLines
        {
            get { return _showLines; }
            set { _showLines = value; }
        }

        [XmlAttribute]
        public bool ShowExpandCollapse
        {
            get { return _showExpandCollapse; }
            set { _showExpandCollapse = value; }
        }

        [XmlAttribute]
        public char PathSeparator
        {
            get { return _pathSeparator; }
            set { _pathSeparator = value; }
        }

        #endregion

        public SPSTreeXML()
        {
            BranchItems = new SPSBranchItem[] { };
            LeafItem = new SPSLeafItem();
            ExpandAll = true;
            NodeIndent = 2;
            ShowLines = false;
            ShowExpandCollapse = true;
            ImageSet = TreeViewImageSet.Msdn;
        }

        private string SortData()
        {
            string sort = string.Empty;

            foreach (SPSBranchItem item in BranchItems)
            {
                if (!string.IsNullOrEmpty(item.FieldName))
                    sort += item.FieldName + FIELD_SEPARATOR;
            }

            return sort.TrimEnd(',');
        }


        /// <summary>
        /// Fills the specified tree view.
        /// </summary>
        /// <param name="treeView">The tree view.</param>
        /// <param name="dv">The dv.</param>
        public void Fill(TreeView treeView, DataView dv)
        {
            treeView.Nodes.Clear();            

            dv.Sort = SortData();
            dv.BeginInit();

            TreeNodeCollection nodes = treeView.Nodes;

            int startLevel = 0;

            int row = 0;

            do
            {

                DataRowView drv = dv[row];
                string currentKey = GenerateKeyForLevelBreak(drv);


                // Get Root Collection
                if (nodes == null)
                {
                    nodes = treeView.Nodes;
                }

                // Add tree nodes

                TreeNode node = null;

                for (int level = startLevel; level < BranchItems.Length; level++)
                {
                    string field = BranchItems[level].FieldName;

                    node = new TreeNode
                               {
                                       Text = string.Format("</a>{0}<a href=\"#\">", drv[field]),
                                       Value = drv["_RowNumber"].ToString(),
                                       NavigateUrl = "javascript:void(0);"
                               };

                    if (!string.IsNullOrEmpty(BranchItems[level].ImageUrl))
                    {
                        node.ImageUrl = BranchItems[level].ImageUrl;
                    }
                    
                    nodes.Add(node);
                    nodes = node.ChildNodes;
                    //currentKey += drv[field];
                }

                // Add leaf nodes

                TreeNode leafNode = null;

               
                    do
                    {
                        string field = LeafItem.FieldName;

                        leafNode = new TreeNode(drv[field].ToString())
                                   {
                                           Value = drv["_RowNumber"].ToString()
                                   };

                        if (!string.IsNullOrEmpty(LeafItem.ImageUrl))
                        {
                            leafNode.ImageUrl = LeafItem.ImageUrl;
                        }
                        
                        nodes.Add(leafNode);

                        row++;

                        if (row >= dv.Count)
                            break;

                        drv = dv[row];
                        Debug.WriteLine(currentKey + " -> " + GenerateKeyForLevelBreak(drv));
                    }
                    while (currentKey == GenerateKeyForLevelBreak(drv));
                

                string checkKey = string.Empty;
                int breakLevel = 0;
                TreeNode parentNode = leafNode.Parent;

                // Get BreakLevel 
                // Search level where break
                for (int level = 0; level < BranchItems.Length; level++)
                {
                    checkKey += drv[BranchItems[level].FieldName];

                    if (currentKey.StartsWith(checkKey))
                    {
                        breakLevel++;
                    }
                }


                // Parent Node
                // Search the parent of break
                startLevel = breakLevel;

                while (breakLevel != 0 && parentNode.Parent != null)
                {
                    parentNode = parentNode.Parent;
                }

                // Get Start Collection
                if (startLevel == 0)
                {
                    // Root level
                    nodes = null;
                }
                else
                {
                    // Other level
                    nodes = parentNode.ChildNodes;
                }
            }
            while (row < dv.Count);

            if (ExpandAll)
                treeView.ExpandAll();
        }

        /// <summary>
        /// Decorates the specified tree view.
        /// </summary>
        /// <param name="treeView">The tree view.</param>
        public void Decorate(TreeView treeView)
        {
            treeView.ImageSet = ImageSet;
            treeView.ExpandDepth = ExpandDepth;
            treeView.ShowLines = ShowLines;
            treeView.ShowExpandCollapse = ShowExpandCollapse;
            treeView.PathSeparator = PathSeparator;
        }

        /// <summary>
        /// Generates the key for level break.
        /// </summary>
        /// <param name="drv">The DRV.</param>
        /// <returns>The break key</returns>
        private string GenerateKeyForLevelBreak(DataRowView drv)
        {
            string key = string.Empty;
            for (int level = 0; level < BranchItems.Length; level++)
            {
                key += drv[BranchItems[level].FieldName];
            }
            return key;
        }
    }
}
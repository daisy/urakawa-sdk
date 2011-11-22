using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using urakawa.core;

namespace urakawa.daisy.export
{
    partial class Daisy3_Export
    {

        protected virtual void CreateXhtmlDocuments()
        {
            m_ListOfLevels = new List<TreeNode>();
            Dictionary<string, string> old_New_IDMap = new Dictionary<string, string>();

TreeNode rNode = m_Presentation.RootNode;
            //XmlNode bookNode = XmlDocumentHelper.GetFirstChildElementWithName(DTBookDocument, true, "book", null); //DTBookDocument.GetElementsByTagName("book")[0];

            m_ListOfLevels.Add(m_Presentation.RootNode);

            //m_TreeNode_XmlNodeMap.Add(rNode, bookNode);
            XmlNode currentXmlNode = null;
            bool isHeadingNodeAvailable = true;

            rNode.AcceptDepthFirst(
                    delegate(TreeNode n)
                    {
                        if (RequestCancellation) return false;
                        
                        if (doesTreeNodeTriggerNewSmil(n))
                        {
                            if (!isHeadingNodeAvailable && m_ListOfLevels.Count > 1)
                            {
                                m_ListOfLevels.RemoveAt(m_ListOfLevels.Count - 1);
                                //System.Windows.Forms.MessageBox.Show ( "removing :" + m_ListOfLevels.Count.ToString () );
                            }
                            m_ListOfLevels.Add(n);
                            isHeadingNodeAvailable = false;
                            reportProgress(-1, UrakawaSDK_daisy_Lang.CreatingXMLFile);
                        }
                        return true;
                    },
                    delegate(urakawa.core.TreeNode n)
                    { });
        }

        protected virtual void CreateXhtmlDocumentForHeading()
        {

        }

    }
}

using System;
using System.Collections.Generic;
using System.Text;
using urakawa.core;
using urakawa.xuk;

namespace urakawa.examples
{
    /// <summary>
    /// Example implementation of a custon <see cref="TreeNode"/>
    /// </summary>
    public class ExampleCustomTreeNode : TreeNode
    {
        /// <summary>
        /// The Xuk namespace uri of <see cref="ExampleCustomTreeNode"/>s 
        /// </summary>
        public new const string XUK_NS = "http://www.daisy.org/urakawa/example";

        /// <summary>
        /// Default constructor - for system use only, 
        /// <see cref="ExampleCustomTreeNode"/>s should only be created via. the <see cref="TreeNodeFactory"/>
        /// </summary>
        public ExampleCustomTreeNode()
        {
            mCustomTreeNodeData = "";
            mLabel = "";
        }

        /// <summary>
        /// Override for default <see cref="Object.ToString"/> method. 
        /// Appends the value of <see cref="Label"/> to the default implementation output
        /// </summary>
        /// <returns>The string representation of <c>this</c> including the <see cref="Label"/> value</returns>
        public override string ToString()
        {
            return String.Format(
                "{0} (Label={1})", base.ToString(), Label);
        }

        /// <summary>
        /// A piece of data to decern the <see cref="ExampleCustomTreeNode"/> from a standard <see cref="TreeNode"/>
        /// </summary>
        public string CustomTreeNodeData
        {
            get { return mCustomTreeNodeData; }
            set { mCustomTreeNodeData = value; }
        }

        private string mCustomTreeNodeData;

        /// <summary>
        /// Gets or sets the label of <c>this</c>
        /// </summary>
        public string Label
        {
            get { return mLabel; }
            set { mLabel = value; }
        }

        private string mLabel;

        /// <summary>
        /// Copies the <see cref="ExampleCustomTreeNode"/>
        /// </summary>
        /// <param name="deep">If	true,	then include the node's	entire subtree.	 
        ///	Otherwise, just	copy the node	itself.</param>
        /// <param name="inclProperties">If true, then include property of the node,
        /// if false just copy the node itself.</param>
        ///	<returns>A <see	cref="ExampleCustomTreeNode"/>	containing the copied	data.</returns>
        ///	<exception cref="urakawa.exception.FactoryCannotCreateTypeException">
        /// Thrown when the <see cref="TreeNodeFactory"/> of the <see cref="Presentation"/> to which the instance belongs
        /// can not create an <see cref="ExampleCustomTreeNode"/> instance
        ///	</exception>
        protected override TreeNode CopyProtected(bool deep, bool inclProperties)
        {
            TreeNode theCopy = base.CopyProtected(deep, inclProperties);
            if (!(theCopy is ExampleCustomTreeNode))
            {
                throw new urakawa.exception.FactoryCannotCreateTypeException(String.Format(
                                                                                 "The TreeNodeFactory of the Presentation can not create an {0}:ExampleCustomTreeNode",
                                                                                 XukAble.XUK_NS));
            }
            ExampleCustomTreeNode theTypedCopy = (ExampleCustomTreeNode) theCopy;
            theTypedCopy.CustomTreeNodeData = CustomTreeNodeData;
            theTypedCopy.Label = Label;
            return theTypedCopy;
        }

        /// <summary>
        /// Creates a copy of <c>this</c>
        /// </summary>
        /// <param name="deep">A <see cref="bool"/> indicating if children are copied</param>
        /// <param name="inclProperties">A <see cref="bool"/> indicating if property are copied</param>
        /// <returns>The copy</returns>
        public new ExampleCustomTreeNode Copy(bool deep, bool inclProperties)
        {
            return CopyProtected(deep, inclProperties) as ExampleCustomTreeNode;
        }

        /// <summary>
        /// Creates a copy of <c>this</c> including property
        /// </summary>
        /// <param name="deep">A <see cref="bool"/> indicating if children are copied</param>
        /// <returns>The copy</returns>
        public new ExampleCustomTreeNode Copy(bool deep)
        {
            return Copy(deep, true);
        }

        /// <summary>
        /// Creates a deep copy of <c>this</c> including property
        /// </summary>
        /// <returns>The copy</returns>
        public new ExampleCustomTreeNode Copy()
        {
            return Copy(true, true);
        }

        /// <summary>
        /// Creates a new ExampleCustomTreeNode with identical content (recursively) as this node,
        /// but compatible with the given Presentation (factories, managers,
        /// channels, etc.). 
        /// </summary>
        /// <param name="destPres">The destination Presentation to which this node (and all its content, recursively) should be exported.</param>
        /// <returns>The exported node</returns>
        /// <exception cref="exception.MethodParameterIsNullException">Thrown when <paramref name="destPres"/> is null</exception>
        /// <exception cref="exception.FactoryCannotCreateTypeException">
        /// Thrown when the facotries of <paramref name="destPres"/> can not create a node in the sub-tree beginning at <c>this</c>
        /// or a property associated object for one of the nodes in the sub-tree
        /// </exception>
        public new ExampleCustomTreeNode Export(Presentation destPres)
        {
            return ExportProtected(destPres) as ExampleCustomTreeNode;
        }

        /// <summary>
        /// Creates a new TreeNode with identical content (recursively) as this node,
        /// but compatible with the given Presentation (factories, managers,
        /// channels, etc.). 
        /// </summary>
        /// <param name="destPres">The destination Presentation to which this node (and all its content, recursively) should be exported.</param>
        /// <returns>The exported node</returns>
        /// <exception cref="exception.MethodParameterIsNullException">Thrown when <paramref name="destPres"/> is null</exception>
        /// <exception cref="exception.FactoryCannotCreateTypeException">
        /// Thrown when the facotries of <paramref name="destPres"/> can not create a node in the sub-tree beginning at <c>this</c>
        /// or a property associated object for one of the nodes in the sub-tree
        /// </exception>
        protected override TreeNode ExportProtected(Presentation destPres)
        {
            ExampleCustomTreeNode node = base.ExportProtected(destPres) as ExampleCustomTreeNode;
            if (node == null)
            {
                throw new exception.FactoryCannotCreateTypeException(String.Format(
                                                                         "The TreeNodefactory cannot create a ExampleCustomTreeNode matching QName {1}:{0}",
                                                                         XukLocalName, XukNamespaceUri));
            }
            node.CustomTreeNodeData = CustomTreeNodeData;
            node.Label = Label;
            return node;
        }

        /// <summary>
        /// Reads the attributes of a ExampleCustomTreeNode xml element
        /// </summary>
        /// <param name="source">The source <see cref="System.Xml.XmlReader"/></param>
        /// <returns>A <see cref="bool"/> indicating if the attributes were succesfully read</returns>
        protected override void XukInAttributes(System.Xml.XmlReader source)
        {
            CustomTreeNodeData = source.GetAttribute("customTreeNodeData");
            Label = source.GetAttribute("label");
            base.XukInAttributes(source);
        }

        /// <summary>
        /// Writes the attributes of a ExampleCustomTreeNode xml element
        /// </summary>
        /// <param name="wr">The destination <see cref="System.Xml.XmlWriter"/></param>
        /// <param name="baseUri">
        /// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
        /// if <c>null</c> absolute <see cref="Uri"/>s are written
        /// </param>
        protected override void XukOutAttributes(System.Xml.XmlWriter wr, Uri baseUri)
        {
            wr.WriteAttributeString("customTreeNodeData", CustomTreeNodeData);
            wr.WriteAttributeString("label", Label);
            base.XukOutAttributes(wr, baseUri);
        }
    }
}
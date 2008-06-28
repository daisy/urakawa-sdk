using System;
using System.Collections.Generic;
using System.Text;
using urakawa.core;

namespace urakawa.examples
{
	/// <summary>
	/// Subclass of <see cref="TreeNodeFactory"/> that is capable of creating <see cref="ExampleCustomTreeNode"/>s
	/// as well as <see cref="TreeNode"/>s
	/// </summary>
	public class ExampleCustomTreeNodeFactory : TreeNodeFactory
	{

		/// <summary>
		/// Default constructor
		/// </summary>
		public ExampleCustomTreeNodeFactory()
			: base()
		{

		}

		/// <summary>
		/// Creates a new <see cref="TreeNode"/> or subclass instance of <see cref="Type"/> matching a given QName
		/// </summary>
		/// <param name="localName">The local localName part of the QName</param>
		/// <param name="namespaceUri">The namespace uri part of the QName</param>
		/// <returns>The created <see cref="TreeNode"/> or subclass instance</returns>
		public override TreeNode createNode(string localName, string namespaceUri)
		{
			if (namespaceUri == ExampleCustomDataModelFactory.EX_CUST_NS)
			{
				if (localName == typeof(ExampleCustomTreeNode).Name)
				{
					ExampleCustomTreeNode newNode = new ExampleCustomTreeNode();
					newNode.Presentation = Presentation;
					return newNode;
				}
			}
			return base.createNode(localName, namespaceUri);
		}
	}
}

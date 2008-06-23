using System;
using System.Collections.Generic;
using System.Text;
using urakawa;

namespace urakawa.examples
{
	/// <summary>
	/// A <see cref="DataModelFactory"/> for the example custom data model, including the ability to create:
	/// <list>
	/// <item><see cref="ExampleCustomTreeNodeFactory"/></item>
	/// <item><see cref="ExampleCustomPropertyFactory"/></item>
	/// </list>
	/// </summary>
	public class ExampleCustomDataModelFactory : DataModelFactory
	{
		/// <summary>
		/// Namespace uri for <see cref="ExampleCustomTreeNode"/> and <see cref="ExampleCustomProperty"/> XUK nodes
		/// </summary>
		public static string EX_CUST_NS = "http://www.daisy.org/urakawa/example";
		/// <summary>
		/// Creates a <see cref="ExampleCustomPropertyFactory"/> 
		/// </summary>
		/// <returns>The <see cref="ExampleCustomPropertyFactory"/></returns>
		public override property.PropertyFactory createPropertyFactory()
		{
			return createPropertyFactory(typeof(ExampleCustomPropertyFactory).Name, EX_CUST_NS);
		}
		/// <summary>
		/// Creates a <see cref="property.PropertyFactory"/> of type matching a given Xuk QName
		/// </summary>
		/// <param name="localName">The local name part of the given Xuk QName</param>
		/// <param name="namespaceUri">The namespace uri part of the given Xuk QName</param>
		/// <returns>The created <see cref="property.PropertyFactory"/></returns>
		public override property.PropertyFactory createPropertyFactory(string localName, string namespaceUri)
		{
			if (localName == typeof(ExampleCustomPropertyFactory).Name && namespaceUri == EX_CUST_NS)
			{
				return new ExampleCustomPropertyFactory();
			}
			return base.createPropertyFactory(localName, namespaceUri);
		}
		/// <summary>
		/// Creates a <see cref="ExampleCustomTreeNodeFactory"/>
		/// </summary>
		/// <returns>The <see cref="ExampleCustomTreeNodeFactory"/></returns>
		public override urakawa.core.TreeNodeFactory createTreeNodeFactory()
		{
			return createTreeNodeFactory(typeof(ExampleCustomTreeNodeFactory).Name, EX_CUST_NS);
		}
		/// <summary>
		/// Creates a <see cref="core.TreeNodeFactory"/> of type matching a given Xuk QName
		/// </summary>
		/// <param name="localName">The local name part of the given Xuk QName</param>
		/// <param name="namespaceUri">The namespace uri part of the given Xuk QName</param>
		/// <returns>The created <see cref="core.TreeNodeFactory"/></returns>
		public override urakawa.core.TreeNodeFactory createTreeNodeFactory(string localName, string namespaceUri)
		{
			if (localName == typeof(ExampleCustomTreeNodeFactory).Name && namespaceUri == EX_CUST_NS)
			{
				return new ExampleCustomTreeNodeFactory();
			}
			return base.createTreeNodeFactory(localName, namespaceUri);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Text;
using urakawa.core;

namespace urakawa.examples
{
	/// <summary>
	/// Subclass of <see cref="CoreNodeFactory"/> that is capable of creating <see cref="ExampleCustomCoreNode"/>s
	/// as well as <see cref="CoreNode"/>s
	/// </summary>
	public class ExampleCustomCoreNodeFactory : CoreNodeFactory
	{
		public static string EX_CUST_NS = "http://www.daisy.org/urakawa/example";

		/// <summary>
		/// Default constructor
		/// </summary>
		public ExampleCustomCoreNodeFactory()
			: base()
		{

		}

		/// <summary>
		/// Creates a new <see cref="CoreNode"/> or subclass instance of <see cref="Type"/> matching a given QName
		/// </summary>
		/// <param name="localName">The local name part of the QName</param>
		/// <param name="namespaceUri">The namespace uri part of the QName</param>
		/// <returns>The created <see cref="CoreNode"/> or subclass instance</returns>
		public override CoreNode createNode(string localName, string namespaceUri)
		{
			if (localName == "ExampleCustomCoreNode" && namespaceUri == ExampleCustomCoreNodeFactory.EX_CUST_NS)
			{
				return new ExampleCustomCoreNode(getPresentation());
			}
			return base.createNode(localName, namespaceUri);
		}
	}
}

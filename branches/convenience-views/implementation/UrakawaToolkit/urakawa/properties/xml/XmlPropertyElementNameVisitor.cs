using System;
using urakawa.core;
using urakawa.core.visitor;

namespace urakawa.properties.xml
{
	/// <summary>
	/// Summary description for XmlPropertyElementNameVisitor.
	/// </summary>
	public class XmlPropertyElementNameVisitor : ICoreNodeVisitor
	{
		private System.Collections.Generic.IList<string> mNamesToMatch;
		private System.Collections.Generic.IList<ICoreNode> mNodes;
		private Type mXmlPropertyType;

		/// <summary>
		/// The constructor
		/// </summary>
		public XmlPropertyElementNameVisitor()
		{
			mNamesToMatch = new System.Collections.Generic.List<string>();
			mNodes = new System.Collections.Generic.List<ICoreNode>();
			mXmlPropertyType = typeof(XmlProperty);
		}

		public void setXmlPropertyType(Type newType)
		{
			if (typeof(IXmlProperty).IsAssignableFrom(newType))
			{
				throw new exception.MethodParameterIsWrongTypeException(
					"The new xml property type must a Type that can be assigned to an IXmlProperty");
			}
		}

		/// <summary>
		/// Add an element name to the collection of search terms.  
		/// The search terms should be considered an "OR"-list.
		/// </summary>
		/// <param name="localName">The local name part of the element name</param>
		/// <param name="namespaceUri">The namespace uri part of the element name</param>
		public void addElementName(string localName, string namespaceUri)
		{
			mNamesToMatch.Add(String.Format("{0}:{1}", namespaceUri, localName));
		}



		/// <summary>
		/// Get the results of the tree visit to see if any nodes were found
		/// whose XML properties matched the search request.
		/// </summary>
		/// <returns>The list</returns>
		public System.Collections.Generic.IList<ICoreNode> getResults()
		{
			return mNodes;
		}

		private bool isMatch(string localName, string namespaceUri)
		{
			return mNamesToMatch.Contains(String.Format("{0}:{1}", localName, namespaceUri));
		}
		#region ICoreNodeVisitor Members

		/// <summary>
		/// look at the current node and see if it has an XML property 
		/// that is interesting to us.  if so, add it to our internal list.
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		public bool preVisit(ICoreNode node)
		{
			IXmlProperty xp = (IXmlProperty)node.getProperty(mXmlPropertyType);

			if (xp != null && isMatch(xp.getLocalName(), xp.getNamespaceUri()) == true)
			{
				mNodes.Add(node);
			}
			
			return true;
		}

		/// <summary>
		/// this visitor does nothing post-visit
		/// </summary>
		/// <param name="node"></param>
		public void postVisit(ICoreNode node)
		{
			//empty
		}

		#endregion
	}
}

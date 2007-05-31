using System;
using System.Collections.Generic;
using urakawa.core;
using urakawa.core.visitor;

namespace urakawa.properties.xml
{
	/// <summary>
	/// Summary description for XmlPropertyElementNameVisitor.
	/// </summary>
	public class XmlPropertyElementNameVisitor : ICoreNodeVisitor
	{
		private List<string> mNamesToMatch;
		private List<ICoreNode> mNodes;
		private Type mXmlPropertyType = typeof(XmlProperty);

		/// <summary>
		/// The constructor
		/// </summary>
		public XmlPropertyElementNameVisitor()
		{
			mNamesToMatch = new List<string>();
			mNodes = new List<ICoreNode>();
			mXmlPropertyType = typeof(XmlProperty);
		}

		/// <summary>
		/// Sets the <see cref="Type"/> of the <see cref="XmlProperty"/> to inspect upon visitation
		/// </summary>
		/// <param name="newType">The <see cref="Type"/> - must implement <see cref="XmlProperty"/></param>
		public void setXmlPropertyType(Type newType)
		{
			if (typeof(XmlProperty).IsAssignableFrom(newType))
			{
				throw new exception.MethodParameterIsWrongTypeException(
					"The new xml property type must a Type implements interface XmlProperty");
			}
			mXmlPropertyType = newType;
		}

		/// <summary>
		/// Add an element localName to the collection of search terms.  
		/// The search terms should be considered an "OR"-list.
		/// </summary>
		/// <param name="localName">The local localName part of the element localName</param>
		/// <param name="namespaceUri">The namespace uri part of the element localName</param>
		public void addElementName(string localName, string namespaceUri)
		{
			mNamesToMatch.Add(String.Format("{0}:{1}", namespaceUri, localName));
		}



		/// <summary>
		/// Get the results of the tree visit to see if any nodes were found
		/// whose XML properties matched the search request.
		/// </summary>
		/// <returns>The list</returns>
		public List<ICoreNode> getResults()
		{
			return mNodes;
		}

		private bool isMatch(string localName, string namespaceUri)
		{
			return mNamesToMatch.Contains(String.Format("{0}:{1}", namespaceUri, localName));
		}
		#region ICoreNodeVisitor Members

		/// <summary>
		/// Look at the current node and see if it has an <see cref="XmlProperty"/> 
		/// that is interesting to us.  if so, add it to our internal list.
		/// </summary>
		/// <param name="node">The <see cref="ICoreNode"/> bwing visited</param>
		/// <returns><c>true</c></returns>
		public bool preVisit(ICoreNode node)
		{
			XmlProperty xp = (XmlProperty)node.getProperty(mXmlPropertyType);

			if (xp != null && isMatch(xp.getLocalName(), xp.getNamespaceUri()) == true)
			{
				mNodes.Add(node);
			}
			
			return true;
		}

		/// <summary>
		/// This visitor does nothing post-visit
		/// </summary>
		/// <param name="node">The <see cref="ICoreNode"/> being visited</param>
		public void postVisit(ICoreNode node)
		{
			//empty
		}

		#endregion
	}
}

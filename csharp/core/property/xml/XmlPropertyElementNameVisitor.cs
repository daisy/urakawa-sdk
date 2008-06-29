using System;
using System.Collections.Generic;
using urakawa.core;
using urakawa.core.visitor;

namespace urakawa.property.xml
{
	/// <summary>
	/// Summary description for XmlPropertyElementNameVisitor.
	/// </summary>
	public class XmlPropertyElementNameVisitor : ITreeNodeVisitor
	{
		private List<string> mNamesToMatch;
		private List<TreeNode> mNodes;
		private Type mXmlPropertyType = typeof(XmlProperty);

		/// <summary>
		/// The constructor
		/// </summary>
		public XmlPropertyElementNameVisitor()
		{
			mNamesToMatch = new List<string>();
			mNodes = new List<TreeNode>();
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
		/// whose XML property matched the search request.
		/// </summary>
		/// <returns>The list</returns>
		public List<TreeNode> getResults()
		{
			return mNodes;
		}

		private bool isMatch(string localName, string namespaceUri)
		{
			return mNamesToMatch.Contains(String.Format("{0}:{1}", namespaceUri, localName));
		}
		#region ITreeNodeVisitor Members

		/// <summary>
		/// Look at the current node and see if it has an <see cref="XmlProperty"/> 
		/// that is interesting to us.  if so, add it to our internal list.
		/// </summary>
		/// <param name="node">The <see cref="TreeNode"/> bwing visited</param>
		/// <returns><c>true</c></returns>
		public bool PreVisit(TreeNode node)
		{
			XmlProperty xp = node.GetProperty<XmlProperty>();

			if (xp != null && isMatch(xp.getLocalName(), xp.getNamespaceUri()) == true)
			{
				mNodes.Add(node);
			}
			
			return true;
		}

		/// <summary>
		/// This visitor does nothing post-visit
		/// </summary>
		/// <param name="node">The <see cref="TreeNode"/> being visited</param>
		public void PostVisit(TreeNode node)
		{
			//empty
		}

		#endregion
	}
}

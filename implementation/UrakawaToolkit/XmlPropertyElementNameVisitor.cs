using System;

namespace urakawa.core
{
	/// <summary>
	/// Summary description for XmlPropertyElementNameVisitor.
	/// </summary>
	public class XmlPropertyElementNameVisitor : ICoreNodeVisitor
	{
		private System.Collections.ArrayList mNamesToMatch;
		private System.Collections.ArrayList mNodes;

		/// <summary>
		/// The constructor
		/// </summary>
		public XmlPropertyElementNameVisitor()
		{
			mNamesToMatch = new System.Collections.ArrayList();
			mNodes = new System.Collections.ArrayList();
		}

		/// <summary>
		/// Add an element name to the collection of search terms.  
		/// The search terms should be considered an "OR"-list.
		/// </summary>
		/// <param name="elmName"></param>
		public void addElementName(string elmName)
		{
			mNamesToMatch.Add(elmName);
		}

		/// <summary>
		/// Get the results of the tree visit to see if any nodes were found
		/// whose XML properties matched the search request.
		/// </summary>
		/// <returns></returns>
		public System.Collections.IList getResults()
		{
			return mNodes;
		}

		private bool isMatch(string name)
		{
			bool bfound = false;
			for (int i = 0; i<mNamesToMatch.Count; i++)
			{
				string cmp = (string)mNamesToMatch[i];

				if (cmp == name)
				{
					bfound = true;
					break;
				}
			}

			return bfound;
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
			XmlProperty xp = (XmlProperty)node.getProperty(typeof(XmlProperty));

			if (xp != null && isMatch(xp.getName()) == true)
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

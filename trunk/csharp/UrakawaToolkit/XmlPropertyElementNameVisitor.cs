using System;

namespace urakawa.core
{
	/// <summary>
	/// Summary description for XmlPropertyElementNameVisitor.
	/// </summary>
	public class XmlPropertyElementNameVisitor : ICoreNodeVisitor
	{
		private System.Collections.ArrayList mNamesToMatch;

		//@todo
		//this should be a hierarchy, not a flat list
		private System.Collections.ArrayList mNodes;

		public XmlPropertyElementNameVisitor()
		{
			mNamesToMatch = new System.Collections.ArrayList();
			mNodes = new System.Collections.ArrayList();
		}

		public void addElementName(string elmName)
		{
			mNamesToMatch.Add(elmName);
		}

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
			XmlProperty xp = (XmlProperty)node.getProperty(PropertyType.XML);

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

using System;

namespace urakawa.xuk
{
	/// <summary>
	/// Summary description for Project.
	/// </summary>
	public class Project
	{
		urakawa.core.Presentation mPresentation;

		public Project()
		{
			
		}

		public bool openXUK(Uri fileUri)
		{
			mPresentation = new urakawa.core.Presentation();

			System.Xml.XmlTextReader source = new System.Xml.XmlTextReader(fileUri.ToString());
			source.WhitespaceHandling = System.Xml.WhitespaceHandling.Significant;

			//move to the Presentation element
			while (! (source.Name == "Presentation" && 
				source.NodeType == System.Xml.XmlNodeType.Element)
				&&
				source.EOF == false)
			{
				source.Read();
			}

			bool didItWork = mPresentation.XUKin(source);

			return didItWork;
		}

		public urakawa.core.Presentation getPresentation()
		{
			return mPresentation;
		}

	}
}

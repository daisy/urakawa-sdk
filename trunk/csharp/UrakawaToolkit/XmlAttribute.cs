using System;

namespace urakawa.core
{
	/// <summary>
	/// Summary description for XmlAttribute.
	/// </summary>
	public class XmlAttribute:IXmlAttribute
	{
		public XmlAttribute()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		#region IXmlAttribute Members

		public string Value
		{
			get
			{
				// TODO:  Add XmlAttribute.Value getter implementation
				return null;
			}
			set
			{
				// TODO:  Add XmlAttribute.Value setter implementation
			}
		}

		public string Namespace
		{
			get
			{
				// TODO:  Add XmlAttribute.Namespace getter implementation
				return null;
			}
		}

		public string Name
		{
			get
			{
				// TODO:  Add XmlAttribute.Name getter implementation
				return null;
			}
		}

		public IXmlProperty getParent()
		{
			// TODO:  Add XmlAttribute.getParent implementation
			return null;
		}

		#endregion
	}
}

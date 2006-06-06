using System;
using System.Xml;

namespace urakawa.core
{
	/// <summary>
	/// Summary description for XmlAttribute.
	/// </summary>
	public class XmlAttribute : IXmlAttribute
	{
		IXmlProperty mParent;
		string mName;
		string mNamespace;
		string mValue;

		public XmlAttribute(IXmlProperty parent, string newName, string newNamespace, string newValue)
		{
			if(parent != null)
				mParent = parent;
			else
				throw(new urakawa.exception.MethodParameterIsNullException("Parent IXmlProperty needs to be specified when creating an XMLAtribute."));

			if(newNamespace != null)
				mNamespace = newNamespace;
			else
				throw(new urakawa.exception.MethodParameterIsNullException("Namespace of an XmlAtrribute cannot be null. Empty string is allowed."));

			if(newName != null && newName != "")
				mName = newName;
			else
				throw(new urakawa.exception.MethodParameterIsNullException("Name of an XmlAtrribute cannot be null or empty."));

			if (newValue != null && newValue != "")
				mValue = newValue;
			//@todo
			//throw an exception here or not?  attribute values can probably be empty.
		}
		#region IXmlAttribute Members

		public IXmlAttribute copy()
		{
			XmlAttribute tmpAttr = new XmlAttribute(this.mParent,this.mName,this.mNamespace,this.mValue);
			return tmpAttr;
		}

		public string getValue()
		{
			return mValue;
		}
		public void setValue(string newValue)
		{
			mValue = newValue;
		}

		public string getNamespace()
		{
				return mNamespace;
		}

		public string getName()
		{
			return mName;
		}

		public void setQName(string newName, string newNamespace)
		{
      mName = newName;
      mNamespace = newNamespace;
		}

		public IXmlProperty getParent()
		{
			// TODO:  Add XmlAttribute.getParent implementation
			return mParent;
		}

		#endregion
		#region IXUKable members 

		//marisa's comment: i don't think this one is required
		public bool XUKin(System.Xml.XmlReader source)
		{
      if (source == null)
      {
        throw new exception.MethodParameterIsNullException("Xml Reader is null");
      }

      //if we are not at the opening tag of an XmlAttribute element, return false
      if (!(source.Name == "XmlAttribute" && source.NodeType == System.Xml.XmlNodeType.Element))
      {
        return false;
      }

      bool bFoundError = false;
      string name = source.GetAttribute("name");
      if (name==null || name=="") return false;
      string ns = source.GetAttribute("namespace");
      if (ns==null) ns = "";
      setQName(name, ns);
      string value = "";
      if (!source.IsEmptyElement)
      {
        while (source.Read())
        {
          if (source.NodeType==XmlNodeType.Text)
          {
            value += source.Value;
          }
          else if (source.NodeType==XmlNodeType.SignificantWhitespace)
          {
            value += source.Value;
          }
          else if (source.NodeType==XmlNodeType.Element)
          {
            bFoundError = true;
          }
          else if (source.NodeType==XmlNodeType.EndElement)
          {
            break;
          }
          if (source.EOF) break;
          if (bFoundError) break;
        }
      }
      if (bFoundError)
      {
        return false;
      }
      else
      {
        setValue(value);
        return true;
      }
    }

		public bool XUKout(System.Xml.XmlWriter destination)
		{
			destination.WriteStartElement("XmlAttribute");

			//name is required
			if (mName == "")
				return false;

			destination.WriteAttributeString("name", mName);
			
			if (mNamespace != "")
				destination.WriteAttributeString("namespace", mNamespace);

			destination.WriteString(this.mValue);

			destination.WriteEndElement();

			return true;
		}
		#endregion
	}
}

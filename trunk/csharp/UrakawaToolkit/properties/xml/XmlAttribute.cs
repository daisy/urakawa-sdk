using System;
using System.Xml;
using urakawa.core.property;

namespace urakawa.properties.xml
{
	/// <summary>
	/// Default implementation of <see cref="IXmlAttribute"/>
	/// </summary>
	public class XmlAttribute : IXmlAttribute
	{
		IXmlProperty mParent;
		string mName = "dummy";
		string mNamespace = "";
		string mValue = "";

		/// <summary>
		/// Constructor setting the parent <see cref="IXmlProperty"/>
		/// </summary>
		/// <param name="parent">The parent</param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when the parent is <c>null</c>
		/// </exception>
		protected internal XmlAttribute(IXmlProperty parent)
		{
			if (parent == null)
			{
				throw new exception.MethodParameterIsNullException("The parent of an xml attribute can not be null");
			}
			mParent = parent;
		}

		#region IXmlAttribute Members
		/// <summary>
    /// Creates a copy of the <see cref="XmlAttribute"/>
    /// </summary>
    /// <returns>The copy</returns>
		/// <exception cref="exception.FactoryCanNotCreateTypeException">
		/// Thrown when the <see cref="ICorePropertyFactory"/> of the <see cref="urakawa.core.ICorePresentation"/> 
		/// to which <c>this</c> belongs is not a subclass of <see cref="IXmlPropertyFactory"/>
		/// </exception>
    public IXmlAttribute copy()
		{
			string xukLN = getXukLocalName();
			string xukNS = getXukNamespaceUri();
			IXmlAttribute copyAttr = getParent().getXmlPropertyFactory().createXmlAttribute(
				getParent(), xukLN, xukNS);
			if (copyAttr == null)
			{
				throw new exception.FactoryCanNotCreateTypeException(String.Format(
					"The xml property factory does not support creating xml attributes matching QName {0}:{1}",
					getXukNamespaceUri(), getXukLocalName()));
			}
			copyAttr.setQName(getLocalName(), getNamespaceUri());
			copyAttr.setValue(getValue());
			return copyAttr;
		}

    /// <summary>
    /// Gets the value of gthe <see cref="XmlAttribute"/>
    /// </summary>
    /// <returns>The value</returns>
    public string getValue()
		{
			return mValue;
		}

    /// <summary>
    /// Sets the value of the <see cref="XmlAttribute"/>
    /// </summary>
    /// <param name="newValue">The new value</param>
    public void setValue(string newValue)
		{
			mValue = newValue;
		}

    /// <summary>
    /// Gets the namespace of the <see cref="XmlAttribute"/>
    /// </summary>
    /// <returns>The namespace</returns>
    public string getNamespaceUri()
		{
				return mNamespace;
		}

    /// <summary>
    /// Gets the local localName of the <see cref="XmlAttribute"/>
    /// </summary>
    /// <returns>The local localName</returns>
    public string getLocalName()
		{
			return mName;
		}

    /// <summary>
    /// Sets the QName of the <see cref="XmlAttribute"/> 
    /// </summary>
    /// <param name="newNamespace">The namespace part of the new QName</param>
    /// <param name="newName">The localName part of the new QName</param>
    public void setQName(string newName, string newNamespace)
		{
      mName = newName;
      mNamespace = newNamespace;
		}

    /// <summary>
    /// Gets the parent <see cref="IXmlProperty"/> of <c>this</c>
    /// </summary>
    /// <returns></returns>
    public IXmlProperty getParent()
		{
			return mParent;
		}

		/// <summary>
		/// Sets the parent <see cref="IXmlProperty"/> of <c>this</c>. 
		/// Is intended for internal use by the owning <see cref="IXmlProperty"/>,
		/// calling this method may lead to corruption of the data model
		/// </summary>
		/// <param name="newParent">The new parent</param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when the new parent is <c>null</c>
		/// </exception>
		public void setParent(IXmlProperty newParent)
		{
			if (newParent == null)
			{
				throw new exception.MethodParameterIsNullException(
					"The parent of an xml attribute can not be null");
			}
			mParent = newParent;
		}
		#endregion

		#region IXUKAble members 

		//marisa's comment: i don't think this one is required
    /// <summary>
    /// Reads the <see cref="XmlAttribute"/> instance from an XmlAttribute element in a XUK file
    /// </summary>
    /// <param name="source">The source <see cref="XmlReader"/></param>
    /// <returns>A <see cref="bool"/> indicating if the read was succesful</returns>
		public bool XukIn(System.Xml.XmlReader source)
		{
      if (source == null)
      {
        throw new exception.MethodParameterIsNullException("Xml Reader is null");
      }
			if (source.NodeType != System.Xml.XmlNodeType.Element) return false;

      string name = source.GetAttribute("localName");
      if (name==null || name=="") return false;
      string ns = source.GetAttribute("namespaceUri");
      if (ns==null) ns = "";
      setQName(name, ns);
			string v = "";
      if (!source.IsEmptyElement)
      {
				v = source.ReadString();
      }
			mValue = v;
			return true;
    }

    /// <summary>
    /// Writes a XmlAttribute element representing the <see cref="XmlAttribute"/> instance
    /// to a XUK file
    /// </summary>
    /// <param name="destination">The destination <see cref="XmlWriter"/></param>
    /// <returns>A <see cref="bool"/> indicating if the write was succesful</returns>
		public bool XukOut(System.Xml.XmlWriter destination)
		{
			destination.WriteStartElement("XmlAttribute", urakawa.ToolkitSettings.XUK_NS);

			//localName is required
			if (mName == "") return false;

			destination.WriteAttributeString("localName", mName);
			
			if (mNamespace != "") destination.WriteAttributeString("namespaceUri", mNamespace);

			destination.WriteString(this.mValue);

			destination.WriteEndElement();

			return true;
		}

		
		/// <summary>
		/// Gets the local localName part of the QName representing a <see cref="XmlAttribute"/> in Xuk
		/// </summary>
		/// <returns>The local localName part</returns>
		public string getXukLocalName()
		{
			return this.GetType().Name;
		}

		/// <summary>
		/// Gets the namespace uri part of the QName representing a <see cref="XmlAttribute"/> in Xuk
		/// </summary>
		/// <returns>The namespace uri part</returns>
		public string getXukNamespaceUri()
		{
			return urakawa.ToolkitSettings.XUK_NS;
		}

		#endregion
	}
}

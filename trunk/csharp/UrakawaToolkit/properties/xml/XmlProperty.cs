using System;
using System.Collections.Generic;
using System.Xml;
using urakawa.core;
using urakawa.core.property;
using urakawa.core.visitor;
using urakawa.exception;
using urakawa.properties.channel;

namespace urakawa.properties.xml
{
	/// <summary>
	/// Default implementation of <see cref="XmlProperty"/> interface
	/// </summary>
	public class XmlProperty : Property
	{
		private string mLocalName = "dummy";
		private string mNamespaceUri = "";
		private IDictionary<string, XmlAttribute> mAttributes = new Dictionary<string, XmlAttribute>();

		#region XmlProperty Members

		/// <summary>
		/// Gets the <see cref="XmlType"/> of the <see cref="XmlProperty"/>
		/// </summary>
		/// <returns>Always <see cref="XmlType.ELEMENT"/></returns>
		public XmlType getXmlType()
		{
			return XmlType.ELEMENT;
		}

		/// <summary>
		/// Gets the local localName of <c>this</c>
		/// </summary>
		/// <returns>The local localName</returns>
		public string getLocalName()
		{
			return mLocalName;
		}

		/// <summary>
		/// Gets the namespace uri of <c>this</c>
		/// </summary>
		/// <returns>The namespace uri</returns>
		public string getNamespaceUri()
		{
			return mNamespaceUri;
		}

		/// <summary>
		/// Gets the <see cref="IXmlPropertyFactory"/> associated with <c>this</c> via the <see cref="ICorePresentation"/>
		/// of the owning <see cref="ICoreNode"/>
		/// </summary>
		/// <returns>The <see cref="IXmlPropertyFactory"/></returns>
		/// <exception cref="exception.FactoryCanNotCreateTypeException">
		/// Thrown when the <see cref="ICorePropertyFactory"/> of the <see cref="ICorePresentation"/>
		/// of the <see cref="ICoreNode"/> that owns <c>this</c> is not a subclass of <see cref="IXmlPropertyFactory"/>
		/// </exception>
		/// <remarks>
		/// This method is conveniencs for 
		/// <c>(IXmlPropertyFactory)getOwner().getPresentation().getPropertyFactory()</c></remarks>
		public IXmlPropertyFactory getXmlPropertyFactory()
		{
			ICorePropertyFactory coreFact = getOwner().getPresentation().getPropertyFactory();
			if (!(coreFact is IXmlPropertyFactory))
			{
				throw new exception.FactoryCanNotCreateTypeException(
					"The property factory of the presentation does not subclass IXmlPropertyfactory");
			}
			return (IXmlPropertyFactory)coreFact;
		}

		/// <summary>
		/// Sets the QName of <c>this</c> (i.e. the local localName and namespace uri)
		/// </summary>
		/// <param name="newName">
		/// The local localName part of the new QName
		/// - must not be <c>null</c> or <see cref="String.Empty"/>
		/// </param>
		/// <param name="newNamespace">
		/// The namespace uri part of the new QName - must not be <c>null</c>
		/// </param>
		public void setQName(string newName, string newNamespace)
		{
			if (newName == null)
			{
				throw new exception.MethodParameterIsNullException("The local localName must not be null");
			}
			if (newName == String.Empty)
			{
				throw new exception.MethodParameterIsEmptyStringException("The local localName must not be empty");
			}
			if (newNamespace == null)
			{
				throw new exception.MethodParameterIsNullException("The namespace uri must not be null");
			}
			mLocalName = newName;
			mNamespaceUri = newNamespace;
		}

		/// <summary>
		/// Gets a list of the <see cref="XmlAttribute"/>s of <c>this</c>
		/// </summary>
		/// <returns>The list</returns>
		public List<XmlAttribute> getListOfAttributes()
		{
			return new List<XmlAttribute>(mAttributes.Values);
		}

		/// <summary>
		/// Sets an <see cref="XmlAttribute"/>, possibly overwriting an existing one
		/// </summary>
		/// <param name="newAttribute">The <see cref="XmlAttribute"/> to set</param>
		/// <returns>A <see cref="bool"/> indicating if an existing <see cref="XmlAttribute"/> was overwritten</returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when the <see cref="XmlAttribute"/> to set is <c>null</c>
		/// </exception>
		public bool setAttribute(XmlAttribute newAttribute)
		{
			if (newAttribute==null)
			{
				throw new exception.MethodParameterIsNullException("Can not set a null xml attribute");
			}
			string key = String.Format("{1}:{0}", newAttribute.getLocalName(), newAttribute.getNamespaceUri());
			if (mAttributes.ContainsKey(key))
			{
				mAttributes[key] = newAttribute;
				return true;
			}
			else
			{
				mAttributes.Add(key, newAttribute);
				return false;
			}
		}

		/// <summary>
		/// Sets an <see cref="XmlAttribute"/>, possibly overwriting an existing one
		/// </summary>
		/// <param name="localName">The local localName of the new attribute</param>
		/// <param name="namespaceUri">The namespace uri part of the new attribute</param>
		/// <param name="value">The value of the new attribute</param>
		/// <returns>A <see cref="bool"/> indicating if an existing <see cref="XmlAttribute"/> was overwritten</returns>
		/// <exception cref="exception.FactoryCanNotCreateTypeException">
		/// <see cref="getXmlPropertyFactory"/> for information on when this <see cref="Exception"/> is thrown
		/// </exception>
		public bool setAttribute(string localName, string namespaceUri, string value)
		{
			XmlAttribute newAttribute = getXmlPropertyFactory().createXmlAttribute(this);
			return setAttribute(newAttribute);
		}

		/// <summary>
		/// Gets the <see cref="XmlAttribute"/> with a given QName
		/// </summary>
		/// <param name="localName">The local localName part of the given QName</param>
		/// <param name="namespaceUri">The namespce uri part of the given QName</param>
		/// <returns>The <see cref="XmlAttribute"/> if found, otherwise <c>null</c></returns>
		public XmlAttribute getAttribute(string localName, string namespaceUri)
		{
			string key = String.Format("{1}:{0}", localName, namespaceUri);
			if (mAttributes.ContainsKey(key))
			{
				return mAttributes[key];
			}
			return null;
		}

		#endregion

		#region Property Members

		/// <summary>
		/// Creates a copy of <c>this</c> including copies of any <see cref="XmlAttribute"/>s
		/// </summary>
		/// <returns>The copy</returns>
		public new XmlProperty copy()
		{
			Property prop = base.copy();
			if (!(prop is XmlProperty))
			{
				throw new exception.FactoryCanNotCreateTypeException(String.Format(
					"The property factory can not create a XmlProperty matching QName {0}:{1}",
					getXukNamespaceUri(), getXukLocalName()));
			}
			XmlProperty xmlProp = (XmlProperty)prop;
			xmlProp.setQName(getLocalName(), getNamespaceUri());
			foreach (XmlAttribute attr in getListOfAttributes())
			{
				xmlProp.setAttribute(attr.copy());
			}
			return xmlProp;
		}

		#endregion
		
		#region IXUKAble members

		/// <summary>
		/// Reads the attributes of a XmlProperty xuk element.
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		/// <returns>A <see cref="bool"/> indicating if the attributes was succefully read</returns>
		protected override bool XukInAttributes(XmlReader source)
		{
			string ln = source.GetAttribute("localName");
			if (ln == null || ln == "") return false;
			string ns = source.GetAttribute("namespaceUri");
			if (ns == null) ns = "";
			setQName(ln, ns);
			return true;
		}

		/// <summary>
		/// Reads a child of a XmlProperty xuk element. 
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		/// <returns>A <see cref="bool"/> indicating if the child was succefully read</returns>
		protected override bool XukInChild(XmlReader source)
		{
			bool readItem = false;
			if (source.NamespaceURI == ToolkitSettings.XUK_NS)
			{
				readItem = true;
				switch (source.LocalName)
				{
					case "mXmlAttributes":
						if (!XukInXmlAttributes(source)) return false;
						break;
					default:
						readItem = false;
						break;
				}
			}
			if (!(readItem || source.IsEmptyElement))
			{
				source.ReadSubtree().Close();//Read past unknown child 
			}
			return true;
		}

		private bool XukInXmlAttributes(XmlReader source)
		{
			if (!source.IsEmptyElement)
			{
				while (source.Read())
				{
					if (source.NodeType == XmlNodeType.Element)
					{
						XmlAttribute attr = getXmlPropertyFactory().createXmlAttribute(this, source.LocalName, source.NamespaceURI);
						if (attr != null)
						{
							if (!attr.XukIn(source)) return false;
							setAttribute(attr);
						}
						else if (!source.IsEmptyElement)
						{
							source.ReadSubtree().Close();
						}
					}
					else if (source.NodeType == XmlNodeType.EndElement)
					{
						break;
					}
					if (source.EOF) break;
				}
			}
			return true;
		}

		/// <summary>
		/// Writes the attributes of a XmlProperty element
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <returns>A <see cref="bool"/> indicating if the write was succesful</returns>
		protected override bool XukOutAttributes(XmlWriter destination)
		{
			destination.WriteAttributeString("localName", getLocalName());
			destination.WriteAttributeString("namespaceUri", getNamespaceUri());
			return true;
		}

		/// <summary>
		/// Write the child elements of a XmlProperty element.
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <returns>A <see cref="bool"/> indicating if the write was succesful</returns>
		protected override bool XukOutChildren(XmlWriter destination)
		{
			List<XmlAttribute> attrs = getListOfAttributes();
			if (attrs.Count > 0)
			{
				destination.WriteStartElement("mXmlAttributes", ToolkitSettings.XUK_NS);
				foreach (XmlAttribute a in attrs)
				{
					a.XukOut(destination);
				}
				destination.WriteEndElement();
			}
			return true;
		}

		#endregion

		#region IValueEquatable<Property> Members

		/// <summary>
		/// Compares <c>this</c> with another <see cref="Property"/> for equality.
		/// </summary>
		/// <param name="other">The other <see cref="Property"/></param>
		/// <returns><c>true</c> if the <see cref="Property"/>s are equal, otherwise <c>false</c></returns>
		public override bool ValueEquals(Property other)
		{
			if (!base.ValueEquals(other)) return false;
			XmlProperty xmlProp = (XmlProperty)other;
			if (getLocalName() != xmlProp.getLocalName()) return false;
			if (getNamespaceUri() != xmlProp.getNamespaceUri()) return false;
			List<XmlAttribute> thisAttrs = getListOfAttributes();
			List<XmlAttribute> otherAttrs = xmlProp.getListOfAttributes();
			if (thisAttrs.Count != otherAttrs.Count) return false;
			foreach (XmlAttribute thisAttr in thisAttrs)
			{
				XmlAttribute otherAttr = xmlProp.getAttribute(thisAttr.getLocalName(), thisAttr.getNamespaceUri());
				if (otherAttr == null) return false;
				if (otherAttr.getValue() != thisAttr.getValue()) return false;
			} 
			return true;
		}

		#endregion
	}


	/// <summary>
	/// The possible types of <see cref="XmlProperty"/>s
	/// </summary>
	public enum XmlType
	{
		/// <summary>
		/// Element type - the <see cref="XmlProperty"/> represents an XML element
		/// </summary>
		ELEMENT,
		/// <summary>
		/// Text type - the <see cref="XmlProperty"/> represents an XML text node
		/// </summary>
		TEXT
	};

}

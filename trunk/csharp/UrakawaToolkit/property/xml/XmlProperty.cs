using System;
using System.Collections.Generic;
using System.Xml;
using urakawa.core;
using urakawa.property;
using urakawa.core.visitor;
using urakawa.exception;
using urakawa.property.channel;

namespace urakawa.property.xml
{
	/// <summary>
	/// Default implementation of <see cref="XmlProperty"/> interface
	/// </summary>
	public class XmlProperty : Property
	{
		private string mLocalName = null;
		private string mNamespaceUri = "";
		private IDictionary<string, XmlAttribute> mAttributes = new Dictionary<string, XmlAttribute>();

		/// <summary>
		/// Gets the local localName of <c>this</c>
		/// </summary>
		/// <returns>The local localName</returns>
		public string getLocalName()
		{
			if (mLocalName == null)
			{
				throw new exception.IsNotInitializedException(
					"The XmlProperty has not been initialized with a local name");
			}
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
		/// Gets the <see cref="IXmlPropertyFactory"/> associated with <c>this</c> via the <see cref="ITreePresentation"/>
		/// of the owning <see cref="TreeNode"/>
		/// </summary>
		/// <returns>The <see cref="IXmlPropertyFactory"/></returns>
		/// <exception cref="exception.FactoryCannotCreateTypeException">
		/// Thrown when the <see cref="IGenericPropertyFactory"/> of the <see cref="ITreePresentation"/>
		/// of the <see cref="TreeNode"/> that owns <c>this</c> is not a subclass of <see cref="IXmlPropertyFactory"/>
		/// </exception>
		/// <remarks>
		/// This method is conveniencs for 
		/// <c>(IXmlPropertyFactory)getTreeNodeOwner().getPresentation().getPropertyFactory()</c></remarks>
		public IXmlPropertyFactory getXmlPropertyFactory()
		{
			IGenericPropertyFactory coreFact = getTreeNodeOwner().getPresentation().getPropertyFactory();
			if (!(coreFact is IXmlPropertyFactory))
			{
				throw new exception.FactoryCannotCreateTypeException(
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
		/// Removes an <see cref="XmlAttribute"/> by QName
		/// </summary>
		/// <param name="localName">The localName part of the QName of the <see cref="XmlAttribute"/> to remove</param>
		/// <param name="namespaceUri">The namespaceUri part of the QName of the <see cref="XmlAttribute"/> to remove</param>
		/// <returns>The removes <see cref="XmlAttribute"/></returns>
		/// <exception cref="exception.XmlAttributeDoesNotExistsException">
		/// Thrown when the <see cref="XmlProperty"/> does not have an event with the given QName</exception>
		public XmlAttribute removeAttribute(string localName, string namespaceUri)
		{
			XmlAttribute attrToRemove = getAttribute(localName, namespaceUri);
			if (attrToRemove == null)
			{
				throw new exception.XmlAttributeDoesNotExistsException(String.Format(
					"The XmlProperty does not have an attribute with QName {1}:{0}", localName, namespaceUri));
			}
			removeAttribute(attrToRemove);
			return attrToRemove;
		}

		/// <summary>
		/// Removes a given <see cref="XmlAttribute"/>
		/// </summary>
		/// <param name="attrToRemove">The <see cref="XmlAttribute"/> to remove</param>
		/// <exception cref="exception.XmlAttributeDoesNotBelongException">
		/// Thrown when the given <see cref="XmlAttribute"/> instance does not belong to the <see cref="XmlProperty"/>
		/// </exception>
		public void removeAttribute(XmlAttribute attrToRemove)
		{
			string key = String.Format("{1}:{0}", attrToRemove.getLocalName(), attrToRemove.getNamespaceUri());
			if (Object.ReferenceEquals(mAttributes[key], attrToRemove))
			{
				mAttributes.Remove(key);
			}
			else
			{
				throw new exception.XmlAttributeDoesNotBelongException(
					"The given XmlAttribute does not belong to the XmlProperty");
			}
		}

		/// <summary>
		/// Sets an <see cref="XmlAttribute"/>, possibly overwriting an existing one
		/// </summary>
		/// <param name="localName">The local localName of the new attribute</param>
		/// <param name="namespaceUri">The namespace uri part of the new attribute</param>
		/// <param name="value">The value of the new attribute</param>
		/// <returns>A <see cref="bool"/> indicating if an existing <see cref="XmlAttribute"/> was overwritten</returns>
		/// <exception cref="exception.FactoryCannotCreateTypeException">
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

		/// <summary>
		/// Gets a copy of <c>this</c>
		/// </summary>
		/// <returns>The copy</returns>
		public new XmlProperty copy()
		{
			return copyProtected() as XmlProperty;
		}

		/// <summary>
		/// Creates a copy of <c>this</c> including copies of any <see cref="XmlAttribute"/>s
		/// </summary>
		/// <returns>The copy</returns>
		protected override Property copyProtected()
		{
			return exportProtected(getPresentation());
		}

		/// <summary>
		/// Creates an export of <c>this</c> for a given destination <see cref="Presentation"/>
		/// </summary>
		/// <param name="destPres">The given destination presentaton</param>
		/// <returns>The exported xml property</returns>
		public new XmlProperty export(Presentation destPres)
		{
			return exportProtected(destPres) as XmlProperty;
		}

		/// <summary>
		/// Creates an export of <c>this</c> for a given destination <see cref="Presentation"/>
		/// </summary>
		/// <param name="destPres">The given destination presentaton</param>
		/// <returns>The exported xml property</returns>
		protected override Property exportProtected(Presentation destPres)
		{
			XmlProperty xmlProp = base.exportProtected(destPres) as XmlProperty;
			if (xmlProp == null)
			{
				throw new exception.FactoryCannotCreateTypeException(String.Format(
					"The property factory can not create an XmlProperty matching QName {0}:{1}",
					getXukNamespaceUri(), getXukLocalName()));
			}
			xmlProp.setQName(getLocalName(), getNamespaceUri());
			foreach (XmlAttribute attr in getListOfAttributes())
			{
				xmlProp.setAttribute(attr.export(destPres, xmlProp));
			}
			return xmlProp;
		}
	
		#region IXUKAble members

		/// <summary>
		/// Reads the attributes of a XmlProperty xuk element.
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected override void xukInAttributes(XmlReader source)
		{
			string ln = source.GetAttribute("localName");
			if (ln == null || ln == "")
			{
				throw new exception.XukException("LocalName attribute is missing from XmlProperty element");
			}
			string ns = source.GetAttribute("namespaceUri");
			if (ns == null) ns = "";
			setQName(ln, ns);
		}

		/// <summary>
		/// Reads a child of a XmlProperty xuk element. 
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected override void xukInChild(XmlReader source)
		{
			bool readItem = false;
			if (source.NamespaceURI == ToolkitSettings.XUK_NS)
			{
				readItem = true;
				switch (source.LocalName)
				{
					case "mXmlAttributes":
						xukInXmlAttributes(source);
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
		}

		private void xukInXmlAttributes(XmlReader source)
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
							attr.xukIn(source);
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
					if (source.EOF) throw new exception.XukException("Unexpectedly reached EOF");
				}
			}
		}

		/// <summary>
		/// Writes the attributes of a XmlProperty element
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <param name="baseUri">
		/// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
		/// if <c>null</c> absolute <see cref="Uri"/>s are written
		/// </param>
		protected override void xukOutAttributes(XmlWriter destination, Uri baseUri)
		{
			base.xukOutAttributes(destination, baseUri);
			destination.WriteAttributeString("localName", getLocalName());
			destination.WriteAttributeString("namespaceUri", getNamespaceUri());
		}

		/// <summary>
		/// Write the child elements of a XmlProperty element.
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <param name="baseUri">
		/// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
		/// if <c>null</c> absolute <see cref="Uri"/>s are written
		/// </param>
		protected override void xukOutChildren(XmlWriter destination, Uri baseUri)
		{
			base.xukOutChildren(destination, baseUri);
			List<XmlAttribute> attrs = getListOfAttributes();
			if (attrs.Count > 0)
			{
				destination.WriteStartElement("mXmlAttributes", ToolkitSettings.XUK_NS);
				foreach (XmlAttribute a in attrs)
				{
					a.xukOut(destination, baseUri);
				}
				destination.WriteEndElement();
			}
		}

		#endregion

		#region IValueEquatable<Property> Members

		/// <summary>
		/// Compares <c>this</c> with another <see cref="Property"/> for equality.
		/// </summary>
		/// <param name="other">The other <see cref="Property"/></param>
		/// <returns><c>true</c> if the <see cref="Property"/>s are equal, otherwise <c>false</c></returns>
		public override bool valueEquals(Property other)
		{
			if (!base.valueEquals(other)) return false;
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

		/// <summary>
		/// Gets a <see cref="string"/> representation of the <see cref="XmlProperty"/>
		/// </summary>
		/// <returns>The <see cref="string"/> representation</returns>
		public override string ToString()
		{
			string displayName = getLocalName();
			if (getNamespaceUri() != "") displayName = getNamespaceUri() + ":" + displayName;
			string attrs = " ";
			foreach (XmlAttribute attr in getListOfAttributes())
			{
				attrs += attr.ToString();
			}
			return String.Format("<{0} {1}/>", displayName, attrs);
		}
	}


}

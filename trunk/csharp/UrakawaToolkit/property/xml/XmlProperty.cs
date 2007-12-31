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
		#region Event related members
		/// <summary>
		/// Event fired after the QName of the <see cref="XmlProperty"/> has changed
		/// </summary>
		public event EventHandler<urakawa.events.property.xml.QNameChangedEventArgs> qNameChanged;
		/// <summary>
		/// Fires the <see cref="qNameChanged"/> event
		/// </summary>
		/// <param name="src">The source, that is the <see cref="XmlProperty"/> whoose QName changed</param>
		/// <param name="newLocalName">The local name part of the new QName</param>
		/// <param name="newNamespaceUri">The namespace uri part of the new QName</param>
		/// <param name="prevLocalName">The local name part of the QName before the change</param>
		/// <param name="prevNamespaceUri">The namespace uri part of the QName before the change</param>
		protected void notifyQNameChanged(XmlProperty src, string newLocalName, string newNamespaceUri, string prevLocalName, string prevNamespaceUri)
		{
			EventHandler<urakawa.events.property.xml.QNameChangedEventArgs> d = qNameChanged;
			if (d != null) d(this, new urakawa.events.property.xml.QNameChangedEventArgs(src, newLocalName, newNamespaceUri, prevLocalName, prevNamespaceUri));
		}

		/// <summary>
		/// Event fired after an attribute of an <see cref="XmlProperty"/> has been set
		/// </summary>
		public event EventHandler<urakawa.events.property.xml.XmlAttributeSetEventArgs> xmlAttributeSet;
		/// <summary>
		/// Fires the <see cref="xmlAttributeSet"/> event
		/// </summary>
		/// <param name="src">The source, that is the <see cref="XmlProperty"/> on which an attribute was set</param>
		/// <param name="attrLN">The local name part of the QName of the attribute that was set</param>
		/// <param name="attrNS">The namespace uri part of the QName of the attribute that was set</param>
		/// <param name="newVal">The new value of the attribute - may be <c>null</c></param>
		/// <param name="prevVal">The previous value of the attribute - may be <c>null</c></param>
		protected void notifyXmlAttributeSet(XmlProperty src, string attrLN, string attrNS, string newVal, string prevVal)
		{
			EventHandler<urakawa.events.property.xml.XmlAttributeSetEventArgs> d = xmlAttributeSet;
			if (d != null) d(this, new urakawa.events.property.xml.XmlAttributeSetEventArgs(src, attrLN, attrNS, newVal, prevVal));
		}

		void this_qNameChanged(object sender, urakawa.events.property.xml.QNameChangedEventArgs e)
		{
			notifyChanged(e);
		}

		void this_xmlAttributeSet(object sender, urakawa.events.property.xml.XmlAttributeSetEventArgs e)
		{
			notifyChanged(e);
		}

		void Attribute_valueChanged(object sender, XmlAttribute.ValueChangedEventArgs e)
		{
			notifyChanged(e);
		}
		#endregion
		/// <summary>
		/// Defayult constructor
		/// </summary>
		internal protected XmlProperty()
		{
			qNameChanged += new EventHandler<urakawa.events.property.xml.QNameChangedEventArgs>(this_qNameChanged);
			xmlAttributeSet += new EventHandler<urakawa.events.property.xml.XmlAttributeSetEventArgs>(this_xmlAttributeSet);
		}

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
		/// Sets the QName of <c>this</c> (i.e. the local localName and namespace uri)
		/// </summary>
		/// <param name="newLocalName">
		/// The local localName part of the new QName
		/// - must not be <c>null</c> or <see cref="String.Empty"/>
		/// </param>
		/// <param name="newNamespaceUri">
		/// The namespace uri part of the new QName - must not be <c>null</c>
		/// </param>
		public void setQName(string newLocalName, string newNamespaceUri)
		{
			if (newLocalName == null)
			{
				throw new exception.MethodParameterIsNullException("The local localName must not be null");
			}
			if (newLocalName == String.Empty)
			{
				throw new exception.MethodParameterIsEmptyStringException("The local localName must not be empty");
			}
			if (newNamespaceUri == null)
			{
				throw new exception.MethodParameterIsNullException("The namespace uri must not be null");
			}
			string prevLN = mLocalName;
			string prevNS = mNamespaceUri;
			mLocalName = newLocalName;
			mNamespaceUri = newNamespaceUri;
			notifyQNameChanged(this, newLocalName, newNamespaceUri, prevLN, prevNS);
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
			string prevValue = null;
			if (mAttributes.ContainsKey(key))
			{
				XmlAttribute prevAttr = mAttributes[key];
				prevValue = prevAttr.getValue();
				removeAttribute(prevAttr);
			}
			mAttributes.Add(key, newAttribute);
			newAttribute.setParent(this);
			newAttribute.valueChanged += new EventHandler<XmlAttribute.ValueChangedEventArgs>(Attribute_valueChanged);
			notifyXmlAttributeSet(this, newAttribute.getLocalName(), newAttribute.getNamespaceUri(), newAttribute.getValue(), prevValue);
			return (prevValue!=null);
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
			if (!Object.ReferenceEquals(mAttributes[key], attrToRemove))
			{
				throw new exception.XmlAttributeDoesNotBelongException(
					"The given XmlAttribute does not belong to the XmlProperty");
			}
			attrToRemove.valueChanged -= new EventHandler<XmlAttribute.ValueChangedEventArgs>(Attribute_valueChanged);
			mAttributes.Remove(key);
			attrToRemove.setParent(null);
			notifyXmlAttributeSet(this, attrToRemove.getLocalName(), attrToRemove.getNamespaceUri(), null, attrToRemove.getValue());
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
			XmlAttribute attr = getAttribute(localName, namespaceUri);
			if (attr == null)
			{
				attr = getPropertyFactory().createXmlAttribute();
				attr.setQName(localName, namespaceUri);
				attr.setValue(value);
				return setAttribute(attr);
			}
			else
			{
				attr.setValue(value);
				return true;
			}
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
		/// Clears the <see cref="XmlAttribute"/> of QName and <see cref="XmlAttribute"/>s
		/// </summary>
		protected override void clear()
		{
			mLocalName = null;
			mNamespaceUri = "";
			foreach (XmlAttribute attr in this.getListOfAttributes())
			{
				removeAttribute(attr);
			}
			base.clear();
		}

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
						XmlAttribute attr = getPropertyFactory().createXmlAttribute(source.LocalName, source.NamespaceURI);
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
			destination.WriteAttributeString("localName", getLocalName());
			destination.WriteAttributeString("namespaceUri", getNamespaceUri());
			base.xukOutAttributes(destination, baseUri);
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
			base.xukOutChildren(destination, baseUri);
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
			string displayName = mLocalName == null ? "null" : mLocalName;
			if (getNamespaceUri() != "") displayName += String.Format(" xmlns='{0}'", getNamespaceUri().Replace("'", "''"));
			string attrs = " ";
			foreach (XmlAttribute attr in getListOfAttributes())
			{

				string attrDisplayName;
				try
				{
					attrDisplayName = attr.getLocalName();
				}
				catch (exception.IsNotInitializedException)
				{
					continue;
				}
				if (attr.getNamespaceUri() != "") attrDisplayName = attr.getNamespaceUri() + ":" + attrDisplayName;
				attrs += String.Format("{0}='{1}'", attrDisplayName, attr.getValue().Replace("'", "''"));
			}
			return String.Format("{0}: <{1} {2}/>", base.ToString(), displayName, attrs);
		}
	}


}

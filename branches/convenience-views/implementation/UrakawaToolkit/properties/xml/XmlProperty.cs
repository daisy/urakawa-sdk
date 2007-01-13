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
	/// Default implementation of <see cref="IXmlProperty"/> interface
	/// </summary>
	public class XmlProperty : IXmlProperty
	{
		private ICoreNode mOwner;
		private string mLocalName = "dummy";
		private string mNamespaceUri = "";
		private IDictionary<string, IXmlAttribute> mAttributes = new Dictionary<string, IXmlAttribute>();

		#region IXmlProperty Members

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
		/// <param localName="newName">
		/// The local localName part of the new QName
		/// - must not be <c>null</c> or <see cref="String.Empty"/>
		/// </param>
		/// <param localName="newNamespace">
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
		/// Gets a list of the <see cref="IXmlAttribute"/>s of <c>this</c>
		/// </summary>
		/// <returns>The list</returns>
		public IList<IXmlAttribute> getListOfAttributes()
		{
			return new List<IXmlAttribute>(mAttributes.Values);
		}

		/// <summary>
		/// Sets an <see cref="IXmlAttribute"/>, possibly overwriting an existing one
		/// </summary>
		/// <param localName="newAttribute">The <see cref="IXmlAttribute"/> to set</param>
		/// <returns>A <see cref="bool"/> indicating if an existing <see cref="IXmlAttribute"/> was overwritten</returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when the <see cref="IXmlAttribute"/> to set is <c>null</c>
		/// </exception>
		public bool setAttribute(IXmlAttribute newAttribute)
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
		/// Sets an <see cref="IXmlAttribute"/>, possibly overwriting an existing one
		/// </summary>
		/// <param localName="localName">The local localName of the new attribute</param>
		/// <param localName="namespaceUri">The namespace uri part of the new attribute</param>
		/// <param localName="value">The value of the new attribute</param>
		/// <returns>A <see cref="bool"/> indicating if an existing <see cref="IXmlAttribute"/> was overwritten</returns>
		/// <exception cref="exception.FactoryCanNotCreateTypeException">
		/// <see cref="getXmlPropertyFactory"/> for information on when this <see cref="Exception"/> is thrown
		/// </exception>
		public bool setAttribute(string localName, string namespaceUri, string value)
		{
			IXmlAttribute newAttribute = getXmlPropertyFactory().createXmlAttribute(this);
			return setAttribute(newAttribute);
		}

		/// <summary>
		/// Gets the <see cref="IXmlAttribute"/> with a given QName
		/// </summary>
		/// <param localName="localName">The local localName part of the given QName</param>
		/// <param localName="namespaceUri">The namespce uri part of the given QName</param>
		/// <returns>The <see cref="IXmlAttribute"/> if found, otherwise <c>null</c></returns>
		public IXmlAttribute getAttribute(string localName, string namespaceUri)
		{
			string key = String.Format("{1}:{0}", localName, namespaceUri);
			if (mAttributes.ContainsKey(key))
			{
				return mAttributes[key];
			}
			return null;
		}

		#endregion

		#region IProperty Members

		IProperty IProperty.copy()
		{
			return copy();
		}

		/// <summary>
		/// Creates a copy of <c>this</c> including copies of any <see cref="IXmlAttribute"/>s
		/// </summary>
		/// <returns>The copy</returns>
		public IXmlProperty copy()
		{
			IProperty prop = this.getOwner().getPresentation().getPropertyFactory().createProperty(
				getXukLocalName(), getXukNamespaceUri());
			if (!(prop is IXmlProperty))
			{
				throw new exception.FactoryCanNotCreateTypeException(String.Format(
					"The property factory can not create a IXmlProperty matching QName {0}:{1}",
					getXukNamespaceUri(), getXukLocalName()));
			}
			IXmlProperty xmlProp = (IXmlProperty)prop;
			xmlProp.setQName(getLocalName(), getNamespaceUri());
			foreach (IXmlAttribute attr in getListOfAttributes())
			{
				xmlProp.setAttribute(attr.copy());
			}
			return xmlProp;
		}

		/// <summary>
		/// Gets the owner <see cref="ICoreNode"/> of <c>this</c>
		/// </summary>
		/// <returns>The owner</returns>
		public ICoreNode getOwner()
		{
			return mOwner;
		}

		/// <summary>
		/// Sets the owner <see cref="ICoreNode"/> of <c>this</c> - intended for internal use,
		/// setting the owner manually may corrupt the core model
		/// </summary>
		/// <param localName="newOwner">The new owner</param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when the new owner is <c>null</c>
		/// </exception>
		public void setOwner(ICoreNode newOwner)
		{
			if (newOwner == null)
			{
				throw new exception.MethodParameterIsNullException("The owner core node can not be null");
			}
			mOwner = newOwner;
		}

		#endregion

		#region IXukAble Members

		/// <summary>
		/// Reads <c>this</c> from an xuk xml element
		/// </summary>
		/// <param localName="source">The source <see cref="XmlReader"/></param>
		/// <returns>A <see cref="bool"/> indicating if the read was succesful</returns>
		public bool XukIn(XmlReader source)
		{
			if (source == null)
			{
				throw new exception.MethodParameterIsNullException("The source XmlReader is null");
			}
			if (source.NodeType != XmlNodeType.Element) return false;
			string ln = source.GetAttribute("localName");
			if (ln == null || ln == String.Empty) return false;
			string ns = source.GetAttribute("namespaceUri");
			if (ns==null) ns = "";
			setQName(ln, ns);
			mAttributes.Clear();
			if (source.IsEmptyElement) return true;
			while (source.Read())
			{
				if (source.NodeType==XmlNodeType.Element)
				{
					IXmlAttribute newAttr = getXmlPropertyFactory().createXmlAttribute(
						this, source.LocalName, source.NamespaceURI);
					if (newAttr==null)
					{
						if (!source.IsEmptyElement)
						{
							//Read past element subtree
							source.ReadSubtree().Close();
						}
					}
					else
					{
						if (!newAttr.XukIn(source)) return false;
						setAttribute(newAttr);
					}
				}
				else if (source.NodeType == XmlNodeType.EndElement)
				{
					break;
				}
				if (source.EOF) break;
			}
			return true;
		}

		/// <summary>
		/// Writes <c>this</c> to a xuk xml element
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <returns>A <see cref="bool"/> indicating if the write was succesful</returns>
		public bool XukOut(XmlWriter destination)
		{
			if (destination == null)
			{
				throw new exception.MethodParameterIsNullException("The destination XmlWriter is null");
			}
			destination.WriteStartElement(getXukLocalName(), getXukNamespaceUri());
			destination.WriteAttributeString("localName", getLocalName());
			if (getNamespaceUri() != String.Empty) destination.WriteAttributeString("namespaceUri", getNamespaceUri());
			foreach (IXmlAttribute attr in mAttributes.Values)
			{
				if (!attr.XukOut(destination)) return false;
			}
			destination.WriteEndElement();
			return true;
		}

		
		/// <summary>
		/// Gets the local localName part of the QName representing a <see cref="XmlProperty"/> in Xuk
		/// </summary>
		/// <returns>The local localName part</returns>
		public string getXukLocalName()
		{
			return this.GetType().Name;
		}

		/// <summary>
		/// Gets the namespace uri part of the QName representing a <see cref="XmlProperty"/> in Xuk
		/// </summary>
		/// <returns>The namespace uri part</returns>
		public string getXukNamespaceUri()
		{
			return urakawa.ToolkitSettings.XUK_NS;
		}

		#endregion

		#region IValueEquatable<IProperty> Members

		/// <summary>
		/// Compares <c>this</c> with another <see cref="IProperty"/> for equality.
		/// </summary>
		/// <param name="other">The other <see cref="IProperty"/></param>
		/// <returns><c>true</c> if the <see cref="IProperty"/>s are equal, otherwise <c>false</c></returns>
		public bool ValueEquals(IProperty other)
		{
			if (!(other is IXmlProperty)) return false;
			IXmlProperty xmlProp = (IXmlProperty)other;
			if (getLocalName() != xmlProp.getLocalName()) return false;
			if (getNamespaceUri() != xmlProp.getNamespaceUri()) return false;
			IList<IXmlAttribute> thisAttrs = getListOfAttributes();
			IList<IXmlAttribute> otherAttrs = xmlProp.getListOfAttributes();
			if (thisAttrs.Count != otherAttrs.Count) return false;
			foreach (IXmlAttribute thisAttr in thisAttrs)
			{
				IXmlAttribute otherAttr = xmlProp.getAttribute(thisAttr.getLocalName(), thisAttr.getNamespaceUri());
				if (otherAttr == null) return false;
				if (otherAttr.getValue() != thisAttr.getValue()) return false;
			} 
			return true;
		}

		#endregion
	}
}

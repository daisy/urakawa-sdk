using System;
using System.Xml;
using urakawa.property;
using urakawa.xuk;

namespace urakawa.property.xml
{
	/// <summary>
	/// Default implementation of <see cref="XmlAttribute"/>
	/// </summary>
	public class XmlAttribute : WithPresentation
	{
		internal class ValueChangedEventArgs : urakawa.events.DataModelChangedEventArgs
		{
			public ValueChangedEventArgs(XmlAttribute src, string newVal, string prevVal) : base(src)
			{
				SourceXmlAttribute = src;
				NewValue = newVal;
				PreviousValue = prevVal;
			}
			public readonly XmlAttribute SourceXmlAttribute;
			public readonly string NewValue;
			public readonly string PreviousValue;
		}

		internal event EventHandler<ValueChangedEventArgs> valueChanged;
		private void notifyValueChanged(XmlAttribute src, string newVal, string prevVal)
		{
			EventHandler<ValueChangedEventArgs> d = valueChanged;
			if (d != null) d(this, new ValueChangedEventArgs(src, newVal, prevVal));
		}

		XmlProperty mParent;
		string mLocalName;
		string mNamespaceUri;
		string mValue;

		/// <summary>
		/// Constructor setting the parent <see cref="XmlProperty"/>
		/// </summary>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when the parent is <c>null</c>
		/// </exception>
		protected internal XmlAttribute()
		{
			mParent = null;
			mLocalName = null;
			mNamespaceUri = "";
			mValue = "";
		}

		#region XmlAttribute Members
		/// <summary>
    /// Creates a copy of the <see cref="XmlAttribute"/>
    /// </summary>
		/// <param name="newParent">The parent xml property of the copy</param>
    /// <returns>The copy</returns>
		/// <exception cref="exception.FactoryCannotCreateTypeException">
		/// Thrown when the <see cref="IGenericPropertyFactory"/> of the <see cref="Presentation"/> 
		/// to which <c>this</c> belongs is not a subclass of <see cref="IXmlPropertyFactory"/>
		/// </exception>
    public XmlAttribute copy(XmlProperty newParent)
		{
			return export(getParent().getPresentation(), newParent);
		}

		/// <summary>
		/// Exports the xml attribute to a given destination presentation 
		/// with a given parent <see cref="XmlProperty"/>
		/// </summary>
		/// <param name="destPres">The given destination presentation</param>
		/// <param name="parent">The given parent xml property</param>
		/// <returns>The exported xml attribute</returns>
		public XmlAttribute export(Presentation destPres, XmlProperty parent)
		{
			if (destPres == null)
			{
				throw new exception.MethodParameterIsNullException(
					"The destination Presentation can not be null");
			}
			if (parent == null)
			{
				throw new exception.MethodParameterIsNullException(
					"The parent XmlProperty can not be null");
			}
			if (parent.getPresentation() != destPres)
			{
				throw new exception.OperationNotValidException(
					"The parent XmlProperty must belong to the destination Presentation");
			}
			string xukLN = getXukLocalName();
			string xukNS = getXukNamespaceUri();
			XmlAttribute exportAttr = destPres.getPropertyFactory().createXmlAttribute(xukLN, xukNS);
			if (exportAttr == null)
			{
				throw new exception.FactoryCannotCreateTypeException(String.Format(
					"The xml property factory does not support creating xml attributes matching QName {0}:{1}", 
					xukLN, xukNS));
			}
			exportAttr.setQName(getLocalName(), getNamespaceUri());
			exportAttr.setValue(getValue());
			return exportAttr;
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
			string prevVal = mValue;
			mValue = newValue;
			if (mValue != prevVal)
			{
				notifyValueChanged(this, mValue, prevVal);
			}
		}

    /// <summary>
    /// Gets the namespace of the <see cref="XmlAttribute"/>
    /// </summary>
    /// <returns>The namespace</returns>
    public string getNamespaceUri()
		{
				return mNamespaceUri;
		}

    /// <summary>
    /// Gets the local localName of the <see cref="XmlAttribute"/>
    /// </summary>
    /// <returns>The local localName</returns>
    public string getLocalName()
		{
			if (mLocalName == null)
			{
				throw new exception.IsNotInitializedException(
					"The XmlAttribute has not been initialized with a local name");
			}
			return mLocalName;
		}

    /// <summary>
    /// Sets the QName of the <see cref="XmlAttribute"/> 
    /// </summary>
    /// <param name="newNamespaceUri">The namespace part of the new QName</param>
    /// <param name="newLocalName">The localName part of the new QName</param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Throw when <paramref name="newNamespaceUri"/> or <paramref name="newLocalName"/> is <c>null</c>
		/// </exception>
		/// <exception cref="exception.MethodParameterIsEmptyStringException">
		/// Thrown when <paramref name="newLocalName"/> is an <see cref="String.Empty"/>
		/// </exception>
		/// <remarks>
		/// If the <see cref="XmlAttribute"/> has already been set on a <see cref="XmlProperty"/>,
		/// setting the QName will overwrite any <see cref="XmlAttribute"/> of the owning <see cref="XmlProperty"/>
		/// with matching QName
		/// </remarks>
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
			if (newLocalName != mLocalName || newNamespaceUri != mNamespaceUri)
			{
				XmlProperty parent = getParent();
				if (parent != null)
				{
					parent.removeAttribute(this);
				}
				mLocalName = newLocalName;
				mNamespaceUri = newNamespaceUri;
				if (parent != null)
				{
					parent.setAttribute(this);
				}
			}
		}

    /// <summary>
    /// Gets the parent <see cref="XmlProperty"/> of <c>this</c>
    /// </summary>
    /// <returns></returns>
    public XmlProperty getParent()
		{
			return mParent;
		}

		/// <summary>
		/// Sets the parent <see cref="XmlProperty"/> of <c>this</c>. 
		/// Is intended for internal use by the owning <see cref="XmlProperty"/>,
		/// calling this method may lead to corruption of the data model
		/// </summary>
		/// <param name="newParent">The new parent</param>
		public void setParent(XmlProperty newParent)
		{
			mParent = newParent;
		}
		#endregion

		#region IXUKAble members 

		/// <summary>
		/// Clears the <see cref="XmlAttribute"/> data
		/// </summary>
		protected override void clear()
		{
			if (getParent() != null)
			{
				getParent().removeAttribute(this);
			}
			mLocalName = null;
			mNamespaceUri = "";
			mValue = "";
			base.clear();
		}

		/// <summary>
		/// Reads the attributes of a XmlAttribute xuk element.
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected override void xukInAttributes(XmlReader source)
		{
			string name = source.GetAttribute("localName");
			if (name == null || name == "")
			{
				throw new exception.XukException("LocalName attribute of XmlAttribute element is missing");
			}
            string value = source.GetAttribute("Value");
            if (value == null)
            {
                throw new exception.XukException("Value attribute of XmlAttribute element is missing");
            }
            mValue = value;

			string ns = source.GetAttribute("namespaceUri");
			if (ns == null) ns = "";
			setQName(name, ns);
		}

		/// <summary>
		/// Writes the attributes of a XmlAttribute element
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <returns>A <see cref="bool"/> indicating if the write was succesful</returns>
		/// <param name="baseUri">
		/// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
		/// if <c>null</c> absolute <see cref="Uri"/>s are written
		/// </param>
		protected override void xukOutAttributes(XmlWriter destination, Uri baseUri)
		{
			//localName is required
			if (mLocalName == "")
			{
				throw new exception.XukException("The XmlAttribute has no name");
			}
			destination.WriteAttributeString("localName", mLocalName);

            if (mValue == null)
            {
                throw new exception.XukException("The XmlAttribute has no value");
            }
            destination.WriteAttributeString("Value", mValue);

			if (mNamespaceUri != "") destination.WriteAttributeString("namespaceUri", mNamespaceUri);
			base.xukOutAttributes(destination, baseUri);
		}
		#endregion

		/// <summary>
		/// Gets a <see cref="string"/> representation of the attribute
		/// </summary>
		/// <returns>The <see cref="string"/> representation</returns>
		public override string ToString()
		{
			string displayName = mLocalName==null?"null":mLocalName;
			if (getNamespaceUri() != "") displayName = getNamespaceUri() + ":" + displayName;
			return String.Format("{1}: {2}='{3}'", base.ToString(), displayName, getValue().Replace("'", "''"));
		}

	}
}

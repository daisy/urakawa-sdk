using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace urakawa.core.property
{
	/// <summary>
	/// Implementation of <see cref="Property"/> that in it self does nothing. 
	/// This class is intended as a base class for built-in or custom implementations of <see cref="Property"/>
	/// </summary>
	public class Property
	{
		/// <summary>
		/// Default constructor - should only be used from subclass constructors or <see cref="ICorePropertyFactory"/>s
		/// </summary>
		protected internal Property()
		{
		}

		private ICoreNode mOwner = null;

		#region Property Members

		/// <summary>
		/// Creates a copy of the property
		/// </summary>
		/// <returns>The copy</returns>
		/// <exception cref="exception.IsNotInitializedException">
		/// Thrown if the property has not been initialized with an owning <see cref="ICoreNode"/>
		/// </exception>
		/// <exception cref="exception.FactoryCanNotCreateTypeException">
		/// Thrown if the <see cref="ICorePropertyFactory"/> associated with the property via. it's owning <see cref="ICoreNode"/>
		/// can not create an <see cref="Property"/> mathcing the Xuk QName of <c>this</c>
		/// </exception>
		public virtual Property copy()
		{
			Property theCopy = getOwner().getPresentation().getPropertyFactory().createProperty(
				getXukLocalName(), getXukNamespaceUri());
			if (theCopy == null)
			{
				throw new exception.FactoryCanNotCreateTypeException(String.Format(
					"The PropertyFactory can not create a Property of type matching QName {1}:{0}",
					getXukLocalName(), getXukNamespaceUri()));
			}
			return theCopy;
		}

		/// <summary>
		/// Gets the owner <see cref="ICoreNode"/> of the property
		/// </summary>
		/// <returns>The owner</returns>
		public ICoreNode getOwner()
		{
			if (mOwner == null)
			{
				throw new exception.IsNotInitializedException(
					"The Property has not been initialized with an owning ICoreNode");
			}
			return mOwner;
		}

		/// <summary>
		/// Sets the owner <see cref="ICoreNode"/> of the property - for internal use only
		/// </summary>
		/// <param name="newOwner">The new owner</param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when the new owner <see cref="ICoreNode"/> is <c>null</c></exception>
		public virtual void setOwner(ICoreNode newOwner)
		{
			if (newOwner == null)
			{
				throw new exception.MethodParameterIsNullException("The owner ICoreNode can not be null");
			}
			mOwner = newOwner;
		}

		#endregion
		
		#region IXUKAble members

		/// <summary>
		/// Reads the <see cref="Property"/> from a Property xuk element
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		/// <returns>A <see cref="bool"/> indicating if the read was succesful</returns>
		public bool XukIn(XmlReader source)
		{
			if (source == null)
			{
				throw new exception.MethodParameterIsNullException("Can not XukIn from an null source XmlReader");
			}
			if (source.NodeType != XmlNodeType.Element) return false;
			if (!XukInAttributes(source)) return false;
			if (!source.IsEmptyElement)
			{
				while (source.Read())
				{
					if (source.NodeType == XmlNodeType.Element)
					{
						if (!XukInChild(source)) return false;
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
		/// Reads the attributes of a Property xuk element.
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		/// <returns>A <see cref="bool"/> indicating if the attributes was succefully read</returns>
		protected virtual bool XukInAttributes(XmlReader source)
		{
			// Read known attributes


			return true;
		}

		/// <summary>
		/// Reads a child of a Property xuk element. 
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		/// <returns>A <see cref="bool"/> indicating if the child was succefully read</returns>
		protected virtual bool XukInChild(XmlReader source)
		{
			bool readItem = false;
			// Read known children, when read set readItem to true


			if (!(readItem || source.IsEmptyElement))
			{
				source.ReadSubtree().Close();//Read past unknown child 
			}
			return true;
		}

		/// <summary>
		/// Write a Property element to a XUK file representing the <see cref="Property"/> instance
		/// </summary>
		/// <param localName="destination">The destination <see cref="XmlWriter"/></param>
		/// <returns>A <see cref="bool"/> indicating if the write was succesful</returns>
		public bool XukOut(XmlWriter destination)
		{
			if (destination == null)
			{
				throw new exception.MethodParameterIsNullException(
					"Can not XukOut to a null XmlWriter");
			}
			destination.WriteStartElement(getXukLocalName(), getXukNamespaceUri());
			if (!XukOutAttributes(destination)) return false;
			if (!XukOutChildren(destination)) return false;
			destination.WriteEndElement();
			return true;
		}

		/// <summary>
		/// Writes the attributes of a Property element
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <returns>A <see cref="bool"/> indicating if the write was succesful</returns>
		protected virtual bool XukOutAttributes(XmlWriter destination)
		{
			return true;
		}

		/// <summary>
		/// Write the child elements of a Property element.
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <returns>A <see cref="bool"/> indicating if the write was succesful</returns>
		protected virtual bool XukOutChildren(XmlWriter destination)
		{
			// Write children
			return true;
		}

		/// <summary>
		/// Gets the local name part of the QName representing a <see cref="Property"/> in Xuk
		/// </summary>
		/// <returns>The local name part</returns>
		public virtual string getXukLocalName()
		{
			return this.GetType().Name;
		}

		/// <summary>
		/// Gets the namespace uri part of the QName representing a <see cref="Property"/> in Xuk
		/// </summary>
		/// <returns>The namespace uri part</returns>
		public virtual string getXukNamespaceUri()
		{
			return urakawa.ToolkitSettings.XUK_NS;
		}

		#endregion

		#region IValueEquatable<Property> Members

		/// <summary>
		///	Determines if a given other <see cref="Property"/> has the same value as <c>this</c>
		/// </summary>
		/// <param name="other">The other property</param>
		/// <returns>A <see cref="bool"/> indicating the value equality</returns>
		public virtual bool ValueEquals(Property other)
		{
			if (!this.GetType().IsInstanceOfType(other)) return false;
			return true;
		}

		#endregion
	}
}

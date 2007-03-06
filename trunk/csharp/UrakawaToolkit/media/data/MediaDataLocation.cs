using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace urakawa.media.data
{
	/// <summary>
	/// Default implementation of <see cref="IMediaDataLocation"/>
	/// </summary>
	public class MediaDataLocation : IMediaDataLocation
	{
		private IMediaData mMediaData;
		private IMediaFactory mMediaFactory;
		private IMediaDataFactory mMediaDataFactory;

		/// <summary>
		/// Constructor associating <c>this</c> with a <see cref="IMediaFactory"/> and a <see cref="IMediaDataFactory"/>
		/// </summary>
		/// <param name="mediaFact"></param>
		/// <param name="mediaDataFact"></param>
		internal protected MediaDataLocation(IMediaFactory mediaFact, IMediaDataFactory mediaDataFact)
		{
			mMediaData = null;
			mMediaFactory = mediaFact;
			mMediaDataFactory = mediaDataFact;
		}

		#region IMediaDataLocation Members


		/// <summary>
		/// Gets the <see cref="IMediaData"/> pointed to by <c>this</c>
		/// </summary>
		/// <returns>The <see cref="IMediaData"/></returns>
		public IMediaData getMediaData()
		{
			return mMediaData;
		}

		/// <summary>
		/// Sets the <see cref="IMediaData"/> pointed to by this
		/// </summary>
		/// <param name="newData">The new <see cref="IMediaData"/> that <c>this</c> should point to</param>
		public void setMediaData(IMediaData newData)
		{
			mMediaData = newData;
		}

		#endregion

		#region IMediaLocation Members

		IMediaLocation IMediaLocation.copy()
		{
			return copy();
		}

		/// <summary>
		/// Creates a copy of this including a copy of the <see cref="IMediaData"/> pointed to by <c>this</c>
		/// </summary>
		/// <returns>The copy</returns>
		public IMediaDataLocation copy()
		{
			IMediaLocation oCopy = getMediaFactory().createMediaLocation(getXukLocalName(), getXukNamespaceUri());
			if (!(oCopy is IMediaDataLocation))
			{
				throw new exception.FactoryCanNotCreateTypeException(String.Format(
					"The MediaFactory can not create a {0}", GetType().Name));
			}
			IMediaDataLocation copy = (IMediaDataLocation)oCopy;
			IMediaData data = getMediaData();
			if (data != null) copy.setMediaData(data.copy());
			return copy;
		}

		/// <summary>
		/// Return the <see cref="IMediaFacotry"/> associated with <c>this</c>
		/// </summary>
		/// <returns>The <see cref="IMediaFactory"/></returns>
		public IMediaFactory getMediaFactory()
		{
			return mMediaFactory;
		}


		/// <summary>
		/// Gets the <see cref="IMediaDataFactory"/> associated with <c>this</c>
		/// </summary>
		/// <returns>The <see cref="IMediaDataFactory"/></returns>
		public IMediaDataFactory getMediaDataFactory()
		{
			throw new Exception("The method or operation is not implemented.");
		}


		#endregion

		#region IXukAble Members



		/// <summary>
		/// Reads the <see cref="MediaDataLocation"/> from a MediaDataLocation xuk element
		/// </summary>
		/// <param localName="source">The source <see cref="System.Xml.XmlReader"/></param>
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
		/// Reads the attributes of a MediaDataLocation xuk element.
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		/// <returns>A <see cref="bool"/> indicating if the attributes was succefully read</returns>
		protected virtual bool XukInAttributes(XmlReader source)
		{
			// Read known attributes


			return true;
		}

		/// <summary>
		/// Reads a child of a MediaDataLocation xuk element. 
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		/// <returns>A <see cref="bool"/> indicating if the child was succefully read</returns>
		protected virtual bool XukInChild(XmlReader source)
		{
			bool readItem = false;
			// Read known children, when read set readItem to true


			if (!(readItem || source.IsEmptyElement))
			{
				source.ReadSubtree().Close();//Read past invalid MediaDataItem element
			}
			return true;
		}

		/// <summary>
		/// Write a MediaDataLocation element to a XUK file representing the <see cref="MediaDataLocation"/> instance
		/// </summary>
		/// <param localName="destination">The destination <see cref="System.Xml.XmlWriter"/></param>
		/// <returns>A <see cref="bool"/> indicating if the write was succesful</returns>
		public bool XukOut(System.Xml.XmlWriter destination)
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
		/// Writes the attributes of a MediaDataLocation element
		/// </summary>
		/// <param localName="destination">The destination <see cref="XmlWriter"/></param>
		/// <returns>A <see cref="bool"/> indicating if the write was succesful</returns>
		protected virtual bool XukOutAttributes(XmlWriter destination)
		{
			return true;
		}

		/// <summary>
		/// Write the child elements of a MediaDataLocation element.
		/// </summary>
		/// <param localName="destination">The destination <see cref="XmlWriter"/></param>
		/// <returns>A <see cref="bool"/> indicating if the write was succesful</returns>
		protected virtual bool XukOutChildren(XmlWriter destination)
		{
			// Write children
			return true;
		}
		
		/// <summary>
		/// Gets the local name part of the QName representing a <see cref="MediaDataLocation"/> in Xuk
		/// </summary>
		/// <returns>The local name part</returns>
		public string getXukLocalName()
		{
			return this.GetType().Name;
		}

		/// <summary>
		/// Gets the namespace uri part of the QName representing a <see cref="MediaDataLocation"/> in Xuk
		/// </summary>
		/// <returns>The namespace uri part</returns>
		public string getXukNamespaceUri()
		{
			return urakawa.ToolkitSettings.XUK_NS;
		}

		#endregion

		#region IValueEquatable<IMediaLocation> Members

		/// <summary>
		/// Determines if another <see cref="IMediaLocation"/> has the same value as <c>this</c>
		/// </summary>
		/// <param name="other">The other <see cref="IMediaLocation"/></param>
		/// <returns><c>true</c> if <c>this</c>  and <paramref name="other"/> have the same value, 
		/// <c>false</c> else</returns>
		public bool ValueEquals(IMediaLocation other)
		{
			if (other is IMediaDataLocation)
			{
				IMediaDataLocation otherMDL = (IMediaDataLocation)other;
				IMediaData data = getMediaData();
				if (data == null)
				{
					return otherMDL.getMediaData() == null;
				}
				else
				{
					return data.ValueEquals(otherMDL.getMediaData());
				}
			}
			return false;
		}

		#endregion
	}
}

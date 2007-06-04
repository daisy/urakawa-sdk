using System;
using System.Collections.Generic;
using System.Xml;

namespace urakawa.media
{

	/// <summary>
	/// SequenceMedia is a collection of same-type media objects
	/// The first object in the collection determines the collection's type.
	/// </summary>
	public class SequenceMedia : IMedia
	{
		private List<IMedia> mSequence;
		private IMediaFactory mMediaFactory;

		/// <summary>
		/// Constructor setting the associated <see cref="IMediaFactory"/>
		/// </summary>
		/// <param name="fact">
		/// The <see cref="IMediaFactory"/> to associate the <see cref="SequenceMedia"/> with
		/// </param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref localName="fact"/> is <c>null</c>
		/// </exception>
		protected internal SequenceMedia(IMediaFactory fact)
		{
			mSequence = new List<IMedia>();
			if (fact == null)
			{
				throw new exception.MethodParameterIsNullException("Factory is null");
			}
			mMediaFactory = fact;
		}

		#region SequenceMedia Members

		/// <summary>
		/// Get the item at the given index
		/// </summary>
		/// <param name="index">Index of the item to return</param>
		/// <returns>The <see cref="IMedia"/> item at the given index</returns>
		/// <exception cref="exception.MethodParameterIsOutOfBoundsException">
		/// Thrown when the given index is out of bounds
		/// </exception>
		public IMedia getItem(int index)
		{
			if (0<=index && index<getCount())
			{
				return (IMedia)mSequence[index];
			}
			else
			{
				throw new exception.MethodParameterIsOutOfBoundsException("SequenceMedia.getItem(" +
					index.ToString() + ") caused MethodParameterIsOutOfBoundsException");
			}
		}


		/// <summary>
		/// Inserts a given <see cref="IMedia"/> item at a given index
		/// </summary>
		/// <param name="index">The given index</param>
		/// <param name="newItem">The given <see cref="IMedia"/> item</param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when the given <see cref="IMedia"/> to insert is <c>null</c>
		/// </exception>
		/// <exception cref="exception.MethodParameterIsOutOfBoundsException">
		/// Thrown when the given index is out of bounds
		/// </exception>
		/// <exception cref="exception.MediaTypeIsIllegalException">
		/// The <see cref="IMedia"/> item to insert has a <see cref="MediaType"/> that 
		/// is incompatible with the <see cref="SequenceMedia"/>
		/// </exception>
		/// <remarks>
		/// The first <see cref="IMedia"/> inserted into an <see cref="SequenceMedia"/> 
		/// determines it's <see cref="MediaType"/>. 
		/// Prior to the first insertion an <see cref="SequenceMedia"/> has <see cref="MediaType"/>
		/// <see cref="MediaType.EMPTY_SEQUENCE"/>
		/// </remarks>
		public void insertItem(int index, IMedia newItem)
		{
			if (index < 0 || getCount() < index)
			{
				throw new exception.MethodParameterIsOutOfBoundsException(
					"The index at which to insert media is out of bounds");
			}
			if (!isAllowed(newItem))
			{
				throw new exception.MediaTypeIsIllegalException(
					"The new media to insert is of a type that is incompatible with the sequence media");
			}
			mSequence.Insert(index, newItem);
		}

		/// <summary>
		/// Appends a new <see cref="IMedia"/> item to the end of the sequence
		/// </summary>
		/// <param name="newItem">The new item</param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when the given <see cref="IMedia"/> to append is <c>null</c>
		/// </exception>
		/// <exception cref="exception.MethodParameterIsWrongTypeException">
		/// The <see cref="IMedia"/> item to append has a <see cref="MediaType"/> that 
		/// is incompatible with the <see cref="SequenceMedia"/>
		/// </exception>
		public void appendItem(IMedia newItem)
		{
			insertItem(getCount(), newItem);
		}

		/// <summary>
		/// Remove an item from the sequence.
		/// </summary>
		/// <param name="index">The index of the item to remove.</param>
		/// <returns>The removed <see cref="IMedia"/> item</returns>
		/// <exception cref="exception.MethodParameterIsOutOfBoundsException">
		/// Thrown when the given index is out of bounds
		/// </exception>
		public IMedia removeItem(int index)
		{
			IMedia removedMedia = getItem(index);
			mSequence.RemoveAt(index);
			return removedMedia;
		}

		/// <summary>
		/// Return the number of items in the sequence.
		/// </summary>
		/// <returns>The number of items</returns>
		public int getCount()
		{
			return mSequence.Count;
		}

		/// <summary>
		/// Gets a list of the <see cref="IMedia"/> items in the sequence
		/// </summary>
		/// <returns>The list</returns>
		public List<IMedia> getListOfItems()
		{
			return new List<IMedia>(mSequence);
		}

		#endregion

		#region IMedia Members


		/// <summary>
		/// Gets the <see cref="IMediaFactory"/> associated with the <see cref="SequenceMedia"/>
		/// </summary>
		/// <returns>The <see cref="IMediaFactory"/></returns>
		public IMediaFactory getMediaFactory()
		{
			return mMediaFactory;
		}


		/// <summary>
		/// Use the first item in the collection to determine if this sequence is continuous or not.
		/// </summary>
		/// <returns></returns>
		public bool isContinuous()
		{
			if (getCount() > 0)
			{
				return getItem(0).isContinuous();
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Use the first item in the collection to determine if this 
		/// sequence is discrete or not.
		/// </summary>
		/// <returns></returns>
		public bool isDiscrete()
		{
			//use the first item in the collection to determine the value
			if (getCount() > 0)
			{
				return getItem(0).isDiscrete();
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// This function always returns true, because this 
		/// object is always considered to be a sequence (even if it contains only one item).
		/// </summary>
		/// <returns><c>true</c></returns>
		public bool isSequence()
		{
			return true;
		}

		/// <summary>
		/// If the sequence is non-empty, then this function will return the <see cref="MediaType"/> of
		/// <see cref="IMedia"/> items it contains (it will only contain one type at a time)
		/// If the sequence is empty, this function will return <see cref="MediaType.EMPTY_SEQUENCE"/>.
		/// </summary>
		/// <returns>The <see cref="MediaType"/></returns>
		public MediaType getMediaType()
		{
			//use the first item in the collection to determine the value
			if (getCount() > 0)
			{
				return getItem(0).getMediaType();
			}
			else
			{
				return MediaType.EMPTY_SEQUENCE;
			}
		}

		IMedia IMedia.copy()
		{
			return copy();
		}

		/// <summary>
		/// Make a copy of this media sequence
		/// </summary>
		/// <returns>The copy</returns>
		public SequenceMedia copy()
		{
			IMedia newMedia = getMediaFactory().createMedia(
				getXukLocalName(), getXukNamespaceUri());
			if (!(newMedia is SequenceMedia))
			{
				throw new exception.FactoryCanNotCreateTypeException(String.Format(
					"The media factory can not create an SequenceMedia matching QName {0}:{1}",
					getXukLocalName(), getXukNamespaceUri()));
			}
			SequenceMedia newSeqMedia = (SequenceMedia)newMedia;
			foreach (IMedia item in mSequence)
			{
				newSeqMedia.insertItem(newSeqMedia.getCount(), item.copy());
			}
			return newSeqMedia;
		}

		#endregion


		/// <summary>
		/// test a new media object to see if it can belong to this collection 
		/// (only objects of the same type are allowed)
		/// </summary>
		/// <param name="proposedAddition"></param>
		/// <returns></returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when the proposed addition is null
		/// </exception>
		private bool isAllowed(IMedia proposedAddition)
		{
			if (proposedAddition == null)
			{
				throw new exception.MethodParameterIsNullException(
					"The proposed addition is null");
			}
			if (getMediaType() == MediaType.EMPTY_SEQUENCE) return true;
			return (getMediaType() == proposedAddition.getMediaType());
		}

		private string getTypeAsString()
		{
			MediaType type = this.getMediaType();
			switch (type)
			{
				case MediaType.EMPTY_SEQUENCE:
					return String.Empty;
				default:
					return type.ToString("g");
			}
		}

		
		#region IXUKAble members

		/// <summary>
		/// Reads the <see cref="SequenceMedia"/> from a SequenceMedia xuk element
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
			mSequence.Clear();
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
		/// Reads the attributes of a SequenceMedia xuk element.
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		/// <returns>A <see cref="bool"/> indicating if the attributes was succefully read</returns>
		protected virtual bool XukInAttributes(XmlReader source)
		{
			// NO known attributes
			return true;
		}

		/// <summary>
		/// Reads a child of a SequenceMedia xuk element. 
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		/// <returns>A <see cref="bool"/> indicating if the child was succefully read</returns>
		protected virtual bool XukInChild(XmlReader source)
		{
			bool readItem = false;
			if (source.NamespaceURI == ToolkitSettings.XUK_NS)
			{
				readItem = true;
				switch (source.LocalName)
				{
					case "mSequence":
						if (!XukInSequence(source)) return false;
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

		private bool XukInSequence(XmlReader source)
		{
			if (!source.IsEmptyElement)
			{
				while (source.Read())
				{
					if (source.NodeType == XmlNodeType.Element)
					{
						IMedia newMedia = getMediaFactory().createMedia(source.LocalName, source.NamespaceURI);
						if (newMedia != null)
						{
							if (!isAllowed(newMedia)) return false;
							if (newMedia.XukIn(source)) return false;
							insertItem(getCount(), newMedia);
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
		/// Write a SequenceMedia element to a XUK file representing the <see cref="SequenceMedia"/> instance
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
		/// Writes the attributes of a SequenceMedia element
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <returns>A <see cref="bool"/> indicating if the write was succesful</returns>
		protected virtual bool XukOutAttributes(XmlWriter destination)
		{
			return true;
		}

		/// <summary>
		/// Write the child elements of a SequenceMedia element.
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <returns>A <see cref="bool"/> indicating if the write was succesful</returns>
		protected virtual bool XukOutChildren(XmlWriter destination)
		{
			if (getCount() > 0)
			{
				destination.WriteStartElement("mSequence", ToolkitSettings.XUK_NS);
				for (int i = 0; i < getCount(); i++)
				{
					if (!getItem(i).XukOut(destination)) return false;
				}
				destination.WriteEndElement();
			}
			return true;
		}

		/// <summary>
		/// Gets the local name part of the QName representing a <see cref="SequenceMedia"/> in Xuk
		/// </summary>
		/// <returns>The local name part</returns>
		public virtual string getXukLocalName()
		{
			return this.GetType().Name;
		}

		/// <summary>
		/// Gets the namespace uri part of the QName representing a <see cref="SequenceMedia"/> in Xuk
		/// </summary>
		/// <returns>The namespace uri part</returns>
		public virtual string getXukNamespaceUri()
		{
			return urakawa.ToolkitSettings.XUK_NS;
		}

		#endregion

		#region IValueEquatable<IMedia> Members

		/// <summary>
		/// Conpares <c>this</c> with a given other <see cref="IMedia"/> for equality
		/// </summary>
		/// <param name="other">The other <see cref="IMedia"/></param>
		/// <returns><c>true</c> if equal, otherwise <c>false</c></returns>
		public bool ValueEquals(IMedia other)
		{
			if (other == null) return false;
			if (other.GetType() != this.GetType()) return false;
			if (!(other is SequenceMedia)) return false;
			SequenceMedia otherSeq = (SequenceMedia)other;
			if (getCount() != otherSeq.getCount()) return false;
			for (int i = 0; i < getCount(); i++)
			{
				if (!getItem(i).ValueEquals(otherSeq.getItem(i))) return false;
			}
			return true;
		}

		#endregion
	}
}

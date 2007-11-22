using System;
using System.Collections.Generic;
using System.Xml;

namespace urakawa.media
{

	/// <summary>
	/// SequenceMedia is a collection of same-type media objects
	/// The first object in the collection determines the collection's type.
	/// </summary>
	public class SequenceMedia : AbstractMedia
	{
		
		#region Event related members
		#endregion

		private List<IMedia> mSequence;
		private bool mAllowMultipleTypes;

		/// <summary>
		/// Constructor setting the associated <see cref="IMediaFactory"/>
		/// </summary>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref localName="fact"/> is <c>null</c>
		/// </exception>
		protected internal SequenceMedia() : base()
		{
			mSequence = new List<IMedia>();
			mAllowMultipleTypes = false;
		}

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
		/// <exception cref="exception.MediaNotAcceptable">
		///	Thrown if the <see cref="SequenceMedia"/> can not accept the media
		/// </exception>
		public void insertItem(int index, IMedia newItem)
		{
			if (newItem == null)
			{
				throw new exception.MethodParameterIsNullException("The new item can not be null");
			}
			if (index < 0 || getCount() < index)
			{
				throw new exception.MethodParameterIsOutOfBoundsException(
					"The index at which to insert media is out of bounds");
			}
			if (!canAcceptMedia(newItem))
			{
				throw new exception.MediaNotAcceptable(
					"The new media to insert is of a type that is incompatible with the sequence media");
			}
			mSequence.Insert(index, newItem);
		}

		/// <summary>
		/// Appends a new <see cref="IMedia"/> item to the end of the sequence
		/// </summary>
		/// <param name="newItem">The new item</param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when the given <see cref="IMedia"/> to insert is <c>null</c>
		/// </exception>
		/// <exception cref="exception.MediaNotAcceptable">
		///	Thrown if the <see cref="SequenceMedia"/> can not accept the media
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

		/// <summary>
		/// Gets a <see cref="bool"/> indicating if multiple <see cref="IMedia"/> types are allowed in the sequence
		/// </summary>
		/// <returns>The <see cref="bool"/></returns>
		public bool getAllowMultipleTypes()
		{
			return mAllowMultipleTypes;
		}

		/// <summary>
		/// Sets a <see cref="bool"/> indicating if multiple <see cref="IMedia"/> types are allowed in the sequence
		/// </summary>
		/// <param name="newValue">The new <see cref="bool"/> value</param>
		public void setAllowMultipleTypes(bool newValue)
		{
			if (!newValue)
			{
				int count = getCount();
				if (count > 0)
				{
					Type firstItemType = getItem(0).GetType();
					int i = 1;
					while (i < count)
					{
						if (getItem(i).GetType() != firstItemType)
						{
							throw new exception.OperationNotValidException(
								"Can not prohibit multiple IMedia types in the sequence, since the Type of the sequence items differ");
						}
					}
				}
			}
			mAllowMultipleTypes = newValue;
		}

		#region IMedia Members


		/// <summary>
		/// Use the first item in the collection to determine if this sequence is continuous or not.
		/// </summary>
		/// <returns></returns>
		public override bool isContinuous()
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
		public override bool isDiscrete()
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
		public override bool isSequence()
		{
			return true;
		}

		/// <summary>
		/// Make a copy of this media sequence
		/// </summary>
		/// <returns>The copy</returns>
		public new SequenceMedia copy()
		{
			return copyProtected() as SequenceMedia;
		}

		/// <summary>
		/// Make a copy of this media sequence
		/// </summary>
		/// <returns>The copy</returns>
		protected override IMedia copyProtected()
		{
			IMedia newMedia = getMediaFactory().createMedia(
				getXukLocalName(), getXukNamespaceUri());
			if (!(newMedia is SequenceMedia))
			{
				throw new exception.FactoryCannotCreateTypeException(String.Format(
					"The media factory can not create an SequenceMedia matching QName {0}:{1}",
					getXukLocalName(), getXukNamespaceUri()));
			}
			SequenceMedia newSeqMedia = (SequenceMedia)newMedia;
			foreach (IMedia item in getListOfItems())
			{
				newSeqMedia.appendItem(item.copy());
			}
			return newSeqMedia;
		}

		/// <summary>
		/// Exports the sequence media to a destination <see cref="Presentation"/>
		/// </summary>
		/// <param name="destPres">The destination presentation</param>
		/// <returns>The exported sequence media</returns>
		public new SequenceMedia export(Presentation destPres)
		{
			return exportProtected(destPres) as SequenceMedia;
		}

		/// <summary>
		/// Exports the sequence media to a destination <see cref="Presentation"/>
		/// </summary>
		/// <param name="destPres">The destination presentation</param>
		/// <returns>The exported sequence media</returns>
		protected override IMedia exportProtected(Presentation destPres)
		{
			SequenceMedia exported = destPres.getMediaFactory().createMedia(
				getXukLocalName(), getXukNamespaceUri()) as SequenceMedia;
			if (exported == null)
			{
				throw new exception.FactoryCannotCreateTypeException(String.Format(
					"The MediaFacotry cannot create a SequenceMedia matching QName {1}:{0}",
					getXukLocalName(), getXukNamespaceUri()));
			}
			foreach (IMedia m in getListOfItems())
			{
				exported.appendItem(m.export(destPres));
			}
			return exported;
		}


		#endregion


		/// <summary>
		/// Test a new media object to see if it can belong to this collection 
		/// (optionally a sequence can allow only a single <see cref="Type"/> of <see cref="IMedia"/>)
		/// </summary>
		/// <param name="proposedAddition">The media to test</param>
		/// <returns></returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when the proposed addition is null
		/// </exception>
		public virtual bool canAcceptMedia(IMedia proposedAddition)
		{
			if (proposedAddition == null)
			{
				throw new exception.MethodParameterIsNullException(
					"The proposed addition is null");
			}
			if (getCount()>0 && !getAllowMultipleTypes())
			{
				if (getItem(0).GetType() != proposedAddition.GetType()) return false;
			}
			return true;
		}

		
		#region IXUKAble members

		/// <summary>
		/// Clears/resets the <see cref="SequenceMedia"/> 
		/// </summary>
		protected override void clear()
		{
			mAllowMultipleTypes = false;
			mSequence.Clear();
			base.clear();
		}

		/// <summary>
		/// Reads the attributes of a SequenceMedia xuk element.
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected override void xukInAttributes(XmlReader source)
		{
			string val = source.GetAttribute("allowMultipleMediaTypes");
			if (val == "true" || val == "1")
			{
				setAllowMultipleTypes(true);
			}
			else
			{
				setAllowMultipleTypes(false);
			}
			base.xukInAttributes(source);
		}

		/// <summary>
		/// Reads a child of a SequenceMedia xuk element. 
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
					case "mSequence":
						xukInSequence(source);
						break;
					default:
						readItem = false;
						break;
				}
			}
			if (!readItem) base.xukIn(source);
		}

		private void xukInSequence(XmlReader source)
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
							newMedia.xukIn(source);
							if (!canAcceptMedia(newMedia))
							{
								throw new exception.XukException(
									String.Format("Media type {0} is not supported by the sequence", newMedia.GetType().FullName));
							}
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
					if (source.EOF) throw new exception.XukException("Unexpectedly reached EOF");
				}
			}
		}

		/// <summary>
		/// Writes the attributes of a SequenceMedia element
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <param name="baseUri">
		/// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
		/// if <c>null</c> absolute <see cref="Uri"/>s are written
		/// </param>
		protected override void xukOutAttributes(XmlWriter destination, Uri baseUri)
		{
			destination.WriteAttributeString("allowMultipleMediaTypes", getAllowMultipleTypes() ? "true" : "false");
			base.xukOutAttributes(destination, baseUri);
		}

		/// <summary>
		/// Write the child elements of a SequenceMedia element.
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <param name="baseUri">
		/// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
		/// if <c>null</c> absolute <see cref="Uri"/>s are written
		/// </param>
		protected override void xukOutChildren(XmlWriter destination, Uri baseUri)
		{
			if (getCount() > 0)
			{
				destination.WriteStartElement("mSequence", ToolkitSettings.XUK_NS);
				for (int i = 0; i < getCount(); i++)
				{
					getItem(i).xukOut(destination, baseUri);
				}
				destination.WriteEndElement();
			}
			base.xukOutChildren(destination, baseUri);
		}
		#endregion

		#region IValueEquatable<IMedia> Members

		/// <summary>
		/// Conpares <c>this</c> with a given other <see cref="IMedia"/> for equality
		/// </summary>
		/// <param name="other">The other <see cref="IMedia"/></param>
		/// <returns><c>true</c> if equal, otherwise <c>false</c></returns>
		public override bool valueEquals(IMedia other)
		{
			if (!base.valueEquals(other)) return false;
			SequenceMedia otherSeq = (SequenceMedia)other;
			if (getCount() != otherSeq.getCount()) return false;
			for (int i = 0; i < getCount(); i++)
			{
				if (!getItem(i).valueEquals(otherSeq.getItem(i))) return false;
			}
			return true;
		}

		#endregion
	}
}

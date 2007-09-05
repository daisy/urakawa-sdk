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
		private bool mAllowMultipleTypes;
		private string mLanguage;

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
			mAllowMultipleTypes = false;
			mLanguage = null;
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

		/// <summary>
		/// Gets a <see cref="bool"/> indicating if multiple <see cref="IMedia"/> types are allowed in the sequence
		/// </summary>
		/// <returns></returns>
		public bool getAllowMultipleTypes()
		{
			return mAllowMultipleTypes;
		}

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
		/// Sets the language of the sequence media
		/// </summary>
		/// <param name="lang">The new language, can be null but not an empty string</param>
		public void setLanguage(string lang)
		{
			if (lang == "")
			{
				throw new exception.MethodParameterIsEmptyStringException(
					"The language can not be an empty string");
			}
			mLanguage = lang;
		}

		/// <summary>
		/// Gets the language of the sequence media
		/// </summary>
		/// <returns>The language</returns>
		public string getLanguage()
		{
			return mLanguage;
		}


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


		IMedia IMedia.export(Presentation destPres)
		{
			return export(destPres);
		}

		/// <summary>
		/// Exports the sequence media to a destination <see cref="Presentation"/>
		/// </summary>
		/// <param name="destPres">The destination presentation</param>
		/// <returns>The exported sequence media</returns>
		public SequenceMedia export(Presentation destPres)
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
		/// Reads the <see cref="SequenceMedia"/> from a SequenceMedia xuk element
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		public void XukIn(XmlReader source)
		{
			if (source == null)
			{
				throw new exception.MethodParameterIsNullException("Can not XukIn from an null source XmlReader");
			}
			if (source.NodeType != XmlNodeType.Element)
			{
				throw new exception.XukException("Can not read SequenceMedia from a non-element node");
			}
			try
			{
				mSequence.Clear();
				XukInAttributes(source);
				if (!source.IsEmptyElement)
				{
					while (source.Read())
					{
						if (source.NodeType == XmlNodeType.Element)
						{
							XukInChild(source);
						}
						else if (source.NodeType == XmlNodeType.EndElement)
						{
							break;
						}
						if (source.EOF) throw new exception.XukException("Unexpectedly reached EOF");
					}
				}

			}
			catch (exception.XukException e)
			{
				throw e;
			}
			catch (Exception e)
			{
				throw new exception.XukException(
					String.Format("An exception occured during XukIn of SequenceMedia: {0}", e.Message),
					e);
			}
		}

		/// <summary>
		/// Reads the attributes of a SequenceMedia xuk element.
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected virtual void XukInAttributes(XmlReader source)
		{
			string val = source.GetAttribute("AllowMultipleMediaTypes");
			if (val == "true" || val == "1")
			{
				setAllowMultipleTypes(true);
			}
			else
			{
				setAllowMultipleTypes(false);
			}
			string lang = source.GetAttribute("Language").Trim();
			if (lang != "") lang = null;
			setLanguage(lang);
		}

		/// <summary>
		/// Reads a child of a SequenceMedia xuk element. 
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected virtual void XukInChild(XmlReader source)
		{
			bool readItem = false;
			if (source.NamespaceURI == ToolkitSettings.XUK_NS)
			{
				readItem = true;
				switch (source.LocalName)
				{
					case "mSequence":
						XukInSequence(source);
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

		private void XukInSequence(XmlReader source)
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
							newMedia.XukIn(source);
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
		/// Write a SequenceMedia element to a XUK file representing the <see cref="SequenceMedia"/> instance
		/// </summary>
		/// <param localName="destination">The destination <see cref="XmlWriter"/></param>
		public void XukOut(XmlWriter destination)
		{
			if (destination == null)
			{
				throw new exception.MethodParameterIsNullException(
					"Can not XukOut to a null XmlWriter");
			}

			try
			{
				destination.WriteStartElement(getXukLocalName(), getXukNamespaceUri());
				XukOutAttributes(destination);
				XukOutChildren(destination);
				destination.WriteEndElement();

			}
			catch (exception.XukException e)
			{
				throw e;
			}
			catch (Exception e)
			{
				throw new exception.XukException(
					String.Format("An exception occured during XukOut of SequenceMedia: {0}", e.Message),
					e);
			}
		}

		/// <summary>
		/// Writes the attributes of a SequenceMedia element
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		protected virtual void XukOutAttributes(XmlWriter destination)
		{
			destination.WriteAttributeString("AllowMultipleMediaTypes", getAllowMultipleTypes() ? "true" : "false");
			if (getLanguage() != null) destination.WriteAttributeString("Language", getLanguage());
		}

		/// <summary>
		/// Write the child elements of a SequenceMedia element.
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		protected virtual void XukOutChildren(XmlWriter destination)
		{
			if (getCount() > 0)
			{
				destination.WriteStartElement("mSequence", ToolkitSettings.XUK_NS);
				for (int i = 0; i < getCount(); i++)
				{
					getItem(i).XukOut(destination);
				}
				destination.WriteEndElement();
			}
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
		public bool valueEquals(IMedia other)
		{
			if (other == null) return false;
			if (getLanguage() != other.getLanguage()) return false;
			if (GetType() != other.GetType()) return false;
			if (!(other is SequenceMedia)) return false;
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

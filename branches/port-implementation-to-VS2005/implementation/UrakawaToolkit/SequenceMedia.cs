using System;
using System.Collections;
using System.Xml;

namespace urakawa.media
{

	/// <summary>
	/// SequenceMedia is a collection of same-type media objects
	/// The first object in the collection determines the collection's type.
	/// </summary>
	public class SequenceMedia : ISequenceMedia
	{
		private IList mSequence;
		private IMediaFactory mMediaFactory;
		
		/// <summary>
		/// The default constructor.
		/// </summary>
		/// <param name="factory">The presentation's media factory</param>
		protected SequenceMedia(IMediaFactory factory)
		{
			mSequence = new ArrayList();

			if (factory == null)
			{
				throw new exception.MethodParameterIsNullException("Factory is null");
			}

			mMediaFactory = factory;
		}

		internal static SequenceMedia create(IMediaFactory factory)
		{
			return new SequenceMedia(factory);
		}

		#region ISequenceMedia Members
		
		/// <summary>
		/// Get the item at the given index
		/// </summary>
		/// <param name="index">Index of the item to return</param>
		/// <returns></returns>
		public IMedia getItem(int index)
		{
			if (isInRange(index) == true)
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
		/// Set the value of item at an index if its type is allowed and the index is in range.
		/// </summary>
		/// <param name="index">Insertion point.</param>
		/// <param name="newItem">New media item</param>
		/// <returns></returns>
		public IMedia setItem(int index, IMedia newItem)
		{
			//first check to see if the new item is null
			if (newItem == null)
			{
				throw new exception.MethodParameterIsNullException("SequenceMedia.setItem(" + 
					index.ToString() + ", null) caused MethodParameterIsNullException");
			}

			//then see if it is allowed, and if the specified position is in range
			bool isItemAllowed = isAllowed(newItem);
			bool isIndexInRange = isInRange(index);

			if (newItem != null && isItemAllowed == true && isIndexInRange == true)
			{
				Object replacedMedia = mSequence[index];

				mSequence[index] = newItem;

				return (IMedia)replacedMedia;
			}
			else
			{
				if (isItemAllowed == false)
				{
					throw new exception.MediaTypeIsIllegalException("SequenceMedia.setItem(" + 
						index.ToString() + ", " + newItem.ToString() + 
						" ) caused MediaTypeIsIllegalException");
				}
				
				if (isIndexInRange == false)
				{
					throw new exception.MethodParameterIsOutOfBoundsException
						("SequenceMedia.setItem(" + 
						index.ToString() + ", " + newItem.ToString() + 
						" ) caused MethodParameterIsOutOfBoundsException");
				}

				return null;
			}
		}

		/// <summary>
		/// Append a media object to the sequence.
		/// If the sequence already contains media objects, this new addition
		/// must be of the same type.
		/// This function throws the exceptions: 
		/// <see cref="urakawa.exception.MethodParameterIsNullException"/>, 
		/// <see cref="urakawa.exception.MediaTypeIsIllegalException"/>, 
		/// <see cref="urakawa.exception.MethodParameterIsOutOfBoundsException"/>
		/// </summary>
		/// <param name="newItem"></param>
		public void appendItem(IMedia newItem)
		{
			//first check to see if the new item is null
			if (newItem == null)
			{
				throw new exception.MethodParameterIsNullException
					("Item to be appended is null");
			}

			//then check to see if its type is allowed in this list
			if (isAllowed(newItem) == true)
			{
				mSequence.Add(newItem);
			}
			else
			{
				string tmp = "";

				if (mSequence.Count > 0)
				{
					tmp = mSequence[0].GetType().Name;
				}

				throw new exception.MediaTypeIsIllegalException(newItem.GetType().Name + 
					" is not allowed in this sequence, because it already contains one or more items" + 
					" of type " + tmp);
			}
		}

		/// <summary>
		/// Remove an item from the sequence.
		/// </summary>
		/// <param name="index">The index of the item to remove.</param>
		/// <returns></returns>
		public IMedia removeItem(int index)
		{
			//remove the item if it is in range
			if (isInRange(index) == true)
			{
				IMedia removedMedia = (IMedia)getItem(index);
				mSequence.RemoveAt(index);

				return removedMedia;
			}
			else
			{
				throw new exception.MethodParameterIsOutOfBoundsException
					(index.ToString() +  " is out of bounds in this sequence");
			}
		}

		/// <summary>
		/// Return the number of items in the sequence.
		/// </summary>
		/// <returns></returns>
		public int getCount()
		{
			return mSequence.Count;
		}

		#endregion

		#region IMedia Members

		/// <summary>
		/// Use the first item in the collection to determine if this sequence is continuous or not.
		/// </summary>
		/// <returns></returns>
		public bool isContinuous()
		{
			if (mSequence.Count > 0)
			{
				return ((IMedia)mSequence[0]).isContinuous();
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
			if (mSequence.Count > 0)
			{
				return ((IMedia)mSequence[0]).isDiscrete();
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
		/// <returns></returns>
		public bool isSequence()
		{
			return true;
		}

		/// <summary>
		/// If the sequence is non-empty, then this function will return the type of
		/// media objects it contains (it will only contain one type at a time)
		/// If the sequence is empty, this function will return <see cref="MediaType.EMPTY_SEQUENCE"/>.
		/// </summary>
		/// <returns></returns>
		public urakawa.media.MediaType getType()
		{
			//use the first item in the collection to determine the value
			if (mSequence.Count > 0)
			{
				return ((IMedia)mSequence[0]).getType();
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
		/// <returns></returns>
		public SequenceMedia copy()
		{
			SequenceMedia newMedia = new SequenceMedia(this.mMediaFactory);
			
			for (int i = 0; i<this.getCount(); i++)
			{
				newMedia.appendItem(this.getItem(i).copy());
			}

			return newMedia;
		}

		#endregion

		#region IXUKable members 

		/// <summary>
		/// Fill in audio data from an XML source.
		/// Assume that the XmlReader cursor is at the opening audio tag.
		/// </summary>
		/// <param name="source">the input XML source</param>
		/// <returns>true or false, depending on whether the data could be processed</returns>
		public bool XUKin(System.Xml.XmlReader source)
		{
			if (source == null)
			{
				throw new exception.MethodParameterIsNullException("Xml Reader is null");
			}

			if (!(source.Name == "SequenceMedia" && source.NodeType == System.Xml.XmlNodeType.Element))
			{
				return false;
			}

			//System.Diagnostics.Debug.WriteLine("XUKin: SequenceMedia");


      MediaType mt;
      try
      {
        mt = (MediaType)Enum.Parse(typeof(MediaType), source.GetAttribute("type"), false);
      }
      catch (Exception)
      {
        return false;
      }


      if (source.IsEmptyElement) 
      {
        return (mt==MediaType.EMPTY_SEQUENCE);
      }
      bool bFoundError = false;
      while (source.Read())
      {
        if (source.NodeType==XmlNodeType.Element)
        {
          MediaType newMediaType = MediaType.EMPTY_SEQUENCE;
          switch (source.LocalName)
          {
            case "Media":
              string type = source.GetAttribute("type");
              try
              {
                newMediaType = (MediaType)Enum.Parse(typeof(MediaType), type, false);
              }
              catch (Exception)
              {
                bFoundError = true;
              }
              break;
            case "SequenceMedia":
              newMediaType = MediaType.EMPTY_SEQUENCE;
              break;
            default:
              bFoundError = true;
              break;
          }
          if (!bFoundError)
          {
            IMedia newMedia = mMediaFactory.createMedia(newMediaType);
			
			  if (((urakawa.core.IXUKable)newMedia).XUKin(source))
			  {
					if (this.getType()==MediaType.EMPTY_SEQUENCE || this.getType()==newMedia.getType())
					{
						this.appendItem(newMedia);
					}
					else 
					{
						bFoundError = true;
					}
			  }
			  else
			  {
				  bFoundError = true;
			  }
          }
        }
        else if (source.NodeType==XmlNodeType.EndElement)
        {
          break;
        }
        if (source.EOF) break;
        if (bFoundError) break;
      }
      return !bFoundError;
	}

		
		/// <summary>
		/// The opposite of <see cref="XUKin"/>, this function writes the object's data
		/// to an XML file
		/// </summary>
		/// <param name="destination">the XML source for outputting data</param>
		/// <returns>false if the sequence is empty, otherwise true</returns>
		public bool XUKout(System.Xml.XmlWriter destination)
		{
			if (destination == null)
			{
				throw new exception.MethodParameterIsNullException("Xml Writer is null");
			}

			//empty sequences are not allowed
			if (mSequence.Count == 0)
				return false;

			destination.WriteStartElement("SequenceMedia");
			
			destination.WriteAttributeString("type", this.getTypeAsString());

			bool bWroteMedia = true;

			for (int i = 0; i<this.mSequence.Count; i++)
			{
				IMedia media = (IMedia)mSequence[i];

				bool bTmp = ((urakawa.core.IXUKable)media).XUKout(destination);
				
				bWroteMedia = bTmp && bWroteMedia;
				
			}

			destination.WriteEndElement();

			return bWroteMedia;
		}
		#endregion

		/// <summary>
		/// test a new media object to see if it can belong to this collection 
		/// (only objects of the same type are allowed)
		/// </summary>
		/// <param name="proposedAddition"></param>
		/// <returns></returns>
		private bool isAllowed(IMedia proposedAddition)
		{
			if (mSequence.Count > 0)
			{
				if (((IMedia)mSequence[0]).getType() == proposedAddition.getType())
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				return true;
			}
		}

		/// <summary>
		/// test an index value to see if it is in range
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		private bool isInRange(int index)
		{
			if (index < mSequence.Count && index >= 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		private string getTypeAsString()
		{
			MediaType type = this.getType();

			if (type == MediaType.AUDIO)
				return "AUDIO";
			else if (type == MediaType.VIDEO)
				return "VIDEO";
			else if (type == MediaType.IMAGE)
				return "IMAGE";
			else if (type == MediaType.TEXT)
				return "TEXT";
			else 
				return "";
		}

	}
}

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
		
		public SequenceMedia(IMediaFactory factory)
		{
			mSequence = new ArrayList();

			if (factory == null)
			{
				throw new exception.MethodParameterIsNullException("Factory is null");
			}

			mMediaFactory = factory;
		}

		/// <summary>
		/// this override is useful while debugging
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return "SequenceMedia";
		}

		#region ISequenceMedia Members
		
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

		public void appendItem(IMedia newItem)
		{
			//first check to see if the new item is null
			if (newItem == null)
			{
				throw new exception.MethodParameterIsNullException("SequenceMedia.appendItem(null) caused MethodParameterIsNullException");
			}

			//then check to see if its type is allowed in this list
			if (isAllowed(newItem) == true)
			{
				mSequence.Add(newItem);
			}
			else
			{
				throw new exception.MediaTypeIsIllegalException("SequenceMedia.appendItem(" + 
					newItem.ToString() + " ) caused MediaTypeIsIllegalException");
			}
		}

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
					("SequenceMedia.removeItem(" + index.ToString() +  
					" ) caused MethodParameterIsOutOfBoundsException");
			}
		}

		public int getCount()
		{
			return mSequence.Count;
		}

		#endregion

		#region IMedia Members

		
		public bool isContinuous()
		{
			//use the first item in the collection to determine the value
			if (mSequence.Count > 0)
			{
				return ((IMedia)mSequence[0]).isContinuous();
			}
			else
			{
				return false;
			}
		}

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

		//@todo
		//should this return false if there is only one item in the sequence?
		//my inclination is to leave it as it is.
		public bool isSequence()
		{
			return true;
		}

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

			System.Diagnostics.Debug.WriteLine("XUKin: SequenceMedia");


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
            if (newMedia.XUKin(source))
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

//			bool bFoundMedia = false;
//			bool bProcessedMedia = false;
//
//			//loop and look for all Media elements
//			while (!(source.Name == "SequenceMedia" && 
//				source.NodeType == System.Xml.XmlNodeType.EndElement)
//				&&
//				source.EOF == false)
//			{
//				source.Read();
//
//				if (source.Name == "Media" && 
//					source.NodeType == System.Xml.XmlNodeType.Element)
//				{
//					string mediaType = source.GetAttribute("type");
//					IMedia newMedia = null;
//
//					if (mediaType == "AUDIO")
//					{
//						newMedia = this.mPresentation.getMediaFactory().createMedia(MediaType.AUDIO);
//					}
//					else if (mediaType == "VIDEO")
//					{
//						newMedia = this.mPresentation.getMediaFactory().createMedia(MediaType.VIDEO);
//					}
//					else if (mediaType == "TEXT")
//					{
//						newMedia = this.mPresentation.getMediaFactory().createMedia(MediaType.TEXT);
//					}
//					else if (mediaType == "IMAGE")
//					{
//						newMedia = this.mPresentation.getMediaFactory().createMedia(MediaType.IMAGE);
//					}
//					else
//					{
//						//not supported
//						return false;
//					}
//
//					if (newMedia != null)
//					{
//						bool bTmpVal = newMedia.XUKin(source);
//
//						//if this is our first time through the loop, simply record
//						//the value of the XUKin processing (from the line above)
//						if (bFoundMedia == false)
//						{
//							bProcessedMedia = bTmpVal;
//						}
//							//else, accumulate our return value by combining it with the previous value
//						else
//						{
//							bProcessedMedia = bProcessedMedia && bTmpVal;
//						}
//
//						if (bTmpVal == true)
//						{
//							this.appendItem(newMedia);
//						}
//
//						bFoundMedia = true;
//					}
//				}
//			}
//			
//			//we should have found one or more media element(s)
//			//and should have successfully processed all of them
//			return (bFoundMedia && bProcessedMedia);
		}

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
				bool bTmp = media.XUKout(destination);

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

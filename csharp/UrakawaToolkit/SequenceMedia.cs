using System;
using System.Collections;

namespace urakawa.media
{

	/// <summary>
	/// SequenceMedia is a collection of same-type media objects
	/// The first object in the collection determines the collection's type.
	/// </summary>
	public class SequenceMedia : ISequenceMedia
	{
		private ArrayList mSequence = new ArrayList();
		
		public SequenceMedia()
		{
		}

		/// <summary>
		/// this override is useful while debugging
		/// </summary>
		/// <returns></returns>
		public string ToString()
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
				return null;
			}
		}

		public IMedia setItem(int index, IMedia newItem)
		{
			//first check to see if the new item is null
			if (newItem == null)
			{
				throw new exception.MethodParameterIsNullException("SequenceMedia.setItem(" + 
					index.ToString() + ", null) caused MethodParameterIsNullException");

				return null;
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

				return;
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
				
				return;
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
				return null;
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
	}
}

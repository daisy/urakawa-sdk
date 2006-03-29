using System;
using System.Collections;

namespace urakawa.media
{

	public class SequenceMedia : IMedia
	{
		private ArrayList mSequence = new ArrayList();
		
		public SequenceMedia()
		{
		}

		public IMedia getItem(int index)
		{
			if (isInRange(index) == true)
			{
				return (IMedia)mSequence[index];
			}
			else
			{
				return null;
			}
		}

		public void setItem(int index, IMedia newItem)
		{
			if (isAllowed(newItem) == true && isInRange(index) == true)
			{
				mSequence[index] = newItem;
			}
		}

		public void appendItem(IMedia newItem)
		{
			if (isAllowed(newItem) == true)
			{
				mSequence.Add(newItem);
			}
		}

		public IMedia removeItem(int index)
		{
			if (isInRange(index) == true)
			{
				IMedia removedMedia = (IMedia)getItem(index);
				mSequence.RemoveAt(index);

				return removedMedia;
			}
			else
			{
				return null;
			}
		}

		public int getCount()
		{
			return mSequence.Count;
		}
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
				return MediaType.NONE;
			}
		}

		#endregion

		/// <summary>
		/// test a new media object to see if it can belong to this collection (only objects of the same type are allowed)
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

using System;
using System.Collections;

namespace urakawa.core.media
{

	public class SequenceMediaObject
	{
		private ArrayList mSequence = new ArrayList();
		
		public SequenceMediaObject()
		{
		}

		public IMediaObject getItem(int index)
		{
			if (index >=0 && index < mSequence.Count)
			{
				return (IMediaObject)mSequence[index];
			}
			else
			{
				return null;
			}
		}

		public void setItem(int index, IMediaObject newItem)
		{
			mSequence[index] = newItem;
		}

		public void appendItem(IMediaObject newItem)
		{
			mSequence.Add(newItem);
		}

		public IMediaObject removeItem(int index)
		{
			if (index >=0 && index < mSequence.Count)
			{
				IMediaObject removedMedia = (IMediaObject)getItem(index);
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
	}
}

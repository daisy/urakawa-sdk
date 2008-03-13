using System;
using System.Collections.Generic;
using System.Text;

namespace WaveFormDisplay
{


	public class WaveFormGraphData
	{
		private class SubArrayCollection : IList<int>
		{
			public SubArrayCollection(int[,] data, int dim0Index, int defaultValue)
			{
				if (data == null) throw new ArgumentNullException();
				if (data.Rank != 2) throw new ArgumentException("Invalid rank - must be 2");
				if (dim0Index < data.GetLowerBound(0) || data.GetUpperBound(0) < dim0Index)
				{
					throw new ArgumentException(String.Format(
						"Invalid dimension 0 index - must be in [{0};{1}]",
						data.GetUpperBound(0), data.GetUpperBound(0)));
				}
				if (data.GetLowerBound(1)!=0)
				{
					throw new ArgumentException(String.Format("Invalid dimension 1 lower bounds {0}", data.GetLowerBound(1)));
				}
				mData = data;
				mDin0Index = dim0Index;
				mDefaultValue = defaultValue;
			}

			private int[,] mData;
			private int mDin0Index, mDefaultValue;

			private IList<int> GetAsList()
			{
				List<int> res = new List<int>();
				for (int i = 0; i <= mData.GetUpperBound(1); i++)
				{
					res.Add(mData[mDin0Index, i]);
				}
				return res;
			}

			#region ICollection<int> Members

			public void Add(int item)
			{
				throw new NotSupportedException("The collectio is read-only");
			}

			public void Clear()
			{
				throw new NotSupportedException("The collectio is read-only");
			}

			public bool Contains(int item)
			{
				if (item == 0) return true;
				return GetAsList().Contains(item);
			}

			public void CopyTo(int[] array, int arrayIndex)
			{
				GetAsList().CopyTo(array, arrayIndex);
			}

			public int Count
			{
				get { return mData.GetUpperBound(1); }
			}

			public bool IsReadOnly
			{
				get { return true; }
			}

			public bool Remove(int item)
			{
				throw new NotSupportedException("The collectio is read-only");
			}

			#endregion

			#region IEnumerable<int> Members

			public IEnumerator<int> GetEnumerator()
			{
				return GetAsList().GetEnumerator();
			}

			#endregion

			#region IEnumerable Members

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			#endregion

			#region IList<int> Members

			public int IndexOf(int item)
			{
				return GetAsList().IndexOf(item);
			}

			public void Insert(int index, int item)
			{
				throw new NotSupportedException("The collectio is read-only");
			}

			public void RemoveAt(int index)
			{
				throw new NotSupportedException("The collectio is read-only");
			}

			public int this[int index]
			{
				get
				{
					if (0 <= index && index < Count)
					{
						return mData[mDin0Index, index];
					}
					return mDefaultValue;
				}
				set
				{
					if (0 <= index && index < Count)
					{
						mData[mDin0Index, index] = value;
					}
				}
			}

			#endregion
		}

		private int[,] mData;

		public WaveFormGraphData(int w)
		{
			if (w < 0) w = 0;
			mData = new int[2, w];
		}

		public int Width { get { return mData.Length; } }

		public IList<int> Mins
		{
			get
			{
				return new SubArrayCollection(mData, 0, 0);
			}
		}

		public IList<int> Maxs
		{
			get
			{
				return new SubArrayCollection(mData, 1, 0);
			}
		}


	}
}

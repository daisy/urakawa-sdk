using System;
using System.Collections.Generic;
using System.Text;

namespace NAudio.Dmo
{
    /// <summary>
    /// Media Object Size Info
    /// </summary>
    public class MediaObjectSizeInfo
    {
        private int _size;

        /// <summary>
        /// Minimum Buffer Size, in bytes
        /// </summary>
        public int Size
        {
            get { return _size; }
            private set { _size = value; }
        }

        private int _maxLookahead;

        /// <summary>
        /// Max Lookahead
        /// </summary>
        public int MaxLookahead
        {
            get { return _maxLookahead; }
            private set { _maxLookahead = value; }
        }

        private int _alignment;

        /// <summary>
        /// Alignment
        /// </summary>
        public int Alignment
        {
            get { return _alignment; }
            private set { _alignment = value; }
        }

        /// <summary>
        /// Media Object Size Info
        /// </summary>
        public MediaObjectSizeInfo(int size, int maxLookahead, int alignment)
        {
            Size = size;
            MaxLookahead = maxLookahead;
            Alignment = alignment;
        }

        /// <summary>
        /// ToString
        /// </summary>        
        public override string ToString()
        {
            return String.Format("Size: {0}, Alignment {1}, MaxLookahead {2}",
                Size, Alignment, MaxLookahead);
        }

    }
}

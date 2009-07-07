using System;
using System.Collections.Generic;
using System.Text;

namespace NAudio.Wave
{
    /// <summary>
    /// An interface for WaveStreams which can report notification of individual samples
    /// </summary>
    public interface ISampleNotifier
    {
        /// <summary>
        /// About to start processing a block of samples
        /// </summary>
        event EventHandler Block;
        /// <summary>
        /// A sample has been detected
        /// </summary>
        event EventHandler<SampleEventArgs> Sample;
    }

    /// <summary>
    /// Sample event arguments
    /// </summary>
    public class SampleEventArgs : EventArgs
    {
        private float _left;

        /// <summary>
        /// Left sample
        /// </summary>
        public float Left
        {
            get { return _left; }
            set { _left = value; }
        }

        private float _right;

        /// <summary>
        /// Right sample
        /// </summary>
        public float Right
        {
            get { return _right; }
            set { _right = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public SampleEventArgs(float left, float right)
        {
            this.Left = left;
            this.Right = right;
        }
    }
}

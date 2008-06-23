using System;
using System.Collections.Generic;
using System.Text;

namespace AudioEngine.PPMeter
{
	/// <summary>
	/// Arguments for the <see cref="PPMeter.PeakOverloadIndicatorClicked"/> event
	/// </summary>
	public class PeakOverloadIndicatorClickedEventArgs : EventArgs
	{
		/// <summary>
		/// Constructor setting the channel whoose peak overload indicator was clicked
		/// </summary>
		/// <param name="ch">The channel whoose peak overload indicator was clicked</param>
		public PeakOverloadIndicatorClickedEventArgs(int ch)
		{
			ChannelNumber = ch;
		}
		/// <summary>
		/// Gets the channel whoose peak overload indicator was clicked
		/// </summary>
		public readonly int ChannelNumber;
	}
}

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace AudioEngine
{
	/// <summary>
	/// <see cref="EventArgs"/> for <see cref="IAudioDevice.Time"/> events
	/// </summary>
	public class TimeEventArgs : EventArgs
	{

		/// <summary>
		/// Gets the current time position of the source <see cref="IAudioDevice"/>
		/// </summary>
		/// <returns>The current time</returns>
		public readonly TimeSpan CurrentTimePosition;

		/// <summary>
		/// Gets the maximal Db value for each channel in the source <see cref="IAudioDevice"/>
		/// since the latest previous <see cref="IAudioDevice.Time"/> event
		/// </summary>
		public readonly double[] MaxDbSinceLatestTime;

		/// <summary>
		/// Constructor setting the current time position of the source <see cref="IAudioDevice"/>
		/// and the max Db value for each channel of audio since the latest previous <see cref="IAudioDevice.Time"/> event
		/// </summary>
		/// <param name="curPos">The current time position</param>
		/// <param name="maxDbs">The max Db values for each channel of audio</param>
		public TimeEventArgs(TimeSpan curPos, double[] maxDbs)
		{
			CurrentTimePosition = curPos;
			MaxDbSinceLatestTime = maxDbs;
		}
	}

	/// <summary>
	/// <see cref="EventArgs"/> for <see cref="IAudioDevice.StateChanged"/> events
	/// </summary>
	public class StateChangedEventArgs : EventArgs
	{

		/// <summary>
		/// Gets the <see cref="AudioDeviceState"/> before the change
		/// </summary>
		public readonly AudioDeviceState PreviousState;

		/// <summary>
		/// Constructor setting the previous <see cref="AudioDeviceState"/>
		/// </summary>
		/// <param name="prevState">The previous state</param>
		public StateChangedEventArgs(AudioDeviceState prevState) : base()
		{
			PreviousState = prevState;
		}
	}

	/// <summary>
	/// <see cref="EventArgs"/> for <see cref="IPlaybackAudioDevice.PlayEnded"/> events
	/// </summary>
	public class EndedEventArgs : EventArgs
	{

		/// <summary>
		/// Default constructor that initializes the end time
		/// </summary>
		public EndedEventArgs(TimeSpan endTime, Stream pcmStream)
		{
			EndTime = endTime;
			PCMStream = pcmStream;
		}

		/// <summary>
		/// Gets the PCM <see cref="Stream"/> that was played/recorded
		/// </summary>
		public readonly Stream PCMStream;

		/// <summary>
		/// Gets the time of end of playback
		/// </summary>
		public readonly TimeSpan EndTime;
	}

	public class OverloadEventArgs : EventArgs
	{
		public OverloadEventArgs(ushort ch, TimeSpan tm)
		{
			Channel = ch;
			OverloadTime = tm;
		}
		public readonly ushort Channel;
		public readonly TimeSpan OverloadTime;
	}
}
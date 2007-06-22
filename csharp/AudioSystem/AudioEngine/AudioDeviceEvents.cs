using System;
using System.Collections.Generic;
using System.Text;

namespace AudioEngine
{
	/// <summary>
	/// <see cref="EventArgs"/> for <see cref="IAudioDevice.Time"/> events
	/// </summary>
	public class TimeEventArgs : EventArgs
	{
		private TimeSpan mCurrentTimePosition;
		private double[] mMaxDbSinceLatestTime;

		/// <summary>
		/// Gets the current time position of the source <see cref="IAudioDevice"/>
		/// </summary>
		/// <returns>The current time</returns>
		public TimeSpan getCurrentTimePosition()
		{
			return mCurrentTimePosition;
		}

		/// <summary>
		/// Gets the maximal Db value for each channel in the source <see cref="IAudioDevice"/>
		/// since the latest previous <see cref="IAudioDevice.Time"/> event
		/// </summary>
		/// <returns>The maximal Db values in an array - <c>null</c> if the values are not available</returns>
		public double[] getMaxDbSinceLatestTime()
		{
			return mMaxDbSinceLatestTime;
		}

		/// <summary>
		/// Constructor setting the current time position of the source <see cref="IAudioDevice"/>
		/// and the max Db value for each channel of audio since the latest previous <see cref="IAudioDevice.Time"/> event
		/// </summary>
		/// <param name="curPos">The current time position</param>
		/// <param name="maxDbs">The max Db values for each channel of audio</param>
		public TimeEventArgs(TimeSpan curPos, double[] maxDbs)
		{
			mCurrentTimePosition = curPos;
			mMaxDbSinceLatestTime = maxDbs;
		}
	}

	/// <summary>
	/// <see cref="EventArgs"/> for <see cref="IAudioDevice.StateChanged"/> events
	/// </summary>
	public class StateChangedEventArgs : EventArgs
	{
		private AudioDeviceState mPreviousState;

		/// <summary>
		/// Gets the <see cref="AudioDeviceState"/> before the change
		/// </summary>
		/// <returns>The previous <see cref="AudioDeviceState"/></returns>
		public AudioDeviceState getPreviousState()
		{
			return mPreviousState;
		}

		/// <summary>
		/// Constructor setting the previous <see cref="AudioDeviceState"/>
		/// </summary>
		/// <param name="prevState"></param>
		public StateChangedEventArgs(AudioDeviceState prevState) : base()
		{
			mPreviousState = prevState;
		}
	}

	/// <summary>
	/// <see cref="EventArgs"/> for <see cref="IPlaybackAudioDevice.PlayEnded"/> events
	/// </summary>
	public class EndedEventArgs : EventArgs
	{
		private TimeSpan mEndTime;

		/// <summary>
		/// Gets the time of end of playback
		/// </summary>
		public TimeSpan EndTime
		{
			get
			{
				return mEndTime;
			}
		}
		/// <summary>
		/// Default constructor that initializes the end time
		/// </summary>
		public EndedEventArgs(TimeSpan endTime)
		{
			mEndTime = endTime;
		}
	}

	/// <summary>
	/// Delegate for <see cref="IAudioDevice.Time"/> events
	/// </summary>
	/// <param name="source">The source <see cref="IAudioDevice"/></param>
	/// <param name="e">Event arguments containing the current position</param>
	public delegate void AudioDeviceTimeEventDelegate(IAudioDevice source, TimeEventArgs e);

	/// <summary>
	/// Delegate for <see cref="IAudioDevice.StateChanged"/> events
	/// </summary>
	/// <param name="source">The source <see cref="IAudioDevice"/></param>
	/// <param name="e">Event arguments containing the previous <see cref="AudioDeviceState"/> of <paramref name="source"/></param>
	public delegate void StateChangedEventDelegate(IAudioDevice source, StateChangedEventArgs e);

	/// <summary>
	/// Delegate for <see cref="IPlaybackAudioDevice.PlayEnded"/> event
	/// </summary>
	/// <param name="source">The source <see cref="IPlaybackAudioDevice"/></param>
	/// <param name="e">Event argument (contains no data)</param>
	public delegate void EndedEventDelegate(IAudioDevice source, EndedEventArgs e);
}
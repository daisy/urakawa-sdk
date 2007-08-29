using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace AudioEngine
{
	/// <summary>
	/// Abstract implementation of <see cref="IAudioDevice"/>
	/// </summary>
	public abstract class AudioDevice : IAudioDevice
	{

		private static ushort smDefaultNumberOfChannels = 1;
		private static uint smDefaultSampleRate = 44100;
		private static ushort smDefaultBitsPerSample = 16;


		#region IAudioDevice Members

		/// <summary>
		/// Event fired during record and playback to show the progression of time
		/// </summary>
		public event AudioDeviceTimeEventDelegate Time;

		/// <summary>
		/// Fires when the <see cref="AudioDevice"/> changes <see cref="AudioDeviceState"/>
		/// </summary>
		public event StateChangedEventDelegate StateChanged;

		/// <summary>
		/// Fires the <see cref="Time"/> event
		/// </summary>
		/// <param name="tPos">The time with which to fire the <see cref="Time"/> event</param>
		/// <param name="maxDbs">The maximal Db values for each channel of audio sincethe latest previous <see cref="Time"/> event</param>
		protected void FireTime(TimeSpan tPos, double[] maxDbs)
		{
			if (Time != null) Time(this, new TimeEventArgs(tPos, maxDbs));
		}

		/// <summary>
		/// Fires the <see cref="StateChanged"/> event
		/// </summary>
		/// <param name="prevState">The <see cref="AudioDeviceState"/> prior to the change</param>
		protected void FireStateChanged(AudioDeviceState prevState)
		{
			if (StateChanged != null) StateChanged(this, new StateChangedEventArgs(prevState));
		}

		private string mName = "";

		/// <summary>
		/// Gets the name of the <see cref="AudioDevice"/>
		/// </summary>
		/// <returns>The name</returns>
		public string getName()
		{
			return mName;
		}

		private ushort mNumberOfChannels = smDefaultNumberOfChannels;

		/// <summary>
		/// Gets the number of channels setting of the <see cref="AudioDevice"/>
		/// </summary>
		/// <returns>The number of channels</returns>
		public ushort getNumberOfChannels()
		{
			return mNumberOfChannels;
		}

		/// <summary>
		/// Sets the number of channels setting of the <see cref="AudioDevice"/>
		/// </summary>
		/// <param name="newNOC">The new number of channels</param>
		public virtual void setNumberOfChannels(ushort newNOC)
		{
			mNumberOfChannels = newNOC;
		}

		private uint mSampleRate = smDefaultSampleRate;

		/// <summary>
		/// Gets the sample rate setting of the <see cref="AudioDevice"/>
		/// </summary>
		/// <returns>The sample rate in Hz</returns>
		public uint getSampleRate()
		{
			return mSampleRate;
		}

		/// <summary>
		/// Sets the sample rate setting of the <see cref="AudioDevice"/>
		/// </summary>
		/// <param name="newSR">The new sample rate in Hz</param>
		/// <remarks>
		/// <c>base.<see cref="setSampleRate"/></c> method must be called in overridden classes,
		/// else <see cref="getSampleRate"/> will not return the desired value
		/// </remarks>
		public virtual void setSampleRate(uint newSR)
		{
			mSampleRate = newSR;
		}

		private ushort mBitsPerSample = smDefaultBitsPerSample;

		/// <summary>
		/// Gets the number of bits per sample setting of the <see cref="AudioDevice"/>
		/// </summary>
		/// <returns>The number of bits per sample</returns>
		public ushort getBitDepth()
		{
			return mBitsPerSample;
		}

		/// <summary>
		/// Sets the number of bits per sample setting of the <see cref="AudioDevice"/>
		/// </summary>
		/// <param name="newBPS">The new number of bits per sample</param>
		public void setBitDepth(ushort newBPS)
		{
			if (((newBPS % 8) != 0) || newBPS==0)
			{
				throw new ApplicationException("The number of bits per sample must be a positive integra number of bytes");
			}
			mBitsPerSample = newBPS;
		}

		/// <summary>
		/// Gets the block align, that is the number of bytes/sample.
		/// Convenience for <c><see cref="getNumberOfChannels"/>()*<see cref="getBitDepth"/>()/8</c>
		/// </summary>
		/// <returns>The bloick align</returns>
		public ushort getBlockAlign()
		{
			return (ushort)(getNumberOfChannels() * getBitDepth() / 8);
		}

		/// <summary>
		/// Gets the byte rate of the <see cref="IAudioDevice"/>.
		/// Convenience for <c><see cref="getSampleRate"/>()*<see cref="getBlockAlign"/>()</c>
		/// </summary>
		/// <returns>The byte rate in bytes/second</returns>
		public uint getByteRate()
		{
			return getSampleRate() * getBlockAlign();
		}

		private AudioDeviceState mState = AudioDeviceState.Stopped;

		/// <summary>
		/// Gets the <see cref="AudioDeviceState"/> of <c>this</c>
		/// </summary>
		/// <returns>The <see cref="AudioDeviceState"/></returns>
		public AudioDeviceState getState()
		{
			return mState;
		}

		/// <summary>
		/// Sets the <see cref="AudioDeviceState"/> of <c>this</c>,
		/// firing the <see cref="IAudioDevice.StateChanged"/> event if nessesary
		/// </summary>
		/// <param name="newState">The new <see cref="AudioDeviceState"/></param>
		protected void setState(AudioDeviceState newState)
		{
			AudioDeviceState prevState = mState;
			mState = newState;
			if (newState != prevState)
			{
				FireStateChanged(prevState);
			}
		}

		/// <summary>
		/// Gets the time equivalent of a given <see cref="byte"/> position
		/// </summary>
		/// <param name="pos">The given <see cref="byte"/> position</param>
		/// <returns>The time equivalent as a <see cref="TimeSpan"/></returns>
		public TimeSpan getTimeEquivalent(long pos)
		{
			return new TimeSpan((pos * TimeSpan.TicksPerSecond) / getByteRate());
		}

		/// <summary>
		/// Gets the position equivalent of a given time
		/// </summary>
		/// <param name="time">The given time</param>
		/// <returns>The position equivalent</returns>
		public long getPositionEquivalent(TimeSpan time)
		{
			return (time.Ticks * getByteRate()) / TimeSpan.TicksPerSecond;
		}

		/// <summary>
		/// Gets the current time during playback and recording
		/// </summary>
		/// <returns>The current time</returns>
		public abstract TimeSpan getCurrentTime();


		#endregion
	}
}

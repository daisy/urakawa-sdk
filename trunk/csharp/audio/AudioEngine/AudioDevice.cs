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

		public AudioDevice()
		{
			setNumberOfChannels(smDefaultBitsPerSample);
			setSampleRate(smDefaultBitsPerSample);
			setBitDepth(smDefaultBitsPerSample);
		}


		#region IAudioDevice Members

		/// <summary>
		/// Event fired during record and playback to show the progression of time
		/// </summary>
		public event EventHandler<TimeEventArgs> Time;

		/// <summary>
		/// Fires when the <see cref="AudioDevice"/> changes <see cref="AudioDeviceState"/>
		/// </summary>
		public event EventHandler<StateChangedEventArgs> StateChanged;

		/// <summary>
		/// Fired when an overload has occured
		/// </summary>
		public event EventHandler<OverloadEventArgs> OverloadOccured;

		/// <summary>
		/// Fires the <see cref="Time"/> event
		/// </summary>
		/// <param name="tPos">The time with which to fire the <see cref="Time"/> event</param>
		/// <param name="maxDbs">The maximal Db values for each channel of audio sincethe latest previous <see cref="Time"/> event</param>
		protected void FireTime(TimeSpan tPos, double[] maxDbs)
		{
			EventHandler<TimeEventArgs> d = Time;
			if (d != null) d(this, new TimeEventArgs(tPos, maxDbs));
		}

		/// <summary>
		/// Fires the <see cref="StateChanged"/> event
		/// </summary>
		/// <param name="prevState">The <see cref="AudioDeviceState"/> prior to the change</param>
		protected void FireStateChanged(AudioDeviceState prevState)
		{
			EventHandler<StateChangedEventArgs> d = StateChanged;
			if (d != null) d(this, new StateChangedEventArgs(prevState));
		}

		/// <summary>
		/// Fires the <see cref="OverloadOccured"/> event
		/// </summary>
		/// <param name="channel">The channel in which the overload occured</param>
		/// <param name="overloadTime">The time at which the overload occured</param>
		protected void FireOverloadOccured(ushort channel, TimeSpan overloadTime)
		{
			EventHandler<OverloadEventArgs> d = OverloadOccured;
			if (d != null) d(this, new OverloadEventArgs(channel, overloadTime));
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

		private uint mSampleRate;

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

		private ushort mBitDepth;

		/// <summary>
		/// Gets the number of bits per sample setting of the <see cref="AudioDevice"/>
		/// </summary>
		/// <returns>The number of bits per sample</returns>
		public ushort getBitDepth()
		{
			return mBitDepth;
		}

		/// <summary>
		/// Sets the number of bits per sample setting of the <see cref="AudioDevice"/>
		/// </summary>
		/// <param name="newBPS">The new number of bits per sample</param>
		public void setBitDepth(ushort newBPS)
		{
			if (((newBPS % 8) != 0) || newBPS==0)
			{
				throw new ApplicationException("The number of bits per sample must be a positive integral number of bytes");
			}
			mBitDepth = newBPS;
			mBytesPerSample = (ushort)(mBitDepth / 8);
			mFullValue = Math.Pow(2, mBitDepth - 1);
		}

		private ushort mBytesPerSample;
		/// <summary>
		/// Gets the number of bytes per sample
		/// </summary>
		public ushort BytesPerSample { get { return mBytesPerSample; } }
		private double mFullValue;
		/// <summary>
		/// Gets tha maximal positive value of a sample
		/// </summary>
		public double FullValue { get { return mFullValue; } }

		/// <summary>
		/// Calculates a absolute value and determines if maximal or minimal values have been hit for a sample
		/// </summary>
		/// <param name="buf">A byte array containing the sample data</param>
		/// <param name="startOffset">The offset in <paramref name="buf"/> at which the sample data begins</param>
		/// <param name="val">A <see cref="double"/> in which to return the absolute value</param>
		/// <param name="overload">
		/// A <see cref="bool"/> for outputting a <see cref="bool"/> indicating if the samle has the maximal/minumal possible value
		/// </param>
		public void calculateSampleValAndMaxMin(byte[] buf, int startOffset, out double val, out bool overload)
		{
			byte b;
			switch (mBytesPerSample)
			{
				case 1:
					b = buf[startOffset];
					val = b;
					overload = (b == Byte.MaxValue);
					break;
				case 2:
					short sVal = BitConverter.ToInt16(buf, startOffset);
					val = Math.Abs((double)sVal);
					overload = (sVal == Int16.MaxValue) || (sVal == Int16.MinValue);
					break;
				case 4:
					int iVal = BitConverter.ToInt32(buf, startOffset);
					val = Math.Abs(iVal);
					overload = (iVal == Int32.MaxValue) || (iVal == Int32.MinValue);
					break;
				default:
					bool maxHit = true;
					bool minHit = true;
					val = 0d;
					for (int i=0; i<mBytesPerSample; i++)
					{
						b = buf[startOffset+i];
						if (i<(mBytesPerSample-1))
						{
							if (b != 0xFF) maxHit = false;
							if (b != 0x0) minHit = false;
						}
						else
						{
							if (b != 0x7F) maxHit = false;
							if (b != 0x80) minHit = false;
						}
						val += Math.Pow(2, 8 * i) * b;
					}
					if (val > mFullValue)
					{
						val = (2 * mFullValue) - val;
					}
					overload = maxHit || minHit;
					break;
			}
			//string stringRep = "";
			//for (int i = 0; i < mBytesPerSample; i++)
			//{
			//    stringRep += buf[startOffset + i].ToString("X2");
			//}
			//System.Diagnostics.Debug.WriteLineIf(maxHit, "Hit max value " + stringRep + ": " + val.ToString("0"));
			//System.Diagnostics.Debug.WriteLineIf(minHit, "Hit min value " + stringRep + ": " + val.ToString("0"));
		}

		/// <summary>
		/// Calculates the Db equivalent of an absolute sample value
		/// </summary>
		/// <param name="absVal">The absolute sample value</param>
		/// <returns>The Db equivalent</returns>
		public double calculateDbValue(double absVal)
		{
			return 20 * Math.Log10(absVal / FullValue);
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

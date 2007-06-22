using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using Microsoft.DirectX.DirectSound;

namespace AudioEngine.DirectX9
{
	/// <summary>
	/// Implamentation of <see cref="IRecordAudioDevice"/> using DirectX 9
	/// </summary>
	public class RecordAudioDevice : AudioDevice, IRecordAudioDevice
	{


		/// <summary>
		/// Default constructor - uses the default <see cref="Capture"/> device
		/// </summary>
		public RecordAudioDevice()
		{
			mRecordCapture = new Capture();
		}

		/// <summary>
		/// Kill the record worker <see cref="Thread"/> ensuring that no more events are raised
		/// </summary>
		public void KillRecordWorker()
		{
			if (mRecordThread.IsAlive) mRecordThread.Abort();
		}

		/// <summary>
		/// The length of the record buffer in milliseconds
		/// </summary>
		public static long BUFFER_MS = 2000;

		/// <summary>
		/// The time to wait between updates while recording
		/// </summary>
		public static int WAIT_TIME_MS = 1;

		#region IRecordAudioDevice Members

		/// <summary>
		/// Records/captures PCM data to an output <see cref="Stream"/>
		/// </summary>
		/// <param name="pcmOutputStream">
		/// An output <see cref="Stream"/> to which the captured PCM audio data is written
		/// </param>
		public void record(Stream pcmOutputStream)
		{
			if (getState() != AudioDeviceState.Stopped)
			{
				throw new ApplicationException("Can only record from stopped state");
			}
			mPCMOutoutStream = pcmOutputStream;
			mOriginPos = mPCMOutoutStream.Position;
			mHasRecordingStopped = false;
			InitializeBuffer();
			ThreadStart ts = new ThreadStart(RecordWorker);
			mRecordThread = new Thread(ts);
			mRecordThread.Start();
		}

		private Capture mRecordCapture;
		private Stream mPCMOutoutStream;
		private bool mHasRecordingStopped;
		private long mOriginPos;
		private int mCycleCount;
		private int mBufferLength;
		private CaptureBuffer mBuffer;
		private Thread mRecordThread;

		private void InitializeBuffer()
		{
			CaptureBufferDescription bd = new CaptureBufferDescription();
			bd.Format = getWaveFormat();
			mBufferLength = (int)getPositionEquivalent(new TimeSpan(BUFFER_MS * TimeSpan.TicksPerMillisecond));
			bd.BufferBytes = mBufferLength;
			mBuffer = new CaptureBuffer(bd, mRecordCapture);
			mCycleCount = 0;
		}

		private void RecordWorker()
		{
			try
			{
				int readLength = mBufferLength / 4;
				int latestReadCursor = 0;
				int latestReadPosition = 0;
				mBuffer.Start(true);
				setState(AudioDeviceState.Recording);
				while (true)
				{
					int captureCursor, readCursor;
					mBuffer.GetCurrentPosition(out captureCursor, out readCursor);
					if (readCursor < latestReadCursor) mCycleCount++;
					int currentReadPosition = readCursor + (mCycleCount * mBufferLength);
					//Do read if there is more than readLength bytes to read or if recording has stopped
					if (mHasRecordingStopped)
					{
						mBuffer.Read(latestReadPosition % mBufferLength, mPCMOutoutStream, currentReadPosition - latestReadPosition, LockFlag.None);
						break;
					}
					else if (latestReadPosition + readLength < currentReadPosition)
					{
						mBuffer.Read(latestReadPosition%mBufferLength, mPCMOutoutStream, currentReadPosition - latestReadPosition, LockFlag.None);
						latestReadPosition = currentReadPosition;
					}
					latestReadCursor = readCursor;
					FireTime(getTimeEquivalent(readCursor + (mCycleCount * mBufferLength)), null);
					Thread.Sleep(WAIT_TIME_MS);
				}
				mBuffer.Dispose();
			}
			catch (ThreadAbortException)
			{
				if (mBuffer != null)
				{
					if (!mBuffer.Disposed) mBuffer.Dispose();
				}
			}
		}


		/// <summary>
		/// Stops recording
		/// </summary>
		public void stopRecording()
		{
			switch (getState())
			{
				case AudioDeviceState.Recording:
				case AudioDeviceState.PausedRecord:
					int ccp, crp;
					mBuffer.GetCurrentPosition(out ccp, out crp);
					System.Diagnostics.Debug.Print("Cursor before stop: {0:0}", ccp);
					mBuffer.Stop();
					mBuffer.GetCurrentPosition(out ccp, out crp);
					System.Diagnostics.Debug.Print("Cursor after stop: {0:0}", ccp);
					setState(AudioDeviceState.Stopped);
					mHasRecordingStopped = true;
					break;
				default:
					throw new ApplicationException("Can only stop recording from recording or paused record states");
			}
		}

		/// <summary>
		/// Pauses recording - the <see cref="AudioDeviceState"/> must be <see cref="AudioDeviceState.Recording"/>
		/// </summary>
		public void pauseRecording()
		{
			if (getState() != AudioDeviceState.Recording)
			{
				throw new ApplicationException("Can only pause recording while recording");
			}
			int ccp, crp;
			mBuffer.GetCurrentPosition(out ccp, out crp);
			System.Diagnostics.Debug.Print("Cursor before stop: {0:0}", ccp);
			mBuffer.Stop();
			mBuffer.GetCurrentPosition(out ccp, out crp);
			System.Diagnostics.Debug.Print("Cursor after stop: {0:0}", ccp);
			setState(AudioDeviceState.PausedPlay);
		}

		/// <summary>
		/// Resumes recording - the <see cref="AudioDeviceState"/> must be <see cref="AudioDeviceState.PausedRecord"/>
		/// </summary>
		public void resumeRecording()
		{
			if (getState() != AudioDeviceState.PausedRecord)
			{
				throw new ApplicationException("Can only resume recording when it is paused");
			}
			mBuffer.Start(true);
			setState(AudioDeviceState.Playing);
		}

		#endregion

		/// <summary>
		/// Gets the current time during recording
		/// </summary>
		/// <returns>The current time</returns>
		public override TimeSpan getCurrentTime()
		{
			switch (getState())
			{
				case AudioDeviceState.PausedRecord:
				case AudioDeviceState.Recording:
					int ccp, crp;
					mBuffer.GetCurrentPosition(out ccp, out crp);
					return getTimeEquivalent(((long)crp) + (long)(mCycleCount * mBufferLength));
				default:
					return TimeSpan.Zero;
			}
		}

		#region IRecordAudioDevice Members

		/// <summary>
		/// Fired when a recording session ends
		/// </summary>
		public event EndedEventDelegate RecordEnded;

		private void FireRecordEnded()
		{
			if (RecordEnded != null) RecordEnded(this, new EndedEventArgs(getCurrentTime()));
		}

		#endregion

	}
}

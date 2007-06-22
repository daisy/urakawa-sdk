using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using AudioEngine.DirectX9;
using urakawa.media.data.audio;

namespace PunchInRec
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
			mPlaybackDevice = new PlaybackAudioDevice(this);
			mPlaybackDevice.StateChanged += new AudioEngine.StateChangedEventDelegate(Device_StateChanged);
			mPlaybackDevice.Time += new AudioEngine.AudioDeviceTimeEventDelegate(Device_Time);
			mRecordDevice = new RecordAudioDevice();
			mRecordDevice.StateChanged += new AudioEngine.StateChangedEventDelegate(Device_StateChanged);
			mRecordDevice.Time += new AudioEngine.AudioDeviceTimeEventDelegate(Device_Time);
			mTimeTrackBar.TickFrequency = 1000;
			mTimeTrackBar.SmallChange = 1;
			mTimeTrackBar.LargeChange = 1000;
			mTimeTrackBar.Minimum = 0;
			mTimeTrackBar.Maximum = 0;
			UpdateTime();
			UpdateTransportControls();
		}

		void Device_Time(AudioEngine.IAudioDevice source, AudioEngine.TimeEventArgs e)
		{
			if (source==mPlaybackDevice)
			{
				UpdateTime(e.getCurrentTimePosition().Add(mLatestPlayStartFileTime));
			}
			else if (source==mRecordDevice)
			{
				mCurFileTime = e.getCurrentTimePosition().Add(mLatestRecordStartFileTime);
				if (mCurFileTime > mPCMInfo.getDuration().getTimeDeltaAsTimeSpan()) mPCMInfo.setDataLength((uint)(mFile.Length - mDataStartPosition));
				UpdateTime(mCurFileTime);
			}
		}

		void Device_StateChanged(AudioEngine.IAudioDevice source, AudioEngine.StateChangedEventArgs e)
		{
			UpdateTransportControls();
			if (
				mPlaybackDevice.getState() == AudioEngine.AudioDeviceState.Stopped
				|| mRecordDevice.getState() == AudioEngine.AudioDeviceState.Stopped)
			{
				GotoTimeSlider();
			}
		}

		private FileStream mFile;
		private PCMDataInfo mPCMInfo;
		private uint mDataStartPosition;
		private TimeSpan mLatestPlayStartFileTime;
		private TimeSpan mLatestRecordStartFileTime;
		private TimeSpan mCurFileTime;
		private PlaybackAudioDevice mPlaybackDevice;
		private RecordAudioDevice mRecordDevice;

		private void mOpenFileButton_Click(object sender, EventArgs e)
		{
			OpenFile();
		}

		private void OpenFile()
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Filter = "WAVE PCM (*.wav)|*.wav";
			ofd.Multiselect = false;
			ofd.AddExtension = true;
			ofd.Title = "Open Wave File";
			ofd.CheckFileExists = false;
			ofd.CheckPathExists = true;
			if (ofd.ShowDialog(this) == DialogResult.OK)
			{
				if (ofd.FileName != mFileTextBox.Text)
				{
					PCMDataInfo pcmInfo;
					FileStream newFile;
					if (File.Exists(ofd.FileName))
					{
						try
						{
							newFile = new FileStream(ofd.FileName, FileMode.Open, FileAccess.ReadWrite, FileShare.Write);
						}
						catch (Exception err)
						{
							MessageBox.Show(
								String.Format("Could not open wave file {0}: {1}", ofd.FileName, err.Message),
								"Open wave file");
							return;
						}
						try
						{
							pcmInfo = PCMDataInfo.parseRiffWaveHeader(newFile);
						}
						catch (Exception err)
						{
							MessageBox.Show(
								String.Format("Invalid wave file {0}: {1}", ofd.FileName, err.Message),
								"Open wave file");
							newFile.Close();
							return;
						}
					}
					else
					{
						try
						{
							newFile = new FileStream(ofd.FileName, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Write);
						}
						catch (Exception err)
						{
							MessageBox.Show(
								String.Format("Could not open wave file {0}: {1}", ofd.FileName, err.Message),
								"Open wave file");
							return;
						}
						pcmInfo = new PCMDataInfo();
						pcmInfo.writeRiffWaveHeader(newFile);
					}
					if (mFile != null)
					{
						mPCMInfo.setDataLength((uint)(mFile.Length - mDataStartPosition));
						FixWaveHeader();
						mFile.Close();
					}
					mFile = newFile;
					mPCMInfo = pcmInfo;
					mDataStartPosition = (uint)mFile.Position;
					mFileTextBox.Text = ofd.FileName;
					mCurFileTime = TimeSpan.Zero;
					UpdateTime();
					UpdateTransportControls();
					GotoTimeSlider();
				}
			}
		}

		private void FixWaveHeader()
		{
			BinaryWriter wr = new BinaryWriter(mFile);
			wr.Seek(4, SeekOrigin.Begin);
			wr.Write(mDataStartPosition + mPCMInfo.getDataLength() - 8);
			wr.Seek((int)(mDataStartPosition - 4U), SeekOrigin.Begin);
			wr.Write(mPCMInfo.getDataLength());
			wr.Flush();
		}

		private string FormatTimeSpan(TimeSpan val)
		{
			return String.Format(
				"{0:00}:{1:00}:{2:00}.{3:000}",
				(24*val.Days)+val.Hours, val.Minutes, val.Seconds, val.Milliseconds);
		}

		delegate void UpdateTransportControlsDelegate();

		private void UpdateTransportControls()
		{
			if (this.InvokeRequired)
			{
				if (this.IsDisposed) return;
				UpdateTransportControlsDelegate d = new UpdateTransportControlsDelegate(UpdateTransportControls);
				this.Invoke(d);
			}
			else
			{
				mTogglePauseButton.Text = "Pause";
				switch (mRecordDevice.getState())
				{
					case AudioEngine.AudioDeviceState.Recording:
						mTimeTrackBar.Enabled = false;
						mPlayButton.Enabled = false;
						mRecordButton.Enabled = false;
						mTogglePauseButton.Enabled = true;
						mStopButton.Enabled = true;
						mOpenFileButton.Enabled = false;
						mExitButton.Enabled = false;
						SyncronizeMenuItems();
						return;
					case AudioEngine.AudioDeviceState.PausedRecord:
						mTimeTrackBar.Enabled = false;
						mPlayButton.Enabled = false;
						mRecordButton.Enabled = false;
						mTogglePauseButton.Enabled = true;
						mTogglePauseButton.Text = "Resume";
						mStopButton.Enabled = true;
						mOpenFileButton.Enabled = false;
						mExitButton.Enabled = false;
						SyncronizeMenuItems();
						return;
				}
				switch (mPlaybackDevice.getState())
				{
					case AudioEngine.AudioDeviceState.Playing:
						mTimeTrackBar.Enabled = false;
						mPlayButton.Enabled = false;
						mRecordButton.Enabled = true;
						mTogglePauseButton.Enabled = true;
						mStopButton.Enabled = true;
						mOpenFileButton.Enabled = false;
						mExitButton.Enabled = false;
						SyncronizeMenuItems();
						return;
					case AudioEngine.AudioDeviceState.PausedPlay:
						mTimeTrackBar.Enabled = false;
						mPlayButton.Enabled = false;
						mRecordButton.Enabled = false;
						mTogglePauseButton.Enabled = true;
						mTogglePauseButton.Text = "Resume";
						mStopButton.Enabled = true;
						mOpenFileButton.Enabled = false;
						mExitButton.Enabled = false;
						SyncronizeMenuItems();
						return;
				}
				if (mFile == null)
				{
					mTimeTrackBar.Enabled = false;
					mPlayButton.Enabled = false;
					mRecordButton.Enabled = false;
				}
				else
				{
					mTimeTrackBar.Enabled = true;
					mPlayButton.Enabled = true;
					mRecordButton.Enabled = true;
				}
				mTogglePauseButton.Enabled = false;
				mStopButton.Enabled = false;
				mOpenFileButton.Enabled = true;
				mExitButton.Enabled = true;

				SyncronizeMenuItems();
			}
		}

		private void SyncronizeMenuItems()
		{
			mOpenFileButton.Enabled = mOpenFileButton.Enabled;
			mExitFileMenuItem.Enabled = mExitButton.Enabled;
			mPlayTransportMenuItem.Enabled = mPlayButton.Enabled;
			mRecordTransportMenuItem.Enabled = mRecordButton.Enabled;
			mTogglePauseTransportMenuItem.Enabled = mTogglePauseButton.Enabled;
			mTogglePauseTransportMenuItem.Text = mTogglePauseButton.Text;
			mStopTransportMenuItem.Enabled = mStopButton.Enabled;
		}

		delegate void UpdateTimeDelegate(TimeSpan time);

		private void UpdateTime()
		{
			UpdateTime(mCurFileTime);
		}

		private void UpdateTime(TimeSpan time)
		{
			if (this.InvokeRequired)
			{
				UpdateTimeDelegate d = new UpdateTimeDelegate(UpdateTime);
				this.Invoke(d, new object[] { time });
			}
			else
			{
				TimeSpan fileDur = TimeSpan.Zero;
				if (mPCMInfo != null) fileDur = mPCMInfo.getDuration().getTimeDeltaAsTimeSpan();
				mTimeLabel.Text = String.Format(
					"{0}/{1}", FormatTimeSpan(time), FormatTimeSpan(fileDur));
				if (time<=fileDur)
				{
					mTimeTrackBar.Maximum = (int)(fileDur.Ticks / TimeSpan.TicksPerMillisecond);
					mTimeTrackBar.Value = (int)(time.Ticks / TimeSpan.TicksPerMillisecond);
				}
			}
		}

		private void mCloseButton_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void mTimeTrackBar_Scroll(object sender, EventArgs e)
		{
			TimeTrackerValueToCurFileTime();
		}

		private void TimeTrackerValueToCurFileTime()
		{
			mCurFileTime = TimeSpan.FromMilliseconds(mTimeTrackBar.Value);
			UpdateTime();
		}

		private void mStopButton_Click(object sender, EventArgs e)
		{
			Stop();
		}

		private void Stop()
		{
			switch (mRecordDevice.getState())
			{
				case AudioEngine.AudioDeviceState.Recording:
				case AudioEngine.AudioDeviceState.PausedRecord:
					mCurFileTime = mRecordDevice.getCurrentTime();
					mRecordDevice.stopRecording();
					break;
			}
			switch (mPlaybackDevice.getState())
			{
				case AudioEngine.AudioDeviceState.Playing:
				case AudioEngine.AudioDeviceState.PausedPlay:
					mPlaybackDevice.stopPlayback();
					while (mPlaybackDevice.getState() != AudioEngine.AudioDeviceState.Stopped)
					{
						System.Threading.Thread.Sleep(0);
					}
					break;
			}
			UpdateTime();
			UpdateTransportControls();
		}

		private void mPlayButton_Click(object sender, EventArgs e)
		{
			Play();
		}

		private void Play()
		{
			mFile.Seek(mDataStartPosition, SeekOrigin.Begin);
			mLatestPlayStartFileTime = mCurFileTime;
			mPlaybackDevice.play(mFile, mCurFileTime, mPCMInfo.getDuration().getTimeDeltaAsTimeSpan());
			mPlayButton.Focus();
		}

		private void mPauseButton_Click(object sender, EventArgs e)
		{
			TogglePause();
		}

		private void TogglePause()
		{
			if (mRecordDevice.getState() == AudioEngine.AudioDeviceState.Recording)
			{
				mRecordDevice.pauseRecording();
				UpdateTime(mRecordDevice.getCurrentTime());
			}
			else if (mRecordDevice.getState() == AudioEngine.AudioDeviceState.PausedRecord)
			{
				mRecordDevice.resumeRecording();
			}
			else if (mPlaybackDevice.getState() == AudioEngine.AudioDeviceState.Playing)
			{
				mPlaybackDevice.pausePlayback();
				mCurFileTime = mPlaybackDevice.getCurrentTime().Add(mLatestPlayStartFileTime);
				UpdateTime();
			}
			else if (mPlaybackDevice.getState() == AudioEngine.AudioDeviceState.PausedPlay)
			{
				mPlaybackDevice.resumePlayback();
			}
			UpdateTransportControls();
		}

		private void mRecordButton_Click(object sender, EventArgs e)
		{
			Record();
		}

		private void Record()
		{
			if (mPlaybackDevice.getState() != AudioEngine.AudioDeviceState.Stopped)
			{
				mCurFileTime = mPlaybackDevice.getCurrentTime().Add(mLatestPlayStartFileTime);
				mPlaybackDevice.stopPlayback();
				while (mPlaybackDevice.getState() != AudioEngine.AudioDeviceState.Stopped)
				{
					System.Threading.Thread.Sleep(1);
				}
			}
			mLatestRecordStartFileTime = mCurFileTime;
			long curFileTimeOffset = mPCMInfo.getByteRate()*mCurFileTime.Ticks / TimeSpan.TicksPerSecond;
			curFileTimeOffset -= (curFileTimeOffset % mPCMInfo.getBlockAlign());
			mFile.Seek(mDataStartPosition + curFileTimeOffset, SeekOrigin.Begin);
			mRecordDevice.record(mFile);
			mStopButton.Focus();
		}

		private void mExitFileMenuItem_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void mOpenFileMenuItem_Click(object sender, EventArgs e)
		{
			OpenFile();
		}

		private void mPlayTransportMenuItem_Click(object sender, EventArgs e)
		{
			Play();
		}

		private void mRecordTransportMenuItem_Click(object sender, EventArgs e)
		{
			Record();
		}

		private void mTogglePauseTransportMenuItem_Click(object sender, EventArgs e)
		{
			TogglePause();
		}

		private void mStopTransportMenuItem_Click(object sender, EventArgs e)
		{
			Stop();
		}

		private void mTimeTrackBar_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Modifiers == Keys.Shift)
			{
				mTimeTrackBar.SmallChange = 1;
				mTimeTrackBar.LargeChange = 1000;
			}
			else if (e.Modifiers == Keys.Control)
			{
				mTimeTrackBar.SmallChange = 100;
				mTimeTrackBar.LargeChange = 10000;
			}
			else
			{
				mTimeTrackBar.SmallChange = 10;
				mTimeTrackBar.LargeChange = 1000;
			}
			if (e.Modifiers == Keys.None)
			{
				if (e.KeyCode == Keys.Home)
				{
					mTimeTrackBar.Value = mTimeTrackBar.Minimum;
					TimeTrackerValueToCurFileTime();
				}
				if (e.KeyCode == Keys.End)
				{
					mTimeTrackBar.Value = mTimeTrackBar.Maximum;
					TimeTrackerValueToCurFileTime();
				}
			}
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (mRecordDevice.getState() != AudioEngine.AudioDeviceState.Stopped)
			{
				mRecordDevice.stopRecording();
				mRecordDevice.KillRecordWorker();
			}
			if (mPlaybackDevice.getState() != AudioEngine.AudioDeviceState.Stopped)
			{
				mPlaybackDevice.stopPlayback();
				mPlaybackDevice.killPlaybackWorker();
			}
			if (mFile!=null) FixWaveHeader();
		}

		private void mGotoTimeSliderTransportMenuItem_Click(object sender, EventArgs e)
		{
			GotoTimeSlider();
		}

		private void GotoTimeSlider()
		{
			if (this.InvokeRequired)
			{
				MethodInvoker d = new MethodInvoker(GotoTimeSlider);
				this.Invoke(d);
			}
			else
			{
				if (mTimeTrackBar.Enabled) mTimeTrackBar.Focus();
			}
		}
	}
}
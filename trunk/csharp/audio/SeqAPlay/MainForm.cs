using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using AudioEngine;
using AudioEngine.DirectX9;
using urakawa.media.data.utilities;
using urakawa.media.data.audio;

namespace SeqAPlay
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
			mPlaybackDevice = new PlaybackAudioDevice(this);
			mPlaybackDevice.setBitDepth(16);
			mPlaybackDevice.setNumberOfChannels(1);
			mPlaybackDevice.setSampleRate(22050);
			mPlaybackDevice.StateChanged += new EventHandler<StateChangedEventArgs>(PlaybackDevice_StateChanged);
			mPlaybackDevice.Time += new EventHandler<AudioEngine.TimeEventArgs>(PlaybackDevice_Time);
			mPlaybackDevice.OverloadOccured += new EventHandler<OverloadEventArgs>(PlaybackDevice_OverloadOccured);
			mPlaybackDevice.PlayEnded += new EventHandler<EndedEventArgs>(PlaybackDevice_PlayEnded);
			SetTimeLabel();
			UpdatePlaybackButtons(mPlaybackDevice.getState());
			mPlaybackSpeedNumericUpDown.Value = 1;
			UpdatePlaybackSpeedControl();
			mHorizontalPPMeter.Resize += new EventHandler(HorizontalPPMeter_Resize);
			mHorizontalPPMeter.PeakOverloadIndicatorClicked
				+= new EventHandler<AudioEngine.PPMeter.PeakOverloadIndicatorClickedEventArgs>(PeakOverloadIndicatorClicked);
			mVerticalPPMeter.Resize += new EventHandler(VerticalPPMeter_Resize);
			mVerticalPPMeter.PeakOverloadIndicatorClicked
				+= new EventHandler<AudioEngine.PPMeter.PeakOverloadIndicatorClickedEventArgs>(PeakOverloadIndicatorClicked);
		}

		void PeakOverloadIndicatorClicked(object sender, AudioEngine.PPMeter.PeakOverloadIndicatorClickedEventArgs e)
		{
			AudioEngine.PPMeter.PPMeter meter = sender as AudioEngine.PPMeter.PPMeter;
			if (meter != null) meter.SetPeakOverloadCount(e.ChannelNumber, 0);
		}

		void PlaybackDevice_PlayEnded(object sender, EndedEventArgs e)
		{
			Application.DoEvents();//Ensure that all pending events finish their execution
			mHorizontalPPMeter.ForceFullFallback();
			mVerticalPPMeter.ForceFullFallback();
			if (mPCMInputStream != null)
			{
				mPCMInputStream.Close();
				mPCMInputStream = null;
			}
		}

		//private ulong[] mOverloadCount;

		void PlaybackDevice_OverloadOccured(object sender, OverloadEventArgs e)
		{
			//if (this.InvokeRequired)
			//{
			//    EventHandler<OverloadEventArgs> d = PlaybackDevice_OverloadOccured;
			//    this.Invoke(d, sender, e);
			//    return;
			//}
			//if (mOverloadCount != null)
			//{
			//    if (mOverloadCount.Length != mPlaybackDevice.getNumberOfChannels()) mOverloadCount = null;
			//}
			//if (mOverloadCount == null) mOverloadCount = new ulong[mPlaybackDevice.getNumberOfChannels()];
			//mOverloadCount[e.Channel] += 1;
			//string cap = "";
			//for (int c = 0; c < mOverloadCount.Length; c++)
			//{
			//    cap += String.Format("{0:0}:{1:0};", c, mOverloadCount[c]);
			//}
			//this.Text = String.Format("Sequence Audio Player ({0})", cap);
			mHorizontalPPMeter.SetPeakOverloadCount(e.Channel, mHorizontalPPMeter.GetPeakOverloadCount(e.Channel) + 1);
			mVerticalPPMeter.SetPeakOverloadCount(e.Channel, mVerticalPPMeter.GetPeakOverloadCount(e.Channel) + 1);
		}

		void HorizontalPPMeter_Resize(object sender, EventArgs e)
		{
			int barPad = (int)Math.Ceiling(5f * ((float)mHorizontalPPMeter.Height) / 77f);
			if (barPad != mHorizontalPPMeter.BarPadding) mHorizontalPPMeter.BarPadding = barPad;
			float verEmSize = 6f * ((float)mHorizontalPPMeter.Height) / 80f;
			float emSize = 6f * ((float)mHorizontalPPMeter.Width) / 200f;
			if (verEmSize < emSize) emSize = verEmSize;
			if (mHorizontalPPMeter.Font.Size != emSize)
			{
				mHorizontalPPMeter.Font = new Font(mHorizontalPPMeter.Font.FontFamily, emSize);
			}
		}

		void VerticalPPMeter_Resize(object sender, EventArgs e)
		{
			int barPad = (int)Math.Ceiling(5f * ((float)mVerticalPPMeter.Width) / 80f);
			if (barPad != mVerticalPPMeter.BarPadding) mVerticalPPMeter.BarPadding = barPad;
			float verEmSize = 6f * ((float)mVerticalPPMeter.Width) / 80f;
			float emSize = 6f * ((float)mVerticalPPMeter.Height) / 200f;
			if (verEmSize < emSize) emSize = verEmSize;
			if (mVerticalPPMeter.Font.Size != emSize)
			{
				mVerticalPPMeter.Font = new Font(mVerticalPPMeter.Font.FontFamily, emSize);
			}
		}

		private void UpdatePlaybackSpeedControl()
		{
			mPlaybackSpeedNumericUpDown.Minimum = (decimal)mPlaybackDevice.getMinPlaybackSpeed();
			mPlaybackSpeedNumericUpDown.Maximum = (decimal)mPlaybackDevice.getMaxPlaybackSpeed();
		}

		private TimeSpan mCurrentPos = TimeSpan.Zero;

		void PlaybackDevice_Time(object source, TimeEventArgs e)
		{
			mCurrentPos = e.CurrentTimePosition;
			SetTimeLabel();
			double[] maxDbs = e.MaxDbSinceLatestTime;
			if (maxDbs == null)
			{
				for (int i = 0; i < mHorizontalPPMeter.NumberOfChannels; i++)
				{
					mHorizontalPPMeter.SetValue(i, Double.NegativeInfinity);
				}
				for (int i = 0; i < mVerticalPPMeter.NumberOfChannels; i++)
				{
					mVerticalPPMeter.SetValue(i, Double.NegativeInfinity);
				}

			}
			else
			{
				for (int i = 0; i < maxDbs.Length; i++)
				{
					mHorizontalPPMeter.SetValue(i, maxDbs[i]);
				}
				for (int i = 0; i < mVerticalPPMeter.NumberOfChannels; i++)
				{
					mVerticalPPMeter.SetValue(i, maxDbs[i]);
				}
			}
		}

		private void SetTimeLabel()
		{
			if (this.InvokeRequired)
			{
				MethodInvoker d = new MethodInvoker(SetTimeLabel);
				this.Invoke(d);
			}
			else
			{
				mTimeLabel.Text = FormatTimeSpan(mCurrentPos);
			}
		}

		private string FormatTimeSpan(TimeSpan val)
		{
			return String.Format(
				"{0:0}.{1:00}:{2:00}:{3:00}.{4:000}",
				val.Days, val.Hours, val.Minutes, val.Seconds, val.Milliseconds);
		}

		void PlaybackDevice_StateChanged(object source, StateChangedEventArgs e)
		{
			if (source==mPlaybackDevice)
			{
				UpdatePlaybackButtons(mPlaybackDevice.getState());
				UpdateInputFilesButtons(mPlaybackDevice.getState());
			}
		}

		private IPlaybackAudioDevice mPlaybackDevice;

		delegate void UpdateButtonsCallback(AudioDeviceState state);

		private void UpdatePlaybackButtons(AudioDeviceState state)
		{
			if (this.InvokeRequired)
			{
				UpdateButtonsCallback d = new UpdateButtonsCallback(UpdatePlaybackButtons);
				this.Invoke(d, new object[] { state });
			}
			else
			{
				mPauseButton.Text = "Pause";
				switch (state)
				{
					case AudioDeviceState.Stopped:
						mPlayButton.Enabled = true;
						mPauseButton.Enabled = false;
						mStopButton.Enabled = false;
						mCurrentPos = TimeSpan.Zero;
						SetTimeLabel();
						break;
					case AudioDeviceState.PausedPlay:
						mPauseButton.Text = "Resume";
						goto case AudioDeviceState.Playing;
					case AudioDeviceState.Playing:
						mPlayButton.Enabled = false;
						mPauseButton.Enabled = true;
						mStopButton.Enabled = true;
						break;
					default:
						goto case AudioDeviceState.Stopped;
				}
			}
		}

		private void mPlayButton_Click(object sender, EventArgs e)
		{
			mPlaybackDevice.stopPlayback();
			if (mInputFilesListView.Items.Count == 0)
			{
				MessageBox.Show(this, "There are no input files to play", "Play");
				return;
			}
			/* Playback of all files using both SubStream and SequenceStream */
			List<Stream> ifStreams = new List<Stream>(mInputFilesListView.Items.Count);
			foreach (ListViewItem inputFileItem in mInputFilesListView.Items)
			{
				FileStream ifs = new FileStream(inputFileItem.SubItems[2].Text, FileMode.Open, FileAccess.Read);
				PCMDataInfo pcmInfo = PCMDataInfo.ParseRiffWaveHeader(ifs);
				long startPos = ifs.Position;
				ifs.Position = 0;
				SubStream subIfs = new SubStream(ifs, startPos, pcmInfo.DataLength);
				ifStreams.Add(subIfs);
			}
			mPCMInputStream = new SequenceStream(ifStreams);
			mPlaybackDevice.play(mPCMInputStream);
		}

		Stream mPCMInputStream; 

		private void mStopButton_Click(object sender, EventArgs e)
		{
			mPlaybackDevice.stopPlayback();
			if (mPCMInputStream != null) mPCMInputStream.Close();
		}

		private void TogglePause()
		{
			switch (mPlaybackDevice.getState())
			{
				case AudioDeviceState.Playing:
					mPlaybackDevice.pausePlayback();
					break;
				case AudioDeviceState.PausedPlay:
					mPlaybackDevice.resumePlayback();
					break;
			}
		}

		private void mPauseButton_Click(object sender, EventArgs e)
		{
			TogglePause();
		}

		private void mInputFilesListView_SelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateInputFilesButtons(mPlaybackDevice.getState());
		}

		private delegate void UpdateInputFilesButtonsDelegate(AudioDeviceState state);

		private void UpdateInputFilesButtons(AudioDeviceState state)
		{
			if (this.InvokeRequired)
			{
				UpdateInputFilesButtonsDelegate d = new UpdateInputFilesButtonsDelegate(UpdateInputFilesButtons);
				this.Invoke(d, new object[] { state });
			}
			else
			{
				if (state == AudioDeviceState.Stopped)
				{
					mAddInputFileButton.Enabled = true;
					mRemoveInputFileButton.Enabled = (mInputFilesListView.SelectedIndices.Count > 0);
					mClearInputFilesButton.Enabled = (mInputFilesListView.Items.Count > 0);
				}
				else
				{
					mAddInputFileButton.Enabled = false;
					mRemoveInputFileButton.Enabled = false;
					mClearInputFilesButton.Enabled = false;
				}
			}
		}

		private void mAddInputFileButton_Click(object sender, EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Filter = "WAVE PCM (*.wav)|*.wav";
			ofd.Title = "Add WAVE PCM Input Files";
			ofd.CheckFileExists = true;
			ofd.CheckPathExists = true;
			ofd.Multiselect = true;
			if (ofd.ShowDialog(this) == DialogResult.OK)
			{
				List<ListViewItem> addedItems = new List<ListViewItem>();
				foreach (string file in ofd.FileNames)
				{
					FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Write);
					PCMDataInfo pcmInfo;
					try
					{
						pcmInfo = PCMDataInfo.ParseRiffWaveHeader(fs);
					}
					catch (Exception err)
					{
						if (MessageBox.Show(
							this,
							String.Format("Could not parse wave file {0}: {1}", file, err.Message),
							"Add files",
							MessageBoxButtons.OKCancel) == DialogResult.OK)
						{
							continue;
						}
						else
						{
							return;
						}
					}
					finally
					{
						fs.Close();
					}
					if (mInputFilesListView.Items.Count == 0)
					{
						mPlaybackDevice.setBitDepth(pcmInfo.BitDepth);
						mPlaybackDevice.setSampleRate(pcmInfo.SampleRate);
						mPlaybackDevice.setNumberOfChannels(pcmInfo.NumberOfChannels);
						mHorizontalPPMeter.NumberOfChannels = pcmInfo.NumberOfChannels;
						mVerticalPPMeter.NumberOfChannels = pcmInfo.NumberOfChannels;
						UpdatePlaybackSpeedControl();
					}
					else
					{
						if (
							mPlaybackDevice.getBitDepth() != pcmInfo.BitDepth
							|| mPlaybackDevice.getSampleRate() != pcmInfo.SampleRate
							|| mPlaybackDevice.getNumberOfChannels() != pcmInfo.NumberOfChannels)
						{
							string msg = String.Format(
								"Wave file {0} is not compatible with the previously added files.\n"
								+"Must have {1:0} channels, sample rate {2:0} and bit depth {3:0}",
								file, 
								mPlaybackDevice.getNumberOfChannels(), 
								mPlaybackDevice.getSampleRate(),
								mPlaybackDevice.getBitDepth());
							if (MessageBox.Show(this, msg, "Add files", MessageBoxButtons.OKCancel)==DialogResult.OK)
							{
								continue;
							}
							else
							{
								return;
							}
						}
					}
					string[] itemArr = new string[] { Path.GetFileName(file), FormatTimeSpan(pcmInfo.Duration.TimeDeltaAsTimeSpan), file };
					addedItems.Add(new ListViewItem(itemArr));
				}
				mInputFilesListView.Items.AddRange(addedItems.ToArray());
			}
			UpdateInputFilesButtons(mPlaybackDevice.getState());
		}

		private void mRemoveInputFileButton_Click(object sender, EventArgs e)
		{
			foreach (ListViewItem lvi in mInputFilesListView.SelectedItems)
			{
				mInputFilesListView.Items.Remove(lvi);
			}
			UpdateInputFilesButtons(mPlaybackDevice.getState());
			UpdatePlaybackSpeedControl();
		}

		private void mClearInputFilesButton_Click(object sender, EventArgs e)
		{
			mInputFilesListView.Items.Clear();
			UpdateInputFilesButtons(mPlaybackDevice.getState());
			UpdatePlaybackSpeedControl();
		}

		private void mPlaybackSpeedNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			mPlaybackDevice.setPlaybackSpeed((double)mPlaybackSpeedNumericUpDown.Value);
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (mPlaybackDevice.getState() == AudioDeviceState.Stopped)
			{
				mPlaybackDevice.stopPlayback();
				mPlaybackDevice.killPlaybackWorker();
				mHorizontalPPMeter.ForceFullFallback();
				mVerticalPPMeter.ForceFullFallback();
			}
			else
			{
				MessageBox.Show(
					this,
					"You must stop playback before closing",
					this.Text);
				e.Cancel = true;
			}
			//if (mUpdatePPMThread != null)
			//{
			//  if (mUpdatePPMThread.IsAlive) mUpdatePPMThread.Abort();
			//}
		}
	}
}
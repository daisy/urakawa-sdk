using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace WaveFormDisplay
{
	public partial class WaveFormDisplay : UserControl
	{
		public WaveFormDisplay()
		{
			InitializeComponent();
			UpdateWaveFormGraphs();
		}

		protected override void OnResize(EventArgs e)
		{
			UpdateWaveFormGraphSizes();
			UpdateGraphData();
			base.OnResize(e);
		}

		private void UpdateGraphData()
		{
			if (mGraphs == null) return;
			if (GetAudioDataDelegate != null && (BitDepth==8 || BitDepth==16))
			{
				Stream pcmStream = GetAudioDataDelegate(ClipBegin, ClipEnd);
				try
				{
					BinaryReader rd = new BinaryReader(pcmStream);
					ushort blockAlign = (ushort)(NumberOfChannels * BitDepth / 8);
					uint nos = (uint)((pcmStream.Length - pcmStream.Position) / blockAlign);
					for (int c = 0; c < NumberOfChannels; c++)
					{
						mGraphs[c].GraphMinData = new int[this.Width];
						mGraphs[c].GraphMaxData = new int[this.Width];
					}
					int s = 0;
					int prevX = 0;
					if (BitDepth==8)
					{
						while (s < nos)
						{
							int curX = (int)((s * this.Width) / nos);
							for (int c = 0; c < NumberOfChannels; c++)
							{
								int val = pcmStream.ReadByte();
								val = val>>4;
								if (mGraphs[c].GraphMaxData[prevX] < val) mGraphs[c].GraphMaxData[prevX] = val;
								if (val < mGraphs[c].GraphMinData[prevX]) mGraphs[c].GraphMinData[prevX] = val;
								if (curX > prevX)
								{
									for (int x = prevX+1; x < curX; x++)
									{
										mGraphs[c].GraphMaxData[x] = mGraphs[c].GraphMaxData[prevX];
										mGraphs[c].GraphMinData[x] = mGraphs[c].GraphMinData[prevX];
									}
								}
							}
							s++;
						}
					}
					else if (BitDepth == 16)
					{
						while (s < nos)
						{
							for (int c = 0; c < NumberOfChannels; c++)
							{
								int val = rd.ReadInt16();
								val = val>>2;
							}
						}
					}
				}
				finally
				{
					pcmStream.Close();
				}
			}
			else
			{
				foreach (WaveFormGraph grph in mGraphs)
				{
					grph.GraphMaxData = null;
					grph.GraphMinData = null;
				}
			}
		}

		private Stream mPCMDataStream;
		public Stream PCMDataStream
		{
			get
			{
				return mPCMDataStream;
			}

			set
			{
				mPCMDataStream = value;
			}
		}

		private uint mSampleRate;
		public uint SampleRate
		{
			get
			{
				return mSampleRate;
			}

			set
			{
				mSampleRate = value;
			}
		}
		private ushort mBitDepth;
		public ushort BitDepth
		{
			get { return mBitDepth; }
			set { mBitDepth = value; }
		}

		private ushort mNumberOfChannels = 1;
		public ushort NumberOfChannels
		{
			get { return mNumberOfChannels; }
			set {
				if (mNumberOfChannels != value)
				{
					mNumberOfChannels = value;
					UpdateWaveFormGraphs();
				}
			}
		}

		private WaveFormGraph[] mGraphs;
		private void UpdateWaveFormGraphs()
		{
			if (mGraphs == null)
			{
				mGraphs = new WaveFormGraph[NumberOfChannels];
				for (ushort c = 0; c < NumberOfChannels; c++)
				{
					mGraphs[c] = new WaveFormGraph();
					mGraphs[c].Channel = c;
					this.Controls.Add(mGraphs[c]);
				}
			}
			else
			{
				if (mGraphs.Length < NumberOfChannels)
				{
					WaveFormGraph[] newArray = new WaveFormGraph[NumberOfChannels];
					mGraphs.CopyTo(newArray, 0);
					for (ushort c = (ushort)mGraphs.Length; c < NumberOfChannels; c++)
					{
						newArray[c] = new WaveFormGraph();
						newArray[c].Channel = c;
						this.Controls.Add(newArray[c]);
					}
					mGraphs = newArray;
				}
				else if (mGraphs.Length > NumberOfChannels)
				{
					WaveFormGraph[] newArray = new WaveFormGraph[NumberOfChannels];
					for (ushort c = 0; c < NumberOfChannels; c++)
					{
						newArray[c] = mGraphs[c];
						newArray[c].Channel = c;
					}
					for (int c = NumberOfChannels; c < mGraphs.Length; c++)
					{
						this.Controls.Remove(mGraphs[c]);
					}
					mGraphs = newArray;
				}
			}
			UpdateWaveFormGraphSizes();
		}

		private void UpdateWaveFormGraphSizes()
		{
			if (mGraphs!=null)
			{
				if (mGraphs.Length > 0)
				{
					int dy = this.Height / mGraphs.Length;
					for (int c = 0; c < mGraphs.Length; c++)
					{
						mGraphs[c].Left = 0;
						mGraphs[c].Width = this.Width;
						mGraphs[c].Top = c * dy;
						mGraphs[c].Height = dy;
						mGraphs[c].Invalidate();
					}
				}
			}
		}

		private TimeSpan mClipBegin;

		public TimeSpan ClipBegin
		{
			get { return mClipBegin; }
			set { mClipBegin = value; }
		}
		private TimeSpan mClipEnd;

		public TimeSpan ClipEnd
		{
			get { return mClipEnd; }
			set { mClipEnd = value; }
		}

		private GetAudioDataDelegate mGetAudioDataDelegate;

		public GetAudioDataDelegate GetAudioDataDelegate
		{
			get { return mGetAudioDataDelegate; }
			set { mGetAudioDataDelegate = value; }
		} 
	}

	public delegate Stream GetAudioDataDelegate(TimeSpan clipBegin, TimeSpan clipEnd);
}
using System;
using System.Collections.Generic;
using System.Text;
using AudioEngine;
using urakawa.core;
using urakawa.property.channel;
using urakawa.media.data;
using urakawa.media.data.audio;
using urakawa.media.timing;

namespace urakawa.AudioEngine
{
	public class UrakawaPlaybackAudioDevice
	{


		public UrakawaPlaybackAudioDevice(global::AudioEngine.IPlaybackAudioDevice playDev)
		{
			if (playDev == null)
			{
				throw new exception.MethodParameterIsNullException("The underlying IPlaybackAudioDevice cannot be null");
			}
			mPlaybackAudioDevice = playDev;
			mPlaybackAudioDevice.Time += new AudioDeviceTimeEventDelegate(PlaybackAudioDevice_Time);
		}

		void PlaybackAudioDevice_Time(IAudioDevice source, global::AudioEngine.TimeEventArgs e)
		{
			TimeEventArgs ee = new TimeEventArgs(new Time(e.getCurrentTimePosition()), e.getMaxDbSinceLatestTime());
			FireTime(ee);
		}

		private void PlaybackAudioDevice_PlayEnded(IAudioDevice source, global::AudioEngine.EndedEventArgs e)
		{
			e.PCMStream.Close();
			mPlaybackAudioDevice.PlayEnded -= new EndedEventDelegate(PlaybackAudioDevice_PlayEnded);
			FireAudioMediaDataPlayEnded(new Time(e.EndTime));
		}

		public event EventHandler<EndedEventArgs> TreeNodePlayEnded;

		private void FireTreeNodePlayEnded(Time endTime)
		{
			EventHandler<EndedEventArgs> d = TreeNodePlayEnded;
			if (d != null) d(this, new EndedEventArgs(endTime));
		}

		public event EventHandler<EndedEventArgs> AudioMediaDataPlayEnded;

		private void FireAudioMediaDataPlayEnded(Time endTime)
		{
			EventHandler<EndedEventArgs> d = AudioMediaDataPlayEnded;
			if (d != null) d(this, new EndedEventArgs(endTime));
		}

		public event EventHandler<TimeEventArgs> Time;

		private void FireTime(TimeEventArgs e)
		{
			EventHandler<TimeEventArgs> d = Time;
			if (Time != null) d(this, e);
		}

		private global::AudioEngine.IPlaybackAudioDevice mPlaybackAudioDevice;

		protected void setDevicePCMFormat(PCMFormatInfo pcmFormat)
		{
			mPlaybackAudioDevice.setNumberOfChannels(pcmFormat.getNumberOfChannels());
			mPlaybackAudioDevice.setSampleRate(pcmFormat.getSampleRate());
			mPlaybackAudioDevice.setBitDepth(pcmFormat.getBitDepth());
		}

		public void play(AudioMediaData data)
		{
			setDevicePCMFormat(data.getPCMFormat());
			mPlaybackAudioDevice.PlayEnded += new EndedEventDelegate(PlaybackAudioDevice_PlayEnded);
			try
			{
				mPlaybackAudioDevice.play(data.getAudioData());
			}
			catch (Exception e)
			{
				mPlaybackAudioDevice.PlayEnded -= new EndedEventDelegate(PlaybackAudioDevice_PlayEnded);
				throw e;
			}
		}

		public void play(TreeNode node, Channel audioCh, bool contAfterNode)
		{
			if (node == null)
			{
				throw new exception.MethodParameterIsNullException("Can not play a null TreeNode");
			}
			if (audioCh == null)
			{
				throw new exception.MethodParameterIsNullException("The audio channel to play was null");
			}
			TreeNodePlaybackHandler playbackHandler = new TreeNodePlaybackHandler(node, audioCh, contAfterNode, this);
			playbackHandler.PlayHandlingEnded += new EventHandler(TreeNodePlaybackHandler_PlayHandlingEnded);
			playbackHandler.play();
		}

		void TreeNodePlaybackHandler_PlayHandlingEnded(object sender, EventArgs e)
		{
			TreeNodePlaybackHandler handler = sender as TreeNodePlaybackHandler;
			if (handler != null)
			{
				handler.PlayHandlingEnded -= new EventHandler(TreeNodePlaybackHandler_PlayHandlingEnded);

				handler.Dispose();
			}
		}

		private class TreeNodePlaybackHandler : IDisposable
		{
			private TreeNode mCurrentTreeNode;
			public TreeNode getCurrentTreeNode() { return mCurrentTreeNode; }
			private Channel mAudioChannel;
			public Channel getAudioChannel() { return mAudioChannel; }
			private bool mContinueAfterTreeNode;
			public bool willContinueAfterCurrentTreeNode() { return mContinueAfterTreeNode; }
			private UrakawaPlaybackAudioDevice mPlaybacAudiokDevice;
			private urakawa.navigation.TypeFilterNavigator<TreeNode> mTreeNodeNavigator;
			private Time mElapsedTime;
			public Time getElapsedTime() { return mElapsedTime; }
			public void resetElapsedTime() { mElapsedTime = urakawa.media.timing.Time.Zero; }

			public TreeNodePlaybackHandler(TreeNode node, Channel audioCh, bool contAfterNode, UrakawaPlaybackAudioDevice playDev)
			{
				mCurrentTreeNode = node;
				mAudioChannel = audioCh;
				mContinueAfterTreeNode = contAfterNode;
				mPlaybacAudiokDevice = playDev;
				mPlaybacAudiokDevice.AudioMediaDataPlayEnded += new EventHandler<EndedEventArgs>(PlaybacAudiokDevice_AudioMediaDataPlayEnded);
				mTreeNodeNavigator = new urakawa.navigation.TypeFilterNavigator<TreeNode>();
				resetElapsedTime();
			}

			public event EventHandler PlayHandlingEnded;

			private void FirePlayHandlingEnded()
			{
				EventHandler d = PlayHandlingEnded;
				if (d != null) d(this, EventArgs.Empty);
			}

			public void play()
			{
				if (mCurrentTreeNode == null)
				{
					mPlaybacAudiokDevice.FireTreeNodePlayEnded(mElapsedTime);
					return;
				}
				ManagedAudioMedia mam = null;
				ChannelsProperty chProp = mCurrentTreeNode.getProperty(typeof(ChannelsProperty)) as ChannelsProperty;
				if (chProp != null) mam = chProp.getMedia(mAudioChannel) as ManagedAudioMedia;
				if (mam == null)
				{
					if (mContinueAfterTreeNode)
					{
						mCurrentTreeNode = mTreeNodeNavigator.getNext(mCurrentTreeNode);
						play();
					}
				}
				else
				{
					mPlaybacAudiokDevice.play(mam.getMediaData());
				}
			}

			void PlaybacAudiokDevice_AudioMediaDataPlayEnded(object sender, EndedEventArgs e)
			{
				mElapsedTime.addTime(e.getEndTime());
				if (mContinueAfterTreeNode)
				{
					mCurrentTreeNode = mTreeNodeNavigator.getNext(mCurrentTreeNode);
					play();
				}
			}


			#region IDisposable Members

			public void Dispose()
			{
				if (mPlaybacAudiokDevice != null)
				{
					mPlaybacAudiokDevice.AudioMediaDataPlayEnded -= new EventHandler<EndedEventArgs>(PlaybacAudiokDevice_AudioMediaDataPlayEnded);
				}
				mPlaybacAudiokDevice = null;
				mTreeNodeNavigator = null;
				mCurrentTreeNode = null; 
				mAudioChannel = null;
			}

			#endregion
		}


	}

	public class EndedEventArgs : EventArgs
	{
		private Time mEndTime;
		public Time getEndTime() { return mEndTime; }

		public EndedEventArgs(Time endTime)
		{
			mEndTime = endTime;
		}
	}

	public class TimeEventArgs : EventArgs
	{
		private Time mCurrentPosition;
		private double[] mMaxDbSinceLatestTime;
		public Time getCurrentPosition() { return mCurrentPosition.copy(); }
		public double[] getMaxDbSinceLatestTime() { return mMaxDbSinceLatestTime.Clone() as double[]; }

		public TimeEventArgs(Time curPos, double[] maxDbs)
		{
			mCurrentPosition = curPos;
			mMaxDbSinceLatestTime = maxDbs;
		}
	}
}

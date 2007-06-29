using System;
using System.Collections.Generic;
using System.Text;
using AudioEngine;
using urakawa.media.data;
using urakawa.media.data.audio;
using urakawa.media.timing;

namespace urakawa.AudioEngine
{
	public class UrakawaPlaybackAudioDevice<T> where T : IPlaybackAudioDevice, new()
	{
		public UrakawaPlaybackAudioDevice()
		{
			mPlaybackAudioDevice = new T();
			mPlaybackAudioDevice.PlayEnded += new EndedEventDelegate(PlaybackAudioDevice_PlayEnded);
			mPlaybackAudioDevice.Time += new AudioDeviceTimeEventDelegate(PlaybackAudioDevice_Time);
		}

		void PlaybackAudioDevice_Time(IAudioDevice source, global::AudioEngine.TimeEventArgs e)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		private void PlaybackAudioDevice_PlayEnded(IAudioDevice source, global::AudioEngine.EndedEventArgs e)
		{
			e.PCMStream.Close();
			FirePlayEnded(new Time(e.EndTime));
		}

		public EventHandler<EndedEventArgs> PlayEnded;

		private void FirePlayEnded(Time endTime)
		{
			EventHandler<EndedEventArgs> d = PlayEnded;
			if (d != null) d(this, new EndedEventArgs(endTime));
		}

		public EventHandler<EventArgs> Time;

		private T mPlaybackAudioDevice;

		public void play(AudioMediaData data)
		{
			//6Stream data.getAudioData();
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
}

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace AudioEngine
{
	/// <summary>
	/// Interface for a <see cref="IAudioDevice"/> that supports playback of audio
	/// </summary>
	public interface IPlaybackAudioDevice : IAudioDevice
	{
		/// <summary>
		/// Fired when playback ends
		/// </summary>
		event EventHandler<EndedEventArgs> PlayEnded;

		/// <summary>
		/// Plays PCM audio data from an input <see cref="Stream"/> in it's entire length
		/// </summary>
		/// <param name="pcmInputStream">
		/// An input <see cref="Stream"/> from which the audio PCM data to play is read
		/// </param>
		void play(Stream pcmInputStream);

		/// <summary>
		/// Plays PCM audio data from an input <see cref="Stream"/> starting at a given clip begin time
		/// </summary>
		/// <param name="pcmInputStream">
		/// The time within the PCM audio data in the input <see cref="Stream"/> at which to begin playback
		/// </param>
		/// <param name="clipBegin">
		/// The time within the PCM audio data in the input <see cref="Stream"/> at which to begin playback
		/// </param>
		void play(Stream pcmInputStream, TimeSpan clipBegin);

		/// <summary>
		/// Plays PCM audio data from an input <see cref="Stream"/> starting at a given clip begin time
		/// and ending at a given clip end time
		/// </summary>
		/// <param name="pcmInputStream">
		/// An input <see cref="Stream"/> from which the audio PCM data to play is read
		/// </param>
		/// <param name="clipBegin">
		/// The time within the PCM audio data in the input <see cref="Stream"/> at which to begin playback
		/// </param>
		/// <param name="clipEnd">
		/// The time within the PCM audio data in the input <see cref="Stream"/> at which to end playback
		/// </param>
		void play(Stream pcmInputStream, TimeSpan clipBegin, TimeSpan clipEnd);

		/// <summary>
		/// Stops playback
		/// </summary>
		void stopPlayback();

		/// <summary>
		/// Pauses playback - the <see cref="AudioDeviceState"/> must be <see cref="AudioDeviceState.Playing"/>
		/// </summary>
		void pausePlayback();

		/// <summary>
		/// Resunmes playback - the <see cref="AudioDeviceState"/> must be <see cref="AudioDeviceState.PausedPlay"/>
		/// </summary>
		void resumePlayback();

		/// <summary>
		/// Sets the playback speed of the <see cref="IPlaybackAudioDevice"/>
		/// </summary>
		/// <param name="newSpeed">The new speed must be in the interval
		/// <c>[<see cref="getMinPlaybackSpeed"/>();<see cref="getMaxPlaybackSpeed"/>()]</c></param>
		void setPlaybackSpeed(double newSpeed);

		/// <summary>
		/// Gets the current playback speed
		/// </summary>
		/// <returns>The current playback speed</returns>
		double getPlaybackSpeed();

		/// <summary>
		/// Gets the minimal playback speed supported by the <see cref="IPlaybackAudioDevice"/>. 
		/// May vary with sample rate or other WAVE PCM parameters.
		/// </summary>
		/// <returns>The minimal supported playback speed (in ]0;1])</returns>
		double getMinPlaybackSpeed();

		/// <summary>
		/// Get maximal playback speed speed supported by the <see cref="IPlaybackAudioDevice"/>
		/// May vary with sample rate or other WAVE PCM parameters.
		/// </summary>
		/// <returns>The minimal supported playback speed </returns>
		double getMaxPlaybackSpeed();

		/// <summary>
		/// Kill the playback and update worker <see cref="System.Threading.Thread"/>s ensuring that no more events are raised
		/// </summary>
		void killPlaybackWorker();

	}
}

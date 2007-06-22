using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace AudioEngine
{
	/// <summary>
	/// Interface for a <see cref="IAudioDevice"/> that supports recording of audio
	/// </summary>
	public interface IRecordAudioDevice : IAudioDevice
	{
		/// <summary>
		/// Fired when recording ends
		/// </summary>
		event EndedEventDelegate RecordEnded;

		/// <summary>
		/// Records/captures PCM data to an output <see cref="Stream"/>
		/// </summary>
		/// <param name="pcmOutputStream">
		/// An output <see cref="Stream"/> to which the captured PCM audio data is written
		/// </param>
		void record(Stream pcmOutputStream);

		/// <summary>
		/// Stops recording
		/// </summary>
		void stopRecording();

		/// <summary>
		/// Pauses recording - the <see cref="AudioDeviceState"/> must be <see cref="AudioDeviceState.Recording"/>
		/// </summary>
		void pauseRecording();

		/// <summary>
		/// Resumes recording - the <see cref="AudioDeviceState"/> must be <see cref="AudioDeviceState.PausedRecord"/>
		/// </summary>
		void resumeRecording();
	}
}

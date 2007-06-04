using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using urakawa.media.timing;

namespace urakawa.media.data
{
	/// <summary>
	/// Interface for a generic Audio <see cref="IMediaData"/>
	/// </summary>
	public interface IAudioMediaData : IMediaData
	{
		/// <summary>
		/// Creates a copy of the <see cref="IAudioMediaData"/> instance
		/// </summary>
		/// <returns>The copy</returns>
		new IAudioMediaData copy();

		/// <summary>
		/// Gets the number of channels of audio
		/// </summary>
		/// <returns>The number of channels</returns>
		int getNumberOfChannels();

		/// <summary>
		/// Sets the number of channels of audio
		/// </summary>
		/// <param name="newNumberOfChannels">The new number of channels</param>
		void setNumberOfChannels(int newNumberOfChannels);

		/// <summary>
		/// Gets the bit depth, that is the number of bits per sample
		/// </summary>
		/// <returns>The bit depth</returns>
		int getBitDepth();

		/// <summary>
		/// Sets the bit depth
		/// </summary>
		/// <param name="newBitDepth">The new bit depth</param>
		void setBitDepth(int newBitDepth);

		/// <summary>
		/// Gets the sample rate in Hz
		/// </summary>
		/// <returns>The sample rate</returns>
		int getSampleRate();

		/// <summary>
		/// Sets the sample rate in Hz
		/// </summary>
		/// <param name="newSampleRate">The new sample rate</param>
		void setSampleRate(int newSampleRate);

		/// <summary>
		/// Gets the byte rate of the audio media data
		/// </summary>
		/// <returns>The byte rate in bytes/sec</returns>
		int getByteRate();

		/// <summary>
		/// Gets the count in bytes of the PCM data of the audio media data
		/// </summary>
		/// <returns>The count in bytes</returns>
		int getPCMLength();
		
		/// <summary>
		/// Gets the intrinsic duration of <c>this</c>
		/// </summary>
		/// <returns>The duration</returns>
		TimeDelta getAudioDuration();

		/// <summary>
		/// Gets a <see cref="Stream"/> providing read access to all the audio as raw PCM data
		/// </summary>
		/// <returns>The <see cref="Stream"/></returns>
		Stream getAudioData();
		/// <summary>
		/// Gets a <see cref="Stream"/> providing read access to all audio after a given clip begin <see cref="Time"/> as raw PCM data
		/// </summary>
		/// <param name="clipBegin">The given clip begin <see cref="Time"/></param>
		/// <returns>The <see cref="Stream"/></returns>
		Stream getAudioData(Time clipBegin);
		/// <summary>
		/// Gets a <see cref="Stream"/> providing read access to all audio between given clip begin and end <see cref="Time"/>s
		/// as raw PCM data
		/// </summary>
		/// <param name="clipBegin">The given clip begin <see cref="Time"/></param>
		/// <param name="clipEnd">The given clip end <see cref="Time"/></param>
		/// <returns>The <see cref="Stream"/></returns>
		Stream getAudioData(Time clipBegin, Time clipEnd);
		/// <summary>
		/// Appends audio of a given duration to <c>this</c>
		/// </summary>
		/// <param name="pcmData">A <see cref="Stream"/> providing read access to the input raw PCM audio data</param>
		/// <param name="duration">The duration of the audio to add</param>
		void appendAudioData(Stream pcmData, TimeDelta duration);
		/// <summary>
		/// Inserts audio of a given duration at a given <see cref="Time"/>
		/// </summary>
		/// <param name="pcmData">A <see cref="Stream"/> providing read access to the input raw PCM audio data</param>
		/// <param name="insertPoint"></param>
		/// <param name="duration"></param>
		void insertAudioData(Stream pcmData, Time insertPoint, TimeDelta duration);
		/// <summary>
		/// Replaces audio with a given duration at a given replace point in <see cref="Time"/>
		/// </summary>
		/// <param name="pcmData">A <see cref="Stream"/> providing read access to the input raw PCM audio data</param>
		/// <param name="replacePoint">The given replkace point in <see cref="Time"/></param>
		/// <param name="duration">The duration of the audio to replace</param>
		void replaceAudioData(Stream pcmData, Time replacePoint, TimeDelta duration);
		/// <summary>
		/// Removes all audio after a given clip begin <see cref="Time"/>
		/// </summary>
		/// <param name="clipBegin">The clip begin</param>
		void removeAudio(Time clipBegin);
		/// <summary>
		/// Removes all audio between given clip begin and end <see cref="Time"/>
		/// </summary>
		/// <param name="clipBegin">The givne clip begin <see cref="Time"/></param>
		/// <param name="clipEnd">The givne clip end <see cref="Time"/></param>
		void removeAudio(Time clipBegin, Time clipEnd);
	}
}

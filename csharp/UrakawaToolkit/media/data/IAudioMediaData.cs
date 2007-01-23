using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using urakawa.media.timing;

namespace urakawa.media.data
{
	public interface IAudioMediaData : IMediaData
	{
		int getNumberOfChannels();
		void setNumberOfChannels(int newNumberOfChannels);
		int getBitDepth();
		void setBitDepth(int newBitDepth);
		int getSampleRate();
		void setSampleRate(int newSampleRate);
		ITimeDelta getAudioDuration();
		Stream getAudioData();
		Stream getAudioData(ITime clipBegin);
		Stream getAudioData(ITime clipBegin, ITime clipEnd);
		void appendAudioData(Stream pcmData, ITimeDelta duration);
		void insertAudioData(Stream pcmData, ITime insertPoint, ITimeDelta duration);
		void replaceAudioData(Stream pcmData, ITime replacePoint, ITimeDelta duration);
	}
}

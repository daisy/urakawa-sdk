using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX.DirectSound;

namespace AudioEngine.DirectX9
{
	/// <summary>
	/// Abstract class derived from <see cref="AudioEngine.AudioDevice"/>
	/// that provides DirectX specific helper methods
	/// </summary>
	public abstract class AudioDevice : AudioEngine.AudioDevice
	{
		/// <summary>
		/// Gets a <see cref="WaveFormat"/> corresponding to the PCM format stored 
		/// in <c>this</c> <see cref="AudioDevice"/>
		/// </summary>
		/// <returns>The <see cref="WaveFormat"/></returns>
		public WaveFormat getWaveFormat()
		{
			WaveFormat wf = new WaveFormat();
			wf.AverageBytesPerSecond = (int)getByteRate();
			wf.BitsPerSample = (short)getBitDepth();
			wf.BlockAlign = (short)getBlockAlign();
			wf.Channels = (short)getNumberOfChannels();
			wf.SamplesPerSecond = (int)getSampleRate();
			wf.FormatTag = WaveFormatTag.Pcm;
			return wf;
		}

		/// <summary>
		/// Sets the PCM format of <c>this</c> <see cref="AudioDevice"/>
		/// from a given <see cref="WaveFormat"/>
		/// </summary>
		/// <param name="wf">The given <see cref="WaveFormat"/></param>
		public void setWaveFormat(WaveFormat wf)
		{
			setBitDepth((ushort)wf.BitsPerSample);
			setSampleRate((ushort)wf.SamplesPerSecond);
			setNumberOfChannels((ushort)wf.Channels);
		}
	}
}

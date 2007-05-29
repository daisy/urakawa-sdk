package org.daisy.urakawa.media.data;

import java.io.InputStream;

import org.daisy.urakawa.media.timing.Time;
import org.daisy.urakawa.media.timing.TimeDelta;


/**
 * 
 * @checked against C# implementation [29 May 2007]
 * @todo verify / add comments and exceptions
 *
 */
public abstract class AbstractAudioMediaData extends AbstractMediaData implements AudioMediaData {
	protected MediaDataFactory getMediaDataFactory()
	{
		return null;
	}

	public int getNumberOfChannels()
	{
		return 0;
	}

	public void setNumberOfChannels(int newNumberOfChannels)
	{
	}


	public int getBitDepth()
	{
		return 0;
	}

	public void setBitDepth(int newBitDepth)
	{
	}

	
	public int getSampleRate()
	{
		return 0;
	}

	public void setSampleRate(int newSampleRate)
	{
	}

	public int getByteRate()
	{
		return 0;
	}

	public int getPCMLength()
	{
		return 0;
	}


	public abstract TimeDelta getAudioDuration();


	public InputStream getAudioData()
	{
		return null;
	}

	public InputStream getAudioData(Time clipBegin)
	{
		return null;
	}

	public abstract InputStream getAudioData(Time clipBegin, Time clipEnd);

	public void appendAudioData(InputStream pcmData, TimeDelta duration)
	{

	}

	public abstract void insertAudioData(InputStream pcmData, Time insertPoint, TimeDelta duration);

	public abstract void replaceAudioData(InputStream pcmData, Time replacePoint, TimeDelta duration);
	
	public void removeAudio(Time clipBegin)
	{
		
	}

	public abstract void removeAudio(Time clipBegin, Time clipEnd);

	protected abstract AudioMediaData audioMediaDataCopy();

	protected AbstractMediaData mediaDataCopy()
	{
		return null;
	}

	public MediaData copy() {
		return null;
	}
}

using System;
using System.Collections.Generic;
using System.Text;

namespace urakawa.media.data
{
	public abstract class AudioMediaData : IAudioMediaData
	{
		#region IAudioMediaData Members

		public int getNumberOfChannels()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void setNumberOfChannels()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public int getBitDepth()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void setBitDepth(int newBitDepth)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public int getSampleRate()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void setSampleRate(int newSampleRate)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public System.IO.Stream getAudioData()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public System.IO.Stream getAudioData(urakawa.media.timing.ITime clipBegin)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public System.IO.Stream getAudioData(urakawa.media.timing.ITime clipBegin, urakawa.media.timing.ITime clipEnd)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void appendAudioData(System.IO.Stream pcmData, urakawa.media.timing.ITimeDelta duration)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void insertAudioData(System.IO.Stream pcmData, urakawa.media.timing.ITime insertPoint, urakawa.media.timing.ITimeDelta duration)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void replaceAudioData(System.IO.Stream pcmData, urakawa.media.timing.ITime replacePoint, urakawa.media.timing.ITimeDelta duration)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion

		#region IMediaData Members

		public IMediaDataManager getDataManager()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public string getUid()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public string getName()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void setName(string newName)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public IDataProvider getDataProvider()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public abstract IAudioMediaData copy();

		IMediaData IMediaData.copy()
		{
			return copy();
		}
		
		#endregion

		#region IXukAble Members

		public abstract bool XukIn(System.Xml.XmlReader source);

		public abstract bool XukOut(System.Xml.XmlWriter destination);

		public abstract string getXukLocalName();

		public abstract string getXukNamespaceUri();

		#endregion
	}
}

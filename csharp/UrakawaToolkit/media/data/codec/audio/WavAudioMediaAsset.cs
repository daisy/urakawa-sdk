using System;
using System.Collections.Generic;
using System.Text;
using urakawa.media.data;

namespace urakawa.media.data.codec.audio
{
	public class WavAudioMediaData : AudioMediaData
	{
		public override IAudioMediaData copy()
		{
			throw new Exception("The method or operation is not implemented.");
//TODO: Implement method
		}

		public override bool XukIn(System.Xml.XmlReader source)
		{
			throw new Exception("The method or operation is not implemented.");
//TODO: Implement method
		}

		public override bool XukOut(System.Xml.XmlWriter destination)
		{
			throw new Exception("The method or operation is not implemented.");
//TODO: Implement method
		}

		public override string getXukLocalName()
		{
			throw new Exception("The method or operation is not implemented.");
//TODO: Implement method
		}

		public override string getXukNamespaceUri()
		{
			throw new Exception("The method or operation is not implemented.");
//TODO: Implement method
		}

		public override System.IO.Stream getAudioData()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override System.IO.Stream getAudioData(urakawa.media.timing.ITime clipBegin)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override System.IO.Stream getAudioData(urakawa.media.timing.ITime clipBegin, urakawa.media.timing.ITime clipEnd)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void appendAudioData(System.IO.Stream pcmData, urakawa.media.timing.ITimeDelta duration)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void insertAudioData(System.IO.Stream pcmData, urakawa.media.timing.ITime insertPoint, urakawa.media.timing.ITimeDelta duration)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void replaceAudioData(System.IO.Stream pcmData, urakawa.media.timing.ITime replacePoint, urakawa.media.timing.ITimeDelta duration)
		{
			throw new Exception("The method or operation is not implemented.");
		}
	}
}

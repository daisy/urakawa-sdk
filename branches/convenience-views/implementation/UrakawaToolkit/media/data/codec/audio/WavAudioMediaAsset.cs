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
		}

		public override bool XukIn(System.Xml.XmlReader source)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override bool XukOut(System.Xml.XmlWriter destination)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override string getXukLocalName()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override string getXukNamespaceUri()
		{
			throw new Exception("The method or operation is not implemented.");
		}
	}
}

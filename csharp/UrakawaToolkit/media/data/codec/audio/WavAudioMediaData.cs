using System;
using System.Collections.Generic;
using System.Text;
using urakawa.media.data;

namespace urakawa.media.data.codec.audio
{
	public class WavAudioMediaData : AudioMediaData
	{
		protected internal WavAudioMediaData(IMediaDataManager mngr)
		{
			
		}

		public override IMediaData copy()
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

		public override System.IO.Stream getAudioData()
		{
			throw new Exception("The method or operation is not implemented.");
			//TODO: Implement method
		}

		public override System.IO.Stream getAudioData(urakawa.media.timing.ITime clipBegin)
		{
			throw new Exception("The method or operation is not implemented.");
			//TODO: Implement method
		}

		public override System.IO.Stream getAudioData(urakawa.media.timing.ITime clipBegin, urakawa.media.timing.ITime clipEnd)
		{
			throw new Exception("The method or operation is not implemented.");
			//TODO: Implement method
		}

		public override void appendAudioData(System.IO.Stream pcmData, urakawa.media.timing.ITimeDelta duration)
		{
			throw new Exception("The method or operation is not implemented.");
			//TODO: Implement method
		}

		public override void insertAudioData(System.IO.Stream pcmData, urakawa.media.timing.ITime insertPoint, urakawa.media.timing.ITimeDelta duration)
		{
			throw new Exception("The method or operation is not implemented.");
			//TODO: Implement method
		}

		public override void replaceAudioData(System.IO.Stream pcmData, urakawa.media.timing.ITime replacePoint, urakawa.media.timing.ITimeDelta duration)
		{
			throw new Exception("The method or operation is not implemented.");
			//TODO: Implement method
		}

		public override urakawa.media.timing.ITimeDelta getAudioDuration()
		{
			throw new Exception("The method or operation is not implemented.");
			//TODO: Implement method
		}

		public override void delete()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		protected override IList<IDataProvider> getUsedDataProviders()
		{
			throw new Exception("The method or operation is not implemented.");
		}
	}
}

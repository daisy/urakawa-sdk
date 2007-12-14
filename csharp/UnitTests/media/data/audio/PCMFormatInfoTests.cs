using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using NUnit.Framework;
using urakawa.media.timing;

namespace urakawa.media.data.audio
{
	[TestFixture]
	public class PCMFormatInfoTests
	{
		[Test, Description("Ensure that PCMFormatInfo.getDataLength(PCMFormatInfo.getDuration(i))==i for all i:uint")]
		public void getDuration_getDataLength_RoundTrip()
		{
			PCMFormatInfo pcmInfo = new PCMFormatInfo();
			pcmInfo.setBitDepth(16);
			pcmInfo.setNumberOfChannels(1);
			pcmInfo.setSampleRate(22050);
			getDuration_getDataLength_RoundTrip(pcmInfo);
			pcmInfo.setBitDepth(16);
			pcmInfo.setNumberOfChannels(1);
			pcmInfo.setSampleRate(44100);
			getDuration_getDataLength_RoundTrip(pcmInfo);
			pcmInfo.setBitDepth(8);
			pcmInfo.setNumberOfChannels(2);
			pcmInfo.setSampleRate(22050);
			getDuration_getDataLength_RoundTrip(pcmInfo);
		}

		private void getDuration_getDataLength_RoundTrip(PCMFormatInfo pcmInfo)
		{
			Random rnd = new Random();
			uint ba = pcmInfo.getBlockAlign();
			for (int i = 0; i < 20480; i++)
			{
				uint dl = (uint)Math.Round(rnd.NextDouble() * UInt32.MaxValue);
				dl -= dl % ba;
				uint roundI = pcmInfo.getDataLength(pcmInfo.getDuration(dl));
				Assert.AreEqual(
					dl,
					roundI,
					"Wrong round trip data langth value");
			}
		}

		[Test, Description("Ensure that PCMFormatInfo is xuk round-trip secure")]
		public void xukIn_xukOut_roundTrim()
		{
			PCMFormatInfo pcmInfo;
			pcmInfo = new PCMFormatInfo(1, 44100, 16);
			testRoundTrim(pcmInfo);
			pcmInfo = new PCMFormatInfo(1, 22050, 16);
			testRoundTrim(pcmInfo);
			pcmInfo = new PCMFormatInfo(2, 44100, 16);
			testRoundTrim(pcmInfo);
			pcmInfo = new PCMFormatInfo(2, 22050, 16);
			testRoundTrim(pcmInfo);
			pcmInfo = new PCMFormatInfo(1, 44100, 8);
			testRoundTrim(pcmInfo);
			pcmInfo = new PCMFormatInfo(1, 22050, 8);
			testRoundTrim(pcmInfo);
		}

		private void testRoundTrim(PCMFormatInfo info)
		{
			StringBuilder sb = new StringBuilder();
			XmlWriter wr = XmlWriter.Create(sb);
			info.xukOut(wr, new Uri(System.IO.Directory.GetCurrentDirectory()));
			wr.Close();
			PCMFormatInfo realodedInfo = new PCMFormatInfo();
			XmlReader rd = XmlReader.Create(new System.IO.StringReader(sb.ToString()));
			rd.ReadToFollowing(info.getXukLocalName(), info.getXukNamespaceUri());
			realodedInfo.xukIn(rd);
			Assert.IsTrue(info.valueEquals(realodedInfo));
		}
	}
}

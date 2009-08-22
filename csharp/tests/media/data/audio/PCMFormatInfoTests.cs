using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using AudioLib;
using NUnit.Framework;
using urakawa.media.timing;

namespace urakawa.media.data.audio
{
    [TestFixture]
    public class PCMFormatInfoTests
    {
        [Test]
        public void Duration_DataLength_RoundTrip()
        {
            PCMFormatInfo pcmInfo = new PCMFormatInfo();
            pcmInfo.Data.BitDepth = 16;
            pcmInfo.Data.NumberOfChannels = 1;
            pcmInfo.Data.SampleRate = 22050;
            Duration_DataLength_RoundTrip(pcmInfo);
            pcmInfo.Data.BitDepth = 16;
            pcmInfo.Data.NumberOfChannels = 1;
            pcmInfo.Data.SampleRate = 44100;
            Duration_DataLength_RoundTrip(pcmInfo);
            pcmInfo.Data.BitDepth = 8;
            pcmInfo.Data.NumberOfChannels = 2;
            pcmInfo.Data.SampleRate = 22050;
            Duration_DataLength_RoundTrip(pcmInfo);
        }

        private void Duration_DataLength_RoundTrip(PCMFormatInfo pcmInfo)
        {
            Random rnd = new Random();
            uint ba = pcmInfo.Data.BlockAlign;
            for (int i = 0; i < 20480; i++)
            {
                uint dl = (uint) Math.Round(rnd.NextDouble()*UInt32.MaxValue);
                dl -= dl%ba;
                //uint roundI = pcmInfo.Data.GetDataLength(pcmInfo.Data.GetDuration(dl));
                uint roundI = (uint)pcmInfo.Data.ConvertTimeToBytes(pcmInfo.Data.ConvertBytesToTime(dl));
                Assert.AreEqual(
                    dl,
                    roundI,
                    "Wrong round trip data langth value");
            }
        }

        [Test]
        public void XukIn_XukOut_RoundTrim()
        {
            PCMFormatInfo pcmInfo;
            pcmInfo = new PCMFormatInfo(1, 44100, 16);
            TestRoundTrim(pcmInfo);
            pcmInfo = new PCMFormatInfo(1, 22050, 16);
            TestRoundTrim(pcmInfo);
            pcmInfo = new PCMFormatInfo(2, 44100, 16);
            TestRoundTrim(pcmInfo);
            pcmInfo = new PCMFormatInfo(2, 22050, 16);
            TestRoundTrim(pcmInfo);
            pcmInfo = new PCMFormatInfo(1, 44100, 8);
            TestRoundTrim(pcmInfo);
            pcmInfo = new PCMFormatInfo(1, 22050, 8);
            TestRoundTrim(pcmInfo);
        }

        private void TestRoundTrim(PCMFormatInfo info)
        {
            StringBuilder sb = new StringBuilder();
            XmlWriter wr = XmlWriter.Create(sb);
            info.XukOut(wr, new Uri(System.IO.Directory.GetCurrentDirectory()), null);
            wr.Close();
            PCMFormatInfo realodedInfo = new PCMFormatInfo();
            XmlReader rd = XmlReader.Create(new System.IO.StringReader(sb.ToString()));
            rd.ReadToFollowing(info.XukLocalName, info.XukNamespaceUri);
            realodedInfo.XukIn(rd, null);
            Assert.IsTrue(info.ValueEquals(realodedInfo));
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using AudioLib;
using NUnit.Framework;
using urakawa;
using urakawa.media.data.audio;
using urakawa.media.data.audio.codec;
using urakawa.media.timing;

namespace urakawa.media.data.audio.codec
{
    [TestFixture]
    public class WavAudioMediaDataTests
    {
        protected Project mProject;

        protected Presentation mPresentation
        {
            get { return mProject.Presentations.Get(0); }
        }

        protected WavAudioMediaData mData1;
        protected WavAudioMediaData mData2;
        protected WavAudioMediaData mData3;

        [TestFixtureSetUp]
        public void SetUpFixture()
        {
            Uri projectDir = new Uri(ProjectTests.SampleXukFileDirectoryUri, "MediaTestsSample/");
            if (Directory.Exists(Path.Combine(projectDir.LocalPath, "Data")))
            {
                Directory.Delete(Path.Combine(projectDir.LocalPath, "Data"), true);
            }
            mProject = new Project();
            mProject.AddNewPresentation().RootUri = projectDir;
        }

        [TestFixtureTearDown]
        public void TearDownFixture()
        {
            Uri projectDir = new Uri(ProjectTests.SampleXukFileDirectoryUri, "MediaTestsSample/");
            if (Directory.Exists(Path.Combine(projectDir.LocalPath, "Data")))
            {
                Directory.Delete(Path.Combine(projectDir.LocalPath, "Data"), true);
            }
        }

        [SetUp]
        public void SetUp()
        {
            mData1 = mPresentation.MediaDataFactory.Create<WavAudioMediaData>();
            Assert.IsTrue(mData1.PCMFormat.Data.BitDepth == 16, "default bit depth should be 16 bits");
            Assert.IsTrue(mData1.PCMFormat.Data.NumberOfChannels == 1, "default number of channels should be 1");
            Assert.IsTrue(mData1.PCMFormat.Data.SampleRate == 44100, "default sample rate should be 44100 Hz");
            mData2 = mPresentation.MediaDataFactory.Create<WavAudioMediaData>();
            Assert.IsTrue(mData2.PCMFormat.Data.BitDepth == 16, "default bit depth should be 16 bits");
            Assert.IsTrue(mData2.PCMFormat.Data.NumberOfChannels == 1, "default number of channels should be 1");
            Assert.IsTrue(mData2.PCMFormat.Data.SampleRate == 44100, "default sample rate should be 44100 Hz");
            mData3 = mPresentation.MediaDataFactory.Create<WavAudioMediaData>();
            Assert.IsTrue(mData3.PCMFormat.Data.BitDepth == 16, "default bit depth should be 16 bits");
            Assert.IsTrue(mData3.PCMFormat.Data.NumberOfChannels == 1, "default number of channels should be 1");
            Assert.IsTrue(mData3.PCMFormat.Data.SampleRate == 44100, "default sample rate should be 44100 Hz");
        }

        [TearDown]
        public void TearDown()
        {
            mData1.Delete();
            mData2.Delete();
            mData3.Delete();
        }

        private string GetPath(String fileName)
        {
            return Path.Combine(mPresentation.RootUri.LocalPath, fileName);
        }

        private Stream GetRawStream(String fileName)
        {
            Stream s = new FileStream(GetPath(fileName), FileMode.Open, FileAccess.Read, FileShare.Read);
            s.Seek(44, SeekOrigin.Begin);
            return s;
        }

        private AudioLibPCMFormat GetInfo(string name, out uint dataLength)
        {
            FileStream fs = new FileStream(GetPath(name), FileMode.Open, FileAccess.Read, FileShare.Read);
            try
            {
                return AudioLibPCMFormat.RiffHeaderParse(fs, out dataLength);
            }
            finally
            {
                fs.Close();
            }
        }

        [Test]
        public void TestMediaSamples()
        {
            List<Stream> wavSeq1 = new List<Stream>();
            List<Stream> wavSeq2 = new List<Stream>();
            uint dataLength;
            AudioLibPCMFormat info = GetInfo("audiotest1-mono-44100Hz-16bits.wav", out dataLength);
            Assert.IsTrue(info.NumberOfChannels == 1);
            Assert.IsTrue(info.SampleRate == 44100);
            Assert.IsTrue(info.BitDepth == 16);
            Assert.IsTrue(new TimeDelta(info.ConvertBytesToTime(dataLength)).IsEqualTo(new TimeDelta(1500)));
            info = GetInfo("audiotest1-mono-22050Hz-16bits.wav", out dataLength);
            Assert.IsTrue(info.NumberOfChannels == 1);
            Assert.IsTrue(info.SampleRate == 22050);
            Assert.IsTrue(info.BitDepth == 16);
            Assert.IsTrue(new TimeDelta(info.ConvertBytesToTime(dataLength)).IsEqualTo(new TimeDelta(1500)));
            info = GetInfo("audiotest2-mono-44100Hz-16bits.wav", out dataLength);
            Assert.IsTrue(info.NumberOfChannels == 1);
            Assert.IsTrue(info.SampleRate == 44100);
            Assert.IsTrue(info.BitDepth == 16);
            Assert.IsTrue(new TimeDelta(info.ConvertBytesToTime(dataLength)).IsEqualTo(new TimeDelta(1500)));
            info = GetInfo("audiotest3-mono-44100Hz-16bits.wav", out dataLength);
            Assert.IsTrue(info.NumberOfChannels == 1);
            Assert.IsTrue(info.SampleRate == 44100);
            Assert.IsTrue(info.BitDepth == 16);
            Assert.IsTrue(new TimeDelta(info.ConvertBytesToTime(dataLength)).IsEqualTo(new TimeDelta(1500)));
            info = GetInfo("audiotest1+2-mono-44100Hz-16bits.wav", out dataLength);
            Assert.IsTrue(info.NumberOfChannels == 1);
            Assert.IsTrue(info.SampleRate == 44100);
            Assert.IsTrue(info.BitDepth == 16);
            Assert.IsTrue(new TimeDelta(info.ConvertBytesToTime(dataLength)).IsEqualTo(new TimeDelta(3000)));
            info = GetInfo("audiotest1+3-mono-44100Hz-16bits.wav", out dataLength);
            Assert.IsTrue(info.NumberOfChannels == 1);
            Assert.IsTrue(info.SampleRate == 44100);
            Assert.IsTrue(info.BitDepth == 16);
            Assert.IsTrue(new TimeDelta(info.ConvertBytesToTime(dataLength)).IsEqualTo(new TimeDelta(3000)));
            info = GetInfo("audiotest1+2+3-mono-44100Hz-16bits.wav", out dataLength);
            Assert.IsTrue(info.NumberOfChannels == 1);
            Assert.IsTrue(info.SampleRate == 44100);
            Assert.IsTrue(info.BitDepth == 16);
            Assert.IsTrue(new TimeDelta(info.ConvertBytesToTime(dataLength)).IsEqualTo(new TimeDelta(4500)));
            info = GetInfo("audiotest2+1-mono-44100Hz-16bits.wav", out dataLength);
            Assert.IsTrue(info.NumberOfChannels == 1);
            Assert.IsTrue(info.SampleRate == 44100);
            Assert.IsTrue(info.BitDepth == 16);
            Assert.IsTrue(new TimeDelta(info.ConvertBytesToTime(dataLength)).IsEqualTo(new TimeDelta(3000)));
            info = GetInfo("audiotest-stereo-44100Hz-16bits.wav", out dataLength);
            Assert.IsTrue(info.NumberOfChannels == 2);
            Assert.IsTrue(info.SampleRate == 44100);
            Assert.IsTrue(info.BitDepth == 16);
            Assert.IsTrue(new TimeDelta(info.ConvertBytesToTime(dataLength)).IsEqualTo(new TimeDelta(10000)));
            info = GetInfo("omelet_punk.wav", out dataLength);
            Assert.IsTrue(info.NumberOfChannels == 2);
            Assert.IsTrue(info.SampleRate == 44100);
            Assert.IsTrue(info.BitDepth == 16);
            Assert.IsTrue(new TimeDelta(info.ConvertBytesToTime(dataLength)).IsEqualTo(new TimeDelta(TimeSpan.FromTicks(209784807))));
            info = GetInfo("stereo.wav", out dataLength);
            Assert.IsTrue(info.NumberOfChannels == 2);
            Assert.IsTrue(info.SampleRate == 22050);
            Assert.IsTrue(info.BitDepth == 16);
            Assert.IsTrue(new TimeDelta(info.ConvertBytesToTime(dataLength)).IsEqualTo(new TimeDelta(16620.816300000001)));


            // tests 1+2
            wavSeq1.Clear();
            wavSeq1.Add(new FileStream(GetPath("audiotest1+2-mono-44100Hz-16bits.wav"), FileMode.Open, FileAccess.Read,
                                       FileShare.Read));
            wavSeq2.Clear();
            wavSeq2.Add(new FileStream(GetPath("audiotest1-mono-44100Hz-16bits.wav"), FileMode.Open, FileAccess.Read,
                                       FileShare.Read));
            wavSeq2.Add(new FileStream(GetPath("audiotest2-mono-44100Hz-16bits.wav"), FileMode.Open, FileAccess.Read,
                                       FileShare.Read));
            Assert.IsTrue(StreamUtils.CompareWavSeq(wavSeq1, wavSeq2));
            // tests 1+3
            wavSeq1.Clear();
            wavSeq1.Add(new FileStream(GetPath("audiotest1+3-mono-44100Hz-16bits.wav"), FileMode.Open, FileAccess.Read,
                                       FileShare.Read));
            wavSeq2.Clear();
            wavSeq2.Add(new FileStream(GetPath("audiotest1-mono-44100Hz-16bits.wav"), FileMode.Open, FileAccess.Read,
                                       FileShare.Read));
            wavSeq2.Add(new FileStream(GetPath("audiotest3-mono-44100Hz-16bits.wav"), FileMode.Open, FileAccess.Read,
                                       FileShare.Read));
            Assert.IsTrue(StreamUtils.CompareWavSeq(wavSeq1, wavSeq2));
            // tests 1+2+3
            wavSeq1.Clear();
            wavSeq1.Add(new FileStream(GetPath("audiotest1+2+3-mono-44100Hz-16bits.wav"), FileMode.Open, FileAccess.Read,
                                       FileShare.Read));
            wavSeq2.Clear();
            wavSeq2.Add(new FileStream(GetPath("audiotest1-mono-44100Hz-16bits.wav"), FileMode.Open, FileAccess.Read,
                                       FileShare.Read));
            wavSeq2.Add(new FileStream(GetPath("audiotest2-mono-44100Hz-16bits.wav"), FileMode.Open, FileAccess.Read,
                                       FileShare.Read));
            wavSeq2.Add(new FileStream(GetPath("audiotest3-mono-44100Hz-16bits.wav"), FileMode.Open, FileAccess.Read,
                                       FileShare.Read));
            Assert.IsTrue(StreamUtils.CompareWavSeq(wavSeq1, wavSeq2));
        }


        [Test]
        public void ValueEquals_Basics()
        {
            Assert.IsFalse(mData1.ValueEquals(null), "a created media data shouldn't equal null");
            Assert.IsTrue(mData1.ValueEquals(mData1), "an empty media data should equal itself");
            Assert.IsTrue(mData1.ValueEquals(mData2), "two identically created empty media datas should be equal");
            mData1.AppendPcmData_RiffHeader(Path.Combine(mPresentation.RootUri.LocalPath,
                                                            "audiotest1-mono-44100Hz-16bits.wav"));
            Assert.IsTrue(mData1.ValueEquals(mData1), "a data with content should equal itself");
        }

        [Test]
        public void ValueEquals_PCMFormat()
        {
            mData1.Presentation.MediaDataManager.EnforceSinglePCMFormat = false;

            Assert.IsTrue(mData1.PCMFormat.ValueEquals(mData2.PCMFormat), "[Pre-Condition] PCM formats should be equal");
            Assert.IsTrue(mData1.ValueEquals(mData2), "empty media datas with the same PCM format should be equal");
            mData1.PCMFormat.Data.NumberOfChannels = 1;
            mData2.PCMFormat.Data.NumberOfChannels = 2;
            Assert.IsFalse(mData1.PCMFormat.ValueEquals(mData2.PCMFormat),
                           "[Pre-Condition] PCM formats shouldn't be equal");
            Assert.IsFalse(mData1.ValueEquals(mData2), "empty media datas with different PCM formats shouldn't be equal");
        }

        [Test]
        public void ValueEquals_SameAudioDataSingleWavClip()
        {
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData2.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            Assert.IsTrue(mData1.ValueEquals(mData2), "datas with the same wav content should be equal");
        }

        [Test]
        public void ValueEquals_SameAudioDataSameWavClips()
        {
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest2-mono-44100Hz-16bits.wav"));
            mData2.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData2.AppendPcmData_RiffHeader(GetPath("audiotest2-mono-44100Hz-16bits.wav"));
            Assert.IsTrue(mData1.ValueEquals(mData2), "datas with the same wav content should be equal");
        }

        [Test]
        public void ValueEquals_SameAudioDataDiffWavClips()
        {
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1+2-mono-44100Hz-16bits.wav"));
            mData2.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData2.AppendPcmData_RiffHeader(GetPath("audiotest2-mono-44100Hz-16bits.wav"));
            Assert.IsTrue(mData1.ValueEquals(mData2), "datas with the same wav content should be equal");
        }

        [Test]
        public void ValueEquals_DiffAudioData()
        {
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData2.AppendPcmData_RiffHeader(GetPath("audiotest2-mono-44100Hz-16bits.wav"));
            Assert.IsTrue(mData1.AudioDuration.IsEqualTo(mData2.AudioDuration),
                          "[Pre-Condition] audio duration should be equal");
            Assert.IsFalse(mData1.ValueEquals(mData2), "datas created from different wav files shouldn't be equal");
        }

        [Test]
        public void ValueEquals_DiffPCMFormat()
        {
            mData1.Presentation.MediaDataManager.EnforceSinglePCMFormat = false;

            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData2.PCMFormat.Data.SampleRate = 22050;
            mData2.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-22050Hz-16bits.wav"));
            Assert.IsFalse(mData1.PCMFormat.ValueEquals(mData2.PCMFormat),
                           "[Pre-Condition] PCM formats shouldn't be equal");
            Assert.IsFalse(mData1.ValueEquals(mData2), "datas created from different wav files shouldn't be equal");
        }


        [Test]
        [ExpectedException(typeof (exception.MethodParameterIsOutOfBoundsException))]
        public void InsertAudioData_NegativeInsertPoint()
        {
            mData1.InsertPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"), new Time(-1),
                                               new TimeDelta(1));
        }

        [Test]
        [ExpectedException(typeof (exception.MethodParameterIsOutOfBoundsException))]
        public void InsertAudioData_InsertionGreaterThanDurationEmpty()
        {
            mData1.InsertPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"), new Time(10000),
                                               new TimeDelta(1));
        }

        [Test]
        [ExpectedException(typeof (exception.MethodParameterIsOutOfBoundsException))]
        public void InsertAudioData_InsertionGreaterThanDuration()
        {
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData1.InsertPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"), new Time(10000),
                                               new TimeDelta(1));
        }

        [Test]
        [ExpectedException(typeof (exception.MethodParameterIsOutOfBoundsException))]
        public void InsertAudioData_NegativeDuration()
        {
            mData1.InsertPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"), Time.Zero,
                                               new TimeDelta(-1));
        }

        [Test]
        [ExpectedException(typeof (exception.MethodParameterIsOutOfBoundsException))]
        public void InsertAudioData_DurationGreaterThanData()
        {
            mData1.InsertPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"), Time.Zero,
                                               new TimeDelta(10000));
        }

        [Test]
        public void InsertAudioData_AtTheBeginning()
        {
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1+2-mono-44100Hz-16bits.wav"));
            mData2.AppendPcmData_RiffHeader(GetPath("audiotest2-mono-44100Hz-16bits.wav"));
            mData2.InsertPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"), Time.Zero,
                                               new TimeDelta(1500));
            Assert.IsTrue(mData1.ValueEquals(mData2), "the two audio datas should be equal");
        }

        [Test]
        public void InsertAudioData_AtTheEnd()
        {
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1+2-mono-44100Hz-16bits.wav"));
            mData2.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData2.InsertPcmData_RiffHeader(GetPath("audiotest2-mono-44100Hz-16bits.wav"), new Time(1500),
                                               new TimeDelta(1500));
            Assert.IsTrue(mData1.ValueEquals(mData2), "the two audio datas should be equal");
        }

        [Test]
        public void InsertAudioData_BetweenTwoWavClips()
        {
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1+2+3-mono-44100Hz-16bits.wav"));
            mData2.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData2.AppendPcmData_RiffHeader(GetPath("audiotest3-mono-44100Hz-16bits.wav"));
            mData2.InsertPcmData_RiffHeader(GetPath("audiotest2-mono-44100Hz-16bits.wav"), new Time(1500),
                                               new TimeDelta(1500));
            Assert.IsTrue(mData1.ValueEquals(mData2), "the two audio datas should be equal");
        }

        [Test]
        public void InsertAudioData_InAWavClip()
        {
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1+2+3-mono-44100Hz-16bits.wav"));
            mData2.AppendPcmData_RiffHeader(GetPath("audiotest1+3-mono-44100Hz-16bits.wav"));
            mData2.InsertPcmData_RiffHeader(GetPath("audiotest2-mono-44100Hz-16bits.wav"), new Time(1500),
                                               new TimeDelta(1500));
            Assert.IsTrue(mData1.ValueEquals(mData2), "the two audio datas should be equal");
        }


        [Test]
        public void AppendAudioData_ToEmptyData()
        {
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData2.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            Assert.IsTrue(mData1.ValueEquals(mData2), "the two audio datas should be equal");
        }

        [Test]
        public void AppendAudioData_Stereo_ToEmptyData()
        {
            mData1.Presentation.MediaDataManager.EnforceSinglePCMFormat = false;

            mData1.PCMFormat.Data.NumberOfChannels = 2;
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest-stereo-44100Hz-16bits.wav"));
            mData2.PCMFormat.Data.NumberOfChannels = 2;
            mData2.AppendPcmData_RiffHeader(GetPath("audiotest-stereo-44100Hz-16bits.wav"));
            Assert.IsTrue(mData1.ValueEquals(mData2), "the two audio datas should be equal");
            mData1.RemovePcmData(Time.Zero);
            mData2.RemovePcmData(Time.Zero);
            mData1.AppendPcmData_RiffHeader(GetPath("omelet_punk.wav"));
            mData2.AppendPcmData_RiffHeader(GetPath("omelet_punk.wav"));
            Assert.IsTrue(mData1.ValueEquals(mData2), "the two audio datas should be equal");
            mData1.RemovePcmData(Time.Zero);
            mData2.RemovePcmData(Time.Zero);
            mData1.PCMFormat.Data.SampleRate = 22050;
            mData1.AppendPcmData_RiffHeader(GetPath("stereo.wav"));
            mData2.PCMFormat.Data.SampleRate = 22050;
            mData2.AppendPcmData_RiffHeader(GetPath("stereo.wav"));
            Assert.IsTrue(mData1.ValueEquals(mData2), "the two audio datas should be equal");
        }


		[Test]
		[ExpectedException(typeof (exception.InvalidDataFormatException))]
        public void AppendAudioData_StereoFileToMonoAudioMediaData()
        {
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest-stereo-44100Hz-16bits.wav"));
        }

        [Test]
        public void AppendAudioData_ToEqualSingleWavClip()
        {
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1+2-mono-44100Hz-16bits.wav"));
            mData2.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData2.AppendPcmData_RiffHeader(GetPath("audiotest2-mono-44100Hz-16bits.wav"));
            Assert.IsTrue(mData1.ValueEquals(mData2), "the two audio datas should be equal");
        }

        [Test]
        public void AppendAudioData_ToEqualMultipleWavClips()
        {
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest2-mono-44100Hz-16bits.wav"));
            mData2.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData2.AppendPcmData_RiffHeader(GetPath("audiotest2-mono-44100Hz-16bits.wav"));
            Assert.IsTrue(mData1.ValueEquals(mData2), "the two audio datas should be equal");
        }


        [Test]
        [ExpectedException(typeof (exception.MethodParameterIsOutOfBoundsException))]
        public void ReplaceAudioData_NegativeInsertPoint()
        {
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData1.ReplacePcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"), new Time(-1),
                                                new TimeDelta(1));
        }

        [Test]
        [ExpectedException(typeof (exception.MethodParameterIsOutOfBoundsException))]
        public void ReplaceAudioData_insertionGreaterThanDurationEmpty()
        {
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData1.ReplacePcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"), new Time(10000),
                                                new TimeDelta(1));
        }

        [Test]
        [ExpectedException(typeof (exception.MethodParameterIsOutOfBoundsException))]
        public void ReplaceAudioData_insertionGreaterThanDuration()
        {
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData1.ReplacePcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"), new Time(10000),
                                                new TimeDelta(1));
        }

		[Test]
		[ExpectedException(typeof (exception.MethodParameterIsOutOfBoundsException))]
        public void ReplaceAudioData_NegativeDuration()
        {
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData1.ReplacePcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"), Time.Zero,
                                                new TimeDelta(-1));
        }

		[Test]
        [ExpectedException(typeof (exception.MethodParameterIsOutOfBoundsException))]
        public void ReplaceAudioData_DurationGreaterThanData()
        {
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData1.ReplacePcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"), Time.Zero,
                                                new TimeDelta(10000));
        }
		
		[Test]
        [ExpectedException(typeof (exception.MethodParameterIsOutOfBoundsException))]
        public void ReplaceAudioData_DurationGreaterThanRemainingDataToReplace()
        {
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData1.ReplacePcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"), new Time(10),
                                                new TimeDelta(1500));
        }

		[Test]
		public void ReplaceAudioData_AtTheBeginning()
        {
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1+2+3-mono-44100Hz-16bits.wav"));
            mData2.AppendPcmData_RiffHeader(GetPath("audiotest1+2+3-mono-44100Hz-16bits.wav"));
            mData2.ReplacePcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"), Time.Zero,
                                                new TimeDelta(1500));
            Assert.IsTrue(mData1.ValueEquals(mData2), "the two audio datas should be equal");
        }

        [Test]
        public void ReplaceAudioData_AtTheEnd()
        {
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1+2+3-mono-44100Hz-16bits.wav"));
            mData2.AppendPcmData_RiffHeader(GetPath("audiotest1+2+3-mono-44100Hz-16bits.wav"));
            mData2.ReplacePcmData_RiffHeader(GetPath("audiotest3-mono-44100Hz-16bits.wav"), new Time(3000),
                                                new TimeDelta(1500));
            Assert.IsTrue(mData1.ValueEquals(mData2), "the two audio datas should be equal");
        }

        [Test]
        public void ReplaceAudioData_SmallerThanAWavClip()
        {
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1+2+3-mono-44100Hz-16bits.wav"));
            mData2.AppendPcmData_RiffHeader(GetPath("audiotest1+2+3-mono-44100Hz-16bits.wav"));
            mData2.ReplacePcmData_RiffHeader(GetPath("audiotest2-mono-44100Hz-16bits.wav"), new Time(1500),
                                                new TimeDelta(1500));
            Assert.IsTrue(mData1.ValueEquals(mData2), "the two audio datas should be equal");
        }

        [Test]
        public void ReplaceAudioData_ExactlyAWavClip()
        {
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1+2+3-mono-44100Hz-16bits.wav"));
            mData2.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData2.AppendPcmData_RiffHeader(GetPath("audiotest2-mono-44100Hz-16bits.wav"));
            mData2.AppendPcmData_RiffHeader(GetPath("audiotest3-mono-44100Hz-16bits.wav"));
            mData2.ReplacePcmData_RiffHeader(GetPath("audiotest2-mono-44100Hz-16bits.wav"), new Time(1500),
                                                new TimeDelta(1500));
            Assert.IsTrue(mData1.ValueEquals(mData2), "the two audio datas should be equal");
        }

        [Test]
        public void ReplaceAudioData_SpanningTwoWavClips()
        {
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1+2-mono-44100Hz-16bits.wav"));
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1+2-mono-44100Hz-16bits.wav"));
            mData2.AppendPcmData_RiffHeader(GetPath("audiotest1+2-mono-44100Hz-16bits.wav"));
            mData2.AppendPcmData_RiffHeader(GetPath("audiotest1+2-mono-44100Hz-16bits.wav"));
            mData2.ReplacePcmData_RiffHeader(GetPath("audiotest2+1-mono-44100Hz-16bits.wav"), new Time(1500),
                                                new TimeDelta(3000));
            Assert.IsTrue(mData1.ValueEquals(mData2), "the two audio datas should be equal");
        }


        [Test]
        [ExpectedException(typeof (exception.MethodParameterIsOutOfBoundsException))]
        public void RemovePcmData_ClipBeginNegative()
        {
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData1.RemovePcmData(new Time(-1));
        }

        [Test]
        [ExpectedException(typeof (exception.MethodParameterIsOutOfBoundsException))]
        public void RemovePcmData_ClipBeginGreaterThanDuration()
        {
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData1.RemovePcmData(new Time(10000));
        }

        [Test]
        [ExpectedException(typeof (exception.MethodParameterIsOutOfBoundsException))]
        public void RemovePcmData_ClipBeginGreaterThanClipEnd()
        {
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData1.RemovePcmData(new Time(2), new Time(1));
        }

		[Test]
		[ExpectedException(typeof (exception.MethodParameterIsOutOfBoundsException))]
        public void RemovePcmData_ClipEndNegative()
        {
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData1.RemovePcmData(new Time(1), new Time(-1));
        }

		[Test]
        [ExpectedException(typeof (exception.MethodParameterIsOutOfBoundsException))]
        public void RemovePcmData_ClipEndGreaterThanDuration()
        {
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData1.RemovePcmData(new Time(1), new Time(10000));
        }

		[Test]
        public void RemovePcmData_ClipBeginEqualClipEnd()
        {
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData2.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData2.RemovePcmData(new Time(1000), new Time(1000));
            Assert.IsTrue(mData1.ValueEquals(mData2), "the two audio datas should be equal");
        }

		[Test]
		public void RemovePcmData_FromTheBeginning()
        {
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData2.AppendPcmData_RiffHeader(GetPath("audiotest2+1-mono-44100Hz-16bits.wav"));
            mData2.RemovePcmData(Time.Zero, new Time(1500));
            Assert.IsTrue(mData1.ValueEquals(mData2), "the two audio datas should be equal");
        }

		[Test]
		public void RemovePcmData_AtTheEnd()
        {
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData2.AppendPcmData_RiffHeader(GetPath("audiotest1+2-mono-44100Hz-16bits.wav"));
            mData2.RemovePcmData(new Time(1500), new Time(3000));
            Assert.IsTrue(mData1.ValueEquals(mData2), "the two audio datas should be equal");
        }

		[Test]
		public void RemovePcmData_RemoveAllSingleClip()
        {
            mData2.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData2.RemovePcmData(Time.Zero, new Time(1500));
            Assert.IsTrue(mData1.ValueEquals(mData2), "the two audio datas should be equal");
        }

		[Test]
		public void RemovePcmData_RemoveAllMultiClip()
        {
            mData2.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData2.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData2.RemovePcmData(Time.Zero, new Time(3000));
            Assert.IsTrue(mData1.ValueEquals(mData2), "the two audio datas should be equal");
        }

		[Test]
        public void RemovePcmData_OneArg()
        {
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData2.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData1.RemovePcmData(new Time(1000));
            mData2.RemovePcmData(new Time(1000), new Time(1500));
            Assert.IsTrue(mData1.ValueEquals(mData2), "the two audio datas should be equal");
        }

		[Test]
        public void RemovePcmData_IncludedInAWavClip()
        {
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1+3-mono-44100Hz-16bits.wav"));
            mData2.AppendPcmData_RiffHeader(GetPath("audiotest1+2+3-mono-44100Hz-16bits.wav"));
            mData2.RemovePcmData(new Time(1500), new Time(3000));
            Assert.IsTrue(mData1.ValueEquals(mData2), "the two audio datas should be equal");
        }

		[Test]
        public void RemovePcmData_ExactlyAWavClip()
        {
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1+3-mono-44100Hz-16bits.wav"));
            mData2.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData2.AppendPcmData_RiffHeader(GetPath("audiotest2-mono-44100Hz-16bits.wav"));
            mData2.AppendPcmData_RiffHeader(GetPath("audiotest3-mono-44100Hz-16bits.wav"));
            mData2.RemovePcmData(new Time(1500), new Time(3000));
            Assert.IsTrue(mData1.ValueEquals(mData2), "the two audio datas should be equal");
        }

		[Test]
        public void RemovePcmData_SpanningTwoWavClips()
        {
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1+2-mono-44100Hz-16bits.wav"));
            mData2.AppendPcmData_RiffHeader(GetPath("audiotest1+2-mono-44100Hz-16bits.wav"));
            mData2.AppendPcmData_RiffHeader(GetPath("audiotest1+2-mono-44100Hz-16bits.wav"));
            mData2.RemovePcmData(new Time(1500), new Time(4500));
            Assert.IsTrue(mData1.ValueEquals(mData2), "the two audio datas should be equal");
        }

		[Test]
        [ExpectedException(typeof (exception.MethodParameterIsOutOfBoundsException))]
        public void OpenPcmInputStream_ClipBeginNegative()
        {
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData1.OpenPcmInputStream(new Time(-1));
        }

		[Test]
        [ExpectedException(typeof (exception.MethodParameterIsOutOfBoundsException))]
        public void OpenPcmInputStream_ClipBeginGreaterThanDuration()
        {
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData1.OpenPcmInputStream(new Time(10000));
        }

		[Test]
		[ExpectedException(typeof (exception.MethodParameterIsOutOfBoundsException))]
        public void OpenPcmInputStream_ClipBeginGreaterThanClipEnd()
        {
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData1.OpenPcmInputStream(new Time(2), new Time(1)).Close();
        }

		[Test]
		[ExpectedException(typeof (exception.MethodParameterIsOutOfBoundsException))]
        public void OpenPcmInputStream_ClipEndNegative()
        {
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData1.OpenPcmInputStream(new Time(1), new Time(-1)).Close();
        }

		[Test]
        [ExpectedException(typeof (exception.MethodParameterIsOutOfBoundsException))]
        public void OpenPcmInputStream_ClipEndGreaterThanDuration()
        {
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData1.OpenPcmInputStream(Time.Zero, new Time(10000)).Close();
        }

		[Test]
        public void OpenPcmInputStream_ClipBeginEqualClipEnd()
        {
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            Stream s = mData1.OpenPcmInputStream(new Time(1000), new Time(1000));
            try
            {
                Assert.IsTrue(s.Length == 0, "the returned audio data should be empty");
            }
            finally
            {
                s.Close();
            }
        }

        [Test]
        public void OpenPcmInputStream_EmptyData()
        {
            Assert.IsTrue(!mData1.HasActualPcmData);

            //Stream s = mData1.OpenPcmInputStream();
            //try
            //{
            //    Assert.IsTrue(s.Length == 0, "the returned audio data should be empty");
            //}
            //finally
            //{
            //    s.Close();
            //}
        }

		[Test]
        public void OpenPcmInputStream_Stereo_FromTheBeginning()
        {
            mData1.PCMFormat.Data.NumberOfChannels = 2;
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest-stereo-44100Hz-16bits.wav"));
            Stream s1 = mData1.OpenPcmInputStream(Time.Zero, new Time(10000));
            try
            {
                Stream s2 = GetRawStream("audiotest-stereo-44100Hz-16bits.wav");
                try
                {
                    Assert.IsTrue(StreamUtils.CompareStreams(s1, s2), "the two streams should be equal");
                }
                finally
                {
                    s2.Close();
                }
            }
            finally
            {
                s1.Close();
            }
        }

		[Test]
        public void OpenPcmInputStream_FromTheBeginning()
        {
		    mData1.Presentation.MediaDataManager.EnforceSinglePCMFormat = false;

            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1+2-mono-44100Hz-16bits.wav"));
            Stream s1 = mData1.OpenPcmInputStream(Time.Zero, new Time(1500));
            try
            {
                Stream s2 = GetRawStream("audiotest1-mono-44100Hz-16bits.wav");
                try
                {
                    Assert.IsTrue(StreamUtils.CompareStreams(s1, s2), "the two streams should be equal");
                }
                finally
                {
                    s2.Close();
                }
            }
            finally
            {
                s1.Close();
            }
        }

		[Test]
        public void OpenPcmInputStream_AtTheEnd()
        {
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1+2-mono-44100Hz-16bits.wav"));
            Stream s1 = mData1.OpenPcmInputStream(new Time(1500), new Time(3000));
            try
            {
                Stream s2 = GetRawStream("audiotest2-mono-44100Hz-16bits.wav");
                try
                {
                    Assert.IsTrue(StreamUtils.CompareStreams(s1, s2), "the two streams should be equal");
                }
                finally
                {
                    s2.Close();
                }
            }
            finally
            {
                s1.Close();
            }
        }

		[Test]
        public void OpenPcmInputStream_getAllSingleClip()
        {
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            Stream s1 = mData1.OpenPcmInputStream();
            try
            {
                Stream s2 = GetRawStream("audiotest1-mono-44100Hz-16bits.wav");
                try
                {
                    Assert.IsTrue(StreamUtils.CompareStreams(s1, s2), "the two streams should be equal");
                }
                finally
                {
                    s2.Close();
                }
            }
            finally
            {
                s1.Close();
            }
        }

		[Test]
        public void OpenPcmInputStream_getAllMultiClip()
        {
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest2-mono-44100Hz-16bits.wav"));
            Stream s1 = mData1.OpenPcmInputStream();
            try
            {
                Stream s2 = GetRawStream("audiotest1+2-mono-44100Hz-16bits.wav");
                try
                {
                    Assert.IsTrue(StreamUtils.CompareStreams(s1, s2), "the two streams should be equal");
                }
                finally
                {
                    s2.Close();
                }
            }
            finally
            {
                s1.Close();
            }
        }

		[Test]
        public void OpenPcmInputStream_OneArg()
        {
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            Stream s1 = mData1.OpenPcmInputStream(new Time(1000), new Time(1500));
            try
            {
                Stream s2 = mData1.OpenPcmInputStream(new Time(1000));
                try
                {
                    Assert.IsTrue(StreamUtils.CompareStreams(s1, s2), "the two streams should be equal");
                }
                finally
                {
                    s2.Close();
                }
            }
            finally
            {
                s1.Close();
            }
        }

		[Test]
        public void OpenPcmInputStream_ZeroArg()
        {
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            Stream s1 = mData1.OpenPcmInputStream(Time.Zero, new Time(1500));
            try
            {
                Stream s2 = mData1.OpenPcmInputStream();
                try
                {
                    Assert.IsTrue(StreamUtils.CompareStreams(s1, s2), "the two streams should be equal");
                }
                finally
                {
                    s2.Close();
                }
            }
            finally
            {
                s1.Close();
            }
        }

		[Test]
        public void OpenPcmInputStream_IncludedInAWavClip()
        {
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1+2+3-mono-44100Hz-16bits.wav"));
            Stream s1 = mData1.OpenPcmInputStream(new Time(1500), new Time(3000));
            try
            {
                Stream s2 = GetRawStream("audiotest2-mono-44100Hz-16bits.wav");
                try
                {
                    Assert.IsTrue(StreamUtils.CompareStreams(s1, s2), "the two streams should be equal");
                }
                finally
                {
                    s2.Close();
                }
            }
            finally
            {
                s1.Close();
            }
        }

		[Test]
        public void OpenPcmInputStream_ExactlyAWavClip()
        {
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest2-mono-44100Hz-16bits.wav"));
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest3-mono-44100Hz-16bits.wav"));
            Stream s1 = mData1.OpenPcmInputStream(new Time(1500), new Time(3000));
            try
            {
                Stream s2 = GetRawStream("audiotest2-mono-44100Hz-16bits.wav");
                try
                {
                    Assert.IsTrue(StreamUtils.CompareStreams(s1, s2), "the two streams should be equal");
                }
                finally
                {
                    s2.Close();
                }
            }
            finally
            {
                s1.Close();
            }
        }

		[Test]
        public void OpenPcmInputStream_SpanningTwoWavClips()
        {
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1+2-mono-44100Hz-16bits.wav"));
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1+2-mono-44100Hz-16bits.wav"));
            Stream s1 = mData1.OpenPcmInputStream(new Time(1500), new Time(4500));
            try
            {
                Stream s2 = GetRawStream("audiotest2+1-mono-44100Hz-16bits.wav");
                try
                {
                    Assert.IsTrue(StreamUtils.CompareStreams(s1, s2), "the two streams should be equal");
                }
                finally
                {
                    s2.Close();
                }
            }
            finally
            {
                s1.Close();
            }
        }

		[Test]
        [ExpectedException(typeof (exception.InvalidDataFormatException))]
        public void MergeWith_IncompatiblePCMFormat()
        {
            mData1.PCMFormat.Data.NumberOfChannels = 2;
            mData1.MergeWith(mData2);
        }

		[Test]
		public void MergeWith_EmptyData()
        {
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData2.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData2.MergeWith(mData3);

            Assert.IsTrue(!mData3.HasActualPcmData);
            //Assert.IsTrue(mData3.OpenPcmInputStream().Length == 0);

            Assert.IsTrue(mData1.ValueEquals(mData2), "the two audio datas should be equal");
        }

		[Test]
        public void MergeWith_EmptyCaller()
        {
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData3.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData2.MergeWith(mData3);

            Assert.IsTrue(!mData3.HasActualPcmData);
            //Assert.IsTrue(mData3.OpenPcmInputStream().Length == 0);

            Assert.IsTrue(mData1.ValueEquals(mData2), "the two audio datas should be equal");
        }

		[Test]
        public void MergeWith_OneWavClip()
        {
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1+2-mono-44100Hz-16bits.wav"));
            mData2.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData3.AppendPcmData_RiffHeader(GetPath("audiotest2-mono-44100Hz-16bits.wav"));
            mData2.MergeWith(mData3);

            Assert.IsTrue(!mData3.HasActualPcmData);
            //Assert.IsTrue(mData3.OpenPcmInputStream().Length == 0);

            Assert.IsTrue(mData1.ValueEquals(mData2), "the two audio datas should be equal");
        }

		[Test]
        public void MergeWith_MultiWavClips()
        {
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1+2+3-mono-44100Hz-16bits.wav"));
            mData2.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData3.AppendPcmData_RiffHeader(GetPath("audiotest2-mono-44100Hz-16bits.wav"));
            mData3.AppendPcmData_RiffHeader(GetPath("audiotest3-mono-44100Hz-16bits.wav"));
            mData2.MergeWith(mData3);

            Assert.IsTrue(!mData3.HasActualPcmData);
            //Assert.IsTrue(mData3.OpenPcmInputStream().Length == 0);

            Assert.IsTrue(mData1.ValueEquals(mData2), "the two audio datas should be equal");
        }

		[Test]
        public void MergeWith_MultiWavClipsInCaller()
        {
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1+2+3-mono-44100Hz-16bits.wav"));
            mData2.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData2.AppendPcmData_RiffHeader(GetPath("audiotest2-mono-44100Hz-16bits.wav"));
            mData3.AppendPcmData_RiffHeader(GetPath("audiotest3-mono-44100Hz-16bits.wav"));
            mData2.MergeWith(mData3);

            Assert.IsTrue(!mData3.HasActualPcmData);
            //Assert.IsTrue(mData3.OpenPcmInputStream().Length == 0);

            Assert.IsTrue(mData1.ValueEquals(mData2), "the two audio datas should be equal");
        }

		[Test]
        [ExpectedException(typeof (exception.MethodParameterIsOutOfBoundsException))]
        public void Split_OutOfBoundNegative()
        {
            mData1.Split(new Time(-1));
        }

		[Test]
        [ExpectedException(typeof (exception.MethodParameterIsOutOfBoundsException))]
        public void Split_OutOfBoundBeyond()
        {
            mData1.Split(new Time(1));
        }

		[Test]
        public void Split_EmptyData()
        {
            AudioMediaData split = mData1.Split(Time.Zero);
            Assert.IsTrue(split.ValueEquals(mData2), "the split data should be empty");
            Assert.IsTrue(mData1.ValueEquals(mData3), "the remaining data should be empty");
        }

		[Test]
        public void Split_AtTheBeginning()
        {
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            AudioMediaData split = mData1.Split(Time.Zero);
            mData2.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            Assert.IsTrue(split.ValueEquals(mData2), "the split data is not what was expected");
            Assert.IsTrue(mData1.ValueEquals(mData3), "the remaining data should be empty");
        }

		[Test]
        public void Split_AtTheEnd()
        {
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            AudioMediaData split = mData1.Split(new Time(1500));
            mData3.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            Assert.IsTrue(split.ValueEquals(mData2), "the split data should be empty");
            Assert.IsTrue(mData1.ValueEquals(mData3), "the remaining data is not what was expecte");
        }

		[Test]
        public void Split_InsideAWavClip()
        {
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1+2-mono-44100Hz-16bits.wav"));
            AudioMediaData split = mData1.Split(new Time(1500));
            mData2.AppendPcmData_RiffHeader(GetPath("audiotest2-mono-44100Hz-16bits.wav"));
            mData3.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            Assert.IsTrue(split.ValueEquals(mData2), "the split data is not what was expected");
            Assert.IsTrue(mData1.ValueEquals(mData3), "the remaining data is not what was expected");
        }

		[Test]
        public void Split_InsideAWavClipInMultiClipData()
        {
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest2+1-mono-44100Hz-16bits.wav"));
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest2-mono-44100Hz-16bits.wav"));
            AudioMediaData split = mData1.Split(new Time(3000));
            mData2.AppendPcmData_RiffHeader(GetPath("audiotest1+2-mono-44100Hz-16bits.wav"));
            mData3.AppendPcmData_RiffHeader(GetPath("audiotest1+2-mono-44100Hz-16bits.wav"));
            Assert.IsTrue(split.ValueEquals(mData2), "the split data is not what was expected");
            Assert.IsTrue(mData1.ValueEquals(mData3), "the remaining data is not what was expected");
        }

		[Test]
        public void Split_BetweenWavClips()
        {
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest2-mono-44100Hz-16bits.wav"));
            AudioMediaData split = mData1.Split(new Time(1500));
            mData2.AppendPcmData_RiffHeader(GetPath("audiotest2-mono-44100Hz-16bits.wav"));
            mData3.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            Assert.IsTrue(split.ValueEquals(mData2), "the split data is not what was expected");
            Assert.IsTrue(mData1.ValueEquals(mData3), "the remaining data is not what was expected");
        }

		[Test]
        public void Copy_Empty()
        {
            WavAudioMediaData copy = mData1.Copy();
            Assert.IsTrue(mData1.ValueEquals(copy));
            //List<DataProvider> provList = mData1.UsedDataProviders();
            //List<DataProvider> provListCopy = copy.UsedDataProviders();
            //Assert.IsTrue(provList.Count == provListCopy.Count,"the copy does not have the same number of data providers");
            //for (int i = 0; i < provList.Count; i++)
            //{
            //    Assert.IsTrue(provList[i].ValueEquals(provListCopy[i]),"a data provider of the copy was not equal to the data provider of the original data");
            //}
        }

		[Test]
        public void Copy_OneClip()
        {
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            WavAudioMediaData copy = mData1.Copy();
            Assert.IsTrue(mData1.ValueEquals(copy), "Copy does not have the same value as the original");
            
            
            //List<DataProvider> provList = mData1.UsedDataProviders();
            //List<DataProvider> provListCopy = copy.UsedDataProviders();
            //Assert.IsTrue(provList.Count == provListCopy.Count, "the copy does not have the same number of data providers");
            //for (int i = 0; i < provList.Count; i++)
            //{
            //    Assert.IsTrue(provList[i].ValueEquals(provListCopy[i]), "a data provider of the copy was not equal to the data provider of the original data");
            //}
        }

        private static void CheckOrigCopyCoExistance(WavAudioMediaData orig)
        {
            WavAudioMediaData copy = orig.Copy();
            Stream origStream = orig.OpenPcmInputStream();
            Stream copyStream = copy.OpenPcmInputStream();
            byte[] dataOrig = new byte[1024];
            byte[] dataCopy = new byte[1024];
            for (int i = 0; i < origStream.Length; i += 1024)
            {
                int oC = origStream.Read(dataOrig, 0, 1024);
                int cC = copyStream.Read(dataCopy, 0, 1024);
                Assert.AreEqual(oC, cC, "The number of bytes read from the original and copy Streams do not match");
                Assert.AreEqual(dataOrig, dataCopy, "The data read from the original and copy Streams do not match");
            }
            origStream.Close();
            copyStream.Close();
        }

		[Test]
        public void Copy_AudioStreamsOfOriginalAndCopyCanCoExist()
        {
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            CheckOrigCopyCoExistance(mData1);
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            CheckOrigCopyCoExistance(mData1);
            double ms = mData1.AudioDuration.TimeDeltaAsMillisecondDouble;
            Time cb = new Time(ms/4f);
            Time ce = new Time(3f*ms/4f);
            mData1.RemovePcmData(cb, ce);
            CheckOrigCopyCoExistance(mData1);
        }

        private static byte[] OpenPcmInputStream(AudioMediaData audioMediaData)
        {
            Stream ad = audioMediaData.OpenPcmInputStream();
            byte[] res = new byte[ad.Length];
            ad.Read(res, 0, res.Length);
            ad.Close();
            return res;
        }

        private static void CheckAudioData(AudioMediaData audioMediaData, byte[] expectedData)
        {
            byte[] curData = OpenPcmInputStream(audioMediaData);
            Assert.AreEqual(expectedData, curData, "The audio data is not as expected");
        }

		[Test]
        public void Copy_CheckCopyIndependance()
        {
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            WavAudioMediaData copy = mData1.Copy();
            byte[] copyDataBefore = OpenPcmInputStream(copy);
            double ms = mData1.AudioDuration.TimeDeltaAsMillisecondDouble;
            Time cb = new Time(ms / 4f);
            Time ce = new Time(3f * ms / 4f);
            mData1.RemovePcmData(cb, ce);
            CheckAudioData(copy, copyDataBefore);
            mData2.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            copy = mData2.Copy();
            copyDataBefore = OpenPcmInputStream(copy);
            ms = mData2.AudioDuration.TimeDeltaAsMillisecondDouble;
            mData2.InsertPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"), new Time(ms), null);
            CheckAudioData(copy, copyDataBefore);
            mData3.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            copy = mData3.Copy();
            copyDataBefore = OpenPcmInputStream(copy);
            WavAudioMediaData copy2 = copy.Copy();
            mData3.MergeWith(copy2);
            CheckAudioData(copy, copyDataBefore);
        }

		[Test]
        public void Copy_MultiClips()
        {
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData1.AppendPcmData_RiffHeader(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            WavAudioMediaData copy = mData1.Copy();
            Assert.IsTrue(mData1.ValueEquals(copy));
        }
    }
}
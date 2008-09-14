using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NUnit.Framework;
using urakawa;
using urakawa.core;
using urakawa.property.channel;
using urakawa.media;
using urakawa.media.data.audio;
using urakawa.media.data.audio.codec;
using urakawa.media.data.utilities;
using urakawa.media.timing;

namespace urakawa.media.data.audio.codec
{
    [TestFixture, Description("Tests the WavAudioMediaData functionality")]
    public class WavAudioMediaDataTests
    {
        protected Project mProject;

        protected Presentation mPresentation
        {
            get { return mProject.GetPresentation(0); }
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
            mData1 = mPresentation.MediaDataFactory.CreateWavAudioMediaData();
            Assert.IsTrue(mData1.PCMFormat.BitDepth == 16, "default bit depth should be 16 bits");
            Assert.IsTrue(mData1.PCMFormat.NumberOfChannels == 1, "default number of channels should be 1");
            Assert.IsTrue(mData1.PCMFormat.SampleRate == 44100, "default sample rate should be 44100 Hz");
            mData2 = mPresentation.MediaDataFactory.CreateWavAudioMediaData();
            Assert.IsTrue(mData2.PCMFormat.BitDepth == 16, "default bit depth should be 16 bits");
            Assert.IsTrue(mData2.PCMFormat.NumberOfChannels == 1, "default number of channels should be 1");
            Assert.IsTrue(mData2.PCMFormat.SampleRate == 44100, "default sample rate should be 44100 Hz");
            mData3 = mPresentation.MediaDataFactory.CreateWavAudioMediaData();
            Assert.IsTrue(mData3.PCMFormat.BitDepth == 16, "default bit depth should be 16 bits");
            Assert.IsTrue(mData3.PCMFormat.NumberOfChannels == 1, "default number of channels should be 1");
            Assert.IsTrue(mData3.PCMFormat.SampleRate == 44100, "default sample rate should be 44100 Hz");
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

        private PCMDataInfo GetInfo(string name)
        {
            FileStream fs = new FileStream(GetPath(name), FileMode.Open, FileAccess.Read, FileShare.Read);
            try
            {
                return PCMDataInfo.ParseRiffWaveHeader(fs);
            }
            finally
            {
                fs.Close();
            }
        }

        [Test, Description("Tests the media samples used in this fixture")]
        public void TestMediaSamples()
        {
            PCMDataInfo info;
            List<Stream> wavSeq1 = new List<Stream>();
            List<Stream> wavSeq2 = new List<Stream>();
            info = GetInfo("audiotest1-mono-44100Hz-16bits.wav");
            Assert.IsTrue(info.NumberOfChannels == 1);
            Assert.IsTrue(info.SampleRate == 44100);
            Assert.IsTrue(info.BitDepth == 16);
            Assert.IsTrue(info.Duration.IsEqualTo(new TimeDelta(1500)));
            info = GetInfo("audiotest1-mono-22050Hz-16bits.wav");
            Assert.IsTrue(info.NumberOfChannels == 1);
            Assert.IsTrue(info.SampleRate == 22050);
            Assert.IsTrue(info.BitDepth == 16);
            Assert.IsTrue(info.Duration.IsEqualTo(new TimeDelta(1500)));
            info = GetInfo("audiotest2-mono-44100Hz-16bits.wav");
            Assert.IsTrue(info.NumberOfChannels == 1);
            Assert.IsTrue(info.SampleRate == 44100);
            Assert.IsTrue(info.BitDepth == 16);
            Assert.IsTrue(info.Duration.IsEqualTo(new TimeDelta(1500)));
            info = GetInfo("audiotest3-mono-44100Hz-16bits.wav");
            Assert.IsTrue(info.NumberOfChannels == 1);
            Assert.IsTrue(info.SampleRate == 44100);
            Assert.IsTrue(info.BitDepth == 16);
            Assert.IsTrue(info.Duration.IsEqualTo(new TimeDelta(1500)));
            info = GetInfo("audiotest1+2-mono-44100Hz-16bits.wav");
            Assert.IsTrue(info.NumberOfChannels == 1);
            Assert.IsTrue(info.SampleRate == 44100);
            Assert.IsTrue(info.BitDepth == 16);
            Assert.IsTrue(info.Duration.IsEqualTo(new TimeDelta(3000)));
            info = GetInfo("audiotest1+3-mono-44100Hz-16bits.wav");
            Assert.IsTrue(info.NumberOfChannels == 1);
            Assert.IsTrue(info.SampleRate == 44100);
            Assert.IsTrue(info.BitDepth == 16);
            Assert.IsTrue(info.Duration.IsEqualTo(new TimeDelta(3000)));
            info = GetInfo("audiotest1+2+3-mono-44100Hz-16bits.wav");
            Assert.IsTrue(info.NumberOfChannels == 1);
            Assert.IsTrue(info.SampleRate == 44100);
            Assert.IsTrue(info.BitDepth == 16);
            Assert.IsTrue(info.Duration.IsEqualTo(new TimeDelta(4500)));
            info = GetInfo("audiotest2+1-mono-44100Hz-16bits.wav");
            Assert.IsTrue(info.NumberOfChannels == 1);
            Assert.IsTrue(info.SampleRate == 44100);
            Assert.IsTrue(info.BitDepth == 16);
            Assert.IsTrue(info.Duration.IsEqualTo(new TimeDelta(3000)));
            info = GetInfo("audiotest-stereo-44100Hz-16bits.wav");
            Assert.IsTrue(info.NumberOfChannels == 2);
            Assert.IsTrue(info.SampleRate == 44100);
            Assert.IsTrue(info.BitDepth == 16);
            Assert.IsTrue(info.Duration.IsEqualTo(new TimeDelta(10000)));
            info = GetInfo("omelet_punk.wav");
            Assert.IsTrue(info.NumberOfChannels == 2);
            Assert.IsTrue(info.SampleRate == 44100);
            Assert.IsTrue(info.BitDepth == 16);
            Assert.IsTrue(info.Duration.IsEqualTo(new TimeDelta(TimeSpan.FromTicks(209784807))));
            info = GetInfo("stereo.wav");
            Assert.IsTrue(info.NumberOfChannels == 2);
            Assert.IsTrue(info.SampleRate == 22050);
            Assert.IsTrue(info.BitDepth == 16);
            Assert.IsTrue(info.Duration.IsEqualTo(new TimeDelta(16620.816300000001)));


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


        [Test, Description("Tests value equality with very basic media data")]
        public void ValueEquals_Basics()
        {
            Assert.IsFalse(mData1.ValueEquals(null), "a created media data shouldn't equal null");
            Assert.IsTrue(mData1.ValueEquals(mData1), "an empty media data should equal itself");
            Assert.IsTrue(mData1.ValueEquals(mData2), "two identically created empty media datas should be equal");
            mData1.AppendAudioDataFromRiffWave(Path.Combine(mPresentation.RootUri.LocalPath,
                                                            "audiotest1-mono-44100Hz-16bits.wav"));
            Assert.IsTrue(mData1.ValueEquals(mData1), "a data with content should equal itself");
        }

        [Test, Description("Tests value equality with empty media datas, focusing on the PCM Format")]
        public void ValueEquals_PCMFormat()
        {
            Assert.IsTrue(mData1.PCMFormat.ValueEquals(mData2.PCMFormat), "[Pre-Condition] PCM formats should be equal");
            Assert.IsTrue(mData1.ValueEquals(mData2), "empty media datas with the same PCM format should be equal");
            mData1.NumberOfChannels = 1;
            mData2.NumberOfChannels = 2;
            Assert.IsFalse(mData1.PCMFormat.ValueEquals(mData2.PCMFormat),
                           "[Pre-Condition] PCM formats shouldn't be equal");
            Assert.IsFalse(mData1.ValueEquals(mData2), "empty media datas with different PCM formats shouldn't be equal");
        }

        [Test, Description("Tests that two media datas created from the same unique wav clip are equal")]
        public void ValueEquals_SameAudioDataSingleWavClip()
        {
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData2.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            Assert.IsTrue(mData1.ValueEquals(mData2), "datas with the same wav content should be equal");
        }

        [Test, Description("Tests that two media datas created from the same multiple wav clips are equal")]
        public void ValueEquals_SameAudioDataSameWavClips()
        {
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest2-mono-44100Hz-16bits.wav"));
            mData2.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData2.AppendAudioDataFromRiffWave(GetPath("audiotest2-mono-44100Hz-16bits.wav"));
            Assert.IsTrue(mData1.ValueEquals(mData2), "datas with the same wav content should be equal");
        }

        [Test,
         Description(
             "Tests the equality of two datas, one made of a single wav clip, the other containing several wav clips")]
        public void ValueEquals_SameAudioDataDiffWavClips()
        {
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest1+2-mono-44100Hz-16bits.wav"));
            mData2.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData2.AppendAudioDataFromRiffWave(GetPath("audiotest2-mono-44100Hz-16bits.wav"));
            Assert.IsTrue(mData1.ValueEquals(mData2), "datas with the same wav content should be equal");
        }

        [Test,
         Description(
             "Tests that two audio datas with the same PCM format and bit length but different content are not equal")]
        public void ValueEquals_DiffAudioData()
        {
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData2.AppendAudioDataFromRiffWave(GetPath("audiotest2-mono-44100Hz-16bits.wav"));
            Assert.IsTrue(mData1.AudioDuration.IsEqualTo(mData2.AudioDuration),
                          "[Pre-Condition] audio duration should be equal");
            Assert.IsFalse(mData1.ValueEquals(mData2), "datas created from different wav files shouldn't be equal");
        }

        [Test,
         Description(
             "Tests that two audio datas created from the same audio sample but with different PCM format are not equal"
             )]
        public void ValueEquals_DiffPCMFormat()
        {
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData2.SampleRate = 22050;
            mData2.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-22050Hz-16bits.wav"));
            Assert.IsFalse(mData1.PCMFormat.ValueEquals(mData2.PCMFormat),
                           "[Pre-Condition] PCM formats shouldn't be equal");
            Assert.IsFalse(mData1.ValueEquals(mData2), "datas created from different wav files shouldn't be equal");
        }


        [Test, Description("Tests that trying to insert a data at a negative insertion point raises an exception")]
        [ExpectedException(typeof (exception.MethodParameterIsOutOfBoundsException))]
        public void InsertAudioData_NegativeInsertPoint()
        {
            mData1.InsertAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"), new Time(-1),
                                               new TimeDelta(1));
        }

        [Test,
         Description(
             "Tests that trying to insert a data to an empty audio data object but at a non-zero insertion point raises an exception"
             )]
        [ExpectedException(typeof (exception.MethodParameterIsOutOfBoundsException))]
        public void InsertAudioData_InsertionGreaterThanDurationEmpty()
        {
            mData1.InsertAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"), new Time(10000),
                                               new TimeDelta(1));
        }

        [Test,
         Description(
             "Tests that trying to insert a data at an insertion point greater than the current duration raises an exception"
             )]
        [ExpectedException(typeof (exception.MethodParameterIsOutOfBoundsException))]
        public void InsertAudioData_InsertionGreaterThanDuration()
        {
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData1.InsertAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"), new Time(10000),
                                               new TimeDelta(1));
        }

        [Test, Description("Tests that trying to insert a data with a negative duration raises an exception")]
        [ExpectedException(typeof (exception.MethodParameterIsOutOfBoundsException))]
        public void InsertAudioData_NegativeDuration()
        {
            mData1.InsertAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"), Time.Zero,
                                               new TimeDelta(-1));
        }

        [Test,
         Description(
             "Tests that trying to insert a data with a duration greater than the given data raises an exception")]
        [ExpectedException(typeof (exception.MethodParameterIsOutOfBoundsException))]
        public void InsertAudioData_DurationGreaterThanData()
        {
            mData1.InsertAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"), Time.Zero,
                                               new TimeDelta(10000));
        }

        [Test, Description("Tests inserting audio data at the beginning of some audio data")]
        public void InsertAudioData_AtTheBeginning()
        {
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest1+2-mono-44100Hz-16bits.wav"));
            mData2.AppendAudioDataFromRiffWave(GetPath("audiotest2-mono-44100Hz-16bits.wav"));
            mData2.InsertAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"), Time.Zero,
                                               new TimeDelta(1500));
            Assert.IsTrue(mData1.ValueEquals(mData2), "the two audio datas should be equal");
        }

        [Test, Description("Tests inserting audio data at the end of some audio data")]
        public void InsertAudioData_AtTheEnd()
        {
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest1+2-mono-44100Hz-16bits.wav"));
            mData2.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData2.InsertAudioDataFromRiffWave(GetPath("audiotest2-mono-44100Hz-16bits.wav"), new Time(1500),
                                               new TimeDelta(1500));
            Assert.IsTrue(mData1.ValueEquals(mData2), "the two audio datas should be equal");
        }

        [Test, Description("Tests inserting audio data between two wav clips")]
        public void InsertAudioData_BetweenTwoWavClips()
        {
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest1+2+3-mono-44100Hz-16bits.wav"));
            mData2.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData2.AppendAudioDataFromRiffWave(GetPath("audiotest3-mono-44100Hz-16bits.wav"));
            mData2.InsertAudioDataFromRiffWave(GetPath("audiotest2-mono-44100Hz-16bits.wav"), new Time(1500),
                                               new TimeDelta(1500));
            Assert.IsTrue(mData1.ValueEquals(mData2), "the two audio datas should be equal");
        }

        [Test, Description("Tests inserting audio data in the middle of a wav clip")]
        public void InsertAudioData_InAWavClip()
        {
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest1+2+3-mono-44100Hz-16bits.wav"));
            mData2.AppendAudioDataFromRiffWave(GetPath("audiotest1+3-mono-44100Hz-16bits.wav"));
            mData2.InsertAudioDataFromRiffWave(GetPath("audiotest2-mono-44100Hz-16bits.wav"), new Time(1500),
                                               new TimeDelta(1500));
            Assert.IsTrue(mData1.ValueEquals(mData2), "the two audio datas should be equal");
        }


        [Test, Description("tests the equality of to audio data created by appending the same wav sample to empty data")
        ]
        public void AppendAudioData_ToEmptyData()
        {
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData2.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            Assert.IsTrue(mData1.ValueEquals(mData2), "the two audio datas should be equal");
        }

        [Test,
         Description(
             "tests the equality of to audio data created by appending the same wav sample to empty data - stereo version"
             )]
        public void AppendAudioData_Stereo_ToEmptyData()
        {
            mData1.NumberOfChannels = 2;
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest-stereo-44100Hz-16bits.wav"));
            mData2.NumberOfChannels = 2;
            mData2.AppendAudioDataFromRiffWave(GetPath("audiotest-stereo-44100Hz-16bits.wav"));
            Assert.IsTrue(mData1.ValueEquals(mData2), "the two audio datas should be equal");
            mData1.RemoveAudioData(Time.Zero);
            mData2.RemoveAudioData(Time.Zero);
            mData1.AppendAudioDataFromRiffWave(GetPath("omelet_punk.wav"));
            mData2.AppendAudioDataFromRiffWave(GetPath("omelet_punk.wav"));
            Assert.IsTrue(mData1.ValueEquals(mData2), "the two audio datas should be equal");
            mData1.RemoveAudioData(Time.Zero);
            mData2.RemoveAudioData(Time.Zero);
            mData1.SampleRate = 22050;
            mData1.AppendAudioDataFromRiffWave(GetPath("stereo.wav"));
            mData2.SampleRate = 22050;
            mData2.AppendAudioDataFromRiffWave(GetPath("stereo.wav"));
            Assert.IsTrue(mData1.ValueEquals(mData2), "the two audio datas should be equal");
        }


        [Test, Description("Get exception appending sterio audio to a mono audio media data")]
        [ExpectedException(typeof (urakawa.exception.InvalidDataFormatException))]
        public void AppendAudioData_StereoFileToMonoAudioMediaData()
        {
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest-stereo-44100Hz-16bits.wav"));
        }

        [Test,
         Description(
             "tests that an audio data created by appending one wav clip to another is equal to an audio data containing a single wav clip which is the concatenation of the two others"
             )]
        public void AppendAudioData_ToEqualSingleWavClip()
        {
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest1+2-mono-44100Hz-16bits.wav"));
            mData2.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData2.AppendAudioDataFromRiffWave(GetPath("audiotest2-mono-44100Hz-16bits.wav"));
            Assert.IsTrue(mData1.ValueEquals(mData2), "the two audio datas should be equal");
        }

        [Test, Description("tests that two audio data created by appending multiple wav clips are equal")]
        public void AppendAudioData_ToEqualMultipleWavClips()
        {
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest2-mono-44100Hz-16bits.wav"));
            mData2.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData2.AppendAudioDataFromRiffWave(GetPath("audiotest2-mono-44100Hz-16bits.wav"));
            Assert.IsTrue(mData1.ValueEquals(mData2), "the two audio datas should be equal");
        }


        [Test, Description("Tests that trying to replace a data at a negative insertion point raises an exception")]
        [ExpectedException(typeof (exception.MethodParameterIsOutOfBoundsException))]
        public void ReplaceAudioData_NegativeInsertPoint()
        {
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData1.ReplaceAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"), new Time(-1),
                                                new TimeDelta(1));
        }

        [Test,
         Description(
             "Tests that trying to replace a data to an empty audio data object but at a non-zero insertion point raises an exception"
             )]
        [ExpectedException(typeof (exception.MethodParameterIsOutOfBoundsException))]
        public void ReplaceAudioData_insertionGreaterThanDurationEmpty()
        {
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData1.ReplaceAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"), new Time(10000),
                                                new TimeDelta(1));
        }

        [Test,
         Description(
             "Tests that trying to replace a data at an insertion point greater than the current duration raises an exception"
             )]
        [ExpectedException(typeof (exception.MethodParameterIsOutOfBoundsException))]
        public void ReplaceAudioData_insertionGreaterThanDuration()
        {
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData1.ReplaceAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"), new Time(10000),
                                                new TimeDelta(1));
        }

        [Test, Description("Tests that trying to replace a data with a negative duration raises an exception")]
        [ExpectedException(typeof (exception.MethodParameterIsOutOfBoundsException))]
        public void ReplaceAudioData_NegativeDuration()
        {
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData1.ReplaceAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"), Time.Zero,
                                                new TimeDelta(-1));
        }

        [Test,
         Description(
             "Tests that trying to replace a data with a duration greater than the given data raises an exception")]
        [ExpectedException(typeof (exception.MethodParameterIsOutOfBoundsException))]
        public void ReplaceAudioData_DurationGreaterThanData()
        {
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData1.ReplaceAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"), Time.Zero,
                                                new TimeDelta(10000));
        }

        [Test,
         Description(
             "Tests that trying to replace a data with a duration greater than the remaining original data to replace raises an exception"
             )]
        [ExpectedException(typeof (exception.MethodParameterIsOutOfBoundsException))]
        public void ReplaceAudioData_DurationGreaterThanRemainingDataToReplace()
        {
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData1.ReplaceAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"), new Time(10),
                                                new TimeDelta(1500));
        }

        [Test, Description("Tests replacing an audio data at the beginning of a 1-clip audio data")]
        public void ReplaceAudioData_AtTheBeginning()
        {
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest1+2+3-mono-44100Hz-16bits.wav"));
            mData2.AppendAudioDataFromRiffWave(GetPath("audiotest1+2+3-mono-44100Hz-16bits.wav"));
            mData2.ReplaceAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"), Time.Zero,
                                                new TimeDelta(1500));
            Assert.IsTrue(mData1.ValueEquals(mData2), "the two audio datas should be equal");
        }

        [Test, Description("Tests replacing an audio data at the end of a 1-clip audio data")]
        public void ReplaceAudioData_AtTheEnd()
        {
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest1+2+3-mono-44100Hz-16bits.wav"));
            mData2.AppendAudioDataFromRiffWave(GetPath("audiotest1+2+3-mono-44100Hz-16bits.wav"));
            mData2.ReplaceAudioDataFromRiffWave(GetPath("audiotest3-mono-44100Hz-16bits.wav"), new Time(3000),
                                                new TimeDelta(1500));
            Assert.IsTrue(mData1.ValueEquals(mData2), "the two audio datas should be equal");
        }

        [Test, Description("Tests replacing an audio data stricly included in a wav clip")]
        public void ReplaceAudioData_SmallerThanAWavClip()
        {
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest1+2+3-mono-44100Hz-16bits.wav"));
            mData2.AppendAudioDataFromRiffWave(GetPath("audiotest1+2+3-mono-44100Hz-16bits.wav"));
            mData2.ReplaceAudioDataFromRiffWave(GetPath("audiotest2-mono-44100Hz-16bits.wav"), new Time(1500),
                                                new TimeDelta(1500));
            Assert.IsTrue(mData1.ValueEquals(mData2), "the two audio datas should be equal");
        }

        [Test, Description("Tests replacing an audio data matching exactly a wav clip")]
        public void ReplaceAudioData_ExactlyAWavClip()
        {
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest1+2+3-mono-44100Hz-16bits.wav"));
            mData2.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData2.AppendAudioDataFromRiffWave(GetPath("audiotest2-mono-44100Hz-16bits.wav"));
            mData2.AppendAudioDataFromRiffWave(GetPath("audiotest3-mono-44100Hz-16bits.wav"));
            mData2.ReplaceAudioDataFromRiffWave(GetPath("audiotest2-mono-44100Hz-16bits.wav"), new Time(1500),
                                                new TimeDelta(1500));
            Assert.IsTrue(mData1.ValueEquals(mData2), "the two audio datas should be equal");
        }

        [Test, Description("Tests replacing an audio data spanning across two wav clips")]
        public void ReplaceAudioData_SpanningTwoWavClips()
        {
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest1+2-mono-44100Hz-16bits.wav"));
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest1+2-mono-44100Hz-16bits.wav"));
            mData2.AppendAudioDataFromRiffWave(GetPath("audiotest1+2-mono-44100Hz-16bits.wav"));
            mData2.AppendAudioDataFromRiffWave(GetPath("audiotest1+2-mono-44100Hz-16bits.wav"));
            mData2.ReplaceAudioDataFromRiffWave(GetPath("audiotest2+1-mono-44100Hz-16bits.wav"), new Time(1500),
                                                new TimeDelta(3000));
            Assert.IsTrue(mData1.ValueEquals(mData2), "the two audio datas should be equal");
        }


        [Test, Description("Tests that trying to remove an audio data with a negative clip begin raises an exception")]
        [ExpectedException(typeof (exception.MethodParameterIsOutOfBoundsException))]
        public void RemoveAudioData_ClipBeginNegative()
        {
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData1.RemoveAudioData(new Time(-1));
        }

        [Test,
         Description(
             "Tests that trying to remove an audio data with a clip begin greater than the audio duration raises an exception"
             )]
        [ExpectedException(typeof (exception.MethodParameterIsOutOfBoundsException))]
        public void RemoveAudioData_ClipBeginGreaterThanDuration()
        {
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData1.RemoveAudioData(new Time(10000));
        }

        [Test,
         Description(
             "Tests that trying to remove an audio data with a clip begin greater than the clip end raises an exception"
             )]
        [ExpectedException(typeof (exception.MethodParameterIsOutOfBoundsException))]
        public void RemoveAudioData_ClipBeginGreaterThanClipEnd()
        {
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData1.RemoveAudioData(new Time(2), new Time(1));
        }

        [Test, Description("Tests that trying to remove an audio data with a negative clip end raises an exception")]
        [ExpectedException(typeof (exception.MethodParameterIsOutOfBoundsException))]
        public void RemoveAudioData_ClipEndNegative()
        {
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData1.RemoveAudioData(new Time(1), new Time(-1));
        }

        [Test,
         Description(
             "Tests that trying to remove an audio data with a clip end greater than the audio duration raises an exception"
             )]
        [ExpectedException(typeof (exception.MethodParameterIsOutOfBoundsException))]
        public void RemoveAudioData_ClipEndGreaterThanDuration()
        {
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData1.RemoveAudioData(new Time(1), new Time(10000));
        }

        [Test, Description("Tests that removing an empty clip of data does nothing")]
        public void RemoveAudioData_ClipBeginEqualClipEnd()
        {
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData2.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData2.RemoveAudioData(new Time(1000), new Time(1000));
            Assert.IsTrue(mData1.ValueEquals(mData2), "the two audio datas should be equal");
        }

        [Test, Description("Tests removing the beginning of a 1-clip audio data")]
        public void RemoveAudioData_FromTheBeginning()
        {
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData2.AppendAudioDataFromRiffWave(GetPath("audiotest2+1-mono-44100Hz-16bits.wav"));
            mData2.RemoveAudioData(Time.Zero, new Time(1500));
            Assert.IsTrue(mData1.ValueEquals(mData2), "the two audio datas should be equal");
        }

        [Test, Description("Tests removing the end of a 1-clip audio data")]
        public void RemoveAudioData_AtTheEnd()
        {
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData2.AppendAudioDataFromRiffWave(GetPath("audiotest1+2-mono-44100Hz-16bits.wav"));
            mData2.RemoveAudioData(new Time(1500), new Time(3000));
            Assert.IsTrue(mData1.ValueEquals(mData2), "the two audio datas should be equal");
        }

        [Test, Description("Tests removing all the content of 1-clip audio data")]
        public void RemoveAudioData_RemoveAllSingleClip()
        {
            mData2.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData2.RemoveAudioData(Time.Zero, new Time(1500));
            Assert.IsTrue(mData1.ValueEquals(mData2), "the two audio datas should be equal");
        }

        [Test, Description("Tests removing all the content of multi clip audio data")]
        public void RemoveAudioData_RemoveAllMultiClip()
        {
            mData2.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData2.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData2.RemoveAudioData(Time.Zero, new Time(3000));
            Assert.IsTrue(mData1.ValueEquals(mData2), "the two audio datas should be equal");
        }

        [Test, Description("Tests that the one-arg method behaves the same as the two-arg method")]
        public void RemoveAudioData_OneArg()
        {
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData2.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData1.RemoveAudioData(new Time(1000));
            mData2.RemoveAudioData(new Time(1000), new Time(1500));
            Assert.IsTrue(mData1.ValueEquals(mData2), "the two audio datas should be equal");
        }

        [Test, Description("Tests removing audio data strictly included in a wav clip")]
        public void RemoveAudioData_IncludedInAWavClip()
        {
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest1+3-mono-44100Hz-16bits.wav"));
            mData2.AppendAudioDataFromRiffWave(GetPath("audiotest1+2+3-mono-44100Hz-16bits.wav"));
            mData2.RemoveAudioData(new Time(1500), new Time(3000));
            Assert.IsTrue(mData1.ValueEquals(mData2), "the two audio datas should be equal");
        }

        [Test, Description("Tests removing audio data matching exaclty one wav clip")]
        public void RemoveAudioData_ExactlyAWavClip()
        {
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest1+3-mono-44100Hz-16bits.wav"));
            mData2.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData2.AppendAudioDataFromRiffWave(GetPath("audiotest2-mono-44100Hz-16bits.wav"));
            mData2.AppendAudioDataFromRiffWave(GetPath("audiotest3-mono-44100Hz-16bits.wav"));
            mData2.RemoveAudioData(new Time(1500), new Time(3000));
            Assert.IsTrue(mData1.ValueEquals(mData2), "the two audio datas should be equal");
        }

        [Test, Description("Tests removing audio data spanning across two wav clips")]
        public void RemoveAudioData_SpanningTwoWavClips()
        {
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest1+2-mono-44100Hz-16bits.wav"));
            mData2.AppendAudioDataFromRiffWave(GetPath("audiotest1+2-mono-44100Hz-16bits.wav"));
            mData2.AppendAudioDataFromRiffWave(GetPath("audiotest1+2-mono-44100Hz-16bits.wav"));
            mData2.RemoveAudioData(new Time(1500), new Time(4500));
            Assert.IsTrue(mData1.ValueEquals(mData2), "the two audio datas should be equal");
        }


        [Test, Description("Tests that trying to get an audio data with a negative clip begin raises an exception")]
        [ExpectedException(typeof (exception.MethodParameterIsOutOfBoundsException))]
        public void GetAudioData_ClipBeginNegative()
        {
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData1.GetAudioData(new Time(-1));
        }

        [Test,
         Description(
             "Tests that trying to get an audio data with a clip begin greater than the audio duration raises an exception"
             )]
        [ExpectedException(typeof (exception.MethodParameterIsOutOfBoundsException))]
        public void GetAudioData_ClipBeginGreaterThanDuration()
        {
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData1.GetAudioData(new Time(10000));
        }

        [Test,
         Description(
             "Tests that trying to get an audio data with a clip begin greater than the clip end raises an exception")]
        [ExpectedException(typeof (exception.MethodParameterIsOutOfBoundsException))]
        public void GetAudioData_ClipBeginGreaterThanClipEnd()
        {
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData1.GetAudioData(new Time(2), new Time(1)).Close();
        }

        [Test, Description("Tests that trying to get an audio data with a negative clip end raises an exception")]
        [ExpectedException(typeof (exception.MethodParameterIsOutOfBoundsException))]
        public void GetAudioData_ClipEndNegative()
        {
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData1.GetAudioData(new Time(1), new Time(-1)).Close();
        }

        [Test,
         Description(
             "Tests that trying to get an audio data with a clip end greater than the audio duration raises an exception"
             )]
        [ExpectedException(typeof (exception.MethodParameterIsOutOfBoundsException))]
        public void GetAudioData_ClipEndGreaterThanDuration()
        {
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData1.GetAudioData(Time.Zero, new Time(10000)).Close();
        }

        [Test, Description("Tests that getting an empty audio data returns en empty stream")]
        public void GetAudioData_ClipBeginEqualClipEnd()
        {
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            Stream s = mData1.GetAudioData(new Time(1000), new Time(1000));
            try
            {
                Assert.IsTrue(s.Length == 0, "the returned audio data should be empty");
            }
            finally
            {
                s.Close();
            }
        }

        [Test, Description("Tests that getting the content of an empty data returns an empty stream")]
        public void GetAudioData_EmptyData()
        {
            Stream s = mData1.GetAudioData();
            try
            {
                Assert.IsTrue(s.Length == 0, "the returned audio data should be empty");
            }
            finally
            {
                s.Close();
            }
        }

        [Test, Description("Tests getting the beginning of a 1-clip audio data - stereo")]
        public void GetAudioData_Stereo_FromTheBeginning()
        {
            mData1.NumberOfChannels = 2;
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest-stereo-44100Hz-16bits.wav"));
            Stream s1 = mData1.GetAudioData(Time.Zero, new Time(10000));
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


        [Test, Description("Tests getting the beginning of a 1-clip audio data")]
        public void GetAudioData_FromTheBeginning()
        {
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest1+2-mono-44100Hz-16bits.wav"));
            Stream s1 = mData1.GetAudioData(Time.Zero, new Time(1500));
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

        [Test, Description("Tests getting the end of a 1-clip audio data")]
        public void GetAudioData_AtTheEnd()
        {
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest1+2-mono-44100Hz-16bits.wav"));
            Stream s1 = mData1.GetAudioData(new Time(1500), new Time(3000));
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

        [Test, Description("Tests getting all the content of 1-clip audio data")]
        public void GetAudioData_getAllSingleClip()
        {
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            Stream s1 = mData1.GetAudioData();
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

        [Test, Description("Tests removing all the content of multi clip audio data")]
        public void GetAudioData_getAllMultiClip()
        {
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest2-mono-44100Hz-16bits.wav"));
            Stream s1 = mData1.GetAudioData();
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

        [Test, Description("Tests that the one-arg method behaves the same as the two-arg method")]
        public void GetAudioData_OneArg()
        {
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            Stream s1 = mData1.GetAudioData(new Time(1000), new Time(1500));
            try
            {
                Stream s2 = mData1.GetAudioData(new Time(1000));
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

        [Test, Description("Tests that the 0-arg method behaves the same as the two-arg method")]
        public void GetAudioData_ZeroArg()
        {
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            Stream s1 = mData1.GetAudioData(Time.Zero, new Time(1500));
            try
            {
                Stream s2 = mData1.GetAudioData();
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

        [Test, Description("Tests getting audio data strictly included in a wav clip")]
        public void GetAudioData_IncludedInAWavClip()
        {
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest1+2+3-mono-44100Hz-16bits.wav"));
            Stream s1 = mData1.GetAudioData(new Time(1500), new Time(3000));
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

        [Test, Description("Tests getting audio data matching exaclty one wav clip")]
        public void GetAudioData_ExactlyAWavClip()
        {
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest2-mono-44100Hz-16bits.wav"));
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest3-mono-44100Hz-16bits.wav"));
            Stream s1 = mData1.GetAudioData(new Time(1500), new Time(3000));
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

        [Test, Description("Tests getting audio data spanning across two wav clips")]
        public void GetAudioData_SpanningTwoWavClips()
        {
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest1+2-mono-44100Hz-16bits.wav"));
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest1+2-mono-44100Hz-16bits.wav"));
            Stream s1 = mData1.GetAudioData(new Time(1500), new Time(4500));
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


        [Test, Description("Tests that trying to merge audio data with incompatible PCM format raises an exception")]
        [ExpectedException(typeof (exception.InvalidDataFormatException))]
        public void MergeWith_IncompatiblePCMFormat()
        {
            mData1.NumberOfChannels = 2;
            mData1.MergeWith(mData2);
        }

        [Test, Description("Tests that merging with an empty audio data doesn't change anything")]
        public void MergeWith_EmptyData()
        {
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData2.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData2.MergeWith(mData3);
            Assert.IsTrue(mData3.GetAudioData().Length == 0);
            Assert.IsTrue(mData1.ValueEquals(mData2), "the two audio datas should be equal");
        }

        [Test, Description("Tests merging audio data to an empty audio data")]
        public void MergeWith_EmptyCaller()
        {
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData3.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData2.MergeWith(mData3);
            Assert.IsTrue(mData3.GetAudioData().Length == 0);
            Assert.IsTrue(mData1.ValueEquals(mData2), "the two audio datas should be equal");
        }

        [Test, Description("Tests merging with a 1-clip audio data")]
        public void MergeWith_OneWavClip()
        {
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest1+2-mono-44100Hz-16bits.wav"));
            mData2.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData3.AppendAudioDataFromRiffWave(GetPath("audiotest2-mono-44100Hz-16bits.wav"));
            mData2.MergeWith(mData3);
            Assert.IsTrue(mData3.GetAudioData().Length == 0);
            Assert.IsTrue(mData1.ValueEquals(mData2), "the two audio datas should be equal");
        }

        [Test, Description("Tests merging with a multi-clips audio data")]
        public void MergeWith_MultiWavClips()
        {
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest1+2+3-mono-44100Hz-16bits.wav"));
            mData2.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData3.AppendAudioDataFromRiffWave(GetPath("audiotest2-mono-44100Hz-16bits.wav"));
            mData3.AppendAudioDataFromRiffWave(GetPath("audiotest3-mono-44100Hz-16bits.wav"));
            mData2.MergeWith(mData3);
            Assert.IsTrue(mData3.GetAudioData().Length == 0);
            Assert.IsTrue(mData1.ValueEquals(mData2), "the two audio datas should be equal");
        }

        [Test, Description("Tests merging to a multi-clips audio data")]
        public void MergeWith_MultiWavClipsInCaller()
        {
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest1+2+3-mono-44100Hz-16bits.wav"));
            mData2.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData2.AppendAudioDataFromRiffWave(GetPath("audiotest2-mono-44100Hz-16bits.wav"));
            mData3.AppendAudioDataFromRiffWave(GetPath("audiotest3-mono-44100Hz-16bits.wav"));
            mData2.MergeWith(mData3);
            Assert.IsTrue(mData3.GetAudioData().Length == 0);
            Assert.IsTrue(mData1.ValueEquals(mData2), "the two audio datas should be equal");
        }


        [Test, Description("Tests that trying to split an audio data at a negative point raises an exception")]
        [ExpectedException(typeof (exception.MethodParameterIsOutOfBoundsException))]
        public void Split_OutOfBoundNegative()
        {
            mData1.Split(new Time(-1));
        }

        [Test, Description("Tests that tryng to split an audio beyond its duration raises an exception")]
        [ExpectedException(typeof (exception.MethodParameterIsOutOfBoundsException))]
        public void Split_OutOfBoundBeyond()
        {
            mData1.Split(new Time(1));
        }

        [Test, Description("Tests splitting an empty audio data")]
        public void Split_EmptyData()
        {
            AudioMediaData split = mData1.Split(Time.Zero);
            Assert.IsTrue(split.ValueEquals(mData2), "the split data should be empty");
            Assert.IsTrue(mData1.ValueEquals(mData3), "the remaining data should be empty");
        }

        [Test, Description("Tests splitting an audio data at the beginning")]
        public void Split_AtTheBeginning()
        {
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            AudioMediaData split = mData1.Split(Time.Zero);
            mData2.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            Assert.IsTrue(split.ValueEquals(mData2), "the split data is not what was expected");
            Assert.IsTrue(mData1.ValueEquals(mData3), "the remaining data should be empty");
        }

        [Test, Description("Tests splitting an audio data at the end")]
        public void Split_AtTheEnd()
        {
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            AudioMediaData split = mData1.Split(new Time(1500));
            mData3.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            Assert.IsTrue(split.ValueEquals(mData2), "the split data should be empty");
            Assert.IsTrue(mData1.ValueEquals(mData3), "the remaining data is not what was expecte");
        }

        [Test, Description("Tests splitting an audio at a point within a wav clip in a 1-clip audio data")]
        public void Split_InsideAWavClip()
        {
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest1+2-mono-44100Hz-16bits.wav"));
            AudioMediaData split = mData1.Split(new Time(1500));
            mData2.AppendAudioDataFromRiffWave(GetPath("audiotest2-mono-44100Hz-16bits.wav"));
            mData3.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            Assert.IsTrue(split.ValueEquals(mData2), "the split data is not what was expected");
            Assert.IsTrue(mData1.ValueEquals(mData3), "the remaining data is not what was expected");
        }

        [Test, Description("Tests splitting an audio at a point within a wav clip in a multi-clips audio data")]
        public void Split_InsideAWavClipInMultiClipData()
        {
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest2+1-mono-44100Hz-16bits.wav"));
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest2-mono-44100Hz-16bits.wav"));
            AudioMediaData split = mData1.Split(new Time(3000));
            mData2.AppendAudioDataFromRiffWave(GetPath("audiotest1+2-mono-44100Hz-16bits.wav"));
            mData3.AppendAudioDataFromRiffWave(GetPath("audiotest1+2-mono-44100Hz-16bits.wav"));
            Assert.IsTrue(split.ValueEquals(mData2), "the split data is not what was expected");
            Assert.IsTrue(mData1.ValueEquals(mData3), "the remaining data is not what was expected");
        }

        [Test, Description("Tests splitting an audio at a point between two wav clips")]
        public void Split_BetweenWavClips()
        {
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest2-mono-44100Hz-16bits.wav"));
            AudioMediaData split = mData1.Split(new Time(1500));
            mData2.AppendAudioDataFromRiffWave(GetPath("audiotest2-mono-44100Hz-16bits.wav"));
            mData3.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            Assert.IsTrue(split.ValueEquals(mData2), "the split data is not what was expected");
            Assert.IsTrue(mData1.ValueEquals(mData3), "the remaining data is not what was expected");
        }


        [Test, Description("Tests copying an empty audio data")]
        public void Copy_Empty()
        {
            WavAudioMediaData copy = mData1.Copy();
            Assert.IsTrue(mData1.ValueEquals(copy));
            //List<DataProvider> provList = mData1.getListOfUsedDataProviders();
            //List<DataProvider> provListCopy = copy.getListOfUsedDataProviders();
            //Assert.IsTrue(provList.Count == provListCopy.Count,"the copy does not have the same number of data providers");
            //for (int i = 0; i < provList.Count; i++)
            //{
            //    Assert.IsTrue(provList[i].ValueEquals(provListCopy[i]),"a data provider of the copy was not equal to the data provider of the original data");
            //}
        }

        [Test, Description("Tests copying a 1-clip audio data")]
        public void Copy_OneClip()
        {
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            WavAudioMediaData copy = mData1.Copy();
            Assert.IsTrue(mData1.ValueEquals(copy));
            //List<DataProvider> provList = mData1.getListOfUsedDataProviders();
            //List<DataProvider> provListCopy = copy.getListOfUsedDataProviders();
            //Assert.IsTrue(provList.Count == provListCopy.Count, "the copy does not have the same number of data providers");
            //for (int i = 0; i < provList.Count; i++)
            //{
            //    Assert.IsTrue(provList[i].ValueEquals(provListCopy[i]), "a data provider of the copy was not equal to the data provider of the original data");
            //}
        }

        [Test, Description("Tests copying a multi-clips audio data")]
        public void Copy_MultiClips()
        {
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            mData1.AppendAudioDataFromRiffWave(GetPath("audiotest1-mono-44100Hz-16bits.wav"));
            WavAudioMediaData copy = mData1.Copy();
            Assert.IsTrue(mData1.ValueEquals(copy));
            //List<DataProvider> provList = mData1.getListOfUsedDataProviders();
            //List<DataProvider> provListCopy = copy.getListOfUsedDataProviders();
            //Assert.IsTrue(provList.Count == provListCopy.Count, "the copy does not have the same number of data providers");
            //for (int i = 0; i < provList.Count; i++)
            //{
            //    Assert.IsTrue(provList[i].ValueEquals(provListCopy[i]), "a data provider of the copy was not equal to the data provider of the original data");
            //}
        }
    }
}
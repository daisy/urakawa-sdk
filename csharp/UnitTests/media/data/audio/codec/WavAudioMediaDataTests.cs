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
			get { return mProject.getPresentation(0); }
		}
		protected WavAudioMediaData mData1;
		protected WavAudioMediaData mData2;
		protected WavAudioMediaData mData3;

		[TestFixtureSetUp]
		public void setUpFixture()
		{
			Uri projectDir = new Uri(ProjectTests.SampleXukFileDirectoryUri, "MediaTestsSample/");
			if (Directory.Exists(Path.Combine(projectDir.LocalPath, "Data")))
			{
				Directory.Delete(Path.Combine(projectDir.LocalPath, "Data"), true);
			}
			mProject = new Project();
			mProject.addNewPresentation().setRootUri(projectDir);
		}

		[TestFixtureTearDown]
		public void tearDownFixture()
		{
			Uri projectDir = new Uri(ProjectTests.SampleXukFileDirectoryUri, "MediaTestsSample/");
			if (Directory.Exists(Path.Combine(projectDir.LocalPath, "Data")))
			{
				Directory.Delete(Path.Combine(projectDir.LocalPath, "Data"), true);
			}
		}

		[SetUp]
		public void setUp()
		{
			mData1 = mPresentation.getMediaDataFactory().createMediaData("WavAudioMediaData", urakawa.ToolkitSettings.XUK_NS) as WavAudioMediaData;
			Assert.IsNotNull(mData1, "Could not create a WavAudioMediaData");
			Assert.IsTrue(mData1.getPCMFormat().getBitDepth() == 16, "default bit depth should be 16 bits");
			Assert.IsTrue(mData1.getPCMFormat().getNumberOfChannels() == 1, "default number of channels should be 1");
			Assert.IsTrue(mData1.getPCMFormat().getSampleRate() == 44100, "default sample rate should be 44100 Hz");
			mData2 = mPresentation.getMediaDataFactory().createMediaData("WavAudioMediaData", urakawa.ToolkitSettings.XUK_NS) as WavAudioMediaData;
			Assert.IsNotNull(mData2, "Could not create a WavAudioMediaData");
			Assert.IsTrue(mData2.getPCMFormat().getBitDepth() == 16, "default bit depth should be 16 bits");
			Assert.IsTrue(mData2.getPCMFormat().getNumberOfChannels() == 1, "default number of channels should be 1");
			Assert.IsTrue(mData2.getPCMFormat().getSampleRate() == 44100, "default sample rate should be 44100 Hz");
			mData3 = mPresentation.getMediaDataFactory().createMediaData("WavAudioMediaData", urakawa.ToolkitSettings.XUK_NS) as WavAudioMediaData;
			Assert.IsNotNull(mData3, "Could not create a WavAudioMediaData");
			Assert.IsTrue(mData3.getPCMFormat().getBitDepth() == 16, "default bit depth should be 16 bits");
			Assert.IsTrue(mData3.getPCMFormat().getNumberOfChannels() == 1, "default number of channels should be 1");
			Assert.IsTrue(mData3.getPCMFormat().getSampleRate() == 44100, "default sample rate should be 44100 Hz");
		}

		[TearDown]
		public void tearDown()
		{
			mData1.delete();
			mData2.delete();
			mData3.delete();
		}
		private string getPath(String fileName)
		{
			return Path.Combine(mPresentation.getRootUri().LocalPath, fileName);
		}

		private Stream getRawStream(String fileName)
		{
			Stream s = new FileStream(getPath(fileName), FileMode.Open, FileAccess.Read, FileShare.Read);
			s.Seek(44, SeekOrigin.Begin);
			return s;
		}


		[Test, Description("Tests the media samples used in this fixture")]
		public void testMediaSamples()
		{
			PCMDataInfo info;
			List<Stream> wavSeq1 = new List<Stream>();
			List<Stream> wavSeq2 = new List<Stream>();
			info = PCMDataInfo.parseRiffWaveHeader(new FileStream(getPath("audiotest1-mono-44100Hz-16bits.wav"), FileMode.Open, FileAccess.Read, FileShare.Read));
			Assert.IsTrue(info.getNumberOfChannels() == 1);
			Assert.IsTrue(info.getSampleRate() == 44100);
			Assert.IsTrue(info.getBitDepth() == 16);
			Assert.IsTrue(info.getDuration().isEqualTo(new TimeDelta(1500)));
			info = PCMDataInfo.parseRiffWaveHeader(new FileStream(getPath("audiotest1-mono-22050Hz-16bits.wav"), FileMode.Open, FileAccess.Read, FileShare.Read));
			Assert.IsTrue(info.getNumberOfChannels() == 1);
			Assert.IsTrue(info.getSampleRate() == 22050);
			Assert.IsTrue(info.getBitDepth() == 16);
			Console.WriteLine(info.getDuration().getTimeDeltaAsMillisecondFloat());
			Assert.IsTrue(info.getDuration().isEqualTo(new TimeDelta(1500)));
			info = PCMDataInfo.parseRiffWaveHeader(new FileStream(getPath("audiotest2-mono-44100Hz-16bits.wav"), FileMode.Open, FileAccess.Read, FileShare.Read));
			Assert.IsTrue(info.getNumberOfChannels() == 1);
			Assert.IsTrue(info.getSampleRate() == 44100);
			Assert.IsTrue(info.getBitDepth() == 16);
			Assert.IsTrue(info.getDuration().isEqualTo(new TimeDelta(1500)));
			info = PCMDataInfo.parseRiffWaveHeader(new FileStream(getPath("audiotest3-mono-44100Hz-16bits.wav"), FileMode.Open, FileAccess.Read, FileShare.Read));
			Assert.IsTrue(info.getNumberOfChannels() == 1);
			Assert.IsTrue(info.getSampleRate() == 44100);
			Assert.IsTrue(info.getBitDepth() == 16);
			Assert.IsTrue(info.getDuration().isEqualTo(new TimeDelta(1500)));
			info = PCMDataInfo.parseRiffWaveHeader(new FileStream(getPath("audiotest1+2-mono-44100Hz-16bits.wav"), FileMode.Open, FileAccess.Read, FileShare.Read));
			Assert.IsTrue(info.getNumberOfChannels() == 1);
			Assert.IsTrue(info.getSampleRate() == 44100);
			Assert.IsTrue(info.getBitDepth() == 16);
			Assert.IsTrue(info.getDuration().isEqualTo(new TimeDelta(3000)));
			info = PCMDataInfo.parseRiffWaveHeader(new FileStream(getPath("audiotest1+3-mono-44100Hz-16bits.wav"), FileMode.Open, FileAccess.Read, FileShare.Read));
			Assert.IsTrue(info.getNumberOfChannels() == 1);
			Assert.IsTrue(info.getSampleRate() == 44100);
			Assert.IsTrue(info.getBitDepth() == 16);
			Assert.IsTrue(info.getDuration().isEqualTo(new TimeDelta(3000)));
			info = PCMDataInfo.parseRiffWaveHeader(new FileStream(getPath("audiotest1+2+3-mono-44100Hz-16bits.wav"), FileMode.Open, FileAccess.Read, FileShare.Read));
			Assert.IsTrue(info.getNumberOfChannels() == 1);
			Assert.IsTrue(info.getSampleRate() == 44100);
			Assert.IsTrue(info.getBitDepth() == 16);
			Assert.IsTrue(info.getDuration().isEqualTo(new TimeDelta(4500)));
			// tests 1+2
			wavSeq1.Clear();
			wavSeq1.Add(new FileStream(getPath("audiotest1+2-mono-44100Hz-16bits.wav"), FileMode.Open, FileAccess.Read, FileShare.Read));
			wavSeq2.Clear();
			wavSeq2.Add(new FileStream(getPath("audiotest1-mono-44100Hz-16bits.wav"), FileMode.Open, FileAccess.Read, FileShare.Read));
			wavSeq2.Add(new FileStream(getPath("audiotest2-mono-44100Hz-16bits.wav"), FileMode.Open, FileAccess.Read, FileShare.Read));
			Assert.IsTrue(StreamUtils.compareWavSeq(wavSeq1, wavSeq2));
			// tests 1+3
			wavSeq1.Clear();
			wavSeq1.Add(new FileStream(getPath("audiotest1+3-mono-44100Hz-16bits.wav"), FileMode.Open, FileAccess.Read, FileShare.Read));
			wavSeq2.Clear();
			wavSeq2.Add(new FileStream(getPath("audiotest1-mono-44100Hz-16bits.wav"), FileMode.Open, FileAccess.Read, FileShare.Read));
			wavSeq2.Add(new FileStream(getPath("audiotest3-mono-44100Hz-16bits.wav"), FileMode.Open, FileAccess.Read, FileShare.Read));
			Assert.IsTrue(StreamUtils.compareWavSeq(wavSeq1, wavSeq2));
			// tests 1+2+3
			wavSeq1.Clear();
			wavSeq1.Add(new FileStream(getPath("audiotest1+2+3-mono-44100Hz-16bits.wav"), FileMode.Open, FileAccess.Read, FileShare.Read));
			wavSeq2.Clear();
			wavSeq2.Add(new FileStream(getPath("audiotest1-mono-44100Hz-16bits.wav"), FileMode.Open, FileAccess.Read, FileShare.Read));
			wavSeq2.Add(new FileStream(getPath("audiotest2-mono-44100Hz-16bits.wav"), FileMode.Open, FileAccess.Read, FileShare.Read));
			wavSeq2.Add(new FileStream(getPath("audiotest3-mono-44100Hz-16bits.wav"), FileMode.Open, FileAccess.Read, FileShare.Read));
			Assert.IsTrue(StreamUtils.compareWavSeq(wavSeq1, wavSeq2));

		}










		[Test, Description("Tests value equality with very basic media data")]
		public void valueEquals_Basics()
		{
			Assert.IsFalse(mData1.valueEquals(null), "a created media data shouldn't equal null");
			Assert.IsTrue(mData1.valueEquals(mData1), "an empty media data should equal itself");
			Assert.IsTrue(mData1.valueEquals(mData2), "two identically created empty media datas should be equal");
			mData1.appendAudioDataFromRiffWave(Path.Combine(mPresentation.getRootUri().LocalPath, "audiotest1-mono-44100Hz-16bits.wav"));
			Assert.IsTrue(mData1.valueEquals(mData1), "a data with content should equal itself");

		}

		[Test, Description("Tests value equality with empty media datas, focusing on the PCM Format")]
		public void valueEquals_PCMFormat()
		{
			Assert.IsTrue(mData1.getPCMFormat().valueEquals(mData2.getPCMFormat()), "[Pre-Condition] PCM formats should be equal");
			Assert.IsTrue(mData1.valueEquals(mData2), "empty media datas with the same PCM format should be equal");
			mData1.getPCMFormat().setNumberOfChannels(1);
			mData2.getPCMFormat().setNumberOfChannels(2);
			Assert.IsFalse(mData1.getPCMFormat().valueEquals(mData2.getPCMFormat()), "[Pre-Condition] PCM formats shouldn't be equal");
			Assert.IsFalse(mData1.valueEquals(mData2), "empty media datas with different PCM formats shouldn't be equal");
		}

		[Test, Description("Tests that two media datas created from the same unique wav clip are equal")]
		public void valueEquals_SameAudioDataSingleWavClip()
		{
			mData1.appendAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"));
			mData2.appendAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"));
			Assert.IsTrue(mData1.valueEquals(mData2), "datas with the same wav content should be equal");
		}

		[Test, Description("Tests that two media datas created from the same multiple wav clips are equal")]
		public void valueEquals_SameAudioDataSameWavClips()
		{
			mData1.appendAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"));
			mData1.appendAudioDataFromRiffWave(getPath("audiotest2-mono-44100Hz-16bits.wav"));
			mData2.appendAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"));
			mData2.appendAudioDataFromRiffWave(getPath("audiotest2-mono-44100Hz-16bits.wav"));
			Assert.IsTrue(mData1.valueEquals(mData2), "datas with the same wav content should be equal");
		}

		[Test, Description("Tests the equality of two datas, one made of a single wav clip, the other containing several wav clips")]
		public void valueEquals_SameAudioDataDiffWavClips()
		{
			mData1.appendAudioDataFromRiffWave(getPath("audiotest1+2-mono-44100Hz-16bits.wav"));
			mData2.appendAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"));
			mData2.appendAudioDataFromRiffWave(getPath("audiotest2-mono-44100Hz-16bits.wav"));
			Assert.IsTrue(mData1.valueEquals(mData2), "datas with the same wav content should be equal");
		}

		[Test, Description("Tests that two audio datas with the same PCM format and bit length but different content are not equal")]
		public void valueEquals_DiffAudioData()
		{
			mData1.appendAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"));
			mData2.appendAudioDataFromRiffWave(getPath("audiotest2-mono-44100Hz-16bits.wav"));
			Assert.IsTrue(mData1.getAudioDuration().isEqualTo(mData2.getAudioDuration()), "[Pre-Condition] audio duration should be equal");
			Assert.IsFalse(mData1.valueEquals(mData2), "datas created from different wav files shouldn't be equal");
		}

		[Test, Description("Tests that two audio datas created from the same audio sample but with different PCM format are not equal")]
		public void valueEquals_DiffPCMFormat()
		{
			mData1.appendAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"));
			mData2.getPCMFormat().setSampleRate(22050);
			mData2.appendAudioDataFromRiffWave(getPath("audiotest1-mono-22050Hz-16bits.wav"));
			Assert.IsFalse(mData1.getPCMFormat().valueEquals(mData2.getPCMFormat()), "[Pre-Condition] PCM formats shouldn't be equal");
			Assert.IsFalse(mData1.valueEquals(mData2), "datas created from different wav files shouldn't be equal");
		}












		[Test, Description("Tests that trying to insert a data at a negative insertion point raises an exception")]
		[ExpectedException(typeof(exception.MethodParameterIsOutOfBoundsException))]
		public void insertAudioData_NegativeInsertPoint()
		{
			mData1.insertAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"), new Time(-1), new TimeDelta(1));
		}

		[Test, Description("Tests that trying to insert a data to an empty audio data object but at a non-zero insertion point raises an exception")]
		[ExpectedException(typeof(exception.MethodParameterIsOutOfBoundsException))]
		public void insertAudioData_InsertionGreaterThanDurationEmpty()
		{
			mData1.insertAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"), new Time(10000), new TimeDelta(1));
		}

		[Test, Description("Tests that trying to insert a data at an insertion point greater than the current duration raises an exception")]
		[ExpectedException(typeof(exception.MethodParameterIsOutOfBoundsException))]
		public void insertAudioData_InsertionGreaterThanDuration()
		{
			mData1.appendAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"));
			mData1.insertAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"), new Time(10000), new TimeDelta(1));
		}

		[Test, Description("Tests that trying to insert a data with a negative duration raises an exception")]
		[ExpectedException(typeof(exception.MethodParameterIsOutOfBoundsException))]
		public void insertAudioData_NegativeDuration()
		{
			mData1.insertAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"), Time.Zero, new TimeDelta(-1));
		}

		[Test, Description("Tests that trying to insert a data with a duration greater than the given data raises an exception")]
		[ExpectedException(typeof(exception.MethodParameterIsOutOfBoundsException))]
		public void insertAudioData_DurationGreaterThanData()
		{
			mData1.insertAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"), Time.Zero, new TimeDelta(10000));
		}

		[Test, Description("Tests inserting audio data at the beginning of some audio data")]
		public void insertAudioData_AtTheBeginning()
		{
			mData1.appendAudioDataFromRiffWave(getPath("audiotest1+2-mono-44100Hz-16bits.wav"));
			mData2.appendAudioDataFromRiffWave(getPath("audiotest2-mono-44100Hz-16bits.wav"));
			mData2.insertAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"), Time.Zero, new TimeDelta(1500));
			Assert.IsTrue(mData1.valueEquals(mData2), "the two audio datas should be equal");
		}

		[Test, Description("Tests inserting audio data at the end of some audio data")]
		public void insertAudioData_AtTheEnd()
		{
			mData1.appendAudioDataFromRiffWave(getPath("audiotest1+2-mono-44100Hz-16bits.wav"));
			mData2.appendAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"));
			mData2.insertAudioDataFromRiffWave(getPath("audiotest2-mono-44100Hz-16bits.wav"), new Time(1500), new TimeDelta(1500));
			Assert.IsTrue(mData1.valueEquals(mData2), "the two audio datas should be equal");
		}

		[Test, Description("Tests inserting audio data between two wav clips")]
		public void insertAudioData_BetweenTwoWavClips()
		{
			mData1.appendAudioDataFromRiffWave(getPath("audiotest1+2+3-mono-44100Hz-16bits.wav"));
			mData2.appendAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"));
			mData2.appendAudioDataFromRiffWave(getPath("audiotest3-mono-44100Hz-16bits.wav"));
			mData2.insertAudioDataFromRiffWave(getPath("audiotest2-mono-44100Hz-16bits.wav"), new Time(1500), new TimeDelta(1500));
			Assert.IsTrue(mData1.valueEquals(mData2), "the two audio datas should be equal");
		}

		[Test, Description("Tests inserting audio data in the middle of a wav clip")]
		public void insertAudioData_InAWavClip()
		{
			mData1.appendAudioDataFromRiffWave(getPath("audiotest1+2+3-mono-44100Hz-16bits.wav"));
			mData2.appendAudioDataFromRiffWave(getPath("audiotest1+3-mono-44100Hz-16bits.wav"));
			mData2.insertAudioDataFromRiffWave(getPath("audiotest2-mono-44100Hz-16bits.wav"), new Time(1500), new TimeDelta(1500));
			Assert.IsTrue(mData1.valueEquals(mData2), "the two audio datas should be equal");
		}












		[Test, Description("tests the equality of to audio data created by appending the same wav sample to empty data")]
		public void appendAudioData_ToEmptyData()
		{
			mData1.appendAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"));
			mData2.appendAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"));
			Assert.IsTrue(mData1.valueEquals(mData2), "the two audio datas should be equal");
		}

		[Test, Description("tests that an audio data created by appending one wav clip to another is equal to an audio data containing a single wav clip which is the concatenation of the two others")]
		public void appendAudioData_ToEqualSingleWavClip()
		{
			mData1.appendAudioDataFromRiffWave(getPath("audiotest1+2-mono-44100Hz-16bits.wav"));
			mData2.appendAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"));
			mData2.appendAudioDataFromRiffWave(getPath("audiotest2-mono-44100Hz-16bits.wav"));
			Assert.IsTrue(mData1.valueEquals(mData2), "the two audio datas should be equal");
		}
		[Test, Description("tests that two audio data created by appending multiple wav clips are equal")]
		public void appendAudioData_ToEqualMultipleWavClips()
		{
			mData1.appendAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"));
			mData1.appendAudioDataFromRiffWave(getPath("audiotest2-mono-44100Hz-16bits.wav"));
			mData2.appendAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"));
			mData2.appendAudioDataFromRiffWave(getPath("audiotest2-mono-44100Hz-16bits.wav"));
			Assert.IsTrue(mData1.valueEquals(mData2), "the two audio datas should be equal");
		}









		[Test, Description("Tests that trying to replace a data at a negative insertion point raises an exception")]
		[ExpectedException(typeof(exception.MethodParameterIsOutOfBoundsException))]
		public void replaceAudioData_NegativeInsertPoint()
		{
			mData1.appendAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"));
			mData1.replaceAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"), new Time(-1), new TimeDelta(1));
		}

		[Test, Description("Tests that trying to replace a data to an empty audio data object but at a non-zero insertion point raises an exception")]
		[ExpectedException(typeof(exception.MethodParameterIsOutOfBoundsException))]
		public void replaceAudioData_insertionGreaterThanDurationEmpty()
		{
			mData1.appendAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"));
			mData1.replaceAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"), new Time(10000), new TimeDelta(1));
		}

		[Test, Description("Tests that trying to replace a data at an insertion point greater than the current duration raises an exception")]
		[ExpectedException(typeof(exception.MethodParameterIsOutOfBoundsException))]
		public void replaceAudioData_insertionGreaterThanDuration()
		{
			mData1.appendAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"));
			mData1.replaceAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"), new Time(10000), new TimeDelta(1));
		}

		[Test, Description("Tests that trying to replace a data with a negative duration raises an exception")]
		[ExpectedException(typeof(exception.MethodParameterIsOutOfBoundsException))]
		public void replaceAudioData_NegativeDuration()
		{
			mData1.appendAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"));
			mData1.replaceAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"), Time.Zero, new TimeDelta(-1));
		}

		[Test, Description("Tests that trying to replace a data with a duration greater than the given data raises an exception")]
		[ExpectedException(typeof(exception.MethodParameterIsOutOfBoundsException))]
		public void replaceAudioData_DurationGreaterThanData()
		{
			mData1.appendAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"));
			mData1.replaceAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"), Time.Zero, new TimeDelta(10000));
		}

		[Test, Description("Tests that trying to replace a data with a duration greater than the remaining original data to replace raises an exception")]
		[ExpectedException(typeof(exception.MethodParameterIsOutOfBoundsException))]
		public void replaceAudioData_DurationGreaterThanRemainingDataToReplace()
		{
			mData1.appendAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"));
			mData1.replaceAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"), new Time(10), new TimeDelta(1500));
		}

		[Test, Description("Tests replacing an audio data at the beginning of a 1-clip audio data")]
		public void replaceAudioData_AtTheBeginning()
		{
			mData1.appendAudioDataFromRiffWave(getPath("audiotest1+2+3-mono-44100Hz-16bits.wav"));
			mData2.appendAudioDataFromRiffWave(getPath("audiotest1+2+3-mono-44100Hz-16bits.wav"));
			mData2.replaceAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"), Time.Zero, new TimeDelta(1500));
			Assert.IsTrue(mData1.valueEquals(mData2), "the two audio datas should be equal");
		}

		[Test, Description("Tests replacing an audio data at the end of a 1-clip audio data")]
		public void replaceAudioData_AtTheEnd()
		{
			mData1.appendAudioDataFromRiffWave(getPath("audiotest1+2+3-mono-44100Hz-16bits.wav"));
			mData2.appendAudioDataFromRiffWave(getPath("audiotest1+2+3-mono-44100Hz-16bits.wav"));
			mData2.replaceAudioDataFromRiffWave(getPath("audiotest3-mono-44100Hz-16bits.wav"), new Time(3000), new TimeDelta(1500));
			Assert.IsTrue(mData1.valueEquals(mData2), "the two audio datas should be equal");
		}

		[Test, Description("Tests replacing an audio data stricly included in a wav clip")]
		public void replaceAudioData_SmallerThanAWavClip()
		{
			mData1.appendAudioDataFromRiffWave(getPath("audiotest1+2+3-mono-44100Hz-16bits.wav"));
			mData2.appendAudioDataFromRiffWave(getPath("audiotest1+2+3-mono-44100Hz-16bits.wav"));
			mData2.replaceAudioDataFromRiffWave(getPath("audiotest2-mono-44100Hz-16bits.wav"), new Time(1500), new TimeDelta(1500));
			Assert.IsTrue(mData1.valueEquals(mData2), "the two audio datas should be equal");
		}

		[Test, Description("Tests replacing an audio data matching exactly a wav clip")]
		public void replaceAudioData_ExactlyAWavClip()
		{
			mData1.appendAudioDataFromRiffWave(getPath("audiotest1+2+3-mono-44100Hz-16bits.wav"));
			mData2.appendAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"));
			mData2.appendAudioDataFromRiffWave(getPath("audiotest2-mono-44100Hz-16bits.wav"));
			mData2.appendAudioDataFromRiffWave(getPath("audiotest3-mono-44100Hz-16bits.wav"));
			mData2.replaceAudioDataFromRiffWave(getPath("audiotest2-mono-44100Hz-16bits.wav"), new Time(1500), new TimeDelta(1500));
			Assert.IsTrue(mData1.valueEquals(mData2), "the two audio datas should be equal");
		}

		[Test, Description("Tests replacing an audio data spanning across two wav clips")]
		public void replaceAudioData_SpanningTwoWavClips()
		{
			mData1.appendAudioDataFromRiffWave(getPath("audiotest1+2-mono-44100Hz-16bits.wav"));
			mData1.appendAudioDataFromRiffWave(getPath("audiotest1+2-mono-44100Hz-16bits.wav"));
			mData2.appendAudioDataFromRiffWave(getPath("audiotest1+2-mono-44100Hz-16bits.wav"));
			mData2.appendAudioDataFromRiffWave(getPath("audiotest1+2-mono-44100Hz-16bits.wav"));
			mData2.replaceAudioDataFromRiffWave(getPath("audiotest2+1-mono-44100Hz-16bits.wav"), new Time(1500), new TimeDelta(3000));
			Assert.IsTrue(mData1.valueEquals(mData2), "the two audio datas should be equal");
		}












		[Test, Description("Tests that trying to remove an audio data with a negative clip begin raises an exception")]
		[ExpectedException(typeof(exception.MethodParameterIsOutOfBoundsException))]
		public void removeAudioData_ClipBeginNegative()
		{
			mData1.appendAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"));
			mData1.removeAudioData(new Time(-1));
		}

		[Test, Description("Tests that trying to remove an audio data with a clip begin greater than the audio duration raises an exception")]
		[ExpectedException(typeof(exception.MethodParameterIsOutOfBoundsException))]
		public void removeAudioData_ClipBeginGreaterThanDuration()
		{
			mData1.appendAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"));
			mData1.removeAudioData(new Time(10000));
		}

		[Test, Description("Tests that trying to remove an audio data with a clip begin greater than the clip end raises an exception")]
		[ExpectedException(typeof(exception.MethodParameterIsOutOfBoundsException))]
		public void removeAudioData_ClipBeginGreaterThanClipEnd()
		{
			mData1.appendAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"));
			mData1.removeAudioData(new Time(2), new Time(1));
		}

		[Test, Description("Tests that trying to remove an audio data with a negative clip end raises an exception")]
		[ExpectedException(typeof(exception.MethodParameterIsOutOfBoundsException))]
		public void removeAudioData_ClipEndNegative()
		{
			mData1.appendAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"));
			mData1.removeAudioData(new Time(1), new Time(-1));
		}

		[Test, Description("Tests that trying to remove an audio data with a clip end greater than the audio duration raises an exception")]
		[ExpectedException(typeof(exception.MethodParameterIsOutOfBoundsException))]
		public void removeAudioData_ClipEndGreaterThanDuration()
		{
			mData1.appendAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"));
			mData1.removeAudioData(new Time(1), new Time(10000));
		}

		[Test, Description("Tests that removing an empty clip of data does nothing")]
		public void removeAudioData_ClipBeginEqualClipEnd()
		{
			mData1.appendAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"));
			mData2.appendAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"));
			mData2.removeAudioData(new Time(1000), new Time(1000));
			Assert.IsTrue(mData1.valueEquals(mData2), "the two audio datas should be equal");
		}

		[Test, Description("Tests removing the beginning of a 1-clip audio data")]
		public void removeAudioData_FromTheBeginning()
		{
			mData1.appendAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"));
			mData2.appendAudioDataFromRiffWave(getPath("audiotest2+1-mono-44100Hz-16bits.wav"));
			mData2.removeAudioData(Time.Zero, new Time(1500));
			Assert.IsTrue(mData1.valueEquals(mData2), "the two audio datas should be equal");
		}

		[Test, Description("Tests removing the end of a 1-clip audio data")]
		public void removeAudioData_AtTheEnd()
		{
			mData1.appendAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"));
			mData2.appendAudioDataFromRiffWave(getPath("audiotest1+2-mono-44100Hz-16bits.wav"));
			mData2.removeAudioData(new Time(1500), new Time(3000));
			Assert.IsTrue(mData1.valueEquals(mData2), "the two audio datas should be equal");
		}

		[Test, Description("Tests removing all the content of 1-clip audio data")]
		public void removeAudioData_RemoveAllSingleClip()
		{
			mData2.appendAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"));
			mData2.removeAudioData(Time.Zero, new Time(1500));
			Assert.IsTrue(mData1.valueEquals(mData2), "the two audio datas should be equal");
		}

		[Test, Description("Tests removing all the content of multi clip audio data")]
		public void removeAudioData_RemoveAllMultiClip()
		{
			mData2.appendAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"));
			mData2.appendAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"));
			mData2.removeAudioData(Time.Zero, new Time(3000));
			Assert.IsTrue(mData1.valueEquals(mData2), "the two audio datas should be equal");
		}

		[Test, Description("Tests that the one-arg method behaves the same as the two-arg method")]
		public void removeAudioData_OneArg()
		{
			mData1.appendAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"));
			mData2.appendAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"));
			mData1.removeAudioData(new Time(1000));
			mData2.removeAudioData(new Time(1000), new Time(1500));
			Assert.IsTrue(mData1.valueEquals(mData2), "the two audio datas should be equal");
		}

		[Test, Description("Tests removing audio data strictly included in a wav clip")]
		public void removeAudioData_IncludedInAWavClip()
		{
			mData1.appendAudioDataFromRiffWave(getPath("audiotest1+3-mono-44100Hz-16bits.wav"));
			mData2.appendAudioDataFromRiffWave(getPath("audiotest1+2+3-mono-44100Hz-16bits.wav"));
			mData2.removeAudioData(new Time(1500), new Time(3000));
			Assert.IsTrue(mData1.valueEquals(mData2), "the two audio datas should be equal");
		}

		[Test, Description("Tests removing audio data matching exaclty one wav clip")]
		public void removeAudioData_ExactlyAWavClip()
		{
			mData1.appendAudioDataFromRiffWave(getPath("audiotest1+3-mono-44100Hz-16bits.wav"));
			mData2.appendAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"));
			mData2.appendAudioDataFromRiffWave(getPath("audiotest2-mono-44100Hz-16bits.wav"));
			mData2.appendAudioDataFromRiffWave(getPath("audiotest3-mono-44100Hz-16bits.wav"));
			mData2.removeAudioData(new Time(1500), new Time(3000));
			Assert.IsTrue(mData1.valueEquals(mData2), "the two audio datas should be equal");
		}

		[Test, Description("Tests removing audio data spanning across two wav clips")]
		public void removeAudioData_SpanningTwoWavClips()
		{
			mData1.appendAudioDataFromRiffWave(getPath("audiotest1+2-mono-44100Hz-16bits.wav"));
			mData2.appendAudioDataFromRiffWave(getPath("audiotest1+2-mono-44100Hz-16bits.wav"));
			mData2.appendAudioDataFromRiffWave(getPath("audiotest1+2-mono-44100Hz-16bits.wav"));
			mData2.removeAudioData(new Time(1500), new Time(4500));
			Assert.IsTrue(mData1.valueEquals(mData2), "the two audio datas should be equal");
		}















		[Test, Description("Tests that trying to get an audio data with a negative clip begin raises an exception")]
		[ExpectedException(typeof(exception.MethodParameterIsOutOfBoundsException))]
		public void getAudioData_ClipBeginNegative()
		{
			mData1.appendAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"));
			mData1.getAudioData(new Time(-1));
		}

		[Test, Description("Tests that trying to get an audio data with a clip begin greater than the audio duration raises an exception")]
		[ExpectedException(typeof(exception.MethodParameterIsOutOfBoundsException))]
		public void getAudioData_ClipBeginGreaterThanDuration()
		{
			mData1.appendAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"));
			mData1.getAudioData(new Time(10000));
		}

		[Test, Description("Tests that trying to get an audio data with a clip begin greater than the clip end raises an exception")]
		[ExpectedException(typeof(exception.MethodParameterIsOutOfBoundsException))]
		public void getAudioData_ClipBeginGreaterThanClipEnd()
		{
			mData1.appendAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"));
			mData1.getAudioData(new Time(2), new Time(1)).Close();
		}

		[Test, Description("Tests that trying to get an audio data with a negative clip end raises an exception")]
		[ExpectedException(typeof(exception.MethodParameterIsOutOfBoundsException))]
		public void getAudioData_ClipEndNegative()
		{
			mData1.appendAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"));
			mData1.getAudioData(new Time(1), new Time(-1)).Close();
		}

		[Test, Description("Tests that trying to get an audio data with a clip end greater than the audio duration raises an exception")]
		[ExpectedException(typeof(exception.MethodParameterIsOutOfBoundsException))]
		public void getAudioData_ClipEndGreaterThanDuration()
		{
			mData1.appendAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"));
			mData1.getAudioData(Time.Zero, new Time(10000)).Close();
		}

		[Test, Description("Tests that getting an empty audio data returns en empty stream")]
		public void getAudioData_ClipBeginEqualClipEnd()
		{
			mData1.appendAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"));
			Stream s = mData1.getAudioData(new Time(1000), new Time(1000));
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
		public void getAudioData_EmptyData()
		{
			Stream s = mData1.getAudioData();
			try
			{
				Assert.IsTrue(s.Length == 0, "the returned audio data should be empty");
			}
			finally
			{
				s.Close();
			}
		}

		[Test, Description("Tests getting the beginning of a 1-clip audio data")]
		public void getAudioData_FromTheBeginning()
		{
			mData1.appendAudioDataFromRiffWave(getPath("audiotest1+2-mono-44100Hz-16bits.wav"));
			Stream s1 = mData1.getAudioData(Time.Zero, new Time(1500));
			try
			{
				Stream s2 = getRawStream("audiotest1-mono-44100Hz-16bits.wav");
				try
				{
					Assert.IsTrue(StreamUtils.compareStreams(s1, s2), "the two streams should be equal");
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
		public void getAudioData_AtTheEnd()
		{
			mData1.appendAudioDataFromRiffWave(getPath("audiotest1+2-mono-44100Hz-16bits.wav"));
			Stream s1 = mData1.getAudioData(new Time(1500), new Time(3000));
			try
			{
				Stream s2 = getRawStream("audiotest2-mono-44100Hz-16bits.wav");
				try
				{
					Assert.IsTrue(StreamUtils.compareStreams(s1, s2), "the two streams should be equal");
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
		public void getAudioData_getAllSingleClip()
		{
			mData1.appendAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"));
			Stream s1 = mData1.getAudioData();
			try
			{
				Stream s2 = getRawStream("audiotest1-mono-44100Hz-16bits.wav");
				try
				{
					Assert.IsTrue(StreamUtils.compareStreams(s1, s2), "the two streams should be equal");
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
		public void getAudioData_getAllMultiClip()
		{
			mData1.appendAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"));
			mData1.appendAudioDataFromRiffWave(getPath("audiotest2-mono-44100Hz-16bits.wav"));
			Stream s1 = mData1.getAudioData();
			try
			{
				Stream s2 = getRawStream("audiotest1+2-mono-44100Hz-16bits.wav");
				try
				{
					Assert.IsTrue(StreamUtils.compareStreams(s1, s2), "the two streams should be equal");
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
		public void getAudioData_OneArg()
		{
			mData1.appendAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"));
			Stream s1 = mData1.getAudioData(new Time(1000), new Time(1500));
			try
			{
				Stream s2 = mData1.getAudioData(new Time(1000));
				try
				{
					Assert.IsTrue(StreamUtils.compareStreams(s1, s2), "the two streams should be equal");
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
		public void getAudioData_ZeroArg()
		{
			mData1.appendAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"));
			Stream s1 = mData1.getAudioData(Time.Zero, new Time(1500));
			try
			{
				Stream s2 = mData1.getAudioData();
				try
				{
					Assert.IsTrue(StreamUtils.compareStreams(s1, s2), "the two streams should be equal");
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
		public void getAudioData_IncludedInAWavClip()
		{
			mData1.appendAudioDataFromRiffWave(getPath("audiotest1+2+3-mono-44100Hz-16bits.wav"));
			Stream s1 = mData1.getAudioData(new Time(1500), new Time(3000));
			try
			{
				Stream s2 = getRawStream("audiotest2-mono-44100Hz-16bits.wav");
				try
				{
					Assert.IsTrue(StreamUtils.compareStreams(s1, s2), "the two streams should be equal");
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
		public void getAudioData_ExactlyAWavClip()
		{
			mData1.appendAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"));
			mData1.appendAudioDataFromRiffWave(getPath("audiotest2-mono-44100Hz-16bits.wav"));
			mData1.appendAudioDataFromRiffWave(getPath("audiotest3-mono-44100Hz-16bits.wav"));
			Stream s1 = mData1.getAudioData(new Time(1500), new Time(3000));
			try
			{
				Stream s2 = getRawStream("audiotest2-mono-44100Hz-16bits.wav");
				try
				{
					Assert.IsTrue(StreamUtils.compareStreams(s1, s2), "the two streams should be equal");
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
		public void getAudioData_SpanningTwoWavClips()
		{
			mData1.appendAudioDataFromRiffWave(getPath("audiotest1+2-mono-44100Hz-16bits.wav"));
			mData1.appendAudioDataFromRiffWave(getPath("audiotest1+2-mono-44100Hz-16bits.wav"));
			Stream s1 = mData1.getAudioData(new Time(1500), new Time(4500));
			try
			{
				Stream s2 = getRawStream("audiotest2+1-mono-44100Hz-16bits.wav");
				try
				{
					Assert.IsTrue(StreamUtils.compareStreams(s1, s2), "the two streams should be equal");
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
		[ExpectedException(typeof(exception.InvalidDataFormatException))]
		public void mergeWith_IncompatiblePCMFormat()
		{
			mData1.getPCMFormat().setNumberOfChannels(2);
			mData1.mergeWith(mData2);
		}

		[Test, Description("Tests that merging with an empty audio data doesn't change anything")]
		public void mergeWith_EmptyData()
		{
			mData1.appendAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"));
			mData2.appendAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"));
			mData2.mergeWith(mData3);
			Assert.IsTrue(mData3.getAudioData().Length == 0);
			Assert.IsTrue(mData1.valueEquals(mData2), "the two audio datas should be equal");
		}

		[Test, Description("Tests merging audio data to an empty audio data")]
		public void mergeWith_EmptyCaller()
		{
			mData1.appendAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"));
			mData3.appendAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"));
			mData2.mergeWith(mData3);
			Assert.IsTrue(mData3.getAudioData().Length == 0);
			Assert.IsTrue(mData1.valueEquals(mData2), "the two audio datas should be equal");
		}

		[Test, Description("Tests merging with a 1-clip audio data")]
		public void mergeWith_OneWavClip()
		{
			mData1.appendAudioDataFromRiffWave(getPath("audiotest1+2-mono-44100Hz-16bits.wav"));
			mData2.appendAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"));
			mData3.appendAudioDataFromRiffWave(getPath("audiotest2-mono-44100Hz-16bits.wav"));
			mData2.mergeWith(mData3);
			Assert.IsTrue(mData3.getAudioData().Length == 0);
			Assert.IsTrue(mData1.valueEquals(mData2), "the two audio datas should be equal");
		}

		[Test, Description("Tests merging with a multi-clips audio data")]
		public void mergeWith_MultiWavClips()
		{
			mData1.appendAudioDataFromRiffWave(getPath("audiotest1+2+3-mono-44100Hz-16bits.wav"));
			mData2.appendAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"));
			mData3.appendAudioDataFromRiffWave(getPath("audiotest2-mono-44100Hz-16bits.wav"));
			mData3.appendAudioDataFromRiffWave(getPath("audiotest3-mono-44100Hz-16bits.wav"));
			mData2.mergeWith(mData3);
			Assert.IsTrue(mData3.getAudioData().Length == 0);
			Assert.IsTrue(mData1.valueEquals(mData2), "the two audio datas should be equal");
		}

		[Test, Description("Tests merging to a multi-clips audio data")]
		public void mergeWith_MultiWavClipsInCaller()
		{
			mData1.appendAudioDataFromRiffWave(getPath("audiotest1+2+3-mono-44100Hz-16bits.wav"));
			mData2.appendAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"));
			mData2.appendAudioDataFromRiffWave(getPath("audiotest2-mono-44100Hz-16bits.wav"));
			mData3.appendAudioDataFromRiffWave(getPath("audiotest3-mono-44100Hz-16bits.wav"));
			mData2.mergeWith(mData3);
			Assert.IsTrue(mData3.getAudioData().Length == 0);
			Assert.IsTrue(mData1.valueEquals(mData2), "the two audio datas should be equal");
		}








		[Test, Description("Tests that trying to split an audio data at a negative point raises an exception")]
		[ExpectedException(typeof(exception.MethodParameterIsOutOfBoundsException))]
		public void split_OutOfBoundNegative()
		{
			mData1.split(new Time(-1));
		}

		[Test, Description("Tests that tryng to split an audio beyond its duration raises an exception")]
		[ExpectedException(typeof(exception.MethodParameterIsOutOfBoundsException))]
		public void split_OutOfBoundBeyond()
		{
			mData1.split(new Time(1));
		}

		[Test, Description("Tests splitting an empty audio data")]
		public void split_EmptyData()
		{
			AudioMediaData split = mData1.split(Time.Zero);
			Assert.IsTrue(split.valueEquals(mData2), "the split data should be empty");
			Assert.IsTrue(mData1.valueEquals(mData3), "the remaining data should be empty");
		}

		[Test, Description("Tests splitting an audio data at the beginning")]
		public void split_AtTheBeginning()
		{
			mData1.appendAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"));
			AudioMediaData split = mData1.split(Time.Zero);
			mData2.appendAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"));
			Assert.IsTrue(split.valueEquals(mData2), "the split data is not what was expected");
			Assert.IsTrue(mData1.valueEquals(mData3), "the remaining data should be empty");
		}

		[Test, Description("Tests splitting an audio data at the end")]
		public void split_AtTheEnd()
		{
			mData1.appendAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"));
			AudioMediaData split = mData1.split(new Time(1500));
			mData3.appendAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"));
			Assert.IsTrue(split.valueEquals(mData2), "the split data should be empty");
			Assert.IsTrue(mData1.valueEquals(mData3), "the remaining data is not what was expecte");
		}

		[Test, Description("Tests splitting an audio at a point within a wav clip in a 1-clip audio data")]
		public void split_InsideAWavClip()
		{
			mData1.appendAudioDataFromRiffWave(getPath("audiotest1+2-mono-44100Hz-16bits.wav"));
			AudioMediaData split = mData1.split(new Time(1500));
			mData2.appendAudioDataFromRiffWave(getPath("audiotest2-mono-44100Hz-16bits.wav"));
			mData3.appendAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"));
			Assert.IsTrue(split.valueEquals(mData2), "the split data is not what was expected");
			Assert.IsTrue(mData1.valueEquals(mData3), "the remaining data is not what was expected");
		}

		[Test, Description("Tests splitting an audio at a point within a wav clip in a multi-clips audio data")]
		public void split_InsideAWavClipInMultiClipData()
		{
			mData1.appendAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"));
			mData1.appendAudioDataFromRiffWave(getPath("audiotest2+1-mono-44100Hz-16bits.wav"));
			mData1.appendAudioDataFromRiffWave(getPath("audiotest2-mono-44100Hz-16bits.wav"));
			AudioMediaData split = mData1.split(new Time(3000));
			mData2.appendAudioDataFromRiffWave(getPath("audiotest1+2-mono-44100Hz-16bits.wav"));
			mData3.appendAudioDataFromRiffWave(getPath("audiotest1+2-mono-44100Hz-16bits.wav"));
			Assert.IsTrue(split.valueEquals(mData2), "the split data is not what was expected");
			Assert.IsTrue(mData1.valueEquals(mData3), "the remaining data is not what was expected");
		}

		[Test, Description("Tests splitting an audio at a point between two wav clips")]
		public void split_BetweenWavClips()
		{
			mData1.appendAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"));
			mData1.appendAudioDataFromRiffWave(getPath("audiotest2-mono-44100Hz-16bits.wav"));
			AudioMediaData split = mData1.split(new Time(1500));
			mData2.appendAudioDataFromRiffWave(getPath("audiotest2-mono-44100Hz-16bits.wav"));
			mData3.appendAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"));
			Assert.IsTrue(split.valueEquals(mData2), "the split data is not what was expected");
			Assert.IsTrue(mData1.valueEquals(mData3), "the remaining data is not what was expected");
		}









		[Test, Description("Tests copying an empty audio data")]
		public void copy_Empty()
		{
			WavAudioMediaData copy = mData1.copy();
			Assert.IsTrue(mData1.valueEquals(copy));
			//List<IDataProvider> provList = mData1.getListOfUsedDataProviders();
			//List<IDataProvider> provListCopy = copy.getListOfUsedDataProviders();
			//Assert.IsTrue(provList.Count == provListCopy.Count,"the copy does not have the same number of data providers");
			//for (int i = 0; i < provList.Count; i++)
			//{
			//    Assert.IsTrue(provList[i].valueEquals(provListCopy[i]),"a data provider of the copy was not equal to the data provider of the original data");
			//}
		}

		[Test, Description("Tests copying a 1-clip audio data")]
		public void copy_OneClip()
		{
			mData1.appendAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"));
			WavAudioMediaData copy = mData1.copy();
			Assert.IsTrue(mData1.valueEquals(copy));
			//List<IDataProvider> provList = mData1.getListOfUsedDataProviders();
			//List<IDataProvider> provListCopy = copy.getListOfUsedDataProviders();
			//Assert.IsTrue(provList.Count == provListCopy.Count, "the copy does not have the same number of data providers");
			//for (int i = 0; i < provList.Count; i++)
			//{
			//    Assert.IsTrue(provList[i].valueEquals(provListCopy[i]), "a data provider of the copy was not equal to the data provider of the original data");
			//}
		}

		[Test, Description("Tests copying a multi-clips audio data")]
		public void copy_MultiClips()
		{
			mData1.appendAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"));
			mData1.appendAudioDataFromRiffWave(getPath("audiotest1-mono-44100Hz-16bits.wav"));
			WavAudioMediaData copy = mData1.copy();
			Assert.IsTrue(mData1.valueEquals(copy));
			//List<IDataProvider> provList = mData1.getListOfUsedDataProviders();
			//List<IDataProvider> provListCopy = copy.getListOfUsedDataProviders();
			//Assert.IsTrue(provList.Count == provListCopy.Count, "the copy does not have the same number of data providers");
			//for (int i = 0; i < provList.Count; i++)
			//{
			//    Assert.IsTrue(provList[i].valueEquals(provListCopy[i]), "a data provider of the copy was not equal to the data provider of the original data");
			//}
		}
	}
}

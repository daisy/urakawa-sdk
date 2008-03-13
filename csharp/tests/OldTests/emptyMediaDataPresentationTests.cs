using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NUnit.Framework;
using urakawa.core;
using urakawa.property.channel;
using urakawa.media;
using urakawa.media.data;
using urakawa.media.data.audio;
using urakawa.media.timing;

namespace urakawa.unitTests.mediaDataTests
{
	[TestFixture]
	public class EmptyMediaDataPresentationTests
	{
		private Project mProject;
		private Uri mProjectUri;

		[SetUp]
		public void Init()
		{
			mProjectUri = new Uri(System.IO.Directory.GetCurrentDirectory());
			mProjectUri = new Uri(mProjectUri, "../XukWorks/EmptyMediaDataPresentationTests/");
			mProject = new Project();
			mProject.addNewPresentation();
			mProject.getPresentation(0).setRootUri(mProjectUri);
			mProject.getPresentation(0).getMediaDataManager().setDefaultPCMFormat(new PCMFormatInfo(1, 22050, 16));
			mProject.getPresentation(0).getMediaDataManager().setEnforceSinglePCMFormat(true);
		}

		[TearDown]
		public void Terminate()
		{
			string projDir = System.IO.Path.GetDirectoryName(mProjectUri.LocalPath);
			if (System.IO.Directory.Exists(projDir)) System.IO.Directory.Delete(projDir, true);
		}

		[Test]
		public void ImportAudio()
		{
			ManagedAudioMedia mam = (ManagedAudioMedia)mProject.getPresentation(0).getMediaFactory().createMedia("ManagedAudioMedia", ToolkitSettings.XUK_NS);
			string path = "../../XukWorks/MediaDataSample/Data/aud000000.wav";
			mam.getMediaData().appendAudioDataFromRiffWave(path);
			Assert.AreEqual(
				93312, mam.getMediaData().getPCMLength(), 
				"Expected wav file ../MediaDataDample/Data/aud000000.wav to contain 93312 bytes of PCM data");
			path = "../../XukWorks/MediaDataSample/Data/aud000001.wav";
			mam.getMediaData().appendAudioDataFromRiffWave(path);
			Assert.AreEqual(
				93312 + 231542, mam.getMediaData().getPCMLength(),
				"Expected wav file ../MediaDataDample/Data/aud000000.wav to contain 93312 bytes of PCM data");
		}

		[Test]
		[ExpectedException(typeof(urakawa.exception.InvalidDataFormatException))]
		public void ImportInvalidPCMFormatAudio()
		{
			mProject.getPresentation(0).getMediaDataManager().setDefaultSampleRate(44100);
			ImportAudio();
		}

		[Test]
		public void MergeAudio()
		{
			ManagedAudioMedia mam0 = (ManagedAudioMedia)mProject.getPresentation(0).getMediaFactory().createMedia("ManagedAudioMedia", ToolkitSettings.XUK_NS);
			string path = "../../XukWorks/MediaDataSample/Data/aud000000.wav";
			mam0.getMediaData().appendAudioDataFromRiffWave(path);
			Assert.AreEqual(
				93312, mam0.getMediaData().getPCMLength(),
				"Expected wav file ../MediaDataDample/Data/aud000000.wav to contain 93312 bytes of PCM data");
			ManagedAudioMedia mam1 = (ManagedAudioMedia)mProject.getPresentation(0).getMediaFactory().createMedia(
				"ManagedAudioMedia", ToolkitSettings.XUK_NS);
			path = "../../XukWorks/MediaDataSample/Data/aud000001.wav";
			mam1.getMediaData().appendAudioDataFromRiffWave(path);
			Assert.AreEqual(
				231542, mam1.getMediaData().getPCMLength(),
				"Expected wav file ../MediaDataDample/Data/aud000000.wav to contain 93312 bytes of PCM data");
			mam0.mergeWith(mam1);
			Assert.AreEqual(
				93312+231542, mam0.getMediaData().getPCMLength(),
				"Expected the merged ManagedAudioMedia to contain 93312+231542 bytes of PCM data");
			Assert.AreEqual(
				0, mam1.getMediaData().getPCMLength(),
				"Expected the managerAudioMedia with which there was merged to have no PCM data");
		}

		[Test]
		public void SplitAudio()
		{
			List<ManagedAudioMedia> mams = new List<ManagedAudioMedia>();
			mams.Add((ManagedAudioMedia)mProject.getPresentation(0).getMediaFactory().createMedia("ManagedAudioMedia", ToolkitSettings.XUK_NS));
			string path = "../../XukWorks/MediaDataSample/Data/aud000000.wav";
			mams[0].getMediaData().appendAudioDataFromRiffWave(path);
			double initMSecs = mams[0].getDuration().getTimeDeltaAsMillisecondFloat();
			double msecs, diff;
			for (int i = 0; i < 6; i++)
			{
				msecs = mams[i].getDuration().getTimeDeltaAsMillisecondFloat();
				mams.Add(mams[i].split(new Time(msecs / 2)));
				diff = Math.Abs((msecs / 2) - mams[i].getDuration().getTimeDeltaAsMillisecondFloat());
				Assert.Less(
					diff, 0.1,
					"The difference the split ManagedAudioMedia actual and expec duration is more than 0.1ms");
				diff = Math.Abs((msecs / 2) - mams[i + 1].getDuration().getTimeDeltaAsMillisecondFloat());
				Assert.Less(
					diff, 0.1,
					"The difference the split ManagedAudioMedia actual and expec duration is more than 0.1ms");
			}
			msecs = 0;

			foreach (ManagedAudioMedia m in mams)
			{
				double s = m.getDuration().getTimeDeltaAsMillisecondFloat();
				msecs += s;
			}
			diff = Math.Abs(msecs-initMSecs);
			Assert.Less(
				diff, 0.1,
				"The difference between the initial duration and the sum of the 7 splitted ManagedAudioMedia is more than 0.1ms");
		}
	}
}

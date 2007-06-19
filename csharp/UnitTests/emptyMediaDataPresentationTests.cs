using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NUnit.Framework;
using urakawa.core;
using urakawa.properties.channel;
using urakawa.media;
using urakawa.media.data;
using urakawa.media.data.audio;

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
			mProject = new Project(mProjectUri);
			mProject.getPresentation().getMediaDataManager().getDefaultPCMFormat().setSampleRate(22050);
			mProject.getPresentation().getMediaDataManager().getDefaultPCMFormat().setNumberOfChannels(1);
			mProject.getPresentation().getMediaDataManager().getDefaultPCMFormat().setBitDepth(16);
			mProject.getPresentation().getMediaDataManager().setEnforceSinglePCMFormat(true);
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
			ManagedAudioMedia mam = (ManagedAudioMedia)mProject.getPresentation().getMediaFactory().createMedia(MediaType.AUDIO);
			string path = "../../XukWorks/MediaDataSample/Data/aud000000.wav";
			mam.getMediaData().appendAudioDataFromRiffWave(path);
			Assert.AreEqual(
				93312, mam.getMediaData().getPCMLength(), 
				"Expected wav file ../MediaDataDample/Data/aud000000.wav to contain 93312 bytes of PCM data");
		}

		[Test]
		[ExpectedException(typeof(urakawa.exception.InvalidDataFormatException))]
		public void ImportInvalidPCMformatAudio()
		{
			mProject.getPresentation().getMediaDataManager().getDefaultPCMFormat().setSampleRate(44100);
			ImportAudio();
		}

		[Test]
		public void MergeAudio()
		{
			ManagedAudioMedia mam0 = (ManagedAudioMedia)mProject.getPresentation().getMediaFactory().createMedia(MediaType.AUDIO);
			string path = "../../XukWorks/MediaDataSample/Data/aud000000.wav";
			mam0.getMediaData().appendAudioDataFromRiffWave(path);
			Assert.AreEqual(
				93312, mam0.getMediaData().getPCMLength(),
				"Expected wav file ../MediaDataDample/Data/aud000000.wav to contain 93312 bytes of PCM data");
			ManagedAudioMedia mam1 = (ManagedAudioMedia)mProject.getPresentation().getMediaFactory().createMedia(MediaType.AUDIO);
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
	}
}

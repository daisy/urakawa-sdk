using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using AudioLib;
using NUnit.Framework;
using urakawa.core;
using urakawa.property.channel;
using urakawa.media;
using urakawa.media.data;
using urakawa.media.data.audio;
using urakawa.media.timing;
using urakawa.xuk;

namespace urakawa.oldTests
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
            mProject.AddNewPresentation();
            mProject.Presentations.Get(0).RootUri = mProjectUri;
            mProject.Presentations.Get(0).MediaDataManager.DefaultPCMFormat = new PCMFormatInfo(1, 22050, 16);
            mProject.Presentations.Get(0).MediaDataManager.EnforceSinglePCMFormat = true;
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
            ManagedAudioMedia mam = mProject.Presentations.Get(0).MediaFactory.Create<ManagedAudioMedia>();
            string path = "../../XukWorks/MediaDataSample/Data/aud000000.wav";
            mam.AudioMediaData.AppendAudioDataFromRiffWave(path);
            Assert.AreEqual(
                93312, AudioLibPCMFormat.ConvertTimeToBytes(mam.Duration.TimeDeltaAsMillisecondDouble, (int)mam.AudioMediaData.PCMFormat.SampleRate, mam.AudioMediaData.PCMFormat.BlockAlign),
            
                "Expected wav file ../MediaDataDample/Data/aud000000.wav to contain 93312 bytes of PCM data");
            path = "../../XukWorks/MediaDataSample/Data/aud000001.wav";
            mam.AudioMediaData.AppendAudioDataFromRiffWave(path);
            Assert.AreEqual(
                93312 + 231542, AudioLibPCMFormat.ConvertTimeToBytes(mam.Duration.TimeDeltaAsMillisecondDouble, (int)mam.AudioMediaData.PCMFormat.SampleRate, mam.AudioMediaData.PCMFormat.BlockAlign),
                "Expected wav file ../MediaDataDample/Data/aud000000.wav to contain 93312 bytes of PCM data");
        }

        [Test]
        [ExpectedException(typeof (urakawa.exception.InvalidDataFormatException))]
        public void ImportInvalidPCMFormatAudio()
        {
            mProject.Presentations.Get(0).MediaDataManager.DefaultSampleRate = 44100;
            ImportAudio();
        }

        [Test]
        public void MergeAudio()
        {
            ManagedAudioMedia mam0 = mProject.Presentations.Get(0).MediaFactory.Create<ManagedAudioMedia>();
            string path = "../../XukWorks/MediaDataSample/Data/aud000000.wav";
            mam0.AudioMediaData.AppendAudioDataFromRiffWave(path);
            Assert.AreEqual(
                93312, AudioLibPCMFormat.ConvertTimeToBytes(mam0.Duration.TimeDeltaAsMillisecondDouble, (int)mam0.AudioMediaData.PCMFormat.SampleRate, mam0.AudioMediaData.PCMFormat.BlockAlign),
                "Expected wav file ../MediaDataDample/Data/aud000000.wav to contain 93312 bytes of PCM data");
            ManagedAudioMedia mam1 = mProject.Presentations.Get(0).MediaFactory.Create<ManagedAudioMedia>();
            path = "../../XukWorks/MediaDataSample/Data/aud000001.wav";
            mam1.AudioMediaData.AppendAudioDataFromRiffWave(path);
            Assert.AreEqual(
                231542, AudioLibPCMFormat.ConvertTimeToBytes(mam1.Duration.TimeDeltaAsMillisecondDouble, (int)mam1.AudioMediaData.PCMFormat.SampleRate, mam1.AudioMediaData.PCMFormat.BlockAlign),
                "Expected wav file ../MediaDataDample/Data/aud000000.wav to contain 93312 bytes of PCM data");
            mam0.MergeWith(mam1);
            Assert.AreEqual(
                93312 + 231542, AudioLibPCMFormat.ConvertTimeToBytes(mam0.Duration.TimeDeltaAsMillisecondDouble, (int)mam0.AudioMediaData.PCMFormat.SampleRate, mam0.AudioMediaData.PCMFormat.BlockAlign),
                "Expected the merged ManagedAudioMedia to contain 93312+231542 bytes of PCM data");
            Assert.AreEqual(
                0, AudioLibPCMFormat.ConvertTimeToBytes(mam1.Duration.TimeDeltaAsMillisecondDouble, (int)mam1.AudioMediaData.PCMFormat.SampleRate, mam1.AudioMediaData.PCMFormat.BlockAlign),
                "Expected the managerAudioMedia with which there was merged to have no PCM data");
        }

        [Test]
        public void SplitAudio()
        {
            List<ManagedAudioMedia> mams = new List<ManagedAudioMedia>();
            mams.Add(mProject.Presentations.Get(0).MediaFactory.Create<ManagedAudioMedia>());
            string path = "../../XukWorks/MediaDataSample/Data/aud000000.wav";
            mams[0].AudioMediaData.AppendAudioDataFromRiffWave(path);
            double initMSecs = mams[0].Duration.TimeDeltaAsMillisecondDouble;
            double msecs, diff;
            for (int i = 0; i < 6; i++)
            {
                msecs = mams[i].Duration.TimeDeltaAsMillisecondDouble;
                mams.Add(mams[i].Split(new Time(msecs/2)));
                diff = Math.Abs((msecs/2) - mams[i].Duration.TimeDeltaAsMillisecondDouble);
                Assert.Less(
                    diff, 0.1,
                    "The difference the split ManagedAudioMedia actual and expec duration is more than 0.1ms");
                diff = Math.Abs((msecs/2) - mams[i + 1].Duration.TimeDeltaAsMillisecondDouble);
                Assert.Less(
                    diff, 0.1,
                    "The difference the split ManagedAudioMedia actual and expec duration is more than 0.1ms");
            }
            msecs = 0;

            foreach (ManagedAudioMedia m in mams)
            {
                double s = m.Duration.TimeDeltaAsMillisecondDouble;
                msecs += s;
            }
            diff = Math.Abs(msecs - initMSecs);
            Assert.Less(
                diff, 0.1,
                "The difference between the initial duration and the sum of the 7 splitted ManagedAudioMedia is more than 0.1ms");
        }
    }
}
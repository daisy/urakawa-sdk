using System;
using NUnit.Framework;
using urakawa.media;

namespace urakawa.test.unitTests
{
	/// <summary>
	/// Summary description for MediaObjectTests.
	/// </summary>
	[TestFixture]
	public class AudioMediaTests
	{
		/// <summary>
		/// Assign clip begin and end values and see if the duration is correct
		/// Use milliseconds (long)
		/// </summary>
		[Test]public void CheckAudioDuration_SimpleMS()
		{
			MediaFactory factory = new MediaFactory();
			AudioMedia audio = (AudioMedia)factory.createMedia(MediaType.AUDIO);

			audio.setClipBegin(new Time(0));
			audio.setClipEnd(new Time(1000));

			TimeDelta td = (TimeDelta)audio.getDuration();

			Assert.AreEqual(1000, td.getTimeDeltaAsMilliseconds());
		}

		/// <summary>
		/// Split the audio node and check the new end time
		/// Use milliseconds (long)
		/// </summary>
		[Test]public void SplitAudioNodeCheckNewTimes_SimpleMS()
		{
			MediaFactory factory = new MediaFactory();
			AudioMedia audio = (AudioMedia)factory.createMedia(MediaType.AUDIO);

			audio.setClipBegin(new Time(0));
			audio.setClipEnd(new Time(1000));

			AudioMedia new_audio = (AudioMedia)audio.split(new Time(600));

			//check begin/end times for original node
			Time t = (Time)audio.getClipBegin();
			Assert.AreEqual(0, t.getAsMilliseconds());

			t = (Time)audio.getClipEnd();
			Assert.AreEqual(600, t.getAsMilliseconds());

			//check begin/end times for newly created node
			t = (Time)new_audio.getClipBegin();
			Assert.AreEqual(600, t.getAsMilliseconds());

			t = (Time)new_audio.getClipEnd();
			Assert.AreEqual(1000, t.getAsMilliseconds());

		}

		/// <summary>
		/// Split the audio node and check the new duration
		/// Use milliseconds (long)
		/// </summary>
		[Test]public void SplitAudioCheckNewDuration_SimpleMS()
		{
			MediaFactory factory = new MediaFactory();
			AudioMedia audio = (AudioMedia)factory.createMedia(MediaType.AUDIO);

			audio.setClipBegin(new Time(0));
			audio.setClipEnd(new Time(1000));

			AudioMedia new_audio = (AudioMedia)audio.split(new Time(600));

			TimeDelta td_1 = (TimeDelta)audio.getDuration();
			TimeDelta td_2 = (TimeDelta)new_audio.getDuration();
			
			Assert.AreEqual(600, td_1.getTimeDeltaAsMilliseconds());
			Assert.AreEqual(400, td_2.getTimeDeltaAsMilliseconds());	
		}

		/// <summary>
		/// 1. set the location and check that it has been set correctly
		/// 2. set to a different location, using a different MediaLocation constructor,
		/// and see that a. it is correct and b. it is not the same as the previous location
		/// </summary>
		[Test]public void setAndGetMediaLocation()
		{
			string src = "myfile.ext";
			string src2 = "myotherfile.ext";
			
			MediaFactory factory = new MediaFactory();
			AudioMedia audio = (AudioMedia)factory.createMedia(MediaType.AUDIO);

			MediaLocation loc = new MediaLocation(src);
			MediaLocation loc2 = new MediaLocation();
			loc2.mLocation = src2;

			audio.setLocation(loc);

			Assert.AreSame(loc.mLocation, src);

			audio.setLocation(loc2);

			Assert.AreNotSame(loc, loc2);
			Assert.AreSame(loc2.mLocation, src2);
		}

		[Test]public void checkTypeAfterCopy()
		{
			MediaFactory factory = new MediaFactory();
			AudioMedia audio = (AudioMedia)factory.createMedia(MediaType.AUDIO);

			AudioMedia audio_copy = audio.copy();

			Assert.AreEqual(audio_copy.getType(), MediaType.AUDIO);
		}

    [Test]public void checkAudioMediaCopy()
    {
      MediaFactory factory = new MediaFactory();
      AudioMedia audio = (AudioMedia)factory.createMedia(MediaType.AUDIO);
      bool exceptionOccured = false;
      try
      {
        IMedia audio_copy = ((IMedia)audio).copy();
      }
      catch (exception.OperationNotValidException)
      {
        exceptionOccured = true;
      }
      Assert.IsFalse(
        exceptionOccured, 
        "IMedia.copy() method was probably not overridden in AudioMedia subclass of ExternalMedia");
    }

		/// <summary>
		/// Check that the media node is reporting its static properties correctly
		/// </summary>
		[Test]public void checkTheFacts()
		{
			MediaFactory factory = new MediaFactory();
			AudioMedia audio = (AudioMedia)factory.createMedia(MediaType.AUDIO);

			Assert.AreEqual(audio.isContinuous(), true);
			Assert.AreEqual(audio.isDiscrete(), false);
			Assert.AreEqual(audio.isSequence(), false);
			Assert.AreEqual(audio.getType(), MediaType.AUDIO);
		}
	}
}

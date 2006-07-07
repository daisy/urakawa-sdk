using System;
using NUnit.Framework;
using urakawa.media;

namespace urakawa.test.unitTests
{
	/// <summary>
	/// Test media objects
	/// </summary>
	[TestFixture]
	public class MediaObjectTests
	{
		private MediaFactory factory;

		[SetUp]public void Init()
		{
			factory = new MediaFactory();
		}

		/// <summary>
		/// Assign clip begin and end values and see if the duration is correct
		/// Use milliseconds (long)
		/// </summary>
		[Test]public void CheckAudioDuration_SimpleMS()
		{
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
		[Test]public void SplitAudioObjectCheckNewTimes_SimpleMS()
		{
			AudioMedia obj = (AudioMedia)factory.createMedia(MediaType.AUDIO);

			obj.setClipBegin(new Time(0));
			obj.setClipEnd(new Time(1000));

			AudioMedia new_obj = (AudioMedia)obj.split(new Time(600));

			//check begin/end times for original node
			Time t = (Time)obj.getClipBegin();
			Assert.AreEqual(0, t.getAsMilliseconds());

			t = (Time)obj.getClipEnd();
			Assert.AreEqual(600, t.getAsMilliseconds());

			//check begin/end times for newly created node
			t = (Time)new_obj.getClipBegin();
			Assert.AreEqual(600, t.getAsMilliseconds());

			t = (Time)new_obj.getClipEnd();
			Assert.AreEqual(1000, t.getAsMilliseconds());

		}

		/// <summary>
		/// Split the audio node and check the new duration
		/// Use milliseconds (long)
		/// </summary>
		[Test]public void SplitVideoObjectCheckNewDuration_SimpleMS()
		{
			VideoMedia obj = (VideoMedia)factory.createMedia(MediaType.VIDEO);

			obj.setClipBegin(new Time(0));
			obj.setClipEnd(new Time(1000));

			VideoMedia new_obj = (VideoMedia)obj.split(new Time(600));

			TimeDelta td_1 = (TimeDelta)obj.getDuration();
			TimeDelta td_2 = (TimeDelta)new_obj.getDuration();
			
			Assert.AreEqual(600, td_1.getTimeDeltaAsMilliseconds());
			Assert.AreEqual(400, td_2.getTimeDeltaAsMilliseconds());	
		}

		/// <summary>
		/// 1. set the location and check that it has been set correctly
		/// 2. set to a different location, using a different MediaLocation constructor,
		/// and see that a. it is correct and b. it is not the same as the previous location
		/// </summary>
		[Test]public void setAndGetImageMediaLocation()
		{
			string src = "myfile.ext";
			string src2 = "myotherfile.ext";
			
			ImageMedia obj = (ImageMedia)factory.createMedia(MediaType.IMAGE);

			MediaLocation loc = new MediaLocation(src);
			MediaLocation loc2 = new MediaLocation();
			loc2.mLocation = src2;

			obj.setLocation(loc);

			Assert.AreSame(loc.mLocation, src);

			obj.setLocation(loc2);

			Assert.AreNotSame(loc, loc2);
			Assert.AreSame(loc2.mLocation, src2);
		}

		[Test]public void checkTypeAfterCopy()
		{
			AudioMedia audio = (AudioMedia)factory.createMedia(MediaType.AUDIO);

			AudioMedia audio_copy = audio.copy();

			Assert.AreEqual(audio_copy.getType(), MediaType.AUDIO);
		}

    [Test]public void checkAudioMediaCopy()
    {
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
		[Test]public void checkAudioMediaStaticProperties()
		{
			AudioMedia obj = (AudioMedia)factory.createMedia(MediaType.AUDIO);

			Assert.AreEqual(obj.isContinuous(), true);
			Assert.AreEqual(obj.isDiscrete(), false);
			Assert.AreEqual(obj.isSequence(), false);
			Assert.AreEqual(obj.getType(), MediaType.AUDIO);
		}

		/// <summary>
		/// see if a new empty sequence is indeed empty and has the right type
		/// </summary>
		[Test]public void isEmptySequenceReallyEmpty()
		{
			SequenceMedia obj = (SequenceMedia)factory.createMedia(MediaType.EMPTY_SEQUENCE);

			Assert.AreEqual(MediaType.EMPTY_SEQUENCE, obj.getType());
			Assert.AreEqual(true, obj.isSequence());
			Assert.AreEqual(0, obj.getCount());
			Assert.AreEqual(false, obj.isContinuous());
			Assert.AreEqual(false, obj.isDiscrete());
		}
	}
}

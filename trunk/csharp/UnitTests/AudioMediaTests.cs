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
	}
}

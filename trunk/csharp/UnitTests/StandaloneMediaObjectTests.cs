using System;
using NUnit.Framework;
using urakawa.media;
using urakawa.media.timing;

namespace urakawa.unitTests.fixtures.standalone
{
	/// <summary>
	/// Test media objects
	/// </summary>
	[TestFixture]
	public class StandaloneMediaObjectTests
	{
		private Presentation pres;
		private IMediaFactory factory;

		[SetUp]
		public void Init()
		{
			pres = new Presentation(new Uri(System.IO.Directory.GetCurrentDirectory()));
			factory = pres.getMediaFactory();
		}

		/// <summary>
		/// Assign clip begin and end values and see if the duration is correct
		/// Use milliseconds (long)
		/// </summary>
		[Test]
		public void CheckAudioDuration_SimpleMS()
		{
			ExternalAudioMedia audio = (ExternalAudioMedia)factory.createMedia(
				typeof(ExternalAudioMedia).Name, ToolkitSettings.XUK_NS);

			audio.setClipBegin(new Time(0));
			audio.setClipEnd(new Time(1000));

			TimeDelta td = (TimeDelta)audio.getDuration();

			Assert.AreEqual(1000, td.getTimeDeltaAsMilliseconds());
		}

		/// <summary>
		/// Split the audio node and check the new end time
		/// Use milliseconds (long)
		/// </summary>
		[Test]
		public void SplitAudioObjectCheckNewTimes_SimpleMS()
		{
			ExternalAudioMedia obj = (ExternalAudioMedia)factory.createMedia(
				typeof(ExternalAudioMedia).Name, ToolkitSettings.XUK_NS);

			obj.setClipBegin(new Time(0));
			obj.setClipEnd(new Time(1000));

			ExternalAudioMedia new_obj = obj.split(new Time(600));

			//check begin/end times for original node
			Time t = (Time)obj.getClipBegin();
			Assert.AreEqual(0, t.getTimeAsMilliseconds());

			t = (Time)obj.getClipEnd();
			Assert.AreEqual(600, t.getTimeAsMilliseconds());

			//check begin/end times for newly created node
			t = (Time)new_obj.getClipBegin();
			Assert.AreEqual(600, t.getTimeAsMilliseconds());

			t = (Time)new_obj.getClipEnd();
			Assert.AreEqual(1000, t.getTimeAsMilliseconds());

		}

		/// <summary>
		/// Split the audio node and check the new duration
		/// Use milliseconds (long)
		/// </summary>
		[Test]
		public void SplitVideoObjectCheckNewDuration_SimpleMS()
		{
			IVideoMedia obj = (IVideoMedia)factory.createMedia(MediaType.VIDEO);

			obj.setClipBegin(new Time(0));
			obj.setClipEnd(new Time(1000));

			IVideoMedia new_obj = (IVideoMedia)obj.split(new Time(600));

			ITimeDelta td_1 = obj.getDuration();
			ITimeDelta td_2 = new_obj.getDuration();

			Assert.AreEqual(600, td_1.getTimeDeltaAsMilliseconds());
			Assert.AreEqual(400, td_2.getTimeDeltaAsMilliseconds());
		}

		/// <summary>
		/// 1. set the src location and check that it has been set correctly
		/// 2. set to a different src location, using a different string,
		/// and see that a. it is correct and b. it is not the same as the previous location
		/// </summary>
		[Test]
		public void setAndGetImageMediaLocation()
		{
			string src = "myfile.ext";
			string src2 = "myotherfile.ext";

			IImageMedia obj = (IImageMedia)factory.createMedia(MediaType.IMAGE);

			obj.setSrc(src);

			Assert.AreEqual(obj.getSrc(), src);

			obj.setSrc(src2);

			Assert.AreNotEqual(src, src2);
			Assert.AreEqual(obj.getSrc(), src2);
		}

		[Test]
		public void checkTypeAfterCopy()
		{
			IAudioMedia audio = (IAudioMedia)factory.createMedia(MediaType.AUDIO);

			IAudioMedia audio_copy = (IAudioMedia)audio.copy();

			Assert.AreEqual(audio_copy.getMediaType(), MediaType.AUDIO);
		}

		[Test]
		public void checkAudioMediaCopy()
		{
			ExternalAudioMedia audio = (ExternalAudioMedia)factory.createMedia(
				typeof(ExternalAudioMedia).Name, ToolkitSettings.XUK_NS);
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
		[Test]
		public void checkAudioMediaStaticProperties()
		{
			ExternalAudioMedia obj = (ExternalAudioMedia)factory.createMedia(
				typeof(ExternalAudioMedia).Name, ToolkitSettings.XUK_NS);
			Assert.AreEqual(obj.isContinuous(), true);
			Assert.AreEqual(obj.isDiscrete(), false);
			Assert.AreEqual(obj.isSequence(), false);
			Assert.AreEqual(obj.getMediaType(), MediaType.AUDIO);
		}

		/// <summary>
		/// see if a new empty sequence is indeed empty and has the right type
		/// </summary>
		[Test]
		public void isEmptySequenceReallyEmpty()
		{
			SequenceMedia obj = (SequenceMedia)factory.createMedia(MediaType.EMPTY_SEQUENCE);

			Assert.AreEqual(MediaType.EMPTY_SEQUENCE, obj.getMediaType());
			Assert.AreEqual(true, obj.isSequence());
			Assert.AreEqual(0, obj.getCount());
			Assert.AreEqual(false, obj.isContinuous());
			Assert.AreEqual(false, obj.isDiscrete());
		}

		/// <summary>
		/// See if the SequenceMedia object will throw an illegal type exception
		/// when we try to add different media types to it
		/// Also check that it contains the correct number of objects after the append fails
		/// And see that its type is correctly reported.
		/// </summary>
		[Test]
		[ExpectedException(typeof(exception.MediaTypeIsIllegalException))]
		public void canSequenceMediaHoldOnlyOneMediaType()
		{
			ISequenceMedia obj = (ISequenceMedia)factory.createMedia(MediaType.EMPTY_SEQUENCE);

			IAudioMedia audio_obj = (IAudioMedia)factory.createMedia(MediaType.AUDIO);
			ITextMedia text_obj = (ITextMedia)factory.createMedia(MediaType.TEXT);

			obj.insertItem(obj.getCount(), audio_obj);

			obj.insertItem(obj.getCount(), text_obj);

			//make sure there is only one item in the sequence right now
			Assert.AreEqual(1, obj.getCount());

			//make sure the sequence has the correct type
			Assert.AreEqual(MediaType.AUDIO, obj.getMediaType());
		}

		/// <summary>
		/// This test checks that if you change the text on a TextMedia copy
		/// that the original (source) TextMedia object does not have its
		/// text changed as well.
		/// </summary>
		[Test]
		public void CopyTextMediaRenameAndCheckAgain()
		{
			ITextMedia text_obj = (ITextMedia)factory.createMedia(MediaType.TEXT);
			text_obj.setText("original media object");

			ITextMedia copy_obj = (ITextMedia)text_obj.copy();

			copy_obj.setText("copied media object");

			Assert.AreNotEqual(text_obj.getText(), copy_obj.getText());
		}

		[Test]
		public void PlainTextMediaGetTextHttpTest()
		{
			TestPlainTextMediaGetText("http://www.daisy.org/z3986/2005/ncx-2005-1.dtd", "<!-- NCX 2005-1 DTD  2005-06-26");
		}

		[Test]
		public void PlainTextMediaGetTextFileTest()
		{
			TestPlainTextMediaGetText("../XukWorks/xuk.xsd", "<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
		}

		private void TestPlainTextMediaGetText(string uri, string expectedStartOfFile)
		{
			ExternalTextMedia text_obj = (ExternalTextMedia)factory.createMedia(
				typeof(ExternalTextMedia).Name, ToolkitSettings.XUK_NS);
			text_obj.setSrc(uri);
			string text = text_obj.getText();
			Assert.IsTrue(text.StartsWith(expectedStartOfFile), "The file at uri {0} did not start with '{1}'", uri, expectedStartOfFile);
		}

		[Test]
		public void PlainTextMediaSetTextFileTest()
		{
			ExternalTextMedia text_obj = (ExternalTextMedia)factory.createMedia(
				typeof(ExternalTextMedia).Name, ToolkitSettings.XUK_NS);
			text_obj.setSrc("temp.txt");
			string text = "Test textual content\næøåÆØÅ@£€";
			text_obj.setText(text);
			TestPlainTextMediaGetText(text_obj.getSrc(), text);
			text = text + "\nAppended this";
			text_obj.setText(text);
			TestPlainTextMediaGetText(text_obj.getSrc(), text);
			Uri tempFileUri = new Uri(factory.getPresentation().getBaseUri(), text_obj.getSrc());
			System.IO.File.Delete(tempFileUri.LocalPath);
		}

		[Test]
		[ExpectedException(typeof(exception.OperationNotValidException))]
		public void PlainTextMediaSetTextHttpTest()
		{
			ExternalTextMedia text_obj = (ExternalTextMedia)factory.createMedia(
				typeof(ExternalTextMedia).Name, ToolkitSettings.XUK_NS);
			text_obj.setSrc("http://www.daisy.org/z3986/2005/ncx-2005-1.dtd");
			text_obj.setText("Oops, I replaced the Z39.86-2005 version 1 NCX DTD");
		}
	}
}

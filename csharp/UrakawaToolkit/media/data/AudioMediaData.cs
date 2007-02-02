using System;
using System.Collections.Generic;
using System.Text;
using urakawa.media.timing;

namespace urakawa.media.data
{
	/// <summary>
	/// Abstract implementation of interface <see cref="IAudioMediaData"/>.
	/// Implements PCM format accessors (number of channels, bit depth, sample rate) 
	/// and leaves all other methods abstract
	/// </summary>
	public abstract class AudioMediaData : IAudioMediaData
	{

		#region IAudioMediaData Members

		private int mNumberOfChannels;

		/// <summary>
		/// Gets the number of channels of audio
		/// </summary>
		/// <returns>The number of channels</returns>
		public int getNumberOfChannels()
		{
			return mNumberOfChannels;
		}

		/// <summary>
		/// Sets the number of channels of audio
		/// </summary>
		/// <param name="newNumberOfChannels">The new number of channels</param>
		/// <exception cref="exception.MethodParameterIsOutOfBoundsException">
		/// Thrown when the new number of channels is not positive or in derived classes when otherwise out of bounds
		/// </exception>
		public virtual void setNumberOfChannels(int newNumberOfChannels)
		{
			if (newNumberOfChannels < 1)
			{
				throw new exception.MethodParameterIsOutOfBoundsException(
					"The number of channels must be positive");
			}
			mNumberOfChannels = newNumberOfChannels;
		}

		private int mBitDepth;

		/// <summary>
		/// Gets the number of bits used to store each sample of audio data
		/// </summary>
		/// <returns>The bit depth</returns>
		public int getBitDepth()
		{
			return mBitDepth;
		}

		/// <summary>
		/// Sets the number of bits used to store each sample of audio data
		/// </summary>
		/// <param name="newBitDepth">The new bit depth</param>
		/// <exception cref="exception.MethodParameterIsOutOfBoundsException">
		/// Thrown when the new bit depth is not positive or in derived classes when otherwise out of bounds
		/// </exception>
		public virtual void setBitDepth(int newBitDepth)
		{
			if (newBitDepth < 1)
			{
				throw new exception.MethodParameterIsOutOfBoundsException(
					"The bit depth must be positive");
			}
			mBitDepth = newBitDepth;
		}

		private int mSampleRate;

		/// <summary>
		/// Gets the sample rate of the audio data
		/// </summary>
		/// <returns>The sample rate in Hz</returns>
		public int getSampleRate()
		{
			return mSampleRate;
		}

		/// <summary>
		/// Sets the sample rate of the audio data
		/// </summary>
		/// <param name="newSampleRate">The new sample rate</param>
		/// <exception cref="exception.MethodParameterIsOutOfBoundsException">
		/// Thrown when the new sample rate is not positive or in derived classes when otherwise out of bounds
		/// </exception>
		public void setSampleRate(int newSampleRate)
		{
			if (newSampleRate < 1)
			{
				throw new exception.MethodParameterIsOutOfBoundsException("The sample rate must be positive");
			}
			mSampleRate = newSampleRate;
		}


		/// <summary>
		/// Gets the intrinsic duration of the audio data
		/// </summary>
		/// <returns>The duration as an <see cref="ITimeDelta"/></returns>
		public abstract ITimeDelta getAudioDuration();


		/// <summary>
		/// Gets an input <see cref="Stream"/> giving access to all audio data as raw PCM
		/// </summary>
		/// <returns>The input <see cref="Stream"/></returns>
		public abstract System.IO.Stream getAudioData();

		/// <summary>
		/// Gets an input <see cref="Stream"/> giving access to the audio data after a given <see cref="ITime"/> 
		/// as raw PCM
		/// </summary>
		/// <returns>The input <see cref="Stream"/></returns>
		public abstract System.IO.Stream getAudioData(ITime clipBegin);

		public abstract System.IO.Stream getAudioData(urakawa.media.timing.ITime clipBegin, urakawa.media.timing.ITime clipEnd);

		public abstract void appendAudioData(System.IO.Stream pcmData, ITimeDelta duration);

		public abstract void insertAudioData(System.IO.Stream pcmData, ITime insertPoint, urakawa.media.timing.ITimeDelta duration);

		public abstract void replaceAudioData(System.IO.Stream pcmData, ITime replacePoint, ITimeDelta duration);

		#endregion

		#region IMediaData Members

		/// <summary>
		/// Protected member that stores the <see cref="IMediaDataManager"/> associated with <c>this</c>.
		/// Must be assigned at creation.
		/// </summary>
		protected IMediaDataManager mMediaDataManager;

		/// <summary>
		/// Gets the <see cref="IMediaDataManager"/> associated with <c>this</c>
		/// </summary>
		/// <returns>The associated <see cref="IMediaDataManager"/></returns>
		public IMediaDataManager getDataManager()
		{
			return mMediaDataManager;
		}

		/// <summary>
		/// Gets the UID of <c>this</c>.
		/// Convenience for <c><see cref="getDataManager"/>().<see cref="IMediaDataManager.getUidOfMediaData"/>(this)</c>
		/// </summary>
		/// <returns>The UID</returns>
		public string getUid()
		{
			return getDataManager().getUidOfMediaData(this);
		}

		public string getName()
		{
			throw new Exception("The method or operation is not implemented.");
//TODO: Implement method
		}

		public void setName(string newName)
		{
			throw new Exception("The method or operation is not implemented.");
//TODO: Implement method
		}

		public abstract IAudioMediaData copy();

		IMediaData IMediaData.copy()
		{
			return copy();
		}
		
		#endregion

		#region IXukAble Members


		public abstract bool XukIn(System.Xml.XmlReader source);

		public abstract bool XukOut(System.Xml.XmlWriter destination);

		/// <summary>
		/// Gets the local name part of the QName representing the class in Xuk
		/// </summary>
		/// <returns>The local name part</returns>
		public virtual string getXukLocalName()
		{
			return this.GetType().Name;
		}

		/// <summary>
		/// Gets the namespace uri part of the QName representing the class in Xuk
		/// </summary>
		/// <returns>The namespace uri part</returns>
		public virtual string getXukNamespaceUri()
		{
			return ToolkitSettings.XUK_NS;
		}

		#endregion
	}
}

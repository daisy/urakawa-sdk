using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using urakawa.xuk;
using urakawa.media.timing;

namespace urakawa.media.data.audio
{
	/// <summary>
	/// Represents information describing a RAW PCM format
	/// </summary>
	public class PCMFormatInfo : IValueEquatable<PCMFormatInfo>, IXukAble
	{
		/// <summary>
		/// Fires when the PCM format has changed
		/// </summary>
		public event EventHandler FormatChanged;

		private void FireFormatChanged()
		{
			if (FormatChanged!=null) FormatChanged(this, new EventArgs());
		}

		/// <summary>
		/// Default constructor
		/// </summary>
		public PCMFormatInfo()
		{
			setNumberOfChannels(1);
			setSampleRate(44100);
			setBitDepth(16);
		}

		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <param name="other">The PCMFormatInfo to copy</param>
		public PCMFormatInfo(PCMFormatInfo other)
		{
			setNumberOfChannels(other.getNumberOfChannels());
			setSampleRate(other.getSampleRate());
			setBitDepth(other.getBitDepth());
		}

		private ushort mNumberOfChannels = 1;

		/// <summary>
		/// Gets the number of channels of audio
		/// </summary>
		public ushort getNumberOfChannels()
		{
			return mNumberOfChannels;
		}

		/// <summary>
		/// Sets the number of channels of audio
		/// </summary>
		public virtual void setNumberOfChannels(ushort newValue)
		{
			if (newValue < 1)
			{
				throw new exception.MethodParameterIsOutOfBoundsException(
					"Minimum number of channels is 1");
			}
			if (mNumberOfChannels != newValue)
			{
				mNumberOfChannels = newValue;
				FireFormatChanged();
			}
		}

		private uint mSampleRate = 44100;

		/// <summary>
		/// Gets the sample rate in Hz of the audio
		/// </summary>
		public uint getSampleRate()
		{
			return mSampleRate;
		}

		/// <summary>
		/// Sets the sample rate in Hz of the audio
		/// </summary>
		public virtual void setSampleRate(uint newValue)
		{
			if (mSampleRate < 1)
			{
				throw new exception.MethodParameterIsOutOfBoundsException(
					"Sample rate must be positive");
			}
			if (mSampleRate != newValue)
			{
				mSampleRate = newValue;
				FireFormatChanged();
			}
		}

		private ushort mBitDepth = 16;
		/// <summary>
		/// Gets the depth in bits of the audio, ie. the size in bits of each sample of audio
		/// </summary>
		public ushort getBitDepth()
		{
			return mBitDepth;
		}

		/// <summary>
		/// Sets the depth in bits of the audio, ie. the size in bits of each sample of audio
		/// </summary>
		public virtual void setBitDepth(ushort newValue)
		{
			if ((newValue % 8) != 0)
			{
				throw new exception.MethodParameterIsOutOfBoundsException(
					"Bit depth must be a multiple of 8");
			}
			if (newValue < 8)
			{
				throw new exception.MethodParameterIsOutOfBoundsException(
					"Bit depth must be a least 8");
			}
			if (mBitDepth != newValue)
			{
				mBitDepth = newValue;
				FireFormatChanged();
			}
		}


		/// <summary>
		/// Gets the byte rate of the raw PCM data
		/// </summary>
		public uint getByteRate()
		{
			return getNumberOfChannels() * getSampleRate() * getBitDepth() / 8U;
		}

		/// <summary>
		/// Gets the size in bytes of a single block (i.e. a sample from each channel)
		/// </summary>
		public ushort getBlockAlign()
		{
			return (ushort)(getNumberOfChannels() * (getBitDepth() / 8));
		}

		/// <summary>
		/// Determines if the <see cref="PCMFormatInfo"/> is compatible with a given other <see cref="PCMDataInfo"/>
		/// </summary>
		/// <param name="pcmInfo">The other PCMDataInfo</param>
		/// <returns>A <see cref="bool"/> indicating the compatebility</returns>
		public bool isCompatibleWith(PCMFormatInfo pcmInfo)
		{
			if (pcmInfo == null) return false;
			if (getNumberOfChannels() != pcmInfo.getNumberOfChannels()) return false;
			if (getSampleRate() != pcmInfo.getSampleRate()) return false;
			if (getBitDepth() != pcmInfo.getBitDepth()) return false;
			return true;
		}

		/// <summary>
		/// Gets the duration of PCM data in the format of a given length
		/// </summary>
		/// <param name="dataLen">The length</param>
		/// <returns>The duration</returns>
		public TimeDelta getDuration(uint dataLen)
		{
			if (getByteRate() == 0)
			{
				throw new exception.InvalidDataFormatException("The PCM data has byte rate 0");
			}
			int seconds = 0;
			uint br = getByteRate();
			while (dataLen > br)
			{
				seconds++;
				dataLen -= br;
			}
			double restSecs = ((double)dataLen) / ((double)br);
			long ticks = 
				TimeSpan.TicksPerSecond * seconds 
				+ (long)Math.Round(restSecs * TimeSpan.TicksPerSecond);
			TimeSpan dur = TimeSpan.FromTicks(ticks);
			return new TimeDelta(dur);
		}

		/// <summary>
		/// Gets the PCM data length corresponding to a given duration
		/// </summary>
		/// <param name="duration">The given duration</param>
		/// <returns>The PCM data length</returns>
		public uint getDataLength(TimeDelta duration)
		{
			uint blockCount = (uint)((duration.getTimeDeltaAsTimeSpan().Ticks * getSampleRate())/(TimeSpan.TicksPerSecond));
			uint res = blockCount * getBlockAlign();
			return res;
		}

		
		#region IXUKAble members

		/// <summary>
		/// Reads the <see cref="PCMFormatInfo"/> from a PCMFormatInfo xuk element
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		public void XukIn(XmlReader source)
		{
			if (source == null)
			{
				throw new exception.MethodParameterIsNullException("Can not XukIn from an null source XmlReader");
			}
			if (source.NodeType != XmlNodeType.Element)
			{
				throw new exception.XukException("Can not read PCMFormatInfo from a non-element node");
			}
			try
			{
				XukInAttributes(source);
				if (!source.IsEmptyElement)
				{
					while (source.Read())
					{
						if (source.NodeType == XmlNodeType.Element)
						{
							XukInChild(source);
						}
						else if (source.NodeType == XmlNodeType.EndElement)
						{
							break;
						}
						if (source.EOF) throw new exception.XukException("Unexpectedly reached EOF");
					}
				}

			}
			catch (exception.XukException e)
			{
				throw e;
			}
			catch (Exception e)
			{
				throw new exception.XukException(
					String.Format("An exception occured during XukIn of PCMFormatInfo: {0}", e.Message),
					e);
			}
		}

		/// <summary>
		/// Reads the attributes of a PCMFormatInfo xuk element.
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected virtual void XukInAttributes(XmlReader source)
		{
			string attr = source.GetAttribute("NumberOfChannels");
			if (attr==null)
			{
				throw new exception.XukException("Attribute NumberOfChannels is missing");
			}
			ushort noc;
			if (!UInt16.TryParse(attr, out noc))
			{
				throw new exception.XukException(String.Format(
					"Attribute NumberOfChannels value {0} is not an unsigned short integer",
					attr));
			}
			setNumberOfChannels(noc);
			uint sr;
			attr = source.GetAttribute("SampleRate");
			if (attr == null)
			{
				throw new exception.XukException("Attribute SampleRate is missing");
			}
			if (!UInt32.TryParse(attr, out sr))
			{
				throw new exception.XukException(String.Format(
					"Attribute SampleRate value {0} is not an unsigned integer",
					attr));

			}
			setSampleRate(sr);
			ushort bd;
			attr = source.GetAttribute("BitDepth");
			if (attr == null)
			{
				throw new exception.XukException("Attribute BitDepth is missing");
			}
			if (!UInt16.TryParse(attr, out bd))
			{
				throw new exception.XukException(String.Format(
					"Attribute BitDepth value {0} is not an unsigned short integer",
					attr));

			}
			setBitDepth(bd);
		}

		/// <summary>
		/// Reads a child of a PCMFormatInfo xuk element. 
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected virtual void XukInChild(XmlReader source)
		{
			bool readItem = false;
			// Read known children, when read set readItem to true


			if (!(readItem || source.IsEmptyElement))
			{
				source.ReadSubtree().Close();//Read past unknown child 
			}
		}

		/// <summary>
		/// Write a PCMFormatInfo element to a XUK file representing the <see cref="PCMFormatInfo"/> instance
		/// </summary>
		/// <param localName="destination">The destination <see cref="XmlWriter"/></param>
		public void XukOut(XmlWriter destination)
		{
			if (destination == null)
			{
				throw new exception.MethodParameterIsNullException(
					"Can not XukOut to a null XmlWriter");
			}

			try
			{
				destination.WriteStartElement(getXukLocalName(), getXukNamespaceUri());
				XukOutAttributes(destination);
				XukOutChildren(destination);
				destination.WriteEndElement();

			}
			catch (exception.XukException e)
			{
				throw e;
			}
			catch (Exception e)
			{
				throw new exception.XukException(
					String.Format("An exception occured during XukOut of PCMFormatInfo: {0}", e.Message),
					e);
			}
		}

		/// <summary>
		/// Writes the attributes of a PCMFormatInfo element
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		protected virtual void XukOutAttributes(XmlWriter destination)
		{
			destination.WriteAttributeString("NumberOfChannels", getNumberOfChannels().ToString());
			destination.WriteAttributeString("SampleRate", getSampleRate().ToString());
			destination.WriteAttributeString("BitDepth", getBitDepth().ToString());
		}

		/// <summary>
		/// Write the child elements of a PCMFormatInfo element.
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		protected virtual void XukOutChildren(XmlWriter destination)
		{

		}

		/// <summary>
		/// Gets the local name part of the QName representing a <see cref="PCMFormatInfo"/> in Xuk
		/// </summary>
		/// <returns>The local name part</returns>
		public virtual string getXukLocalName()
		{
			return this.GetType().Name;
		}

		/// <summary>
		/// Gets the namespace uri part of the QName representing a <see cref="PCMFormatInfo"/> in Xuk
		/// </summary>
		/// <returns>The namespace uri part</returns>
		public virtual string getXukNamespaceUri()
		{
			return urakawa.ToolkitSettings.XUK_NS;
		}

		#endregion


		#region IValueEquatable<PCMFormatInfo> Members

		/// <summary>
		/// Determines if <c>this</c> has the same value as a given other <see cref="PCMFormatInfo"/>
		/// </summary>
		/// <param name="other">The given other PCMFormatInfo with which to compare</param>
		/// <returns>A <see cref="bool"/> indicating value equality</returns>
		public bool valueEquals(PCMFormatInfo other)
		{
			if (other == null) return false;
			if (other.GetType() != GetType()) return false;
			if (!isCompatibleWith(other)) return false;
			return true;
		}

		#endregion

		
	}

}

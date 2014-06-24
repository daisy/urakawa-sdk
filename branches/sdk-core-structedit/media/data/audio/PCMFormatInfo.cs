using System;
using System.Xml;
using AudioLib;
using urakawa.xuk;

namespace urakawa.media.data.audio
{
    [XukNameUglyPrettyAttribute("PCMInf", "PCMFormatInfo")]
    public class PCMFormatInfo : XukAble, IValueEquatable<PCMFormatInfo>
    {
        public override bool PrettyFormat
        {
            set { throw new NotImplementedException("PrettyFormat"); }
            get
            {
                return XukAble.m_PrettyFormat_STATIC;
            }
        }

        private AudioLibPCMFormat m_Data;
        public AudioLibPCMFormat Data
        {
            get
            {
                return m_Data;
            }
        }

        public override string ToString()
        {
            return Data.BitDepth + " bits " + String.Format("{0:0.###}", Data.SampleRate / 1000.0) + " KHz " + (Data.NumberOfChannels == 1 ? "Mono" : (Data.NumberOfChannels == 2 ? "Stereo" : "" + Data.NumberOfChannels));
        }
        
        /// <summary>
        /// Default constructor
        /// </summary>
        public PCMFormatInfo()
            : this(1, 44100, 16)
        {
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="other">The PCMFormatInfo to copy</param>
        public PCMFormatInfo(PCMFormatInfo other)
            : this(other.Data)
        {
        }

        /// <summary>
        /// Constructor initializing the <see cref="PCMFormatInfo"/> with given number of channels, sample rate and bit depth value
        /// </summary>
        /// <param name="noc">The given number of channels value</param>
        /// <param name="sr">The given sample rate value</param>
        /// <param name="bd">The given bit depth value</param>
        public PCMFormatInfo(ushort noc, uint sr, ushort bd)
        {
            m_Data = new AudioLibPCMFormat(noc, sr, bd);
        }

        public PCMFormatInfo(AudioLibPCMFormat format)
            : this(format.NumberOfChannels, format.SampleRate, format.BitDepth)
        {
        }


        /// <summary>
        /// Create a copy of the <see cref="PCMFormatInfo"/>
        /// </summary>
        /// <returns>The copy</returns>
        public PCMFormatInfo Copy()
        {
            return new PCMFormatInfo(this);
        }

        #region IXUKAble members

        /// <summary>
        /// Reads the attributes of a PCMFormatInfo xuk element.
        /// </summary>
        /// <param name="source">The source <see cref="XmlReader"/></param>
        protected override void XukInAttributes(XmlReader source)
        {
            base.XukInAttributes(source);

            string attr = source.GetAttribute(XukStrings.NumberOfChannels);
            if (attr == null)
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
            Data.NumberOfChannels = noc;
            uint sr;
            attr = source.GetAttribute(XukStrings.SampleRate);
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
            Data.SampleRate = sr;
            ushort bd;
            attr = source.GetAttribute(XukStrings.BitDepth);
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
            Data.BitDepth = bd;
        }

        /// <summary>
        /// Writes the attributes of a PCMFormatInfo element
        /// </summary>
        /// <param name="destination">The destination <see cref="XmlWriter"/></param>
        /// <param name="baseUri">
        /// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
        /// if <c>null</c> absolute <see cref="Uri"/>s are written
        /// </param>
        protected override void XukOutAttributes(XmlWriter destination, Uri baseUri)
        {
            base.XukOutAttributes(destination, baseUri);

            destination.WriteAttributeString(XukStrings.NumberOfChannels, Data.NumberOfChannels.ToString());
            destination.WriteAttributeString(XukStrings.SampleRate, Data.SampleRate.ToString());
            destination.WriteAttributeString(XukStrings.BitDepth, Data.BitDepth.ToString());
        }

        #endregion

        #region IValueEquatable<PCMFormatInfo> Members

        /// <summary>
        /// Determines if <c>this</c> has the same value as a given other <see cref="PCMFormatInfo"/>
        /// </summary>
        /// <param name="other">The given other PCMFormatInfo with which to compare</param>
        /// <returns>A <see cref="bool"/> indicating value equality</returns>
        public virtual bool ValueEquals(PCMFormatInfo other)
        {
            if (other == null)
            {
                //System.Diagnostics.Debug.Fail("! ValueEquals !"); 
                return false;
            }
            if (other.GetType() != GetType())
            {
                //System.Diagnostics.Debug.Fail("! ValueEquals !"); 
                return false;
            }
            if (!Data.IsCompatibleWith(other.Data))
            {
                //System.Diagnostics.Debug.Fail("! ValueEquals !"); 
                return false;
            }
            return true;
        }

        #endregion
    }
}
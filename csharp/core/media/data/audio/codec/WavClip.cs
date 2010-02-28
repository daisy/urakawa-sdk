using System;
using System.IO;
using AudioLib;
using urakawa.data;
using urakawa.media.timing;

namespace urakawa.media.data.audio.codec
{
    /// <summary>
    /// Represents a generic media clip
    /// </summary>
    public abstract class Clip
    {
        private Time mClipBegin = new Time();

        /// <summary>
        /// Gets (a copy of) the clip begin <see cref="Time"/> of <c>this</c>
        /// </summary>
        /// <returns>
        /// The clip begin <see cref="Time"/> - can not be <c>null</c>
        /// </returns>
        public Time ClipBegin
        {
            get { return mClipBegin.Copy(); }
            set
            {
                if (value == null)
                {
                    throw new exception.MethodParameterIsNullException("Clip begin of a WavClip can not be null");
                }
                if (value.IsGreaterThan(ClipEnd))
                {
                    throw new exception.MethodParameterIsOutOfBoundsException(
                        "The new clip begin is beyond the current clip end");
                }
                mClipBegin = value.Copy();
            }
        }

        private Time mClipEnd = null;

        /// <summary>
        /// Gets (a copy of) the clip end <see cref="Time"/> of <c>this</c>
        /// </summary>
        /// <returns>The clip end <see cref="Time"/></returns>
        public Time ClipEnd
        {
            get
            {
                if (mClipEnd == null) return Time.Zero.AddTimeDelta(MediaDuration);
                return mClipEnd.Copy();
            }
            set
            {
                if (value == null)
                {
                    mClipEnd = null;
                }
                else
                {
                    if (value.IsLessThan(ClipBegin))
                    {
                        throw new exception.MethodParameterIsOutOfBoundsException(
                            "The new clip end time is before current clip begin");
                    }
                    mClipEnd = value.Copy();
                }
            }
        }

        /// <summary>
        /// Determines if clip end is tied to the end of the underlying media
        /// </summary>
        /// <returns>
        /// A <see cref="bool"/> indicating if clip end is tied to the end of the underlying media
        /// </returns>
        public bool IsClipEndTiedToEOM
        {
            get { return (mClipEnd == null); }
        }

        /// <summary>
        /// Gets the duration of the clip
        /// </summary>
        /// <returns>The duration of as a <see cref="TimeDelta"/></returns>
        public TimeDelta Duration
        {
            get { return ClipEnd.GetTimeDelta(ClipBegin); }
        }

        /// <summary>
        /// Gets the duration of the underlying media
        /// </summary>
        /// <returns>The duration of the underlying media</returns>
        public abstract TimeDelta MediaDuration { get; }
    }

    /// <summary>
    /// Represents a RIFF WAVE PCM audio data clip
    /// </summary>
    public class WavClip : Clip, IValueEquatable<WavClip>
    {
        /// <summary>
        /// Constructor setting the <see cref="urakawa.data.DataProvider"/>, 
        /// clip begin and clip end will in this case be initialized to <c>null</c>,
        /// which means beginning/end if the RIFF WAVE PCM data
        /// </summary>
        /// <param name="clipDataProvider">The <see cref="urakawa.data.DataProvider"/></param>
        public WavClip(DataProvider clipDataProvider)
            : this(clipDataProvider, new Time(), null)
        {
        }

        /// <summary>
        /// Constructor setting the <see cref="urakawa.data.DataProvider"/> and clip begin/end values
        /// </summary>
        /// <param name="clipDataProvider">The <see cref="urakawa.data.DataProvider"/> - can not be <c>null</c></param>
        /// <param name="clipBegin">The clip begin <see cref="Time"/> - can not be <c>null</c></param>
        /// <param name="clipEnd">
        /// The clip end <see cref="Time"/>
        /// - a <c>null</c> value ties clip end to the end of the underlying wave audio</param>
        public WavClip(DataProvider clipDataProvider, Time clipBegin, Time clipEnd)
        {
            if (clipDataProvider == null)
            {
                throw new exception.MethodParameterIsNullException("The data provider of a WavClip can not be null");
            }
            mDataProvider = clipDataProvider;
            ClipBegin = clipBegin;
            ClipEnd = clipEnd;
        }

        private TimeDelta cachedDuration = null;

        /// <summary>
        /// Gets the duration of the underlying RIFF wav file 
        /// </summary>
        /// <returns>The duration</returns>
        public override TimeDelta MediaDuration
        {
            get
            {
                if (cachedDuration == null)
                {
                    Stream raw = DataProvider.OpenInputStream();

                    uint dataLength;
                    AudioLibPCMFormat format;
                    try
                    {
                        format = AudioLibPCMFormat.RiffHeaderParse(raw, out dataLength);
                    }
                    finally
                    {
                        raw.Close();
                    }
                    cachedDuration = new TimeDelta(format.ConvertBytesToTime(dataLength));
                }
                return cachedDuration;
            }
        }

        /// <summary>
        /// Creates a copy of the wav clip
        /// </summary>
        /// <returns>The copy</returns>
        public WavClip Copy()
        {
            Time clipEnd = null;
            if (!IsClipEndTiedToEOM) clipEnd = ClipEnd.Copy();
            //TODO: Check that sharing DataProvider with the copy is not a problem
            // REMARK: FileDataProviders: once created, binary content (including RIFF header) is never changed.
            // therefore, OPEN-only FileStream access should work concurrently (i.e. FileShare.Read)
            return new WavClip(DataProvider, ClipBegin.Copy(), clipEnd);
        }

        /// <summary>
        /// Exports the clip to a destination <see cref="Presentation"/>
        /// </summary>
        /// <param name="destPres">The destination <see cref="Presentation"/></param>
        /// <returns>The exported clip</returns>
        public WavClip Export(Presentation destPres)
        {
            Time clipEnd = null;
            if (!IsClipEndTiedToEOM) clipEnd = ClipEnd.Copy();
            return new WavClip(DataProvider.Export(destPres), ClipBegin.Copy(), clipEnd);
        }

        private DataProvider mDataProvider;

        /// <summary>
        /// Gets the <see cref="urakawa.data.DataProvider"/> storing the RIFF WAVE PCM audio data of <c>this</c>
        /// </summary>
        /// <returns>The <see cref="urakawa.data.DataProvider"/></returns>
        public DataProvider DataProvider
        {
            get { return mDataProvider; }
        }

        /// <summary>
        /// Gets an input <see cref="Stream"/> providing read access to the raw PCM audio data
        /// </summary>
        /// <returns>The raw PCM audio data <see cref="Stream"/></returns>
        public Stream OpenPcmInputStream()
        {
            return OpenPcmInputStream(Time.Zero);
        }

        /// <summary>
        /// Gets an input <see cref="Stream"/> providing read access to the raw PCM audio data
        /// after a given sub-clip begin time
        /// </summary>
        /// <param name="subClipBegin"></param>
        /// <returns>The raw PCM audio data <see cref="Stream"/></returns>
        /// <seealso cref="OpenPcmInputStream(urakawa.media.timing.Time,urakawa.media.timing.Time)"/>
        public Stream OpenPcmInputStream(Time subClipBegin)
        {
            return OpenPcmInputStream(subClipBegin, Time.Zero.AddTimeDelta(Duration));
        }

        /// <summary>
        /// Gets an input <see cref="Stream"/> providing read access to the raw PCM audio data
        /// between given sub-clip begin and end times
        /// </summary>
        /// <param name="subClipBegin">The beginning of the sub-clip</param>
        /// <param name="subClipEnd">The end of the sub-clip</param>
        /// <returns>The raw PCM audio data <see cref="Stream"/></returns>
        /// <remarks>
        /// <para>Sub-clip times must be in the interval <c>[0;this.getAudioDuration()]</c>.</para>
        /// <para>
        /// The sub-clip is
        /// relative to clip begin of the WavClip, that if <c>this.getClipBegin()</c>
        /// returns <c>00:00:10</c>, <c>this.getClipEnd()</c> returns <c>00:00:50</c>, 
        /// <c>x</c> and <c>y</c> is <c>00:00:05</c> and <c>00:00:30</c> respectively, 
        /// then <c>this.GetAudioData(x, y)</c> will get the audio in the underlying wave audio between
        /// <c>00:00:15</c> and <c>00:00:40</c>
        /// </para>
        /// </remarks>
        public Stream OpenPcmInputStream(Time subClipBegin, Time subClipEnd)
        {
            if (subClipBegin == null)
            {
                throw new exception.MethodParameterIsNullException("subClipBegin must not be null");
            }
            if (subClipEnd == null)
            {
                throw new exception.MethodParameterIsNullException("subClipEnd must not be null");
            }
            if (
                subClipBegin.IsLessThan(Time.Zero)
                || subClipEnd.IsLessThan(subClipBegin)
                || subClipEnd.IsGreaterThan(Time.Zero.AddTimeDelta(Duration))
                )
            {
                throw new exception.MethodParameterIsOutOfBoundsException(
                    "The interval [subClipBegin;subClipEnd] must be non-empty and contained in [0;GetDuration()]");
            }

            Stream raw = DataProvider.OpenInputStream();
            uint dataLength;
            AudioLibPCMFormat format = AudioLibPCMFormat.RiffHeaderParse(raw, out dataLength);
            Time rawEndTime = Time.Zero.AddTimeDelta(new TimeDelta(format.ConvertBytesToTime(dataLength)));

            //Time rawEndTime = Time.Zero.AddTimeDelta(MediaDuration); // We don't call this to avoid unnecessary I/O (Strem.Open() twice)

            if (
                ClipBegin.IsLessThan(Time.Zero)
                || ClipBegin.IsGreaterThan(ClipEnd)
                || ClipEnd.IsGreaterThan(rawEndTime)
                )
            {
                string msg = String.Format(
                    "WavClip [{0};{1}] is empty or not within the underlying wave data stream ([0;{2}])",
                    ClipBegin, ClipEnd, rawEndTime);
                throw new exception.InvalidDataFormatException(msg);
            }
            /*
            TimeDelta clipDuration = Duration;
            if (subClipBegin.IsEqualTo(Time.Zero) && subClipEnd.IsEqualTo(Time.Zero.AddTimeDelta(clipDuration)))
            {
                // Stream.Position is at the end of the RIFF header, we need to bring it back to the begining
                return new SubStream(
                raw,
                raw.Position, raw.Length - raw.Position); 
            }
            */
            Time rawClipBegin = ClipBegin.AddTime(subClipBegin);
            Time rawClipEnd = ClipBegin.AddTime(subClipEnd);

            long beginPos = raw.Position + format.ConvertTimeToBytes(rawClipBegin.TimeAsMillisecondDouble);

            long endPos = raw.Position + format.ConvertTimeToBytes(rawClipEnd.TimeAsMillisecondDouble);

            return new SubStream(
                raw,
                beginPos,
                endPos - beginPos);
        }

        #region IValueEquatable<WavClip> Members

        /// <summary>
        /// Determines of <c>this</c> has the same value as a given other instance
        /// </summary>
        /// <param name="other">The other instance</param>
        /// <returns>A <see cref="bool"/> indicating the result</returns>
        public bool ValueEquals(WavClip other)
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
            if (!ClipBegin.IsEqualTo(other.ClipBegin))
            {
                //System.Diagnostics.Debug.Fail("! ValueEquals !"); 
                return false;
            }
            if (IsClipEndTiedToEOM != other.IsClipEndTiedToEOM)
            {
                //System.Diagnostics.Debug.Fail("! ValueEquals !"); 
                return false;
            }
            if (!ClipEnd.IsEqualTo(other.ClipEnd))
            {
                //System.Diagnostics.Debug.Fail("! ValueEquals !"); 
                return false;
            }
            if (!DataProvider.ValueEquals(other.DataProvider))
            {
                //System.Diagnostics.Debug.Fail("! ValueEquals !"); 
                return false;
            }
            return true;
        }

        #endregion
    }
}

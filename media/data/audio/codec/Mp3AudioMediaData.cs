using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using urakawa.data;
using urakawa.media.timing;

namespace urakawa.media.data.audio.codec
{
    [XukNameUglyPrettyAttribute("mp4Au", "Mp4AudioMediaData")]
    public class Mp4AudioMediaData : Mp3AudioMediaData
    {
        public override string MimeType
        {
            get { return DataProviderFactory.AUDIO_MP4_MIME_TYPE; }
        }

    }
    [XukNameUglyPrettyAttribute("oggAu", "OggAudioMediaData")]
    public class OggAudioMediaData : Mp3AudioMediaData
    {
        public override string MimeType
        {
            get { return DataProviderFactory.AUDIO_OGG_MIME_TYPE; }
        }

    }

    [XukNameUglyPrettyAttribute("mp3Au", "Mp3AudioMediaData")]
    public class Mp3AudioMediaData : AudioMediaData
    {
        public override string MimeType
        {
            get { return DataProviderFactory.AUDIO_MP3_MIME_TYPE; }
        }

        public override Time AudioDuration
        {
            get
            {
#if DEBUG
                Debugger.Break();
#endif
                throw new NotImplementedException();
            }
        }

        public override Stream OpenPcmInputStream(Time clipBegin, Time clipEnd)
        {
#if DEBUG
            Debugger.Break();
#endif
            throw new NotImplementedException();
        }

        public override bool HasActualPcmData
        {
            get
            {
#if DEBUG
                Debugger.Break();
#endif
                throw new NotImplementedException();
            }
        }

        public override void AppendPcmData(DataProvider fileDataProvider)
        {
#if DEBUG
            Debugger.Break();
#endif
            throw new NotImplementedException();
        }

        public override void AppendPcmData(DataProvider fileDataProvider, Time clipBegin, Time clipEnd)
        {
#if DEBUG
            Debugger.Break();
#endif
            throw new NotImplementedException();
        }

        public override void InsertPcmData(Stream pcmData, Time insertPoint, Time duration)
        {
#if DEBUG
            Debugger.Break();
#endif
            throw new NotImplementedException();
        }

        public override void RemovePcmData(Time clipBegin, Time clipEnd)
        {
#if DEBUG
            Debugger.Break();
#endif
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using urakawa.data;
using urakawa.media.timing;

namespace urakawa.media.data.audio.codec
{
    public class Mp4AudioMediaData : Mp3AudioMediaData
    {
        public override string MimeType
        {
            get { return DataProviderFactory.AUDIO_MP4_MIME_TYPE; }
        }

        public override string GetTypeNameFormatted()
        {
            return xuk.XukStrings.Mp4AudioMediaData;
        }
    }

    public class OggAudioMediaData : Mp3AudioMediaData
    {
        public override string MimeType
        {
            get { return DataProviderFactory.AUDIO_OGG_MIME_TYPE; }
        }

        public override string GetTypeNameFormatted()
        {
            return xuk.XukStrings.OggAudioMediaData;
        }
    }

    public class Mp3AudioMediaData : AudioMediaData
    {
        public override string MimeType
        {
            get { return DataProviderFactory.AUDIO_MP3_MIME_TYPE; }
        }

        public override string GetTypeNameFormatted()
        {
            return xuk.XukStrings.Mp3AudioMediaData;
        }

        public override Time AudioDuration
        {
            get { throw new NotImplementedException(); }
        }

        public override Stream OpenPcmInputStream(Time clipBegin, Time clipEnd)
        {
            throw new NotImplementedException();
        }

        public override bool HasActualPcmData
        {
            get { throw new NotImplementedException(); }
        }

        public override void AppendPcmData(DataProvider fileDataProvider)
        {
            throw new NotImplementedException();
        }

        public override void AppendPcmData(DataProvider fileDataProvider, Time clipBegin, Time clipEnd)
        {
            throw new NotImplementedException();
        }

        public override void InsertPcmData(Stream pcmData, Time insertPoint, Time duration)
        {
            throw new NotImplementedException();
        }

        public override void RemovePcmData(Time clipBegin, Time clipEnd)
        {
            throw new NotImplementedException();
        }
    }
}

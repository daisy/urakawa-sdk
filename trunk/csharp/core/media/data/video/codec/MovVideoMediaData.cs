using urakawa.data;

namespace urakawa.media.data.video.codec
{
    [XukNameUglyPrettyAttribute("movVd", "MovVideoMediaData")]
    public class MovVideoMediaData : VideoMediaData
    {
        public override string MimeType
        {
            get { return DataProviderFactory.VIDEO_MOV_MIME_TYPE; }
        }

    }
}



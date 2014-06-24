using System.Collections.Generic;
using urakawa.media.data;

namespace urakawa.command
{
    public interface IUsingMediaData
    {
        IEnumerable<MediaData> UsedMediaData { get; }
    }
}

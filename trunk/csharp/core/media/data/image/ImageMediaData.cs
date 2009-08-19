using System;
using System.Collections.Generic;
using System.Text;

namespace urakawa.media.data.image
{
    public abstract class ImageMediaData: MediaData
    {
        protected abstract override MediaData CopyProtected();        
    }
}

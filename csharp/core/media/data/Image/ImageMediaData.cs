using System;
using System.Collections.Generic;
using System.Text;
using AudioLib;

namespace urakawa.media.data.Image
{
    public abstract class ImageMediaData : MediaData
    {
        protected abstract override MediaData CopyProtected();        
    }
}

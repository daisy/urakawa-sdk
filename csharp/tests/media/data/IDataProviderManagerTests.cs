using System;
using System.Collections.Generic;
using System.Text;

namespace urakawa.media.data
{
    public abstract class IDataProviderManagerTests
    {
        protected IDataProviderManager mManager = null;

        protected urakawa.Presentation mPresentation
        {
            get { return mManager.Presentation; }
        }

        protected urakawa.Project mProject
        {
            get { return mPresentation.Project; }
        }
    }
}

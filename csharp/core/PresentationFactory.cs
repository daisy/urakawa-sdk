using System;
using urakawa.xuk;

namespace urakawa
{
    [XukNameUglyPrettyAttribute("presFct", "PresentationFactory")]
    public class PresentationFactory : GenericXukAbleFactory<Presentation>
    {
        public override bool PrettyFormat
        {
            set { throw new NotImplementedException("PrettyFormat"); }
            get
            {
                return XukAble.m_PrettyFormat_STATIC;
            }
        }

        public virtual Presentation Create(Project proj, Uri uri, string dataFolderPrefix)
        {
            Presentation pres = Create<Presentation>();
            pres.Project = proj;
            pres.RootUri = uri;

            if (!String.IsNullOrEmpty(dataFolderPrefix))
            {
                pres.DataProviderManager.SetDataFileDirectoryWithPrefix(dataFolderPrefix);
            }

            if (
#if DEBUG
                true ||
#endif
                pres.PrettyFormat)
            {
                pres.WarmUpAllFactories();
            }

            return pres;
        }

    }
}
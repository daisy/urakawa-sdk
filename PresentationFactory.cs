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

        public virtual Presentation Create(Project proj, Uri uri, string dataFolderCustomName)
        {
            Presentation pres = Create<Presentation>();
            pres.Project = proj;
            pres.RootUri = uri;

            if (!String.IsNullOrEmpty(dataFolderCustomName))
            {
                pres.DataProviderManager.SetCustomDataFileDirectory(dataFolderCustomName);
            }

#if DEBUG
            pres.WarmUpAllFactories();
#endif

            return pres;
        }

    }
}
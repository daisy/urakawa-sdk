using urakawa.xuk;

namespace urakawa.ExternalFiles
{
    [XukNameUglyPrettyAttribute("ExFlDtFct", "ExternalFileDataFactory")]
    public class ExternalFileDataFactory : GenericWithPresentationFactory<ExternalFileData>
    {
        public ExternalFileDataFactory(Presentation pres)
            : base(pres)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance">The <see cref="ExternalFileData"/> instance to initialize</param>
        /// <remarks>
        /// In derived factories, this method can be overridden in order to do additional initialization.
        /// In this case the developer must remember to call <c>base.InitializeInstance(instance)</c>
        /// </remarks>
        protected override void InitializeInstance(ExternalFileData instance)
        {
            base.InitializeInstance(instance);
            if (m_skipManagerInitialization)
            {
                m_skipManagerInitialization = false;
                return;
            }

            Presentation.ExternalFilesDataManager.AddManagedObject(instance);
        }

        private bool m_skipManagerInitialization = false;
        public ExternalFileData Create_SkipManagerInitialization(string xukLN, string xukNS)
        {
            m_skipManagerInitialization = true;
            return Create(xukLN, xukNS);
        }
    }
}

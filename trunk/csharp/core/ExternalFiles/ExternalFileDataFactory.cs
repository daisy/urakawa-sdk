using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using urakawa.xuk;

namespace urakawa.ExternalFiles
    {
    public class ExternalFileDataFactory : GenericWithPresentationFactory<ExternalFileData>
        {
        public override string GetTypeNameFormatted ()
            {
            return XukStrings.ExternalFileDataFactory;
            }


        public ExternalFileDataFactory  ( Presentation pres )
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
            Presentation.ExternalFilesDataManager.AddManagedObject(instance);
        }

        




        }
    }

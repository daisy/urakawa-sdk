using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using urakawa.xuk;
using urakawa.data;

namespace urakawa.ExternalFiles
    {
    public sealed class ExternalFilesDataManager:XukAbleManager<ExternalFileData>
        {
        public ExternalFilesDataManager ( Presentation pres )
            : base ( pres, "MEF" )
            {
            }
        public override string GetTypeNameFormatted ()
            {
            return XukStrings.ExternalFileDataManager;
            }

        public override bool CanAddManagedObject (ExternalFileData fileDataObject)
            {
            return true;
            }

        


        /// <summary>
        /// Creates a copy of a given ExternalFileData
        /// </summary>
        /// <param name="data">The ExternalFileData to copy</param>
        /// <returns>The copy</returns>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when <paramref name="data"/> is <c>null</c>
        /// </exception>
        /// <exception cref="exception.IsNotManagerOfException">
        /// Thrown when <paramref name="data"/> is not managed by <c>this</c>
        /// </exception>
        public ExternalFileData CopyExternalFileData (ExternalFileData  data )
            {
            if (data == null)
                {
                throw new exception.MethodParameterIsNullException ( "Can not copy a null ExternalFileData" );
                }
            if (data.Presentation.ExternalFilesDataManager != this)
                {
                throw new exception.IsNotManagerOfException (
                    "Can not copy ExternalFileData that is not managed by this" );
                }
            return data.Copy ();
            }



        /// <summary>
        /// Creates a copy of the ExternalFileData with a given UID
        /// </summary>
        /// <param name="uid">The given UID</param>
        /// <returns>The copy</returns>
        /// <exception cref="exception.IsNotManagerOfException">
        /// Thrown when <c>this</c> does not manage a ExternalFileData data with the given UID
        /// </exception>
        public ExternalFileData CopyExternalFileData ( string uid )
            {
            ExternalFileData data = GetManagedObject ( uid );
            if (data == null)
                {
                throw new exception.IsNotManagerOfException ( String.Format (
                                                                "The ExternalFileData manager does not manage a ExternalFileData with UID {0}",
                                                                uid ) );
                }
            return CopyExternalFileData( data );
            }



        /// <summary>
        /// Clears the <see cref="ExternalFileDataDataManager"/> disassociating any linked <see cref="ExternalFileData"/>
        /// </summary>
        protected override void Clear ()
            {
            foreach (ExternalFileData EFd in ManagedObjects.ContentsAs_ListCopy)
                {
                ManagedObjects.Remove ( EFd);
                }
            base.Clear ();
            }

        public List<DataProvider> UsedDataProviders
            {
            get
                {
                List<DataProvider> usedDataProviders = new List<DataProvider> ();
                foreach (ExternalFileData eFD in ManagedObjects.ContentsAs_YieldEnumerable)
                    {
                    foreach (DataProvider prov in eFD.UsedDataProviders)
                        {
                        if (!usedDataProviders.Contains ( prov ))
                            {
                            usedDataProviders.Add ( prov );
                            }
                        }
                    }
                return usedDataProviders;
                }
            }


        public override bool ValueEquals ( WithPresentation other )
            {
            if (!base.ValueEquals ( other ))
                {
                return false;
                }
            ExternalFilesDataManager otherManager = other as ExternalFilesDataManager;

            if (otherManager == null)
                {
                return false;
                }

            
            return true;
            }


        }
    }

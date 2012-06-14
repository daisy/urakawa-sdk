using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using urakawa;
using urakawa.core;
using ICSharpCode.SharpZipLib.Zip;

namespace urakawa.daisy.export
{
    public partial class Daisy3_Export
    {

        public void CreateEPUBDirectoryStructure()
        {

            Directory.CreateDirectory(Path.Combine(m_OutputDirectory, "META-INF"));
            Directory.CreateDirectory(Path.Combine(m_OutputDirectory, "OPS"));

            StreamWriter writer = File.CreateText(Path.Combine(m_OutputDirectory, "mimetype"));
            writer.Write("application/epub+zip");
            writer.Close();
            writer = null;
            //"mimetype"
            /*
            string strPls = null;
            foreach (ExternalFiles.ExternalFileData efd in m_Presentation.ExternalFilesDataManager.ManagedObjects.ContentsAs_Enumerable)
            {

                if (efd is ExternalFiles.PLSExternalFileData
                    && !string.IsNullOrEmpty(efd.OriginalRelativePath)
                    && efd.IsPreservedForOutputFile
                    && strPls== null)
                {
                    StreamReader sr = new StreamReader(efd.OpenInputStream());
                    strPls = sr.ReadToEnd();
                    // create directories here
                }
            }
             */
            //PackageToZip();
        }

        public void PackageToZip ()
        {
            FastZip zip = new FastZip();
            zip.UseZip64 = UseZip64.On;
            
            string parentDirectory = Directory.GetParent(m_OutputDirectory).FullName;
            string fileName = Path.GetFileName(m_OutputDirectory);
            string filePath = Path.Combine(parentDirectory, fileName + ".zip");
            
            zip.CreateZip( filePath, m_OutputDirectory, true, null);
        }

    }
}

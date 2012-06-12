using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using urakawa;
using urakawa.core;

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
        }

    }
}

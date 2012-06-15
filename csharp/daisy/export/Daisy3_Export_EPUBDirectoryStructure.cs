using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using urakawa;
using urakawa.core;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;

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
            PackageToZip();
        }

        public void PackageToZip ()
        {
            FastZipEvents zipEvents = new FastZipEvents();
            zipEvents.ProcessFile = ProcessEvents;

            FastZip zip = new FastZip(zipEvents);
            zip.CreateEmptyDirectories = true ;
            //zip.UseZip64 = UseZip64.On;
            
            string parentDirectory = Directory.GetParent(m_OutputDirectory).FullName;
            string fileName = Path.GetFileName(m_OutputDirectory);
            string filePath = Path.Combine(parentDirectory, fileName + ".zip");
            if (File.Exists(filePath)) File.Delete(filePath);

            string emptyDirectoryPath = Path.Combine(m_OutputDirectory, fileName);
            Directory.CreateDirectory(emptyDirectoryPath);
            //zip.CreateZip( filePath, m_OutputDirectory, true, null);
            zip.CreateZip(filePath, emptyDirectoryPath, false, null);

            Directory.Delete(emptyDirectoryPath);
            ZipFile zippeFile = new ZipFile(filePath ) ;
            zippeFile.BeginUpdate();
            //zippeFile.Delete(emptyDirectoryPath);
            string initialPath = Directory.GetParent(m_OutputDirectory).FullName;
            //System.Windows.Forms.MessageBox.Show(initialPath);
            //zippeFile.AddDirectory(
            string mimeTypePath = Path.Combine(m_OutputDirectory,"mimetype")  ;
            ICSharpCode.SharpZipLib.Zip.StaticDiskDataSource dataSource = new StaticDiskDataSource(mimeTypePath) ;
            //zippeFile.Add(dataSource , Path.Combine(Path.GetFileName (emptyDirectoryPath), "mimetype"), CompressionMethod.Stored);
            zippeFile.Add(dataSource, mimeTypePath.Replace(initialPath,"") ,CompressionMethod.Stored);
            
            string [] listOfFiles = Directory.GetFiles(m_OutputDirectory, "*.*", SearchOption.TopDirectoryOnly);
            for(int i = 0 ; i < listOfFiles.Length ; i++ )
            {
                if ( listOfFiles[i] != mimeTypePath )
                {
                    zippeFile.Add(listOfFiles[i], listOfFiles[i].Replace(initialPath,""));
                }
            }
            string[] listOfDirectories = Directory.GetDirectories(m_OutputDirectory, "*.*", SearchOption.AllDirectories);
            for (int i = 0; i < listOfDirectories.Length; i++)
            {
                
                string[] listOfInternalFiles = Directory.GetFiles(listOfDirectories[i], "*.*", SearchOption.TopDirectoryOnly);
                //if (listOfInternalFiles.Length == 0) zippeFile.AddDirectory(listOfDirectories[i]);
                for (int j = 0; j < listOfInternalFiles.Length; j++)
                {
                    
                        zippeFile.Add(listOfInternalFiles[j], listOfInternalFiles[j].Replace(initialPath, ""));
                    
                }
                
            }

            //zippeFile.Add(Path.Combine(m_OutputDirectory, "mimetype"), "mimetype");
            
            zippeFile.CommitUpdate();

            zippeFile.Close();
        }

        private void ProcessEvents(object sender, ScanEventArgs args)
        {
        }

    }
}

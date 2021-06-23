using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using NAudio.Wave.WaveFormats;
using NAudio.Dsp;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;

namespace AudioLib
{
   public class ReadWriteCueMarkers
    {
       private CueWaveFileReader m_CueReader;
       private List<double> m_CuePointsList;
       private string[] m_CueLabelList;

       public ReadWriteCueMarkers()
       {
        
       }
       public void ReadCues(string pathOfAudioFile)
       {
           try
           {
               if (Path.GetExtension(pathOfAudioFile).ToLower() == ".wav")
               {
                   m_CueReader = new CueWaveFileReader(pathOfAudioFile);
               }
           }
           catch (Exception e)
           {
               MessageBox.Show(e.ToString());
           }

           if (m_CueReader != null && m_CueReader.Cues != null)
           {
               int[]  list = m_CueReader.Cues.CuePositions;
               m_CueLabelList = m_CueReader.Cues.CueLabels;
               int cueLabelCount = 0;
               m_CuePointsList = new List<double>();

               foreach (int cuePoint in list)
               {
                   int sampleRate = m_CueReader.WaveFormat.SampleRate;
                   double tempCuePointInSec = (cuePoint / sampleRate) * 1000;
                   m_CuePointsList.Add(tempCuePointInSec);                
                   cueLabelCount++;
               }
           }

       }

       // cue points will be in milliseconds 
       public List<double> ListOfCuePoints
       {
           get
           {
               if (m_CuePointsList != null)
                   return m_CuePointsList;
               else
                   return null;
           }
       }

       public string[] ListOfCueLabels
       {
           get
           {
               if (m_CueLabelList != null)
                   return m_CueLabelList;
               else
                   return null;
           }
       }

       public void WriteCues(Dictionary<string, List<double>> DictionaryOfCuePoints, bool IsInsertCuePointsInBook = false)
       {
           string SourceFile = string.Empty;
           List<double> CuePoints = new List<double>();
           int CuePointTime;
           string cueFileFolder = string.Empty;
           foreach (var entry in DictionaryOfCuePoints)
           {
               SourceFile = entry.Key;
               CuePoints = entry.Value;

               string sourceFileFolder = System.IO.Path.GetDirectoryName(SourceFile);
               string sourceFileName = System.IO.Path.GetFileName(SourceFile);
               string sourceFilePath = sourceFileFolder + "\\" + sourceFileName;
               cueFileFolder = sourceFileFolder + "\\" + "Audio_files_cuepoints";

               if (!Directory.Exists(cueFileFolder))
               {
                   Directory.CreateDirectory(cueFileFolder);
               }

               string CueAudioFilePath = cueFileFolder + "\\" + "Cues_" + sourceFileName;
               CueWaveFileReader sourceStream = new CueWaveFileReader(SourceFile);
               using (CueWaveFileWriter writer = new CueWaveFileWriter(CueAudioFilePath, sourceStream.WaveFormat))
               {
                   AudioLibPCMFormat pcmFormat = new AudioLibPCMFormat((ushort)sourceStream.WaveFormat.Channels, (uint)sourceStream.WaveFormat.SampleRate, (ushort)sourceStream.WaveFormat.BitsPerSample);
                   int BUFFER_SIZE = (int)pcmFormat.ConvertTimeToBytes(1500 * AudioLibPCMFormat.TIME_UNIT);  //132300;
                   byte[] buffer = new byte[BUFFER_SIZE];
                   int byteRead;
                   writer.Flush();

                   while ((byteRead = sourceStream.Read(buffer, 0, buffer.Length)) > 0)
                   {
                       writer.Write(buffer, 0, byteRead);
                   }
                   foreach (double CuePoint in CuePoints)
                   {
                       CuePointTime = (int)CuePoint * sourceStream.WaveFormat.SampleRate;
                       writer.AddCue(CuePointTime, string.Empty);
                   }

               }
               if (IsInsertCuePointsInBook)
               {
                   sourceStream.Dispose();
                   if (File.Exists(sourceFilePath))
                       File.Delete(sourceFilePath);

                   File.Move(CueAudioFilePath, sourceFilePath);
               }
           }

           if (IsInsertCuePointsInBook && Directory.Exists(cueFileFolder))
           {
               Directory.Delete(cueFileFolder);
           }
       }
    }
}

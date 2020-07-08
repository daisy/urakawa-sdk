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
   public class ReadCueMarkers
    {
       private CueWaveFileReader m_CueReader;
       private List<double> m_CuePointsList;
       private string[] m_CueLabelList;

       public ReadCueMarkers(string pathOfAudioFile)
       {
           try
           {
               if (Path.GetExtension(pathOfAudioFile).ToLower() == ".wav")
               {
                   m_CueReader = new CueWaveFileReader(pathOfAudioFile);
                   ReadCues();
               }
           }
           catch (Exception e)
           {
               MessageBox.Show(e.ToString());
           }
       }
       private void ReadCues()
       {

           if (m_CueReader.Cues != null)
           {
               int[]  list = m_CueReader.Cues.CuePositions;
               m_CueLabelList = m_CueReader.Cues.CueLabels;
               int cueLabelCount = 0;
               m_CuePointsList = new List<double>();

               foreach (int cuePoint in list)
               {
                   double tempCuePointInSec = (cuePoint / 44100)*1000;
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
    }
}

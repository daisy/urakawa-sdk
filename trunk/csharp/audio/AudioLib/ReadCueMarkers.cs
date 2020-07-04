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
       private List<decimal> m_CuePointsList;
       private string[] m_CueLabelList;

       public ReadCueMarkers(string pathOfAudioFile)
       {

           m_CueReader = new CueWaveFileReader(pathOfAudioFile);
           ReadCues();
       }
       private void ReadCues()
       {

           if (m_CueReader.Cues != null)
           {
               //MessageBox.Show(m_CueReader.Cues.Count.ToString());
               int[]  list = m_CueReader.Cues.CuePositions;
               m_CueLabelList = m_CueReader.Cues.CueLabels;
               int cueLabelCount = 0;
               m_CuePointsList = new List<decimal>();

               foreach (int cuePoint in list)
               {
                   decimal tempCuePointInSec = cuePoint / 44100;
                   m_CuePointsList.Add(tempCuePointInSec);                
                   cueLabelCount++;
               }
           }

       }

       public List<decimal> ListOfCuePoints
       {
           get
           {
               return m_CuePointsList;
           }
       }

       public string[] ListOfCueLabels
       {
           get
           {
               return m_CueLabelList;
           }
       }
    }
}

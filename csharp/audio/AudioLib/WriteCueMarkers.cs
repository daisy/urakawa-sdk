using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NAudio.Wave;
using System.IO;

namespace AudioLib
{
    public class WriteCueMarkers
    {

        public WriteCueMarkers()
        {
         

        }

        public void WriteCues(Dictionary<string,List<double>> DictionaryOfCuePoints, bool IsInsertCuePointsInBook = false)
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
                        writer.AddCue(CuePointTime,string.Empty);
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

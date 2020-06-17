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
    public class WavAudioProcessing
    {
        public string IncreaseAmplitude(string fileName, float processingFactor)
        {
            var inPath = fileName;
            string outputFileName = fileName.Substring(0, fileName.Length - 4);
            var outPath = outputFileName + " IncreaseAmplitude" + ".wav";
            using (var reader = new AudioFileReader(inPath))
            {                
                reader.Volume = processingFactor;
                //// write out to a new WAV file
                WaveFileWriter.CreateWaveFile16(outPath, reader);
            }
            
            return outPath;
        }

       // public string FadeIn(string fileName, double duration)
        public string FadeIn(string fileName, double duration, double FadeInStartTime)
        {
            // ffmpeg implementation of fade in

            string outputFileName = fileName.Substring(0, fileName.Length - 4);
            var outPath = outputFileName + "ffmpegNoiseReduction.wav";
            using (var reader = new AudioFileReader(fileName))
            {
                string ffmpegWorkingDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string ffmpegPath = Path.Combine(ffmpegWorkingDir, "ffmpeg.exe");
                if (!File.Exists(ffmpegPath))
                    throw new FileNotFoundException("Invalid compression library path " + ffmpegPath);

                if (!File.Exists(fileName))
                    throw new FileNotFoundException("Invalid source file path " + fileName);


                Process m_process = new Process();

                m_process.StartInfo.FileName = ffmpegPath;

                m_process.StartInfo.RedirectStandardOutput = false;
                m_process.StartInfo.RedirectStandardError = false;
                m_process.StartInfo.UseShellExecute = true;
                m_process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

                //ffmpeg -y -i "004_a_word_on_life_skills.wav" -af afade=t=in:st=20:d=20 "004_a_word_on_life_skillsFadeIn.wav"
                m_process.StartInfo.Arguments = string.Format("-y -i " + "\"" + fileName + "\"" + " -af afade=t=in:st=" + FadeInStartTime + ":d=" + duration + " \"" + outPath + "\"");


                m_process.Start();
                m_process.WaitForExit();

                return outPath;

            }




            // NAudio implementation of Fade In

            //var inPath = fileName;
            //string outputFileName = fileName.Substring(0, fileName.Length - 4);
            //var outPath = outputFileName + "FadeInTemp.wav";
            //using (var reader = new AudioFileReader(inPath))
            //{
            //    //TimeSpan span = reader.TotalTime;
            //    var fader = new FadeInOutSampleProvider(reader, false);
            //    //double totalTime = span.TotalMilliseconds;
            //    fader.BeginFadeIn(duration);
            //    var stwp = new SampleToWaveProvider(fader);
            //    WaveFileWriter.CreateWaveFile(outPath, stwp);

            //}

            //using (var reader = new AudioFileReader(outPath))
            //{
            //    outPath = outputFileName + "FadeIn.wav";
            //    //var temp = new Wave32To16Stream(reader);
            //    WaveFileWriter.CreateWaveFile16(outPath, reader);
            //}
            //return outPath;
        }

        //public string FadeOut(string fileName, double duration)
        public string FadeOut(string fileName, double duration, double FadeOutStartTime)
        {
            // ffmpeg implementation of fade out

            string outputFileName = fileName.Substring(0, fileName.Length - 4);
            var outPath = outputFileName + "ffmpegNoiseReduction.wav";
            using (var reader = new AudioFileReader(fileName))
            {
                string ffmpegWorkingDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string ffmpegPath = Path.Combine(ffmpegWorkingDir, "ffmpeg.exe");
                if (!File.Exists(ffmpegPath))
                    throw new FileNotFoundException("Invalid compression library path " + ffmpegPath);

                if (!File.Exists(fileName))
                    throw new FileNotFoundException("Invalid source file path " + fileName);


                Process m_process = new Process();

                m_process.StartInfo.FileName = ffmpegPath;

                m_process.StartInfo.RedirectStandardOutput = false;
                m_process.StartInfo.RedirectStandardError = false;
                m_process.StartInfo.UseShellExecute = true;
                m_process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

                //ffmpeg -y -i "004_a_word_on_life_skills.wav" -af afade=t=out:st=20:d=20 "004_a_word_on_life_skillsFadeout.wav"
                m_process.StartInfo.Arguments = string.Format("-y -i " + "\"" + fileName + "\"" + " -af afade=t=out:st=" + FadeOutStartTime + ":d=" + duration + " \"" + outPath + "\"");


                m_process.Start();
                m_process.WaitForExit();

                return outPath;

            }



            //  NAudio Implementation of Fade Out

            //var inPath = fileName;
            //string outputFileName = fileName.Substring(0, fileName.Length - 4);
            //var outPath = outputFileName + "FadeInTemp.wav";
            //using (var reader = new AudioFileReader(inPath))
            //{
            //    //TimeSpan span = reader.TotalTime;
            //    var fader = new FadeInOutSampleProvider(reader, false);
            //    //double totalTime = span.TotalMilliseconds;
            //    fader.BeginFadeOut(duration);
            //    var stwp = new SampleToWaveProvider(fader);
            //    WaveFileWriter.CreateWaveFile(outPath, stwp);

            //}

            //using (var reader = new AudioFileReader(outPath))
            //{
            //    outPath = outputFileName + "FadeIn.wav";
            //    //var temp = new Wave32To16Stream(reader);
            //    WaveFileWriter.CreateWaveFile16(outPath, reader);
            //}
            //return outPath;
        }

        public string Normalize(string fileName, float processingFactor)
        {
            var inPath = fileName;
            //var outPath = @"H:\Repos\normalized1.wav";
            string outputFileName = fileName.Substring(0, fileName.Length - 4);
            var outPath = outputFileName + "normalized.wav";
            // var outPath = fileName + "\\ normalized.wav";
            float max = 0;

            using (var reader = new AudioFileReader(inPath))
            {
                // find the max peak
                float[] buffer = new float[reader.WaveFormat.SampleRate];
                int read;
                do
                {
                    read = reader.Read(buffer, 0, buffer.Length);
                    for (int n = 0; n < read; n++)
                    {
                        var abs = Math.Abs(buffer[n]);
                        if (abs > max) max = abs;
                    }
                } while (read > 0);
                Console.WriteLine("Max sample value: {0}", max);
                if (max == 0 || max > 1.0f)
                    throw new InvalidOperationException("File cannot be normalized");

                // rewind and amplify
                reader.Position = 0;
                //int SelectedItem = Int32.Parse(comboBox1.SelectedItem.ToString());
                //reader.Volume = SelectedItem;
                reader.Volume = (1.0f / max) * processingFactor;

                // write out to a new WAV file
                WaveFileWriter.CreateWaveFile16(outPath, reader);
                return outPath;
            }
        }

        public string NoiseReductionFfmpegAfftdn(string fileName, decimal noiseReductionVal, decimal noiseFloorVal)
        {
            string outputFileName = fileName.Substring(0, fileName.Length - 4);
            var outPath = outputFileName + "ffmpegNoiseReduction.wav";
            using (var reader = new AudioFileReader(fileName))
            {
                string ffmpegWorkingDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string ffmpegPath = Path.Combine(ffmpegWorkingDir, "ffmpeg.exe");
                if (!File.Exists(ffmpegPath))
                    throw new FileNotFoundException("Invalid compression library path " + ffmpegPath);

                if (!File.Exists(fileName))
                    throw new FileNotFoundException("Invalid source file path " + fileName);


                Process m_process = new Process();

                m_process.StartInfo.FileName = ffmpegPath;

                m_process.StartInfo.RedirectStandardOutput = false;
                m_process.StartInfo.RedirectStandardError = false;
                m_process.StartInfo.UseShellExecute = true;
                m_process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

                //m_process.StartInfo.Arguments = string.Format("-y -i " + "\"" + fileName + "\"" + " -af afftdn=nr=50:nf=-20 " + "\"" + outPath + "\"");
                m_process.StartInfo.Arguments = string.Format("-y -i " + "\"" + fileName + "\"" + " -af afftdn=nr=" + noiseReductionVal + ":nf=" + noiseFloorVal + " \"" + outPath + "\"");


                m_process.Start();
                m_process.WaitForExit();

                return outPath;

            }

        }

        public string AudioMixing(string fileName, string audioToMix, decimal weightOfAudio, decimal droupoutTransition, bool IsEndOfStreamDurationChecked,
             bool IsSecondAudioToMixSelected = false, string secondAudioToMix = "", decimal weightOfSecondAudio = 0)
        {
            string outputFileName = fileName.Substring(0, fileName.Length - 4);
            var outPath = outputFileName + "ffmpegAudioMix.wav";
            using (var reader = new AudioFileReader(fileName))
            {
                try
                {
                    string ffmpegWorkingDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                    string ffmpegPath = Path.Combine(ffmpegWorkingDir, "ffmpeg.exe");
                    if (!File.Exists(ffmpegPath))
                        throw new FileNotFoundException("Invalid compression library path " + ffmpegPath);

                    if (!File.Exists(fileName))
                        throw new FileNotFoundException("Invalid source file path " + fileName);


                    Process m_process = new Process();

                    m_process.StartInfo.FileName = ffmpegPath;

                    m_process.StartInfo.RedirectStandardOutput = false;
                    m_process.StartInfo.RedirectStandardError = false;
                    m_process.StartInfo.UseShellExecute = true;
                    m_process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

                    //ffmpeg -y -i "004_a_word_on_life_skills.wav" -i "YOU TOOK MY HEART.wav" -filter_complex amix=inputs=2:duration=longest:weights="1 0.05" "AudioMixSample.wav"
                    if (IsSecondAudioToMixSelected)
                    {
                        if (IsEndOfStreamDurationChecked)
                        {
                            m_process.StartInfo.Arguments = string.Format("-y -i " + "\"" + fileName + "\"" + " -i \"" + audioToMix + "\"" + " -i \"" + secondAudioToMix + "\"" + " -filter_complex amix=inputs=3:duration=first:dropout_transition=" + droupoutTransition + ":weights=" + "\"1 " + weightOfAudio + " " + weightOfSecondAudio + "\"" + " \"" + outPath + "\"");
                        }
                        else
                        {
                            m_process.StartInfo.Arguments = string.Format("-y -i " + "\"" + fileName + "\"" + " -i \"" + audioToMix + "\"" + " -i \"" + secondAudioToMix + "\"" + " -filter_complex amix=inputs=3:duration=longest:dropout_transition=" + droupoutTransition + ":weights=" + "\"1 " + weightOfAudio + " " + weightOfSecondAudio + "\"" + " \"" + outPath + "\"");
                        }
                    }
                    else
                    {
                        if (IsEndOfStreamDurationChecked)
                        {
                            m_process.StartInfo.Arguments = string.Format("-y -i " + "\"" + fileName + "\"" + " -i \"" + audioToMix + "\"" + " -filter_complex amix=inputs=2:duration=first:dropout_transition=" + droupoutTransition + ":weights=" + "\"1 " + weightOfAudio + "\"" + " \"" + outPath + "\"");
                        }
                        else
                        {
                            m_process.StartInfo.Arguments = string.Format("-y -i " + "\"" + fileName + "\"" + " -i \"" + audioToMix + "\"" + " -filter_complex amix=inputs=2:duration=longest:dropout_transition=" + droupoutTransition + ":weights=" + "\"1 " + weightOfAudio + "\"" + " \"" + outPath + "\"");
                        }
                    }


                    m_process.Start();
                    m_process.WaitForExit();

                }
                catch
                {
                    MessageBox.Show("Something went wrong");
                }

                return outPath;
            }

        }


        public enum AudioProcessingKind { Amplify, FadeIn, FadeOut, Normalize, SoundTouch, NoiseReduction, AudioMixing } ;
    }
}

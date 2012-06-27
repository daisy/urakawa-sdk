using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Speech;
using System.Speech.AudioFormat;
using System.Speech.Synthesis;
using System.Speech.Synthesis.TtsEngine;

namespace AudioLib
{
    public class TextToSpeech
    {
        private readonly AudioLibPCMFormat m_PcmFormat;
        private readonly SpeechSynthesizer m_SpeechSynthesizer;

        public TextToSpeech(AudioLibPCMFormat pcmFormat, SpeechSynthesizer speechSynthesizer)
        {
            m_PcmFormat = pcmFormat;
            
            m_SpeechSynthesizer = speechSynthesizer;
        }

        public bool SpeakString( string input, string fileFullPath)
        {
 
    SpeechAudioFormatInfo formatInfo = new SpeechAudioFormatInfo((int)m_PcmFormat.SampleRate, AudioBitsPerSample.Sixteen, m_PcmFormat.NumberOfChannels == 2 ? AudioChannel.Stereo : AudioChannel.Mono);
                  if (fileFullPath != null)
                  {
                      m_SpeechSynthesizer.SetOutputToWaveFile(fileFullPath, formatInfo);
                      m_SpeechSynthesizer.SpeakAsync(input);
                      m_SpeechSynthesizer.SetOutputToNull(); 
}

else
{
    m_SpeechSynthesizer.SpeakAsync(input);
//speech.Speak(input, SpFlags);
}
             
            return true;
        }




    }
}

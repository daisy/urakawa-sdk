using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Forms;
using javazoom.jl.decoder;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using System.Diagnostics;
using NAudio.Wave.Asio;
using SoundTouch;
using SoundTouch.Utility;
using Buffer = System.Buffer;
#if true || SOUNDTOUCH_INTEGER_SAMPLES
using TSampleType = System.Int16; // short (System.Int32 == int)
using TLongSampleType = System.Int64; // long
#else
using TSampleType = System.Single;
using TLongSampleType = System.Double;
#endif

#if ENABLE_VST
using Jacobi.Vst.Core;
using Jacobi.Vst.Core.Host;
using Jacobi.Vst.Interop.Host;

//using Jacobi.Vst.Framework.Plugin;
//using Jacobi.Vst.Interop.Plugin;
#endif

namespace AudioLib
{
    public enum AudioFileFormats {WAV, MP3, MP4, AMR, GP3} 
    //public class WavAmplify : WavNormalize
    //{
    //    public WavAmplify(string fullpath, double amp)
    //        : base(fullpath, amp)
    //    {
    //    }
    //}

    public class WavAmplify : DualCancellableProgressReporter
    {
        private string m_fullpath = null;
        private double m_amp = 0;

        public WavAmplify(string fullpath, double amp)
        {
            m_fullpath = fullpath;
            m_amp = amp;
        }

        public override void DoWork()
        {
            RequestCancellation = false;

            AudioLibPCMFormat audioPCMFormat = null;

            string fullpath_ = m_fullpath + "_.wav";

            Stream audioStream = null;
            ulong audioStreamRiffOffset = 0;
            Stream destStream = null;

#if ENABLE_VST
            VstPluginContext ctx = null;
            Form form = null;

            VstAudioBufferManager vstBufManIn = null;
            VstAudioBufferManager vstBufManOut = null;

            VstAudioBuffer[] vstBufIn = null;
            VstAudioBuffer[] vstBufOut = null;

            VstAudioPrecisionBufferManager vstBufManIn_ = null;
            VstAudioPrecisionBufferManager vstBufManOut_ = null;

            VstAudioPrecisionBuffer[] vstBufIn_ = null;
            VstAudioPrecisionBuffer[] vstBufOut_ = null;
#endif

            bool okay = false;
            try
            {
                audioStream = File.Open(m_fullpath, FileMode.Open, FileAccess.Read, FileShare.Read);

                uint dataLength;
                AudioLibPCMFormat pcmInfo = null;

                pcmInfo = AudioLibPCMFormat.RiffHeaderParse(audioStream, out dataLength);

                //audioPCMFormat = new PCMFormatInfo(pcmInfo);
                audioPCMFormat = new AudioLibPCMFormat();
                audioPCMFormat.CopyFrom(pcmInfo);

                //destStream = File.Open(fullpath_, FileMode.CreateNew, FileAccess.Write, FileShare.Write);
                destStream = new FileStream(fullpath_, FileMode.Create, FileAccess.Write, FileShare.None);
                audioStreamRiffOffset = audioPCMFormat.RiffHeaderWrite(destStream, 0);
                DebugFix.Assert((long)audioStreamRiffOffset == destStream.Position);

                int bytesToTransfer =
                    (int)
                        Math.Min(audioStream.Length - (long)audioStreamRiffOffset, audioPCMFormat.ConvertTimeToBytes(500 * AudioLibPCMFormat.TIME_UNIT)); // automatically block-aligned to frame size (sample bytes * number of channels)
                byte[] byteBuffer = new byte[bytesToTransfer];

                int bytesReadFromAudioStream = 0;

                long totalAudioBytes = audioStream.Length;
                long currentAudioBytes = 0;
                int previousPercent = -100;






#if ENABLE_VST
                string workingDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string fullPath = Path.Combine(workingDir, "VST\\xxx.dll");

                fullPath = Microsoft.VisualBasic.Interaction.InputBox("DLL path or name", "VST audio plugin", fullPath, -1, -1);

                IVstHostCommandStub stub = new VST.HostCommandStub();

                try
                {
                    ctx = VstPluginContext.Create(fullPath, stub);
                }
                catch (Exception e)
                {
#if DEBUG
                    Debugger.Break();
#endif
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);

                    return;
                }

                //ctx.Set("FullPath", fullPath);
                //ctx.Set("IVstHostCommandStub", stub);

                ctx.PluginCommandStub.Open();

                if (ctx.PluginInfo.Flags.HasFlag(VstPluginFlags.CanReplacing))
                //if ((ctx.PluginInfo.Flags & VstPluginFlags.CanReplacing) == 0)
                //if (ctx.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.Bypass)) != VstCanDoResult.Yes) //"IVstPluginAudioProcessor"
                {
#if DEBUG
                    Debugger.Break();
#endif
                }

                if (ctx.PluginInfo.Flags.HasFlag(VstPluginFlags.CanDoubleReplacing))
                //if ((ctx.PluginInfo.Flags & VstPluginFlags.CanDoubleReplacing) == 0)
                //if (ctx.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.Bypass)) != VstCanDoResult.Yes) //"IVstPluginAudioProcessor"
                {
#if DEBUG
                    Debugger.Break();
#endif
                }

                form = new Form();
                form.Show();

                if (ctx.PluginInfo.Flags.HasFlag(VstPluginFlags.HasEditor))
                //if ((ctx.PluginInfo.Flags & VstPluginFlags.HasEditor) == 0)
                {
#if DEBUG
                    Debugger.Break();
#endif
                    IntPtr hWnd = form.Handle;
                    ctx.PluginCommandStub.EditorOpen(hWnd);
                }

                try
                {
                    Rectangle rect = new Rectangle();
                    ctx.PluginCommandStub.EditorGetRect(out rect);

                    if (rect.Width > 0)
                    {
                        form.Size = new Size(rect.Width, rect.Height);
                    }
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debugger.Break();
#endif
                }

                ctx.PluginCommandStub.SetSampleRate((float)audioPCMFormat.SampleRate);

                ctx.PluginCommandStub.SetProcessPrecision(VstProcessPrecision.Process32);

                //int nVSTSamples = (int)Math.Round(audioPCMFormat.SampleRate/100.0); // 100ms
                int nVSTSamples = (int)Math.Round(bytesToTransfer / 2.0 / audioPCMFormat.NumberOfChannels);
                ctx.PluginCommandStub.SetBlockSize(nVSTSamples);

                int inputCount = ctx.PluginInfo.AudioInputCount;
                if (inputCount <= 0)
                {
#if DEBUG
                    Debugger.Break();
#endif
                    inputCount = audioPCMFormat.NumberOfChannels;
                }
                int outputCount = ctx.PluginInfo.AudioOutputCount;
                if (outputCount <= 0)
                {
#if DEBUG
                    Debugger.Break();
#endif
                    outputCount = audioPCMFormat.NumberOfChannels;
                }

                vstBufManIn = new VstAudioBufferManager(inputCount, nVSTSamples);
                vstBufManOut = new VstAudioBufferManager(outputCount, nVSTSamples);

                vstBufIn = vstBufManIn.ToArray();
                vstBufOut = vstBufManOut.ToArray();

                vstBufManIn_ = new VstAudioPrecisionBufferManager(inputCount, nVSTSamples);
                vstBufManOut_ = new VstAudioPrecisionBufferManager(outputCount, nVSTSamples);

                vstBufIn_ = vstBufManIn_.ToArray();
                vstBufOut_ = vstBufManOut_.ToArray();

                ctx.PluginCommandStub.MainsChanged(true);
                ctx.PluginCommandStub.StartProcess();

                //ctx.PluginCommandStub.ProcessEvents()
#endif



                while ((bytesReadFromAudioStream = audioStream.Read(byteBuffer, 0, bytesToTransfer)) > 0)
                {
                    DebugFix.Assert(bytesReadFromAudioStream <= bytesToTransfer);

                    if (RequestCancellation) return;

                    currentAudioBytes += bytesReadFromAudioStream;

                    int percent = (int)Math.Round(100.0 * currentAudioBytes / (double)totalAudioBytes);

                    previousPercent = percent;

                    reportProgress_Throttle(percent, "[ " + percent + "% ] " +
                                            Math.Round(currentAudioBytes / (double)1024) + " / " +
                                            Math.Round(totalAudioBytes / (double)1024) + " (kB)");

#if ENABLE_VST
                    ctx.PluginCommandStub.EditorIdle();

                    int iSample = -1;
                    for (iSample = 0; iSample < nVSTSamples; iSample++)
                    {
                        for (int channel = 0; channel < inputCount; channel++) //audioPCMFormat.NumberOfChannels
                        {
                            vstBufIn[channel][iSample] = 0.0f;

                            vstBufIn_[channel][iSample] = 0.0;
                        }
                        for (int channel = 0; channel < outputCount; channel++) //audioPCMFormat.NumberOfChannels
                        {
                            vstBufOut[channel][iSample] = 1.0f;

                            vstBufOut_[channel][iSample] = 1.0;
                        }
                    }
                    iSample = -1;
#endif

                    for (int i = 0; i < bytesReadFromAudioStream; ) //i += m_CurrentAudioPCMFormat.BlockAlign)
                    {
                        if (RequestCancellation) return;

#if ENABLE_VST
                        iSample++;
#endif

                        for (int channel = 0; channel < audioPCMFormat.NumberOfChannels; channel++)
                        {
                            if (RequestCancellation) return;

                            if (i >= bytesReadFromAudioStream)
                            {
                                break;
                            }
                            byte byte1 = byteBuffer[i++];

                            if (i >= bytesReadFromAudioStream)
                            {
                                break;
                            }
                            byte byte2 = byteBuffer[i++];

                            short sample =
                                BitConverter.IsLittleEndian
                                    ? (short)(byte1 | (byte2 << 8))
                                    : (short)((byte1 << 8) | byte2);

#if ENABLE_VST
                            if (channel < outputCount)
                            {
                                float sampleF = (float)sample / 32768f;
                                if (sampleF > 1.0f)
                                {
                                    sampleF = 1.0f;
                                }
                                if (sampleF < -1.0f)
                                {
                                    sampleF = -1.0f;
                                }
                                vstBufIn[channel][iSample] = sampleF;

                                double sampleD = (double)sample / 32768d;
                                if (sampleD > 1.0d)
                                {
                                    sampleD = 1.0d;
                                }
                                if (sampleD < -1.0d)
                                {
                                    sampleD = -1.0d;
                                }
                                vstBufIn_[channel][iSample] = sampleD;
                            }
#endif

#if DEBUG
                            //// Little Endian
                            //short s1 = (short)(byte1 | (byte2 << 8));
                            //short s2 = (short)(byte1 + byte2 * 256);

                            //// Big Endian
                            //short s3 = (short)((byte1 << 8) | byte2);
                            //short s4 = (short)(byte1 * 256 + byte2);

                            short checkedSample = BitConverter.ToInt16(byteBuffer, i - 2);
                            DebugFix.Assert(checkedSample == sample);

                            checkedSample = (short)(byte1 + byte2 * 256);
                            DebugFix.Assert(checkedSample == sample);
#endif //DEBUG

                            const short MaxValue = short.MaxValue; // Int 16 signed 32767 Int16.MaxValue
                            const short MinValue = short.MinValue; // Int 16 signed -32768

                            int sample_ = (int)Math.Round(sample * m_amp);
                            if (sample_ > MaxValue)
                            {
                                sample = MaxValue;
                            }
                            else if (sample_ < MinValue) //short.MinValue
                            {
                                sample = MinValue;
                            }
                            else
                            {
                                sample = (short)sample_;
                            }

                            byte[] sampleBytes = BitConverter.GetBytes(sample);
                            //BitConverter.IsLittleEndian
                            // byte order is already-handled by BitConverter
                            byteBuffer[i - 2] = sampleBytes[0];
                            byteBuffer[i - 1] = sampleBytes[1];
                        }
                    }

#if ENABLE_VST
                    for (int i = 0; i < byteBuffer.Length; i++)
                    {
                        byteBuffer[i] = 0;
                    }

                    try
                    {
                        if (ctx.PluginInfo.Flags.HasFlag(VstPluginFlags.CanDoubleReplacing))
                        {
                            ctx.PluginCommandStub.ProcessReplacing(vstBufIn_, vstBufOut_);
                        }
                        if (ctx.PluginInfo.Flags.HasFlag(VstPluginFlags.CanReplacing))
                        {
                            ctx.PluginCommandStub.ProcessReplacing(vstBufIn, vstBufOut);
                        }
                    }
                    catch (System.AccessViolationException ex1)
                    {
                        //noop
                        bool debuggerBreak = true;
                    }
                    catch (Exception ex)
                    {
                        //noop
                        bool debuggerBreak = true;
                    }

                    int iByte = 0;
                    for (iSample = 0; iSample < nVSTSamples; iSample++)
                    {
                        for (int channel = 0; channel < audioPCMFormat.NumberOfChannels; channel++)
                        {
                            short sample = 0;

                            if (channel < outputCount)
                            {
                                float sampleF = vstBufOut[channel][iSample];
                                sampleF *= 32768f;
                                if (sampleF > Int16.MaxValue)
                                {
                                    sampleF = Int16.MaxValue;
                                }
                                if (sampleF < Int16.MinValue)
                                {
                                    sampleF = Int16.MinValue;
                                }
                                sample = (short) Math.Round(sampleF);

                                if (ctx.PluginInfo.Flags.HasFlag(VstPluginFlags.CanDoubleReplacing))
                                {
                                    double sampleD = vstBufOut_[channel][iSample];
                                    sampleD *= 32768d;
                                    if (sampleD > Int16.MaxValue)
                                    {
                                        sampleD = Int16.MaxValue;
                                    }
                                    if (sampleD < Int16.MinValue)
                                    {
                                        sampleD = Int16.MinValue;
                                    }
                                    sample = (short) Math.Round(sampleD);
                                }
                            }

                            byte[] sampleBytes = BitConverter.GetBytes(sample);
                            //BitConverter.IsLittleEndian
                            // byte order is already-handled by BitConverter

                            if (iByte >= bytesReadFromAudioStream)
                            {
                                break;
                            }
                            byteBuffer[iByte++] = sampleBytes[0];

                            if (iByte >= bytesReadFromAudioStream)
                            {
                                break;
                            }
                            byteBuffer[iByte++] = sampleBytes[1];
                        }
                    }
#endif

                    destStream.Write(byteBuffer, 0, bytesReadFromAudioStream);
                }

                okay = true;
            }
            catch (Exception ex)
            {
                okay = false;
            }
            finally
            {
#if ENABLE_VST

                if (ctx != null)
                {
                    ctx.PluginCommandStub.StopProcess();
                    ctx.PluginCommandStub.MainsChanged(false);

                    ctx.PluginCommandStub.EditorClose();

                    //ctx.PluginCommandStub.Close();
                    ctx.Dispose();
                }

                if (form != null)
                {
                    form.Close();
                }


                if (vstBufManIn != null)
                {
                    vstBufManIn.Dispose();
                }
                if (vstBufManOut != null)
                {
                    vstBufManOut.Dispose();
                }

                if (vstBufManIn_ != null)
                {
                    vstBufManIn_.Dispose();
                }
                if (vstBufManOut_ != null)
                {
                    vstBufManOut_.Dispose();
                }

#endif

                if (audioStream != null)
                {
                    audioStream.Close();
                }
                if (destStream != null)
                {
                    destStream.Position = 0;
                    audioStreamRiffOffset = audioPCMFormat.RiffHeaderWrite(
                        destStream,
                        (uint)
                            (destStream.Length -
                             (long)
                                 audioStreamRiffOffset));
                    destStream.Close();
                }
            }

            if (okay && File.Exists(fullpath_))
            {
                File.Delete(m_fullpath);
                File.Move(fullpath_, m_fullpath);
                try
                {
                    File.SetAttributes(m_fullpath, FileAttributes.Normal);
                }
                catch
                {
                }
            }
        }
    }

    public class WavNormalize : DualCancellableProgressReporter
    {
        private string m_fullpath = null;
        private float m_amp = 0;

        public WavNormalize(string fullpath, float amp)
        {
            m_fullpath = fullpath;
            m_amp = amp;
        }

        private Process m_process = null;
        private void cancellationRequestedEvent(object sender, DualCancellableProgressReporter.CancellationRequestedEventArgs e)
        {
            if (m_process != null)
            {
                try
                {
                    m_process.Kill();
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debugger.Break();
#endif //DEBUG
                }
            }
        }

        public override void DoWork()
        {
            RequestCancellation = false;
            CancellationRequestedEvent += cancellationRequestedEvent;


            string fullpath_ = m_fullpath + "_.wav";

            string workingDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string normalizeExe = Path.Combine(workingDir, "normalize.exe");
            if (!File.Exists(normalizeExe))
            {
#if DEBUG
                Debugger.Break();
#endif //DEBUG

                RequestCancellation = true;
                return;
            }

            bool okay = false;
            try
            {
                string msg = Path.GetFileName(m_fullpath);
                reportProgress(-1, msg);

                //https://neon1.net/prog/normalizer.html
                string argumentString = (m_amp != 0 ? ("-l " + m_amp) : "") //"-m 50.00 -s 98.00"
                    + " -o \"" + fullpath_ + "\" \"" + m_fullpath + "\"";
                Console.WriteLine(normalizeExe + " " + argumentString);

                m_process = new Process();

                m_process.StartInfo.FileName = normalizeExe;
                m_process.StartInfo.RedirectStandardOutput = false;
                m_process.StartInfo.RedirectStandardError = false;
                m_process.StartInfo.UseShellExecute = true;
                m_process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                m_process.StartInfo.Arguments = argumentString;
                m_process.Start();
                m_process.WaitForExit();

                if (RequestCancellation)
                {
                    okay = false;
                }
                else
                {
                    okay = true;

                    if (!m_process.StartInfo.UseShellExecute && m_process.ExitCode != 0)
                    {
                        StreamReader stdErr = m_process.StandardError;
                        if (!stdErr.EndOfStream)
                        {
                            string toLog = stdErr.ReadToEnd();
                            if (!string.IsNullOrEmpty(toLog))
                            {
                                Console.WriteLine(toLog);
                            }
                        }
                    }
                    else if (!m_process.StartInfo.UseShellExecute)
                    {
                        StreamReader stdOut = m_process.StandardOutput;
                        if (!stdOut.EndOfStream)
                        {
                            string toLog = stdOut.ReadToEnd();
                            if (!string.IsNullOrEmpty(toLog))
                            {
                                Console.WriteLine(toLog);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);

#if DEBUG
                Debugger.Break();
#endif //DEBUG
                okay = false;
            }
            finally
            {
                //noop
            }

            if (okay && File.Exists(fullpath_))
            {
                File.Delete(m_fullpath);
                File.Move(fullpath_, m_fullpath);
                try
                {
                    File.SetAttributes(m_fullpath, FileAttributes.Normal);
                }
                catch
                {
                }
            }
        }
    }

    public class WavSoundTouch : DualCancellableProgressReporter
    {
        SoundTouch<TSampleType, TLongSampleType> m_SoundTouch;

        private byte[] m_SoundTouch_ByteBuffer = null;
        private TSampleType[] m_SoundTouch_SampleBuffer = null;

        private string m_fullpath = null;
        private double m_factor = 0.0f;

        public WavSoundTouch(string fullpath, double factor
            //, AudioLibPCMFormat audioPCMFormat
            )
        {
            m_fullpath = fullpath;
            m_factor = factor;
        }

        public override void DoWork()
        {
            RequestCancellation = false;

            AudioLibPCMFormat audioPCMFormat = null;

            string fullpath_ = m_fullpath + "_.wav";

            m_SoundTouch = new SoundTouch<TSampleType, TLongSampleType>();

            m_SoundTouch.SetSetting(SettingId.UseAntiAliasFilter, 1);

            // Speech optimised
            m_SoundTouch.SetSetting(SettingId.SequenceDurationMs, 40);
            m_SoundTouch.SetSetting(SettingId.SeekwindowDurationMs, 15);
            m_SoundTouch.SetSetting(SettingId.OverlapDurationMs, 8);

            ////m_SoundTouch.Flush();
            //m_SoundTouch.Clear();

            m_SoundTouch.SetSetting(SettingId.UseQuickseek, 0);


            m_SoundTouch.SetTempo((float)m_factor);
            //m_SoundTouch.SetTempoChange(m_factor * 100);

            m_SoundTouch.SetPitchSemiTones(0);
            m_SoundTouch.SetRateChange(0);

            Stream audioStream = null;
            ulong audioStreamRiffOffset = 0;
            Stream destStream = null;

            bool okay = false;
            try
            {
                audioStream = File.Open(m_fullpath, FileMode.Open, FileAccess.Read, FileShare.Read);

                uint dataLength;
                AudioLibPCMFormat pcmInfo = null;

                pcmInfo = AudioLibPCMFormat.RiffHeaderParse(audioStream, out dataLength);

                //audioPCMFormat = new PCMFormatInfo(pcmInfo);
                audioPCMFormat = new AudioLibPCMFormat();
                audioPCMFormat.CopyFrom(pcmInfo);

                ////AudioLib.SampleRate.Hz22050
                //if (audioPCMFormat.SampleRate > 22050)
                //{
                //    m_SoundTouch.SetSetting(SettingId.UseQuickseek, 1);
                //}

                m_SoundTouch.SetSampleRate((int)audioPCMFormat.SampleRate);
                m_SoundTouch.SetChannels((int)audioPCMFormat.NumberOfChannels);

                //destStream = File.Open(fullpath_, FileMode.CreateNew, FileAccess.Write, FileShare.Write);
                destStream = new FileStream(fullpath_, FileMode.Create, FileAccess.Write, FileShare.None);
                audioStreamRiffOffset = audioPCMFormat.RiffHeaderWrite(destStream, 0);
                DebugFix.Assert((long)audioStreamRiffOffset == destStream.Position);


                int sampleSizePerChannel = audioPCMFormat.BitDepth / 8;
                // == audioPCMFormat.BlockAlign / audioPCMFormat.NumberOfChannels;

#if DEBUG
                int sizeOfTypeInBytes = Marshal.SizeOf(typeof(TSampleType));
                DebugFix.Assert(sizeOfTypeInBytes == sampleSizePerChannel);

                sizeOfTypeInBytes = sizeof(TSampleType);
                DebugFix.Assert(sizeOfTypeInBytes == sampleSizePerChannel);
#endif // DEBUG

                int bytesToTransfer =
                    (int)
                        Math.Min(audioStream.Length - (long)audioStreamRiffOffset, audioPCMFormat.ConvertTimeToBytes(2000 * AudioLibPCMFormat.TIME_UNIT)); // automatically block-aligned to frame size (sample bytes * number of channels)

                if (m_SoundTouch_ByteBuffer == null)
                {
                    Console.WriteLine("ALLOCATING m_SoundTouch_ByteBuffer");
                    m_SoundTouch_ByteBuffer = new byte[bytesToTransfer];
                }
                //else if (m_SoundTouch_ByteBuffer.Length < bytesToTransfer)
                //{
                //    Console.WriteLine("m_SoundTouch_ByteBuffer.resize");
                //    Array.Resize(ref m_SoundTouch_ByteBuffer, bytesToTransfer);
                //}

                //int bytesToReadFromAudioStream = bytesToTransferToCircularBuffer;
                //DebugFix.Assert(bytesToReadFromAudioStream <= bytesToTransferToCircularBuffer);

                int bytesReadFromAudioStream = 0;

                long totalAudioBytes = audioStream.Length;
                long currentAudioBytes = 0;
                int previousPercent = -100;

                while ((bytesReadFromAudioStream = audioStream.Read(m_SoundTouch_ByteBuffer, 0, bytesToTransfer)) > 0)
                {
                    if (RequestCancellation) return;

                    currentAudioBytes += bytesReadFromAudioStream;

                    int percent = (int)Math.Round(100.0 * currentAudioBytes / (double)totalAudioBytes);

                    previousPercent = percent;

                    reportProgress_Throttle(percent, "[ " + percent + "% ] " +
                        Math.Round(currentAudioBytes / (double)1024) + " / " +
                        Math.Round(totalAudioBytes / (double)1024) + " (kB)");

                    DebugFix.Assert(bytesReadFromAudioStream <= bytesToTransfer);

                    int soundTouch_SampleBufferLength = (bytesReadFromAudioStream * 8) / audioPCMFormat.BitDepth; // 16


                    if (m_SoundTouch_SampleBuffer == null)
                    {
                        Console.WriteLine("ALLOCATING m_SoundTouch_SampleBuffer");
                        m_SoundTouch_SampleBuffer = new TSampleType[soundTouch_SampleBufferLength];
                    }
                    //else if (m_SoundTouch_SampleBuffer.Length < soundTouch_SampleBufferLength)
                    //{
                    //    Console.WriteLine("m_SoundTouch_SampleBuffer.resize");
                    //    Array.Resize(ref m_SoundTouch_SampleBuffer, soundTouch_SampleBufferLength);
                    //}

                    int sampleBufferIndex = 0;
                    for (int i = 0; i < bytesReadFromAudioStream; ) //i += m_CurrentAudioPCMFormat.BlockAlign)
                    {
                        if (RequestCancellation) return;

                        //short sampleLeft = 0;
                        //short sampleRight = 0;

                        for (int channel = 0; channel < audioPCMFormat.NumberOfChannels; channel++)
                        {
                            if (RequestCancellation) return;

                            byte byte1 = m_SoundTouch_ByteBuffer[i++];
                            byte byte2 = m_SoundTouch_ByteBuffer[i++];

                            short sample =
                                BitConverter.IsLittleEndian
                                    ? (short)(byte1 | (byte2 << 8))
                                    : (short)((byte1 << 8) | byte2);

#if DEBUG
                            //// Little Endian
                            //short s1 = (short)(byte1 | (byte2 << 8));
                            //short s2 = (short)(byte1 + byte2 * 256);

                            //// Big Endian
                            //short s3 = (short)((byte1 << 8) | byte2);
                            //short s4 = (short)(byte1 * 256 + byte2);

                            short checkedSample = BitConverter.ToInt16(m_SoundTouch_ByteBuffer, i - 2);
                            DebugFix.Assert(checkedSample == sample);

                            checkedSample = (short)(byte1 + byte2 * 256);
                            DebugFix.Assert(checkedSample == sample);
#endif //DEBUG

                            //if (channel == 0)
                            //{
                            //    sampleLeft = sample;
                            //}
                            //else
                            //{
                            //    sampleRight = sample;
                            //}

                            m_SoundTouch_SampleBuffer[sampleBufferIndex++] = sample;
                        }

                        //m_SoundTouch_SampleBuffer[sampleBufferIndex - 2] = sampleRight; // sampleLeft;
                        //m_SoundTouch_SampleBuffer[sampleBufferIndex - 1] = 0; // sampleRight;
                    }


                    int soundTouch_SampleBufferLength_Channels = soundTouch_SampleBufferLength /
                                                                 audioPCMFormat.NumberOfChannels;

                    int soundTouch_SampleBufferLength_Channels_FULL = m_SoundTouch_SampleBuffer.Length /
                                                                      audioPCMFormat.NumberOfChannels;

                    //m_SoundTouch.Flush();

                    int samplesReceived = -1;

                    //while (
                    //    (samplesReceived = m_SoundTouch.ReceiveSamples(
                    //        (ArrayPtr<TSampleType>)m_SoundTouch_SampleBuffer,
                    //        soundTouch_SampleBufferLength_Channels_FULL
                    //        )) > 0)
                    //{
                    //    // Ignore.
                    //    bool debug = true;
                    //}

                    m_SoundTouch.PutSamples((ArrayPtr<TSampleType>)m_SoundTouch_SampleBuffer,
                        soundTouch_SampleBufferLength_Channels);

                    int totalBytesReceivedFromSoundTouch = 0;

                    //totalBytesReceivedFromSoundTouch = soundTouch_SampleBufferLength * sampleSizePerChannel;

                    while (
                        (samplesReceived = m_SoundTouch.ReceiveSamples(
                            (ArrayPtr<TSampleType>)m_SoundTouch_SampleBuffer,
                            soundTouch_SampleBufferLength_Channels_FULL
                            )) > 0)
                    {
                        if (RequestCancellation) return;

                        samplesReceived *= audioPCMFormat.NumberOfChannels;

                        int bytesReceivedFromSoundTouch = samplesReceived * sampleSizePerChannel;

                        int predictedTotal = totalBytesReceivedFromSoundTouch + bytesReceivedFromSoundTouch;
                        if ( //predictedTotal > bytesReadFromAudioStream ||
                            predictedTotal > m_SoundTouch_ByteBuffer.Length)
                        {
#if DEBUG
                            // The breakpoint should never hit,
                            // because the output audio data is smaller than the source
                            // due to the time stretching making the playback faster.
                            // (the ratio is about the same as factor)

                            //DebugFix.Assert(factor < 1);

                            if (m_factor >= 1)
                            {
                                Debugger.Break();
                            }
#endif
                            break;
                        }

                        //unsafe
                        //{
                        //    fixed (short* pShorts = m_SoundTouch_SampleBuffer)
                        //        Marshal.Copy((IntPtr)pShorts, 0, m_SoundTouch_ByteBuffer, actualSamples);
                        //}

                        if (false)
                        {
                            // TODO: check little / big endian
                            Buffer.BlockCopy(m_SoundTouch_SampleBuffer,
                                0,
                                m_SoundTouch_ByteBuffer,
                                totalBytesReceivedFromSoundTouch,
                                bytesReceivedFromSoundTouch);
                        }
                        else
                        {
                            //try
                            //{
                            //}
                            //catch (Exception ex)
                            //{
                            //    Debugger.Break();
                            //}

                            int checkTotalBytes = 0;

                            for (int s = 0; s < samplesReceived; s += audioPCMFormat.NumberOfChannels)
                            {
                                if (RequestCancellation) return;

                                if (false)
                                {
                                    int sampleSizePerChannels = sampleSizePerChannel * audioPCMFormat.NumberOfChannels;

                                    checkTotalBytes += sampleSizePerChannels;

                                    // TODO: check little / big endian
                                    Buffer.BlockCopy(m_SoundTouch_SampleBuffer,
                                        s * sampleSizePerChannel,
                                        m_SoundTouch_ByteBuffer,
                                        totalBytesReceivedFromSoundTouch
                                        + s * sampleSizePerChannel,
                                        sampleSizePerChannels);
                                }
                                else
                                {
                                    for (int channel = 0; channel < audioPCMFormat.NumberOfChannels; channel++)
                                    {
                                        if (RequestCancellation) return;

                                        if (true)
                                        {
                                            checkTotalBytes += sampleSizePerChannel;

                                            sampleBufferIndex = (s + channel);
                                            //TSampleType sample = m_SoundTouch_SampleBuffer[sampleBufferIndex];

                                            int byteIndex = sampleBufferIndex * sampleSizePerChannel;

                                            // TODO: check little / big endian
                                            Buffer.BlockCopy(m_SoundTouch_SampleBuffer,
                                                byteIndex,
                                                m_SoundTouch_ByteBuffer,
                                                totalBytesReceivedFromSoundTouch + byteIndex,
                                                sampleSizePerChannel);
                                        }
                                        else
                                        {
                                            TSampleType sample = m_SoundTouch_SampleBuffer[s + channel];

                                            byte[] sampleBytes;
                                            if (BitConverter.IsLittleEndian)
                                            {
                                                sampleBytes = BitConverter.GetBytes(sample);
                                            }
                                            else
                                            {
                                                sampleBytes =
                                                    BitConverter.GetBytes(
                                                        (short)((sample & 0xFF) << 8 | (sample & 0xFF00) >> 8));
                                            }

                                            DebugFix.Assert(sampleSizePerChannel == sampleBytes.Length);

                                            checkTotalBytes += sampleSizePerChannel;

                                            Buffer.BlockCopy(sampleBytes,
                                                0,
                                                m_SoundTouch_ByteBuffer,
                                                totalBytesReceivedFromSoundTouch
                                                + (s + channel) * sampleSizePerChannel,
                                                sampleSizePerChannel);
                                        }
                                    }
                                }
                            }

                            DebugFix.Assert(checkTotalBytes == bytesReceivedFromSoundTouch);


                            //if (bytesReceivedFromSoundTouch > 0
                            //    //&& totalBytesReceivedFromSoundTouch <= circularBufferBytesAvailableForWriting
                            //    )
                            //{
                            //    destStream.Write(m_SoundTouch_ByteBuffer, totalBytesReceivedFromSoundTouch, bytesReceivedFromSoundTouch);
                            //}
                        }

                        totalBytesReceivedFromSoundTouch += bytesReceivedFromSoundTouch;

                        //if (//totalBytesReceivedFromSoundTouch >= bytesReadFromAudioStream ||
                        //    totalBytesReceivedFromSoundTouch >= circularBufferBytesAvailableForWriting)
                        //{
                        //    break;
                        //}
                    }

                    if (totalBytesReceivedFromSoundTouch > 0
                        //&& totalBytesReceivedFromSoundTouch <= circularBufferBytesAvailableForWriting
                        )
                    {
                        destStream.Write(m_SoundTouch_ByteBuffer, 0, totalBytesReceivedFromSoundTouch);
                    }
                }

                okay = true;
            }
            catch (Exception ex)
            {
                okay = false;
            }
            finally
            {
                if (audioStream != null)
                {
                    audioStream.Close();
                }
                if (destStream != null)
                {
                    destStream.Position = 0;
                    audioStreamRiffOffset = audioPCMFormat.RiffHeaderWrite(
                        destStream,
                        (uint)
                            (destStream.Length -
                             (long)
                                 audioStreamRiffOffset));
                    destStream.Close();
                }
            }

            if (okay && File.Exists(fullpath_))
            {
                File.Delete(m_fullpath);
                File.Move(fullpath_, m_fullpath);
                try
                {
                    File.SetAttributes(m_fullpath, FileAttributes.Normal);
                }
                catch
                {
                }
            }
        }
    }

    public class WavFormatConverter : DualCancellableProgressReporter
    {
        static WavFormatConverter()
        {
#if DEBUG
            string workingDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string lameExe = Path.Combine(workingDir, "lame.exe");
            if (!File.Exists(lameExe))
            {
                Debugger.Break();
            }
            string faadExe = Path.Combine(workingDir, "faad.exe");
            if (!File.Exists(faadExe))
            {
                Debugger.Break();
            }
#endif //DEBUG
        }


        private Process m_process = null;
        private void cancellationRequestedEvent(object sender, DualCancellableProgressReporter.CancellationRequestedEventArgs e)
        {
            if (m_process != null)
            {
                try
                {
                    m_process.Kill();
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debugger.Break();
#endif //DEBUG
                }
            }
        }

        public const string AUDIO_MP3_EXTENSION = ".mp3";
        public const string AUDIO_WAV_EXTENSION = ".wav";

        public override void DoWork()
        {
            //RequestCancellation = false;
            //CancellationRequestedEvent += cancellationRequestedEvent;
        }

        private bool m_OverwriteOutputFiles;
        public bool OverwriteOutputFiles
        {
            get { return m_OverwriteOutputFiles; }
            set { m_OverwriteOutputFiles = value; }
        }

        public WavFormatConverter(bool overwriteOutputFiles, bool skipACM)
        {
            OverwriteOutputFiles = overwriteOutputFiles;
            m_SkipACM = skipACM;

            CancellationRequestedEvent += cancellationRequestedEvent;
        }

        public string ConvertSampleRate(string sourceFile, string destinationDirectory, AudioLibPCMFormat pcmFormat, out AudioLibPCMFormat originalPcmFormat)
        {
            if (!File.Exists(sourceFile))
                throw new FileNotFoundException("Invalid source file path");

            if (!Directory.Exists(destinationDirectory))
                throw new FileNotFoundException("Invalid destination directory");

            string destinationFilePath = null;
            WaveStream sourceStream = null;
            WaveFormatConversionStream conversionStream = null;

            try
            {
                WaveFormat destFormat = new WaveFormat((int)pcmFormat.SampleRate,
                                                                   pcmFormat.BitDepth,
                                                                   pcmFormat.NumberOfChannels);
                sourceStream = new WaveFileReader(sourceFile);

                conversionStream = new WaveFormatConversionStream(destFormat, sourceStream);

                destinationFilePath = GenerateOutputFileFullname(sourceFile, destinationDirectory, pcmFormat);

                WaveFormat sourceFormat = sourceStream.WaveFormat;
                originalPcmFormat = new AudioLibPCMFormat((ushort)sourceFormat.Channels, (uint)sourceFormat.SampleRate, (ushort)sourceFormat.BitsPerSample);
                bool formatsEqual1 = sourceFormat.Equals(destFormat);
                bool formatsEqual2 = originalPcmFormat.Equals(pcmFormat);
                DebugFix.Assert(formatsEqual1 && formatsEqual1 || !formatsEqual1 && !formatsEqual1);
                if (formatsEqual1 || formatsEqual2)
                {
#if DEBUG
                    Debugger.Break();
#endif //DEBUG
                    File.Copy(sourceFile, destinationFilePath);
                    try
                    {
                        File.SetAttributes(destinationFilePath, FileAttributes.Normal);
                    }
                    catch
                    {
                    }
                }
                else
                {

                    //WaveFileWriter.CreateWaveFile(destinationFilePath, conversionStream);
                    using (WaveFileWriter writer = new WaveFileWriter(destinationFilePath, conversionStream.WaveFormat))
                    {
                        //const int BUFFER_SIZE = 1024 * 8; // 8 KB MAX BUFFER
                        int BUFFER_SIZE = (int)pcmFormat.ConvertTimeToBytes(1500 * AudioLibPCMFormat.TIME_UNIT);
                        //int debug = pcmStream.GetReadSize(4000);
                        byte[] buffer = new byte[BUFFER_SIZE];
                        int byteRead;
                        writer.Flush();

                        string msg = Path.GetFileName(sourceFile) + " / " + Path.GetFileName(destinationFilePath);
                        //"Resampling WAV audio...";
                        reportProgress(-1, msg);

                        while ((byteRead = conversionStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            if (RequestCancellation)
                            {
                                writer.Close();
                                if (File.Exists(destinationFilePath))
                                {
                                    File.Delete(destinationFilePath);
                                }
                                return null;
                            }

                            int percent = (int)(100.0 * sourceStream.Position / sourceStream.Length);
                            reportProgress_Throttle(percent, msg); // + sourceStream.Position + "/" + sourceStream.Length);

                            writer.WriteData(buffer, 0, byteRead);
                        }
                    }
                }
            }
            finally
            {
                if (conversionStream != null)
                {
                    conversionStream.Close();
                }
                if (sourceStream != null)
                {
                    sourceStream.Close();
                }
            }

            return destinationFilePath;
        }

        private AudioLibPCMFormat Mp3ToWav_Mp3Sharp(string mp3FilePath, string destinationDirectory, out string wavFilePath)
        {
            Stream mp3Stream = File.Open(mp3FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);

            Bitstream mp3BitStream = new Bitstream(new BackStream(mp3Stream, 1024 * 10));
            Header mp3Frame = mp3BitStream.readFrame();

            AudioLibPCMFormat mp3PcmFormat = new AudioLibPCMFormat(
                (ushort)(mp3Frame.mode() == Header.SINGLE_CHANNEL ? 1 : 2),
                (uint)mp3Frame.frequency(),
                16);

            wavFilePath = GenerateOutputFileFullname(
                                    mp3FilePath + WavFormatConverter.AUDIO_WAV_EXTENSION,
                                    destinationDirectory,
                                    mp3PcmFormat);

            FileStream wavFileStream = new FileStream(wavFilePath, FileMode.Create, FileAccess.Write, FileShare.Read);

            ulong wavFilRiffHeaderLength = mp3PcmFormat.RiffHeaderWrite(wavFileStream, 0);

            long wavFileStreamStart = wavFileStream.Position;

            Decoder decoder = new Decoder();
            ObufferStreamWrapper outputBuffer = new ObufferStreamWrapper(wavFileStream, mp3PcmFormat.NumberOfChannels);
            decoder.OutputBuffer = outputBuffer;

            string msg = Path.GetFileName(mp3FilePath) + " / " + Path.GetFileName(wavFilePath); // "Decoding MP3 to WAV audio (CSharp Lib)..."
            reportProgress(-1, msg);

            try
            {
                while (true)
                {
                    if (RequestCancellation)
                    {
                        mp3Stream.Close();
                        wavFileStream.Close();
                        if (File.Exists(wavFilePath))
                        {
                            File.Delete(wavFilePath);
                        }
                        return null;
                    }

                    int percent = (int)(100.0 * mp3Stream.Position / mp3Stream.Length);
                    reportProgress_Throttle(percent, msg); // + mp3Stream.Position + "/" + mp3Stream.Length);


                    Header header = mp3BitStream.readFrame();
                    if (header == null)
                    {
                        break;
                    }

                    try
                    {
                        Obuffer decoderOutput = decoder.decodeFrame(header, mp3BitStream);
                    }
                    catch
                    {
                        mp3BitStream.closeFrame();
                        continue;
                    }
                    mp3BitStream.closeFrame();
                }
            }
            finally
            {
            }

            mp3Stream.Close();

            long wavFileStreamEnd = wavFileStream.Position;

            if ((ulong)wavFileStreamEnd == wavFilRiffHeaderLength)
            {
                wavFileStream.Close();
                if (File.Exists(wavFilePath))
                {
                    File.Delete(wavFilePath);
                }
                return null;
            }

            wavFileStream.Position = 0;
            mp3PcmFormat.RiffHeaderWrite(wavFileStream, (uint)(wavFileStreamEnd - wavFileStreamStart));

            wavFileStream.Close();

            return mp3PcmFormat;
        }

        private AudioLibPCMFormat Mp3ToWav_NAudio(string mp3FilePath, string destinationDirectory, out string wavFilePath)
        {
            AudioLibPCMFormat mp3PcmFormat = null;

            using (Mp3FileReader reader = new Mp3FileReader(mp3FilePath))
            {
                using (WaveStream pcmStream = WaveFormatConversionStream.CreatePcmStream(reader))
                {
                    mp3PcmFormat = new AudioLibPCMFormat(
                        (ushort)pcmStream.WaveFormat.Channels,
                        (uint)pcmStream.WaveFormat.SampleRate,
                        (ushort)pcmStream.WaveFormat.BitsPerSample);

                    wavFilePath = GenerateOutputFileFullname(
                                            mp3FilePath + WavFormatConverter.AUDIO_WAV_EXTENSION,
                                            destinationDirectory,
                                            mp3PcmFormat);

                    //WaveFileWriter.CreateWaveFile(wavFilePath, pcmStream);
                    using (WaveFileWriter writer = new WaveFileWriter(wavFilePath, pcmStream.WaveFormat))
                    {
                        //const int BUFFER_SIZE = 1024 * 8; // 8 KB MAX BUFFER
                        int BUFFER_SIZE = (int)mp3PcmFormat.ConvertTimeToBytes(1500 * AudioLibPCMFormat.TIME_UNIT);
                        //int debug = pcmStream.GetReadSize(4000);
                        byte[] buffer = new byte[BUFFER_SIZE];
                        int byteRead;
                        writer.Flush();

                        string msg = Path.GetFileName(mp3FilePath) + " / " + Path.GetFileName(wavFilePath); //"Decoding MP3 to WAV audio (ACM Codec) ..."
                        reportProgress(-1, msg);

                        try
                        {
                            while ((byteRead = pcmStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                if (RequestCancellation)
                                {
                                    writer.Close();
                                    if (File.Exists(wavFilePath))
                                    {
                                        File.Delete(wavFilePath);
                                    }
                                    return null;
                                }

                                int percent = (int)(100.0 * reader.Position / reader.Length);
                                reportProgress_Throttle(percent, msg); // + reader.Position + "/" + reader.Length);

                                writer.WriteData(buffer, 0, byteRead);
                            }
                        }
                        finally
                        {

                        }
                    }
                }
            }

            return mp3PcmFormat;
        }

        public string UnCompressMp4_AACFile(string sourceFile, string destinationDirectory, AudioLibPCMFormat pcmFormat, out AudioLibPCMFormat originalPcmFormat)
        {
            if (!File.Exists(sourceFile))
                throw new FileNotFoundException("Invalid source file path");

            if (!Directory.Exists(destinationDirectory))
                throw new FileNotFoundException("Invalid destination directory");

            string destinationFile = GenerateOutputFileFullname(
                                    sourceFile + WavFormatConverter.AUDIO_WAV_EXTENSION,
                                    destinationDirectory,
                                    null);
            string tempFile = Path.Combine(Path.GetTempPath(),
                Path.GetFileName(destinationFile));

            //        AudioLibPCMFormat mp4PcmFormat = new AudioLibPCMFormat(
            //            (ushort)pcmStream.WaveFormat.Channels,
            //            (uint)pcmStream.WaveFormat.SampleRate,
            //            (ushort)pcmStream.WaveFormat.BitsPerSample);

            //AudioLibPCMFormat mp4PcmFormat = new AudioLibPCMFormat(
            //    (ushort)(mp3Frame.mode() == Header.SINGLE_CHANNEL ? 1 : 2),
            //    (uint)mp3Frame.frequency(),
            //    16);


            AudioLibPCMFormat mp4PcmFormat = null;

            originalPcmFormat = null;

            bool okay = false;
            try
            {
                if (File.Exists(tempFile)) File.Delete(tempFile);
                //Console.WriteLine("Temp: " + tempFileName);
                //Console.WriteLine("Destination: " + destinationFile);

                string workingDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

                if (m_process != null)
                {
#if DEBUG
                    Debugger.Break();
#endif //DEBUG
                }

                m_process = new Process();

                m_process.StartInfo.FileName = Path.Combine(workingDir, "faad.exe");
                m_process.StartInfo.RedirectStandardOutput = false;
                m_process.StartInfo.RedirectStandardError = false;
                m_process.StartInfo.UseShellExecute = true;
                m_process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                m_process.StartInfo.Arguments = "-o \"" + tempFile + "\" \"" + sourceFile + "\"";

                m_process.Start();
                m_process.WaitForExit();

                if (RequestCancellation)
                {
                    okay = false;
                }
                else
                {
                    okay = true;

                    if (!m_process.StartInfo.UseShellExecute && m_process.ExitCode != 0)
                    {
                        StreamReader stdErr = m_process.StandardError;
                        if (!stdErr.EndOfStream)
                        {
                            string toLog = stdErr.ReadToEnd();
                            if (!string.IsNullOrEmpty(toLog))
                            {
                                Console.WriteLine(toLog);
                            }
                        }
                    }
                    else if (!m_process.StartInfo.UseShellExecute)
                    {
                        StreamReader stdOut = m_process.StandardOutput;
                        if (!stdOut.EndOfStream)
                        {
                            string toLog = stdOut.ReadToEnd();
                            if (!string.IsNullOrEmpty(toLog))
                            {
                                Console.WriteLine(toLog);
                            }
                        }
                    }
                }
                if (File.Exists(tempFile))
                {
                    File.Move(tempFile, destinationFile);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);

#if DEBUG
                Debugger.Break();
#endif //DEBUG
                okay = false;
            }
            finally
            {
                //noop
            }

            if (okay && File.Exists(destinationFile))
            {
                uint dataLength;
                Stream stream = null;
                try
                {
                    stream = File.Open(destinationFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                    if (stream != null)
                    {
                        mp4PcmFormat = AudioLibPCMFormat.RiffHeaderParse(stream, out dataLength);
                    }
                }
                finally
                {
                    if (stream != null)
                    {
                        stream.Close();
                    }
                }

                if (mp4PcmFormat != null)
                {
                    originalPcmFormat = new AudioLibPCMFormat();
                    originalPcmFormat.CopyFrom(mp4PcmFormat);
                }

                if (mp4PcmFormat != null)
                {
                    string destinationFileUPDATED = GenerateOutputFileFullname(
                        sourceFile + WavFormatConverter.AUDIO_WAV_EXTENSION,
                        destinationDirectory,
                        mp4PcmFormat);

                    try
                    {
                        File.Move(destinationFile, destinationFileUPDATED);
                        try
                        {
                            File.SetAttributes(destinationFileUPDATED, FileAttributes.Normal);
                        }
                        catch
                        {
                        }
                        destinationFile = destinationFileUPDATED;
                    }
                    catch
                    {
                    }
                }
            }

            if (!okay || mp4PcmFormat == null || RequestCancellation)
            {
                if (File.Exists(destinationFile))
                {
                    File.Delete(destinationFile);
                }
                return null;
            }

            DebugFix.Assert(File.Exists(destinationFile));

            if (pcmFormat == null) // auto detect
            {
                pcmFormat = mp4PcmFormat;
            }

            if (pcmFormat != null
                &&
                !mp4PcmFormat.IsCompatibleWith(pcmFormat))
            {
                AudioLibPCMFormat originalWavPcmFormat;
                string newDestinationFilePath = ConvertSampleRate(destinationFile, destinationDirectory, pcmFormat, out originalWavPcmFormat);
                if (originalWavPcmFormat != null)
                {
                    DebugFix.Assert(mp4PcmFormat.Equals(originalWavPcmFormat));
                }


                if (File.Exists(destinationFile))
                {
                    File.Delete(destinationFile);
                }
                if (RequestCancellation)
                {
                    return null;
                }
                return newDestinationFilePath;
            }

            return destinationFile;
        }

        private bool m_SkipACM;
        public string UnCompressMp3File(string sourceFile, string destinationDirectory, AudioLibPCMFormat pcmFormat, out AudioLibPCMFormat originalPcmFormat)
        {
            if (!File.Exists(sourceFile))
                throw new FileNotFoundException("Invalid source file path");

            if (!Directory.Exists(destinationDirectory))
                throw new FileNotFoundException("Invalid destination directory");

            originalPcmFormat = null;

            string destinationFile = null;
            AudioLibPCMFormat mp3PcmFormat = null;

            #region PERFORMANCE TESTING
            //Stopwatch watch = new Stopwatch();

            //watch.Start();
            //mp3PcmFormat = Mp3ToWav_Mp3Sharp(sourceFile, destinationFile);
            //watch.Stop();
            //long csMS = watch.ElapsedMilliseconds;

            //if (File.Exists(destinationFile))
            //{
            //    File.Delete(destinationFile);
            //}

            //watch.Reset();
            //watch.Start();
            //mp3PcmFormat = Mp3ToWav_NAudio(sourceFile, destinationFile);
            //watch.Stop();
            //long acmMS = watch.ElapsedMilliseconds;

            //DebugFix.Assert(acmMS < csMS);
            //Console.WriteLine("C# / ACM RATIO: " + csMS / acmMS);
            #endregion

            if (!m_SkipACM)
            {
                try
                {
                    mp3PcmFormat = Mp3ToWav_NAudio(sourceFile, destinationDirectory, out destinationFile);
                }
                catch (Exception ex)
                {
                    mp3PcmFormat = null;
                    if (ex.GetType().FullName.IndexOf("acm", StringComparison.OrdinalIgnoreCase) >= 0
                        || ex.Message.IndexOf("acm", StringComparison.OrdinalIgnoreCase) >= 0
                        ) // Hacky sucky !! Needs testing on machines that don't support ACM
                    {
#if DEBUG
                        Debugger.Break();
#endif
                        m_SkipACM = true;
                    }
                    if (destinationFile != null && File.Exists(destinationFile))
                    {
                        File.Delete(destinationFile);
                    }

                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            }

            if (mp3PcmFormat != null)
            {
                originalPcmFormat = new AudioLibPCMFormat();
                originalPcmFormat.CopyFrom(mp3PcmFormat);
            }

            if (RequestCancellation)
            {
                return null;
            }
            if (mp3PcmFormat == null || m_SkipACM)
            {
                mp3PcmFormat = Mp3ToWav_Mp3Sharp(sourceFile, destinationDirectory, out destinationFile);
            }

            if (mp3PcmFormat == null || RequestCancellation)
            {
                if (File.Exists(destinationFile))
                {
                    File.Delete(destinationFile);
                }
                return null;
            }

            if (pcmFormat == null) // auto detect
            {
                pcmFormat = mp3PcmFormat;
            }

            DebugFix.Assert(File.Exists(destinationFile));

            if (pcmFormat != null
                &&
                !mp3PcmFormat.IsCompatibleWith(pcmFormat))
            {
                AudioLibPCMFormat originalWavPcmFormat;
                string newDestinationFilePath = ConvertSampleRate(destinationFile, destinationDirectory, pcmFormat, out originalWavPcmFormat);
                if (originalWavPcmFormat != null)
                {
                    DebugFix.Assert(mp3PcmFormat.Equals(originalWavPcmFormat));
                }

                if (File.Exists(destinationFile))
                {
                    File.Delete(destinationFile);
                }
                if (RequestCancellation)
                {
                    return null;
                }
                return newDestinationFilePath;
            }

            return destinationFile;
        }

        public string UnCompressWavFile(string sourceFile, string destinationDirectory, AudioLibPCMFormat pcmFormat)
        {
            throw new System.NotImplementedException();

            // following code fails at ACM codec, so commented for now.
            /*
            if (!File.Exists(sourceFile))
                throw new FileNotFoundException("Invalid source file path");

            if (!Directory.Exists(destinationDirectory))
                throw new FileNotFoundException("Invalid destination directory");

            string destinationFilePath = null;
            WaveStream sourceStream = null;
            WaveFormatConversionStream conversionStream = null;
            try
                {
                WaveFormat destFormat = new WaveFormat ( (int)destinationPCMFormat.SampleRate,
                                                                   destinationPCMFormat.BitDepth,
                                                                   destinationPCMFormat.NumberOfChannels );
                sourceStream = new WaveFileReader ( sourceFile );

                WaveStream intermediateStream = WaveFormatConversionStream.CreatePcmStream ( sourceStream );
                conversionStream = new WaveFormatConversionStream ( destFormat, intermediateStream);

                destinationFilePath = GenerateOutputFileFullname ( sourceFile, destinationDirectory, destinationPCMFormat );
                WaveFileWriter.CreateWaveFile ( destinationFilePath, conversionStream );
                }
                        finally
                {
                if (conversionStream != null)
                    {
                    conversionStream.Close ();
                    }
                if (sourceStream != null)
                    {
                    sourceStream.Close ();
                    }
                }
            return destinationFilePath;
             */
        }

        public bool CompressWavToMp3(string sourceFile, string destinationFile, AudioLibPCMFormat pcmFormat, ushort bitRate_mp3Output)
        {
            return CompressWavToMp3(sourceFile, destinationFile, pcmFormat, bitRate_mp3Output, null, true, null);
        }

        public bool CompressWavToMp3(string sourceFile, string destinationFile, AudioLibPCMFormat pcmFormat, ushort bitRate_mp3Output, string extraparamChannels, bool extraParamResample, string extraParamReplayGain)
        {
            if (!File.Exists(sourceFile))
                throw new FileNotFoundException("Invalid source file path " + sourceFile);

            bool okay = false;
            try
            {
                string LameWorkingDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                //string outputFilePath = Path.Combine (
                //Path.GetDirectoryName ( sourceFile ),
                //Path.GetFileNameWithoutExtension ( sourceFile ) + DataProviderFactory.AUDIO_MP3_EXTENSION );

                if (pcmFormat.SampleRate < 22050)
                {
                    bitRate_mp3Output = 32;
                }

                string sampleRate = String.Format("{0}", pcmFormat.SampleRate); //Math.Round(pcmFormat.SampleRate / 1000.0, 3));
                sampleRate = sampleRate.Substring(0, 2) + '.' + sampleRate.Substring(2, 3);
                string sampleRateArg = " --resample " + sampleRate;
                if (!extraParamResample) sampleRateArg = "";

                string replayGainArg = !string.IsNullOrEmpty(extraParamReplayGain) ? (extraParamReplayGain.StartsWith(" ") ? extraParamReplayGain : " " + extraParamReplayGain) :
                    "";

                string channelsArg = pcmFormat.NumberOfChannels == 1 ? "m" : "s";
                if (!string.IsNullOrEmpty(extraparamChannels)) channelsArg = extraparamChannels;
                //string argumentString = "-b " + bitRate_mp3Output.ToString ()  + " --cbr --resample default -m m \"" + sourceFile + "\" \"" + destinationFile + "\"";
                string argumentString = "-b " + bitRate_mp3Output.ToString()
                    + " --cbr"
                    + sampleRateArg
                    + " -m " + channelsArg
                    + replayGainArg
                    + " \"" + sourceFile + "\" \"" + destinationFile + "\"";
                Console.WriteLine(argumentString);

                if (m_process != null)
                {
#if DEBUG
                    Debugger.Break();
#endif //DEBUG
                }

                m_process = new Process();

                m_process.StartInfo.FileName = Path.Combine(LameWorkingDir, "lame.exe");
                m_process.StartInfo.RedirectStandardOutput = false;
                m_process.StartInfo.RedirectStandardError = false;
                m_process.StartInfo.UseShellExecute = true;
                m_process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                m_process.StartInfo.Arguments = argumentString;
                //{
                //FileName = Path.Combine(LameWorkingDir, "lame.exe"),

                //RedirectStandardError = false,
                //RedirectStandardOutput = false,
                //UseShellExecute = true,
                //WindowStyle = ProcessWindowStyle.Hidden,
                //Arguments = argumentString
                //}
                //};
                m_process.Start();
                m_process.WaitForExit();


                if (RequestCancellation)
                {
                    okay = false;
                }
                else
                {
                    okay = true;

                    if (!m_process.StartInfo.UseShellExecute && m_process.ExitCode != 0)
                    {
                        StreamReader stdErr = m_process.StandardError;
                        if (!stdErr.EndOfStream)
                        {
                            string toLog = stdErr.ReadToEnd();
                            if (!string.IsNullOrEmpty(toLog))
                            {
                                Console.WriteLine(toLog);
                            }
                        }
                    }
                    else if (!m_process.StartInfo.UseShellExecute)
                    {
                        StreamReader stdOut = m_process.StandardOutput;
                        if (!stdOut.EndOfStream)
                        {
                            string toLog = stdOut.ReadToEnd();
                            if (!string.IsNullOrEmpty(toLog))
                            {
                                Console.WriteLine(toLog);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);

#if DEBUG
                Debugger.Break();
#endif //DEBUG
                okay = false;
            }
            finally
            {
                //noop
            }

            if (okay && File.Exists(destinationFile))
            {
                return true;
            }

            return false;

            //ProcessStartInfo process_Lame = new ProcessStartInfo();

            //process_Lame.FileName = Path.Combine(LameWorkingDir, "lame.exe");

            //process_Lame.Arguments = argumentString;

            //process_Lame.WindowStyle = ProcessWindowStyle.Hidden;
            ////System.Windows.Forms.MessageBox.Show ( process_Lame.FileName );
            ////System.Windows.Forms.MessageBox.Show ( process_Lame.Arguments );
            //Process p = Process.Start(process_Lame);
            //p.WaitForExit();
        }

        public bool CompressWavToMP4And3GP(string sourceFile, string destinationFile, AudioLibPCMFormat pcmFormat, ushort bitRate_Output)
        {
            string ffmpegWorkingDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string ffmpegPath = Path.Combine(ffmpegWorkingDir, "ffmpeg.exe");
            if (!File.Exists(ffmpegPath))
                throw new FileNotFoundException("Invalid compression library path " + ffmpegPath);

            if (!File.Exists(sourceFile))
                throw new FileNotFoundException("Invalid source file path " + sourceFile);

            bool okay = false;
            try
            {

                string sampleRate = String.Format("{0}", pcmFormat.SampleRate); //Math.Round(pcmFormat.SampleRate / 1000.0, 3));
                sampleRate = sampleRate.Substring(0, 2) + '.' + sampleRate.Substring(2, 3);
                string sampleRateArg = " --resample " + sampleRate;
                

                string channelsArg = pcmFormat.NumberOfChannels == 1 ? "m" : "s";
                string argumentString = null;
                string extension = Path.GetExtension (destinationFile ).ToLower() ;

                if (extension == ".mp4" || extension == ".m4a")
                {
                    argumentString = "-i " + "\"" + sourceFile + "\"" + " -b:a " + bitRate_Output + " \"" + destinationFile + "\"";
                }
                else if (extension == ".3gp")
                {
                    argumentString =  String.Format(@"-i {0} -b 400k -acodec aac -strict experimental  -ac 1 -ar 16000 -ab 24k {1}", "\"" +sourceFile + "\"", "\"" +destinationFile + "\"");
                }
                else if (extension == ".amr")
                {
                    argumentString = String.Format(@"-i {0} -ar 8000 -ab 12.2k {1}", "\"" + sourceFile + "\"" , "\"" + destinationFile + "\"" );
                }
                    
                    
                Console.WriteLine(argumentString);

                if (m_process != null)
                {
#if DEBUG
                    Debugger.Break();
#endif //DEBUG
                }

                m_process = new Process();
                
                m_process.StartInfo.FileName = ffmpegPath;
                
                m_process.StartInfo.RedirectStandardOutput = false;
                m_process.StartInfo.RedirectStandardError = false;
                m_process.StartInfo.UseShellExecute = true;
                m_process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                m_process.StartInfo.Arguments = argumentString;
                
                m_process.Start();
                m_process.WaitForExit();


                if (RequestCancellation)
                {
                    okay = false;
                }
                else
                {
                    okay = true;

                    if (!m_process.StartInfo.UseShellExecute && m_process.ExitCode != 0)
                    {
                        StreamReader stdErr = m_process.StandardError;
                        if (!stdErr.EndOfStream)
                        {
                            string toLog = stdErr.ReadToEnd();
                            if (!string.IsNullOrEmpty(toLog))
                            {
                                Console.WriteLine(toLog);
                            }
                        }
                    }
                    else if (!m_process.StartInfo.UseShellExecute)
                    {
                        StreamReader stdOut = m_process.StandardOutput;
                        if (!stdOut.EndOfStream)
                        {
                            string toLog = stdOut.ReadToEnd();
                            if (!string.IsNullOrEmpty(toLog))
                            {
                                Console.WriteLine(toLog);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);

#if DEBUG
                Debugger.Break();
#endif //DEBUG
                okay = false;
            }
            finally
            {
                //noop
            }

            if (okay && File.Exists(destinationFile))
            {
                return true;
            }

            return false;

        }

        private string GenerateOutputFileFullname(string sourceFile, string destinationDirectory, AudioLibPCMFormat pcmFormat)
        {
            //FileInfo sourceFileInfo = new FileInfo(sourceFile);
            //string sourceFileName = sourceFileInfo.Name.Replace(sourceFileInfo.Extension, "");

            string sourceFileName = Path.GetFileNameWithoutExtension(sourceFile);
            string sourceFileExt = Path.GetExtension(sourceFile);
            if (!string.IsNullOrEmpty(sourceFileExt))
            {
                sourceFileExt = sourceFileExt.ToLower();
            }

            string channels = pcmFormat == null ? null : (pcmFormat.NumberOfChannels == 1 ? "Mono" : (pcmFormat.NumberOfChannels == 2 ? "Stereo" : pcmFormat.NumberOfChannels.ToString()));

            string destFile = null;

            if (OverwriteOutputFiles)
            {
                destFile = Path.Combine(destinationDirectory,
                                           sourceFileName

                                           +
                                           (pcmFormat == null ? "" :
                                           ("_"
                                           + pcmFormat.BitDepth
                                           + "-"
                                           + channels
                                           + "-"
                                           + pcmFormat.SampleRate))

                                           + sourceFileExt);
            }
            else
            {
                Random random = new Random();

                int loopCounter = 0;
                do
                {
                    loopCounter++;
                    if (loopCounter > 10000)
                    {
                        throw new Exception("Not able to generate destination file name");
                    }
                    string randomStr = "_" + random.Next(100000).ToString();

                    destFile = Path.Combine(destinationDirectory,
                                        sourceFileName

                                           +
                                           (pcmFormat == null ? "" :
                                           ("_"
                                           + pcmFormat.BitDepth
                                           + "-"
                                           + channels
                                           + "-"
                                           + pcmFormat.SampleRate))

                                        + randomStr
                                        + sourceFileExt);
                } while (File.Exists(destFile));
            }

            return destFile;
        }
    }

    internal class ObufferStreamWrapper : Obuffer
    {
        BinaryWriter m_WrappedStreamBinaryWriter;
        private short[] m_SamplesBuffer;
        private short[] m_SamplesBufferPointer;
        private int m_NumberOfChannels;

        public ObufferStreamWrapper(Stream outputStream, int numberOfChannels)
        {
            m_WrappedStreamBinaryWriter = new BinaryWriter(outputStream);
            m_NumberOfChannels = numberOfChannels;

            m_SamplesBuffer = new short[OBUFFERSIZE];
            m_SamplesBufferPointer = new short[MAXCHANNELS];

            clear_buffer();
        }

        public override void append(int channel, short value)
        {
            m_SamplesBuffer[m_SamplesBufferPointer[channel]] = value;
            m_SamplesBufferPointer[channel] += (short)m_NumberOfChannels;
        }

        public override void write_buffer(int val)
        {
            for (int sample = 0; sample < m_SamplesBufferPointer[0]; sample++)
            {
                m_WrappedStreamBinaryWriter.Write(m_SamplesBuffer[sample]);
            }

            clear_buffer();
        }

        public override void close()
        {
            m_WrappedStreamBinaryWriter.Close();
        }

        public override void clear_buffer()
        {
            for (int i = 0; i < m_NumberOfChannels; ++i)
            {
                m_SamplesBufferPointer[i] = (short)i;
            }
        }

        public override void set_stop_flag() { }
    }
}

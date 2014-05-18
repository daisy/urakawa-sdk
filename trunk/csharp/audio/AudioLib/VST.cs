#if ENABLE_VST

using System;
using System.IO;
using System.Diagnostics;

using Jacobi.Vst.Core;
using Jacobi.Vst.Core.Host;

//using Jacobi.Vst.Interop.Host;
//using Jacobi.Vst.Framework.Plugin;
//using Jacobi.Vst.Interop.Plugin;


namespace AudioLib.VST
{
    public class HostCommandStub : IVstHostCommandStub
    {
        #region IVstHostCommandsStub Members

        private IVstPluginContext m_PluginContext = null;
        public IVstPluginContext PluginContext
        {
            get { return m_PluginContext; }
            set { m_PluginContext = value; }
        }

        #endregion

        #region IVstHostCommands20 Members

        public bool BeginEdit(int index)
        {
            return false;
        }

        public VstCanDoResult CanDo(string cando)
        {
            return VstCanDoResult.Unknown;
        }

        public bool CloseFileSelector(VstFileSelect fileSelect)
        {
            return false;
        }

        public bool EndEdit(int index)
        {
            return false;
        }

        public VstAutomationStates GetAutomationState()
        {
            return VstAutomationStates.Off;
        }

        public int GetBlockSize()
        {
            return 1024;
        }

        public string GetDirectory()
        {
            return null;
        }

        public int GetInputLatency()
        {
            return 0;
        }

        public VstHostLanguage GetLanguage()
        {
            return VstHostLanguage.NotSupported;
        }

        public int GetOutputLatency()
        {
            return 0;
        }

        public VstProcessLevels GetProcessLevel()
        {
            return VstProcessLevels.Unknown;
        }

        public string GetProductString()
        {
            return "VST.NET";
        }

        public float GetSampleRate()
        {
            return 44.1f;
        }

        public VstTimeInfo GetTimeInfo(VstTimeInfoFlags filterFlags)
        {
            VstTimeInfo vstTimeInfo = new VstTimeInfo();

            vstTimeInfo.SamplePosition = 0.0;
            vstTimeInfo.SampleRate = 44100;
            vstTimeInfo.NanoSeconds = 0.0;
            vstTimeInfo.PpqPosition = 0.0;
            vstTimeInfo.Tempo = 120.0;
            vstTimeInfo.BarStartPosition = 0.0;
            vstTimeInfo.CycleStartPosition = 0.0;
            vstTimeInfo.CycleEndPosition = 0.0;
            vstTimeInfo.TimeSignatureNumerator = 4;
            vstTimeInfo.TimeSignatureDenominator = 4;
            vstTimeInfo.SmpteOffset = 0;
            vstTimeInfo.SmpteFrameRate = new Jacobi.Vst.Core.VstSmpteFrameRate();
            vstTimeInfo.SamplesToNearestClock = 0;
            vstTimeInfo.Flags = VstTimeInfoFlags.NanoSecondsValid |
                VstTimeInfoFlags.TempoValid |
                VstTimeInfoFlags.PpqPositionValid |
                VstTimeInfoFlags.CyclePositionValid |
                VstTimeInfoFlags.BarStartPositionValid |
                VstTimeInfoFlags.TimeSignatureValid |
                VstTimeInfoFlags.TransportPlaying;

            return vstTimeInfo;
        }

        public string GetVendorString()
        {
            return "Jacobi Software";
        }

        public int GetVendorVersion()
        {
            return 1000;
        }

        public bool IoChanged()
        {
            return false;
        }

        public bool OpenFileSelector(VstFileSelect fileSelect)
        {
            return false;
        }

        public bool ProcessEvents(VstEvent[] events)
        {
            return false;
        }

        public bool SizeWindow(int width, int height)
        {
            return false;
        }

        public bool UpdateDisplay()
        {
            return true;
        }

        #endregion

        #region IVstHostCommands10 Members

        public int GetCurrentPluginID()
        {
            return PluginContext.PluginInfo.PluginID;
        }

        public int GetVersion()
        {
            return 2400;
        }

        public void ProcessIdle()
        {
            bool debuggerBreak = true;
        }

        public void SetParameterAutomated(int index, float value)
        {
            bool debuggerBreak = true;
        }

        #endregion
    }
}

#endif
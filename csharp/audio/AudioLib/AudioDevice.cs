#if USE_SLIMDX
using SlimDX.DirectSound;
#else
using System;
using System.Security;
using Microsoft.DirectX.DirectSound;
#endif

namespace AudioLib
{
    /// <summary>
    /// Small wrapper around DirectX devices to make it a little more friendly.
    /// </summary>
//#if NET40
//    [SecuritySafeCritical]
//#endif
    public abstract class AudioDevice
    {
        private DeviceInformation mDevInfo;
        /// <summary>
        /// An informative string (i.e. name) for this device.
        /// </summary>
        public string Name { get { return mDevInfo.Description; } }

        protected AudioDevice(DeviceInformation devInfo) { mDevInfo = devInfo; }

        public override string ToString() { return Name; }

        public static bool operator ==(AudioDevice d1, AudioDevice d2)
        {
            return object.ReferenceEquals(d1, d2);
        }
        public static bool operator !=(AudioDevice d1, AudioDevice d2)
        {
            return !object.ReferenceEquals(d1, d2);
        }
    }

//#if NET40
//    [SecuritySafeCritical]
//#endif
    public class OutputDevice : AudioDevice
    {
#if USE_SLIMDX
        private DirectSound mDevice;

        public DirectSound Device
        {
            get { return mDevice; }
        }

        public OutputDevice(DeviceInformation devInfo)
            : base(devInfo)
        {
            mDevice = new DirectSound(devInfo.DriverGuid);
        }
#else
        private Device mDevice;

        public Device Device
        {
            get { return mDevice; }
        }

        public OutputDevice(DeviceInformation devInfo)
            : base(devInfo)
        {
            mDevice = new Device(devInfo.DriverGuid);
        }
#endif
    }

//#if NET40
//    [SecuritySafeCritical]
//#endif
    public class InputDevice : AudioDevice
    {
#if USE_SLIMDX
        private DirectSoundCapture mCapture;

        public DirectSoundCapture Capture
        {
            get { return mCapture; }
        }

        public InputDevice(DeviceInformation devInfo)
            : base(devInfo)
        {
            mCapture = new DirectSoundCapture(devInfo.DriverGuid);
        }
#else
        private Capture mCapture;

        public Capture Capture
        {
            get { return mCapture; }
        }

        public InputDevice(DeviceInformation devInfo)
            : base(devInfo)
        {
            mCapture = new Capture(devInfo.DriverGuid);
        }
#endif
    }
}

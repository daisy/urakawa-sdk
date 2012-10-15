#if USE_SHARPDX
using SharpDX.DirectSound;
#else
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
        private
#if USE_SHARPDX
 DirectSound
#else
 Device
#endif
 mDevice;

        public
#if USE_SHARPDX
 DirectSound
#else
 Device
#endif
 Device
        {
            get { return mDevice; }
        }

        public OutputDevice(DeviceInformation devInfo)
            : base(devInfo)
        {
            mDevice = new
#if USE_SHARPDX
 DirectSound
#else
 Device
#endif
(devInfo.DriverGuid);
        }
    }

    //#if NET40
    //    [SecuritySafeCritical]
    //#endif
    public class InputDevice : AudioDevice
    {
        private
#if USE_SHARPDX
 DirectSoundCapture
#else
 Capture
#endif
 mCapture;

        public
#if USE_SHARPDX
 DirectSoundCapture
#else
 Capture
#endif
 Capture
        {
            get { return mCapture; }
        }

        public InputDevice(DeviceInformation devInfo)
            : base(devInfo)
        {
            mCapture = new
#if USE_SHARPDX
 DirectSoundCapture
#else
 Capture
#endif
(devInfo.DriverGuid);
        }
    }
}

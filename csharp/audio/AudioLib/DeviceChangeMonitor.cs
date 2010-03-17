using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace AudioLib
    {
    class DeviceChangeMonitor:Form
        {

        public const int
WM_DeviceChange = 0x0219;

        public const int
        DBT_DeviceArrival = 0x8000,
        DBT_DeviceRemovalComplete = 0x8004;
        

        public delegate void DeviceChangedNotifyDelegate ( Message msg );
        public static event DeviceChangedNotifyDelegate  DeviceChanged;
        private static DeviceChangeMonitor m_Instance;

        public static void StartMonitoringDevices ()
            {
            Thread monitorThread = new Thread ( runForm );
            monitorThread.SetApartmentState ( ApartmentState.STA );
            monitorThread.IsBackground = true;
            monitorThread.Start ();
            }


        public static void StopMonitoringDevices ()
            {
            if (m_Instance == null) throw new InvalidOperationException ( "Device monitoring is not active" );
            DeviceChanged = null;
            m_Instance.Invoke ( new MethodInvoker ( m_Instance.endForm ) );
            }

        private static void runForm ()
            {
            Application.Run ( new DeviceChangeMonitor () );
            }

        private void endForm ()
            {
            this.Close ();
            }

        protected override void SetVisibleCore ( bool value )
            {
                        if (m_Instance == null) CreateHandle ();
            m_Instance = this;
            value = false;
            base.SetVisibleCore ( value );
            }


        protected override void WndProc ( ref Message m)
            {
                        if (m.Msg == 0x219)
                {
                                if (DeviceChanged != null)
                    DeviceChanged ( m );
                }
            base.WndProc ( ref m );
            }

        private void InitializeComponent ()
            {
            this.SuspendLayout ();
            // 
            // DeviceChangeNotifier
            // 
            //this.ClientSize = new System.Drawing.Size ( 22, 22 );
            this.Name = "DeviceChangeNotifier";
            this.Text = "invisible window";
            this.ResumeLayout ( false );

            }


        }
    }

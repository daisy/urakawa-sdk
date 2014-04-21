using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace AudioLib
{
    public interface IDualCancellableProgressReporter
    {
        event ProgressChangedEventHandler ProgressChangedEvent;
        void reportProgress(int percent, string msg);
        event ProgressChangedEventHandler SubProgressChangedEvent;
        void reportSubProgress(int percent, string msg);
        bool RequestCancellation { get; set; }

        void AddSubCancellable(IDualCancellableProgressReporter other);
        void RemoveSubCancellable(IDualCancellableProgressReporter other);

        void RunTask();
        void DoWork();
        void SetDoEventsMethod(Action action);
    }

    public abstract class DualCancellableProgressReporter : IDualCancellableProgressReporter
    {
        private Stopwatch m_stopWatch = null;

        public event ProgressChangedEventHandler ProgressChangedEvent;
        public void reportProgress(int percent, string msg)
        {
            if (m_stopWatch == null || m_stopWatch.ElapsedMilliseconds > 300)
            {
                if (m_stopWatch != null)
                {
                    m_stopWatch.Stop();
                }
                reportSubProgress(-1, null);
                ProgressChangedEventHandler d = ProgressChangedEvent;
                if (d != null)
                {
                    d(this, new ProgressChangedEventArgs(percent, string.IsNullOrEmpty(msg) ? null : msg));
                }
                if (m_stopWatch == null)
                {
                    m_stopWatch = new Stopwatch();
                }
                m_stopWatch.Reset();
                m_stopWatch.Start();
            }
        }

        public event ProgressChangedEventHandler SubProgressChangedEvent;
        public void reportSubProgress(int percent, string msg)
        {
            if (m_DoEventsMethod != null) m_DoEventsMethod();

            ProgressChangedEventHandler d = SubProgressChangedEvent;
            if (d != null)
            {
                d(this, new ProgressChangedEventArgs(percent, msg));
            }
        }


        public event CancellationRequestedEventHandler CancellationRequestedEvent;
        public delegate void CancellationRequestedEventHandler(object sender, CancellationRequestedEventArgs e);
        public class CancellationRequestedEventArgs : EventArgs
        {
            public CancellationRequestedEventArgs()
            {
            }
        }


        private bool m_RequestCancellation;
        public bool RequestCancellation
        {
            get
            {
                if (m_RequestCancellation)
                    return true;

                lock (m_subCancellables)
                {
                    foreach (IDualCancellableProgressReporter cancellable in m_subCancellables)
                    {
                        if (cancellable.RequestCancellation)
                            return true;
                    }
                }

                return false;
            }
            set
            {
                m_RequestCancellation = value;

                lock (m_subCancellables)
                {
                    foreach (IDualCancellableProgressReporter cancellable in m_subCancellables)
                    {
                        cancellable.RequestCancellation = value;
                    }
                }

                if (m_RequestCancellation)
                {
                    CancellationRequestedEventHandler d = CancellationRequestedEvent;
                    if (d != null)
                    {
                        d(this, new CancellationRequestedEventArgs());
                    }
                }
            }
        }

        List<IDualCancellableProgressReporter> m_subCancellables = new List<IDualCancellableProgressReporter>();

        public void AddSubCancellable(IDualCancellableProgressReporter other)
        {
            other.RequestCancellation = RequestCancellation;
            lock (m_subCancellables)
            {
                m_subCancellables.Add(other);
            }
            other.ProgressChangedEvent += OnSubCancellableProgressChanged;
            other.SubProgressChangedEvent += OnSubCancellableSubProgressChanged;
        }

        public void RemoveSubCancellable(IDualCancellableProgressReporter other)
        {
            lock (m_subCancellables)
            {
                m_subCancellables.Remove(other);
            }

            other.ProgressChangedEvent -= OnSubCancellableProgressChanged;
            other.SubProgressChangedEvent -= OnSubCancellableSubProgressChanged;
        }

        private void OnSubCancellableProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            reportProgress(e.ProgressPercentage, (string)e.UserState);
        }

        private void OnSubCancellableSubProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            reportSubProgress(e.ProgressPercentage, (string)e.UserState);
        }

        public void RunTask()
        {
            try
            {
                DoWork();
            }
            catch (Exception ex)
            {
#if DEBUG
                Debugger.Break();
#endif
                throw new Exception("Tobi catch level #1", ex);
            }
            finally
            {
                if (m_stopWatch != null)
                {
                    m_stopWatch.Stop();
                }
            }
        }

        public abstract void DoWork();

        protected Action m_DoEventsMethod;
        public void SetDoEventsMethod(Action action)
        {
            m_DoEventsMethod = action;
        }
    }
}

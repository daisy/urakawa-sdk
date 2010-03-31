using System;
using System.Collections.Generic;
using System.ComponentModel;

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

        void DoWork();
        void SetDoEventsMethod(Action action);
    }

    public abstract class DualCancellableProgressReporter : IDualCancellableProgressReporter
    {
        public event ProgressChangedEventHandler ProgressChangedEvent;
        public void reportProgress(int percent, string msg)
        {
            reportSubProgress(-1, null);
            ProgressChangedEventHandler d = ProgressChangedEvent;
            if (d != null)
            {
                d(this, new ProgressChangedEventArgs(percent, msg));
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
            }
        }

        List<IDualCancellableProgressReporter> m_subCancellables = new List<IDualCancellableProgressReporter>();

        public void AddSubCancellable(IDualCancellableProgressReporter other)
        {
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

        public abstract void DoWork();

        protected Action m_DoEventsMethod;
        public void SetDoEventsMethod(Action action)
        {
            m_DoEventsMethod = action;
        }
    }
}

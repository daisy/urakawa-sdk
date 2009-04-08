using System;
using System.Threading;
using System.Windows.Forms;
using urakawa.progress;

namespace DaisyToXukUI
{
    public partial class Progress : Form
    {
        private bool mCancelPressed = false;
        private string mActionDescription = "";

        public Progress()
        {
            InitializeComponent();
        }
        public static void ExecuteProgressAction(ProgressAction action, out bool wasCancelled)
        {
            wasCancelled = false;
            Progress w = new Progress();
            action.Progress += new System.EventHandler<urakawa.events.progress.ProgressEventArgs>(w.action_progress);
            action.Finished += new System.EventHandler<urakawa.events.progress.FinishedEventArgs>(w.action_finished);
            action.Cancelled += new System.EventHandler<urakawa.events.progress.CancelledEventArgs>(w.action_cancelled);
            w.mActionDescription = action.ShortDescription;
            w.Text = w.mActionDescription;
            Thread executeThread = new Thread(ExecuteWorker);
            executeThread.Start(action);
            DialogResult result = w.ShowDialog();
            if (result == DialogResult.Cancel || result == DialogResult.Abort)
            {
                wasCancelled = true;
            }
            else
            {
                wasCancelled = false;
            }
        }

        private static void ExecuteWorker(object o)
        {
            ProgressAction action = o as ProgressAction;
            if (action != null) action.Execute();
        }



        private void action_cancelled(object sender, urakawa.events.progress.CancelledEventArgs e)
        {
            DoClose();
        }

        private void action_finished(object sender, urakawa.events.progress.FinishedEventArgs e)
        {
            DoClose();
        }
        private delegate void DoClose_Delegate();
        private void DoClose()
        {
            if (mCancelPressed)
            {
                DialogResult = DialogResult.Cancel;
            }
            else
            {
                DialogResult = DialogResult.OK;
            }
            return;

            if (InvokeRequired)
            {
                Invoke(new DoClose_Delegate(DoClose));
            }
            Close();
        }

        private void action_progress(object sender, urakawa.events.progress.ProgressEventArgs e)
        {
            double val = e.Current;
            double max = e.Total;
            if (val != mVal || max != mMax || 0 != mMin)
            {
                mMin = 0;
                mVal = val;
                mMax = max;
                //Debug.Print("Progress: Current={0:0}, Total={1:0}, IsCancelled={2}", val, max, e.IsCancelled);
                //Thread.Sleep(10);
                UpdateUI();
            }
            if (mCancelPressed) e.Cancel();
        }

        private double mVal = 0;
        private double mMin = 0;
        private double mMax = 0;

        private delegate void UpdateUI_Delegate();
        private void UpdateUI()
        {
            if (InvokeRequired)
            {
                Invoke(new UpdateUI_Delegate(UpdateUI));
            }
            if (mMax == mVal)
            {
                if (mProgressBar.Value != mProgressBar.Maximum)
                {
                    mProgressBar.Style = ProgressBarStyle.Marquee;
                }
            }
            else
            {
                mProgressBar.Minimum = (int) mMin;
                mProgressBar.Maximum = (int) mMax;
                mProgressBar.Value = (int) mVal;
            }
            Text = string.Format("\"{2}\" {0:0}/{1:0}", mVal, mMax, mActionDescription);

        }
        private void button1_Click(object sender, EventArgs e)
        {
            mCancelPressed = true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace AudioEngine.PPMeter
{
	/// <summary>
	/// A bar of a <see cref="PPMeter"/>
	/// </summary>
	public partial class PPMBar : Control
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		public PPMBar()
		{
			InitializeComponent();
			SetStyle(ControlStyles.ResizeRedraw, true);
			mFallbackThread = new Thread(new ThreadStart(FallbackWorker));
			ForeColor = Color.Yellow;
			ShownValue = Minimum;
			Value = Minimum;
		}

		/// <summary>
		/// Default destructor - kills the fallback <see cref="Thread"/> is alive
		/// </summary>
		~PPMBar()
		{
			if (mFallbackThread.IsAlive) mFallbackThread.Abort();
		}

		/// <summary>
		/// Repaints the invalidated area of the bar to indicate the currently shown value
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			Rectangle filledRect = new Rectangle(ClientRectangle.X, ClientRectangle.Y, FilledWidth(ShownValue), Height);
			Rectangle unfilledRect = new Rectangle(filledRect.Right, ClientRectangle.Y, Width - filledRect.Width, Height);
			filledRect.Intersect(e.ClipRectangle);
			unfilledRect.Intersect(e.ClipRectangle);
			SolidBrush sb = new SolidBrush(BackColor);
			e.Graphics.FillRectangle(sb, unfilledRect);
			System.Drawing.Drawing2D.LinearGradientBrush lgb 
				= new System.Drawing.Drawing2D.LinearGradientBrush(ClientRectangle, ForeColor, SpectrumEndColor, 0f);
			System.Drawing.Drawing2D.ColorBlend cb = new System.Drawing.Drawing2D.ColorBlend(4);
			cb.Colors[0] = ForeColor;
			cb.Colors[1] = ForeColor;
			cb.Colors[2] = SpectrumEndColor;
			cb.Colors[3] = SpectrumEndColor;
			cb.Positions[0] = 0f;
			cb.Positions[1] = (float)(1f + 12f / Minimum);
			cb.Positions[2] = (float)(1f + 6f / Minimum);
			cb.Positions[3] = 1f;
			lgb.InterpolationColors = cb;
			e.Graphics.FillRectangle(lgb, filledRect);
		}

		private Color mSpectrumEndColor = Color.Red;

		/// <summary>
		/// The spectrum end <see cref="Color"/> of the bar.
		/// <see cref="Control.ForeColor"/> is used as spectrum start <see cref="Color"/>
		/// </summary>
		public Color SpectrumEndColor
		{
			get
			{
				return mSpectrumEndColor;
			}

			set
			{
				mSpectrumEndColor = value;
			}
		}

		private int FilledWidth(double val)
		{
			int res;
			if (val < Minimum)
			{
				res = 0;
			}
			else if (val > 0)
			{
				res = Width;
			}
			else
			{
				res = (int)((Minimum-val) * Width / Minimum);
			}
			return res;
		}

		private double mValue;
		private double mShownValue;

		private double ShownValue
		{
			get
			{
				return mShownValue;
			}

			set
			{
				double newValue;
				if (value > 0)
				{
					newValue = 0;
				}
				else if (value < Minimum)
				{
					newValue = Minimum;
				}
				else
				{
					newValue = value;
				}
				if (newValue!=mShownValue)
				{
					int dx, w;
					if (newValue < mShownValue)
					{
						dx = FilledWidth(newValue);
						w = FilledWidth(mShownValue)-dx;
					}
					else
					{
						dx = FilledWidth(mShownValue);
						w = FilledWidth(newValue)-dx;
					}
					mShownValue = newValue;
					Invalidate(new Rectangle(ClientRectangle.X+dx, ClientRectangle.Y, w, Height));
					InvokeUpdate();
				}
			}
		}


		private void InvokeUpdate()
		{
			if (InvokeRequired)
			{
				BeginInvoke(new MethodInvoker(InvokeUpdate));
			}
			else
			{
				Update();
			}
		}

		private Thread mFallbackThread;

		private void FallbackWorker()
		{
			try
			{
				DateTime latestUpdateTime = DateTime.Now;
				while (true)
				{
					TimeSpan timeSinceLatestUpdate = DateTime.Now.Subtract(latestUpdateTime);
					double maxDiff = Double.PositiveInfinity;
					if (FallbackSecondsPerDb.TotalMilliseconds > 0)
					{
						maxDiff = timeSinceLatestUpdate.TotalMilliseconds / FallbackSecondsPerDb.TotalMilliseconds;
					}
					//System.Diagnostics.Debug.Print(
					//  "{0} {1} {2:0.000} {3:0.0}", latestUpdateTime, timeSinceLatestUpdate, maxDiff, ShownValue);
					latestUpdateTime += timeSinceLatestUpdate;
					if (Value < ShownValue - maxDiff)
					{
						ShownValue -= maxDiff;
					}
					else
					{
						ShownValue = Value;
					}
					mValueMutex.WaitOne();
					try
					{
						if (ShownValue == Value) return;
					}
					finally
					{
						mValueMutex.ReleaseMutex();
					}
					Thread.Sleep(1);
					if (this.Disposing) return;
				}
			}
			catch (ThreadAbortException)
			{
			}
		}

		private Mutex mValueMutex = new Mutex();

		/// <summary>
		/// Gets or sets the Db value the bar should show
		/// </summary>
		public double Value
		{
			get
			{
				return mValue;
			}
			set
			{
				double newValue;
				if (value > 0) 
				{
					newValue = 0;
				}
				else if (value < Minimum)
				{
					newValue = Minimum;
				}
				else
				{
					newValue = value;
				}
				if (newValue != mValue)
				{
					mValue = newValue;
					if (!mFallbackThread.IsAlive)
					{
						mFallbackThread = new Thread(new ThreadStart(FallbackWorker));
						mFallbackThread.Start();
					}
				}
			}
		}

		private double mMinimum = -72;

		/// <summary>
		/// Gets the minimal Db value shown by the bar
		/// </summary>
		public double Minimum
		{
			get
			{
				return mMinimum;
			}
			set
			{
				double newValue = value;
				if (newValue > -1) newValue = -1;
				if (mMinimum!=newValue)
				{
					mMinimum = newValue;
					Value = Value;//Ensure that mValue is between Minimum and 0
					Invalidate();
					ShownValue = Value;//Force Value to be shown without delay
				}
			}
		}


		private TimeSpan mFallbackSecondsPerDb = TimeSpan.FromMilliseconds(75);

		/// <summary>
		/// Gets or sets the fallback time per Db of the bar
		/// </summary>
		public TimeSpan FallbackSecondsPerDb
		{
			get
			{
				return mFallbackSecondsPerDb;
			}

			set
			{
				if (value < TimeSpan.Zero)
				{
					mFallbackSecondsPerDb = TimeSpan.Zero;
				}
				else
				{
					mFallbackSecondsPerDb = value;
				}
			}
		}

		/// <summary>
		/// Forces the bar to fall back to <see cref="Value"/> without the delay specified by <see cref="FallbackSecondsPerDb"/>
		/// </summary>
		public void ForceFullFallback()
		{
			if (mFallbackThread.IsAlive) mFallbackThread.Abort();
			ShownValue = Value;
			Invalidate();
		}
	}
}

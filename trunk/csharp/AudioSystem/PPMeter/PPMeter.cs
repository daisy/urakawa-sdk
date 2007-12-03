using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace AudioEngine.PPMeter
{
	/// <summary>
	/// A PPM meter (Peak Programme) supporting 1 or 2 channels
	/// </summary>
	public partial class PPMeter : UserControl
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		public PPMeter()
		{
			InitializeComponent();
			mBars = new PPMBar[] { mPPMBar1, mPPMBar2 };
		}

		/// <summary>
		/// Updates <see cref="PPMBar"/> sizes and invalidates the meter
		/// </summary>
		/// <param name="e">Standard event arguments</param>
		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			UpdateBarSizes();
			Invalidate();
		}

		/// <summary>
		/// Clears the background
		/// </summary>
		/// <param name="e">The paint event arguments</param>
		protected override void OnPaintBackground(PaintEventArgs e)
		{
			e.Graphics.Clear(BackColor);
			base.OnPaintBackground(e);
		}

		
		/// <summary>
		/// Draws the scale of the meter
		/// </summary>
		/// <param name="e">The paint event arguments</param>
		protected override void OnPaint(PaintEventArgs e)
		{
			double val = -3f;
			Pen p = new Pen(SpectrumEndColor, 1);
			StringFormat labelStringFormat = new StringFormat();
			labelStringFormat.Alignment = StringAlignment.Center;
			while (val > Minimum)
			{
				int dx = (int)(Width * (1 - (val / Minimum)));
				Point p1 = new Point(e.ClipRectangle.X + dx, ClientRectangle.Y+Font.Height);
				Point p2 = new Point(e.ClipRectangle.X + dx, ClientRectangle.Bottom);
				e.Graphics.DrawLine(p,p1, p2);
				if (val % 6f == 0)
				{
					Point p3 = new Point(p1.X, 0);
					e.Graphics.DrawString(val.ToString(), Font, new SolidBrush(SpectrumEndColor), p3, labelStringFormat);
				}
				val -= 3f;
			}
			base.OnPaint(e);
		}

		private int mNumberOfChannels = 2;

		private PPMBar[] mBars;

		private int mBarPadding = 5;

		/// <summary>
		/// The padding in points between the <see cref="PPMBar"/>s of the meter
		/// </summary>
		public int BarPadding
		{
			get
			{
				return mBarPadding;
			}

			set
			{
				if (value < 0)
				{
					mBarPadding = 0;
				}
				else
				{
					mBarPadding = value;
				}
			}
		}

		/// <summary>
		/// Gets the height of each <see cref="PPMBar"/> in the meter
		/// </summary>
		/// <returns>The bar height in points</returns>
		public int GetBarHeight()
		{
			int barHeight = (Height - Font.Height - BarPadding * (1+NumberOfChannels)) / (NumberOfChannels);
			if (barHeight < 0) barHeight = 0;
			return barHeight;
		}

		private void UpdateBarSizes()
		{
			if (mBars == null) return;
			if (Height < Font.Height + NumberOfChannels * (BarPadding + 1))
			{
				Height = Font.Height + NumberOfChannels * (BarPadding + 1);
			}
			int barHeight = GetBarHeight();
			for (int c = 0; c < mBars.Length; c++)
			{
				mBars[c].Left = 0;
				mBars[c].Width = Width;
				if (c < NumberOfChannels)
				{
					mBars[c].Top = c * (barHeight + BarPadding) + Font.Height + BarPadding;
					mBars[c].Height = barHeight;
				}
				else
				{
					mBars[c].Height = 0;
				}
			}
		}

		/// <summary>
		/// Gets or sets the number of channels shown in the meter - can be 1 or 2
		/// </summary>
		public int NumberOfChannels
		{
			get
			{
				return mNumberOfChannels;
			}

			set
			{
				if (value<1) 
				{
					mNumberOfChannels = 1;
				}
				else if (value>2)
				{
					mNumberOfChannels = 2;
				}
				else
				{
					mNumberOfChannels = value;
				}
				for (int c = 0; c < mBars.Length; c++)
				{
					mBars[c].Visible = c < mNumberOfChannels;
				}
				UpdateBarSizes();
			}
		}

		private TimeSpan mFallbackSecondsPerDb = TimeSpan.FromMilliseconds(75);

		/// <summary>
		/// Gets or sets the fallback time per Db
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
				foreach (PPMBar bar in mBars)
				{
					bar.FallbackSecondsPerDb = mFallbackSecondsPerDb;
				}
			}
		}

		/// <summary>
		/// Gets the Db value of the meter for a given channel
		/// </summary>
		/// <param name="ch"></param>
		/// <returns></returns>
		public double GetValue(int ch)
		{
			if (0 <= ch && ch < mBars.Length)
			{
				return mBars[ch].Value;
			}
			return Double.NegativeInfinity;
		}

		/// <summary>
		/// Sets the Db value of the meter in a given channel
		/// </summary>
		/// <param name="ch">The channel (0 or 1)</param>
		/// <param name="val">The Db value</param>
		public void SetValue(int ch, double val)
		{
			if (0 <= ch && ch < mBars.Length)
			{
				mBars[ch].Value = val;
			}
		}

		private double mMinimum = -35;

		/// <summary>
		/// Gets the minimal Db value shown by the meter
		/// </summary>
		public double Minimum
		{
			get
			{
				return mMinimum;
			}

			set
			{
				double newVal = value;
				if (newVal > -1) newVal = -1;
				mMinimum = newVal;
				foreach (PPMBar bar in mBars)
				{
					bar.Minimum = mMinimum;
				}
			}
		}

		/// <summary>
		/// Forces the meter to fallback without the delay specified by <see cref="FallbackSecondsPerDb"/>
		/// </summary>
		public void ForceFullFallback()
		{
			foreach (PPMBar bar in mBars)
			{
				bar.ForceFullFallback();
			}
		}

		private Color mSpectrumEndColor = Color.Red;

		/// <summary>
		/// Gets or sets the spectrum end <see cref="Color"/> of the meter. 
		/// The <see cref="Control.ForeColor"/> is used as spectrum start <see cref="Color"/>
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
				foreach (PPMBar bar in mBars)
				{
					bar.SpectrumEndColor = mSpectrumEndColor;
				}
			}
		}

		/// <summary>
		/// Updates the bar position and sizes on <see cref="Font"/> change
		/// </summary>
		/// <param name="e"></param>
		protected override void OnFontChanged(EventArgs e)
		{
			base.OnFontChanged(e);
			UpdateBarSizes();
		}
	}
}

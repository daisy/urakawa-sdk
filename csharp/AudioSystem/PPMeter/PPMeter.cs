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

		private Orientation mBarOrientation = Orientation.Horizontal;

		private void UpdateBarOrientation()
		{
			foreach (PPMBar bar in mBars) bar.BarOrientation = BarOrientation;
		}

		/// <summary>
		/// Gets or sets the orientation of the <see cref="PPMBar"/>
		/// </summary>
		public Orientation BarOrientation
		{
			get
			{
				return mBarOrientation;
			}
			set
			{
				if (mBarOrientation != value)
				{
					mBarOrientation = value;
					UpdateBarOrientation();
					UpdateBarSizes();
					Invalidate();
				}
			}
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
			//if (BarOrientation == Orientation.Horizontal)
			//{
			//  labelStringFormat.Alignment = StringAlignment.Center;
			//}
			//else if (BarOrientation == Orientation.Vertical)
			//{
			//  labelStringFormat.Alignment = StringAlignment.Near;
			//}
			int horzSpacing = (int)(3f*Width/Minimum);
			while (val > Minimum)
			{
				if (BarOrientation == Orientation.Horizontal)
				{
					int dx = (int)(Width * (1 - (val / Minimum)));
					Point p1 = new Point(e.ClipRectangle.X + dx, ClientRectangle.Y + Font.Height);
					Point p2 = new Point(e.ClipRectangle.X + dx, ClientRectangle.Bottom);
					e.Graphics.DrawLine(p, p1, p2);
					if (val % 6f == 0)
					{
						Rectangle r = new Rectangle(p1.X - horzSpacing, e.ClipRectangle.Y, 2 * horzSpacing, Font.Height);
						TextRenderer.DrawText(e.Graphics, val.ToString(), Font, r, SpectrumEndColor);
						//Point p3 = new Point(p1.X, 0);
						//e.Graphics.DrawString(val.ToString(), Font, new SolidBrush(SpectrumEndColor), p3, labelStringFormat);
					}
				}
				else if (BarOrientation == Orientation.Vertical)
				{
					int dy = (int)((Height * val) / Minimum);
					Point p1 = new Point(e.ClipRectangle.X + Font.Height, ClientRectangle.Y + dy);
					Point p2 = new Point(ClientRectangle.Right, ClientRectangle.Y + dy);
					e.Graphics.DrawLine(p, p1, p2);
					if (val % 6f == 0)
					{
						Rectangle r = new Rectangle(e.ClipRectangle.X, p1.Y - Font.Height, Font.Height + BarPadding, Font.Height);
						TextRenderer.DrawText(e.Graphics, val.ToString(), Font, r, SpectrumEndColor);
						//Point p3 = new Point(e.ClipRectangle.X, p1.Y-Font.Height);
						//e.Graphics.DrawString(val.ToString(), Font, new SolidBrush(SpectrumEndColor), p3, labelStringFormat);
					}
				}
				else
				{
					throw new ApplicationException(String.Format(
						"Unknown Orientation {0:d}", BarOrientation));
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
		/// (when <see cref="BarOrientation"/> is <see cref="Orienatation.Horizontal"/>)
		/// </summary>
		/// <returns>The bar height in points</returns>
		public int GetBarHeight()
		{
			int barHeight = (Height - Font.Height - BarPadding * (1+NumberOfChannels)) / (NumberOfChannels);
			if (barHeight < 0) barHeight = 0;
			return barHeight;
		}

		/// <summary>
		/// Gets the width of each <see cref="PPMBar"/> in the meter 
		/// (when <see cref="BarOrientation"/> is <see cref="Orienatation.Vertical"/>)
		/// </summary>
		/// <returns>The bar width in points</returns>
		public int GetBarWidth()
		{
			int barWidth = (Width - Font.Height - BarPadding * (1 + NumberOfChannels)) / (NumberOfChannels);
			if (barWidth < 0) barWidth = 0;
			return barWidth;
		}

		private void UpdateBarSizes()
		{
			if (mBars == null) return;
			if (BarOrientation == Orientation.Horizontal)
			{
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
			else if (BarOrientation == Orientation.Vertical)
			{
				if (Width < Font.Height + NumberOfChannels * (BarPadding + 1))
				{
					Width = Font.Height + NumberOfChannels * (BarPadding + 1);
				}
				int barWidth = GetBarWidth();
				for (int c = 0; c < mBars.Length; c++)
				{
					mBars[c].Top = 0;
					mBars[c].Height = Height;
					if (c < NumberOfChannels)
					{
						mBars[c].Left = c * (barWidth + BarPadding) + Font.Height + BarPadding;
						mBars[c].Width = barWidth;
					}
					else
					{
						mBars[c].Width = 0;
						mBars[c].Left = Width;
					}
				}
			}
			else
			{
				throw new ApplicationException(String.Format(
					"Unknown Orientation {0:d}", BarOrientation));
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
				if (mNumberOfChannels != value)
				{
					if (value < 1)
					{
						mNumberOfChannels = 1;
					}
					else if (value > 2)
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
				Invalidate();
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

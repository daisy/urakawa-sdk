using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WaveFormDisplay
{
	public partial class WaveFormGraph : Control
	{
		public WaveFormGraph()
		{
			InitializeComponent();
		}

		protected override void OnPaint(PaintEventArgs pe)
		{
			Brush backBrush = new SolidBrush(this.BackColor);
			Brush foreBrush = new SolidBrush(this.ForeColor);
			Pen forePen = new Pen(foreBrush);
			Rectangle effectiveRectangle = new Rectangle(pe.ClipRectangle.Left, 0, pe.ClipRectangle.Width, this.Height);
			pe.Graphics.FillRectangle(backBrush, effectiveRectangle);
			if (GraphMinData != null && GraphMaxData != null)
			{
				for (int x = effectiveRectangle.Left; x <= effectiveRectangle.Right; x++)
				{
					if (x < GraphMinData.Length && x < GraphMaxData.Length)
					{
						pe.Graphics.DrawLine(forePen, getPoint(x, GraphMaxData[x]), getPoint(x, GraphMinData[x]));
					}
				}
			}
			pe.Graphics.DrawLine(forePen, getPoint(effectiveRectangle.Left, 0), getPoint(effectiveRectangle.Right, 0));
			pe.Graphics.DrawString(
				String.Format("Ch{0:0}", Channel),
				this.Font, foreBrush, new PointF(0, 0));


			// Calling the base class OnPaint
			base.OnPaint(pe);
		}

		private Point getPoint(int x, int value)
		{
			int y = (int)Math.Round(0.5 * this.Height - (1.0 - 1.0 / Int32.MaxValue));
			return new Point(x, y);
		}


		private int[] mGraphMinData;
		public int[] GraphMinData
		{
			get
			{
				return mGraphMinData;
			}
			set
			{
				mGraphMinData = value;
			}
		}

		private int[] mGraphMaxData;
		public int[] GraphMaxData
		{
			get
			{
				return mGraphMaxData;
			}
			set
			{
				mGraphMaxData = value;
			}
		}

		private ushort mChannel = 0;

		public ushort Channel
		{
			get { return mChannel; }
			set { mChannel = value; }
		}
		
	}
}

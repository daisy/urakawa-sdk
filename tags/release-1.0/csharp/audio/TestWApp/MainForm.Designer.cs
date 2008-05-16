namespace TestWApp
{
	partial class MainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.mWaveFormDisplay = new WaveFormDisplay.WaveFormDisplay();
			this.SuspendLayout();
			// 
			// mWaveFormDisplay
			// 
			this.mWaveFormDisplay.BitDepth = ((ushort)(0));
			this.mWaveFormDisplay.ClipBegin = System.TimeSpan.Parse("00:00:00");
			this.mWaveFormDisplay.ClipEnd = System.TimeSpan.Parse("00:00:00");
			this.mWaveFormDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mWaveFormDisplay.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.mWaveFormDisplay.Location = new System.Drawing.Point(0, 0);
			this.mWaveFormDisplay.Name = "mWaveFormDisplay";
			this.mWaveFormDisplay.NumberOfChannels = ((ushort)(2));
			this.mWaveFormDisplay.PCMDataStream = null;
			this.mWaveFormDisplay.SampleRate = ((uint)(0u));
			this.mWaveFormDisplay.Size = new System.Drawing.Size(709, 370);
			this.mWaveFormDisplay.TabIndex = 0;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(709, 370);
			this.Controls.Add(this.mWaveFormDisplay);
			this.Name = "MainForm";
			this.Text = "Form1";
			this.ResumeLayout(false);

		}

		#endregion

		private WaveFormDisplay.WaveFormDisplay mWaveFormDisplay;
	}
}


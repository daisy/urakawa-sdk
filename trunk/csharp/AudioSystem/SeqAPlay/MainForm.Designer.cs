namespace SeqAPlay
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
			this.mInputFilesLabel = new System.Windows.Forms.Label();
			this.mPlayButton = new System.Windows.Forms.Button();
			this.mPauseButton = new System.Windows.Forms.Button();
			this.mStopButton = new System.Windows.Forms.Button();
			this.mInputFilesListView = new System.Windows.Forms.ListView();
			this.mFileNameColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.mDurationColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.mFullPathColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.mAddInputFileButton = new System.Windows.Forms.Button();
			this.mRemoveInputFileButton = new System.Windows.Forms.Button();
			this.mClearInputFilesButton = new System.Windows.Forms.Button();
			this.mTimeLabel = new System.Windows.Forms.Label();
			this.mPlaybackSpeedNumericUpDown = new System.Windows.Forms.NumericUpDown();
			this.mPPMTextBox = new System.Windows.Forms.TextBox();
			this.mPPMeter = new AudioEngine.PPMeter.PPMeter();
			((System.ComponentModel.ISupportInitialize)(this.mPlaybackSpeedNumericUpDown)).BeginInit();
			this.SuspendLayout();
			// 
			// mInputFilesLabel
			// 
			this.mInputFilesLabel.AutoSize = true;
			this.mInputFilesLabel.Location = new System.Drawing.Point(12, 13);
			this.mInputFilesLabel.Name = "mInputFilesLabel";
			this.mInputFilesLabel.Size = new System.Drawing.Size(55, 13);
			this.mInputFilesLabel.TabIndex = 0;
			this.mInputFilesLabel.Text = "Input Files";
			// 
			// mPlayButton
			// 
			this.mPlayButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.mPlayButton.Location = new System.Drawing.Point(13, 297);
			this.mPlayButton.Name = "mPlayButton";
			this.mPlayButton.Size = new System.Drawing.Size(91, 23);
			this.mPlayButton.TabIndex = 2;
			this.mPlayButton.Text = "Play";
			this.mPlayButton.UseVisualStyleBackColor = true;
			this.mPlayButton.Click += new System.EventHandler(this.mPlayButton_Click);
			// 
			// mPauseButton
			// 
			this.mPauseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.mPauseButton.Location = new System.Drawing.Point(110, 297);
			this.mPauseButton.Name = "mPauseButton";
			this.mPauseButton.Size = new System.Drawing.Size(91, 23);
			this.mPauseButton.TabIndex = 3;
			this.mPauseButton.Text = "Pause";
			this.mPauseButton.UseVisualStyleBackColor = true;
			this.mPauseButton.Click += new System.EventHandler(this.mPauseButton_Click);
			// 
			// mStopButton
			// 
			this.mStopButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.mStopButton.Location = new System.Drawing.Point(506, 297);
			this.mStopButton.Name = "mStopButton";
			this.mStopButton.Size = new System.Drawing.Size(91, 23);
			this.mStopButton.TabIndex = 4;
			this.mStopButton.Text = "Stop";
			this.mStopButton.UseVisualStyleBackColor = true;
			this.mStopButton.Click += new System.EventHandler(this.mStopButton_Click);
			// 
			// mInputFilesListView
			// 
			this.mInputFilesListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
									| System.Windows.Forms.AnchorStyles.Left)
									| System.Windows.Forms.AnchorStyles.Right)));
			this.mInputFilesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.mFileNameColumnHeader,
            this.mDurationColumnHeader,
            this.mFullPathColumnHeader});
			this.mInputFilesListView.Location = new System.Drawing.Point(12, 93);
			this.mInputFilesListView.Name = "mInputFilesListView";
			this.mInputFilesListView.Size = new System.Drawing.Size(487, 198);
			this.mInputFilesListView.TabIndex = 5;
			this.mInputFilesListView.UseCompatibleStateImageBehavior = false;
			this.mInputFilesListView.View = System.Windows.Forms.View.Details;
			this.mInputFilesListView.SelectedIndexChanged += new System.EventHandler(this.mInputFilesListView_SelectedIndexChanged);
			// 
			// mFileNameColumnHeader
			// 
			this.mFileNameColumnHeader.Text = "File Name";
			this.mFileNameColumnHeader.Width = 100;
			// 
			// mDurationColumnHeader
			// 
			this.mDurationColumnHeader.Text = "Duration";
			this.mDurationColumnHeader.Width = 100;
			// 
			// mFullPathColumnHeader
			// 
			this.mFullPathColumnHeader.Text = "Full Path";
			this.mFullPathColumnHeader.Width = 250;
			// 
			// mAddInputFileButton
			// 
			this.mAddInputFileButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.mAddInputFileButton.Location = new System.Drawing.Point(505, 35);
			this.mAddInputFileButton.Name = "mAddInputFileButton";
			this.mAddInputFileButton.Size = new System.Drawing.Size(91, 23);
			this.mAddInputFileButton.TabIndex = 6;
			this.mAddInputFileButton.Text = "Add";
			this.mAddInputFileButton.UseVisualStyleBackColor = true;
			this.mAddInputFileButton.Click += new System.EventHandler(this.mAddInputFileButton_Click);
			// 
			// mRemoveInputFileButton
			// 
			this.mRemoveInputFileButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.mRemoveInputFileButton.Enabled = false;
			this.mRemoveInputFileButton.Location = new System.Drawing.Point(505, 64);
			this.mRemoveInputFileButton.Name = "mRemoveInputFileButton";
			this.mRemoveInputFileButton.Size = new System.Drawing.Size(91, 23);
			this.mRemoveInputFileButton.TabIndex = 7;
			this.mRemoveInputFileButton.Text = "Remove";
			this.mRemoveInputFileButton.UseVisualStyleBackColor = true;
			this.mRemoveInputFileButton.Click += new System.EventHandler(this.mRemoveInputFileButton_Click);
			// 
			// mClearInputFilesButton
			// 
			this.mClearInputFilesButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.mClearInputFilesButton.Enabled = false;
			this.mClearInputFilesButton.Location = new System.Drawing.Point(506, 93);
			this.mClearInputFilesButton.Name = "mClearInputFilesButton";
			this.mClearInputFilesButton.Size = new System.Drawing.Size(91, 23);
			this.mClearInputFilesButton.TabIndex = 8;
			this.mClearInputFilesButton.Text = "Clear";
			this.mClearInputFilesButton.UseVisualStyleBackColor = true;
			this.mClearInputFilesButton.Click += new System.EventHandler(this.mClearInputFilesButton_Click);
			// 
			// mTimeLabel
			// 
			this.mTimeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.mTimeLabel.AutoSize = true;
			this.mTimeLabel.Location = new System.Drawing.Point(207, 302);
			this.mTimeLabel.Name = "mTimeLabel";
			this.mTimeLabel.Size = new System.Drawing.Size(70, 13);
			this.mTimeLabel.TabIndex = 9;
			this.mTimeLabel.Text = "00:00:00.000";
			// 
			// mPlaybackSpeedNumericUpDown
			// 
			this.mPlaybackSpeedNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.mPlaybackSpeedNumericUpDown.DecimalPlaces = 2;
			this.mPlaybackSpeedNumericUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
			this.mPlaybackSpeedNumericUpDown.Location = new System.Drawing.Point(295, 298);
			this.mPlaybackSpeedNumericUpDown.Name = "mPlaybackSpeedNumericUpDown";
			this.mPlaybackSpeedNumericUpDown.Size = new System.Drawing.Size(120, 20);
			this.mPlaybackSpeedNumericUpDown.TabIndex = 10;
			this.mPlaybackSpeedNumericUpDown.ValueChanged += new System.EventHandler(this.mPlaybackSpeedNumericUpDown_ValueChanged);
			// 
			// mPPMTextBox
			// 
			this.mPPMTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.mPPMTextBox.Location = new System.Drawing.Point(506, 10);
			this.mPPMTextBox.Name = "mPPMTextBox";
			this.mPPMTextBox.ReadOnly = true;
			this.mPPMTextBox.Size = new System.Drawing.Size(90, 20);
			this.mPPMTextBox.TabIndex = 12;
			this.mPPMTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// mPPMeter
			// 
			this.mPPMeter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
									| System.Windows.Forms.AnchorStyles.Right)));
			this.mPPMeter.BackColor = System.Drawing.SystemColors.ControlText;
			this.mPPMeter.BarPadding = 5;
			this.mPPMeter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.mPPMeter.FallbackSecondsPerDb = System.TimeSpan.Parse("00:00:00.0750000");
			this.mPPMeter.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.mPPMeter.Location = new System.Drawing.Point(110, 10);
			this.mPPMeter.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
			this.mPPMeter.Minimum = -70;
			this.mPPMeter.Name = "mPPMeter";
			this.mPPMeter.NumberOfChannels = 1;
			this.mPPMeter.Size = new System.Drawing.Size(389, 77);
			this.mPPMeter.SpectrumEndColor = System.Drawing.Color.Red;
			this.mPPMeter.TabIndex = 14;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(609, 332);
			this.Controls.Add(this.mPPMeter);
			this.Controls.Add(this.mPPMTextBox);
			this.Controls.Add(this.mPlaybackSpeedNumericUpDown);
			this.Controls.Add(this.mTimeLabel);
			this.Controls.Add(this.mClearInputFilesButton);
			this.Controls.Add(this.mRemoveInputFileButton);
			this.Controls.Add(this.mAddInputFileButton);
			this.Controls.Add(this.mInputFilesListView);
			this.Controls.Add(this.mStopButton);
			this.Controls.Add(this.mPauseButton);
			this.Controls.Add(this.mPlayButton);
			this.Controls.Add(this.mInputFilesLabel);
			this.MinimumSize = new System.Drawing.Size(533, 285);
			this.Name = "MainForm";
			this.Text = "Sequence Audio Player";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
			((System.ComponentModel.ISupportInitialize)(this.mPlaybackSpeedNumericUpDown)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label mInputFilesLabel;
		private System.Windows.Forms.Button mPlayButton;
		private System.Windows.Forms.Button mPauseButton;
		private System.Windows.Forms.Button mStopButton;
		private System.Windows.Forms.ListView mInputFilesListView;
		private System.Windows.Forms.Button mAddInputFileButton;
		private System.Windows.Forms.Button mRemoveInputFileButton;
		private System.Windows.Forms.Button mClearInputFilesButton;
		private System.Windows.Forms.Label mTimeLabel;
		private System.Windows.Forms.NumericUpDown mPlaybackSpeedNumericUpDown;
		private System.Windows.Forms.ColumnHeader mFileNameColumnHeader;
		private System.Windows.Forms.ColumnHeader mDurationColumnHeader;
		private System.Windows.Forms.ColumnHeader mFullPathColumnHeader;
		private System.Windows.Forms.TextBox mPPMTextBox;
		private AudioEngine.PPMeter.PPMeter mPPMeter;
	}
}


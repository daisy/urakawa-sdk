namespace PunchInRec
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
			this.components = new System.ComponentModel.Container();
			this.mFileLabel = new System.Windows.Forms.Label();
			this.mFileTextBox = new System.Windows.Forms.TextBox();
			this.mOpenFileButton = new System.Windows.Forms.Button();
			this.mPlayButton = new System.Windows.Forms.Button();
			this.mRecordButton = new System.Windows.Forms.Button();
			this.mTogglePauseButton = new System.Windows.Forms.Button();
			this.mStopButton = new System.Windows.Forms.Button();
			this.mTimeLabel = new System.Windows.Forms.Label();
			this.mTimeTrackBar = new System.Windows.Forms.TrackBar();
			this.mExitButton = new System.Windows.Forms.Button();
			this.mMainMenu = new System.Windows.Forms.MainMenu(this.components);
			this.mFileMenuItem = new System.Windows.Forms.MenuItem();
			this.mOpenFileMenuItem = new System.Windows.Forms.MenuItem();
			this.mExitFileMenuItem = new System.Windows.Forms.MenuItem();
			this.mTransportMenuItem = new System.Windows.Forms.MenuItem();
			this.mPlayTransportMenuItem = new System.Windows.Forms.MenuItem();
			this.mRecordTransportMenuItem = new System.Windows.Forms.MenuItem();
			this.mTogglePauseTransportMenuItem = new System.Windows.Forms.MenuItem();
			this.mStopTransportMenuItem = new System.Windows.Forms.MenuItem();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.mGotoTimeSliderTransportMenuItem = new System.Windows.Forms.MenuItem();
			this.mHelpMenuItem = new System.Windows.Forms.MenuItem();
			this.mAboutHelpMenuItem = new System.Windows.Forms.MenuItem();
			((System.ComponentModel.ISupportInitialize)(this.mTimeTrackBar)).BeginInit();
			this.SuspendLayout();
			// 
			// mFileLabel
			// 
			this.mFileLabel.AutoSize = true;
			this.mFileLabel.Location = new System.Drawing.Point(13, 13);
			this.mFileLabel.Name = "mFileLabel";
			this.mFileLabel.Size = new System.Drawing.Size(52, 13);
			this.mFileLabel.TabIndex = 0;
			this.mFileLabel.Text = "Wave file";
			// 
			// mFileTextBox
			// 
			this.mFileTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
									| System.Windows.Forms.AnchorStyles.Right)));
			this.mFileTextBox.Location = new System.Drawing.Point(98, 10);
			this.mFileTextBox.Name = "mFileTextBox";
			this.mFileTextBox.ReadOnly = true;
			this.mFileTextBox.Size = new System.Drawing.Size(320, 20);
			this.mFileTextBox.TabIndex = 1;
			// 
			// mOpenFileButton
			// 
			this.mOpenFileButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.mOpenFileButton.Location = new System.Drawing.Point(424, 10);
			this.mOpenFileButton.Name = "mOpenFileButton";
			this.mOpenFileButton.Size = new System.Drawing.Size(75, 23);
			this.mOpenFileButton.TabIndex = 2;
			this.mOpenFileButton.Text = "Open";
			this.mOpenFileButton.UseVisualStyleBackColor = true;
			this.mOpenFileButton.Click += new System.EventHandler(this.mOpenFileButton_Click);
			// 
			// mPlayButton
			// 
			this.mPlayButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.mPlayButton.Location = new System.Drawing.Point(19, 102);
			this.mPlayButton.Name = "mPlayButton";
			this.mPlayButton.Size = new System.Drawing.Size(75, 23);
			this.mPlayButton.TabIndex = 5;
			this.mPlayButton.Text = "Play";
			this.mPlayButton.UseVisualStyleBackColor = true;
			this.mPlayButton.Click += new System.EventHandler(this.mPlayButton_Click);
			// 
			// mRecordButton
			// 
			this.mRecordButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.mRecordButton.Location = new System.Drawing.Point(100, 102);
			this.mRecordButton.Name = "mRecordButton";
			this.mRecordButton.Size = new System.Drawing.Size(75, 23);
			this.mRecordButton.TabIndex = 6;
			this.mRecordButton.Text = "Record";
			this.mRecordButton.UseVisualStyleBackColor = true;
			this.mRecordButton.Click += new System.EventHandler(this.mRecordButton_Click);
			// 
			// mTogglePauseButton
			// 
			this.mTogglePauseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.mTogglePauseButton.Location = new System.Drawing.Point(181, 102);
			this.mTogglePauseButton.Name = "mTogglePauseButton";
			this.mTogglePauseButton.Size = new System.Drawing.Size(75, 23);
			this.mTogglePauseButton.TabIndex = 7;
			this.mTogglePauseButton.Text = "Pause";
			this.mTogglePauseButton.UseVisualStyleBackColor = true;
			this.mTogglePauseButton.Click += new System.EventHandler(this.mPauseButton_Click);
			// 
			// mStopButton
			// 
			this.mStopButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.mStopButton.Location = new System.Drawing.Point(262, 102);
			this.mStopButton.Name = "mStopButton";
			this.mStopButton.Size = new System.Drawing.Size(75, 23);
			this.mStopButton.TabIndex = 8;
			this.mStopButton.Text = "Stop";
			this.mStopButton.UseVisualStyleBackColor = true;
			this.mStopButton.Click += new System.EventHandler(this.mStopButton_Click);
			// 
			// mTimeLabel
			// 
			this.mTimeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.mTimeLabel.AutoSize = true;
			this.mTimeLabel.Location = new System.Drawing.Point(16, 83);
			this.mTimeLabel.Name = "mTimeLabel";
			this.mTimeLabel.Size = new System.Drawing.Size(138, 13);
			this.mTimeLabel.TabIndex = 4;
			this.mTimeLabel.Text = "00:00:00.000/00:00:00.000";
			// 
			// mTimeTrackBar
			// 
			this.mTimeTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
									| System.Windows.Forms.AnchorStyles.Right)));
			this.mTimeTrackBar.LargeChange = 60000;
			this.mTimeTrackBar.Location = new System.Drawing.Point(16, 37);
			this.mTimeTrackBar.Name = "mTimeTrackBar";
			this.mTimeTrackBar.Size = new System.Drawing.Size(483, 42);
			this.mTimeTrackBar.SmallChange = 1000;
			this.mTimeTrackBar.TabIndex = 3;
			this.mTimeTrackBar.TickFrequency = 1000;
			this.mTimeTrackBar.Scroll += new System.EventHandler(this.mTimeTrackBar_Scroll);
			this.mTimeTrackBar.KeyDown += new System.Windows.Forms.KeyEventHandler(this.mTimeTrackBar_KeyDown);
			// 
			// mExitButton
			// 
			this.mExitButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.mExitButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.mExitButton.Location = new System.Drawing.Point(424, 102);
			this.mExitButton.Name = "mExitButton";
			this.mExitButton.Size = new System.Drawing.Size(75, 23);
			this.mExitButton.TabIndex = 9;
			this.mExitButton.Text = "Exit";
			this.mExitButton.UseVisualStyleBackColor = true;
			this.mExitButton.Click += new System.EventHandler(this.mCloseButton_Click);
			// 
			// mMainMenu
			// 
			this.mMainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mFileMenuItem,
            this.mTransportMenuItem,
            this.mHelpMenuItem});
			// 
			// mFileMenuItem
			// 
			this.mFileMenuItem.Index = 0;
			this.mFileMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mOpenFileMenuItem,
            this.mExitFileMenuItem});
			this.mFileMenuItem.Text = "&File";
			// 
			// mOpenFileMenuItem
			// 
			this.mOpenFileMenuItem.Index = 0;
			this.mOpenFileMenuItem.Shortcut = System.Windows.Forms.Shortcut.CtrlO;
			this.mOpenFileMenuItem.Text = "Open";
			this.mOpenFileMenuItem.Click += new System.EventHandler(this.mOpenFileMenuItem_Click);
			// 
			// mExitFileMenuItem
			// 
			this.mExitFileMenuItem.Index = 1;
			this.mExitFileMenuItem.Text = "Exit";
			this.mExitFileMenuItem.Click += new System.EventHandler(this.mExitFileMenuItem_Click);
			// 
			// mTransportMenuItem
			// 
			this.mTransportMenuItem.Index = 1;
			this.mTransportMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mPlayTransportMenuItem,
            this.mRecordTransportMenuItem,
            this.mTogglePauseTransportMenuItem,
            this.mStopTransportMenuItem,
            this.menuItem1,
            this.mGotoTimeSliderTransportMenuItem});
			this.mTransportMenuItem.Text = "&Transport";
			// 
			// mPlayTransportMenuItem
			// 
			this.mPlayTransportMenuItem.Index = 0;
			this.mPlayTransportMenuItem.Shortcut = System.Windows.Forms.Shortcut.F9;
			this.mPlayTransportMenuItem.Text = "Play";
			this.mPlayTransportMenuItem.Click += new System.EventHandler(this.mPlayTransportMenuItem_Click);
			// 
			// mRecordTransportMenuItem
			// 
			this.mRecordTransportMenuItem.Index = 1;
			this.mRecordTransportMenuItem.Shortcut = System.Windows.Forms.Shortcut.F10;
			this.mRecordTransportMenuItem.Text = "Record";
			this.mRecordTransportMenuItem.Click += new System.EventHandler(this.mRecordTransportMenuItem_Click);
			// 
			// mTogglePauseTransportMenuItem
			// 
			this.mTogglePauseTransportMenuItem.Index = 2;
			this.mTogglePauseTransportMenuItem.Shortcut = System.Windows.Forms.Shortcut.F11;
			this.mTogglePauseTransportMenuItem.Text = "Pause";
			this.mTogglePauseTransportMenuItem.Click += new System.EventHandler(this.mTogglePauseTransportMenuItem_Click);
			// 
			// mStopTransportMenuItem
			// 
			this.mStopTransportMenuItem.Index = 3;
			this.mStopTransportMenuItem.Shortcut = System.Windows.Forms.Shortcut.F12;
			this.mStopTransportMenuItem.Text = "Stop";
			this.mStopTransportMenuItem.Click += new System.EventHandler(this.mStopTransportMenuItem_Click);
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 4;
			this.menuItem1.Text = "-";
			// 
			// mGotoTimeSliderTransportMenuItem
			// 
			this.mGotoTimeSliderTransportMenuItem.Index = 5;
			this.mGotoTimeSliderTransportMenuItem.Shortcut = System.Windows.Forms.Shortcut.F8;
			this.mGotoTimeSliderTransportMenuItem.Text = "Goto Time Slider";
			this.mGotoTimeSliderTransportMenuItem.Click += new System.EventHandler(this.mGotoTimeSliderTransportMenuItem_Click);
			// 
			// mHelpMenuItem
			// 
			this.mHelpMenuItem.Index = 2;
			this.mHelpMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mAboutHelpMenuItem});
			this.mHelpMenuItem.Text = "&Help";
			// 
			// mAboutHelpMenuItem
			// 
			this.mAboutHelpMenuItem.Index = 0;
			this.mAboutHelpMenuItem.Text = "&About";
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.mExitButton;
			this.ClientSize = new System.Drawing.Size(511, 137);
			this.Controls.Add(this.mExitButton);
			this.Controls.Add(this.mTimeTrackBar);
			this.Controls.Add(this.mTimeLabel);
			this.Controls.Add(this.mStopButton);
			this.Controls.Add(this.mTogglePauseButton);
			this.Controls.Add(this.mRecordButton);
			this.Controls.Add(this.mPlayButton);
			this.Controls.Add(this.mOpenFileButton);
			this.Controls.Add(this.mFileTextBox);
			this.Controls.Add(this.mFileLabel);
			this.Menu = this.mMainMenu;
			this.MinimumSize = new System.Drawing.Size(519, 164);
			this.Name = "MainForm";
			this.Text = "Punch-in Recorder";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
			((System.ComponentModel.ISupportInitialize)(this.mTimeTrackBar)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label mFileLabel;
		private System.Windows.Forms.TextBox mFileTextBox;
		private System.Windows.Forms.Button mOpenFileButton;
		private System.Windows.Forms.Button mPlayButton;
		private System.Windows.Forms.Button mRecordButton;
		private System.Windows.Forms.Button mTogglePauseButton;
		private System.Windows.Forms.Button mStopButton;
		private System.Windows.Forms.Label mTimeLabel;
		private System.Windows.Forms.TrackBar mTimeTrackBar;
		private System.Windows.Forms.Button mExitButton;
		private System.Windows.Forms.MainMenu mMainMenu;
		private System.Windows.Forms.MenuItem mFileMenuItem;
		private System.Windows.Forms.MenuItem mOpenFileMenuItem;
		private System.Windows.Forms.MenuItem mExitFileMenuItem;
		private System.Windows.Forms.MenuItem mTransportMenuItem;
		private System.Windows.Forms.MenuItem mPlayTransportMenuItem;
		private System.Windows.Forms.MenuItem mRecordTransportMenuItem;
		private System.Windows.Forms.MenuItem mTogglePauseTransportMenuItem;
		private System.Windows.Forms.MenuItem mStopTransportMenuItem;
		private System.Windows.Forms.MenuItem mHelpMenuItem;
		private System.Windows.Forms.MenuItem mAboutHelpMenuItem;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem mGotoTimeSliderTransportMenuItem;
	}
}


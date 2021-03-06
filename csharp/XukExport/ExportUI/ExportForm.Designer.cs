namespace ExportUI
    {
    partial class ExportForm
        {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose ( bool disposing )
            {
            if (disposing && (components != null))
                {
                components.Dispose ();
                }
            base.Dispose ( disposing );
            }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent ()
            {
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog ();
            this.m_btnOpen = new System.Windows.Forms.Button ();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog ();
            this.m_btnBrowseOutput = new System.Windows.Forms.Button ();
            this.SuspendLayout ();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // m_btnOpen
            // 
            this.m_btnOpen.Location = new System.Drawing.Point ( 0, 0 );
            this.m_btnOpen.Name = "m_btnOpen";
            this.m_btnOpen.Size = new System.Drawing.Size ( 75, 23 );
            this.m_btnOpen.TabIndex = 0;
            this.m_btnOpen.Text = "&Open";
            this.m_btnOpen.UseVisualStyleBackColor = true;
            this.m_btnOpen.Click += new System.EventHandler ( this.m_btnOpen_Click );
            // 
            // m_btnBrowseOutput
            // 
            this.m_btnBrowseOutput.Location = new System.Drawing.Point ( 0, 50 );
            this.m_btnBrowseOutput.Name = "m_btnBrowseOutput";
            this.m_btnBrowseOutput.Size = new System.Drawing.Size ( 75, 23 );
            this.m_btnBrowseOutput.TabIndex = 1;
            this.m_btnBrowseOutput.Text = "Browse Output";
            this.m_btnBrowseOutput.UseVisualStyleBackColor = true;
            this.m_btnBrowseOutput.Click += new System.EventHandler ( this.m_btnBrowseOutput_Click );
            // 
            // ExportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF ( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size ( 292, 273 );
            this.Controls.Add ( this.m_btnBrowseOutput );
            this.Controls.Add ( this.m_btnOpen );
            this.Name = "ExportForm";
            this.Text = "Form1";
            this.ResumeLayout ( false );

            }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button m_btnOpen;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button m_btnBrowseOutput;
        }
    }


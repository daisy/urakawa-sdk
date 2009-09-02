using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

using urakawa;
using urakawa.core;

namespace ExportUI
    {
    public partial class ExportForm : Form
        {
        private urakawa.Project mProject = null;
        private urakawa.Presentation m_Presentation;
        private DaisyExport.DAISY3Export m_DaisyExport = null;

        public ExportForm ()
            {
            InitializeComponent ();
            }

        public void Open ( string path )
            {
            mProject = new urakawa.Project ();

            //mProject.dataIsMissing += new EventHandler<urakawa.events.media.data.DataIsMissingEventArgs> ( OnDataIsMissing );
            Uri newUri = new Uri ( path );
            try
                {
                mProject.OpenXuk ( newUri );
                }
            catch (System.Exception ex)
                {
                MessageBox.Show ( ex.ToString () );
                }

            m_Presentation = mProject.Presentations.Get ( 0 );
            //MessageBox.Show ( m_Presentation.RootNode.Children.Count.ToString ()) ;
            m_DaisyExport = new DaisyExport.DAISY3Export ( m_Presentation );

            }


        private void OnDataIsMissing ( object sender, EventArgs e )
            {
            MessageBox.Show ( "data missing" );
            }

        private void m_btnOpen_Click ( object sender, EventArgs e )
            {
            if (openFileDialog1.ShowDialog () == DialogResult.OK)
                {
                Open ( openFileDialog1.FileName );
                }
            }

        private void m_btnBrowseOutput_Click ( object sender, EventArgs e )
            {
            if (folderBrowserDialog1.ShowDialog () == DialogResult.OK)
                {
                m_DaisyExport.ExportToDaisy3 ( folderBrowserDialog1.SelectedPath );
                }

            }

        }
    }
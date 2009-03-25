using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DTbookToXuk;

namespace DTbookToXukUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnLoad_Click_1(object sender, EventArgs e)
        {
            txtBookName.Clear();
            var open = new OpenFileDialog();
            //open.InitialDirectory = @"C:\";
            open.Filter = "XML Files (*.xml)|*.xml|All files(*.*)|*.*";
            open.FilterIndex = 1;
            open.RestoreDirectory = true;
            if (open.ShowDialog(this) == DialogResult.OK)
            {
                //m_DTBook_FilePath = open.FileName;
                // txtBookName.Text = m_DTBook_FilePath;
                //uriDTBook = new Uri(m_DTBook_FilePath);
                txtBookName.Text = open.FileName;
                var uriDTBook = new Uri(txtBookName.Text);
                new DTBooktoXukConversion(uriDTBook);
            }

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtBookName.Clear();
        }
       
        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}

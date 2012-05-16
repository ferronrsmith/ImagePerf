using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ImagePerf
{
    public partial class ImgShrink : Form
    {
        public ImgShrink()
        {
            InitializeComponent();
        }

        private void BtnProcessClick(object sender, EventArgs e)
        {
           lblError.Text = string.Empty;
           lblError.Text = Utils.ProcessImages(txtPath.Text, txtDest.Text);
        }

        private void BtnGenerateReportClick(object sender, EventArgs e)
        {
            lblError.Text = string.Empty;
            lblError.Text = Utils.GenerateReport(txtPath.Text,txtDest.Text);
        }

        private void BtnGenReportClick(object sender, EventArgs e)
        {
            lblError.Text = string.Empty;
            lblError.Text = Utils.GenerateByteArrayReport(txtPath.Text);
        }

    }
}

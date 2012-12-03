using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Tangerine.UI
{
    public partial class frmAbout : Form
    {
        public frmAbout()
        {
            InitializeComponent();
        }

        private void lnkXAPSpy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://github.com/sensepost/XAPSpy");
        }
    }
}

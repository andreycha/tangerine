using System.Diagnostics;
using System.Windows.Forms;

namespace Tangerine.UI
{
    public partial class frmAbout : Form
    {
        private static readonly string AppVersion = "0.5";

        public frmAbout()
        {
            InitializeComponent();
            lblVersion.Text = AppVersion;
        }

        private void lnkXAPSpy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://github.com/sensepost/XAPSpy");
        }
    }
}

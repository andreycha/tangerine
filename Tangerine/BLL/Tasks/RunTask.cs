using System;
using System.Windows.Forms;
using Microsoft.SmartDevice.Connectivity;

namespace Tangerine.BLL.Tasks
{
    /// <summary>
    /// This task runs given XAP application on the emulator.
    /// </summary>
    internal class RunTask
    {
        private readonly XAP m_xap;

        internal RunTask(XAP xap)
        {
            m_xap = xap;
        }

        internal void Run()
        {
            Device emulator = new EmulatorRetriever().GetEmulator();
            emulator.Connect();
            Guid appGUID = new Guid(m_xap.ProductId);
            if (emulator.IsApplicationInstalled(appGUID))
            {
                RemoteApplication app = emulator.GetApplication(appGUID);
                app.Launch();
            }
            else
            {
                MessageBox.Show("Application was not found!", "Error");
            }
        }
    }
}

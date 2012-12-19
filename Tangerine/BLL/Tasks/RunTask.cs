using System;
using System.Windows.Forms;
using Microsoft.SmartDevice.Connectivity;
using Tangerine.BLL.Devices;
using Tangerine.Devices;

namespace Tangerine.BLL.Tasks
{
    /// <summary>
    /// This task runs given XAP application on the emulator.
    /// </summary>
    internal class RunTask
    {
        private readonly XAP m_xap;
        private readonly DeviceType m_deviceType;

        internal RunTask(XAP xap, DeviceType deviceType)
        {
            m_xap = xap;
            m_deviceType = deviceType;
        }

        internal void Run()
        {
            Device emulator = new DeviceRetriever().GetDevice(m_deviceType);
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

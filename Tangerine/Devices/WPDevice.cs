using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Tangerine.Devices
{
    /// <summary>
    /// Wrapper around WP device or emulator.
    /// </summary>
    public abstract class WPDevice
    {
        private object m_device;

        protected object Device
        {
            get { return m_device; }
        }

        protected WPDevice(object device)
        {
            if (device == null)
            {
                throw new ArgumentNullException("device");
            }
            if (device.GetType().FullName != "Microsoft.SmartDevice.Connectivity.Device")
            {
                throw new ArgumentException("Wrong type", "device");
            }

            m_device = device;
        }

        public abstract void Connect();

        public abstract void InstallApplication(Guid productId, Guid instanceId, string applicationGenre, string iconPath, string xapPackage);

        public abstract void LaunchApplication(Guid productId);

        public abstract bool IsApplicationInstalled(Guid productId);

        public abstract void UninstallApplication(Guid productId);

        public abstract void Disconnect();
    }
}

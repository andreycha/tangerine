using System;
using System.IO;

namespace Tangerine.Devices
{
    /// <summary>
    /// Wrapper around WP device or emulator.
    /// </summary>
    public class WPDevice
    {
        private object m_device;

        protected object Device
        {
            get { return m_device; }
        }

        public WPDevice(object device)
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

        public virtual void Connect()
        {
            var connectMethod = Device.GetType().GetMethod("Connect");
            connectMethod.Invoke(Device, new object[0]);
        }

        public virtual void InstallApplication(Guid productId, Guid instanceId, string applicationGenre, string iconPath, string xapPackage)
        {
            var installMethod = Device.GetType().GetMethod("InstallApplication");
            installMethod.Invoke(Device, new object[] { productId, instanceId, applicationGenre, iconPath, xapPackage });
        }

        public virtual void LaunchApplication(Guid productId)
        {
            // get app
            var getAppMethod = Device.GetType().GetMethod("GetApplication");
            var app = getAppMethod.Invoke(Device, new object[] { productId });
            // launch it
            var launchMethod = app.GetType().GetMethod("Launch");
            launchMethod.Invoke(app, new object[0]);
        }

        public virtual bool IsApplicationInstalled(Guid productId)
        {
            var isAppInstalledMethod = Device.GetType().GetMethod("IsApplicationInstalled");
            return (bool)isAppInstalledMethod.Invoke(Device, new object[] { productId });
        }

        public virtual void UninstallApplication(Guid productId)
        {
            // get app
            var getAppMethod = Device.GetType().GetMethod("GetApplication");
            var app = getAppMethod.Invoke(Device, new object[] { productId });
            // uninstall it
            var uninstallMethod = app.GetType().GetMethod("Uninstall");
            uninstallMethod.Invoke(app, new object[0]);
        }

        public virtual void Disconnect()
        {
            var connectMethod = Device.GetType().GetMethod("Disconnect");
            connectMethod.Invoke(Device, new object[0]);
        }

        public virtual void ReceiveFile(Guid productId, string sourceFile, string targetPath)
        {
            // get app
            var getAppMethod = Device.GetType().GetMethod("GetApplication");
            var app = getAppMethod.Invoke(Device, new object[] { productId });
            // get isolated store
            var getIsolatedStoreMethod = app.GetType().GetMethod("GetIsolatedStore");
            var isolatedStore = getIsolatedStoreMethod.Invoke(app, new object[0]);
            // receive file
            var receiveFileMethod = isolatedStore.GetType().GetMethod("ReceiveFile");
            sourceFile = Path.DirectorySeparatorChar + sourceFile;
            receiveFileMethod.Invoke(isolatedStore, new object[] { sourceFile, targetPath, true });
        }
    }
}

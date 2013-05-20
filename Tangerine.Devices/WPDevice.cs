using System;
using System.IO;
using System.Reflection;
using Tangerine.Common;

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
            InvokeMethod(connectMethod, Device, new object[0]);
        }

        protected object InvokeMethod(MethodInfo method, object obj, params object[] parameters)
        {
            try
            {
                return method.Invoke(obj, parameters);
            }
            catch (TargetInvocationException e)
            {
                throw ExceptionHelper.GetRealExceptionWithStackTrace(e);
            }
        }

        public virtual void InstallApplication(Guid productId, Guid instanceId, string applicationGenre, string iconPath, string xapPackage)
        {
            var installMethod = Device.GetType().GetMethod("InstallApplication");
            InvokeMethod(installMethod, Device, new object[] { productId, instanceId, applicationGenre, iconPath, xapPackage });
        }

        public virtual void LaunchApplication(Guid productId)
        {
            // get app
            var getAppMethod = Device.GetType().GetMethod("GetApplication");
            var app = InvokeMethod(getAppMethod, Device, new object[] { productId });
            // launch it
            var launchMethod = app.GetType().GetMethod("Launch");
            InvokeMethod(launchMethod, app, new object[0]);
        }

        public virtual bool IsApplicationInstalled(Guid productId)
        {
            var isAppInstalledMethod = Device.GetType().GetMethod("IsApplicationInstalled");
            return (bool)InvokeMethod(isAppInstalledMethod, Device, new object[] { productId });
        }

        public virtual void UninstallApplication(Guid productId)
        {
            // get app
            var getAppMethod = Device.GetType().GetMethod("GetApplication");
            var app = InvokeMethod(getAppMethod, Device, new object[] { productId });
            // uninstall it
            var uninstallMethod = app.GetType().GetMethod("Uninstall");
            InvokeMethod(uninstallMethod, app, new object[0]);
        }

        public virtual void Disconnect()
        {
            var disconnectMethod = Device.GetType().GetMethod("Disconnect");
            InvokeMethod(disconnectMethod, Device, new object[0]);
        }

        public virtual void ReceiveFile(Guid productId, string sourceFile, string targetPath)
        {
            // get app
            var getAppMethod = Device.GetType().GetMethod("GetApplication");
            var app = InvokeMethod(getAppMethod, Device, new object[] { productId });
            // get isolated store
            var getIsolatedStoreMethod = app.GetType().GetMethod("GetIsolatedStore");
            var isolatedStore = InvokeMethod(getIsolatedStoreMethod, app, new object[0]);
            // receive file
            var receiveFileMethod = isolatedStore.GetType().GetMethod("ReceiveFile");
            sourceFile = Path.DirectorySeparatorChar + sourceFile;
            InvokeMethod(receiveFileMethod, isolatedStore, new object[] { sourceFile, targetPath, true });
        }
    }
}

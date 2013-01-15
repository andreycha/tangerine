using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Tangerine.Devices
{
    /// <summary>
    /// Wrapper around WP7 device or emulator.
    /// </summary>
    public class WP7Device : WPDevice
    {
        public WP7Device(object device)
            : base(device)
        {
        }

        public override void Connect()
        {
            var connectMethod = Device.GetType().GetMethod("Connect");
            connectMethod.Invoke(Device, new object[0]);
        }

        public override void InstallApplication(Guid productId, Guid instanceId, string applicationGenre, string iconPath, string xapPackage)
        {
            var installMethod = Device.GetType().GetMethod("InstallApplication");
            installMethod.Invoke(Device, new object[] { productId, instanceId, applicationGenre, iconPath, xapPackage });
        }

        public override void LaunchApplication(Guid productId)
        {
            // get app
            var getAppMethod = Device.GetType().GetMethod("GetApplication");
            var app = getAppMethod.Invoke(Device, new object[] { productId });
            // launch it
            var launchMethod = app.GetType().GetMethod("Launch");
            launchMethod.Invoke(app, new object[0]);
        }

        public override bool IsApplicationInstalled(Guid productId)
        {
            var isAppInstalledMethod = Device.GetType().GetMethod("IsApplicationInstalled");
            return (bool)isAppInstalledMethod.Invoke(Device, new object[] { productId });
        }

        public override void UninstallApplication(Guid productId)
        {
            // get app
            var getAppMethod = Device.GetType().GetMethod("GetApplication");
            var app = getAppMethod.Invoke(Device, new object[] { productId });
            // uninstall it
            var uninstallMethod = app.GetType().GetMethod("Uninstall");
            uninstallMethod.Invoke(app, new object[0]);
        }

        public override void Disconnect()
        {
            var connectMethod = Device.GetType().GetMethod("Disconnect");
            connectMethod.Invoke(Device, new object[0]);
        }
    }
}

using System;
using System.Collections;
using System.Reflection;
using Tangerine.Common;
using Tangerine.Devices;

namespace Tangerine.BLL.Devices
{
    internal sealed class DeviceRetriever
    {
        private const string WP7SDKAssemblyFullname = "Microsoft.Smartdevice.Connectivity, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
        private const string WP8SDKAssemblyFullname = "Microsoft.Smartdevice.Connectivity, Version=11.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";

        private const string WP7SDKNotInstalledMessage = "Windows Phone 7 SDK not installed.";
        private const string WP8SDKNotInstalledMessage = "Windows Phone 8 SDK not installed.";

        private const string DatastoreManagerTypeName = "Microsoft.SmartDevice.Connectivity.DatastoreManager";

        private const int LocaleId = 1033;

        private const string WP7PlatformName = "Windows Phone 7";
        private const string WP8PlatformName = "Windows Phone 8";

        private const string WP70EmulatorName = "Windows Phone 7 Emulator";
        private const string WP71EmulatorName = "Windows Phone Emulator";
        private const string WP8EmulatorName = "Emulator";

        private const string WP7DeviceName = "Windows Phone Device";
        private const string WP8DeviceName = "Device";

        private static Assembly m_wp7sdkAssembly;
        private static Assembly m_wp8sdkAssembly;

        private static Assembly WP7SDKAssembly
        {
            get
            {
                if (m_wp7sdkAssembly == null)
                {
                    try
                    {
                        m_wp7sdkAssembly = Assembly.Load(WP7SDKAssemblyFullname);
                    }
                    catch (Exception e)
                    {
                        throw new InvalidOperationException(WP7SDKNotInstalledMessage, e);
                    }
                }
                return m_wp7sdkAssembly;
            }
        }

        private static Assembly WP8SDKAssembly
        {
            get
            {
                if (m_wp8sdkAssembly == null)
                {
                    try
                    {
                        m_wp8sdkAssembly = Assembly.Load(WP8SDKAssemblyFullname);
                    }
                    catch (Exception e)
                    {
                        throw new InvalidOperationException(WP8SDKNotInstalledMessage, e);
                    }
                }
                return m_wp8sdkAssembly;
            }
        }

        private static object GetDatastore(PlatformVersion version)
        {
            Assembly assembly = SelectSDKAssembly(version);
            Type datastoreType = assembly.GetType(DatastoreManagerTypeName);
            var ctor = datastoreType.GetConstructor(new Type[] { typeof(int) });
            return ctor.Invoke(new object[] { LocaleId });
        }

        private static Assembly SelectSDKAssembly(PlatformVersion version)
        {
            switch (version)
            {
                case PlatformVersion.Version71:
                    return WP7SDKAssembly;
                case PlatformVersion.Version80:
                    return WP8SDKAssembly;
                default:
                    throw new InvalidOperationException(String.Format("Platform version '{0} is not supported.'", version));
            }
        }

        private static object GetPlatform(PlatformVersion version)
        {
            object platform = null;

            var datastore = GetDatastore(version);
            var getPlatformsMethod = datastore.GetType().GetMethod("GetPlatforms", new Type[0]);
            var platforms = (IEnumerable)getPlatformsMethod.Invoke(datastore, new object[0]);
            foreach (var p in platforms)
            {
                var nameProperty = p.GetType().GetProperty("Name").GetGetMethod();
                var name = (string)nameProperty.Invoke(p, new object[0]);
                if (version == PlatformVersion.Version71)
                {
                    if (name == WP7PlatformName)
                    {
                        platform = p;
                        break;
                    }
                }
                else if (version == PlatformVersion.Version80)
                {
                    if (name == WP8PlatformName)
                    {
                        platform = p;
                        break;
                    }
                }
            }

            if (platform == null)
            {
                throw new InvalidOperationException(String.Format("Platform for version '{0}' was not found.", version));
            }

            return platform;
        }

        /// <summary>
        /// Returns Windows Phone device or emulator.
        /// </summary>
        /// <exception cref="InvalidOperationException">If no Windows Phone device or emulator is registered.</exception>
        internal WPDevice GetDevice(DeviceType deviceType, PlatformVersion version)
        {
            switch (deviceType)
            {
                case DeviceType.Emulator:
                    return GetEmulator(version);
                case DeviceType.Device:
                    return GetDevice(version);
                default:
                    throw new NotSupportedException(String.Format("Device type '{0}' is not supported.", deviceType.ToString()));
            }
        }

        private WPDevice GetEmulator(PlatformVersion version)
        {
            var platform = GetPlatform(version);
            var getDevicesMethod = platform.GetType().GetMethod("GetDevices", new Type[0]);
            var devices = (IEnumerable)getDevicesMethod.Invoke(platform, new object[0]);

            object emulator = null;

            foreach (var d in devices)
            {
                var nameProperty = d.GetType().GetProperty("Name").GetGetMethod();
                var name = (string)nameProperty.Invoke(d, new object[0]);
                if (CheckEmulator(name, version))
                {
                    emulator = d;
                    break;
                }
            }

            if (emulator == null)
            {
                throw new InvalidOperationException("There is no registered Windows Phone emulator.");
            }

            return CreateDevice(emulator, version);
        }

        private WPDevice CreateDevice(object device, PlatformVersion version)
        {
            switch (version)
            {
                case PlatformVersion.Version71:
                    return new WPDevice(device);
                case PlatformVersion.Version80:
                    return new WP8Device(device);
                default:
                    return new WPDevice(device);
            }
        }

        private bool CheckEmulator(string name, PlatformVersion version)
        {
            bool found = false;

            switch (version)
            {
                case PlatformVersion.Version71:
                    found = (name == WP70EmulatorName) || name.StartsWith(WP71EmulatorName);
                    break;
                case PlatformVersion.Version80:
                    found = name.StartsWith(WP8EmulatorName);
                    break;
                default:
                    throw new NotSupportedException(String.Format("SDK '{0}' is not supported.", version.ToString()));
            }

            return found;
        }

        private WPDevice GetDevice(PlatformVersion version)
        {
            var platform = GetPlatform(version);
            var getDevicesMethod = platform.GetType().GetMethod("GetDevices", new Type[0]);
            var devices = (IEnumerable)getDevicesMethod.Invoke(platform, new object[0]);

            object device = null;

            foreach (var d in devices)
            {
                var nameProperty = d.GetType().GetProperty("Name").GetGetMethod();
                var name = (string)nameProperty.Invoke(d, new object[0]);
                if (CheckDevice(name, version))
                {
                    device = d;
                    break;
                }
            }

            if (device == null)
            {
                throw new InvalidOperationException("There is no registered Windows Phone device.");
            }

            return CreateDevice(device, version);
        }

        private bool CheckDevice(string name, PlatformVersion version)
        {
            bool found = false;

            switch (version)
            {
                case PlatformVersion.Version71:
                    found = (name == WP7DeviceName);
                    break;
                case PlatformVersion.Version80:
                    found = (name == WP8DeviceName);
                    break;
                default:
                    throw new NotSupportedException(String.Format("SDK '{0}' is not supported.", version.ToString()));
            }

            return found;
        }
    }
}

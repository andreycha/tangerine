using System;
using System.Collections;
using System.Reflection;
using Tangerine.Devices;

namespace Tangerine.BLL.Devices
{
    internal sealed class DeviceRetriever
    {
        private const string WP7SDKAssemblyFullname = "Microsoft.Smartdevice.Connectivity, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
        private const string WP8SDKAssemblyFullname = "Microsoft.Smartdevice.Connectivity, Version=11.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";

        private const string WPSDKNotInstalledMessage = "Windows Phone SDK not installed.";

        private const string DatastoreManagerTypeName = "Microsoft.SmartDevice.Connectivity.DatastoreManager";

        private const int LocaleId = 1033;

        private const string WP7PlatformName = "Windows Phone 7";
        private const string WP8PlatformName = "Windows Phone 8";

        private const string WP70EmulatorName = "Windows Phone 7 Emulator";
        private const string WP71EmulatorName = "Windows Phone Emulator";
        private const string WP8EmulatorName = "Emulator";

        private const string WP7DeviceName = "Windows Phone Device";
        private const string WP8DeviceName = "Device";

        private static bool? m_isWP7SDKInstalled;
        private static bool? m_isWP8SDKInstalled;

        private static Assembly m_wp7sdkAssembly;
        private static Assembly m_wp8sdkAssembly;
        private static object m_datastore;
        private static object m_platform;

        private static bool IsWP7SDKInstalled
        {
            get
            {
                if (!m_isWP7SDKInstalled.HasValue)
                {
                    m_isWP7SDKInstalled = true;
                    try
                    {
                        var a = WP7SDKAssembly;
                    }
                    catch (Exception)
                    {
                        m_isWP7SDKInstalled = false;
                    }
                }
                return m_isWP7SDKInstalled.Value;
            }
        }

        private static bool IsWP8SDKInstalled
        {
            get
            {
                if (!m_isWP8SDKInstalled.HasValue)
                {
                    m_isWP8SDKInstalled = true;
                    try
                    {
                        var a = WP8SDKAssembly;
                    }
                    catch (Exception)
                    {
                        m_isWP8SDKInstalled = false;
                    }
                }
                return m_isWP8SDKInstalled.Value;
            }
        }

        private static Assembly WP7SDKAssembly
        {
            get
            {
                if (m_wp7sdkAssembly == null)
                {
                    m_wp7sdkAssembly = Assembly.Load(WP7SDKAssemblyFullname);
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
                    m_wp8sdkAssembly = Assembly.Load(WP8SDKAssemblyFullname);
                }
                return m_wp8sdkAssembly;
            }
        }

        private static object Datastore
        {
            get
            {
                if (m_datastore == null)
                {
                    Assembly assembly = null;
                    if (IsWP8SDKInstalled)
                    {
                        assembly = WP8SDKAssembly;
                    }
                    else if (IsWP7SDKInstalled)
                    {
                        assembly = WP7SDKAssembly;
                    }
                    else
                    {
                        throw new InvalidOperationException(WPSDKNotInstalledMessage);
                    }
                    Type datastoreType = assembly.GetType(DatastoreManagerTypeName);
                    var ctor = datastoreType.GetConstructor(new Type[] { typeof(int) });
                    m_datastore = ctor.Invoke(new object[] { LocaleId });
                }
                return m_datastore;
            }
        }

        private static object Platform
        {
            get
            {
                if (m_platform == null)
                {
                    var getPlatformsMethod = Datastore.GetType().GetMethod("GetPlatforms", new Type[0]);
                    var platforms = (IEnumerable)getPlatformsMethod.Invoke(Datastore, new object[0]);
                    foreach (var p in platforms)
                    {
                        var nameProperty = p.GetType().GetProperty("Name").GetGetMethod();
                        var name = (string)nameProperty.Invoke(p, new object[0]);
                        if (name == WP8PlatformName)
                        {
                            m_platform = p;
                            break;
                        }
                        else if (name == WP7PlatformName)
                        {
                            m_platform = p;
                            break;
                        }
                    }
                }
                return m_platform;
            }
        }

        /// <summary>
        /// Returns Windows Phone device or emulator.
        /// </summary>
        /// <exception cref="InvalidOperationException">If no Windows Phone device or emulator is registered.</exception>
        internal WPDevice GetDevice(DeviceType deviceType)
        {
            SDKVersion sdkVersion;

            if (IsWP8SDKInstalled)
            {
                sdkVersion = SDKVersion.Version8;
            }
            else if (IsWP7SDKInstalled)
            {
                sdkVersion = SDKVersion.Version7;
            }
            else
            {
                throw new InvalidOperationException(WPSDKNotInstalledMessage);
            }

            switch (deviceType)
            {
                case DeviceType.Emulator:
                    return GetEmulator(Platform, sdkVersion);
                case DeviceType.Device:
                    return GetDevice(Platform, sdkVersion);
                default:
                    throw new NotSupportedException(String.Format("Device type '{0}' is not supported.", deviceType.ToString()));
            }
        }

        private WPDevice GetEmulator(object platform, SDKVersion sdkVersion)
        {
            var getDevicesMethod = platform.GetType().GetMethod("GetDevices", new Type[0]);
            var devices = (IEnumerable)getDevicesMethod.Invoke(platform, new object[0]);

            object emulator = null;

            foreach (var d in devices)
            {
                var nameProperty = d.GetType().GetProperty("Name").GetGetMethod();
                var name = (string)nameProperty.Invoke(d, new object[0]);
                if (CheckEmulator(name, sdkVersion))
                {
                    emulator = d;
                    break;
                }
            }

            if (emulator == null)
            {
                throw new InvalidOperationException("There is no registered Windows Phone emulator.");
            }

            return new WP7Device(emulator);
        }

        private bool CheckEmulator(string name, SDKVersion sdkVersion)
        {
            bool found = false;

            switch (sdkVersion)
            {
                case SDKVersion.Version7:
                    found = (name == WP70EmulatorName) || name.StartsWith(WP71EmulatorName);
                    break;
                case SDKVersion.Version8:
                    found = name.StartsWith(WP8EmulatorName);
                    break;
                default:
                    throw new NotSupportedException(String.Format("SDK '{0}' is not supported.", sdkVersion.ToString()));
            }

            return found;
        }

        private WPDevice GetDevice(object platform, SDKVersion sdkVersion)
        {
            var getDevicesMethod = platform.GetType().GetMethod("GetDevices", new Type[0]);
            var devices = (IEnumerable)getDevicesMethod.Invoke(platform, new object[0]);

            object device = null;

            foreach (var d in devices)
            {
                var nameProperty = d.GetType().GetProperty("Name").GetGetMethod();
                var name = (string)nameProperty.Invoke(d, new object[0]);
                if (CheckDevice(name, sdkVersion))
                {
                    device = d;
                    break;
                }
            }

            if (device == null)
            {
                throw new InvalidOperationException("There is no registered Windows Phone device.");
            }

            return new WP7Device(device);
        }

        private bool CheckDevice(string name, SDKVersion sdkVersion)
        {
            bool found = false;

            switch (sdkVersion)
            {
                case SDKVersion.Version7:
                    found = (name == WP7DeviceName);
                    break;
                case SDKVersion.Version8:
                    found = (name == WP8DeviceName);
                    break;
                default:
                    throw new NotSupportedException(String.Format("SDK '{0}' is not supported.", sdkVersion.ToString()));
            }

            return found;
        }


        private enum SDKVersion
        {
            Version7,
            Version8
        }
    }
}

using System;
using System.Collections;
using System.Reflection;
using Tangerine.Devices;

namespace Tangerine.BLL.Devices
{
    internal sealed class DeviceRetriever
    {
        private const string WP7SDKAssemblyFullname = "Microsoft.Smartdevice.Connectivity, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";

        private const string WP7SDKNotInstalledMessage = "Windows Phone 7 SDK not installed";

        private const string DatastoreManagerTypeName = "Microsoft.SmartDevice.Connectivity.DatastoreManager";

        private const int LocaleId = 1033;

        private const string WP7PlatformName = "Windows Phone 7";
        private const string WP70EmulatorName = "Windows Phone 7 Emulator";
        private const string WP71EmulatorName = "Windows Phone Emulator";
        private const string WPDeviceName = "Windows Phone Device";

        private static bool? m_isWP7SDKInstalled;

        private static Assembly m_wp7sdkAssembly;
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

        private static object Datastore
        {
            get
            {
                if (m_datastore == null)
                {
                    Assembly assembly = null;
                    if (IsWP7SDKInstalled)
                    {
                        assembly = WP7SDKAssembly;
                    }
                    else
                    {
                        throw new InvalidOperationException(WP7SDKNotInstalledMessage);
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
                    var getPlatformsMethod = Datastore.GetType().GetMethod("GetPlatforms");
                    var platforms = (IEnumerable)getPlatformsMethod.Invoke(Datastore, new object[0]);
                    foreach (var p in platforms)
                    {
                        var nameProperty = p.GetType().GetProperty("Name").GetGetMethod();
                        var name = (string)nameProperty.Invoke(p, new object[0]);
                        if (name == WP7PlatformName)
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
            if (IsWP7SDKInstalled)
            {
                switch (deviceType)
                {
                    case DeviceType.Emulator:
                        return GetEmulator(Platform);
                    case DeviceType.Device:
                        return GetDevice(Platform);
                    default:
                        throw new NotSupportedException(String.Format("Device type '{0}' is not supported", deviceType.ToString()));
                }
            }
            else
            {
                throw new InvalidOperationException(WP7SDKNotInstalledMessage);
            }
        }

        private WPDevice GetEmulator(object platform)
        {
            var getDevicesMethod = platform.GetType().GetMethod("GetDevices");
            var devices = (IEnumerable)getDevicesMethod.Invoke(platform, new object[0]);

            object emulator = null;

            foreach (var d in devices)
            {
                var nameProperty = d.GetType().GetProperty("Name").GetGetMethod();
                var name = (string)nameProperty.Invoke(d, new object[0]);
                if (name == WP70EmulatorName)
                {
                    emulator = d;
                    break;
                }
                else if (name.StartsWith(WP71EmulatorName))
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

        private WPDevice GetDevice(object platform)
        {
            var getDevicesMethod = platform.GetType().GetMethod("GetDevices");
            var devices = (IEnumerable)getDevicesMethod.Invoke(platform, new object[0]);

            object device = null;

            foreach (var d in devices)
            {
                var nameProperty = d.GetType().GetProperty("Name").GetGetMethod();
                var name = (string)nameProperty.Invoke(d, new object[0]);
                if (name == WPDeviceName)
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
    }
}

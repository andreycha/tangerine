using System;
using System.Linq;
using Microsoft.SmartDevice.Connectivity;
using Tangerine.Devices;

namespace Tangerine.BLL.Devices
{
    internal sealed class DeviceRetriever
    {
        private const int LocaleId = 1033;
        private const string WP7Platform = "Windows Phone 7";
        private const string WP70Emulator = "Windows Phone 7 Emulator";
        private const string WP71Emulator = "Windows Phone Emulator";
        private const string WPDevice = "Windows Phone Device";

        /// <summary>
        /// Returns Windows Phone emulator.
        /// </summary>
        /// <exception cref="InvalidOperationException">If no Windows Phone emulator is registered.</exception>
        /// <returns>Windows Phone emulator.</returns>
        internal Device GetDevice(DeviceType deviceType)
        {
            DatastoreManager datastoreManager = new DatastoreManager(LocaleId);
            Platform platform = datastoreManager.GetPlatforms().Single(p => p.Name == WP7Platform);

            switch (deviceType)
            {
                case DeviceType.Emulator:
                    return GetEmulator(platform);
                case DeviceType.Device:
                    return GetDevice(platform);
                default:
                    throw new NotSupportedException(String.Format("Device type '{0}' is not supported", deviceType.ToString()));
            }
        }

        private Device GetEmulator(Platform platform)
        {
            Device emulator = platform.GetDevices().FirstOrDefault(d => d.Name == WP70Emulator);
            if (emulator == null)
            {
                emulator = platform.GetDevices().FirstOrDefault(d => d.Name.StartsWith(WP71Emulator));
            }

            if (emulator == null)
            {
                throw new InvalidOperationException("There is no registered Windows Phone emulator.");
            }

            return emulator;
        }

        private Device GetDevice(Platform platform)
        {
            Device device = platform.GetDevices().FirstOrDefault(d => d.Name == WPDevice);

            if (device == null)
            {
                throw new InvalidOperationException("Windows Phone device was not found.");
            }

            return device;
        }
    }
}

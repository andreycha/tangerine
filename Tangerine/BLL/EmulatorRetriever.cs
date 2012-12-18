using System;
using System.Linq;
using Microsoft.SmartDevice.Connectivity;

namespace Tangerine.BLL
{
    internal sealed class EmulatorRetriever
    {
        private const int LocaleId = 1033;
        private const string WP7Platform = "Windows Phone 7";
        private const string WP70Emulator = "Windows Phone 7 Emulator";
        private const string WP71Emulator = "Windows Phone Emulator";

        /// <summary>
        /// Returns Windows Phone emulator.
        /// </summary>
        /// <exception cref="InvalidOperationException">If no Windows Phone emulator is registered.</exception>
        /// <returns>Windows Phone emulator.</returns>
        internal Device GetEmulator()
        {
            DatastoreManager datastoreManager = new DatastoreManager(LocaleId);
            Platform platform = datastoreManager.GetPlatforms().Single(p => p.Name == WP7Platform);
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
    }
}

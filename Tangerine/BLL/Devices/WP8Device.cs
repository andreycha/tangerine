using System;
using System.Reflection;
using Tangerine.Devices;

namespace Tangerine.BLL.Devices
{
    public class WP8Device : WPDevice
    {
        private string EmulatorLaunchError = "0x80131500";

        public WP8Device(object device) : base(device)
        {
        }

        public override void Connect()
        {
            try
            {
                base.Connect();
            }
            catch (TargetInvocationException e)
            {
                if (e.InnerException.Message.Contains(EmulatorLaunchError))
                {
                    throw new InvalidOperationException("Can't run Windows Phone 8 emulator.");
                }
                else
                {
                    throw;
                }
            }
        }
    }
}

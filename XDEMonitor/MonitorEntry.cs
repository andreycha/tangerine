using System;

namespace XDEMonitor
{
    internal sealed class MonitorEntry
    {
        internal string WriteBuffer { get; private set; }

        internal DateTime TimeStamp { get; private set; }

        internal MonitorEntry(string writeBuffer)
        {
            WriteBuffer = writeBuffer;
            TimeStamp = DateTime.Now;
        }
    }
}

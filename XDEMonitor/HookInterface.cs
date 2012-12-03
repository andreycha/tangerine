using System;
using System.Text.RegularExpressions;

namespace XDEMonitor
{
    public class HookInterface : MarshalByRefObject
    {
        private readonly object m_lockObject = new object();

        public void ReportException(Exception exception)
        {
            Console.WriteLine("The target process has reported an error:\r\n" + exception.ToString());
        }

        public void Ping()
        {
        }

        public void OnWriteConsole(int inClientPID, string[] buffers)
        {
            if (frmMain.isMonitoring)
            {
                lock (m_lockObject)
                {
                    for (int i = buffers.Length - 1; i >= 0; i--)
                    {
                        string pattern = @"PID:\w+\sTID:\w+\s";
                        string result = Regex.Replace(buffers[i], pattern, "");
                        frmMain.monitorQueue.Enqueue(new MonitorEntry(result));
                    }
                }
            }
        }
    }
}
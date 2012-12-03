using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using EasyHook;
using XDEMonitor;

namespace XDEHook
{
    public class Main : IEntryPoint
    {
        private readonly object m_lockObject = new object();

        private HookInterface m_hookInterface;
        private LocalHook m_writeConsoleHook;
        private Stack<string> m_queue = new Stack<string>();
        
        public Main(RemoteHooking.IContext inContext, string inChannelName)
        {
            m_hookInterface = RemoteHooking.IpcConnectClient<HookInterface>(inChannelName);
            m_hookInterface.Ping();
        }

        public void Run(RemoteHooking.IContext inContext, string inChannelName)
        {
            if (InstallHook())
            {
                RemoteHooking.WakeUpProcess();
                WaitForHostProcessTermination();
            }
        }

        private bool InstallHook()
        {
            bool installationSucceeded = true;

            try
            {
                m_writeConsoleHook = LocalHook.Create(
                    LocalHook.GetProcAddress("kernel32.dll", "WriteFile"),
                    new DWriteFile(WriteFile_Hooked),
                    this
                    );
                m_writeConsoleHook.ThreadACL.SetExclusiveACL(new Int32[] { 0 });
            }
            catch (Exception e)
            {
                m_hookInterface.ReportException(e);
                installationSucceeded = false;
            }

            return installationSucceeded;
        }

        private void WaitForHostProcessTermination()
        {
            try
            {
                while (true)
                {
                    Thread.Sleep(500);

                    // transmit newly monitored file accesses
                    if (m_queue.Count > 0)
                    {
                        string[] package;
                        lock (m_lockObject)
                        {
                            package = m_queue.ToArray();
                            m_queue.Clear();
                        }
                        m_hookInterface.OnWriteConsole(RemoteHooking.GetCurrentProcessId(), package);
                    }
                    else
                    {
                        m_hookInterface.Ping();
                    }
                }
            }
            catch
            {
                // Ping() will raise an exception if host is unreachable
            }
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
        delegate bool DWriteFile(
            IntPtr hFile,
            IntPtr lpBuffer, 
            uint nNumberOfBytesToWrite,
            out uint lpNumberOfBytesWritten,
            [In] IntPtr lpOverlapped
            );

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        static extern bool WriteFile(
            IntPtr hFile,
            byte[] lpBuffer, 
            uint nNumberOfBytesToWrite,
            out uint lpNumberOfBytesWritten,
            [In] IntPtr lpOverlapped
            );

        private bool WriteFile_Hooked(
            IntPtr hFile,
            IntPtr lpBuffer,
            uint nNumberOfBytesToWrite,
            out uint lpNumberOfBytesWritten,
            [In] IntPtr lpOverlapped
            )
        {
            byte[] bytes = new byte[nNumberOfBytesToWrite];

            try
            {
                Main This = (Main)HookRuntimeInfo.Callback;
                lock (This.m_queue)
                {
                    ProcessModule module = HookRuntimeInfo.CallingUnmanagedModule;
                    if (module.ModuleName == "XDE.exe")
                    {
                        PutMessageInQueue(lpBuffer, bytes, nNumberOfBytesToWrite, This);
                    }                    
                }
            }
            catch
            {
                // TODO: how can happen and what to do?
            }

            // call original API
            return WriteFile(hFile, bytes, nNumberOfBytesToWrite, out lpNumberOfBytesWritten, lpOverlapped);
        }

        private void PutMessageInQueue(IntPtr lpBuffer, byte[] bytes, uint nNumberOfBytesToWrite, Main This)
        {
            for (uint i = 0; i < nNumberOfBytesToWrite; i++)
            {
                bytes[i] = Marshal.ReadByte(lpBuffer, (int)i);
            }

            string output = "";
            for (uint i = 0; i < nNumberOfBytesToWrite; i++)
            {
                output += (char)bytes[i];
            }

            This.m_queue.Push(output);
        }
    }
}

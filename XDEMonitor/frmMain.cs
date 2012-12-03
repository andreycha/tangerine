using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Remoting;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using EasyHook;

namespace XDEMonitor
{
    public partial class frmMain : Form
    {
        internal static Queue<MonitorEntry> monitorQueue = new Queue<MonitorEntry>();
        internal static bool isMonitoring = true;

        private string channelName = null;
        private Regex methodRegex = new Regex("(\\*Type: )(?<type>.+), (Method name: )(?<method>.+)");
        private Regex paramRegex = new Regex("(\\*Parameter name: )(?<varname>[\\w\\W]*)");
        private Regex valueRegex = new Regex("(?<var>[\\w\\W]*)");
        private Regex returnRegex = new Regex("(\\*Return value: )(?<method>.+)");

        public frmMain()
        {
            InitializeComponent();
            RemoteHooking.IpcCreateServer<HookInterface>(ref channelName, WellKnownObjectMode.Singleton);
            if (!InjectDll())
            {
                Application.Exit();
            }
            timer_Tick(null, null);
        }

        public bool InjectDll()
        {
            Process[] psArray = Process.GetProcessesByName("XDE");
            if (psArray.Length != 1)
            {
                return false;
            }

            int pid = psArray[0].Id;

            try
            {
                RemoteHooking.Inject(pid, "XDEHook.dll", "XDEHook.dll", channelName);
            }
            catch (Exception e)
            {
                MessageBox.Show("There was an error while connecting to target: \r\n" + e.ToString(), "Error");
                return false;
            }

            labelMsg.Text = "API hook status: Successfull, PID=" + pid.ToString();

            return true;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();

            try
            {
                lock (monitorQueue)
                {
                    StringBuilder item = null;
                    string strParameters = "";
                    string strReturnValue = "";
                    bool isDumpingVariables = false;
                    bool isDumpingReturnValue = false;

                    while (monitorQueue.Count > 0)
                    {
                        MonitorEntry entry = monitorQueue.Dequeue();

                        // beginning of method dump
                        if (methodRegex.IsMatch(entry.WriteBuffer))
                        {
                            if (item != null)
                            {
                                if (isDumpingVariables)
                                {
                                    item.AppendFormat("({0})", strParameters);
                                    // close previous method dump
                                    isDumpingVariables = false;
                                    strParameters = "";
                                }
                                if (isDumpingReturnValue)
                                {
                                    item.Append(strReturnValue);
                                    // close previous method dump
                                    isDumpingReturnValue = false;
                                    strReturnValue = "";
                                }
                                item.AppendLine();
                                rtbConsole.AppendText(item.ToString());
                                rtbConsole.ScrollToCaret();
                            }

                            item = new StringBuilder().AppendFormat("[{0}] ", entry.TimeStamp.ToLongTimeString());

                            Match match = methodRegex.Match(entry.WriteBuffer);
                            string strType = GetCleanString(match.Groups["type"].ToString());
                            item.Append(strType).Append(".");

                            string strMethodName = GetCleanString(match.Groups["method"].ToString());
                            item.Append(strMethodName);                            
                        }
                        // beginning of var name
                        else if (paramRegex.IsMatch(entry.WriteBuffer))
                        {
                            Match match = paramRegex.Match(entry.WriteBuffer);
                            if (!String.IsNullOrEmpty(strParameters))
                            {
                                strParameters += ", ";
                            }
                            strParameters += GetCleanString(match.Groups["varname"].ToString()) + ": ";
                            isDumpingVariables = true;
                        }
                        else if (returnRegex.IsMatch(entry.WriteBuffer))
                        {
                            if (item != null)
                            {
                                if (isDumpingVariables)
                                {
                                    item.AppendFormat("({0})", strParameters);
                                    // close previous parameters dump
                                    isDumpingVariables = false;
                                    strParameters = "";
                                }
                                if (isDumpingReturnValue)
                                {
                                    item.Append(strReturnValue);
                                    // close previous return dump
                                    strReturnValue = "";
                                }
                                item.AppendLine();
                                rtbConsole.AppendText(item.ToString());
                                rtbConsole.ScrollToCaret();
                            }

                            isDumpingVariables = false;
                            isDumpingReturnValue = true;
                            Match match = returnRegex.Match(entry.WriteBuffer);

                            item = new StringBuilder();
                            item.AppendFormat("\tRETURN VALUE ({0}): ", GetCleanString(match.Groups["method"].ToString()));
                        }
                        else if (isDumpingVariables)
                        {
                            Match match = valueRegex.Match(entry.WriteBuffer);
                            strParameters += GetCleanString(match.Groups["var"].ToString());
                        }
                        else if (isDumpingReturnValue)
                        {
                            Match match = valueRegex.Match(entry.WriteBuffer);
                            strReturnValue += GetCleanString(match.Groups["var"].ToString());
                        }
                    }

                    if (item != null)
                    {
                        if (isDumpingVariables)
                        {
                            item.AppendFormat("({0})", strParameters);
                        }
                        if (isDumpingReturnValue)
                        {
                            item.Append(strReturnValue);
                        }
                        item.AppendLine();
                        rtbConsole.AppendText(item.ToString());
                        rtbConsole.ScrollToCaret();
                    }
                }
            }
            finally
            {
                timer.Start();
            }
        }

        private string GetCleanString(string str)
        {
            if (String.IsNullOrEmpty(str))
            {
                return str;
            }

            return str.Replace("\r\n", "").Replace("\r", "");
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            switch (isMonitoring)
            {
                case false:
                    timer.Start();
                    isMonitoring = true;
                    btnStop.Text = "Stop";                    
                    break;

                case true:
                    timer.Stop();
                    isMonitoring = false;
                    btnStop.Text = "Start";
                    break;
            }
        }
       
        private void btnSearch_Click(object sender, EventArgs e)
        {
            string strSearch = textSearchStr.Text.ToLower();
            if (strSearch != "")
            {
                ResetBackgroundColor();
                HighlightMatchedSearchItems(strSearch);
            }            
        }

        private void ResetBackgroundColor()
        {
            //foreach (ListViewItem item in listViewXDE.Items)
            //{
            //    foreach (ListViewItem.ListViewSubItem subItem in item.SubItems)
            //    {
            //        item.BackColor = Color.White;
            //        subItem.BackColor = Color.White;
            //    }
            //}
        }

        private void HighlightMatchedSearchItems(string strSearch)
        {
            //foreach (ListViewItem item in listViewXDE.Items)
            //{
            //    foreach (ListViewItem.ListViewSubItem subItem in item.SubItems)
            //    {
            //        if (subItem.Text.ToLower().Contains(strSearch))
            //        {
            //            item.BackColor = Color.Blue;
            //            subItem.BackColor = Color.Blue;
            //            listViewXDE.TopItem = item;
            //        }
            //    }
            //}
        }
        
        private void onSaveAs(object sender, EventArgs e)
        {
            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Title = "Select destination file";
                dlg.DefaultExt = ".txt";
                dlg.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    WriteToFile(dlg.FileName);
                }
            }
        }

        private void WriteToFile(string fileName)
        {
            using (TextWriter writer = new StreamWriter(fileName))
            {
                //foreach (ListViewItem item in listViewXDE.Items)
                //{
                //    writer.WriteLine("========");
                //    foreach (ListViewItem.ListViewSubItem subItem in item.SubItems)
                //    {
                        writer.Write(rtbConsole.Text/*subItem.Text*/);
                //    }
                //    writer.WriteLine("========");
                //}
            }
        }

        private void OnExit(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}

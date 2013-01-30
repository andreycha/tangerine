using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Aga.Controls.Tree;
using Microsoft.Win32;
using Tangerine.BLL;
using Tangerine.BLL.Hooks;
using Tangerine.Common;
using Tangerine.Devices;
using Tangerine.UI.BLL;
using Tangerine.UI.BLL.AssemblyTree;

namespace Tangerine.UI
{
    public partial class frmMain : Form, IMainView
    {
        private const string XDEKeyName = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\XDE";

        private readonly MainPresenter m_presenter;

        private TreeModel m_TreeModel;
        private ContextMenu m_contextMenu;

        public frmMain()
        {
            InitializeComponent();

            InitializeDeployContextMenu();

            HideExcessTabs();

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            m_presenter = new MainPresenter(this);

            trvAssemblies.Model = m_TreeModel;
            trvAssemblies.ShowNodeToolTips = true;
            nodeIcon1.ToolTipProvider = new ToolTipProv("IO operations");
            nodeIcon2.ToolTipProvider = new ToolTipProv("Net operations");
            nodeIcon3.ToolTipProvider = new ToolTipProv("Security operations");
        }

        private void InitializeDeployContextMenu()
        {
            var emulatorMenuItem = new MenuItem("Emulator", DeployToEmulator);
            var deviceMenuItem = new MenuItem("Device", DeployToDevice);

            m_contextMenu = new ContextMenu();
            m_contextMenu.MenuItems.Add(emulatorMenuItem);
            m_contextMenu.MenuItems.Add(deviceMenuItem);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            if (exception != null)
            {
                ShowError(exception);
            }
        }

        private void ShowError(Exception exception)
        {
#if DEBUG
            MessageBox.Show(
                exception.Message + Environment.NewLine + Environment.NewLine + exception.StackTrace,
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
                );
#else
            MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
#endif
        }

        public void AddOutputText(string text)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(AddOutputTextInternal), text);
            }
            else
            {
                AddOutputTextInternal(text);
            }
        }

        private void AddOutputTextInternal(string text)
        {
            txtOutput.AppendText(text);
            txtOutput.AppendText(Environment.NewLine);
            txtOutput.Update();
        }

        public void ResetButton(string target)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(ResetButtonInternal), target);
            }
            else
            {
                ResetButtonInternal(target);
            }
        }

        public void SetManifestInformation(IManifest manifest)
        {
            if (InvokeRequired)
            { 
                BeginInvoke(new Action<IManifest>(SetManifestInfoInternal), manifest);
            }
            else
            {
                SetManifestInfoInternal(manifest);
            }
        }

        void SetManifestInfoInternal(IManifest manifest)
        {
            lblProductId.Text = "Product ID: " + manifest.ProductId;
            lblTitle.Text = "Title: " + manifest.Title;
            lblVersion.Text = "Version: " + manifest.Version;
            lblPlatformVersion.Text = "Platform version: " + Util.GetEnumDescription(manifest.PlatformVersion);
            lblAuthor.Text = "Author: " + manifest.Author;
            tbxCapabilities.Clear();
            tbxRequirements.Clear();
            tbxScreenResolutions.Clear();
            tbxFileTypes.Clear();
            tbxURIs.Clear();
            foreach (var capability in manifest.Capabilities)
            {
                tbxCapabilities.AppendText(capability.Id + ": " + capability.Description + "\r\n");
            }
            foreach (var requirement in manifest.Requirements)
            {
                tbxRequirements.AppendText(string.Format("{0}: {1}\r\n", requirement.Id, requirement.Description));
            }
            foreach (var screenResolution in manifest.ScreenResolutions)
            {
                tbxScreenResolutions.AppendText(string.Format("{0}: {1}\r\n", screenResolution.ToString(), Util.GetEnumDescription(screenResolution)));
            }
            if (manifest.PlatformVersion != PlatformVersion.Version71)
            {
                ShowExcessTabs();
                foreach (var fileType in manifest.SupportedFileTypes)
                {
                    tbxFileTypes.AppendText(fileType + "\r\n");
                }
                foreach (var uri in manifest.AssociatedURIs)
                {
                    tbxURIs.AppendText(uri + "\r\n");
                }
            }
            else
            {
                HideExcessTabs();
            }
        }

        void HideExcessTabs()
        {
            if (tabXapFileInformation.TabPages.Contains(tabAssociations))
                tabXapFileInformation.TabPages.Remove(tabAssociations);
        }

        void ShowExcessTabs()
        {
            if (!tabXapFileInformation.TabPages.Contains(tabAssociations))
                tabXapFileInformation.TabPages.Add(tabAssociations);
        }

        public void SetExpanded(Node node)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<Node>(SetExpandedInternal), node);
            }
            else
            {
                SetExpandedInternal(node);
            }
        }

        private void SetExpandedInternal(Node node)
        {
            var treeNode = trvAssemblies.FindNode(m_TreeModel.GetPath(node));
            treeNode.IsExpanded = true;
        }

        public void SetTreeModel(TreeModel treeModel)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<TreeModel>(SetTreeModelInternal), treeModel);
            }
            else
            {
                SetTreeModelInternal(treeModel);
            }
        }

        private void SetTreeModelInternal(TreeModel treeModel)
        {
            m_TreeModel = treeModel;
            trvAssemblies.Model = m_TreeModel;
        }

        public void InitializeAssemblyTree(IEnumerable<string> assemblies)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<IEnumerable<string>>(InitializeAssemblyTreeInternal), assemblies);
            }
            else
            {
                InitializeAssemblyTreeInternal(assemblies);
            }
        }

        void InitializeAssemblyTreeInternal(IEnumerable<string> assemblies)
        {
        }

        private void ResetButtonInternal(string target)
        {
            switch (target)
            {
                case "deploy":
                    btnBrowseFile.Enabled = true;
                    btnBrowseFolder.Enabled = true;
                    btnDeploy.Enabled = true;
                    break;

                case "run":
                    xDEMonitorToolStripMenuItem.Enabled = true;
                    btnBrowseFile.Enabled = true;
                    btnBrowseFolder.Enabled = true;
                    btnDeploy.Enabled = true;
                    break;
            }
        }

        private void CheckXDEConsole()
        {
            RegistryKey localMachineKey = Registry.LocalMachine;
            RegistryKey xdeKey = localMachineKey.OpenSubKey(@"SOFTWARE\Microsoft\XDE", true);
            if (xdeKey != null)
            {
                object keyValue = Registry.GetValue(XDEKeyName, "EnableConsole", "Not Exists");
                if (keyValue.ToString() == "Not Exists")
                {                     
                    xdeKey.SetValue("EnableConsole", 1, RegistryValueKind.DWord);
                }
                else
                {
                    int b = (int)xdeKey.GetValue("EnableConsole");
                    if (b != 1)
                    {
                        xdeKey.SetValue("EnableConsole", 1, RegistryValueKind.DWord);
                    }
                }
            }
            else
            {
                MessageBox.Show("Phone Emulator is not installed!", "Error", MessageBoxButtons.OK);
            }
        }

        private void btnBrowseFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlgFile = new OpenFileDialog())
            {
                dlgFile.Title = "Select target application";
                dlgFile.DefaultExt = ".xap";
                dlgFile.Filter = "XAP (*.xap)|*.xap";
                if (dlgFile.ShowDialog() == DialogResult.OK)
                {
                    LoadApplication(dlgFile.FileName);
                }
            }
        }

        private void LoadApplication(string path)
        {
            lbMethods.Items.Clear();
            RefreshHookButtons();
            txtFilePath.Text = path;
            txtOutput.Clear();
            
            AddOutputText("Parsing target application file...");

            m_presenter.LoadApplication(path);

            btnAddHook.Enabled = true;
        }

        private void btnBrowseFolder_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dlgFolder = new FolderBrowserDialog())
            {
                dlgFolder.Description = "Select folder with target application";
                dlgFolder.ShowNewFolderButton = false;
                if (dlgFolder.ShowDialog() == DialogResult.OK)
                {
                    LoadApplication(dlgFolder.SelectedPath);
                }
            }
        }
             
        private void btnDeploy_Click(object sender, EventArgs e)
        {
            m_contextMenu.Show(btnDeploy, new Point(btnDeploy.Width, 0));
        }

        private void PrepareForDeploy()
        {
            txtOutput.Clear();
            btnBrowseFile.Enabled = false;
            btnBrowseFolder.Enabled = false;
            btnDeploy.Enabled = false;

            KillXDEMonitor();
        }

        private void DeployToEmulator(object sender, EventArgs e)
        {
            PrepareForDeploy();
            m_presenter.Deploy(DeviceType.Emulator);
        }

        private void DeployToDevice(object sender, EventArgs e)
        {
            PrepareForDeploy();
            m_presenter.Deploy(DeviceType.Device);
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            CheckXDEConsole();            
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            KillXDEMonitor();
            m_presenter.Cleanup();
        }
    
        private void KillXDEMonitor()
        {
            Process[] ps = Process.GetProcessesByName("XDEMonitor");
            if (ps.Length != 0)
            {
                ps[0].Kill();
            }
        }

        private void OnToolsXDEMonitor_Clicked(object sender, EventArgs e)
        {
            Process ps = new Process();
            ps.StartInfo.FileName = Directory.GetCurrentDirectory().ToString() + "\\"
                + ConfigurationManager.AppSettings["MonitorProgram"];
            ps.StartInfo.WorkingDirectory = Directory.GetCurrentDirectory().ToString();

            try
            {
                ps.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error while launching XDEMonitor, please check the tool path and name in the config file." + "\n\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                    );
            }
        }

        private void OnExit(object sender, EventArgs e)
        {
            KillXDEMonitor();
            Application.Exit();
        }

        string IMainView.ShowSearchForm(IEnumerable<string> searchItems)
        {
            using (frmAddHook form = new frmAddHook(searchItems))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    return form.GetSelectedItem();
                }
            }
            return null;
        }

        MethodHook IMainView.GetSelectedHook()
        {
            return GetSelectedHook();
        }

        private MethodHook GetSelectedHook()
        {
            return (MethodHook)lbMethods.SelectedItem;
        }

        void IMainView.ShowEditHookForm(MethodHook methodHook)
        {
            using (frmEditHook form = new frmEditHook(methodHook))
            {
                form.ShowDialog();
            }
        }

        private void btnAddMethod_Click(object sender, EventArgs e)
        {
            m_presenter.AddMethodHook();
        }

        void IMainView.AddMethod(MethodHook methodHook)
        {
            lbMethods.Items.Add(methodHook);
            lbMethods.SelectedItem = methodHook;
        }

        private void chbLogMethods_CheckedChanged(object sender, EventArgs e)
        {
            bool logMethods = chbLogMethods.Checked;
            chbLogParameters.Enabled = logMethods;
            chbLogReturnValues.Enabled = logMethods;
            if (!logMethods)
            {
                chbLogParameters.Checked = false;
                chbLogReturnValues.Checked = false;
            }
        }

        public bool GetLogMethodNames()
        {
            return chbLogMethods.Checked;
        }

        public bool GetLogParameterValues()
        {
            return chbLogParameters.Checked;
        }

        private void lbMethods_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshHookButtons();
        }

        private void RefreshHookButtons()
        {
            bool isMethodSelected = (lbMethods.SelectedIndex > -1);
            btnEditHook.Enabled = isMethodSelected;
            btnRemoveHook.Enabled = isMethodSelected;
        }

        private void btnEditHook_Click(object sender, EventArgs e)
        {
            m_presenter.EditMethodHook();
        }

        public IEnumerable<MethodHook> GetHooks()
        {
            return lbMethods.Items.Cast<MethodHook>();
        }

        private void btnRemoveHook_Click(object sender, EventArgs e)
        {
            MethodHook hookToRemove = GetSelectedHook();
            lbMethods.Items.Remove(hookToRemove);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (frmAbout form = new frmAbout())
            {
                form.ShowDialog();
            }
        }

        private void trvAssemblies_NodeMouseDoubleClick(object sender, TreeNodeAdvMouseEventArgs e)
        {
            var methodNode = e.Node.Tag as MethodNode;
            if (methodNode != null)
            {
                using (frmDisassembledMethod form = new frmDisassembledMethod(methodNode.Definition))
                {
                    form.ShowDialog();
                }
            }
        }

        void IMainView.ShowError(Exception exception)
        {
            ShowError(exception);
        }

        public bool GetLogReturnValues()
        {
            return chbLogReturnValues.Checked;
        }
    }    
}

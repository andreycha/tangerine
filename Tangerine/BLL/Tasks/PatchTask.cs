using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using Microsoft.SmartDevice.Connectivity;
using Shell32;
using Tangerine.BLL.Hooks;

namespace Tangerine.BLL.Tasks
{
    /// <summary>
    /// This task instrumentates given XAP, resigns it and deploys to the emulator.
    /// </summary>
    internal class PatchTask
    {
        private const string NormalAppGenre = "NormalApp";
        public const string InstrumentedFilesFolder = "Instrumented";
        public const string InstrumentedXAPFolder = "InstrumentedXAP";

        private readonly XAP m_xap;
        private readonly Action<string> m_addText;
        private readonly Action<string> m_resetButton;

        private readonly byte[] m_emptyZip = new byte[] { 80, 75, 5, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private readonly IHookProvider m_hookProvider;
        private readonly DeployerThreadConfig m_config;

        internal PatchTask(
            XAP xap,
            IHookProvider hookProvider,
            DeployerThreadConfig config,
            Action<string> addText,
            Action<string> resetButton
            )
        {
            m_xap = xap;
            m_addText = addText;
            m_resetButton = resetButton;
            m_hookProvider = hookProvider;
            m_config = config;
        }

        internal void Run()
        {
            if (!m_xap.IsUnpacked)
            {
                m_xap.Parse();
            }

            if (m_xap.RemoveDRM())
            {
                m_addText.Invoke("DRM file was removed.");
            }

            PrepareFilesForPatching();

            foreach (var asm in m_xap.Assemblies)
            {
                string asmName = Path.GetFileName(asm.AssemblyPath);
                string asmPath = GetInstrumentedFilePath(asmName);
                try
                {
                    AssemblyName.GetAssemblyName(asmPath);
                    m_addText.Invoke("Patching " + asm.AssemblyPath);
                    AssemblyPatcher patcher = new AssemblyPatcher(asmPath, m_hookProvider);
                    patcher.PatchAssembly();
                }
                catch (BadImageFormatException)
                {
                    m_addText.Invoke("Skipping native dll file: " + asm.AssemblyPath);
                }
            }

            m_addText.Invoke("Finished patching assemblies.");

            m_addText.Invoke("Signing dll files...");
            m_xap.ReplaceSignatures();
            m_addText.Invoke("(Done)");

            string newFileName = CreateNewXAP();

            InstallApplication(newFileName);

            RunXDEMonitor();
        }

        private void PrepareFilesForPatching()
        {
            string instrumentedXAPPath = GetInstrumentedXAPPath();
            if (Directory.Exists(instrumentedXAPPath))
            {
                Directory.Delete(instrumentedXAPPath, true);
            }

            Directory.CreateDirectory(instrumentedXAPPath);

            string instrumentedPath = GetInstrumentedPath();
            if (Directory.Exists(instrumentedPath))
            {
                Directory.Delete(instrumentedPath, true);
            }

            Directory.CreateDirectory(instrumentedPath);
            
            foreach (string file in Directory.GetFiles(m_xap.GetPath()))
            {
                string target = GetInstrumentedFilePath(file);
                File.Copy(file, target, true);
            }
        }

        private string GetInstrumentedXAPPath()
        {
            return Path.Combine(m_xap.GetPath(), InstrumentedXAPFolder);
        }

        private string GetInstrumentedPath()
        {
            return Path.Combine(m_xap.GetPath(), InstrumentedFilesFolder);
        }

        private string GetInstrumentedFilePath(string file)
        {
            return Path.Combine(GetInstrumentedPath(), Path.GetFileName(file));
        }

        private string CreateNewXAP()
        {
            m_addText.Invoke("Creating new XAP file...");

            string tempFileName = Path.Combine(GetInstrumentedXAPPath(), m_xap.ProductId);
            string zipFileName = tempFileName + ".zip";
            string newFileName = tempFileName + ".xap";

            File.Delete(zipFileName);
            File.Delete(newFileName);
            
            CreateEmptyZip(zipFileName);
            CopyZipFiles(GetInstrumentedPath(), zipFileName);
            
            // wait for shell32.dll
            Thread.Sleep(m_config.ZipWaitTime);    
            File.Move(zipFileName, newFileName);
            m_addText.Invoke("(Done)");

            return newFileName;
        }

        private void CreateEmptyZip(string destinationFile)
        {
            using (FileStream fs = File.Create(destinationFile))
            {
                fs.Write(m_emptyZip, 0, m_emptyZip.Length);
                fs.Flush();
            }
        }

        private static void CopyZipFiles(string sourceFolder, string destinationFile)
        {
            Shell shell = new Shell();
            Folder srcFolder = shell.NameSpace(sourceFolder);
            Folder destFolder = shell.NameSpace(destinationFile);
            FolderItems items = srcFolder.Items();
            destFolder.CopyHere(items, 20);
        }

        private void InstallApplication(string newFileName)
        {
            m_addText.Invoke("Connecting to emulator...");

            Device emulator = new EmulatorRetriever().GetEmulator();
            emulator.Connect();
            Guid appGUID = UninstallApplication(emulator);
            m_addText.Invoke("(Done)");

            m_addText.Invoke("Deploying application...");

            emulator.InstallApplication(appGUID, appGUID, NormalAppGenre, m_xap.IconPath, newFileName);
            emulator.Disconnect();

            m_addText.Invoke("(Done)");
        }

        private Guid UninstallApplication(Device emulator)
        {
            Guid appGUID = new Guid(m_xap.ProductId);

            if (emulator.IsApplicationInstalled(appGUID))
            {
                m_addText.Invoke("(Done)");
                m_addText.Invoke("Uninstalling previous version...");
                RemoteApplication app = emulator.GetApplication(appGUID);
                app.Uninstall();
            }

            return appGUID;
        }

        public void RunXDEMonitor()
        {
            Process processObj = new Process();
            processObj.StartInfo.FileName = "XDEMonitor.exe";
            processObj.Start();

            m_addText.Invoke("Running XDE Monitor...");

            m_resetButton.Invoke("run");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using Microsoft.SmartDevice.Connectivity;
using Tangerine.BLL.Hooks;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;

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
            string xapFileName = tempFileName + ".xap";

            File.Delete(zipFileName);
            File.Delete(xapFileName);

            CreateZip(zipFileName, GetInstrumentedPath());

            File.Move(zipFileName, xapFileName);
            m_addText.Invoke("(Done)");

            return xapFileName;
        }

        private void CreateEmptyZip(string destinationFile)
        {
            using (FileStream fs = File.Create(destinationFile))
            {
                fs.Write(m_emptyZip, 0, m_emptyZip.Length);
                fs.Flush();
            }
        }

        public void CreateZip(string zipFileName, string folderName)
        {
            using (FileStream stream = File.Create(zipFileName))
            {
                using (ZipOutputStream zipStream = new ZipOutputStream(stream))
                {
                    zipStream.SetLevel(3); //0-9, 9 the highest level of compression
                    zipStream.Password = null;
                    int folderOffset = folderName.Length + (folderName.EndsWith("\\") ? 0 : 1);
                    CompressFolder(folderName, zipStream, folderOffset);
                }
            }
        }

        private void CompressFolder(string path, ZipOutputStream zipStream, int folderOffset)
        {
            string[] files = Directory.GetFiles(path);

            foreach (string filename in files)
            {
                FileInfo fi = new FileInfo(filename);

                string entryName = filename.Substring(folderOffset); // Makes the name in zip based on the folder
                entryName = ZipEntry.CleanName(entryName); // Removes drive from name and fixes slash direction

                ZipEntry newEntry = new ZipEntry(entryName);
                newEntry.DateTime = fi.LastWriteTime;
                newEntry.Size = fi.Length;
                
                zipStream.UseZip64 = UseZip64.Off;
                zipStream.PutNextEntry(newEntry);

                // Zip the file in buffered chunks
                // the "using" will close the stream even if an exception occurs
                byte[] buffer = new byte[4096];
                using (FileStream streamReader = File.OpenRead(filename))
                {
                    StreamUtils.Copy(streamReader, zipStream, buffer);
                }
                zipStream.CloseEntry();
            }

            // traverse through other directories
            string[] folders = Directory.GetDirectories(path);
            foreach (string folder in folders)
            {
                CompressFolder(folder, zipStream, folderOffset);
            }
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

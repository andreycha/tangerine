using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ICSharpCode.SharpZipLib.Zip;

namespace Tangerine.BLL
{
    public sealed class XAP : IManifest
    {
        private const string DRMFileName = "WMAppPRHeader.xml";
        private const string ManifestFileName = "WMAppManifest.xml";

        private readonly List<string> m_excludedAssembliesPatterns = new List<string>()
        {
            "System",
            "Microsoft",
            "Newtonsoft",
            "GalaSoft",
            "Coding4Fun",
            "BugSense",
            "ImageTools",
            "ICSharpCode",
            "SharpZipLib",
            "PhoneCodeContractsAssemblies"
        };
        private readonly string m_XAPFilePath;

        private Dictionary<string, XAPAssembly> m_xapAssemblies = new Dictionary<string, XAPAssembly>();
        private List<string> m_dllFiles;
        private string m_unpackPath;

        public string ProductId { get; private set; }
        
        public string Title { get; private set; }
        
        public string Author { get; private set; }
        
        public string Version { get; private set; }

        public string PlatformVersion { get; private set; }

        public IEnumerable<Capability> Capabilities { get; private set; }

        public IEnumerable<XAPAssembly> Assemblies 
        {
            get { return m_xapAssemblies.Values; }
        }

        public string IconPath { get; private set; }
        
        public bool IsUnpacked { get; private set; }

        internal delegate void XAPFileUnzippedHandler(object xap, FileInfoEventArgs fileInfo);
        internal event XAPFileUnzippedHandler FileUnzipped;

        internal XAP(string path)
        {
            var dirInfo = new DirectoryInfo(path);
            if ((dirInfo.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
            {
                if (!dirInfo.Exists)
                {
                    string err = String.Format("Target application '{0}' does not exist.", path);
                    throw new FileNotFoundException(err);
                }
                IsUnpacked = true;
                m_unpackPath = path;
            }
            else
            {
                if (!File.Exists(path))
                {
                    string err = String.Format("Target application '{0}' does not exist.", path);
                    throw new FileNotFoundException(err);
                }
                IsUnpacked = false;
            }

            m_XAPFilePath = path;
            m_dllFiles = new List<string>();
            IconPath = "";
        }

        internal void OnXAPFileUnzipped(object xap, FileInfoEventArgs fileInfo)
        {
            if (FileUnzipped != null)
            {
                FileUnzipped(this, fileInfo);
            }
        }

        internal void Parse()
        {
            m_xapAssemblies = new Dictionary<string, XAPAssembly>();
            m_dllFiles = new List<string>();

            if (!IsUnpacked)
            {
                m_unpackPath = UnZip(m_XAPFilePath);
            }

            string[] asmFiles = Directory.GetFiles(m_unpackPath, "*.dll");
            foreach (var asmFile in asmFiles)
            {
                string asmFileName = Path.GetFileName(asmFile);
                if (!ExcludeAssembly(asmFileName))
                {
                    XAPAssembly xapAssembly = new XAPAssembly(asmFile);
                    m_xapAssemblies.Add(asmFile, xapAssembly);
                }
                m_dllFiles.Add(asmFile);
            }

            ReadManifest();
        }

        private bool ExcludeAssembly(string asmName)
        {
            return m_excludedAssembliesPatterns.Any(p => asmName.StartsWith(p));
        }

        private void ReadManifest()
        {
            var filePath = Path.Combine(m_unpackPath, ManifestFileName);
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Manifest file was not found. Check whether you specified valid application.");
            }

            ManifestReader reader = new ManifestReader(filePath);
            ProductId = reader.GetProductId();
            Title = reader.GetTitle();
            Version = reader.GetVersion();
            Author = reader.GetAuthor();
            Capabilities = reader.GetCapabilities();
            PlatformVersion = reader.GetAppPlatformVersion();
        }

        private string UnZip(string filePath)
        {
            string tmpPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tmpPath);
            try
            {
                using (ZipInputStream zipStream = new ZipInputStream(File.OpenRead(filePath)))
                {
                    ZipEntry zipEntry;
                    while ((zipEntry = zipStream.GetNextEntry()) != null)
                    {
                        if (zipEntry.IsFile)
                        {
                            if (zipEntry.Name != "")
                            {
                                if (zipEntry.Name.Contains("\\") || zipEntry.Name.Contains("/"))
                                {
                                    string tmpFilename = zipEntry.Name.Replace("/", "\\");
                                    int pos = tmpFilename.LastIndexOf("\\");
                                    string str = tmpFilename.Substring(0, pos);
                                    string cdir = "";
                                    string[] dirs = str.Split(new char[] { '\\' });
                                    foreach (string dir in dirs)
                                    {
                                        if (!Directory.Exists(tmpPath + "\\" + cdir + "\\" + dir))
                                        {
                                            Directory.CreateDirectory(tmpPath + "\\" + cdir + "\\" + dir);
                                        }
                                        cdir = cdir + "\\" + dir;
                                    }
                                }

                                string strNewFile = @"" + tmpPath + @"\" + zipEntry.Name;
                                WriteEntryToFile(zipStream, strNewFile);
                            }
                        }
                        else if (zipEntry.IsDirectory)
                        {
                            string strNewDir = @"" + tmpPath + @"\" + zipEntry.Name;
                            if (!Directory.Exists(strNewDir))
                            {
                                Directory.CreateDirectory(strNewDir);
                            }
                        }

                    }
                    zipStream.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception occured while decompressing the source XAP file: " + ex.Message);               
            }

            IsUnpacked = true;
            m_unpackPath = tmpPath;

            return tmpPath;
        }

        private void WriteEntryToFile(ZipInputStream zipStream, string filename)
        {
            using (FileStream streamWriter = File.Create(filename))
            {
                FileInfoEventArgs fileInfo = new FileInfoEventArgs(filename);
                OnXAPFileUnzipped(this, fileInfo);

                byte[] buffer = new byte[2048];
                while (true)
                {
                    int nSize = zipStream.Read(buffer, 0, buffer.Length);
                    if (nSize > 0)
                    {
                        streamWriter.Write(buffer, 0, nSize);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        internal bool RemoveDRM()
        {
            bool removed = false;

            if (IsUnpacked)
            {
                string strDRMFilename = Path.Combine(m_unpackPath, DRMFileName);
                if (File.Exists(strDRMFilename))
                {
                    File.Delete(strDRMFilename);
                    removed = true;
                }
            }

            return removed;
        }

        internal void ReplaceSignatures()
        {
            foreach (string filepath in m_dllFiles)
            {
                SignFile(filepath);
            }
        }

        private void SignFile(string filepath)
        {
            Process processObj = new Process();
            processObj.StartInfo.FileName = "signcode.exe";
            processObj.StartInfo.Arguments = "-spc pubkeycert.cer -v privkey.pvk -a md5 " + filepath;
            processObj.StartInfo.UseShellExecute = false;
            processObj.StartInfo.CreateNoWindow = true;
            processObj.StartInfo.RedirectStandardOutput = true;
            processObj.Start();
            processObj.WaitForExit();
            // TODO: throw exception if signing removed smth
            /*string strCmdResult = */processObj.StandardOutput.ReadToEnd();
        }

        internal string GetPath()
        {
            return m_unpackPath;
        }
    }
}

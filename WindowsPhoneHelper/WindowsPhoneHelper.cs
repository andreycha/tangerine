using System;
using System.IO;
using System.IO.IsolatedStorage;

namespace WindowsPhoneHelper
{
    public static class WindowsPhoneHelper
    {
        public static void LogToFile(string text, string filename)
        {
            using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            using (var isoStream = isoStore.OpenFile(filename, FileMode.Append, FileAccess.Write))
            using (var sw = new StreamWriter(isoStream))
            {
                sw.Write(text);
                sw.Write(Environment.NewLine);
            }
        }
    }
}

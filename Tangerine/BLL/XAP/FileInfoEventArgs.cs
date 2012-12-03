using System;

namespace Tangerine.BLL
{
    internal sealed class FileInfoEventArgs : EventArgs
    {
        public string Path { get; private set; }

        public FileInfoEventArgs(string path)
        {
            Path = path;
        }
    }
}

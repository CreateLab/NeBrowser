using System;
using System.IO;

namespace NeBrowser.Helpers
{
    public class PathConstant
    {
        public static readonly string FolderPath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "NeBrowser");
        public static readonly string StatePath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "NeBrowser/state.txt");
    }
}
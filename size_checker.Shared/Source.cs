using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Windows.UI.Xaml.Controls;

namespace size_checker
{
    public class Source
    {
        public string Name { get; set; }
    }

    public class LocalFolderSource : Source
    {
        public string Path { get; set; }
    }

    public class LocalDriveSource : LocalFolderSource
    {
        public DriveType Type { get; set; }
    }
}

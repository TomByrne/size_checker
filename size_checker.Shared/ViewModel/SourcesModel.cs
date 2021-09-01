using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using Windows.Storage;

namespace size_checker.ViewModel
{
    public class SourcesModel : INotifyPropertyChanged
    {
        static String SourceDataKey = "SourceData";
        static String SourceDataFile = "sources.json";

        public static SourcesModel instance = new SourcesModel();

        public ObservableCollection<Source> Drives { get; }
        public ObservableCollection<Source> Sources { get; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string p = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));

        public SourcesModel()
        {
            Sources = new ObservableCollection<Source>();
            Drives = new ObservableCollection<Source>();
            SearchForDrives();
            LoadSources();
        }

        void SearchForDrives()
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in drives)
            {
                Drives.Add(new LocalDriveSource
                {
                    Name = drive.IsReady ? drive.VolumeLabel : null,
                    Path = drive.Name,
                    Type = drive.DriveType
                });
            }
            RaisePropertyChanged("Drives");
        }

        public void AddLocalFolder(StorageFolder folder)
        {
            Sources.Add(new LocalFolderSource { Name = '/' + folder.DisplayName, Path = folder.Path });
            SaveSources();
            RaisePropertyChanged("Sources");
        }

        async void LoadSources()
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile sourcesFile = await localFolder.GetItemAsync(SourceDataFile) as StorageFile;
            String SourcesStr;
            try
            {
                SourcesStr = await FileIO.ReadTextAsync(sourcesFile);
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to read " + SourceDataFile);
                return;
            }

            try
            {
                List<LocalFolderSource> sources = JsonConvert.DeserializeObject<List<LocalFolderSource>>(SourcesStr);
                if (sources == null) throw new Exception();
                foreach(Source source in sources)
                {
                    Sources.Add(source);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to parse " + SourceDataFile);
                return;
            }

            RaisePropertyChanged("Sources");
        }

        async void SaveSources()
        {
            String SourcesStr = JsonConvert.SerializeObject(Sources, Formatting.Indented);

            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile sourcesFile = await localFolder.CreateFileAsync(SourceDataFile, CreationCollisionOption.ReplaceExisting);
            try
            {
                await FileIO.WriteTextAsync(sourcesFile, SourcesStr);
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to write " + SourceDataFile);
            }
        }
    }
}

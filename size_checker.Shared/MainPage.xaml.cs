using System;
using Windows.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using Windows.Storage;
using Windows.Storage.Pickers;
using System.IO;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace size_checker
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        public ObservableCollection<Source> Sources { get; set; }
        public ObservableCollection<NavigationViewItemBase> NavItems { get; set; }

        ApplicationDataCompositeValue SourcesData;

        String SourceDataKey = "SourceData";

        public MainPage()
        {
            this.InitializeComponent();
            NavItems = new ObservableCollection<NavigationViewItemBase>();
            this.SearchForSources();
        }

        void SearchForSources()
        {

            Sources = new ObservableCollection<Source>();

            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in drives)
            {
                if (!drive.IsReady) continue;

                String Name = drive.Name + " " + (drive.IsReady ? drive.VolumeLabel : "<Unavailable>");
                Symbol Glyph;
                if (!drive.IsReady)
                {
                    Glyph = Symbol.DisconnectDrive;
                }
                else
                {
                    switch (drive.DriveType)
                    {
                        case DriveType.Network:
                            Glyph = Symbol.World;
                            break;

                        default:
                            Glyph = Symbol.MapDrive;
                            break;
                    }
                }
                Sources.Add(new Source { Name = Name, Glyph = Glyph, Tooltip = Name, IsEnabled = drive.IsReady });
            }

            GenerateNavItems();
        }

        async void OnAddButtonClick(object sender, ItemClickEventArgs e)
        {
            var folderPicker = new FolderPicker();
            folderPicker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
            folderPicker.FileTypeFilter.Add("*");
            StorageFolder pickedFolder = await folderPicker.PickSingleFolderAsync();
            if (pickedFolder != null)
            {
                Console.WriteLine(pickedFolder.DisplayName);
                Sources.Add(new Source { Name = pickedFolder.Path, Glyph = Symbol.Folder, Tooltip = pickedFolder.Path, IsEnabled = true });
                GenerateNavItems();
            }
            else
            {
                Console.WriteLine("Add Source aborted");
            }

        }

        void GenerateNavItems()
        {
            NavItems.Clear();

            NavItems.Add(new NavigationViewItemHeader { Content = "Sources"});

            foreach (Source Source in Sources)
            {
                NavItems.Add(new NavigationViewItem
                {
                    Content = Source.Name,
                    Icon = new SymbolIcon(Source.Glyph),
                    IsEnabled = Source.IsEnabled,
                });
            }

        }
    }

}

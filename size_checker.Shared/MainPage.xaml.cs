using System;
using Windows.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using Windows.Storage;
using Windows.Storage.Pickers;
using System.IO;
using Newtonsoft.Json;
using Windows.UI.Xaml;
using size_checker.ViewModel;
using System.ComponentModel;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace size_checker
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public SourcesModel SourcesModel = SourcesModel.instance;

        public ObservableCollection<NavigationViewItemBase> NavItems { get; set; }

        public MainPage()
        {
            this.InitializeComponent();
            NavItems = new ObservableCollection<NavigationViewItemBase>();

            SourcesModel.PropertyChanged += new PropertyChangedEventHandler((object sender, PropertyChangedEventArgs e) => GenerateNavItems());
            GenerateNavItems();
        }

        async void OnAddButtonClick(object sender, RoutedEventArgs e)
        {
            var folderPicker = new FolderPicker();
            folderPicker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
            folderPicker.FileTypeFilter.Add("*");
            StorageFolder pickedFolder = await folderPicker.PickSingleFolderAsync();
            if (pickedFolder != null)
            {
                Console.WriteLine(pickedFolder.DisplayName);
                SourcesModel.AddLocalFolder(pickedFolder);
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

            if (SourcesModel.Drives.Count > 0)
            {
                NavItems.Add(new NavigationViewItemHeader { Content = "Drives" });

                foreach (Source Source in SourcesModel.Drives)
                {
                    NavItems.Add(SourceToNav(Source));
                }
            }

            if (SourcesModel.Sources.Count > 0)
            {
                NavItems.Add(new NavigationViewItemHeader { Content = "Folders" });

                foreach (Source Source in SourcesModel.Sources)
                {
                    NavItems.Add(SourceToNav(Source));
                }
            }

        }

        static NavigationViewItemBase SourceToNav(Source Source)
        {
            String Name = Source.Name;
            String Info = null;
            Symbol IconGlyph;
            bool IsEnabled = true;

            switch (Source)
            {
                case LocalDriveSource drive:
                    Info = drive.Path;
                    switch (drive.Type)
                    {
                        case DriveType.Network:
                            IconGlyph = Symbol.World;
                            break;

                        default:
                            IconGlyph = Symbol.MapDrive;
                            break;
                    }
                    break;

                case LocalFolderSource folder:
                    IconGlyph = Symbol.Folder;
                    Info = folder.Path;
                    break;

                default:
                    throw new Exception("Unknown Source Type");
            }

            if(Name == null && Info != null)
            {
                Name = Info;
                Info = null;
            }

            StackPanel panel = new StackPanel();

            panel.Children.Add(new TextBlock
            {
                Text = Name
            });

            if(Info != null)
            {
                panel.Children.Add(new TextBlock
                {
                    Text = Info,
                    FontSize = 10,
                    TextWrapping = TextWrapping.Wrap
                });
            }

            return new NavigationViewItem
            {
                Content = panel,
                Icon = new SymbolIcon(IconGlyph),
                IsEnabled = IsEnabled
            };
        }
    }

}

using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using Newtonsoft.Json;
using SWPMapParser.Destination;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Utility.MVVM;

namespace SWPMapParser
{
    internal class MainWindowViewModel : ObservableObject
    {
        private readonly IDialogCoordinator _instance;

        private List<Map> _maps = new(); //Last parsed maps

        private bool _downloadButtonEnabled;

        public bool DownloadButtonEnabled
        {
            get => _downloadButtonEnabled;
            set => SetField(ref _downloadButtonEnabled, value);
        }

        public MainWindowViewModel(IDialogCoordinator instance)
        {
            _instance = instance;
            DownloadButtonEnabled = false;
        }

        public ICommand UploadMapCommand => new DelegateCommand(UploadMap);

        public ICommand DownloadMapCommand => new DelegateCommand(DownloadMaps);


        private async void UploadMap()
        {
            var dialog = new OpenFileDialog();

            dialog.Filter = "Maps (*.json)|*.json";

            if(dialog.ShowDialog() == true)
            {
                var progress = await _instance.ShowProgressAsync(this, "Create Map", "The map is currently under process");

                var mapName = await _instance.ShowInputAsync(this, "Map Name", "Input the maps name");

                _maps = new List<Map> { MapParser.Parse(mapName, dialog.FileName) };

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    DownloadButtonEnabled = true;
                });

                await progress.CloseAsync();
            }
        }

        public async void UploadMap(List<string> maps)
        {
            var progress = await _instance.ShowProgressAsync(this, "Create Map", "The map is currently under process");

            _maps.Clear();

            foreach(var map in maps)
            {
                var mapName = await _instance.ShowInputAsync(this, "Map Name", $"Input the maps name for file '{map}'",
                    new MetroDialogSettings { DefaultText = map.Split("\\").Last().Split(".").First()});

                _maps.Add(MapParser.Parse(mapName, map));
            }

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                DownloadButtonEnabled = true;
            });

            await progress.CloseAsync();
        }

        private void DownloadMaps()
        {
            if (Directory.Exists("maps"))
                Directory.Delete("maps", true);

            Directory.CreateDirectory("maps");

            var dialog = new SaveFileDialog();
            dialog.Filter = "Maps (*.zip)|*.zip";
            dialog.FileName = "maps.zip";

            if(dialog.ShowDialog() == true)
            {
                foreach(var map in _maps)
                {
                    File.WriteAllText($"maps/{map.Name}.json", JsonConvert.SerializeObject(map));
                }

                ZipFile.CreateFromDirectory("maps", dialog.FileName);

                _instance.ShowMessageAsync(this, "Saved!", "Maps were saved in " + dialog.FileName);
            }
        }
    }
}

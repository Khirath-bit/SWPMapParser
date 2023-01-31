using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using Newtonsoft.Json;
using SWPMapParser.Destination;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Utility.MVVM;

namespace SWPMapParser
{
    internal class MainWindowViewModel : ObservableObject
    {
        private readonly IDialogCoordinator _instance;

        private Map _map; //Last parsed map

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

        public ICommand DownloadMapCommand => new DelegateCommand(DownloadMap);


        private async void UploadMap()
        {
            var dialog = new OpenFileDialog();

            dialog.Filter = "Maps (*.json)|*.json";

            if(dialog.ShowDialog() == true)
            {
                var progress = await _instance.ShowProgressAsync(this, "Create Map", "The map is currently under process");

                var mapName = await _instance.ShowInputAsync(this, "Map Name", "Input the maps name");

                _map = MapParser.Parse(mapName, dialog.FileName);

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    DownloadButtonEnabled = true;
                });

                await progress.CloseAsync();
            }
        }

        private void DownloadMap()
        {
            var dialog = new SaveFileDialog();
            dialog.Filter = "Maps (*.json)|*.json";
            dialog.FileName = _map.Name + ".json";

            if(dialog.ShowDialog() == true)
            {
                File.WriteAllText(dialog.FileName, JsonConvert.SerializeObject(_map));

                _instance.ShowMessageAsync(this, "Saved!", "Map was saved in " + dialog.FileName);
            }
        }
    }
}

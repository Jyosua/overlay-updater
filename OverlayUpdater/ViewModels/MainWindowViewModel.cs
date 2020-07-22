using OverlayUpdater.Models;
using OverlayUpdater.Services;
using ReactiveUI;
using System;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;

namespace OverlayUpdater.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private WebHostService webService = new WebHostService();
        private JSONFileService fileService = new JSONFileService();
        private ProgressBarJSON json = new ProgressBarJSON();

        public MainWindowViewModel()
        {
            SelectFolderClickCommand = ReactiveCommand.CreateFromTask(SelectFolderCommand);
            StartServerCommand = ReactiveCommand.CreateFromTask(StartCommand);
            StopServerCommand = ReactiveCommand.Create(StopCommand);
        }

        public ReactiveCommand<Unit,Unit> SelectFolderClickCommand { get; }
        public ReactiveCommand<Unit, Unit> StartServerCommand { get; }
        public ReactiveCommand<Unit, Unit> StopServerCommand { get; }
        public Func<Task<string>> OverlayFolderPickHandler;

        private bool enableStartServerButton;
        public bool EnableStartServerButton
        {
            get => enableStartServerButton;
            set => this.RaiseAndSetIfChanged(ref enableStartServerButton, value);
        }

        private bool enableStopServerButton;
        public bool EnableStopServerButton
        {
            get => enableStopServerButton;
            set => this.RaiseAndSetIfChanged(ref enableStopServerButton, value);
        }

        private string folderPath;
        public string FolderPath
        {
            get => folderPath;
            set => this.RaiseAndSetIfChanged(ref folderPath, value);
        }

        private string title;
        public string Title
        {
            get => title;
            set
            {
                this.RaiseAndSetIfChanged(ref title, value);
                if (JSONNeedsSync())
                    SyncAndWriteJSON();
            }
        }

        private string subtitle;
        public string Subtitle
        {
            get => subtitle;
            set
            {
                this.RaiseAndSetIfChanged(ref subtitle, value);
                if (JSONNeedsSync())
                    SyncAndWriteJSON();
            }
        }

        private uint max;
        public uint Max
        {
            get => max;
            set
            {
                this.RaiseAndSetIfChanged(ref max, value);
                if (JSONNeedsSync())
                    SyncAndWriteJSON();
            }
        }

        private uint current;
        public uint Current
        {
            get => current;
            set
            {
                this.RaiseAndSetIfChanged(ref current, value);
                if (JSONNeedsSync())
                    SyncAndWriteJSON();
            }
        }

        public async Task SelectFolderCommand()
        {
            await SetOverlayFolder();
            var model = await ReadJSONFile();
            if (model == null)
                return;
            UpdateValues(model);
            UpdateServerButtons();
        }

        public async Task StartCommand()
        {
            var task = webService.Start(folderPath);
            Thread.Sleep(5);
            UpdateServerButtons();
            return;
        }

        public void StopCommand()
        {
            webService.Shutdown();
            UpdateServerButtons();
        }

        public async Task SetOverlayFolder()
        {
            if (OverlayFolderPickHandler == null)
                return;

            var folderPath = await OverlayFolderPickHandler();
            FolderPath = folderPath;
        }

        public async Task<ProgressBarJSON> ReadJSONFile()
        {
            if (string.IsNullOrEmpty(FolderPath))
                return null;

            return await fileService.ReadFile(FolderPath);
        }

        public void UpdateServerButtons()
        {
            EnableStartServerButton = !string.IsNullOrEmpty(FolderPath) && !webService.ServerRunning;
            EnableStopServerButton = !string.IsNullOrEmpty(FolderPath) && webService.ServerRunning;
        }

        public void UpdateValues(ProgressBarJSON json)
        {
            SyncJSON(json);
            Title = json.Title;
            Subtitle = json.Subtitle;
            Max = json.Max;
            Current = json.Current;
        }

        public bool JSONNeedsSync()
        {
            return json.Title != title ||
                    json.Subtitle != subtitle ||
                    json.Max != max ||
                    json.Current != current;
        }

        public void SyncJSON(ProgressBarJSON json) => SyncJSON(json.Title, json.Subtitle, json.Max, json.Current);
        public void SyncJSON(string title, string subtitle, uint max, uint current)
        {
            json.Title = title;
            json.Subtitle = subtitle;
            json.Max = max;
            json.Current = current;
        }

        public void SyncAndWriteJSON()
        {
            SyncJSON(title, subtitle, max, current);
            if (string.IsNullOrEmpty(FolderPath))
                return;
            var task = fileService.WriteFile(FolderPath, json);
        }
    }
}

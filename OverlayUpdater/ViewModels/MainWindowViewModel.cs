using Avalonia.Controls;
using OverlayUpdater.Models;
using OverlayUpdater.Services;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Text;
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
            SelectClickCommand = ReactiveCommand.CreateFromTask(ClickCommand);
        }

        public ReactiveCommand<Unit,Unit> SelectClickCommand { get; }
        public Func<Task<string>> JSONFilePickHandler;

        private string jsonPath;
        public string JsonPath
        {
            get => jsonPath;
            set => this.RaiseAndSetIfChanged(ref jsonPath, value);
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

        public async Task ClickCommand()
        {
            await SetJSONFile();
            var model = await ReadJSONFile();
            if (model == null)
                return;
            UpdateValues(model);
            StartServer();
        }

        private void StartServer()
        {
            var task = webService.Start(json);
        }

        public async Task SetJSONFile()
        {
            if (JSONFilePickHandler == null)
                return;

            var filepath = await JSONFilePickHandler();
            JsonPath = filepath;
        }

        public async Task<ProgressBarJSON> ReadJSONFile()
        {
            if (string.IsNullOrEmpty(JsonPath))
                return null;

            return await fileService.ReadFile(JsonPath);
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
            return this.json.Title != title ||
                    this.json.Subtitle != subtitle ||
                    this.json.Max != max ||
                    this.json.Current != current;
        }

        public void SyncJSON(ProgressBarJSON json) => SyncJSON(json.Title, json.Subtitle, json.Max, json.Current);
        public void SyncJSON(string title, string subtitle, uint max, uint current)
        {
            this.json.Title = title;
            this.json.Subtitle = subtitle;
            this.json.Max = max;
            this.json.Current = current;
        }

        public void SyncAndWriteJSON()
        {
            SyncJSON(title, subtitle, max, current);
            if (string.IsNullOrEmpty(JsonPath))
                return;
            var task = fileService.WriteFile(JsonPath, json);
        }
    }
}

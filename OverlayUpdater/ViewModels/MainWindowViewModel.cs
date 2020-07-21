using Avalonia.Controls;
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
        public MainWindowViewModel()
        {
            SelectClickCommand = ReactiveCommand.CreateFromTask(SetJSONFile);
        }

        public ReactiveCommand<Unit,Unit> SelectClickCommand { get; }
        public Func<Task<string>> JSONFilePickHandler;
        private string jsonPath;

        public string JsonPath
        {
            get => jsonPath;
            set => this.RaiseAndSetIfChanged(ref jsonPath, value);
        }

        public string Greeting => "Welcome to Avalonia!";

        public async Task SetJSONFile()
        {
            if (JSONFilePickHandler == null)
                return;

            var filepath = await JSONFilePickHandler();
            JsonPath = filepath;
        }
    }
}

using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using OverlayUpdater.ViewModels;
using System;
using System.Threading.Tasks;

namespace OverlayUpdater.Views
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            DataContextChanged += DataContextChangeHandler;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void DataContextChangeHandler(object sender, EventArgs wat)
        {
            var vm = this.DataContext as MainWindowViewModel;
            if (vm != null)
                vm.JSONFilePickHandler = PickJSONFile;
        }

        private async Task<string> PickJSONFile()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.AllowMultiple = false;
            dialog.Filters.Add(new FileDialogFilter() { Name = "JSON", Extensions = { "json" } });

            string[] result = await dialog.ShowAsync(this);

            if (result != null)
            {
                return result[0];
            }

            return null;
        }
    }
}

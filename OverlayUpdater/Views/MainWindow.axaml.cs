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
            var vm = DataContext as MainWindowViewModel;
            if (vm != null)
                vm.OverlayFolderPickHandler = OpenFolderHandler;
        }

        private async Task<string> OpenFolderHandler()
        {
            var dialog = new OpenFolderDialog();
            return await dialog.ShowAsync(this);
        }
    }
}

using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using AvaloniaDesktopApp.ViewModels;

namespace AvaloniaDesktopApp.Controls;

public partial class MediaMenuControl : UserControl
{
    private MediaMenuViewModel _viewModel;
    public MediaMenuControl()
    {
        InitializeComponent();
        _viewModel = new MediaMenuViewModel();
        DataContext = _viewModel;
        Loaded += MainWindow_Loaded;
    }
    
    private async void MainWindow_Loaded(object? sender, RoutedEventArgs e)
    {
        await _viewModel.InitializeDataAsync();
    }
}
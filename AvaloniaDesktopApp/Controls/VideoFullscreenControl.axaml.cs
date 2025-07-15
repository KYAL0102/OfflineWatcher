using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using AvaloniaDesktopApp.ViewModels;
using Core.Entities;

namespace AvaloniaDesktopApp.Controls;

public partial class VideoFullscreenControl : UserControl
{
    public VideoFullscreenViewModel _viewModel;
    public VideoFullscreenControl(Video? video)
    {
        InitializeComponent();
        _viewModel = new VideoFullscreenViewModel(video);
        DataContext = _viewModel;
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object? sender, RoutedEventArgs e)
    {
        await _viewModel.InitializeDataAsync();
    }
}
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using AvaloniaDesktopApp.ViewModels;
using Core.Entities;

namespace AvaloniaDesktopApp.Controls;

public partial class VideoFullscreenControl : UserControl
{
    private VideoFullscreenViewModel? _viewModel;

    public VideoFullscreenControl()
        : this(null)
    {
    }

    public VideoFullscreenControl(Video? video)
    {
        InitializeComponent();
        InitializeViewModel(video);
    }

    private void InitializeViewModel(Video? video)
    {
        _viewModel = new VideoFullscreenViewModel(video);
        DataContext = _viewModel;
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object? sender, RoutedEventArgs e)
    {
        if (_viewModel is not null)
        {
            await _viewModel.InitializeDataAsync();
        }
    }
}
using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using AvaloniaDesktopApp.ViewModels;

namespace AvaloniaDesktopApp.Controls;

public partial class MovieContentControl : UserControl
{
    private MovieContentViewModel _viewModel;
    public MovieContentControl()
    {
        InitializeComponent();
        _viewModel = new MovieContentViewModel();
        DataContext = _viewModel;
        Loaded += MainWindow_Loaded;
    }

    private async void MainWindow_Loaded(object? sender, RoutedEventArgs e)
    {
        await _viewModel.InitializeDataAsync();
    }

    private async void ScrollViewer_ScrollChanged(object? sender, ScrollChangedEventArgs e)
    {
        var scrollViewer = (ScrollViewer?)sender;
        if (scrollViewer != null &&
            scrollViewer.Offset.Y + scrollViewer.Viewport.Height >= scrollViewer.Extent.Height - 100)
        {
            await _viewModel.LoadNextBatchAsync();
        }
    }
}
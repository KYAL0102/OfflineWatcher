using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using AvaloniaDesktopApp.ViewModels;

namespace AvaloniaDesktopApp.Controls;

public partial class SearchControl : UserControl
{
    private SearchViewModel _viewModel;

    public SearchControl()
    {
        InitializeComponent();
        _viewModel = new SearchViewModel();
        DataContext = _viewModel;
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object? sender, RoutedEventArgs e)
    {
        await _viewModel.InitializeDataAsync();
    }
}
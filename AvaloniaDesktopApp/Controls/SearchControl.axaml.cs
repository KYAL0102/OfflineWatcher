using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using AvaloniaDesktopApp.ViewModels;

namespace AvaloniaDesktopApp.Controls;

public partial class SearchControl : UserControl
{
    private SearchViewModel? _viewModel;

    public SearchControl()
    {
        InitializeComponent();
        InitializeViewModel();
    }

    private void InitializeViewModel()
    {
        _viewModel = new SearchViewModel();
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
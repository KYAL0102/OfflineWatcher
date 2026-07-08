using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using AvaloniaDesktopApp.ViewModels;
using Core.Entities;

namespace AvaloniaDesktopApp.Controls;

public partial class MoviePageControl : UserControl
{
    private MoviePageViewModel? _viewModel;

    public MoviePageControl()
        : this(null)
    {
    }

    public MoviePageControl(Movie? movie)
    {
        InitializeComponent();
        InitializeViewModel(movie);
    }

    private void InitializeViewModel(Movie? movie)
    {
        _viewModel = new MoviePageViewModel(movie);
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
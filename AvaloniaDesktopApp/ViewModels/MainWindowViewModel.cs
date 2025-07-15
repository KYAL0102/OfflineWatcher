using System.Threading.Tasks;
using Avalonia.Controls;
using AvaloniaDesktopApp.Controls;
using Core;
using Core.Entities;

namespace AvaloniaDesktopApp.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly Window? _window;
    private WindowState? _oldWindowState = null;
    
    private UserControl? _currentPage = null;
    public UserControl? CurrentPage
    {
        get { return _currentPage; }
        set
        {
            _currentPage = value;
            OnPropertyChanged();
        }
    }

    public MainWindowViewModel(Window? window = null)
    {
        _window = window;
        CurrentPage = new MediaMenuControl();
        
        Messenger.Subscribe(Globals.NavigateToMenuAction, _ => NavigateToMenu());
        Messenger.Subscribe(Globals.NavigateToMoviePageAction, message => NavigateToMoviePage(message.Data));
        Messenger.Subscribe(Globals.NavigateToSeriesPageAction, message => NavigateToSeriesPage(message.Data));
        Messenger.Subscribe(Globals.NavigateToVideoFullscreenAction, message => NavigateToVideoFullScreen(message.Data));
    }

    public async Task InitializeDataAsync()
    {
        await Task.CompletedTask;
    }
    
    private void NavigateToMenu()
    {
        if (_window != null && _oldWindowState != null)
        {
            _window.CanResize = true;
            _window.WindowState = (WindowState)_oldWindowState;
        }
        CurrentPage = new MediaMenuControl();
    }

    private void NavigateToMoviePage(object? item)
    {
        if (_window != null && _oldWindowState != null)
        {
            _window.CanResize = true;
            _window.WindowState = (WindowState)_oldWindowState;
        }
        if (item is Movie movie) CurrentPage = new MoviePageControl(movie);
    }

    private void NavigateToVideoFullScreen(object? item)
    {
        if (_window != null)
        {
            _window.CanResize = false;
            _oldWindowState = _window.WindowState;
            _window.WindowState = WindowState.Normal;
            _window.WindowState = WindowState.Maximized;
        }
        if (item is Video video) CurrentPage = new VideoFullscreenControl(video);
    }

    private void NavigateToSeriesPage(object? item)
    {
        if (_window != null && _oldWindowState != null)
        {
            _window.CanResize = true;
            _window.WindowState = (WindowState)_oldWindowState;
        }
        //if (item is Series series)  CurrentPage = new SeriesPage(_controller, series);
    }
}
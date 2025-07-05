using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Core;
using Core.Entities;
using MVVMBase;
using FuzzySharp;
using WpfDesktopApp.Views;

namespace WpfDesktopApp.ViewModels
{
    public class MainViewModel : BaseViewModel, INotifyPropertyChanged
    {
        private readonly WindowController? _controller;
        private readonly Window? _window;
        private WindowStyle? _oldWindowStyle = null;
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
        
        public MainViewModel(WindowController? controller, Window? window) : base(controller)
        {
            _controller = controller;
            _window = window;
            CurrentPage = new MediaMenu(controller);
            
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
            if (_window != null && _oldWindowState != null && _oldWindowStyle != null)
            {
                _window.ResizeMode = ResizeMode.CanResize;
                _window.WindowState = (WindowState)_oldWindowState;
                _window.WindowStyle = (WindowStyle)_oldWindowStyle;
            }
            CurrentPage = new MediaMenu(_controller);
        }

        private void NavigateToMoviePage(object? item)
        {
            if (_window != null && _oldWindowState != null && _oldWindowStyle != null)
            {
                _window.ResizeMode = ResizeMode.CanResize;
                _window.WindowState = (WindowState)_oldWindowState;
                _window.WindowStyle = (WindowStyle)_oldWindowStyle;
            }
            if (item is Movie movie) CurrentPage = new MoviePage(_controller, movie);
        }

        private void NavigateToVideoFullScreen(object? item)
        {
            if (_window != null)
            {
                _oldWindowState = _window.WindowState;
                _oldWindowStyle = _window.WindowStyle;
                _window.ResizeMode = ResizeMode.NoResize;
                _window.WindowStyle = WindowStyle.None;
                _window.WindowState = WindowState.Normal;
                _window.WindowState = WindowState.Maximized;
            }
            if (item is Video video) CurrentPage = new VideoFullscreen(_controller, video);
        }

        private void NavigateToSeriesPage(object? item)
        {
            
        }
        
    }
}

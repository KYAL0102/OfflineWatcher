using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AvaloniaDesktopApp.Tools;
using CommunityToolkit.Mvvm.Input;
using Core;
using Core.Entities;
using ReactiveUI;

namespace AvaloniaDesktopApp.ViewModels;

public class MoviePageViewModel : ViewModelBase
{
    private Movie? _movie;
    private Movie? Movie
    {
        get => _movie;
        set
        {
            _movie = value;

            MovieName = _movie?.NameInCurrentLanguage;
            MovieFilePath = _movie?.VideoOfMovie.PathToVideoFile;
        }
    }

    private string? _movieName = string.Empty;
    public string? MovieName
    {
        get => _movieName;
        set
        {
            if (value == null) _movieName = string.Empty;
            
            _movieName = value;
            OnPropertyChanged();
        }
    }
    
    private string? _movieFilePath = string.Empty;

    public string? MovieFilePath
    {
        get => _movieFilePath;
        set
        {
            if (value == null) _movieFilePath = string.Empty;
            
            _movieFilePath = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<Video> Extras { get; set; } = [];

    public RelayCommand BackToMenuCommand { get; set; }
    public RelayCommand PlayMovieWithVlcCommand { get; set; }
    public RelayCommand PlayMovieCommand { get; set; }
    public ReactiveCommand<object?, Unit> PlayVideoCommand { get; set; }
    public ReactiveCommand<object?, Unit> PlayVideoWithVlcCommand { get; set; }
    public MoviePageViewModel(Movie? movie)
    {
        Movie = movie;
        Movie?.Extras.ForEach(e => Extras.Add(e));
        BackToMenuCommand = new RelayCommand(
            BackToMenu,
            () => true);
        PlayMovieWithVlcCommand = new RelayCommand(
            PlayMovieWithVlc,
            () => Movie != null);
        PlayMovieCommand = new RelayCommand(
            PlayMovie,
            () => Movie != null);
        PlayVideoCommand = ReactiveCommand.Create<object?>(
            PlayVideo,
            this.WhenAnyValue(x => x.Movie).Select(m => m != null));
        PlayVideoWithVlcCommand = ReactiveCommand.Create<object?>(
            PlayVideoWithVlc,
            this.WhenAnyValue(x => x.Movie).Select(m => m != null));
    }
    
    public async Task InitializeDataAsync()
    {
        await Task.CompletedTask;
    }
    
    private void PlayMovie()
    {
        if (Movie != null) PlayVideo(Movie.VideoOfMovie);
    }

    private void PlayMovieWithVlc()
    {
        if (MovieFilePath != null) PlayVideoWithVlc(MovieFilePath);
    }

    private void PlayVideo(object? item)
    {
        Globals.CurrentStreamItem = Movie;
        if (item is Video video)
        {
            Messenger.Publish(new Message
            {
                Action = Globals.NavigateToVideoFullscreenAction,
                Data = video
            });
        }
    }

    private void PlayVideoWithVlc(object? filePath)
    {
        if (filePath is not string path) return;

        var vlcPath = VlcFinder.FindVlcInstallation();
        if (vlcPath == null)
        {
            _ = MessageBoxHelper.ShowMessageBox("VLC not found");
            return;
        }

        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = vlcPath,
            Arguments = path
        };

        try
        {
            Process.Start(startInfo);
        }
        catch (Exception ex)
        {
            _ = MessageBoxHelper.ShowMessageBox(ex.Message);
        }
    }
    
    private void BackToMenu()
    {
        Messenger.Publish(new Message
        {
            Action = Globals.NavigateToMenuAction,
        });
    }
}
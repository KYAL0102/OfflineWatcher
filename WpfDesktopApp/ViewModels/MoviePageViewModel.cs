using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Core;
using Core.Entities;
using Microsoft.Win32;
using MVVMBase;

namespace WpfDesktopApp.ViewModels;

public class MoviePageViewModel : BaseViewModel
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
    public RelayCommand PlayVideoCommand { get; set; }
    public RelayCommand PlayVideoWithVlcCommand { get; set; }
    
    public MoviePageViewModel(WindowController? controller, Movie? movie) : base(controller)
    {
        Movie = movie;
        Movie?.Extras.ForEach(e => Extras.Add(e));
        BackToMenuCommand = new RelayCommand(
            _ => BackToMenu(),
            _ => true);
        PlayMovieWithVlcCommand = new RelayCommand(
            _ => PlayMovieWithVlc(),
            _ => Movie != null);
        PlayMovieCommand = new RelayCommand(
            _ => PlayMovie(),
            _ => Movie != null);
        PlayVideoCommand = new RelayCommand(
            PlayVideo,
            _ => Movie != null);
        PlayVideoWithVlcCommand = new RelayCommand(
            PlayVideoWithVlc,
            _ => Movie != null);
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
            MessageBox.Show("VLC not found");
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
            MessageBox.Show(ex.Message);
        }
    }

    public async Task InitializeDataAsync()
    {
        await Task.CompletedTask;
    }

    private void BackToMenu()
    {
        Messenger.Publish(new Message
        {
            Action = Globals.NavigateToMenuAction,
        });
    }
}
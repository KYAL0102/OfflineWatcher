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
using DynamicData;
using ReactiveUI;

namespace AvaloniaDesktopApp.ViewModels;

public class MoviePageViewModel : ViewModelBase
{
    private const string LOREM_IPSUM = "Lorem ipsum dolor sit amet consectetur adipiscing elit." +
                                        "Quisque faucibus ex sapien vitae pellentesque sem placerat. " +
                                        "In id cursus mi pretium tellus duis convallis. " +
                                        "Tempus leo eu aenean sed diam urna tempor. " +
                                        "Pulvinar vivamus fringilla lacus nec metus bibendum egestas. " +
                                        "Iaculis massa nisl malesuada lacinia integer nunc posuere. " +
                                        "Ut hendrerit semper vel class aptent taciti sociosqu. " +
                                        "Ad litora torquent per conubia nostra inceptos himenaeos.";

    private Movie? _movie;
    private Movie? Movie
    {
        get => _movie;
        set
        {
            _movie = value;

            MovieName = _movie?.NameInCurrentLanguage;
            MovieFilePath = _movie?.VideoOfMovie.PathToVideoFile;
            MovieReleaseYear = _movie?.YearOfRelease;
            MovieDuration = $"{_movie?.VideoOfMovie.DurationInMinutes}";
            MovieIMDbRating = _movie?.IMDbRating;
            MovieIMDbReviewAmount = _movie?.IMDbReviewAmout;
            MovieDescription = _movie?.DescriptionInCurrentLanguage;//TODO
            MovieGenres.Clear();
            if (_movie != null) MovieGenres.AddRange(_movie.Genres);
        }
    }

    private string? _movieName = string.Empty;
    public string? MovieName
    {
        get => _movieName;
        set
        {
            if (value == null) value = string.Empty;
            
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
            if (value == null) value = string.Empty;

            _movieFilePath = value;
            OnPropertyChanged();
        }
    }

    private int? _movieReleaseYear = -1;
    public int? MovieReleaseYear
    {
        get => _movieReleaseYear;
        set
        {
            if (value == null) return;

            _movieReleaseYear = value;
            OnPropertyChanged();
        }
    }

    private string _movieDurationInMinutes = "0";
    public string MovieDuration
    {
        get => $"{_movieDurationInMinutes} min";
        set
        {
            if (value == null) value = "0";
            _movieDurationInMinutes = value;
            OnPropertyChanged();
        }
    }

    private double? _movieIMDbRating = -1f;
    public double? MovieIMDbRating
    {
        get => _movieIMDbRating;
        set
        {
            if (value == null) value = -1;

            _movieIMDbRating = value;
            OnPropertyChanged();
        }
    }

    private int? _movieIMDbReviewAmount = -1;
    public int? MovieIMDbReviewAmount
    {
        get => _movieIMDbReviewAmount;
        set
        {
            if (value == null) value = -1;

            _movieIMDbReviewAmount = value;
            OnPropertyChanged();
        }
    }

    private string? _movieDescription;
    public string? MovieDescription
    {
        get => _movieDescription;
        set
        {
            if (value == null) value = LOREM_IPSUM;
            _movieDescription = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<Genre> MovieGenres { get; } = [];

    public ObservableCollection<Video> Extras { get; } = [];

    public RelayCommand BackToMenuCommand { get; set; }
    public RelayCommand PlayMovieWithVlcCommand { get; set; }
    public RelayCommand PlayMovieCommand { get; set; }
    public ReactiveCommand<object?, Unit> PlayVideoCommand { get; set; }
    public ReactiveCommand<object?, Unit> PlayVideoWithVlcCommand { get; set; }
    private bool _clickedPlay { get; set; }
    public MoviePageViewModel(Movie? movie)
    {
        _clickedPlay = false;
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
        if (Movie != null && !_clickedPlay) PlayVideo(Movie.VideoOfMovie);
    }

    private void PlayMovieWithVlc()
    {
        if (MovieFilePath != null && !_clickedPlay) PlayVideoWithVlc(MovieFilePath);
    }

    private void PlayVideo(object? item)
    {
        _clickedPlay = true;
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
        _clickedPlay = true;

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
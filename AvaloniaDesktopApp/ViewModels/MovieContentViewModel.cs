using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Core;
using Core.Entities;
using DynamicData;

namespace AvaloniaDesktopApp.ViewModels;

public class MovieContentViewModel : ViewModelBase
{
    public Movie? SelectedMovie
    {
        get => null;
        set => OnMovieSelected(value);
    }
    private readonly List<Movie> _movies = Globals.Movies;
    private readonly List<MovieRow> _allMovieRows = new();
    private int _currentBatchIndex = 0;
    private const int BatchSize = 5; // Load 5 rows at a time
    private SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1,1);

    public record MovieRow
    {
        public int Priority { get; set; } = -1;
        public string Header { get; set; } = string.Empty;
        public List<Movie> DisplayedMovies { get; set; } = [];
    }
    public ObservableCollection<MovieRow> MovieRows { get; set; } = [];

    public MovieContentViewModel() { }

    public async Task InitializeDataAsync()
    {
        if (_movies.Count == 0)
        {
            //IsProgressbarVisible = true;
            var movies = await ImportController.GetAllMoviesAsync();
            _movies.AddRange(movies);
            //IsProgressbarVisible = false;
        }

        await LoadInitialBatchAsync();
    }

    private async Task LoadInitialBatchAsync()
    {
        await OrganizeMoviesIntoGenresAsync();
        await LoadNextBatchAsync();
    }

    private async Task OrganizeMoviesIntoGenresAsync()
    {
        var genres = GenreManager.Instance.GetMenuGenres();
        var movies = _movies.ToList();
        var bag = new ConcurrentBag<MovieRow>();
        var tasks = new List<Task>();

        foreach (var g in genres)
        {
            var task = Task.Run(() =>
            {
                var displayedMovies = movies.Where(m => m.Genres.Contains(g.Key)).ToList();
                HelperMethods.Shuffle(displayedMovies);

                var mr = new MovieRow
                {
                    Priority = g.Value,
                    Header = g.Key.GenreName,
                    DisplayedMovies = displayedMovies
                };
                bag.Add(mr);
            });
            tasks.Add(task);
        }
        await Task.WhenAll(tasks);

        _allMovieRows.Clear();
        _allMovieRows.AddRange(bag.OrderBy(i => i.Priority));
    }

    public async Task LoadNextBatchAsync()
    {
        await _semaphoreSlim.WaitAsync();

        var startIndex = _currentBatchIndex;
        var endIndex = Math.Min(startIndex + BatchSize, _allMovieRows.Count);

        if (startIndex >= _allMovieRows.Count)
        {
            _semaphoreSlim.Release();
            return;
        }

        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            for (int i = startIndex; i < endIndex; i++)
            {
                Console.WriteLine($"Adding {_allMovieRows[i].Header} ({i})");
                MovieRows.Add(_allMovieRows[i]);
            }
        });

        _currentBatchIndex = endIndex;
        _semaphoreSlim.Release();
    }

    private void OnMovieSelected(Movie? movie)
    {
        if (movie != null) Console.WriteLine($"Movie '{movie.VideoOfMovie.Names[Globals.CurrentLanguage]}' selected");
        Messenger.Publish(new Message
        {
            Action = Globals.NavigateToMoviePageAction,
            Data = movie
        });
    }
    
    private async void Image_Loaded(object sender, RoutedEventArgs e)
    {
        if (sender is Image image && image.DataContext is Movie movie)
        {
            // Load the actual image asynchronously
            var bitmap = await Task.Run(() =>
                Bitmap.DecodeToWidth(new FileStream(movie.ImagePath, FileMode.Open), 150));
            await Dispatcher.UIThread.InvokeAsync(() => image.Source = bitmap);
        }
    }

}

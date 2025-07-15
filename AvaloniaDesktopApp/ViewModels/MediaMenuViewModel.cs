using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Core;
using Core.Entities;

namespace AvaloniaDesktopApp.ViewModels;

public class MediaMenuViewModel : ViewModelBase
{
    public bool IsProgressbarVisible { get; set; } = false;
    public Series? SelectedSeries
    {
        get => null;
        set => OnSeriesSelected(value);
    }

    public Movie? SelectedMovie
    {
        get => null;
        set => OnMovieSelected(value);
    }
        
    private readonly List<Series> _series = Globals.Series;
    public ObservableCollection<Series> DisplayedSeries { get; set; } = [];
        
    private readonly List<Movie> _movies = Globals.Movies;
    public ObservableCollection<Movie> DisplayedMovies { get; set; } = [];
        
    private string _searchText = string.Empty;

    public string SearchText
    {
        get => _searchText;
        set
        {
            _searchText = value;
            FilterDisplayData(_searchText);
        }
    }
    public MediaMenuViewModel()
    {
        
    }
    
    public async Task InitializeDataAsync()
    {
        if (_series.Count == 0 || _movies.Count == 0)
        {
            IsProgressbarVisible = true;
            _series.AddRange(await ImportController.GetAllSeriesAsync());
            _movies.AddRange(await ImportController.GetAllMoviesAsync());
            IsProgressbarVisible = false;
        }
            
        //foreach (var s in _series) Console.WriteLine($"'{s.Names[Language.English]}' with {s.Genres.Count} genres and {s.Episodes.Count} episodes");
        //foreach (var m in _movies) Console.WriteLine($"'{m.VideoOfMovie.Names[Language.English]}' with {m.Genres.Count} genres and {m.Extras.Count} extras");
            
        FilterDisplayData();
            
        await Task.CompletedTask;
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
        
    private void OnSeriesSelected(Series? series)
    { 
        if (series != null) Console.WriteLine($"Series '{series.Names[Globals.CurrentLanguage]}' selected");
        Messenger.Publish(new Message
        {
            Action = Globals.NavigateToSeriesPageAction,
            Data = series
        });
    }

    private void FilterDisplayData(string searchText = "", int similarityThreshold = 50)
    {
        DisplayedSeries.Clear();
        DisplayedMovies.Clear();
            
        var series = _series.ToList();
        var movies = _movies.ToList();
            
        if (!string.IsNullOrEmpty(searchText)) 
        {
            var namesOfSeries = series.Select(s => s.Names[Globals.CurrentLanguage]).ToList();; 
            var namesOfMovies = movies.Select(m => m.VideoOfMovie.Names[Globals.CurrentLanguage]).ToList();;
            
            var seriesResults = FuzzySharp.Process.ExtractTop(searchText, namesOfSeries, limit: 10);
            var moviesResults = FuzzySharp.Process.ExtractTop(searchText, namesOfMovies, limit: 10);
            
            var extractedResults = moviesResults.ToList();
                
            series = seriesResults
                .Select(result => new { Name = result.Value, Score = result.Score})
                .Join(series, 
                    result => result.Name, 
                    s => s.Names[Globals.CurrentLanguage], 
                    (result, s) => new { Series = s, Score = result.Score})
                .OrderByDescending(item => item.Score)
                .Where(item => item.Score > similarityThreshold)
                .Select(item => item.Series)
                .ToList();
                
            movies = extractedResults
                .Select(result => new { Name = result.Value, Score = result.Score })
                .Join(movies, 
                    result => result.Name, 
                    m => m.VideoOfMovie.Names[Globals.CurrentLanguage], 
                    (result, m) => new { Movie = m, Score = result.Score })
                .OrderByDescending(item => item.Score)
                .Where(item => item.Score > similarityThreshold)
                .Select(item => item.Movie)
                .ToList();
        }
        else
        {
            series = series.OrderBy(s => s.Names[Globals.CurrentLanguage]).ToList(); 
            movies = movies.OrderBy(m => m.VideoOfMovie.Names[Globals.CurrentLanguage]).ToList();
        }
            
        series.ForEach(DisplayedSeries.Add);
        movies.ForEach(DisplayedMovies.Add);
    }
}
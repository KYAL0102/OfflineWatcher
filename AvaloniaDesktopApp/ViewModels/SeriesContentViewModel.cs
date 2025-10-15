using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using Core;
using Core.Entities;

namespace AvaloniaDesktopApp.ViewModels;

public class SeriesContentViewModel : ViewModelBase
{
    public Series? SelectedSeries
    {
        get => null;
        set => OnSeriesSelected(value);
    }
    private readonly List<Series> _series = Globals.Series.ToList();
    private readonly List<SeriesRow> _allSeriesPerGenreRows = new();
    private int _currentBatchIndex = 0;
    private const int BatchSize = 5; // Load 5 rows at a time
    private SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);
    
    public record SeriesRow
    {
        public int Priority { get; set; } = -1;
        public string Header { get; set; } = string.Empty;
        public List<Series> DisplayedSeries { get; set; } = [];
    }
    public ObservableCollection<SeriesRow> SeriesRows { get; set; } = [];
    public SeriesContentViewModel() { }

    public async Task InitializeDataAsync()
    {
        if (_series.Count == 0)
        {
            //IsProgressbarVisible = true;
            var series = await ImportController.GetAllSeriesAsync();
            _series.AddRange(series);
            Globals.Series.AddRange(series);
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
        var movies = _series.ToList();
        var bag = new ConcurrentBag<SeriesRow>();
        var tasks = new List<Task>();

        bag.Add(new SeriesRow
        {
            Priority = 1,
            Header = "All",
            DisplayedSeries = _series.ToList()
        });

        await Task.CompletedTask;

        _allSeriesPerGenreRows.Clear();
        _allSeriesPerGenreRows.AddRange(bag);
    }

    public async Task LoadNextBatchAsync()
    {
        await _semaphoreSlim.WaitAsync();

        var startIndex = _currentBatchIndex;
        var endIndex = Math.Min(startIndex + BatchSize, _allSeriesPerGenreRows.Count);

        if (startIndex >= _allSeriesPerGenreRows.Count)
        {
            _semaphoreSlim.Release();
            return;
        }

        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            for (int i = startIndex; i < endIndex; i++)
            {
                SeriesRows.Add(_allSeriesPerGenreRows[i]);
            }
        });

        _currentBatchIndex = endIndex;
        _semaphoreSlim.Release();
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
}

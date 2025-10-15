using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Core;
using Core.Entities;
using ReactiveUI;

namespace AvaloniaDesktopApp.ViewModels;

public class SearchViewModel : ViewModelBase
{
    private const double SimilarityThreshold = 90;
    private string _searchText = string.Empty;
    public string SearchText
    {
        get => _searchText;
        set
        {
            _searchText = value;
            SearchForTitle(_searchText);
        }
    }

    private StreamItem? _selectedItem = null;
    public StreamItem? SelectedItem
    {
        get => _selectedItem;
        set
        {
            if (_selectedItem != value) _selectedItem = value;
        }
    }
    private List<StreamItem> _allStreamItems = [];
    public ObservableCollection<StreamItem> DisplayedStreamItems { get; set; } = [];
    public SearchViewModel() { }

    public async Task InitializeDataAsync()
    {
        if (Globals.Movies.Count == 0)
        {
            var movies = await ImportController.GetAllMoviesAsync();
            Globals.Movies.AddRange(movies);
            _allStreamItems.AddRange(movies);
        }
        else _allStreamItems.AddRange(Globals.Movies);

        if (Globals.Series.Count == 0)
        {
            var series = await ImportController.GetAllSeriesAsync();
            Globals.Series.AddRange(series);
            _allStreamItems.AddRange(series);
        }
        else _allStreamItems.AddRange(Globals.Series);
    }

    private void SearchForTitle(string searchText)
    {
        DisplayedStreamItems.Clear();
        var namesOfItems = _allStreamItems.Select(item => item.NameInCurrentLanguage).ToList();

        var finalItems = FuzzySharp.Process.ExtractTop(searchText, namesOfItems, limit: 10);

        finalItems
            .Select(result => new { Name = result.Value, Score = result.Score })
            .Join(_allStreamItems,
                result => result.Name,
                i => i.Names[Globals.CurrentLanguage],
                (result, i) => new { Item = i, Score = result.Score })
            .OrderByDescending(i => i.Score)
            .Where(i => i.Score >= SimilarityThreshold)
            .Select(i => i.Item)
            .ToList()
            .ForEach(DisplayedStreamItems.Add);
    }
}

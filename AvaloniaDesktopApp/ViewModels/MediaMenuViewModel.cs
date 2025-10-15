using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using AvaloniaDesktopApp.Controls;
using CommunityToolkit.Mvvm.Input;
using Core;
using Core.Entities;

namespace AvaloniaDesktopApp.ViewModels;

public class MediaMenuViewModel : ViewModelBase
{
    public bool IsProgressbarVisible { get; set; } = false;
            
    private string _searchText = string.Empty;

    public string SearchText
    {
        get => _searchText;
        set
        {
            _searchText = value;
            //FilterDisplayData(_searchText);
        }
    }

    private string _currentHeader = string.Empty;
    public string CurrentHeader
    {
        get => _currentHeader;
        set
        {
            _currentHeader = value;
            OnPropertyChanged();
        }
    }

    private UserControl? _currentControl = null;
    public UserControl? CurrentControl
    {
        get { return _currentControl; }
        set
        {
            _currentControl = value;
            OnPropertyChanged();
        }
    }

    private MovieContentControl? _movieContentControl = null;
    private SeriesContentControl? _seriesContentControl = null;

    public RelayCommand ShowSearchCommand { get; set; }
    public RelayCommand ShowMoviesCommand { get; set; }
    public RelayCommand ShowSeriesCommand { get; set; }
    public RelayCommand ShowSettingsCommand { get; set; }

    public MediaMenuViewModel()
    {
        ShowSearchCommand = new RelayCommand(
            () => throw new NotImplementedException(),
            () => false
        );
        ShowMoviesCommand = new RelayCommand(
            () => ShowMovies(),
            () => true
        );
        ShowSeriesCommand = new RelayCommand(
            () => ShowSeries(),
            () => true
        );
        ShowSettingsCommand = new RelayCommand(
            () => throw new NotImplementedException(),
            () => false
        );

        ShowMovies();
    }

    private void ShowMovies()
    {
        CurrentHeader = "Movies";

        if (_movieContentControl == null)
        {
            _movieContentControl = new();
        }

        CurrentControl = _movieContentControl;
    }
    
    private void ShowSeries()
    {
        CurrentHeader = "Series";

        if (_seriesContentControl == null)
        {
            _seriesContentControl = new();
        }

        CurrentControl = _seriesContentControl;
    }

    public async Task InitializeDataAsync()
    {
        await Task.CompletedTask;
    }
}
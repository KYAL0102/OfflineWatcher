using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using Core;
using Core.Entities;
using MVVMBase;

namespace WpfDesktopApp.ViewModels;

public class SeriesPageViewModel : BaseViewModel
{
    private Series? _series = null;
    public ObservableCollection<Video> Episodes { get; set; } = [];
    
    public RelayCommand BackToMenuCommand { get; set; }
    public RelayCommand PlayVideoCommand { get; set; }
    public RelayCommand PlayVideoWithVlcCommand { get; set; }
    public SeriesPageViewModel(WindowController? controller, Series? series) : base(controller)
    {
        _series = series;
        _series?.Episodes.ForEach(e => Episodes.Add(e));
        
        BackToMenuCommand = new RelayCommand(
            _ => BackToMenu(),
            _ => true);
        PlayVideoCommand = new RelayCommand(
            PlayVideo,
            _ => _series != null);
        PlayVideoWithVlcCommand = new RelayCommand(
            PlayVideoWithVlc,
            _ => _series != null);
    }
    
    public async Task InitializeDataAsync()
    {
        await Task.CompletedTask;
    }
    
    private void PlayVideo(object? item)
    {
        Globals.CurrentStreamItem = _series;
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
    
    private void BackToMenu()
    {
        Messenger.Publish(new Message
        {
            Action = Globals.NavigateToMenuAction,
        });
    }
}
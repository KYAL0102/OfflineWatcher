using System.Threading.Tasks;
using Core.Entities;

namespace AvaloniaDesktopApp.ViewModels;

public class VideoFullscreenViewModel : ViewModelBase
{
    private Video? _video = null;
    public Video? Video
    {
        get => _video;
        set
        {
            _video = value;
            if (_video == null) return;

            VideoFilePath = _video.PathToVideoFile;
        }
    }

    private string _videoFilePath = string.Empty;
    public string VideoFilePath
    {
        get => _videoFilePath;
        set
        {
            _videoFilePath = value;
            OnPropertyChanged();
        }
    }
    public VideoFullscreenViewModel(Video? video = null)
    {
        Video = video;
    }
    
    public async Task InitializeDataAsync()
    {
        await Task.CompletedTask;
    }
}
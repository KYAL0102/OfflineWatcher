using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using LibVLCSharp.Shared;
using LibVLCSharp.Shared.Structures;
using ReactiveUI;

namespace AvaloniaDesktopApp.Controls;

public partial class VideoPlayer : UserControl
{
    private const string PlayButtonImagePath = "avares://Assets/Images/play.png";
    private const string PauseButtonImagePath = "avares://Assets/Images/pause.png";
    
    private LibVLC? _libVLC;
    private MediaPlayer? _mediaPlayer;
    private DispatcherTimer? _timer;

    public static readonly StyledProperty<Uri> VideoSourceProperty =
        AvaloniaProperty.Register<VideoPlayer, Uri>(nameof(VideoSource));

    public Uri VideoSource
    {
        get => GetValue(VideoSourceProperty);
        set => SetValue(VideoSourceProperty, value);
    }
    
    private bool _videoIsRunning = false;
    public bool VideoIsRunning
    {
        get => _videoIsRunning;
        set
        {
            _videoIsRunning = value;
            try
            {
                var path = PlayButtonImagePath;
                if (_videoIsRunning) path = PauseButtonImagePath;
                
                VideoStateButtonImage.Source = new Bitmap(path);
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Message);
                Console.ResetColor();
            }
        }
    }

    public VideoPlayer()
    {
        InitializeComponent();
        VideoSourceProperty.Changed.Subscribe(OnVideoSourceChanged);
        
        _libVLC = new LibVLC(new string[]
        {
            "--verbose=2",
            "--no-xlib",
            "--no-video-title-show",
            "--avcodec-hw=none",
            "--aout=wasapi",
            "--vout=direct3d11"
        });
        _mediaPlayer = new MediaPlayer(_libVLC);
        VideoView.MediaPlayer = _mediaPlayer;
        Unloaded += VideoPlayer_Unloaded;
        _mediaPlayer.MediaChanged += MediaPlayer_MediaChanged;
        _mediaPlayer.TimeChanged += MediaPlayer_TimeChanged;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void OnVideoSourceChanged(AvaloniaPropertyChangedEventArgs args)
    {
        var control = (VideoPlayer)args.Sender;
        var newUri = (Uri) args.NewValue!;

        if (control._libVLC != null)
        {
            var media = new Media(control._libVLC, newUri.LocalPath, FromType.FromPath);
            control._mediaPlayer?.Play(media);
        }
    }

    private void MediaPlayer_MediaChanged(object? sender, MediaPlayerMediaChangedEventArgs e)
    {
        var mediaPlayer = _mediaPlayer;
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            if (mediaPlayer == null) return;
            
            var audioTracks = mediaPlayer.AudioTrackDescription;
            var spuTracks = mediaPlayer.SpuDescription;
            mediaPlayer.SetSpu(-1);
            TrackDescription? trackDescription = audioTracks.FirstOrDefault(track => track.Name.Contains("[English]"));
            if (trackDescription is { } t)
            {
                mediaPlayer.SetAudioTrack(t.Id);
            }
        }, DispatcherPriority.Background);
    }

    private void VideoPlayer_Unloaded(object? sender, RoutedEventArgs e)
    {
        Task.Run(() =>
        {
            _mediaPlayer?.Stop();
            _mediaPlayer?.Dispose();
            _mediaPlayer = null;
            _libVLC?.Dispose();
        });
    }

    private void UIElement_OnMouseDown(object sender, PointerPressedEventArgs e)
    {
        ChangeVideoState(sender, e);
    }

    private void ChangeVideoState(object sender, RoutedEventArgs e)
    {
        VideoIsRunning = !VideoIsRunning;
        if (VideoIsRunning)
            _mediaPlayer?.Play();
        else
            _mediaPlayer?.Pause();
    }

    private void OnMouseLeave(object sender, PointerEventArgs e)
    {
        InnerGrid.Opacity = 0;
        InnerUpperGrid.Opacity = 0;
        _timer?.Stop();
    }

    private void OnMouseMove(object sender, PointerEventArgs e)
    {
        if (InnerGrid.Opacity == 0) InnerGrid.Opacity = 1;
        if (InnerUpperGrid.Opacity == 0) InnerUpperGrid.Opacity = 1;
        if (_timer == null)
        {
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(3000)
            };
            _timer.Tick += HideTimer_Tick;
        }
        _timer.Stop();
        _timer.Start();
    }

    private void HideTimer_Tick(object? sender, EventArgs e)
    {
        _timer?.Stop();
        InnerGrid.Opacity = 0;
        InnerUpperGrid.Opacity = 0;
    }

    private void SoundSlider_OnValueChanged(object? sender, RangeBaseValueChangedEventArgs rangeBaseValueChangedEventArgs)
    {
        if (_mediaPlayer != null)
        {
            _mediaPlayer.Volume = (int)rangeBaseValueChangedEventArgs.NewValue;
        }
    }

    private bool _manualSliderChange = false;
    private bool _manualSliderChanged = false;
    private DateTime _lastSliderUpdate = DateTime.MinValue;

    private async void VideoSlider_OnValueChanged(object? sender, RangeBaseValueChangedEventArgs rangeBaseValueChangedEventArgs)
    {
        if (!_manualSliderChange && _mediaPlayer != null && sender is Slider slider && slider.IsPointerOver)
        {
            var now = DateTime.Now;
            if ((now - _lastSliderUpdate).TotalMilliseconds < 100) return;
            _lastSliderUpdate = now;
            _manualSliderChange = true;
            try
            {
                var newValue = (long)(_mediaPlayer.Length / (VideoSlider.Maximum - VideoSlider.Minimum) * VideoSlider.Value);
                if (newValue >= 0 && newValue <= _mediaPlayer.Length)
                {
                    await Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        _mediaPlayer.Time = newValue;
                    });
                    _manualSliderChanged = true;
                }
            }
            finally
            {
                _manualSliderChange = false;
            }
        }
    }

    private bool _isUpdatingTimeLabel = false;

    private async void MediaPlayer_TimeChanged(object? sender, MediaPlayerTimeChangedEventArgs e)
    {
        if (_isUpdatingTimeLabel) return;
        _isUpdatingTimeLabel = true;
        try
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                if (_mediaPlayer != null && !_manualSliderChange && !_manualSliderChanged)
                {
                    var newSliderValue = ((VideoSlider.Maximum - VideoSlider.Minimum) / _mediaPlayer.Length) * _mediaPlayer.Time;
                    VideoSlider.Value = newSliderValue;
                }
                if (_manualSliderChanged)
                {
                    _manualSliderChanged = false;
                }
                TimeSpan timeSpan = TimeSpan.FromMilliseconds(e.Time);
                TimeLabel.Content = timeSpan.ToString(@"hh\:mm\:ss");
            });
        }
        finally
        {
            _isUpdatingTimeLabel = false;
        }
    }

    private void CancelPlay(object sender, RoutedEventArgs e)
    {
        // Implement your navigation logic here
    }
}
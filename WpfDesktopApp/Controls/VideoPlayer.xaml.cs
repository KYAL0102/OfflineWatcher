using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Core;
using Core.Entities;
using LibVLCSharp.Shared;
using LibVLCSharp.Shared.Structures;
using MediaPlayer = LibVLCSharp.Shared.MediaPlayer;

namespace WpfDesktopApp.Controls;

public partial class VideoPlayer : UserControl
{
    private const string PlayButtonImagePath = "pack://application:,,,/Resources/Images/play.png";
    private const string PauseButtonImagePath = "pack://application:,,,/Resources/Images/pause.png";

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
                
                VideoStateButtonImage.Source = new BitmapImage(new Uri(path));
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Message);
                Console.ResetColor();
            }
        }
    }

    public static readonly DependencyProperty VideoSourceProperty =
        DependencyProperty.Register(
            nameof(VideoSource),
            typeof(Uri),
            typeof(VideoPlayer),
            new PropertyMetadata(null, OnVideoSourceChanged));

    public Uri VideoSource
    {
        get => (Uri)GetValue(VideoSourceProperty);
        set => SetValue(VideoSourceProperty, value);
    }
    
    private LibVLC _libVLC;
    private MediaPlayer? _mediaPlayer = null;
    public VideoPlayer()
    {
        InitializeComponent();
        
        LibVLCSharp.Shared.Core.Initialize();

        _libVLC = new LibVLC(new string[]
        {
            /*"--verbose=2",
            "--no-xlib",
            "--no-video-title-show",
            "--avcodec-hw=none",
            "--aout=wasapi", // Try using the WASAPI audio output module
            "--vout=direct3d11" // Try using the Direct3D11 video output module*/
        });
        VideoIsRunning = false;
        _mediaPlayer = new MediaPlayer(_libVLC);
        VideoView.MediaPlayer = _mediaPlayer;

        Unloaded += VideoPlayer_Unloaded;
        _mediaPlayer.MediaChanged += MediaPlayer_MediaChanged;
        _mediaPlayer.TimeChanged += MediaPlayer_TimeChanged;
    }
    
    private void VideoView_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        e.Handled = true;

        var parent = VisualTreeHelper.GetParent(this);
        if (parent != null && parent is UIElement uiElement)
        {
            var args = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
            {
                RoutedEvent = MouseWheelEvent,
                Source = sender
            };
            uiElement.RaiseEvent(args);
        }
    }

    
    private static void OnVideoSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (VideoPlayer)d;
        var newUri = (Uri)e.NewValue;

        if (newUri != null && control._mediaPlayer != null)
        {
            var media = new Media(control._libVLC, newUri.LocalPath, FromType.FromPath);
            control.VideoIsRunning = true;
            control._mediaPlayer.Play(media);
        }
    }
    
    private void MediaPlayer_MediaChanged(object? sender, MediaPlayerMediaChangedEventArgs e)
    {
        if (_mediaPlayer == null) return;

        var mediaPlayer = _mediaPlayer;

        // Give it a moment to load the tracks
        Dispatcher.InvokeAsync(() =>
        {
            // Now you can try to access the track descriptions
            var audioTracks = mediaPlayer.AudioTrackDescription;
            var spuTracks = mediaPlayer.SpuDescription;

            // Set no subtitle as default
            mediaPlayer.SetSpu(-1);

            TrackDescription? trackDescription = null;
            if (audioTracks.Any(track => track.Name.Contains("[English]")))
            {
                trackDescription = audioTracks.FirstOrDefault(track => track.Name.Contains("[English]"));
            }
            if (trackDescription is { } t) mediaPlayer.SetAudioTrack(t.Id);

            // Optionally, you can set the default audio track if needed
            /*if (audioTracks.Length > 0)
            {
                mediaPlayer.SetAudioTrack(audioTracks[0].Id);
            }*/

        }, System.Windows.Threading.DispatcherPriority.Background);
    }
    
    private void VideoPlayer_Unloaded(object sender, RoutedEventArgs e)
    {
        Task.Run(() =>
        {
            _mediaPlayer?.Stop();
            _mediaPlayer?.Dispose();
            _mediaPlayer = null;
            _libVLC.Dispose();
        });
    }

    private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        ChangeVideoState(sender, e);
    }

    private void ChangeVideoState(object sender, RoutedEventArgs e)
    {
        VideoIsRunning = !VideoIsRunning;

        if (VideoIsRunning) _mediaPlayer?.Play();
        else _mediaPlayer?.Pause();
    }

    private DispatcherTimer? _timer = null;
    private void OnMouseLeave(object sender, MouseEventArgs e)
    {
        InnerGrid.Opacity = 0;
        InnerUpperGrid.Opacity = 0;
        _timer?.Stop();
    }

    private void OnMouseMove(object sender, MouseEventArgs e)
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

        Mouse.OverrideCursor = null;
        _timer.Stop();
        _timer.Start();
    }
    
    private void HideTimer_Tick(object? sender, System.EventArgs e)
    {
        _timer?.Stop();
        InnerGrid.Opacity = 0;
        InnerUpperGrid.Opacity = 0;
        Mouse.OverrideCursor = Cursors.None;  // Hide cursor
    }

    private void SoundSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (_mediaPlayer != null)
        {
            Console.WriteLine($"Old volume: {_mediaPlayer.Volume}");
            Console.WriteLine($"New volume: {(int)e.NewValue}");
            _mediaPlayer.Volume = (int)e.NewValue;
        }
    }

    private DateTime _lastTimeUpdate = DateTime.MinValue;
    private void VideoSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (_mediaPlayer is not { IsPlaying: true }) return;

        // Debounce rapid changes
        var now = DateTime.Now;
        if ((now - _lastTimeUpdate).TotalMilliseconds < 200) return;
        _lastTimeUpdate = now;

        var newValue = (long)(_mediaPlayer.Length / (VideoSlider.Maximum - VideoSlider.Minimum) * VideoSlider.Value);
        if (newValue >= 0 && newValue <= _mediaPlayer.Length)
        {
            _mediaPlayer.Time = newValue;
        }
    }

    private bool _isUpdatingTimeLabel = false;
    private void MediaPlayer_TimeChanged(object? sender, MediaPlayerTimeChangedEventArgs e)
    {
        if (_isUpdatingTimeLabel) return; // Prevent reentrant calls

        _isUpdatingTimeLabel = true;
        try
        {
            Dispatcher.Invoke(() =>
            {
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
        var action = Globals.NavigateToMoviePageAction;
        if (Globals.CurrentStreamItem is Series) action = Globals.NavigateToSeriesPageAction;
        Messenger.Publish(new Message
        {
            Action = action,
            Data = Globals.CurrentStreamItem,
        });
    }
}
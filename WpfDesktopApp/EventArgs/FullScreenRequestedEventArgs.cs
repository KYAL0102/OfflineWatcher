using System.Windows;
using WpfDesktopApp.Controls;

namespace WpfDesktopApp.EventArgs;

public class FullScreenRequestedEventArgs(RoutedEvent routedEvent, VideoPlayer videoPlayer)
    : RoutedEventArgs(routedEvent)
{
    public VideoPlayer VideoPlayer { get; set; } = videoPlayer;
}
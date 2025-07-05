using System.Windows.Controls;
using Core.Entities;
using MVVMBase;
using WpfDesktopApp.ViewModels;

namespace WpfDesktopApp.Views;

public partial class VideoFullscreen : UserControl
{
    private VideoFullscreenViewModel _viewModel;
    public VideoFullscreen(WindowController? controller, Video? video = null)
    {
        InitializeComponent();
        _viewModel = new VideoFullscreenViewModel(controller, video);
        DataContext = _viewModel;
        Loaded += UserControl_Loaded;
    }
    
    private async void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        await _viewModel.InitializeDataAsync();
    }
}
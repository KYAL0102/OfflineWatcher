using System.Windows.Controls;
using MVVMBase;
using WpfDesktopApp.ViewModels;

namespace WpfDesktopApp.Views;

public partial class MediaMenu : UserControl
{
    private readonly MenuViewModel _viewModel;
    
    public MediaMenu(WindowController? controller)
    {
        InitializeComponent();
        _viewModel = new MenuViewModel(controller);
        DataContext = _viewModel;
        Loaded += UserControl_Loaded;
    }

    private async void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        await _viewModel.InitializeDataAsync();
    }
}
using System.Windows.Controls;
using Core.Entities;
using MVVMBase;
using WpfDesktopApp.ViewModels;

namespace WpfDesktopApp.Views;

public partial class SeriesPage : UserControl
{
    private SeriesPageViewModel _viewModel;
    
    public SeriesPage(WindowController? controller, Series? series = null)
    {
        InitializeComponent();
        _viewModel = new SeriesPageViewModel(controller, series);
        DataContext = _viewModel;
        Loaded += UserControl_Loaded;
    }
    
    private async void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        await _viewModel.InitializeDataAsync();
    }
}
using System.Windows;
using System.Windows.Controls;
using Core.Entities;
using MVVMBase;
using WpfDesktopApp.ViewModels;

namespace WpfDesktopApp.Views;

public partial class MoviePage : UserControl
{
    private MoviePageViewModel _viewModel;
    public MoviePage(WindowController? controller, Movie? movie = null)
    {
        InitializeComponent();
        _viewModel = new MoviePageViewModel(controller, movie);
        DataContext = _viewModel;
        Loaded += UserControl_Loaded;
    }
    
    private async void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        await _viewModel.InitializeDataAsync();
    }
}
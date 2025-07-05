using System.Windows;
using MVVMBase;
using WpfDesktopApp.ViewModels;

namespace WpfDesktopApp.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;
        public MainWindow()
        {
            InitializeComponent();
            var controller = new WindowController(this);
            _viewModel = new MainViewModel(controller, this);
            DataContext = _viewModel;
            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await _viewModel.InitializeDataAsync();
            
        }
    }
}
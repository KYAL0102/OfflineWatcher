using Core.Contracts;
using MVVMBase;
using Persistence;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfDesktopApp.ViewModels;

namespace WpfDesktopApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IUnitOfWork _unitOfWork = new UnitOfWork();
        private readonly MainViewModel _viewModel;
        public MainWindow()
        {
            InitializeComponent();
            var controller = new WindowController(this);
            _viewModel = new MainViewModel(controller, _unitOfWork);
            DataContext = _viewModel;
            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await _viewModel.InitializeDataAsync();
        }
    }
}
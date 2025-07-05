using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MVVMBase
{
    public class WindowController
    {
        private readonly Window _window;

        public WindowController(Window window)
        {
            _window = window;
        }
        public void ShowWindow(BaseViewModel viewModel)
        {
            _window.DataContext = viewModel;
            _window.Show();
        }

        public void ShowDialog(BaseViewModel viewModel)
        {
            _window.DataContext = viewModel;
            _window.ShowDialog();
        }

        public void CloseWindow()
        {
            _window.Close();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMBase
{
    public class BaseViewModel : NotifyPropertyChanged
    {
        public WindowController? Controller { get; }

        public BaseViewModel(WindowController? controller)
        {
            Controller = controller;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ImageBWConverter.ViewModel.Commands
{
    public class LoadImageCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        Action _execute;

        public LoadImageCommand(Action execute)
        {
            _execute = execute;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _execute.Invoke();
        }
    }
}

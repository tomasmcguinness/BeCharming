using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BeCharming.Metro.ViewModels
{
    public class MainViewModel
    {
        public event EventHandler OpenAddTargetDialog;

        public MainViewModel()
        {
            AddTarget = new DelegateCommand(AddTargetExecute);
        }

        public ICommand AddTarget { get; set; }

        public void AddTargetExecute(object state)
        {
            if (OpenAddTargetDialog != null)
            {
                OpenAddTargetDialog(this, null);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BeCharming.Metro.ViewModels
{
    public class ShareTargetsViewModel
    {
        public ShareTargetsViewModel()
        {
            Targets = new ObservableCollection<ShareTarget>();
            TargetSelectedCommand = new DelegateCommand(TargetSelected);
        }

        public ObservableCollection<ShareTarget> Targets { get; private set; }
        public ICommand TargetSelectedCommand { get; set; }

        public void LoadTargets()
        {
            ShareTarget t = new ShareTarget();
            t.Name = "Test";
            t.IP = "192.168.1.8";
            Targets.Add(t);
        }

        public void TargetSelected(object state)
        {
            var service = new ListenerClient();
        }
    }
}

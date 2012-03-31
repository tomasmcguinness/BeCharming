using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeCharming.Metro.ViewModels
{
    public class ShareTargetsViewModel
    {
        public ShareTargetsViewModel()
        {
            Targets = new ObservableCollection<ShareTarget>();
        }

        public ObservableCollection<ShareTarget> Targets { get; private set; }

        public void LoadTargets()
        {
            ShareTarget t = new ShareTarget();
            t.Name = "Test";
            Targets.Add(t);
        }
    }
}

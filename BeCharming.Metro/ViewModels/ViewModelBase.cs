using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeCharming.Metro.ViewModels
{
    public class ViewModelBase
    {
        private Windows.UI.Core.CoreDispatcher _dispatcher;
        public ViewModelBase(Windows.UI.Core.CoreDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        protected Windows.UI.Core.CoreDispatcher Dispatcher
        {
            get { return _dispatcher; }
        }

    }
}

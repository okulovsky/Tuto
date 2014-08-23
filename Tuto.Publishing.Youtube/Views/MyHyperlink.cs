using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace Tuto.Publishing.Youtube.Views
{
    public class MyHyperlink : Hyperlink
    {
        public MyHyperlink()
        {
            RequestNavigate += (s, a) => { Process.Start(NavigateUri.AbsoluteUri); };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuto.Navigator;

namespace Tuto.Publishing
{
    public class VideoWrapLatexModel : NotifierModel
    {
        public RelayCommand Open { get; private set; }
        public RelayCommand Compile { get; private set; }
        public RelayCommand Edit { get; private set; }

        

    }
}

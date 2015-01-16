using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Tuto.Navigator;

namespace Tuto.Publishing
{
    public interface IYoutubeBlockModel
    {
        Brush StatusColor { get; }
        RelayCommand Push { get; }
        RelayCommand View { get; }
    }
}

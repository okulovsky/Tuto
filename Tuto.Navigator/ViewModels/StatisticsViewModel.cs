using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Navigator
{
    public class StatisticsViewModel : NotifierModel
    {

        public int TotalDirty { get; set; }
        public int TotalClean { get; set; }
        public double Percentage { get { if (TotalDirty == 0) return 0; return 100 - (100 * TotalClean) / TotalDirty; } }

        public int EpisodesCount { get; set; }
    }
}

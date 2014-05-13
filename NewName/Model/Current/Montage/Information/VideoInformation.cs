using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    public class VideoInformation
    {
        public List<EpisodInfo> Episodes { get;  set; }
        public VideoInformation()
        {
            Episodes = new List<EpisodInfo>();
        }

    }
}

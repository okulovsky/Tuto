using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    public class VideoInformationV4
    {
        public List<EpisodInfoV4> Episodes { get;  set; }
        public VideoInformationV4()
        {
            Episodes = new List<EpisodInfoV4>();
        }

    }
}

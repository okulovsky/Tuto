using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuto.Model;
using Tuto.Publishing.YoutubeData;

namespace Tuto.Publishing
{
    public static class StaticItems
    {
        public static IYoutubeProcessor YoutubeProcessor { get; set; }
        public static GlobalData GlobalData { get; set; }
    }
}

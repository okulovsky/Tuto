using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuto.Model;
using Tuto.Navigator;
using Tuto.Publishing.YoutubeData;

namespace Tuto.Publishing
{
    public partial class VideoWrap : VideoItem
    {
        public YoutubeVideoBlockModel VideoBlock { get; private set; }

        public void Initialize()
        {
            VideoBlock = new YoutubeVideoBlockModel(this);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Publishing
{
    public static class YoutubeDataBinding
    {
        public static void LoadYoutubeData(Item root, DirectoryInfo directory)
        {
            DataBinding<IYoutubeClipItem>.PullFromFile(root, z => z.YoutubeClip, directory);
            DataBinding<IYoutubePlaylistItem>.PullFromFile(root, z => z.YoutubePlaylist, directory);
        }

        public static void SaveYoutubeData(Item root, DirectoryInfo directory)
        {
            DataBinding<IYoutubeClipItem>.SaveLayer(root, z => z.YoutubeClip, directory);
            DataBinding<IYoutubePlaylistItem>.SaveLayer(root, z => z.YoutubePlaylist, directory);
        }
    }
}

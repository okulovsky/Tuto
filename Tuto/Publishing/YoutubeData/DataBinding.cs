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
            DataBinding<VideoItem>.PullFromFile<YoutubeClip>(root, directory);
            DataBinding<LectureItem>.PullFromFile<YoutubePlaylist>(root, directory);
        }

        public static void SaveYoutubeData(Item root, DirectoryInfo directory)
        {
            DataBinding<VideoItem>.SaveLayer<YoutubeClip>(root, directory);
            DataBinding<LectureItem>.SaveLayer<YoutubePlaylist>(root, directory);
        }
    }
}

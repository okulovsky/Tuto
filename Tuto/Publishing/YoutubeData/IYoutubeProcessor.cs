using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Publishing.YoutubeData
{
    public interface IYoutubeProcessor
    {
        void Authorize(DirectoryInfo directory);
        List<YoutubeClip> GetAllClips();
        void UpdateVideo(YoutubeClip clip, YoutubeVideoUpdateInfo updateInfo);
    }
}

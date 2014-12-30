using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuto.Model;

namespace Tuto.Publishing
{
    public enum Status
    {
        MatchedOld,
        MatchedNew,
        NotFoundAtYoutube,
        NotExpectedAtYoutube,
        DeletedFromYoutube,
        DeletedFromTuto,
        DeletedFromBoth,
    }

    public class VideoWrap : VideoItem, IYoutubeClipItem
    {

        public string VideoURLFull { get { if (YoutubeClip == null) return ""; return YoutubeClip.VideoURLFull; } }
        public string VideoURLShort { get { if (YoutubeClip == null) return ""; return YoutubeClip.Id; } }
        public string YoutubeName { get { if (YoutubeClip == null) return ""; return YoutubeClip.Name; } }

        public VideoWrap()
        {
        }

       

        public YoutubeClip YoutubeClip
        {
            get; set; 
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuto.Model;

namespace Tuto.Publishing
{

    public class LectureWrap : LectureItem, IYoutubePlaylistItem
    {

        public string PlaylistLinkShort
        {
            get
            {
                if (YoutubePlaylist != null) return YoutubePlaylist.PlaylistId;
                return "Missing";
            }
        }

        public string PlalistLinkFull
        {
            get
            {
                return "http://google.com";
            }
        }

        public YoutubePlaylist YoutubePlaylist
        {
            get;
            set;
        }
    }
}

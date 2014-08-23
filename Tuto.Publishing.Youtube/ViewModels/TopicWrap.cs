using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuto.Model;

namespace Tuto.Publishing.Youtube
{

    public class TopicWrap : Wrap
    {
        public Topic Topic { get; set; }
        public TopicLevel CorrespondedLevel { get; set; }

        public PublishedTopic Published { get; set; }
        public PlaylistData Playlist { get; set; }
        public bool PlaylistRequired { get; set; }

        public string PlaylistLinkShort
        {
            get
            {
                if (!PlaylistRequired) return "";
                if (Playlist != null) return Playlist.Id;
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

        public override int Digits
        {
            get
            {
                return CorrespondedLevel.Digits;
            }
        }

    }
}

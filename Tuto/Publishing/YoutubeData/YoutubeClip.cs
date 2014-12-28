using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Publishing
{
    public class YoutubeClip
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string VideoURLFull { get { return "http://youtube.com/watch?v=" + Id; } }
        public string GDataURL { get { return "http://gdata.youtube.com/feeds/api/videos/" + Id; } }
        public override string ToString()
        {
            return Name;
        }
    }

    public interface IYoutubeClipItem : IItem
    {
        YoutubeClip YoutubeClip { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuto.Model;
using Tuto;
using Tuto.Navigator;

namespace Tuto.Publishing
{

    public class LectureWrap : LectureItem, IYoutubePlaylistItem, IYoutubeProcessorHolder
    {
        

        public string PlaylistUrlShort
        {
            get
            {
                if (YoutubePlaylist != null) return YoutubePlaylist.PlaylistId;
                return "Missing";
            }
        }

        public string PlaylistUrlFull
        {
            get
            {
                if (YoutubePlaylist == null) return "http://google.com";
                return "https://www.youtube.com/playlist?list="+YoutubePlaylist.PlaylistId;
            }
        }

        public YoutubePlaylist youtubePlaylist;

        public YoutubePlaylist YoutubePlaylist
        {
            get { return youtubePlaylist; }
            set { 
                youtubePlaylist = value; 
                NotifyPropertyChanged();
                this.NotifyByExpression(z => z.PlaylistUrlShort);
                this.NotifyByExpression(z => z.PlaylistUrlFull);
            }
        }

        public RelayCommand UpdatePlaylistCommand { get; private set; }
        public RelayCommand UpdateAllCommand { get; private set; }

        void UpdateLecture()
        {
            if (YoutubePlaylist == null)
                YoutubePlaylist = Processor.CreatePlaylist(Topic.Caption);
            Processor.FillPlaylist(YoutubePlaylist, Subtree().OfType<VideoWrap>().Select(z => z.YoutubeClip).Where(z => z != null));
        }

        void UpdateAll()
        {
            UpdateLecture();
            foreach (var e in Subtree().OfType<VideoWrap>())
                e.UpdateVideo();
        }

        public YoutubeData.IYoutubeProcessor Processor
        {
            get;
            set; 
        }

        public LectureWrap()
        {
            UpdatePlaylistCommand = new RelayCommand(UpdateLecture);
            UpdateAllCommand = new RelayCommand(UpdateAll);
        }
    }
}

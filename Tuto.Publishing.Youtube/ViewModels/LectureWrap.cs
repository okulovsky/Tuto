using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuto.Model;
using Tuto;
using Tuto.Navigator;
using Tuto.Publishing.YoutubeData;

namespace Tuto.Publishing
{

    public class LectureWrap : LectureItem
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
            get { return Get<YoutubePlaylist>(); }
            set {
                Store<YoutubePlaylist>(value); 
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
          //  Processor.FillPlaylist(YoutubePlaylist, Subtree().OfType<VideoWrap>().Select(z => z.YoutubeClip).Where(z => z != null));
        }

        void UpdateAll()
        {
            //UpdateLecture();
            //foreach (var e in Subtree().OfType<VideoWrap>())
            //    e.UpdateVideo();
        }

        public YoutubeData.IYoutubeProcessor Processor
        {
            get { return Get<IYoutubeProcessor>(); }
            set { Store<IYoutubeProcessor>(value); }
        }

        public LectureWrap()
        {
            UpdatePlaylistCommand = new RelayCommand(UpdateLecture);
            UpdateAllCommand = new RelayCommand(UpdateAll);
        }
    }
}

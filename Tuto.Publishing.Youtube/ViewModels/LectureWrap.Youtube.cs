using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Tuto.Model;
using Tuto.Navigator;

namespace Tuto.Publishing
{
    public class YoutubeLectureBlockModel : NotifierModel
    {
        readonly LectureWrap Wrap;
        string dueTitle;
        
        void InitializeDueNames()
        {
            var prefix = "";
            prefix += StaticItems.GlobalData.CourseAbbreviation;
            var path = Wrap.PathFromRoot.Skip(1).ToArray();
            for (int levelNumber = 0; levelNumber < path.Length; levelNumber++)
            {

                TopicLevel level = new TopicLevel();
                if (levelNumber < StaticItems.GlobalData.TopicLevels.Count)
                    level = StaticItems.GlobalData.TopicLevels[levelNumber];
                prefix += "-";
                prefix += string.Format("{0:D" + level.Digits + "}", path[levelNumber].NumberInTopic + 1);
            }

            dueTitle = prefix + " " + Wrap.Topic.Caption;
        }

        public YoutubeLectureBlockModel(LectureWrap wrap)
        {
            Wrap = wrap;
            InitializeDueNames();
            Push = new RelayCommand(CmPush);
            View = new RelayCommand(CmGo, () => YoutubePlaylist != null);
        }

        IEnumerable<YoutubeVideoBlockModel> VideoBlocks
        {
            get
            {
                return Wrap.Subtree().OfType<VideoWrap>().Select(z => z.VideoBlock);
            }
        }

        public YoutubePlaylist YoutubePlaylist
        {
            get { return Wrap.Get<YoutubePlaylist>(); }
        }

        public Brush StatusColor
        {
            get
            {
                if (VideoBlocks.Any(z => z.StatusColor == Brushes.Red)) return Brushes.Red;
                if (VideoBlocks.Any(z=>z.StatusColor == Brushes.Yellow)) return Brushes.Yellow;
                if (YoutubePlaylist == null) return Brushes.Yellow;
                if (YoutubePlaylist.PlaylistTitle != dueTitle) return Brushes.Yellow;
                return Brushes.Green;
            }
        }
     
        public RelayCommand Push { get; private set; }

        public RelayCommand View { get; private set; }

        public void CmPush()
        {
            var playlist = YoutubePlaylist;
            if (playlist == null)
                playlist = StaticItems.YoutubeProcessor.CreatePlaylist(Wrap.Topic.Caption);
            StaticItems.YoutubeProcessor.FillPlaylist(playlist, VideoBlocks.Select(z=>z.YoutubeClip).Where(z => z != null));

            foreach (var e in VideoBlocks)
                e.CmPush();

            Wrap.Store<YoutubePlaylist>(playlist);
            this.NotifyByExpression(z => z.StatusColor);

        }

        public void CmGo()
        {
            if (YoutubePlaylist==null) return;
            Process.Start("https://www.youtube.com/playlist?list=" + YoutubePlaylist.PlaylistId);
        }
    }
}

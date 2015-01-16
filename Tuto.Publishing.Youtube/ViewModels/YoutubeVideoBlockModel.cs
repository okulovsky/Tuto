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
    public class YoutubeVideoBlockModel
    {
        string dueTitle;
        string dueDescription;

        void InitializeDueNames()
        {
            var prefix = "";
            var description = "";
            prefix += StaticItems.GlobalData.CourseAbbreviation;
            var path = Wrap.PathFromRoot.Skip(1).ToArray();
            for (int levelNumber = 0; levelNumber < path.Length; levelNumber++)
            {

                TopicLevel level = new TopicLevel();
                if (levelNumber < StaticItems.GlobalData.TopicLevels.Count)
                    level = StaticItems.GlobalData.TopicLevels[levelNumber];
                prefix += "-";
                prefix += string.Format("{0:D" + level.Digits + "}", path[levelNumber].NumberInTopic + 1);

                var topic = path[levelNumber] as FolderOrLectureItem;
                if (topic != null)
                {
                    description += level.Caption + " " + (path[levelNumber].NumberInTopic + 1) + ". " + topic.Topic.Caption + "\r\n";
                }
            }

            dueTitle = prefix + " " + Wrap.Video.Name;
            description += StaticItems.GlobalData.DescriptionPS;
            dueDescription = description;
        }

        public YoutubeVideoBlockModel(VideoWrap wrap)
        {
            Wrap = wrap;
            InitializeDueNames();
            Push = new RelayCommand(CmPush, () => YoutubeClip != null);
            View = new RelayCommand(CmGo, () => YoutubeClip != null);
        }
        

        public VideoWrap Wrap { get; private set; }

        public YoutubeClip YoutubeClip { get { return Wrap.Get<YoutubeClip>(); } }

        public Brush StatusColor
        {
            get
            {
                if (YoutubeClip == null) return Brushes.Red;
                if (YoutubeClip.Name != dueTitle || YoutubeClip.Description != dueDescription)
                    return Brushes.Yellow;
                return Brushes.Green;
            }
        }
     
        public RelayCommand Push { get; private set; }

        public RelayCommand View { get; private set; }

        public void CmPush()
        {
            var YoutubeClip = Wrap.Get<YoutubeClip>();
            if (YoutubeClip == null) return;
            var clip = new YoutubeClip { Id = YoutubeClip.Id };
            clip.Name = dueTitle;
            clip.Description = dueDescription;
            StaticItems.YoutubeProcessor.UpdateVideo(clip);
            YoutubeClip = clip;
        }

        public void CmGo()
        {
            if (YoutubeClip==null) return;
            Process.Start(YoutubeClip.VideoURLFull);
        }
    }
}

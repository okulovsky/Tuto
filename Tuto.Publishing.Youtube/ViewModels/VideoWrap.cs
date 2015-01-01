using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuto.Model;
using Tuto.Navigator;
using Tuto.Publishing.YoutubeData;

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

    public class VideoWrap : VideoItem
    {

        public string VideoURLFull { get { if (YoutubeClip == null) return ""; return YoutubeClip.VideoURLFull; } }
        public string VideoURLShort { get { if (YoutubeClip == null) return ""; return YoutubeClip.Id; } }
        public string YoutubeName { get { if (YoutubeClip == null) return ""; return YoutubeClip.Name; } }
        public IYoutubeProcessor Processor { get { return Get<IYoutubeProcessor>(); } set { Store<IYoutubeProcessor>(value); } } 
        public VideoWrap()
        {
            UpdateVideoCommand = new RelayCommand(UpdateVideo, ()=>YoutubeClip!=null);
        }

        public RelayCommand UpdateVideoCommand { get; private set; }

        public void UpdateVideo()
        {
            if (YoutubeClip == null) return;
            var prefix = "";
            var description = "";
            prefix += GlobalData.CourseAbbreviation;
            var path = PathFromRoot.Skip(1).ToArray();
            for (int levelNumber = 0; levelNumber < path.Length; levelNumber++)
            {

                TopicLevel level = new TopicLevel();
                if (levelNumber < GlobalData.TopicLevels.Count)
                    level = GlobalData.TopicLevels[levelNumber];
                prefix += "-";
                prefix += string.Format("{0:D" + level.Digits + "}", path[levelNumber].NumberInTopic+1);

                var topic = path[levelNumber] as FolderOrLectureItem;
                if (topic != null)
                {
                    description += level.Caption + " " + (path[levelNumber].NumberInTopic+1) + ". " + topic.Topic.Caption + "\r\n";
                }
            }

            var title = prefix + " " + this.Video.Name;
            description += GlobalData.DescriptionPS;
            var clip = new YoutubeClip { Id = YoutubeClip.Id };
            clip.Name = title;
            clip.Description = description;
            Processor.UpdateVideo(clip);
            YoutubeClip = clip;
        }

       
        public YoutubeClip YoutubeClip
        {
            get { return Get<YoutubeClip>(); }
            set
            {
                Store<YoutubeClip>(value);
                NotifyPropertyChanged();
                this.NotifyByExpression(z => z.YoutubeName);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Model
{
    [DataContract]
    public class GlobalData
    {
        [DataMember]
        public VoiceSettings VoiceSettings { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string TitleImage { get; set; }

        [DataMember]
        public string WatermarkImage { get; set; }

        [DataMember]
        public List<FinishedVideo> VideoData { get; internal set; }

        [DataMember]
        public Topic TopicsRoot { get; internal set; }

        [DataMember]
        public List<TopicLevel> TopicLevels { get; internal set; }

        [DataMember]
        public string CourseAbbreviation { get; set; }

        [DataMember]
        public string Keywords { get; set; }

        public DirectoryInfo GlobalDataFolder { get; internal set; }

        public GlobalLocations Locations { get; private set; }

        internal void AfterLoad(DirectoryInfo location)
        {
            GlobalDataFolder = location;
            Locations = new GlobalLocations(this);
        }

        public GlobalData()
        {
            VoiceSettings = new VoiceSettings();
            TopicsRoot = new Topic();
            VideoData = new List<FinishedVideo>();
            Name = "";
            Locations = new GlobalLocations(this);
            TopicLevels = new List<TopicLevel>();
        }
    }
}

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
		public const string VideoListName = "VideoSummaries.txt";

        [DataMember]
        public VoiceSettings VoiceSettings { get; set; }

        #region Decorations of the video
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string TitleImage { get; set; }

        [DataMember]
        public string WatermarkImage { get; set; }

        #endregion

        #region Catalog 
        [DataMember]
        public List<FinishedVideo> VideoData { get; set; }

        [DataMember]
        public Topic TopicsRoot { get; set; }

        [DataMember]
        public List<TopicLevel> TopicLevels { get; internal set; }

        #endregion

        #region Publishing options

        [DataMember]
        public string CourseAbbreviation { get; set; }

        [DataMember]
        public string Keywords { get; set; }

        [DataMember]
        public string DescriptionPS { get; set; }

        #endregion

		[DataMember]
		public string RelativeVideoListPath { get; set; }

        [DataMember]
        public WorkSettings WorkSettings { get; set; }

        [DataMember]
        public bool CrossFadesEnabled { get; set; }

        [DataMember]
        public bool ShowProcesses { get; set; }

        public DirectoryInfo GlobalDataFolder { get; internal set; }

        public GlobalLocations Locations { get; private set; }

        public void AfterLoad(DirectoryInfo location)
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
            WorkSettings = new WorkSettings();
        }
    }
}

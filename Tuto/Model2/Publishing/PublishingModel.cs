using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Tuto.Model;

namespace Tuto.Publishing
{
	[DataContract]
	public class PublishingModel
	{
		[DataMember]
		public PublishingSettings Settings { get; set; }
		[DataMember]
		public List<VideoPublishSummary> Videos { get; set; }
        [DataMember]
        public CourseStructure CourseStructure { get; set; }
        [DataMember]
        public DataLayer<YoutubeClip> YoutubeClipData { get; set; }
        [DataMember]
        public DataLayer<YoutubePlaylist> YoutubePlaylistData { get; set; }

		public List<VideoPublishSummary> NonDistributedVideos { get; set; }
		public Videotheque Videotheque { get; set; }
		public FileInfo Location { get; set; }

		public PublishingModel()
		{
			Settings = new PublishingSettings();
			Videos = new List<VideoPublishSummary>();
			CourseStructure = new CourseStructure();
            YoutubeClipData = new DataLayer<YoutubeClip>();
            YoutubePlaylistData = new DataLayer<YoutubePlaylist>();
		}

       

		public void Save()
		{
            foreach(var e in Videotheque.Episodes)
            {
                var clip = YoutubeClipData.Records.Where(z => z.Guid == e.Item2.Guid).FirstOrDefault();
                if (clip == null) continue;
                if (clip.Data.Id == e.Item2.YoutubeId) continue;
                e.Item2.YoutubeId = clip.Data.Id;
                e.Item1.Save();
            }
			HeadedJsonFormat.Write(Location, this);
		}
	}
}

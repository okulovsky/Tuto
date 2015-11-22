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

		public List<VideoPublishSummary> NonDistributedVideos { get; set; }
		public Videotheque Videotheque { get; set; }
		public FileInfo Location { get; set; }

		public PublishingModel()
		{
			Settings = new PublishingSettings();
			Videos = new List<VideoPublishSummary>();
			CourseStructure = new CourseStructure();
		}

		public void Save()
		{
			HeadedJsonFormat.Write(Location, this);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Publishing
{
	[DataContract]
    public class YoutubeClip
    {
		[DataMember]
        public string Id { get; set; }
		[DataMember]
		public string Name { get; set; }
		[DataMember]
		public string Description { get; set; }
		public string VideoURLFull { get { return "http://youtube.com/watch?v=" + Id; } }
		public string GDataURL { get { return "http://gdata.youtube.com/feeds/api/videos/" + Id; } }
		[DataMember]
		public Guid? StoredGuid { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Model
{
    [DataContract]
    public class EpisodInfo
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public Guid TopicId { get; set; }
        [DataMember]
        public Guid AuthorId { get; set; }
        [DataMember]
        public TimeSpan Duration { get; set; }
        [DataMember]
        public int NumberInTopic { get; set; }
    }
}

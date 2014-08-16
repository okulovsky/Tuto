using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Model
{
    [DataContract]
    public class FinishedVideo
    {
        [DataMember]
        public Guid Guid { get; private set; }

        [DataMember]
        public Guid TopicGuid { get; set; }

        [DataMember]
        public int NumberInTopic { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public TimeSpan Duration { get; set; }

        public FinishedVideo(EpisodInfo info)
        {
            Guid = info.Guid;
            Name = info.Name;
            Duration = info.Duration;
        }
    }
}

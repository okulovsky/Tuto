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
        public Guid Guid { get; internal set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public TimeSpan Duration { get; set; }

        public EpisodInfo(Guid guid)
        {
            this.Guid = guid;
        }
    }
}

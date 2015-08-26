using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Model
{
    public enum OutputTypes { Patch, Output, None};
    [DataContract]
    public class EpisodInfo
    {
        [DataMember]
        public Guid Guid { get; internal set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string YoutubeId { get; set; }

        [DataMember]
        public string ItemPlaylistId { get; set; }

        [DataMember]
        public string PlaylistId { get; set; }

        [DataMember]
        public PatchModel PatchModel { get; set; }

        [DataMember]
        public OutputTypes OutputType { get; set; }

        [DataMember]
        public bool Dirty { get; set; }

        [DataMember]
        public TimeSpan Duration { get; set; }

        public EpisodInfo(Guid guid)
        {
            this.Guid = guid;
        }
    }
}

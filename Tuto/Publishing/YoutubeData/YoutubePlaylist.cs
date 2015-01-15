using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Publishing
{
    [DataContract]
    public class YoutubePlaylist
    {
        [DataMember]
        public string PlaylistId { get; set; }
        [DataMember]
        public string PlaylistTitle { get; set; }
    }


  
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Publishing.Youtube
{
    [DataContract]
    public class YoutubeSettings
    {
        [DataMember]
        public string DeveloperKey { get; set; }
        [DataMember]
        public string Username { get; set; }
        [DataMember]
        public string ChannelId { get; set; }
        [DataMember]
        public string ChannelUserId { get; set; }

    }
}

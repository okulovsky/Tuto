using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Publishing.Youtube.Model
{
    [DataContract]
    public class PublishData
    {
        [DataMember]
        public string DeveloperKey { get; private set; }

        [DataMember]
        public string Username { get; private set; }

        [DataMember]
        public string ChannelId { get; private set; }

        [DataMember]
        public List<PublishedVideo> Videos { get; private set; }
        
    }
}

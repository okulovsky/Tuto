using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Tuto.Model;

namespace Tuto.Publishing.Youtube
{
    [DataContract]
    public class PublishedVideo
    {
        [DataMember]
        public Guid Guid { get; set; }
        [DataMember]
        public string ClipId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Model
{
    [DataContract]
    public class TopicLevel
    {
        [DataMember]
        public string Caption { get; set; }
        [DataMember]
        public int Digits { get; set; }
    }
}

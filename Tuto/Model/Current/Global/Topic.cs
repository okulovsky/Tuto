using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Model
{
    [DataContract]
    public class Topic
    {
        [DataMember]
        public Guid Guid { get; private set; }
        [DataMember]
        public string Caption { get; set; }
        [DataMember]
        public List<Topic> Items { get; private set; }


        public Topic()
        {
            Guid = Guid.NewGuid();
            Caption = "[Новый раздел]";
            Items = new List<Topic>();
        }
    }
}

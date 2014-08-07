using System;
using System.Collections.Generic;
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

        public Topic Parent { get; private set; }

        public void Load()
        {
            foreach (var e in Items)
            {
                e.Parent = this;
                e.Load();
            }
        }

        public Topic()
        {
            Guid = Guid.NewGuid();
            Caption = "[Новый раздел]";
            Items = new List<Topic>();
        }
    }
}

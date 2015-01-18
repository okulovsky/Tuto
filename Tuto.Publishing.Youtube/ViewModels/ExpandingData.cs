using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Publishing
{
    [DataContract]
    public class ExpandingData
    {
        [DataMember]
        public bool IsExpanded { get; set; }
    }
}

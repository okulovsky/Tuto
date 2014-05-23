using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Model
{
    [DataContract]
    public class FileContainer
    {
        [DataMember]
        public MontageModel MontageModel { get; set; }

        [DataMember]
        public WindowState WindowState { get; set; }

        [DataMember]
        public int Version { get; set; }

        public FileContainer()
        {
            Version = 1;
        }
    }
}

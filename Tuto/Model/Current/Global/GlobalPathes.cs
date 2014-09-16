using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Model
{
    [DataContract]
    public class ExternalPathes
    {
        [DataMember]
        public string FFMPEGPath { get; set; }

        [DataMember]
        public string VSFilterLibraryPath { get; set; }
    }
}

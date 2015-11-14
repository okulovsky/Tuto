using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Model2
{
    [DataContract]
    public class PathsSettings
    {
        [DataMember]
        public string RawPath { get; set; }
        [DataMember]
        public string OutputPath { get; set; }
        [DataMember]
        public string ModelPath { get; set; }
        [DataMember]
        public string TempPath { get; set; }
    }
}

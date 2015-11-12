using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Model
{
    [DataContract]
    public class PathesSettings
    {
        [DataMember]
        public string InputPath { get; set; }
        [DataMember]
        public string OutputPath { get; set; }
        [DataMember]
        public string ModelPath { get; set; }
        [DataMember]
        public string TempPath { get; set; }
    }
}

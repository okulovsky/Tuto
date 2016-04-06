using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Model
{
    [DataContract]
    public class Patch
    {
        [DataMember]
        public int Begin { get; set; }
        [DataMember]
        public int End { get; set; }
        [DataMember]
        public SubtitlePatch Subtitles { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Model
{
    [DataContract]
    public class SubtitleFix : IComparable
    {
        [DataMember]
        public int StartTime { get; set; }
        [DataMember]
        public int Length { get; set; }
        [DataMember]
        public string Text { get; set; }

        public int CompareTo(object obj)
        {
            return StartTime.CompareTo((obj as SubtitleFix).StartTime);
        }
    }
}

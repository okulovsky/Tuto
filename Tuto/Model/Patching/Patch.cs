using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Model
{
    public enum PatchType
    {
        Empty,
        Subtitles,
        Video
    }

    [DataContract]
    public class Patch
    {
        [DataMember]
        public int Begin { get; set; }
        [DataMember]
        public int End { get; set; }
        [DataMember]
        PatchBucket Bucket { get; set; }

        public PatchData Data { get { if (Bucket == null) return null; return Bucket.Data; } set { Bucket.Data = value; } }

        public bool IsVideoPatch { get { return Data is VideoPatchBase; } }
        public VideoPatchBase VideoData { get { return Data as VideoPatchBase; } }

        public Patch()
        {
            Bucket = new PatchBucket();
        }


    }
}

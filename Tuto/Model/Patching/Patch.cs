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

        public bool IsVideoPatch { get { return Data is VideoPatch; } }
        public VideoPatch VideoData { get { return Data as VideoPatch; } }

        public Patch()
        {
            Bucket = new PatchBucket();
        }


        public PatchType Type
        {
            get
            {
                if (Data == null) return PatchType.Empty;
                if (Data is SubtitlePatch) return PatchType.Subtitles;
                if (Data is VideoPatch) return PatchType.Video;
                return PatchType.Empty;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Model
{
    [DataContract]
    class PatchBucket
    {
        [DataMember]
        SubtitlePatch Subtitles;

        [DataMember]
        VideoPatch Video;

        public PatchData Data
        {
            get
            {
                if (Subtitles != null) return Subtitles;
                if (Video != null) return Video;
                return null;
            }
            set
            {
                Subtitles = value as SubtitlePatch;
                Video = value as VideoPatch;
            }
        }
    }
}

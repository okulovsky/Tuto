using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Model
{
    public enum VideoPatchOverlayType
    {
        Replace,
        KeepSoundTruncateVideo,
        KeepSoundAddSilence
    }

    [DataContract]
    public class VideoPatch : PatchData
    {
        [DataMember]
        public string RelativeFileName { get; set; }
        [DataMember]
        public int Duration { get; set; }
        [DataMember]
        public VideoPatchOverlayType OverlayType { get; set; }
    }
}

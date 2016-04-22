using System;
using System.Collections.Generic;
using System.IO;
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
    public abstract class VideoPatchBase : PatchData
    {

        public abstract FileInfo GetFileName(Videotheque v);
        [DataMember]
        public int Duration { get; set; }
        [DataMember]
        public VideoPatchOverlayType OverlayType { get; set; }
    }

    [DataContract]
    public class VideoFilePatch : VideoPatchBase
    {
        [DataMember]
        public string RelativeFileName { get; set; }

        public override FileInfo GetFileName(Videotheque v)
        {
            return new FileInfo(Path.Combine(v.PatchFolder.FullName, RelativeFileName));
        } 
    }


    [DataContract]
    public class TutoPatch : VideoPatchBase
    {
        [DataMember]
        public Guid Guid { get; set; }

        public override FileInfo GetFileName(Videotheque v)
        {
            throw new NotImplementedException();
        }
    }
}

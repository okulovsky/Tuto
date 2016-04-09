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
        VideoFilePatch Video;

        [DataMember]
        ImagePatch Image;

        [DataMember]
        TutoPatch Tuto;

        public PatchData Data
        {
            get
            {
                if (Subtitles != null) return Subtitles;
                if (Video != null) return Video;
                if (Image != null) return Image;
                if (Tuto != null) return Tuto;
                return null;
            }
            set
            {
                Subtitles = value as SubtitlePatch;
                Video = value as VideoFilePatch;
                Image = value as ImagePatch;
                Tuto = value as TutoPatch;
            }
        }
    }
}

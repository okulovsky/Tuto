using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Model
{
    [DataContract]
    public class GlobalData
    {
        [DataMember]
        public VoiceSettings VoiceSettings { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public FileInfo TitleImage { get; set; }

        [DataMember]
        public FileInfo WatermarkImage { get; set; }

        public GlobalData()
        {
            VoiceSettings = new VoiceSettings();
            Name = "";
        }
    }
}

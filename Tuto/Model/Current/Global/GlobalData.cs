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
        public string TitleImage { get; set; }

        [DataMember]
        public string WatermarkImage { get; set; }

        [DataMember]
        public List<FinishedVideo> VideoData { get; internal set; }

        [DataMember]
        public Topic TopicsRoot { get; internal set; }

        public GlobalData()
        {
            VoiceSettings = new VoiceSettings();
            TopicsRoot = new Topic();
            VideoData = new List<FinishedVideo>();
            Name = "";
        }
    }
}

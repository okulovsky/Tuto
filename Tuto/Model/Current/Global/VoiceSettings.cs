using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Model
{
    [DataContract]
    public class VoiceSettings
    {
        [DataMember]
        public int MaxDistanceToSilence { get; set; }
        [DataMember]
        public int SilenceMargin { get; set; }

        [DataMember]
        public int Test { get; set; }

        public VoiceSettings()
        {
            MaxDistanceToSilence = 1000;
            SilenceMargin = 300;
            Test = 5;
        }
    }
}

using System;
using System.Collections.Generic;
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

        public GlobalData()
        {
            VoiceSettings = new VoiceSettings();
        }
    }
}

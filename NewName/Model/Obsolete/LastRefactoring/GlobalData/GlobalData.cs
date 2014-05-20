using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    public class GlobalDataV4
    {
        public VoiceSettingsV4 VoiceSettings { get; set; }

        public GlobalDataV4()
        {
            VoiceSettings = new VoiceSettingsV4();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    public class VoiceSettingsV4
    {
        public int MaxDistanceToSilence { get; set; }
        public int SilenceMargin { get; set; }

        public VoiceSettingsV4()
        {
            MaxDistanceToSilence = 1000;
            SilenceMargin = 300;
        }
    }
}

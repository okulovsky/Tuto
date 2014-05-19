using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    public class VoiceSettings
    {
        public int MaxDistanceToSilence { get; set; }
        public int SilenceMargin { get; set; }

        public VoiceSettings()
        {
            MaxDistanceToSilence = 1000;
            SilenceMargin = 300;
        }
    }
}

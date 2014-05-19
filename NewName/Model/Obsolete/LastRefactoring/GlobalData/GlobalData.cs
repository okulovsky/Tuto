using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    public class GlobalData
    {
        public VoiceSettings VoiceSettings { get; set; }

        public GlobalData()
        {
            VoiceSettings = new VoiceSettings();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Navigator.Initialization
{
    public class SelfTestJob
    {
        public string Description { get; set; }
        public string[] Patches { get; set; }
        public RecoveryPrompt RecoveryPrompt { get; set; }
    }
}

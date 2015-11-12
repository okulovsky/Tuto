using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Navigator.Initialization
{
    public class RecoveryPrompt
    {
        public string Prompt { get; set; }
        public string DefaultOption { get; set; }
        public string BrowseOption { get; set; }
        public string BrowseFileName { get; set; }
        public bool FileIsRequired { get; set; }
    }
}

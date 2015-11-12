using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Model
{
    [DataContract]
    public class VideothequeEnvironmentSettings
    {
        [DataMember]
        public string FFMPEGPath { get; set; }
        [DataMember]
        public string SoxPath { get; set; }        
        [DataMember]
        public List<string> LastLoadedProjects { get; set; }
    }
}

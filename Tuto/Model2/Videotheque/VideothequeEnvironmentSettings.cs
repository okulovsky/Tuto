using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Model
{
    [DataContract]
    public class VideothequeStartupSettings
    {
        [DataMember]
        public string FFMPEGPath { get; set; }
        [DataMember]
        public string SoxPath { get; set; }        
        [DataMember]
        public List<string> LastLoadedProjects { get; set; }

		public VideothequeStartupSettings()
		{
			LastLoadedProjects = new List<string>();
		}
    }
}

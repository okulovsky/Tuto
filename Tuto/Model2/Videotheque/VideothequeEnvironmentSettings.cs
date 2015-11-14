using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Model2
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

		public VideothequeEnvironmentSettings()
		{
			LastLoadedProjects = new List<string>();
		}
    }
}

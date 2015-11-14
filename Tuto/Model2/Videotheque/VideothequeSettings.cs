using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Tuto.Model;

namespace Tuto.Model2
{
    [DataContract]
    public class VideothequeSettings
    {
        [DataMember]
        public VoiceSettings VoiceSettings { get; set; }
        [DataMember]
        public WorkSettings WorkSettings { get; set; }
		//[DataMember]
		//public VideoSettings VideoSettings { get; set;} 
        [DataMember]
        public PathsSettings PathsSettings { get; set; }

        #region other settings. This code should be placed to some subclass

        [Obsolete]
        [DataMember]
        public bool ShowProcesses { get; set; }

        #endregion

        public VideothequeSettings()
		{
			VoiceSettings = new VoiceSettings();
			WorkSettings = new WorkSettings();
			PathsSettings = new PathsSettings();
		}
    }
}

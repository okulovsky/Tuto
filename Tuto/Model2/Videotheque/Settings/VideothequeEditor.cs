using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Model
{
    [DataContract]
    public class VideothequeEditorSettings
    {
        [DataMember]
        public double DefaultFinalAcceleration { get; set; }

        [DataMember]
        public bool CrossFadesEnabled { get; set; }
        

        public VideothequeEditorSettings()
        {
            DefaultFinalAcceleration = 1;
        }
    }
}

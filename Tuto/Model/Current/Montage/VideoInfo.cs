using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Model
{
    /// <summary>
    /// The additional information about the video
    /// </summary>
    [DataContract]
    public class VideoInformation
    {

        

        /// <summary>
        /// Textual description of the episodes inside the video
        /// </summary>
        [DataMember]
        public List<EpisodInfo> Episodes { get;  private set; }
        public VideoInformation()
        {
            Episodes = new List<EpisodInfo>();
        }

    }
}

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

        [DataMember]
        long CreationTimeTicks { get; set; }

        [DataMember]
        long LastModificationTimeTicks { get; set; }

        public DateTime CreationTime
        {
            get
            {
                return new DateTime(CreationTimeTicks);
            }
            set
            {
                CreationTimeTicks = value.Ticks;
            }
        }

        public DateTime LastModificationTime
        {
            get
            {
                return new DateTime(LastModificationTimeTicks);
            }
            set
            {
                LastModificationTimeTicks = value.Ticks;
            }
        }


      

        public VideoInformation()
        {
            Episodes = new List<EpisodInfo>();
        }

    }
}

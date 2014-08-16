using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Model
{
    [DataContract]
    public class FinishedVideoData
    {
        [DataMember]
        public Guid Guid { get; private set; }

        [DataMember]
        public Guid TopicGuid { get; set; }

        [DataMember]
        public int NumberInTopic { get; set; }

        [DataMember]
        public string Name { get; private set; }

        [DataMember]
        public TimeSpan Duration { get; private set; }

        [DataMember]
        public string RelativeFileLocation { get; private set; }

        [DataMember]
        public string RelativeSourceFolderLocation { get; private set; }

        [DataMember]
        public int EpisodeNumber { get; private set; }


        internal void Load(EditorModel model, int episodeNumber)
        {
            Name = model.Montage.Information.Episodes[episodeNumber].Name;
            Duration = model.Montage.Information.Episodes[episodeNumber].Duration;
            RelativeFileLocation = model.Global.Locations.RelativeToGlobal(model.Locations.GetOutputFile(episodeNumber).FullName);
            RelativeSourceFolderLocation = model.Global.Locations.RelativeToGlobal(model.Locations.LocalFilePath.Directory.FullName);
            EpisodeNumber = episodeNumber;
        }


        public FinishedVideoData(EditorModel model, int episodeNumber)
        {
            Guid = model.Montage.Information.Episodes[episodeNumber].Guid;
            Load(model, episodeNumber);
        }
    }
}

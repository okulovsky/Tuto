using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Model
{
    [DataContract]
    public class FinishedVideo
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

        [DataMember]
        public string FullName { get; private set; }

        [DataMember]
        public string PatchedName { get; private set; }

        [DataMember]
        public string YoutubeId { get; set; }

        [DataMember]
        public string ItemPlaylistId { get; set; }

        [DataMember]
        public string PlaylistPosition { get; set; }

        [DataMember]
        public string PlaylistId { get; set; }

        internal void Load(EditorModel model, int episodeNumber)
        {
            Name = model.Montage.Information.Episodes[episodeNumber].Name;
            Duration = model.Montage.Information.Episodes[episodeNumber].Duration;
            RelativeFileLocation = model.Global.Locations.RelativeToGlobal(model.Locations.GetOutputFile(episodeNumber).FullName);
            RelativeSourceFolderLocation = model.Global.Locations.RelativeToGlobal(model.Locations.LocalFilePath.Directory.FullName);
            EpisodeNumber = episodeNumber;
            FullName = model.Locations.GetOutputFile(episodeNumber).FullName;
            PatchedName = model.Locations.GetSuffixedName(new FileInfo(FullName), "-patched").FullName;
        }


        public FinishedVideo(EditorModel model, int episodeNumber)
        {
            Guid = model.Montage.Information.Episodes[episodeNumber].Guid;
            FullName = model.Locations.GetOutputFile(episodeNumber).FullName;
            PatchedName = model.Locations.GetSuffixedName(new FileInfo(FullName), "-patched").FullName;
            Load(model, episodeNumber);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}

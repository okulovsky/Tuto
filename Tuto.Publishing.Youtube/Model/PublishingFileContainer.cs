using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Tuto.Model;

namespace Tuto.Publishing.Youtube
{
    [DataContract]
    public class PublishingFileContainer
    {
        [DataMember]
        public YoutubeSettings Settings { get;  set; }
        [DataMember]
        public List<PublishedVideo> Videos { get;  private set; }

        public PublishingFileContainer()
        {
            Settings = new YoutubeSettings();
            Videos = new List<PublishedVideo>();
        }

        const string fileName = "publishing.tuto";
        const string header = "Tuto publishing data";

        public static PublishingFileContainer Load(DirectoryInfo folder)
        {
            var file = new FileInfo(Path.Combine(folder.FullName, fileName));
            if (!file.Exists)
            {
                var result = new PublishingFileContainer();
                result.Save(folder);
                return result;
            }
            return HeadedJsonFormat.Read<PublishingFileContainer>(file, header, 0);
        }

        public void Save(DirectoryInfo folder)
        {
            var file = new FileInfo(Path.Combine(folder.FullName, fileName));
            HeadedJsonFormat.Write(file, header, 0, this);
        }
    }
}

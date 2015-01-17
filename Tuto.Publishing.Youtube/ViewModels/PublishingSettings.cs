using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Tuto.Model;
using Tuto.Navigator;

namespace Tuto.Publishing
{
    [DataContract]
   public  class PublishingSettings
    {
        [DataMember]
        public string CourseAbbreviation { get; set; }
        [DataMember]
        public string Keywords { get; set; }
        [DataMember]
        public string DescriptionPS { get; set; }
        [DataMember]
        public List<TopicLevel> TopicLevels { get; private set; }
        [DataMember]
        public string LatexSourceSubdirectory { get;  set; }
        [DataMember]
        public string LatexCompiledSlidesSubdirectory { get;  set; }

        public RelayCommand SaveCommand { get; private set; }

        public DirectoryInfo Location { get; set; }
        
        void Save()
        {
            HeadedJsonFormat.Write(Location, this);
        }

        public PublishingSettings()
        {
            TopicLevels = new List<TopicLevel>();
            SaveCommand = new RelayCommand(Save);
            LatexSourceSubdirectory = "Latex";
            LatexCompiledSlidesSubdirectory = "LatexCompiledSlides";
            CourseAbbreviation = "";
            Keywords = "";
            DescriptionPS = "";
        }
    }
}

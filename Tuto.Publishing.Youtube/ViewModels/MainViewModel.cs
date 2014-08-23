using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuto.Model;
using Tuto.Navigator;

namespace Tuto.Publishing.Youtube
{
    public class MainViewModel
    {
        public DirectoryInfo Directory { get; private set; }
        public YoutubeSettings Settings { get; private set; }
        public TopicWrap[] Root { get; private set; }
        public GlobalData GlobalData { get; private set; }
        YoutubeProcessor Processor;
        public MainViewModel(DirectoryInfo directory, GlobalData data, YoutubeSettings settings, TopicWrap root, YoutubeProcessor processor)
        {
            Directory = directory;
            Processor = processor;
            GlobalData = data;
            Settings = settings;
            Root = new[] { root };
            MakeDescriptionsCommand = new RelayCommand(MakeDescriptions);
            SaveCommand = new RelayCommand(Save);
        }


        public RelayCommand MakeDescriptionsCommand { get; private set; }
        public RelayCommand SaveCommand { get; private set; }

        void MakeDescriptions()
        {
            foreach (var e in Root[0].Subtree().OfType<VideoWrap>())
            {
                if (e.ClipData != null)
                {
                    e.ClipData.Name = Algorithms.GetAbbreviation(GlobalData, e);
                    e.ClipData.Description = Algorithms.GetDescription(GlobalData, e);
                    Processor.UpdateClipData( GlobalData, e.ClipData);
                }
            }

        }

        void Save()
        {
            var container = new PublishingFileContainer();
            container.Settings = Settings;
            container.Videos.AddRange(Root[0].Subtree().OfType<VideoWrap>().Select(z => z.Published).Where(z => z != null));
            container.Save(Directory);
        }
    }
}

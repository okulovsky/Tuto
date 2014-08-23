using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
        public List<FinishedVideo> FinishedNotMatched { get; private set; }
        public List<ClipData> YoutubeNotMatched { get; private set; }

        YoutubeProcessor Processor;
        public MainViewModel(DirectoryInfo directory,
            GlobalData data, 
            YoutubeSettings settings, 
            TopicWrap root, 
            YoutubeProcessor processor, 
            IEnumerable<VideoWrap> allVideos)
        {
            FinishedNotMatched = allVideos
                .Where(z => z.Status == Status.DeletedFromYoutube || z.Status == Status.NotFoundAtYoutube)
                .Select(z=>z.Finished)
                .ToList();
            YoutubeNotMatched = allVideos
                .Where(z => z.Status == Status.NotExpectedAtYoutube || z.Status == Status.DeletedFromTuto)
                .Select(z=>z.ClipData)
                .ToList();

            Directory = directory;
            Processor = processor;
            GlobalData = data;
            Settings = settings;
            Root = new[] { root };
            MakeDescriptionsCommand = new RelayCommand(MakeDescriptions);
            SaveCommand = new RelayCommand(Save);
            MakePlaylistsCommand = new RelayCommand(MakePlaylists);
        }


        public RelayCommand MakeDescriptionsCommand { get; private set; }
        public RelayCommand SaveCommand { get; private set; }
        public RelayCommand MakePlaylistsCommand { get; private set; }

        void MakePlaylists()
        {
            foreach(var e in Root[0].Subtree().OfType<TopicWrap>().Where(z=>z.PlaylistRequired) )
            {
                if (e.Playlist == null)
                {
                    e.Playlist = Processor.CreatePlaylist(e);
                    e.Published = new PublishedTopic { TopicGuid = e.Topic.Guid, PlaylistId=e.Playlist.Id};
                }
                Processor.UpdatePlaylist(e);
                break;
            }
        }

        void MakeDescriptions()
        {
            string errors = "";
            foreach (var e in Root[0].Subtree().OfType<VideoWrap>())
            {
                if (e.ClipData != null)
                {
                    try
                    {
                        e.ClipData.Name = Algorithms.GetAbbreviation(GlobalData, e);
                        e.ClipData.Description = Algorithms.GetDescription(GlobalData, e);
                        Processor.UpdateClipData(GlobalData, e.ClipData);
                    }
                    catch
                    {
                        errors += e.ClipData.Id + " " + e.Finished.Name+"\r\n";
                    }
                }
            }
            if (errors != "")
                MessageBox.Show(errors);

        }

        void Save()
        {
            var container = new PublishingFileContainer();
            container.Settings = Settings;
            container.Videos.AddRange(Root[0].Subtree().OfType<VideoWrap>().Select(z => z.Published).Where(z => z != null));
            container.Topics.AddRange(Root[0].Subtree().OfType<TopicWrap>().Select(z => z.Published).Where(z => z != null));
            container.Save(Directory);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Tuto;
using Tuto.Model;
using Tuto.Publishing.Youtube.Views;
using Tuto.Publishing.YoutubeData;

namespace Tuto.Publishing
{
    public class MainViewModel : NotifierModel
    {
        #region Properties
        DirectoryInfo directory;
        public DirectoryInfo Directory
        {
            get { return directory; }
            set { directory = value; NotifyPropertyChanged(); }
        }


        PublishingSettings settings;
        public PublishingSettings Settings
        {
            get { return settings; }
            set { settings = value; NotifyPropertyChanged(); }
        }
      

        public Item[] root;
        public Item[] Root
        {
            get { return root; }
            set { root = value; NotifyPropertyChanged(); }
        }


		public List<VideoPublishSummary> finishedNotMatched;

        public List<VideoPublishSummary> FinishedNotMatched
        {
            get { return finishedNotMatched; }
            set { finishedNotMatched = value; NotifyPropertyChanged(); }
        }

        public List<YoutubeClip> youtubeNotMatched;

        public List<YoutubeClip> YoutubeNotMatched
        {
            get { return youtubeNotMatched; }
            set { youtubeNotMatched = value; NotifyPropertyChanged(); }
        }
   
        #endregion

		List<IMaterialSource> sources;
        Func<IEnumerable<IMaterialSource>> sourcesFactory;

        public MainViewModel(DirectoryInfo directory, Func<IEnumerable<IMaterialSource>> sourcesFactory)
        {
            this.sourcesFactory = sourcesFactory;
            this.Directory = directory;
            UpdateVideoCommand = new RelayCommand(UpdateVideo);
			UpdateLatexCommand = new RelayCommand(UpdateLatex);
				 
            SaveCommand = new RelayCommand(Save);
            Reload();
        }


        public void Reload()
        {
			var globalData = CourseTreeData.Load(Directory);
            var root = ItemTreeBuilder.Build<FolderWrap, LectureWrap, VideoWrap>(globalData);
            Root = new[] { root };

            Settings= HeadedJsonFormat.Read<PublishingSettings>(Directory);
            if (Settings == null) Settings = new PublishingSettings();
            Settings.Location = Directory;

            sources = sourcesFactory().ToList();
            foreach (var source in sources)
                source.Initialize(Settings);

            Load();
			CreateCommandBlocks();
            DataBinding<IExpandingDataHolder>.PullFromFile<ExpandingData>(root, settings.Location);
            
        }

        public void Closing()
        {
            DataBinding<IExpandingDataHolder>.SaveLayer<ExpandingData>(Root[0], Settings.Location);
        }

		void Load()
		{
			foreach (var s in sources)
				s.Load(Root[0]);
		}


        void Assign<TItem>(Func<IMaterialSource,TItem,ICommandBlockModel> modelCreator)
            where TItem : ICommandBlocksHolder
        {
            foreach (var e in Root[0].Subtree().OfType<TItem>())
                foreach (var b in sources)
                {
                    var cmdBlock = modelCreator(b,e);
                    if (cmdBlock != null)
                        e.CommandBlocks.Add(cmdBlock);
                }
        }

        void CreateCommandBlocks()
        {
            Assign<LectureWrap>((source, wrap) => source.ForLecture(wrap));
            Assign<VideoWrap>((sources, wrap) => sources.ForVideo(wrap));
        }

		void UpdateVideo()
		{
			sources.OfType<YoutubeSource>().FirstOrDefault().Pull(Root[0]);
			Root = new[] { Root[0] };
		}

		void UpdateLatex()
		{
			sources.OfType<LatexSource>().FirstOrDefault().Pull(Root[0]);
			Root = new[] { Root[0] };
		}

        public void Save()
        {
			foreach (var s in sources)
				s.Save(Root[0]);
        }


		//void TestPlaylist()
		//{
		//	//var items = Root[0].Subtree().ToList();
		//	//var lecture = items.OfType<LectureWrap>().First();
		//	//YoutubeProcessor.CreatePlaylist(lecture);

		//	var item = Root[0].Subtree().OfType<VideoWrap>().First();
		//	//YoutubeProcessor.UpdateClipData(GlobalData, item.YoutubeClip);
		//}

        public RelayCommand SaveCommand { get; private set; }
		public RelayCommand UpdateVideoCommand { get; private set; }
		public RelayCommand UpdateLatexCommand { get; private set; }
		public RelayCommand TestCommand { get; private set; }



        //public RelayCommand MakeDescriptionsCommand { get; private set; }
        //public RelayCommand SaveCommand { get; private set; }
        //public RelayCommand MakePlaylistsCommand { get; private set; }
        //public RelayCommand ULearnExportCommand { get; private set; }

        //void MakePlaylists()
        //{
        //    foreach(var e in Root[0].Subtree().OfType<TopicWrap>().Where(z=>z.PlaylistRequired) )
        //    {
        //        if (e.Playlist == null)
        //        {
        //            e.Playlist = Processor.CreatePlaylist(e);
        //            e.Published = new PublishedTopic { TopicGuid = e.Topic.Guid, PlaylistId=e.Playlist.Id};
        //        }
        //        Processor.UpdatePlaylist(e);
        //        break;
        //    }
        //}

//        void UlearnExport()
//        {
//            var topic = SelectedWrap as TopicWrap;
//            if (topic == null) return;
//            if (!topic.Children.OfType<VideoWrap>().Any()) return;
//            var dirDialog = new FolderBrowserDialog();
//            dirDialog.SelectedPath=@"C:\Ulearn\src\Courses\BasicProgramming\Slides";
//            if (dirDialog.ShowDialog() != DialogResult.OK) return;
//            var directory=new DirectoryInfo(dirDialog.SelectedPath);
//            foreach (var e in topic.Children.OfType<VideoWrap>())
//            {
//                var fileTemplate =
//@"using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using uLearn; 
//
//namespace {0}
//{{
//	[Slide(""{1}"", ""{2}"")]
//	class S{3}_{4}
//	{{
//		//#video {5}
//		/*
//		## Заметки по лекции
//		*/
//    }}
//}}
//";
//                var nspace = directory.Name;
//                var title = e.Finished.Name;
//                var guid = Guid.NewGuid();
//                var numberStr = string.Format("{0:D3}", (e.Finished.NumberInTopic+1) * 10);
//                var className = title.Replace(' ', '_');
//                var video = e.ClipData.Id;
//                var content = string.Format(fileTemplate, nspace, title, guid, numberStr, className, video);
//                var fname = string.Format("S{0}_{1}.cs", numberStr, title);
//                File.WriteAllText(Path.Combine(directory.FullName, fname), content);
//            }
//            File.WriteAllText(Path.Combine(directory.FullName, "Title.txt"), topic.Topic.Caption);
//        }
   

        //void MakeDescriptions()
        //{
        //    string errors = "";
        //    foreach (var e in Root[0].Subtree().OfType<VideoWrap>())
        //    {
        //        if (e.ClipData != null)
        //        {
        //            try
        //            {
        //                e.ClipData.Name = Algorithms.GetAbbreviation(GlobalData, e);
        //                e.ClipData.Description = Algorithms.GetDescription(GlobalData, e);
        //                Processor.UpdateClipData(GlobalData, e.ClipData);
        //            }
        //            catch
        //            {
        //                errors += e.ClipData.Id + " " + e.Finished.Name+"\r\n";
        //            }
        //        }
        //    }
        //    if (errors != "")
        //        System.Windows.MessageBox.Show(errors);

        //}

        //void Save()
        //{
        //    var container = new PublishingFileContainer();
        //    container.Settings = Settings;
        //    container.Videos.AddRange(Root[0].Subtree().OfType<VideoWrap>().Select(z => z.Published).Where(z => z != null));
        //    container.Topics.AddRange(Root[0].Subtree().OfType<TopicWrap>().Select(z => z.Published).Where(z => z != null));
        //    container.Save(Directory);
        //}
    }
}

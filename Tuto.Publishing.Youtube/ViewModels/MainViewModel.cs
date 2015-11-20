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

		[Obsolete]
        public MainViewModel(DirectoryInfo directory, Func<IEnumerable<IMaterialSource>> sourcesFactory)
        {
            this.sourcesFactory = sourcesFactory;
            this.Directory = directory;
            UpdateVideoCommand = new RelayCommand(UpdateVideo);
			UpdateLatexCommand = new RelayCommand(UpdateLatex);
				 
            SaveCommand = new RelayCommand(Save);
            ReloadOld();
        }

		[Obsolete]
        public void ReloadOld()
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


        public RelayCommand SaveCommand { get; private set; }
		public RelayCommand UpdateVideoCommand { get; private set; }
		public RelayCommand UpdateLatexCommand { get; private set; }
		public RelayCommand TestCommand { get; private set; }

    }
}

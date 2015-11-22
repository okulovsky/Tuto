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
        public PublishingModel Model { get; private set; }
        public Videotheque Videotheque { get; private set; }

        public Item[] Root { get; private set; }

		List<IMaterialSource> sources;
        Func<IEnumerable<IMaterialSource>> sourcesFactory;

        public MainViewModel(Videotheque videotheque, PublishingModel model, Func<IEnumerable<IMaterialSource>> sourcesFactory)
        {
            this.sourcesFactory = sourcesFactory;
            UpdateVideoCommand = new RelayCommand(UpdateVideo);
            UpdateLatexCommand = new RelayCommand(UpdateLatex);

            SaveCommand = new RelayCommand(Save);
            Reload();
        }

        void Reload()
        {
            var data = new CourseTreeData
            {
                Structure = Model.CourseStructure,
                Videos = Model.Videos
            };
            var root = ItemTreeBuilder.Build<FolderWrap, LectureWrap, VideoWrap>(data);
            Root = new[] { root };
            base.NotifyAll();

            sources = sourcesFactory().ToList();
            foreach (var source in sources)
                source.Initialize(Model.Settings);

            Load();
            CreateCommandBlocks();
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

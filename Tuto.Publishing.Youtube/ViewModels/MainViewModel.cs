﻿using System;
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

namespace Tuto.Publishing
{
    public class MainViewModel : NotifierModel
    {
        public PublishingModel Model { get; private set; }
        public Videotheque Videotheque { get; private set; }

        public Item[] Root { get; private set; }

        public PublishingSettings Settings { get { return Model.Settings; } }

		List<IMaterialSource> sources;
        Func<IEnumerable<IMaterialSource>> sourcesFactory;

        public MainViewModel(Videotheque videotheque, PublishingModel model, Func<IEnumerable<IMaterialSource>> sourcesFactory)
        {
			this.Model = model;
            this.sourcesFactory = sourcesFactory;
            UpdateVideoCommand = new RelayCommand(UpdateVideo);
      
            SaveCommand = new RelayCommand(Save);
            Reload();
        }

        public void Reload()
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
                source.Initialize(Model);

            Load();
            CreateCommandBlocks();
        }


		

        public void Closing()
        {
            Save();
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


        public void Save()
        {
			foreach (var s in sources)
				s.Save(Root[0]);
            Model.Save();
        }


        public RelayCommand SaveCommand { get; private set; }
		public RelayCommand UpdateVideoCommand { get; private set; }
		public RelayCommand TestCommand { get; private set; }

    }
}

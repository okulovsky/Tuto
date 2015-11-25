using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Tuto.Model;
using Tuto.Publishing.YoutubeData;
using Tuto.Publishing.Youtube;
using System.Diagnostics;

namespace Tuto.Publishing
{
	public class YoutubeSource : IMaterialSource
	{
        public PublishingModel Model { get; private set; }
        public readonly IYoutubeProcessor YoutubeProcessor;
		public Matcher<VideoItem, YoutubeClip> LastMatch { get; private set; }

        public YoutubeSource()
        {
            YoutubeProcessor = new YoutubeApisProcessor();
        }

        public void Initialize(PublishingModel model)
		{
            this.Model= model;
			YoutubeProcessor.Authorize(model.Videotheque.TempFolder);
		}
		public void Load(Item root)
		{
            DataBinding<VideoItem>.PullFromLayer(root, Model.YoutubeClipData);
            DataBinding<LectureItem>.PullFromLayer(root, Model.YoutubePlaylistData);
		}

		

		public List<YoutubeClip> GetYoutubeClips()
		{
			List<YoutubeClip> clips = new List<YoutubeClip>();
			try
			{
				clips = YoutubeProcessor.GetAllClips();
				HeadedJsonFormat.Write(new DirectoryInfo(Environment.CurrentDirectory), clips);
				return clips;
			}
			catch (Exception e)
			{
				MessageBox.Show("Loading video from Youtube failed.");
				return null;
			}
		}


		void PullClips(Item root)
		{
			var clips = GetYoutubeClips();
			if (clips == null) return;

			var lectures = root.Subtree().OfType<VideoWrap>();

			var clipHandler = new Matching.MatchItemHandler<YoutubeClip>(z => z.GetProperName(), z => z.Name, z => Process.Start(z.VideoURLFull));
			var lectureHandler = new Matching.MatchItemHandler<VideoWrap>(z => z.Caption, z => z.Caption, z => { });
			var updaters = new Matching.MatchUpdater<VideoWrap, YoutubeClip>(
				(wrap, clip) => wrap.Store<YoutubeClip>(clip),
				(clip, wrap) => { clip.UpdateGuid(wrap.Guid); YoutubeProcessor.UpdateVideo(clip); },
				wrap => wrap.Store<YoutubeClip>(null),
				clip => { clip.UpdateGuid(null); YoutubeProcessor.UpdateVideo(clip); }
				);

			var handlers = new Matching.MatchHandlers<VideoWrap, YoutubeClip>(lectureHandler, clipHandler);

			var keys = new Matching.MatchKeySet<VideoWrap, YoutubeClip, Guid?, string>(
				wrap => wrap.Guid,
				wrap => { var c = wrap.Get<YoutubeClip>(); if (c != null) return c.Id; return null; },
				clip => clip.GetGuid(),
				clip => clip.Id,
				guid => !guid.HasValue,
				id => id == null
				);
			var allData = new Matching.MatchHandlersAndKeys<VideoWrap, YoutubeClip, Guid?, string>(handlers, keys, updaters);

			Matching.MatchingAlgorithm.RunStrongAlgorithm(lectures, clips, allData);

		}

		void PullPlaylists(Item root)
		{
			var playLists = YoutubeProcessor.GetAllPlaylists();
			var topics = root.Subtree().OfType<LectureWrap>().ToList();

			var playListHandler = new Matching.MatchItemHandler<YoutubePlaylist>(
				z => z.PlaylistTitle,
				z => z.PlaylistTitle,
				z => { }
				);

			var topicHandler = new Matching.MatchItemHandler<LectureWrap>(
				z => z.Caption,
				z => z.Caption,
				z => { }
				);

			var updaters = new Matching.MatchUpdater<LectureWrap,YoutubePlaylist>(
				(wrap,list)=>wrap.Store<YoutubePlaylist>(list),
				(list, wrap)=>{},
				wrap=>wrap.Store<YoutubePlaylist>(null),
				list=>{}
				);


			Matching.MatchingAlgorithm.RunWeakAlgorithm(
				topics,
				playLists,
				new Matching.MatchHandlers<LectureWrap, YoutubePlaylist>(topicHandler, playListHandler),
				updaters);
		}

		public void Pull(Item root)
		{
			PullClips(root);
			PullPlaylists(root);
            Save(root);
		}

		public void Save(Item root)
		{
            Model.YoutubeClipData = DataBinding<VideoItem>.GetLayer<YoutubeClip>(root);
            Model.YoutubePlaylistData = DataBinding<LectureItem>.GetLayer<YoutubePlaylist>(root);
		}

		public ICommandBlockModel ForVideo(VideoWrap wrap)
		{
			return new YoutubeVideoCommands(this,wrap);
		}

		public ICommandBlockModel ForLecture(LectureWrap wrap)
		{
            return new YoutubeLectureCommands(this,wrap);
		}
	}
}

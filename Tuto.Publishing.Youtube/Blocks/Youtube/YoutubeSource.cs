using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Tuto.Model;
using Tuto.Publishing.YoutubeData;
using YoutubeApiTest;

namespace Tuto.Publishing
{
	public class YoutubeSource : IMaterialSource
	{
        GlobalData globalData;
        readonly IYoutubeProcessor youtubeProcessor;
		DirectoryInfo directory;
		public Matcher<VideoItem, YoutubeClip> LastMatch { get; private set; }

        public YoutubeSource()
        {
            youtubeProcessor = new YoutubeApisProcessor();
        }

		public void Initialize(GlobalData data)
		{
            this.globalData = data;
			directory = data.GlobalDataFolder;
			youtubeProcessor.Authorize(directory);
		}
		public void Load(Item root)
		{
			YoutubeDataBinding.LoadYoutubeData(root, directory);
		}

		public void Pull(Item root)
		{
			List<YoutubeClip> clips = new List<YoutubeClip>();
			try
			{
				clips = youtubeProcessor.GetAllClips();
			}
			catch
			{
				MessageBox.Show("Loading video from Youtube failed.");
				return;
			}
			LastMatch = Matchers.Clips(clips);
			LastMatch.Push(root);

			var playlists = youtubeProcessor.GetAllPlaylists();
			var listMatcher = Matchers.Playlists(playlists);
			listMatcher.Push(root);

			//Root = new[] { Root[0] };
			//FinishedNotMatched = matcher.UnmatchedTreeItems.Select(z => z.Video).ToList();
			//YoutubeNotMatched = matcher.UnmatchedExternalDataItems.ToList();
		}

		public void Save(Item root)
		{
			YoutubeDataBinding.SaveYoutubeData(root, directory);
		}

		public ICommandBlockModel ForVideo(VideoWrap wrap)
		{
			return new YoutubeVideoCommands(wrap,globalData,youtubeProcessor);
		}

		public ICommandBlockModel ForLecture(LectureWrap wrap)
		{
            return new YoutubeLectureCommands(wrap, globalData, youtubeProcessor);
		}

		
	}
}

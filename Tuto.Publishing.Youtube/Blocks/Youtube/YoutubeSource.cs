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
		IYoutubeProcessor processor = new YoutubeApisProcessor();
		DirectoryInfo directory;
		public void Initialize(GlobalData data)
		{
			directory = data.GlobalDataFolder;
			processor.Authorize(directory);
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
				clips = processor.GetAllClips();
			}
			catch
			{
				MessageBox.Show("Loading video from Youtube failed.");
				return;
			}
			var matcher = Matchers.Clips(clips);
			matcher.Push(root);

			var playlists = StaticItems.YoutubeProcessor.GetAllPlaylists();
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
			return new YoutubeVideoCommands(wrap);
		}

		public ICommandBlockModel ForLecture(LectureWrap wrap)
		{
			return null;
		}

		
	}
}

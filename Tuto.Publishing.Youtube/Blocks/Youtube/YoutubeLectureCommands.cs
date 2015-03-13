using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Tuto.Model;
using Tuto;
using Tuto.Publishing.YoutubeData;

namespace Tuto.Publishing
{
	public class YoutubeLectureCommands : LectureCommandBlockModel<YoutubeSource,YoutubeVideoCommands>
	{
		public YoutubeLectureCommands(YoutubeSource source, LectureWrap wrap)
            : base (source,wrap)
        {
		
			InitializeDueNames();
			Commands.Add(new VisualCommand(new RelayCommand(CmGo), "view.png"));
			Commands.Add(new VisualCommand(new RelayCommand(CmPush), "upload.png"));
			Commands.Add(new VisualCommand(new RelayCommand(CmThumbnail), "thumbnail.png"));

		}

        public override string ImageFileName
        {
            get { return "youtube.png"; }
        }

		public override IEnumerable<BlockStatus> SelfErrors
		{
			get
			{
				if (YoutubePlaylist == null) 
					yield return BlockStatus.Warning("Playlist was not found at YouTube");
				else if (YoutubePlaylist.PlaylistTitle != dueTitle) 
					yield return BlockStatus.Warning("Playlist title do not match");
			}
		}
		
		string dueTitle;

		void InitializeDueNames()
		{
			var prefix = "";
			prefix += Source.Settings.CourseAbbreviation;
			var path = Wrap.PathFromRoot.Skip(1).ToArray();
			for (int levelNumber = 0; levelNumber < path.Length; levelNumber++)
			{

				TopicLevel level = new TopicLevel();
                if (levelNumber < Source.Settings.TopicLevels.Count)
                    level = Source.Settings.TopicLevels[levelNumber];
				prefix += "-";
				prefix += string.Format("{0:D" + level.Digits + "}", path[levelNumber].NumberInTopic + 1);
			}

			dueTitle = prefix + " " + Wrap.Topic.Caption;
		}


		public YoutubePlaylist YoutubePlaylist
		{
			get { return Wrap.Get<YoutubePlaylist>(); }
		}


		void UpdatePlaylist()
		{
			var playlist = YoutubePlaylist;
			if (playlist != null) Source.YoutubeProcessor.DeletePlaylist(playlist);

			playlist = Source.YoutubeProcessor.CreatePlaylist(dueTitle);
			Source.YoutubeProcessor.FillPlaylist(playlist, VideoData.Select(z => z.YoutubeClip).Where(z => z != null));


			Wrap.Store<YoutubePlaylist>(playlist);
			this.NotifyByExpression(z => z.Status);
		}

		public void CmPush()
		{
			UpdatePlaylist();

            foreach (var e in VideoData)
				e.CmPush();
		}

		public void CmThumbnail()
		{
			foreach (var e in VideoData)
				e.CmThumbnail();
		}

		public void CmGo()
		{
			if (YoutubePlaylist == null) return;
			Process.Start("https://www.youtube.com/playlist?list=" + YoutubePlaylist.PlaylistId);
		}

		public override void TryMakeItRight()
		{
			//if (Status.Status == Statuses.Warning && !Status.InheritedFromChildren)
				UpdatePlaylist();
		}
	}
}

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
		}

        public override string ImageFileName
        {
            get { return "youtube.png"; }
        }

		override public BlockStatus Status
		{
			get
			{
				if (VideoData.Any(z => z.Status.Status == Statuses.Error)) return BlockStatus.Error("One or more video have errors");
                if (VideoData.Any(z => z.Status.Status == Statuses.Warning)) return BlockStatus.Warning("One or more video have warnings");
                if (YoutubePlaylist == null) return BlockStatus.Warning("Playlist was not found at YouTube");
                if (YoutubePlaylist.PlaylistTitle != dueTitle) return BlockStatus.Warning("Playlist title do not match");
                return BlockStatus.OK();
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

	

		public void CmPush()
		{
			var playlist = YoutubePlaylist;
            if (playlist != null) Source.YoutubeProcessor.DeletePlaylist(playlist);

            playlist = Source.YoutubeProcessor.CreatePlaylist(dueTitle);
            Source.YoutubeProcessor.FillPlaylist(playlist, VideoData.Select(z => z.YoutubeClip).Where(z => z != null));

            foreach (var e in VideoData)
				e.CmPush();

			Wrap.Store<YoutubePlaylist>(playlist);
			this.NotifyByExpression(z => z.Status);

		}

		public void CmGo()
		{
			if (YoutubePlaylist == null) return;
			Process.Start("https://www.youtube.com/playlist?list=" + YoutubePlaylist.PlaylistId);
		}


	}
}

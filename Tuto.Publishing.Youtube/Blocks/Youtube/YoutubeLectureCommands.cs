using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Tuto.Model;
using Tuto.Navigator;
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

		override public Brush Status
		{
			get
			{
				if (VideoData.Any(z => z.Status == Brushes.Red)) return Brushes.Red;
				if (VideoData.Any(z => z.Status == Brushes.Yellow)) return Brushes.Yellow;
				if (YoutubePlaylist == null) return Brushes.Yellow;
				if (YoutubePlaylist.PlaylistTitle != dueTitle) return Brushes.Yellow;
				return Brushes.Green;
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
			if (playlist == null)
				playlist = Source.YoutubeProcessor.CreatePlaylist(Wrap.Topic.Caption);
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

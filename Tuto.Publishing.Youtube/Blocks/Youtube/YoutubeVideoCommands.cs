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

namespace Tuto.Publishing
{
	public class YoutubeVideoCommands : NotifierModel, ICommandBlockModel
	{
		public List<VisualCommand> Commands { get; private set; }

		public Uri ImageSource
		{
			get { return new Uri("/Img/youtube.png",UriKind.Relative); }
		}

		public Brush Status
		{
			get
			{
				if (YoutubeClip == null) return Brushes.Red;
				if (YoutubeClip.Name != dueTitle || YoutubeClip.Description != dueDescription)
					return Brushes.Yellow;
				return Brushes.Green;
			}
		}

		public YoutubeVideoCommands(VideoWrap wrap)
		{
			Wrap = wrap;
			InitializeDueNames();
			Commands = new List<VisualCommand>();
			Commands.Add(new VisualCommand(new RelayCommand(CmGo, () => YoutubeClip != null), "view.png"));
			Commands.Add(new VisualCommand(new RelayCommand(CmPush, () => YoutubeClip != null), "upload.png"));
		}

		#region Разборка с именами
		string dueTitle;
		string dueDescription;

		void InitializeDueNames()
		{
			var prefix = "";
			var description = "";
			prefix += StaticItems.GlobalData.CourseAbbreviation;
			var path = Wrap.PathFromRoot.Skip(1).ToArray();
			for (int levelNumber = 0; levelNumber < path.Length; levelNumber++)
			{

				TopicLevel level = new TopicLevel();
				if (levelNumber < StaticItems.GlobalData.TopicLevels.Count)
					level = StaticItems.GlobalData.TopicLevels[levelNumber];
				prefix += "-";
				prefix += string.Format("{0:D" + level.Digits + "}", path[levelNumber].NumberInTopic + 1);

				var topic = path[levelNumber] as FolderOrLectureItem;
				if (topic != null)
				{
					description += level.Caption + " " + (path[levelNumber].NumberInTopic + 1) + ". " + topic.Topic.Caption + "\r\n";
				}
			}

			dueTitle = prefix + " " + Wrap.Video.Name;
			description += StaticItems.GlobalData.DescriptionPS;
			dueDescription = description;
		}
		#endregion



		public VideoWrap Wrap { get; private set; }

		public YoutubeClip YoutubeClip { get { return Wrap.Get<YoutubeClip>(); } }

		public void CmPush()
		{
			var YoutubeClip = Wrap.Get<YoutubeClip>();
			if (YoutubeClip == null) return;
			var clip = new YoutubeClip { Id = YoutubeClip.Id };
			clip.Name = dueTitle;
			clip.Description = dueDescription;
			StaticItems.YoutubeProcessor.UpdateVideo(clip);
			Wrap.Store<YoutubeClip>(clip);
			this.NotifyByExpression(z => z.Status);
		}

		public void CmGo()
		{
			if (YoutubeClip == null) return;
			Process.Start(YoutubeClip.VideoURLFull);
		}


	}
}
